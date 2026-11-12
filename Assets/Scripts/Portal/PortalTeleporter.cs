using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using static UnityEngine.GraphicsBuffer;

namespace PRTL
{
    public class PortalTeleporter : MonoBehaviour
    {
        [Tooltip("The destination portal exit point")]
        public Transform reciever;
        public Transform traveller;
        public Transform player;
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

                

                //Quaternion deltaRotation = Quaternion.FromToRotation(transform.up, reciever.transform.up);

                
                //targetTransform.forward = -targetTransform.forward;
                //targetTransform.forward = deltaRotation * targetTransform.forward;

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
            Quaternion deltaRotation = Quaternion.FromToRotation(transform.up, reciever.transform.up);

            // 1. Reposition whichever object actually crossed the threshold
            Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToTarget;
            target.position = reciever.position + positionOffset;

            // 2. ONLY rotate the player and adjust FPMovement if the Player is the target
            if (target.CompareTag("Player"))
            {
                target.forward = -target.forward;
                target.forward = deltaRotation * target.forward;

                FPMovement fpMovement = target.GetComponent<FPMovement>();
                if (fpMovement != null)
                {
                    fpMovement.VerticalVelocity += new Vector3(0, 4f, 0f);

                    Vector3 totalVelocity = fpMovement.HorizontalVelocity + fpMovement.VerticalVelocity;
                    totalVelocity = -totalVelocity;
                    totalVelocity = deltaRotation * totalVelocity;

                    fpMovement.HorizontalVelocity = new Vector3(totalVelocity.x, 0f, totalVelocity.z);
                    fpMovement.VerticalVelocity = new Vector3(0f, totalVelocity.y - 4f, 0f);
                }
            }
            // 3. Handle thrown objects / travellers separately
            else if (target.CompareTag("Traveller"))
            {
                // Rotate the thrown object itself (if desired)
                target.forward = -target.forward;
                target.forward = deltaRotation * target.forward;

                Rigidbody rb = target.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = -rb.linearVelocity;
                    rb.linearVelocity = deltaRotation * rb.linearVelocity;
                    rb.angularVelocity = deltaRotation * rb.angularVelocity;
                }
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