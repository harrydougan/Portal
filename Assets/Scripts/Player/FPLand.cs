using System.Collections;
using UnityEngine;

namespace PRTL

{
    public class FPLand : FPModule
    {
        bool wasGrounded = false;

        private void Update()
        {
            if (!wasGrounded && Controller.Grounded)
            {
                Controller.Landed.Invoke(Controller.VerticalSpeed);
            }
            
            wasGrounded = Controller.Grounded;
        }
    }
}