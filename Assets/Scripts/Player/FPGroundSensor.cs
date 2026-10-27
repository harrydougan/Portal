using System;
using UnityEngine;

namespace PRTL
{ 
    public class FPGroundSensor : FPModule
    {
        [SerializeField] LayerMask groundLayerMask = Physics.DefaultRaycastLayers;
        
        private void Update()
        {
            float positionOffset = 0.5f;
            Vector3 rayPosition = Root.position + Vector3.up * positionOffset;
            Ray ray = new Ray(rayPosition, Vector3.down);

            Controller.DistanceToGround = 100f;

            if (Physics.SphereCast(ray, 0.1f, out var hit, 100f, groundLayerMask)) // Sphere cast less precise than Raycast
            {
                if (Vector3.Angle(hit.normal, Vector3.up) <= CharacterController.slopeLimit)
                {
                    Controller.DistanceToGround = Mathf.Max(hit.distance - positionOffset, 0f);
                }
            }
        }
    }
}