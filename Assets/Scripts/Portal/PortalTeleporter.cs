using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
            //Define usefull quanties relating the two portals, these should really be above but I can't figure out how to make
            //Them accessible!
            Quaternion halfTurn = Quaternion.Euler(0, 0, 180);
            Quaternion relRotation = reciever.rotation * halfTurn * Quaternion.Inverse(transform.rotation);

            //Translate the object to the new portal using a local direction vector measured from the centre of poral to object

            Vector3 portalToObj = target.position - transform.position;
            portalToObj = relRotation * portalToObj;
            target.position = reciever.position + portalToObj;


            //Players and objects get treated differently; Player will always be sent upright and their movement is not RB, 
            //Objects will keep their orientation and use rb.linear and angular velocity
            // 2. ONLY rotate the player and adjust FPMovement if the Player is the target
            if (target.CompareTag("Player"))
            {

                //Rotate the player to come out the second portal in the same orientation plus a 180 half turn around Y-axis
                //Quaternion multiplication goes right to left, inverse the first portals orientation to zero -> Perform mirror flip
                //Change to reciever portal Orientation

                target.rotation = relRotation * player.rotation;

                //This previous orientation might mean the player is not standing upright
                //This defines the final orientation we want which is upright and then passes it to the coroutine if needs be
                Quaternion sendUpright = Quaternion.FromToRotation(target.up, new Vector3(0, 1, 0));
                Quaternion upright = sendUpright * target.rotation;
                float uprightTime = 0.7f;

                //First check are we upright, if not DO it.
                //Need to check because you can't move the camera while uprighting so for flat portals it is unresponsive
                if (Vector3.Dot(target.up, new Vector3(0, 1, 0)) != 1)
                {
                    StartCoroutine(UprightRoutine(target.rotation, upright, target, uprightTime));
                }

                FPMovement fpMovement = target.GetComponent<FPMovement>();
                if (fpMovement != null)
                {
                    // Create new vector with both horizontal and vertical then apply transformations to that, much cleaner
                    Vector3 localHorizontalVelocity = fpMovement.HorizontalVelocity;
                    Vector3 localVerticalVelocity = fpMovement.VerticalVelocity;

                    Vector3 totalVelocity = localHorizontalVelocity + localVerticalVelocity;
                    totalVelocity = relRotation * totalVelocity;

                    //Cast back to silly format
                    fpMovement.HorizontalVelocity = new Vector3(totalVelocity.x, 0f, totalVelocity.z);
                    fpMovement.VerticalVelocity = new Vector3(0f, totalVelocity.y, 0f);
                }
            }


            // 3. Handle thrown objects / travellers separately
            else if (target.CompareTag("Traveller"))
            {


                // Rotate the thrown object itself (if desired)
                target.rotation = relRotation * target.rotation;

                Rigidbody rb = target.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    //Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
                    //localVelocity = Quaternion.Euler(0, 0, 180) * localVelocity;
                    //rb.linearVelocity = reciever.TransformDirection(localVelocity);
                    rb.linearVelocity = relRotation * rb.linearVelocity;

                    //Going to Local coordinates applying the transform and then inverting back to world works just 
                    //As well as using relative Rotation transforms, dealers choice really
                    //The transform method is more general IMO because the former falls apart when the portals local coords are weird
                    //Below is using local everything else is transforms
                    Vector3 localAngular = transform.InverseTransformDirection(rb.angularVelocity);
                    localAngular = Quaternion.Euler(0, 0, 180) * localAngular;
                    rb.angularVelocity = reciever.TransformDirection(localAngular);
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

        private IEnumerator UprightRoutine(Quaternion inital, Quaternion final, Transform playerTransform, float duration)
        {
            if (duration > 0f)
            {
                float startTime = Time.time;
                float endTime = startTime + duration;
                yield return null;
                while (Time.time < endTime)
                {
                    float progress = (Time.time - startTime) / duration;
                    // progress will equal 0 at startTime, 1 at endTime.
                    playerTransform.rotation = Quaternion.Slerp(inital, final, progress);
                    yield return null;
                }
            }
            playerTransform.rotation = final;
        }
    }
}