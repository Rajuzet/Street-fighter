export type PlayerId = string;
export type MatchId = string;
export type SessionId = string;

export type LobbyId = string;

export type DefenseIntentType = 'Block' | 'Dodge' | 'Parry';

export interface CombatAttackIntent {
  playerId: PlayerId;
  moveId: string;
  attackSeq: number;
  clientTime: number;
  comboCount: number;
}

export interface CombatDefenseIntent {
  playerId: PlayerId;
  intentType: DefenseIntentType;
  defenseSeq: number;
  clientTime: number;
}

export interface CombatHitConfirmed {
  matchId: MatchId;
  // Anti-double-hit: server ensures each unique tuple is applied once.
  attackerId: PlayerId;
  targetId: PlayerId;
  attackSeq: number;
  moveId: string;

  damage: number;
  isBlocked: boolean;
  hitPosition?: { x: number; y: number; z: number };
  hitNormal?: { x: number; y: number; z: number };
}

export interface PlayerStateSnapshot {
  playerId: PlayerId;
  health: number;
  stamina: number;
  comboCount: number;
  knockbackState: number;
}

export interface MatchState {
  matchId: MatchId;
  roundIndex: number;
  roundTimeRemaining: number;

  player1: PlayerStateSnapshot;
  player2: PlayerStateSnapshot;

  // server-side authoritative combat sequencing
  lastAttackSeqByPlayer: Record<PlayerId, number>;
  lastDefenseSeqByPlayer: Record<PlayerId, number>;

  // Anti-double-hit bookkeeping
  appliedHitKeys: Record<string, true>;
}

