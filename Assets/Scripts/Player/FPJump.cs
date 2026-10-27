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
        float lastGroundedTime = 0f;

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

            return airTime <= coyoteTime;
        }
        void OnJumpButtonPressed()
        {
            if (CanJump())
            {
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