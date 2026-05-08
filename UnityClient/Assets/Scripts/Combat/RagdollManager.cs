using UnityEngine;

namespace StreetFighter.Combat
{
    public class RagdollManager : MonoBehaviour
    {
        [SerializeField] private Rigidbody[] ragdollRBs;
        [SerializeField] private Collider[] ragdollColliders;
        private Animator animator;
        private Rigidbody mainRB;

        public void EnableRagdoll()
        {
            animator.enabled = false;
            mainRB.isKinematic = true;
            foreach (var rb in ragdollRBs) rb.isKinematic = false;
        }

        public void DisableRagdoll()
        {
            animator.enabled = true;
            mainRB.isKinematic = false;
            foreach (var rb in ragdollRBs) rb.isKinematic = true;
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            mainRB = GetComponent<Rigidbody>();
        }
    }
}
