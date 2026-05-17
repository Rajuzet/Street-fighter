using System.Collections;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Phase 2: ParkourSystem handles vault, ledge grab, and slide mechanics.
    /// Integrates with PlayerController and CharacterAnimationController.
    /// </summary>
    public sealed class ParkourSystem : MonoBehaviour
    {
        [Header("Vault Settings")]
        [SerializeField]
        private float vaultHeight = 1.2f;

        [SerializeField]
        private float vaultSpeed = 4f;

        [SerializeField]
        private LayerMask obstacleLayer;

        [Header("Ledge Grab Settings")]
        [SerializeField]
        private float ledgeGrabHeight = 2f;

        [SerializeField]
        private float ledgeGrabSpeed = 3f;

        [SerializeField]
        private float ledgeHangDuration = 10f;

        [Header("Slide Settings")]
        [SerializeField]
        private float slideDuration = 0.8f;

        [SerializeField]
        private float slideSpeedMultiplier = 1.5f;

        [SerializeField]
        private float slideHeight = 0.5f;

        [Header("Detection")]
        [SerializeField]
        private float detectionDistance = 1.5f;

        [SerializeField]
        private float groundCheckDistance = 0.3f;

        private CharacterController controller;
        private CharacterAnimationController animationController;
        private bool isParkouring;
        private bool isHanging;
        private float originalHeight;
        private Vector3 originalCenter;
        private Coroutine currentParkourAction;

        public bool IsParkouring => isParkouring;
        public bool IsHanging => isHanging;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            animationController = GetComponent<CharacterAnimationController>();
            
            if (controller != null)
            {
                originalHeight = controller.height;
                originalCenter = controller.center;
            }
        }

        private void Update()
        {
            if (isParkouring || isHanging)
                return;

            DetectVault();
            DetectLedgeGrab();
            DetectSlide();
        }

        /// <summary>
        /// Attempts to vault over an obstacle.
        /// </summary>
        private void DetectVault()
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, detectionDistance, obstacleLayer))
            {
                float obstacleHeight = hit.collider.bounds.max.y - transform.position.y;
                if (obstacleHeight > 0.5f && obstacleHeight <= vaultHeight)
                {
                    Vector3 topOrigin = hit.point + Vector3.up * vaultHeight;
                    if (!Physics.Raycast(topOrigin, transform.forward, detectionDistance * 0.5f, obstacleLayer))
                    {
                        currentParkourAction = StartCoroutine(VaultAction(hit.point, hit.normal));
                    }
                }
            }
        }

        private IEnumerator VaultAction(Vector3 hitPoint, Vector3 hitNormal)
        {
            isParkouring = true;
            animationController?.SetVaulting(true);

            Vector3 vaultStart = transform.position;
            Vector3 vaultEnd = hitPoint + hitNormal * 0.5f + Vector3.up * vaultHeight;

            float timer = 0f;
            float duration = Vector3.Distance(vaultStart, vaultEnd) / vaultSpeed;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                Vector3 currentPos = Vector3.Lerp(vaultStart, vaultEnd, t);
                currentPos.y += Mathf.Sin(t * Mathf.PI) * 0.5f;
                
                controller?.Move(currentPos - transform.position);
                yield return null;
            }

            animationController?.SetVaulting(false);
            isParkouring = false;
            currentParkourAction = null;
        }

        private void DetectLedgeGrab()
        {
            Vector3 origin = transform.position + Vector3.up * controller.height * 0.8f;
            if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, detectionDistance, obstacleLayer))
            {
                float ledgeHeight = hit.point.y - transform.position.y;
                if (ledgeHeight > vaultHeight && ledgeHeight <= ledgeGrabHeight)
                {
                    if (!Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.5f))
                    {
                        currentParkourAction = StartCoroutine(LedgeGrabAction(hit.point));
                    }
                }
            }
        }

        private IEnumerator LedgeGrabAction(Vector3 ledgePoint)
        {
            isParkouring = true;
            isHanging = true;
            animationController?.SetLedgeGrabbing(true);

            Vector3 hangPosition = ledgePoint - transform.forward * 0.3f;
            hangPosition.y -= controller.height * 0.4f;

            float timer = 0f;
            float pullUpDuration = 0.5f;

            while (timer < pullUpDuration)
            {
                timer += Time.deltaTime;
                float t = timer / pullUpDuration;
                controller?.Move(Vector3.Lerp(transform.position, hangPosition, t) - transform.position);
                yield return null;
            }

            float hangTimer = 0f;
            while (hangTimer < ledgeHangDuration)
            {
                hangTimer += Time.deltaTime;
                
                if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
                {
                    Vector3 pullUpEnd = ledgePoint + transform.forward * 0.5f + Vector3.up * controller.height * 0.5f;
                    yield return StartCoroutine(PullUpAction(pullUpEnd));
                    break;
                }
                
                yield return null;
            }

            animationController?.SetLedgeGrabbing(false);
            isHanging = false;
            isParkouring = false;
            currentParkourAction = null;
        }

        private IEnumerator PullUpAction(Vector3 endPosition)
        {
            float timer = 0f;
            float duration = 0.4f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                controller?.Move(Vector3.Lerp(transform.position, endPosition, t) - transform.position);
                yield return null;
            }
        }

        private void DetectSlide()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.C) || UnityEngine.Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (!isParkouring)
                {
                    currentParkourAction = StartCoroutine(SlideAction());
                }
            }
        }

        private IEnumerator SlideAction()
        {
            isParkouring = true;
            animationController?.SetSliding(true);

            if (controller != null)
            {
                controller.height = slideHeight;
                controller.center = new Vector3(originalCenter.x, slideHeight / 2f, originalCenter.z);
            }

            Vector3 slideDirection = transform.forward;
            float timer = 0f;

            while (timer < slideDuration)
            {
                timer += Time.deltaTime;
                float speed = slideSpeedMultiplier * (1f - timer / slideDuration);
                controller?.Move(slideDirection * speed * Time.deltaTime);
                yield return null;
            }

            if (controller != null)
            {
                controller.height = originalHeight;
                controller.center = originalCenter;
            }

            animationController?.SetSliding(false);
            isParkouring = false;
            currentParkourAction = null;
        }

        /// <summary>
        /// Cancels any active parkour action.
        /// </summary>
        public void CancelParkour()
        {
            if (currentParkourAction != null)
            {
                StopCoroutine(currentParkourAction);
                currentParkourAction = null;
            }

            if (controller != null)
            {
                controller.height = originalHeight;
                controller.center = originalCenter;
            }

            animationController?.SetVaulting(false);
            animationController?.SetLedgeGrabbing(false);
            animationController?.SetSliding(false);
            isParkouring = false;
            isHanging = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * detectionDistance);
            Gizmos.DrawRay(transform.position + Vector3.up * controller?.height * 0.8f ?? 1.6f, transform.forward * detectionDistance);
        }
    }
}
