using System.Collections.Generic;
using UnityEngine;

namespace PRTL
{
    public class Door : MonoBehaviour
    {
        private static readonly int Down = Animator.StringToHash("Down");
        public bool IsDoorOpen = false;
   
        [SerializeField] private int requiredSwitchesToOpen = 1;
        [SerializeField] private Animator doorAnimator;
        [SerializeField] private AudioSource doorOpenSound;
   
        private List<PressureSwitch> currentSwitchesOpen = new();
        
        public int CurrentSwitchesOpen => currentSwitchesOpen.Count;

        public void AddPressureSwitch(PressureSwitch currentSwitch)
        {
            if (!currentSwitchesOpen.Contains(currentSwitch))
            {
                currentSwitchesOpen.Add(currentSwitch);
            }
            
            TryOpen();
        }

        public void RemovePressureSwitch(PressureSwitch currentSwitch)
        {
            if (currentSwitchesOpen.Contains(currentSwitch))
            {
                currentSwitchesOpen.Remove(currentSwitch);
            }

            TryOpen();
        }

        private void TryOpen()
        {
            if (CurrentSwitchesOpen == requiredSwitchesToOpen)
            {
                OpenDoor();
            } else if (CurrentSwitchesOpen < requiredSwitchesToOpen)
            {
                CloseDoor();
            }
        }

        private void OpenDoor()
        {
            if (!IsDoorOpen)
            {
                doorAnimator.SetBool("Down", true);
                doorOpenSound.Play();
                
            }
            IsDoorOpen = true;
        }

        private void CloseDoor()
        {
            if (IsDoorOpen)
            {
                doorAnimator.SetBool("Down", false);  
            }
            IsDoorOpen = false;
        }
    }
}


