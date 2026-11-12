using UnityEngine;

namespace PRTL
{
    public class PortalCamera : MonoBehaviour
    {
        public Transform playerCamera;
        public Transform portal;
        public Transform otherPortal;

        void LateUpdate()
        {
            if (playerCamera == null || portal == null || otherPortal == null)
                return;

            // Use otherPortal.forward if your Unity Plane is stood upright, 
            // or Vector3.up if using standard forward-facing portals.
            Quaternion flip = Quaternion.AngleAxis(180f, otherPortal.forward);

            Matrix4x4 m = portal.localToWorldMatrix *
                          Matrix4x4.Rotate(flip) *
                          otherPortal.worldToLocalMatrix;

            transform.position = m.MultiplyPoint3x4(playerCamera.position);
            transform.rotation = m.rotation * playerCamera.rotation;

        }
    }

}
