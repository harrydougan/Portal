using UnityEngine;

namespace PRTL
{
    public class ObjectGrabbable : MonoBehaviour
    {
        private Rigidbody objectRigidbody;
        private Transform objectGrabPointTransform;
        private void Awake()
        {
            objectRigidbody = GetComponent<Rigidbody>();
        }
        public void Grab(Transform objectGrabPointTransform)
        {
            this.objectGrabPointTransform = objectGrabPointTransform;
            objectRigidbody.useGravity = false; //Turn off gravity on Object when we pick it up
        }

        public void Drop()
        {
            this.objectGrabPointTransform = null;
            objectRigidbody.useGravity = true;
        }
        private void FixedUpdate()
        {
            if (objectGrabPointTransform != null)
            {
                float lerpSpeed = 10f; 
                Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed); //Smooth objects transition to GrabPoint Position using lerp

                objectRigidbody.MovePosition(newPosition);
            }
        }
    }
}

