
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace PRTL
{
    public class FPMovement : FPModule
    {
        [Header("Movement Parameters")]
        [SerializeField] float maxSpeed = 12f;
        [SerializeField] float crouchSpeedMultiplier = 0.5f;

        [Space(20f)]
        [SerializeField] float acceleration = 80f;
        [SerializeField] float deceleration = 12f;

        [Header("Physics Parameters")]
        [SerializeField] float gravityScale = 2f;

        [Header("Push Settings")]
        [SerializeField] float pushPower = 2.0f;
        [SerializeField] float weightLimit = 50.0f;
        Vector3 horizontalVelocity = new();
        Vector3 verticalVelocity = new();
        bool HasMoveInput => Controller.MoveInput != Vector2.zero;

        #region Unity Methods
        private void Start()
        {
            Controller.SetVerticalSpeed += SetVerticalSpeed;
            Controller.SetHorizontalVelocity += SetHorizontalVelocity;
        }
        private void Update()
        {

            if (HasMoveInput) //Acceleration
            {
                var targetVelocity = Controller.MoveInput.x * Root.right + Controller.MoveInput.y * Root.forward;

                float currentMaxSpeed = Controller.IsCrouching ? maxSpeed * crouchSpeedMultiplier : maxSpeed;
                targetVelocity *= currentMaxSpeed;

                float maxDelta = acceleration * Time.deltaTime;

                horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, maxDelta); 
            }
            else //Deceleration
            {
                float maxDelta = deceleration * Time.deltaTime;

                horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, maxDelta);
            }

            Vector3 motion = horizontalVelocity * Time.deltaTime;

            ApplyGravity(ref motion);

            if (CharacterController.enabled)
            {
                CharacterController.Move(motion);
            }

            Controller.VerticalSpeed = verticalVelocity.y;

        }
        #endregion

        void ApplyGravity(ref Vector3 motion) 
        {
            // If player hits head on a ceiling while moving upward, stop upward momentum.
            if (Controller.DistanceToCeiling <= 0.1f && verticalVelocity.y > 0f)
            {
                verticalVelocity.y = 0f;
            }

            if (!Controller.Grounded) // Grounded checks to see if player is on the ground to prevent infinite verticalVelocity scaling.
            {
                verticalVelocity += Physics.gravity * gravityScale * Time.deltaTime;
            }
            else if (verticalVelocity.y <=0f) // Lets us jump whilst grounded
            {
                verticalVelocity = Vector3.down * 4f; // Stick to ground
            }

            motion += verticalVelocity * Time.deltaTime;
            
        }

        void SetVerticalSpeed(float verticalSpeed)
        {
            verticalVelocity.y = verticalSpeed;
        }

        void SetHorizontalVelocity(Vector3 horizontalVelocity)
        {
            this.horizontalVelocity = horizontalVelocity;
        }

        //For colliding with pushable objects
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody targetRb = hit.collider.attachedRigidbody;

            // Skip if no Rigidbody, or if it is kinematic
            if (targetRb == null || targetRb.isKinematic)
                return;

            // Don't push objects we are standing on (normal pointing mostly upward)
            if (hit.moveDirection.y < -0.3f)
                return;

            // Optional: Skip objects heavier than the player's push limit
            if (targetRb.mass > weightLimit)
                return;

            // Calculate push direction purely along horizontal axes
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            // Apply impulse at contact point or to center of mass
            targetRb.AddForce(pushDir * pushPower, ForceMode.Impulse);
        }
    }
}