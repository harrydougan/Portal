using UnityEngine;

namespace PRTL
{
    public class FPInputHandler : FPModule
    {
        GameInput input;

        #region Unity Methods
        private void OnEnable()
        {
           if (input == null)
            {
                input = new GameInput();
            }
            input.Enable();
        }

        private void OnDisable()
        {
            input.Disable();
        }

        private void Update()
        {
            if (Controller.InputEnabled)
            {
                HandleMovementInput();
            }

        }

        void HandleMovementInput()
        {
            Controller.MoveInput = input.Player.Move.ReadValue<Vector2>();
            Controller.LookInput = input.Player.Look.ReadValue<Vector2>();
            Controller.CrouchInput = input.Player.Crouch.IsPressed();

            if (input.Player.Jump.WasPressedThisFrame())
            {
                Controller.JumpButtonPressed?.Invoke();
            }

            if (input.Player.Crouch.WasPressedThisFrame())
            {
                Controller.CrouchButtonPressed?.Invoke();
            }

            if (input.Player.Crouch.WasReleasedThisFrame())
            {
                Controller.CrouchButtonReleased?.Invoke();
            }
        }
        #endregion
    }
}

