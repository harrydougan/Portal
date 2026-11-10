using UnityEngine;


namespace PRTL
{
    public class FPPlayerPickUpDrop : MonoBehaviour
    {

        [SerializeField] private Transform playerCameraTransform;
        [SerializeField] private Transform objectGrabPointTransform;
        [SerializeField] private LayerMask pickUpLayerMask;

        private ObjectGrabbable objectGrabbable;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (objectGrabbable == null)
                {
                    //Not carrying an object, try to grab.
                    float pickUpDistance = 2f;
                    if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
                    {
                        Debug.Log(raycastHit.transform);
                        if (raycastHit.transform.TryGetComponent(out objectGrabbable))
                        {
                            objectGrabbable.Grab(objectGrabPointTransform);
                            Debug.Log(objectGrabbable);
                        }
                    }
                } else
                {
                    //Currently carrying an object. Drop
                    objectGrabbable.Drop();
                    objectGrabbable = null;
                }

            }
        }
    }
}

