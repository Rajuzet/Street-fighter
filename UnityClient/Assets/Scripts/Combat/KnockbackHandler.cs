using UnityEngine;
using StreetFighter.Data;
using StreetFighter.Combat;

namespace StreetFighter.Combat
{
    /// <summary>
    /// Applies physics knockback to hit targets.
    /// </summary>
    public class KnockbackHandler : MonoBehaviour
    {
        private Rigidbody rb;

        public static void ApplyKnockback(GameObject target, AttackDefinition attack, Vector3 hitDirection)
        {
            var handler = target.GetComponent<KnockbackHandler>();
            if (handler == null) handler = target.AddComponent<KnockbackHandler>();

            handler.ExecuteKnockback(attack.KnockbackForce * hitDirection, attack.KnockbackDuration);
        }

        private void ExecuteKnockback(float force, float duration)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.AddForce(Vector3.up * 2f + force * transform.forward, ForceMode.Impulse);

            StartCoroutine(EndKnockback(duration));
        }

        private System.Collections.IEnumerator EndKnockback(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (rb != null)
            {
                Destroy(rb);
            }
            Destroy(this);
        }
    }
}
