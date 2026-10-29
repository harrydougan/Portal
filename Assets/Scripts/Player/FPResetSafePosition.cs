using UnityEngine;

namespace PRTL
{
    public class FPResetSafePosition : FPModule
    {
        Vector3 safePosition;
        Quaternion safeRotation;
        float nextCheckTime = 0f;

        private void Start()
        {
            safePosition = Root.position;
            safeRotation = Root.rotation;

        }

        private void Update()
        {
           if (Root.position.y <= -1f) // Change for level of death floor on Y
            {
                CharacterController.enabled = false;
                Controller.InputEnabled = false;

                Root.position = safePosition;
                Root.rotation = safeRotation;

                Controller.SetVerticalSpeed(0f);

                Controller.InputEnabled = true;
                CharacterController.enabled = true;
            } 
        }

        private void LateUpdate()
        {
            if (Controller.Grounded && Time.time >= nextCheckTime)
            {
                safePosition = Root.position;
                safeRotation = Root.rotation;

                nextCheckTime = Time.time + 0.5f;
            }
        }
    }

}
