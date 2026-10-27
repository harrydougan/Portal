using UnityEngine;

namespace PRTL
{
    public class FPController : MonoBehaviour
    {
        public CharacterController CharacterController;

        public bool Grounded => CharacterController.isGrounded;

        #region Input Handling
        public Vector2 MoveInput { get; set; }
        public Vector2 LookInput { get; set; }

        #endregion
    }
}

