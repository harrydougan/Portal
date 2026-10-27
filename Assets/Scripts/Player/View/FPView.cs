using System.Collections;
using UnityEngine;

namespace PRTL
{
    public class FPView : FPModule
    {
        [SerializeField] float sensitivity = 0.1f;

        [SerializeField] Vector2 pitchLimits = new(-80f, 80f);

        Transform ViewRoot => transform;
        float currentPitch = 0f;

        private void Update()
        {
            currentPitch -= Controller.LookInput.y * sensitivity;
            currentPitch = Mathf.Clamp(currentPitch, pitchLimits.x, pitchLimits.y);

            ViewRoot.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);

            Root.Rotate(Vector3.up * Controller.LookInput.x * sensitivity);
        }
    }
}