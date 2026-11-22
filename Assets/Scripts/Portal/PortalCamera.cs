
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace PRTL
{
    public class PortalCamera : MonoBehaviour
    {
        public Transform playerCamera;
        public Transform portal;        // The Plane this camera is rendering behind
        public Transform otherPortal;   // The entrance Plane the player is looking into

        public Camera portalCamera;
        public Camera mainCamera;

        [Header("Recursion Settings")]
        [Range(1, 10)]
        public int recursionLimit = 5;
        public MeshRenderer portalScreenRenderer; //Plane mesh renderer on other protal.

        [Header("Oblique Clipping Settings")]
        public float nearClipOffset = 0.05f;
        public float nearClipLimit = 0.1f;

        private Vector3[] renderPositions;
        private Quaternion[] renderRotations;
        
        private void Awake()
        {
            renderPositions = new Vector3[recursionLimit];
            renderRotations = new Quaternion[recursionLimit];
        }


        private void LateUpdate()
        {
            if (playerCamera == null || portal == null || otherPortal == null || portalCamera == null || mainCamera == null)
                return;

            if (portalCamera.targetTexture == null)
                return;

            if (renderPositions == null || renderPositions.Length != recursionLimit)
            {
                renderPositions = new Vector3[recursionLimit];
                renderRotations = new Quaternion[recursionLimit];
            }

            // Matrix that flips 180 degrees around the portal's normal (local Y axis)
            Matrix4x4 halfTurnMatrix = Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, 180f));

            // Start from the player camera's world matrix
            Matrix4x4 localToWorldMatrix = playerCamera.localToWorldMatrix;

            // Step FORWARD (0 -> recursionLimit - 1)
            for (int i = 0; i < recursionLimit; i++)
            {
                // Transform from entrance (otherPortal) local space -> flip 180 around local Y -> exit (portal) world space
                localToWorldMatrix = portal.localToWorldMatrix * halfTurnMatrix * otherPortal.worldToLocalMatrix * localToWorldMatrix;

                renderPositions[i] = localToWorldMatrix.GetColumn(3);
                renderRotations[i] = localToWorldMatrix.rotation;
            }

            // Render BACKWARD (recursionLimit - 1 -> 0)
            for (int i = recursionLimit - 1; i >= 0; i--)
            {
                portalCamera.transform.SetPositionAndRotation(renderPositions[i], renderRotations[i]);

                if (portalScreenRenderer != null)
                {
                    portalScreenRenderer.enabled = (i != recursionLimit - 1);
                }

                SetNearClipPlane();

                var request = new UniversalRenderPipeline.SingleCameraRequest
                {
                    destination = portalCamera.targetTexture
                };

                if (RenderPipeline.SupportsRenderRequest(portalCamera, request))
                {
                    RenderPipeline.SubmitRenderRequest(portalCamera, request);
                }
            }

            if (portalScreenRenderer != null)
            {
                portalScreenRenderer.enabled = true;
            }
        }

        void SetNearClipPlane()
        {

            Transform clipPlane = portal;//Storing a ref to our exit portals transform.
            //System.Math.Sign reduces result to 1 or -1 so we can flip later.
            int dot = System.Math.Sign(Vector3.Dot(clipPlane.up, clipPlane.position - portalCamera.transform.position));//Creates a vector pointing from cam straight to portal.
                                                                                                                        //Compares the Vector agaisnt portals normal.
                                                                                                                        //If they point the same direction (camera behind portal) dot product = 1.
                                                                                                                        //If they point in opposite directions (Camera infront portal) dot product = -1

            Vector3 camSpacePos = portalCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);//Transforms the portals world pos into portal cameras local coords (MultiplyPoints
                                                                                                     //allows us to get trans, rot and scale)
            Vector3 camSpaceNormal = portalCamera.worldToCameraMatrix.MultiplyVector(clipPlane.up) * dot; //Transforms portals normal direction clipPlane.up into
                                                                                                          //camera space. Multiplying by Dot (+1 to -1) makes sure that the normal points
                                                                                                          //away from the camera. Ensuring Unity clips whatever is behind the plane. 

            float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

            // Don't use oblique clip plane if very close to portal
            if (Mathf.Abs(camSpaceDst) > nearClipLimit)
            {
                Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);//Packs the plane equation calculated in camSpaceDst into Vec4 format 

                // Update projection based on new clip plane
                // Calculate matrix with player cam so that player camera settings (fov, etc) are used
                portalCamera.projectionMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
            }
            else
            {
                portalCamera.projectionMatrix = mainCamera.projectionMatrix;//If camera is closer than nearClipLimit. Fallback to standard projection matrix until player finishes crossing.
            }
        }
    }
}