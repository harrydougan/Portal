using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PRTL
{
    public class FPGroundSensor : FPModule
    {
        [SerializeField] LayerMask groundLayerMask = Physics.DefaultRaycastLayers;
        private FPMovement fpMovement;



        private void Update()
        {
            Controller.DistanceToGround = 100f;

            // Skip ground checks while the player is passing through a portal trigger
            if (fpMovement != null)
                return;

            float positionOffset = 0.5f;
            Vector3 rayPosition = Root.position + Vector3.up * positionOffset;
            Ray ray = new Ray(rayPosition, Vector3.down);

            float capsuleRadius = CharacterController.radius * 0.9f;

            if (Physics.SphereCast(ray, capsuleRadius, out var hit, 100f, groundLayerMask))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) <= CharacterController.slopeLimit)
                {
                    Controller.DistanceToGround = Mathf.Max(hit.distance - positionOffset, 0f);
                }
            }
        }
    }
}