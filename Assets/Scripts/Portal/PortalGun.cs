using UnityEngine;



namespace PRTL
{
    public class PortalGun : MonoBehaviour
    {
        [Header("Transforms")]
        public Camera playerCamera;
        public Transform portalA;
        public Transform portalB;

        public Transform testTransform;
        public LayerMask portableLayer;
        public LayerMask obstacleLayer;

        [Header("Portal Dimensions")] 
        public float portalWidth = 2f;
        public float portalHeight = 4f;
        public float surfaceOffset = 0.05f;
        
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
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            //Cast against obstacleLayer so non-portable walls block the shot
            if (Physics.Raycast(ray, out RaycastHit hit, maxRange, obstacleLayer, QueryTriggerInteraction.Ignore))
            {
                //Check if the surface we hit is actually on portableLayer
                bool isPortable = (portableLayer.value & (1 << hit.collider.gameObject.layer)) != 0;

                if (isPortable)
                {
                    Vector3 surfaceNormal = hit.normal;
                    if (Vector3.Dot(surfaceNormal, ray.direction) > 0f)
                    {
                        surfaceNormal = -surfaceNormal;
                    }

                    testTransform.position = hit.point + surfaceNormal * surfaceOffset;

                    Vector3 upDirection = Vector3.up;
                    if (Mathf.Abs(Vector3.Dot(surfaceNormal, Vector3.up)) > 0.99f)
                    {
                        Vector3 projectedForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, surfaceNormal).normalized;
                        upDirection = projectedForward.sqrMagnitude > 0.001f ? projectedForward : Vector3.forward;
                    }
                    testTransform.rotation = Quaternion.LookRotation(surfaceNormal, upDirection);

                    FixOverhangs();
                    FixIntersects();

                    if (CheckOverlap(hit.collider, portal))
                    {
                        portal.position = testTransform.position;
                        portal.rotation = testTransform.rotation;

                        PortalTeleporter teleporter = portal.GetComponentInChildren<PortalTeleporter>();
                        if (teleporter != null)
                        {
                            teleporter.SetSurfaceCollider(hit.collider);
                        }

                        if (!portal.gameObject.activeSelf)
                        {
                            portal.gameObject.SetActive(true);
                        }

                        if (portalSound != null) portalSound.Play();
                        return;
                    }
                }
            }

            // Play invalid sound if ray missed, hit a non-portable wall, or failed overlap checks
            if (invalidSound != null)
            {
                invalidSound.Play();
            }
        }

        private void FixOverhangs()
        {
            float halfWidth = portalWidth * 0.5f;
            float halfHeight = portalHeight * 0.5f;

            Vector3[] testPoints = new Vector3[]
            {
                new Vector3(-halfWidth, 0, 0), // Left
                new Vector3( halfWidth, 0, 0), // Right
                new Vector3(0, -halfHeight, 0), // Bottom
                new Vector3(0,  halfHeight, 0)  // Top
            };

            Vector3[] inwardDirs = new Vector3[]
            {
                testTransform.right,
                -testTransform.right,
                testTransform.up,
                -testTransform.up
            };

            for (int i = 0; i < 4; i++)
            {
                Vector3 worldEdge = testTransform.TransformPoint(testPoints[i]);
        
                // First check if this edge ALREADY has a wall behind it.
                // If a neighboring ProBuilder tile is supporting this edge, do NOT run the inward raycast!
                Vector3 frontCheck = worldEdge + testTransform.forward * 0.05f;
                Vector3 backCheck = worldEdge - testTransform.forward * (surfaceOffset + 0.01f);
                if (Physics.Linecast(frontCheck, backCheck, portableLayer))
                {
                    continue; // This edge is already on a valid wall piece; skip inward seam check
                }

                // Only if the edge is genuinely hanging in mid-air, raycast inward to find the wall edge
                Vector3 edgeOrigin = worldEdge - testTransform.forward * (surfaceOffset + 0.05f);
                float maxDist = (i < 2) ? halfWidth : halfHeight;

                if (Physics.Raycast(edgeOrigin, inwardDirs[i], out RaycastHit hit, maxDist, portableLayer))
                {
                    testTransform.position += inwardDirs[i] * hit.distance;
                }
            }
        }

        private void FixIntersects()
        {
            float halfWidthidth = portalWidth * 0.5f;
            float halfHeighteight = portalHeight * 0.5f;
            
            Vector3[] outwardDirs = new Vector3[]
            {
                -testTransform.right, //Left
                testTransform.right, //Right
                -testTransform.up, //Bottom
                testTransform.up, //Top
            };
            for (int i = 0; i < 4; i++)
            {
                float maxDist = (i < 2) ? halfWidthidth : halfHeighteight;
                // Cast outward from the center slightly in front of the wall
                if (Physics.Raycast(testTransform.position, outwardDirs[i], out RaycastHit hit, maxDist, obstacleLayer))
                {
                    // Push away from the intersecting wall
                    testTransform.position -= outwardDirs[i] * (maxDist - hit.distance);
                }
            }
        }

        private bool CheckOverlap(Collider wallCollider, Transform currentPortal)
        {
            float halfWidth = portalWidth * 0.5f;
            float halfHeight = portalHeight * 0.5f;

            //Check all 4 corners to guarantee no overhangs remain
            Vector3[] corners = new Vector3[]
            {
                new Vector3(-halfWidth, -halfHeight, 0),
                new Vector3( halfWidth, -halfHeight, 0),
                new Vector3(-halfWidth,  halfHeight, 0),
                new Vector3( halfWidth,  halfHeight, 0)
            };

            foreach (Vector3 corner in corners)
            {
                Vector3 worldCorner = testTransform.TransformPoint(corner);
                Vector3 start = worldCorner + testTransform.forward * 0.25f;
                Vector3 end = worldCorner - testTransform.forward * (surfaceOffset + 0.1f);

                if (!Physics.Linecast(start, end, portableLayer))
                {
                    return false;
                }
            }

            // Check that the portal volume isn't clipping inside walls or the other active portal
            Vector3 halfExtents = new Vector3(halfWidth - 0.05f, halfHeight - 0.05f, 0.02f);
            Collider[] overlaps = Physics.OverlapBox(
                testTransform.position,
                halfExtents,
                testTransform.rotation,
                obstacleLayer,
                QueryTriggerInteraction.Ignore
            );

            foreach (Collider col in overlaps)
            {
                if (col == wallCollider || col.transform.IsChildOf(currentPortal) || col.transform == currentPortal)
                {
                    continue;
                }
                return false;
            }

            return true;
        }
    }
}

