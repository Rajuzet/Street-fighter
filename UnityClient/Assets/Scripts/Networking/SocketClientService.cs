using System;
using UnityEngine;
using StreetFighter.Core;

// NOTE: This project currently has no Unity networking framework.
// Phase 3 multiplayer foundation uses backend socket.io messages.
// 
// Implementation uses a lightweight socket client interface.
// In production you must ensure a socket.io-compatible Unity package exists.
// For codebase integrity we isolate networking behind this service.

namespace StreetFighter.Networking
{
    public sealed class SocketClientService : MonoBehaviour
    {
        [Header("Backend")]
        [SerializeField] private string serverUrl = "http://localhost:4000";

        public event Action<string, string> OnLobbyJoined; // lobbyId, sessionId
        public event Action<MatchUpdatePayload> OnMatchUpdate;
        public event Action<string> OnServerError;

        private bool isConnected;
        private string currentSessionId;

        private ISocketClient client;

        private void Awake()
        {
            // Resolve optional socket client implementation.
            client = ServiceLocator.Resolve<ISocketClient>();
        }

        public void Connect(string sessionId)
        {
            currentSessionId = sessionId;

            if (client == null)
            {
                Debug.LogError("SocketClientService: No ISocketClient registered in ServiceLocator.");
                OnServerError?.Invoke("socket_client_missing");
                return;
            }

            if (isConnected) return;

            client.Connect(serverUrl);
            client.On("lobby:joined", OnLobbyJoinedInternal);
            client.On("match:update", OnMatchUpdateInternal);
            client.On("error", OnErrorInternal);

            // Lightweight convention: lobby join is issued by caller.
            isConnected = true;
        }

        public void RequestLobbyJoin(string lobbyId)
        {
            if (client == null || !isConnected) return;

            client.Emit("lobby:join", new
            {
                lobbyId,
                sessionId = currentSessionId
            });
        }

        public void SendCombatIntent(string matchId, CombatAttackIntent intent)
        {
            if (client == null || !isConnected) return;

            client.Emit("combat:attack_intent", new
            {
                matchId,
                intent
            });
        }

        public void SendCombatIntent(string matchId, CombatDefenseIntent intent)
        {
            if (client == null || !isConnected) return;

            client.Emit("combat:defense_intent", new
            {
                matchId,
                intent
            });
        }

        private void OnLobbyJoinedInternal(object payload)
        {
            // Expected payload: { lobbyId, sessionId }
            try
            {
                // Avoid hard dependency on JSON libraries: use dynamic-ish extraction.
                var dict = payload as System.Collections.IDictionary;
                if (dict == null) return;

                var lobbyId = dict["lobbyId"]?.ToString();
                var sessionId = dict["sessionId"]?.ToString();
                if (string.IsNullOrEmpty(lobbyId) || string.IsNullOrEmpty(sessionId)) return;

                OnLobbyJoined?.Invoke(lobbyId, sessionId);
            }
            catch (Exception e)
            {
                Debug.LogError($"OnLobbyJoinedInternal error: {e}");
            }
        }

        private void OnMatchUpdateInternal(object payload)
        {
            try
            {
                // Expected payload: { matchId, state }
                var dict = payload as System.Collections.IDictionary;
                if (dict == null) return;

                var matchId = dict["matchId"]?.ToString();
                var stateObj = dict["state"];

                var parsedState = MatchStatePayload.Parse(stateObj);
                if (parsedState == null) return;

                OnMatchUpdate?.Invoke(new MatchUpdatePayload(matchId, parsedState));
            }
            catch (Exception e)
            {
                Debug.LogError($"OnMatchUpdateInternal error: {e}");
            }
        }

        private void OnErrorInternal(object payload)
        {
            try
            {
                var dict = payload as System.Collections.IDictionary;
                if (dict == null) return;
                var code = dict["code"]?.ToString();
                OnServerError?.Invoke(code ?? "server_error");
            }
            catch (Exception e)
            {
                Debug.LogError($"OnErrorInternal error: {e}");
            }
        }
    }

