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
            if (playerCamera == null || portal == null || otherPortal == null)
                return;

            // 1. Find the rotation difference between the two portals' outward normals (.up)
            Quaternion normalDiff = Quaternion.FromToRotation(otherPortal.up, portal.up);

            // 2. Flip 180 degrees around the exit portal's vertical axis (.forward) so we look OUT of it
            Quaternion flip = Quaternion.AngleAxis(180f, portal.forward);

            // Combine them into a single total rotation difference
            Quaternion totalRotationDiff = flip * normalDiff;

            // 3. Rotate the player's position offset so moving forward into Portal A moves the camera forward out of Portal B
            Vector3 playerOffsetFromPortal = playerCamera.position - otherPortal.position;
            transform.position = portal.position + (totalRotationDiff * playerOffsetFromPortal);

            // 4. Rotate the camera's forward and up directions to match the new perspective
            Vector3 newCameraForward = totalRotationDiff * playerCamera.forward;
            Vector3 newCameraUp = totalRotationDiff * playerCamera.up;
            transform.rotation = Quaternion.LookRotation(newCameraForward, newCameraUp);

        }
    }
}