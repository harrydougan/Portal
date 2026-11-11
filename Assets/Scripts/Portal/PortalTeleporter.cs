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

                

                Quaternion deltaRotation = Quaternion.FromToRotation(transform.up, reciever.transform.up);

                
                targetTransform.forward = -targetTransform.forward;
                targetTransform.forward = deltaRotation * targetTransform.forward;

                // Target crossed the portal threshold plane
                if (dotProduct < 0f)
                {
                    TeleportObject(targetTransform, portalToTarget);
                    overlappingColliders.Remove(col);
                }

                Vector3 verticalVel = col.GetComponent<>();

            }
        }


        //Object
        private void TeleportObject(Transform target, Vector3 portalToTarget)
        {
            // Calculate rotation differential between entrance and exit portal
            float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
            //rotationDiff += 180f;
            Quaternion deltaRotation = Quaternion.FromToRotation(transform.up, reciever.transform.up);

            //Debug.Log(rotationDiff);
            //Debug.Log(transform.forward);
            Debug.Log(transform.up); // Normal to the portal
            //Debug.Log(reciever.transform.forward);
            Debug.Log(reciever.transform.up);

            //Apply rotation and position offset
            target.Rotate(Vector3.up, rotationDiff);

            target.forward = -target.forward;
            target.forward = deltaRotation * target.forward;

            Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToTarget;
            
            target.position = reciever.position + positionOffset;

            // Preserve and redirect physical momentum if a Rigidbody is attached
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = -rb.linearVelocity;

                rb.linearVelocity = deltaRotation * rb.linearVelocity;
                rb.angularVelocity = deltaRotation * rb.angularVelocity;

                Debug.Log(deltaRotation);
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