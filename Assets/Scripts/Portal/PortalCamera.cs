using UnityEngine;

namespace PRTL
{
    public class PortalCamera : MonoBehaviour
    {
        public Transform playerCamera;
        public Transform portal;
        public Transform otherPortal;

        void Update()
        {
            // Offset is the Main cameras position - the other portals position.
            Vector3 playerOffsetFromPortal = playerCamera.position - otherPortal.position;
            // New transform is the Portal Position + The Offset
            transform.position = portal.position + playerOffsetFromPortal;

            //Angular Diff in Rotations
            float angularDiffBetweenPortalRotations = Quaternion.Angle(portal.rotation, otherPortal.rotation);

            //Portal Rotation Diff
            Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDiffBetweenPortalRotations, Vector3.up);

            //Direction camera needs to point
            Vector3 newCameraDirection = portalRotationalDifference * playerCamera.forward;
            transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);

        }
    }

}
