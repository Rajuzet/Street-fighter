using UnityEngine;

namespace StreetFighter.Combat
{
    /// <summary>
    /// Broadcast when an attack hits a target.
    /// </summary>
    public sealed class HitEvent
    {
        public AttackDefinition Attack { get; }
        public GameObject Attacker { get; }
        public GameObject Target { get; }
        public Vector3 HitPosition { get; }
        public Vector3 HitNormal { get; }
        public float DamageAmount { get; }
        public bool IsBlocked { get; }

        public HitEvent(AttackDefinition attack, GameObject attacker, GameObject target, 
            Vector3 hitPosition, Vector3 hitNormal, float damage, bool isBlocked)
        {
            Attack = attack;
            Attacker = attacker;
            Target = target;
            HitPosition = hitPosition;
            HitNormal = hitNormal;
            DamageAmount = damage;
            IsBlocked = isBlocked;
        }
    }

    /// <summary>
    /// Raised when an attack enters its active frames.
    /// </summary>
    public sealed class AttackActivatedEvent
    {
        public AttackDefinition Attack { get; }
        public GameObject Attacker { get; }

        public AttackActivatedEvent(AttackDefinition attack, GameObject attacker)
        {
            Attack = attack;
            Attacker = attacker;
        }
    }

    /// <summary>
    /// Raised when a combo is broken or reset.
    /// </summary>
    public sealed class ComboBrokenEvent
    {
        public int ComboCount { get; }

        public ComboBrokenEvent(int comboCount)
        {
            ComboCount = comboCount;
        }
    }

    /// <summary>
    /// Raised when a new combo is started.
    /// </summary>
    public sealed class ComboStartedEvent
    {
        public AttackDefinition FirstAttack { get; }

        public ComboStartedEvent(AttackDefinition firstAttack)
        {
            FirstAttack = firstAttack;
        }
    }

    /// <summary>
    /// Raised during a successful combo continuation.
    /// </summary>
    public sealed class ComboContinuedEvent
    {
        public int ComboCount { get; }
        public AttackDefinition CurrentAttack { get; }

        public ComboContinuedEvent(int comboCount, AttackDefinition currentAttack)
        {
            ComboCount = comboCount;
            CurrentAttack = currentAttack;
        }
    }

    public sealed class ParryEvent
    {
        public AttackDefinition BlockedAttack { get; }
        public GameObject Parrier { get; }

        public ParryEvent(AttackDefinition attack, GameObject parrier)
        {
            BlockedAttack = attack;
            Parrier = parrier;
        }
    }

    public sealed class DodgeEvent
    {
        public GameObject Dodger { get; }

        public DodgeEvent(GameObject dodger)
        {
            Dodger = dodger;
        }
    }
}
