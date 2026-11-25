using System;
using Unity.VisualScripting;
using UnityEngine;

namespace PRTL
{
    public class PressureSwitch : MonoBehaviour
    {
        [SerializeField] private Door currentDoor;
        //[SerializeField] private Animator animator;

        private void OnTriggerStay(Collider other)
        {
            currentDoor.AddPressureSwitch(this);
            //animator.SetBool(Down, true)
        }

        private void OnTriggerExit(Collider other)
        {
            currentDoor.RemovePressureSwitch(this);
            //animator.SetBool(Down, false)
        }
    }
    
    
}

