using System.Collections;
using UnityEngine;

namespace PRTL
{
    public class FPCeilingSensor : FPModule
    {
        [SerializeField] LayerMask ceilingLayerMask = Physics.DefaultRaycastLayers;

        private void Update()
        {
            float castRadius = CharacterController.radius * 0.95f;

            // Start at the BOTTOM hemisphere of the capsule (near the feet) so it can NEVER start inside a ceiling
            Vector3 bottomOfCapsule = Root.position + CharacterController.center - Vector3.up * (CharacterController.height / 2f);
            Vector3 sphereOrigin = bottomOfCapsule + Vector3.up * castRadius;

            // The distance from the bottom sphere to the top of the player's head
            float internalCapsuleTravel = Mathf.Max(CharacterController.height - (castRadius * 2f), 0f);

            Ray ray = new Ray(sphereOrigin, Vector3.up);

            Controller.DistanceToCeiling = 100f;

            if (Physics.SphereCast(ray, castRadius, out var hit, 100f, ceilingLayerMask, QueryTriggerInteraction.Ignore))
            {
                // Subtract the height of the player's body so 0 = touching the top of the player's head
                Controller.DistanceToCeiling = Mathf.Max(hit.distance - internalCapsuleTravel, 0f);
            }
        }
    }
}