    // ---- Payload models (no placeholders; used for compile-time safety) ----

    [Serializable]
    public sealed class MatchUpdatePayload
    {
        public string MatchId;
        public MatchStatePayload State;

        public MatchUpdatePayload(string matchId, MatchStatePayload state)
        {
            MatchId = matchId;
            State = state;
        }
    }

    [Serializable]
    public sealed class MatchStatePayload
    {
        // Round + combat state needed for Phase 3 foundation.
        public int RoundIndex;
        public float RoundTimeRemaining;

        public PlayerStatePayload Player1;
        public PlayerStatePayload Player2;

        public static MatchStatePayload Parse(object stateObj)
        {
            // Minimal, safe parsing contract.
            // If the backend shape differs, this returns null (and the client will ignore the update).
            try
            {
                var dict = stateObj as System.Collections.IDictionary;
                if (dict == null) return null;

                var result = new MatchStatePayload
                {
                    RoundIndex = dict["roundIndex"] != null ? Convert.ToInt32(dict["roundIndex"]) : 0,
                    RoundTimeRemaining = dict["roundTimeRemaining"] != null ? Convert.ToSingle(dict["roundTimeRemaining"]) : 0f
                };

                result.Player1 = PlayerStatePayload.Parse(dict["player1"]);
                result.Player2 = PlayerStatePayload.Parse(dict["player2"]);

                return result;
            }
            catch
            {
                return null;
            }
        }
    }

    [Serializable]
    public sealed class PlayerStatePayload
    {
        public string PlayerId;
        public float Health;
        public float Stamina;
        public int ComboCount;
        public int KnockbackState;

        public static PlayerStatePayload Parse(object playerObj)
        {
            try
            {
                var dict = playerObj as System.Collections.IDictionary;
                if (dict == null) return null;

                return new PlayerStatePayload
                {
                    PlayerId = dict["playerId"]?.ToString(),
                    Health = dict["health"] != null ? Convert.ToSingle(dict["health"]) : 0f,
                    Stamina = dict["stamina"] != null ? Convert.ToSingle(dict["stamina"]) : 0f,
                    ComboCount = dict["comboCount"] != null ? Convert.ToInt32(dict["comboCount"]) : 0,
                    KnockbackState = dict["knockbackState"] != null ? Convert.ToInt32(dict["knockbackState"]) : 0
                };
            }
            catch
            {
                return null;
            }
        }
    }

    [Serializable]
    public enum DefenseIntentType
    {
        Block = 0,
        Dodge = 1,
        Parry = 2
    }

    [Serializable]
    public sealed class CombatAttackIntent
    {
        public string PlayerId;
        public string MoveId;
        public int AttackSeq;
        public float ClientTime;
        public int ComboCount;

        public CombatAttackIntent(string playerId, string moveId, int attackSeq, float clientTime, int comboCount)
        {
            PlayerId = playerId;
            MoveId = moveId;
            AttackSeq = attackSeq;
            ClientTime = clientTime;
            ComboCount = comboCount;
        }
    }

    [Serializable]
    public sealed class CombatDefenseIntent
    {
        public string PlayerId;
        public DefenseIntentType IntentType;
        public int DefenseSeq;
        public float ClientTime;

        public CombatDefenseIntent(string playerId, DefenseIntentType intentType, int defenseSeq, float clientTime)
        {
            PlayerId = playerId;
            IntentType = intentType;
            DefenseSeq = defenseSeq;
            ClientTime = clientTime;
        }
    }

    // Socket abstraction so we can compile even if socket package is swapped.
    public interface ISocketClient
    {
        void Connect(string serverUrl);
        void Emit(string eventName, object payload);
        void On(string eventName, Action<object> handler);
    }
}

