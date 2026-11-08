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
        }
    }

}
