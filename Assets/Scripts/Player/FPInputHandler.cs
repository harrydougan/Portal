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
            Controller.MoveInput = input.Player.Move.ReadValue<Vector2>();
            Controller.LookInput = input.Player.Look.ReadValue<Vector2>();
        }
        #endregion
    }
}

