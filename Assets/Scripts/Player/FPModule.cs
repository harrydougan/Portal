using System.Collections;
using UnityEngine;

namespace PRTL
{
    public class FPModule : MonoBehaviour
    {
        protected FPController Controller;
        public CharacterController CharacterController => Controller.CharacterController;

        public Transform Root => Controller.transform; //Shortcut for Transform attached to FPController
        
        #region Unity Methods

        protected virtual void OnValidate()
        {
            ResolveDependancies();
        }

        protected virtual void Awake()
        {
            ResolveDependancies();
        }

        protected virtual void ResolveDependancies()
        {
            Controller = GetComponentInParent<FPController>();
        }
        #endregion
    }
}