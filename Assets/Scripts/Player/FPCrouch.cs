using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

namespace PRTL
{
    public class FPCrouch : FPModule
    {
        private void Start()
        {
            Controller.CrouchButtonPressed += OnCrouchButtonPressed;
        }

        void OnCrouchButtonPressed()
        {
            Debug.Log("Crouched");

            Crouch();
        }

        void Crouch()
        {
            CharacterController.height = CharacterController.height * 2f;
        }
    }
}