using UnityEngine;

namespace PRTL
{
    public class PortalTextureSetup : MonoBehaviour
    {
        public Camera cameraA;
        public Camera cameraB;
        public Material cameraAMat;
        public Material cameraBMat;

        void Start()
        {
            if (cameraA.targetTexture != null) //If Cam B already has a target texture assigned
            {
                cameraA.targetTexture.Release();
            }
            //Correct Camera Scaling through portal to screen size
            cameraA.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            cameraAMat.mainTexture = cameraA.targetTexture;

            
            if (cameraB.targetTexture != null) //If Cam B already has a target texture assigned
            {
                cameraB.targetTexture.Release();
            }
            //Correct Camera Scaling through portal to screen size
            cameraB.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            cameraBMat.mainTexture = cameraB.targetTexture;
        }
    }
}


