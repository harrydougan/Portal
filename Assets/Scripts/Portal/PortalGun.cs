using UnityEngine;



namespace PRTL
{
    public class PortalGun : MonoBehaviour
    {
        [Header("Transforms")]
        public Camera playerCamera;
        public Transform portalA;
        public Transform portalB;
        public LayerMask portableLayer;
        [Header("Range")]
        public float maxRange = 100f;
        [Header("Sound Effects")]
        public AudioSource blueSound;
        public AudioSource orangeSound;
        public AudioSource invalidSound;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShootPortal(portalA, blueSound); // Shoot Portal A and Play sound effect
            } else if (Input.GetMouseButtonDown(1))
            {
                ShootPortal(portalB, orangeSound); // Shoot Portal B and Play sound effect
            }
        }

        private void ShootPortal(Transform portal, AudioSource portalSound)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);//Where are ray is projecting from.

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
                
                //Setting Surface Collider of collider hit behind portal.
                PortalTeleporter teleporter = portal.GetComponentInChildren<PortalTeleporter>();
                if (teleporter != null)
                {
                    teleporter.SetSurfaceCollider(hit.collider);
                }

                if (!portal.gameObject.activeSelf)
                {
                    portal.gameObject.SetActive(true);
                }

                if(portalSound != null)
                {
                    portalSound.Play();
                }
            }
            else //If ray is not in max range and player is clicking mouse 0 and 1 play invalid sound
            {
                if(invalidSound != null)
                {
                    invalidSound.Play();
                }
            }
        }
    }
}

