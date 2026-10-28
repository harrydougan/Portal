using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PRTL
{
    public class FPJump : FPModule
    {
        [SerializeField] float jumpHeight = 1f;
        [SerializeField] float gravityScale = 3f;
        [SerializeField] float coyoteTime = 0.1f;

        [Header("Crouch Interaction")]
        [SerializeField] bool canJumpWhileCrouched = true;
        [SerializeField] float minJumpClearance = 0.3f;
        float lastGroundedTime = 0f;
        float standingHeight;

        #region Unity Methods
        private void Start()
        {
            Controller.JumpButtonPressed += OnJumpButtonPressed;
        }

        private void Update()
        {
            if (Controller.Grounded)
            {
                lastGroundedTime = Time.time;
            }
        }

        #endregion
        bool CanJump()
        {
            float airTime = Time.time - lastGroundedTime;

            if (airTime > coyoteTime) return false;

            // If the player is crouching, check crouch-jump rules and ceiling clearance
            if (Controller.IsCrouching)
            {
                if (!canJumpWhileCrouched) return false;
                
                float standUpClearance = (standingHeight - CharacterController.height) + minJumpClearance;
                if (Controller.DistanceToCeiling <= standUpClearance)
                {
                    return false;
                }
            }
            else
            {
                // When standing, check if there is immediate headroom above the player
                if (Controller.DistanceToCeiling <= minJumpClearance)
                {
                    return false;
                }
            }

            return true;
        }
        void OnJumpButtonPressed()
        {
            if (CanJump() && Controller.VerticalSpeed <= 0f)
            {
                // Only stand up if the player isn't holding crouch AND there is full jump headroom
                float fullStandAndJumpRoom = (standingHeight - CharacterController.height) + jumpHeight;
                if (Controller.IsCrouching && !Controller.CrouchInput && Controller.DistanceToCeiling > fullStandAndJumpRoom)
                {
                    Controller.IsCrouching = false;
                    Controller.Uncrouched?.Invoke();
                }

                Jump();
                Controller.Jumped.Invoke();
            }
        }

        void Jump()
        {
            //formula: V = SQUARERT(2 * G * JumpHeight) // Calculates how much vertical speed we will need for set jump height
            float g = Mathf.Abs(Physics.gravity.y) * gravityScale; // Mathsf.Abs to get absolute value
            
            float verticalSpeed = Mathf.Sqrt(2 * g * jumpHeight);

            Controller.SetVerticalSpeed?.Invoke(verticalSpeed);

        }
    }
}