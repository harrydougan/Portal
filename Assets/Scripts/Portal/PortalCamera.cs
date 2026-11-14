using Unity.VisualScripting;
using UnityEngine;

namespace PRTL
{
    public class PortalCamera : MonoBehaviour
    {
        public Transform playerCamera;
        public Transform portal;        // The Plane this camera is rendering behind
        public Transform otherPortal;   // The entrance Plane the player is looking into

        void LateUpdate()
        {
            if (playerCamera == null || portal == null ||  otherPortal == null) 
                return;

            // Combine them into a single total rotation difference
            Quaternion halfTurn = Quaternion.Euler(0f, 0f, 180f);
            Quaternion relativeRotation = portal.rotation * halfTurn * Quaternion.Inverse(otherPortal.rotation);
            // 3. Rotate the player's position offset so moving forward into Portal A moves the camera forward out of Portal B
            Vector3 playerOffsetFromPortal = playerCamera.position - otherPortal.position;
            transform.position = portal.position + (relativeRotation * playerOffsetFromPortal);

            // 4. Rotate the camera's forward and up directions to match the new perspective
            Vector3 newCameraForward = relativeRotation * playerCamera.forward;
            Vector3 newCameraUp = relativeRotation * playerCamera.up;
            transform.rotation = Quaternion.LookRotation(newCameraForward, newCameraUp);




        }
    }
}