using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;
using StreetFighter.Characters;
using StreetFighter.Core;
using StreetFighter.Combat;

namespace StreetFighter.Combat
{
    /// <summary>
    /// Base AI for combat dummy/enemy. States: Idle, Aggro, Attack, Block.
    /// Target locking + combo AI.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(CombatSystemManager))]
    public class CombatAI : MonoBehaviour
    {
        [SerializeField] private Transform playerTarget; // Player lock
        [SerializeField] private float aggroRange = 10f;
        [SerializeField] private float combatSpacing = 3f;
        [SerializeField] private float circleRadius = 5f;
        [SerializeField] private EnemyAggroGroup aggroGroup;
        private enum State { Idle, Circling, Attack, Block, Dodge, React }
        private State currentState;
        private float threatLevel = 0f;
        private float turnCooldown = 0f;

        private CombatSystemManager combat;
        private NavMeshAgent agent;

        private void Awake()
        {
            combat = GetComponent<CombatSystemManager>();
            agent = GetComponent<NavMeshAgent>();
            if (aggroGroup == null)
                aggroGroup = FindObjectOfType<EnemyAggroGroup>() ?? gameObject.AddComponent<EnemyAggroGroup>();
            aggroGroup.RegisterMember(this);
            ServiceLocator.Resolve<IEventBus>()?.Subscribe<HitEvent>(OnHitReaction);
        }

        private void OnDestroy()
        {
            aggroGroup?.UnregisterMember(this);
        }

        private void Update()
        {
            if (playerTarget == null) return;

            float dist = Vector3.Distance(transform.position, playerTarget.position);
            if (dist > aggroRange) 
            {
                currentState = State.Idle;
                return;
            }

            // Update threat from recent damage
            threatLevel = Mathf.Max(threatLevel - Time.deltaTime * 0.5f, 0f);

            // Turn-taking
            turnCooldown -= Time.deltaTime;
            bool canAttack = turnCooldown <= 0 && aggroGroup?.CanAttack(this) == true;

            // Phase 2B.1: if our granted turn expired, notify group.
            if (turnCooldown <= 0f)
            {
                aggroGroup?.OnAttackComplete(this);
                // Prevent repeated completion spam.
                turnCooldown = Mathf.Min(turnCooldown, 0f);
            }


            switch (currentState)
            {
                case State.Idle:
                    if (dist < aggroRange) currentState = State.Circling;
                    break;
                case State.Circling:
                    Vector3 circlePos = playerTarget.position + Quaternion.Euler(0, threatLevel * 90f, 0) * Vector3.forward * circleRadius;
                    agent.SetDestination(circlePos);
                    if (Vector3.Distance(transform.position, circlePos) < combatSpacing && canAttack)
                        currentState = State.Attack;
                    break;
                case State.Attack:
                    // Execute move and schedule turn completion to prevent overlap spam.
                    var nextMove = threatLevel > 2 ? "punch_heavy" : "punch_light";
                    combat.ExecuteMove(nextMove);
                    turnCooldown = 2f; // fallback

                    // Improve: align group completion roughly with attack timing when available.
                    // (No new architecture; keeps current enemy loop.)
                    // Stabilization mode: avoid runtime Resources.Load in combat loops.
                    // Keep existing behavior via fallback timing.
                    // (If combat move data exposes attack timing later, wire it without changing architecture.)


                    currentState = State.Circling;
                    break;

                case State.Dodge:
                case State.React:
                    // Recovery
                    if (turnCooldown <= 0) currentState = State.Circling;
                    break;
            }
        }

        public void AddThreat(float amount)
        {
            threatLevel += amount;
        }

        private void OnHitReaction(HitEvent hit)
        {
            if (hit.Target != gameObject) return;
            AddThreat(hit.DamageAmount * 0.1f);
            if (Random.value < 0.3f)
            {
                currentState = State.Dodge;
                // Trigger dodge anim
            }
            else
            {
                currentState = State.React;
            }
            turnCooldown = 1.5f;
        }

        // Extend for dummy (passive), enemy (aggressive/parry)
    }
}
