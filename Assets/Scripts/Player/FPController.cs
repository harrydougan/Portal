using UnityEngine;
using UnityEngine.Events;

namespace PRTL
{
    public class FPController : MonoBehaviour
    {
        public CharacterController CharacterController;

        public float DistanceToGround = 0f;
        public float DistanceToCeiling = 100f;
        public bool Grounded => DistanceToGround <= 0.2f;
        public bool IsCrouching { get; set; }
        public System.Func<bool> CanStandUp;

        #region Input Handling
        public Vector2 MoveInput { get; set; }
        public Vector2 LookInput { get; set; }
        public bool CrouchInput { get; set; }


        public UnityAction JumpButtonPressed;
        public UnityAction CrouchButtonPressed;
        public UnityAction CrouchButtonReleased;
        #endregion

        public float VerticalSpeed = 0f;

        public UnityAction<float> SetVerticalSpeed;
        public UnityEvent Jumped;
        public UnityEvent<float> Landed;
        public UnityEvent Crouched;
        public UnityEvent Uncrouched;

        #region Unity Methods
        private void Awake()
        {
            Jumped.AddListener(() => Debug.Log("Player Jumped"));

            Landed.AddListener((vs) => Debug.Log($"Player Landed {vs} "));

            Crouched.AddListener(() => Debug.Log("Player Crouched"));

            Uncrouched.AddListener(() => Debug.Log("Player Uncrouched"));
        }
        #endregion
    }
}

