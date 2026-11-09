using UnityEngine;

namespace PRTL
{
    public class PortalTextureSetup : MonoBehaviour
    {
        public Camera cameraB;
        public Material cameraBMat;

        void Start()
        {
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


