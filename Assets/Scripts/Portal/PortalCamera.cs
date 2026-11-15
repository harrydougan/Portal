
using UnityEngine;


namespace PRTL
{
    public class PortalCamera : MonoBehaviour
    {
        public Transform playerCamera;
        public Transform portal;        // The Plane this camera is rendering behind
        public Transform otherPortal;   // The entrance Plane the player is looking into

        public Camera portalCamera;
        public Camera mainCamera;

        public float nearClipOffset = 0.05f;
        public float nearClipLimit = 0.1f;
        void LateUpdate()
        {
            if (playerCamera == null || portal == null || otherPortal == null)
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

            //Oblique projection matrix

            //Set cameras oblique view frustum
            SetNearClipPlane();


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