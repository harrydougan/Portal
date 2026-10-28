
using System.Collections;
using UnityEngine;

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

        Vector3 horizontalVelocity = new();
        Vector3 verticalVelocity = new();
        bool HasMoveInput => Controller.MoveInput != Vector2.zero;

        #region Unity Methods
        private void Start()
        {
            Controller.SetVerticalSpeed += SetVerticalSpeed;
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

            CharacterController.Move(motion);

            Controller.VerticalSpeed = verticalVelocity.y;

        }
        #endregion

        void ApplyGravity(ref Vector3 motion) 
        {
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
    }
}