using UnityEngine;



namespace PRTL
{
    public class PortalGun : MonoBehaviour
    {
        public Camera playerCamera;
        public Transform portalA;
        public Transform portalB;
        public LayerMask portableLayer;
        public float maxRange = 100f;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShootPortal(portalA); // Shoot Portal A
            } else if (Input.GetMouseButtonDown(1))
            {
                ShootPortal(portalB); // Shoot Portal B
            }
        }

        private void ShootPortal(Transform portal)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if(Physics.Raycast(ray, out RaycastHit hit, maxRange, portableLayer))
            {
                portal.position = hit.point + hit.normal * 0.05f; //Place portal slightly off wall to prevent Z-fighting.

                Vector3 upDirection = Vector3.up; //Determines Portals "up" vector.

                if(Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up)) > 0.99f) // handles the walls vs floor/ceiling
                {
                    upDirection = playerCamera.transform.forward; //For floors and ceilings handle portals "up" with players look direction.
                }

                //Apply portal rotation
                portal.rotation = Quaternion.LookRotation(hit.normal, upDirection); // Bc portals front face is Local Y

                if (!portal.gameObject.activeSelf)
                {
                    portal.gameObject.SetActive(true);
                }
            }
        }
    }
}

