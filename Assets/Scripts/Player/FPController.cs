using UnityEngine;
using UnityEngine.Events;

namespace PRTL
{
    public class FPController : MonoBehaviour
    {
        public CharacterController CharacterController;

        public float DistanceToGround = 0f;

        public bool Grounded => DistanceToGround <= 0.2f;

        #region Input Handling
        public Vector2 MoveInput { get; set; }
        public Vector2 LookInput { get; set; }

        public UnityAction JumpButtonPressed;

        public UnityAction CrouchButtonPressed;
        #endregion

        public float VerticalSpeed = 0f;

        public UnityAction<float> SetVerticalSpeed;

        public UnityEvent Jumped;

        public UnityEvent<float> Landed;

        public UnityEvent Crouched;

        #region Unity Methods
        private void Awake()
        {
            Jumped.AddListener(() => Debug.Log("Player Jumped"));

            Landed.AddListener((vs) => Debug.Log($"Player Landed {vs} "));

            Crouched.AddListener(() => Debug.Log("Player Crouched"));
        }
        #endregion
    }
}

