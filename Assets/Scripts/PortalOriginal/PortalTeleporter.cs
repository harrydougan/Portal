using UnityEngine;


namespace PRTL
{
    public class PortalTeleporterOld : MonoBehaviour
    {
        public Transform player;
        public Transform reciever;
        private bool playerIsOverLapping = false;
        void Update()
        {
            if (playerIsOverLapping)
            {
                Vector3 portalToPlayer = player.position - transform.position;
                float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

                
                //If true the player has moved across the portal field.
                if (dotProduct < 0f)
                {
                    //Teleport
                    float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation); // Diff in rotation between the two portals
                    rotationDiff += 180f;
                    player.Rotate(Vector3.up, rotationDiff);

                    Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                    player.position = reciever.position + positionOffset;
                    
                    
                    Physics.SyncTransforms(); //Force Unity to Update Physics properly, doesnt work without.

                    playerIsOverLapping = false;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") //Check other is actually Player
            {
                playerIsOverLapping = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                playerIsOverLapping = false;
            }
        }
    }
}


