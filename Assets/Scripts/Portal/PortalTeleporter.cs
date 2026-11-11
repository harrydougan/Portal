using System.Collections.Generic;
using UnityEngine;

namespace PRTL
{
    public class PortalTeleporter : MonoBehaviour
    {
        [Tooltip("The destination portal exit point")]
        public Transform reciever;

        // Track all overlapping objects currently inside this portal trigger
        private readonly HashSet<Collider> overlappingColliders = new HashSet<Collider>();

        void Update()
        {
            if (overlappingColliders.Count == 0 || reciever == null)
                return;

            // Copy to a temporary list to allow safe removal while iterating
            var collidersToCheck = new List<Collider>(overlappingColliders);

            foreach (var col in collidersToCheck)
            {
                if (col == null)
                {
                    overlappingColliders.Remove(col);
                    continue;
                }

                Transform targetTransform = col.transform;
                Vector3 portalToTarget = targetTransform.position - transform.position;
                float dotProduct = Vector3.Dot(transform.up, portalToTarget);

                // Target crossed the portal threshold plane
                if (dotProduct < 0f)
                {
                    TeleportObject(targetTransform, portalToTarget);
                    overlappingColliders.Remove(col);
                }
            }
        }

        private void TeleportObject(Transform target, Vector3 portalToTarget)
        {
            // Calculate rotation differential between entrance and exit portal
            float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
            rotationDiff += 180f;

            // Apply rotation and position offset
            target.Rotate(Vector3.up, rotationDiff);
            Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToTarget;
            target.position = reciever.position + positionOffset;

            // Preserve and redirect physical momentum if a Rigidbody is attached
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Quaternion deltaRotation = Quaternion.Euler(0f, rotationDiff, 0f);
                rb.linearVelocity = deltaRotation * rb.linearVelocity;
                rb.angularVelocity = deltaRotation * rb.angularVelocity;
            }

            // Inform Unity's physics engine of manual position/rotation changes
            Physics.SyncTransforms();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Traveller"))
            {
                overlappingColliders.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Traveller"))
            {
                overlappingColliders.Remove(other);
            }
        }
    }
}