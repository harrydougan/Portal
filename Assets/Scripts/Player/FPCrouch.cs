using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;

namespace PRTL
{
    public class FPCrouch : FPModule
    {

        [Header("Crouch Settings")]
        [SerializeField] float crouchHeight = 0.85f;
        [SerializeField] float transitionSpeed = 10f;
        [SerializeField] LayerMask ceilingLayerMask = Physics.DefaultRaycastLayers;

        [Header("Camera Adjustment")]
        [SerializeField] Transform cameraHolder;

        float standingHeight;
        Vector3 standingCenter;
        Vector3 crouchCenter;
        float standingCameraY;
        float crouchCameraY;

        #region Unity Methods
        private void Start()
        {
            Controller.CanStandUp = CanStandUp;

            standingHeight = CharacterController.height;
            standingCenter = CharacterController.center;

            float heightDifference = standingHeight - crouchHeight;
            crouchCenter = standingCenter - new Vector3(0f, heightDifference / 2f, 0f);

            if (cameraHolder != null)
            {
                standingCameraY = cameraHolder.localPosition.y;
                crouchCameraY = standingCameraY - (heightDifference * 0.5f);
            }
        }

        private void Update()
        {
            // Determines if player should be crouching.
            if (Controller.CrouchInput)
            {
                if (!Controller.IsCrouching)
                {
                    Controller.IsCrouching = true;
                    Controller.Crouched?.Invoke();
                }
            }
            else if (Controller.IsCrouching) // Only allows player to stand if no obstacles directly above.
            {
                if (CanStandUp())
                {
                    Controller.IsCrouching = false;
                    Controller.Uncrouched?.Invoke();
                }
            }

            //Smooth transition between height and center.
            float targetHeight = Controller.IsCrouching ? crouchHeight : standingHeight;
            Vector3 targetCenter = Controller.IsCrouching ? crouchCenter : standingCenter;

            CharacterController.height = Mathf.Lerp(CharacterController.height, targetHeight, Time.deltaTime * transitionSpeed);
            CharacterController.center = Vector3.Lerp(CharacterController.center, targetCenter, Time.deltaTime * transitionSpeed);

            //Smothly lower/raise the player camera.
            if (cameraHolder != null)
            {
                float targetCamY = Controller.IsCrouching ? crouchCameraY : standingCameraY;
                Vector3 camPos = cameraHolder.localPosition;
                camPos.y = Mathf.Lerp(camPos.y, targetCamY, Time.deltaTime * transitionSpeed);
                cameraHolder.localPosition = camPos;
            }

        }

        bool CanStandUp()
        {
            //Cast upwards from top of capsule to the standing height
            Vector3 sphereOrigin = Root.position + CharacterController.center + Vector3.up * (CharacterController.height / 2f - CharacterController.radius);
            float castDistance = standingHeight - CharacterController.height;

            if (castDistance <= 0.01f) return true;

            return !Physics.SphereCast(sphereOrigin, CharacterController.radius * 0.95f, Vector3.up, out _, castDistance, ceilingLayerMask, QueryTriggerInteraction.Ignore);
        }
        #endregion  
    }
}