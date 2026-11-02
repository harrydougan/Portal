using UnityEngine;
using UnityEngine.Rendering;

namespace PRTL
{
    public class FPResetSafePosition : FPModule
    {
        Vector3 safePosition;
        //Quaternion safeRotation;
        float nextCheckTime = 0f;

        enum EState
        {
            Idle,

            Transition
        }

        EState state = EState.Idle;

        [SerializeField] Volume transitionVolume;

        void SetVolumeVisible(bool visible)
        {
            if (transitionVolume == null)
            {
                return;
            }

            float targetWeight = visible ? 1f : 0f;

            transitionVolume.weight = Mathf.Lerp(
                transitionVolume.weight,
                targetWeight,
                5f * Time.deltaTime
            );
        }

        void ChangeState(EState newState)
        {
            state = newState;

            if (state == EState.Transition)
            {
                CharacterController.enabled = false;
                Controller.InputEnabled = false;

                Controller.SetVerticalSpeed(0f);
                Controller.SetHorizontalVelocity(Vector3.zero);
            }
            else if (state == EState.Idle)
            {
                CharacterController.enabled = true;
                Controller.InputEnabled = true;
            }
        }


        #region Unity Methods
        private void Start()
        {
            safePosition = Root.position;
            //safeRotation = Root.rotation;

            if (transitionVolume != null)
            {
                transitionVolume.weight = 0f; // Sets the PPV to 0 on start
            }
        }

        private void Update()
        {
           if (Root.position.y <= -5f && state == EState.Idle) // Change for level of death floor on Y
            {
                ChangeState(EState.Transition);
            }

           if (state == EState.Transition)
            {

                float maxDelta = 15f * Time.deltaTime; // Speed of transition

                Root.position = Vector3.MoveTowards(Root.position, safePosition, maxDelta);

                //maxDelta = 200f * Time.deltaTime; 

                //Root.rotation = Quaternion.RotateTowards(Root.rotation, safeRotation, maxDelta);

                float dist = Vector3.Distance(Root.position, safePosition);

                if (dist <= 0.05f)
                {
                    Root.position = safePosition;
                    //Root.rotation = safeRotation;

                    ChangeState(EState.Idle);
                }

            }

            SetVolumeVisible(state == EState.Transition);
        }

        private void LateUpdate()
        {
            if (Controller.Grounded && Time.time >= nextCheckTime && state == EState.Idle)
            {
                safePosition = Root.position;
                //safeRotation = Root.rotation;

                nextCheckTime = Time.time + 0.5f;
            }
        }
        #endregion
    }

}
