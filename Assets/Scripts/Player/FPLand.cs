using System.Collections;
using UnityEngine;

namespace PRTL
{
    public class FPLand : FPModule
    {
        bool wasGrounded = false;

        private void Update()
        {
            if (Controller.IsInPortal)
            {
                wasGrounded = false;
                return;
            }

            if (!wasGrounded && Controller.Grounded)
            {
                Controller.Landed.Invoke(Controller.VerticalSpeed);
            }

            wasGrounded = Controller.Grounded;
        }
    }
}