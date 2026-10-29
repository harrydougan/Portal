using UnityEngine;


namespace PRTL
{
    [DefaultExecutionOrder(-1000)]

    public class FPGameEventBusListener : FPModule
    {

        [SerializeField] GameEventBus gameEventBus;

        protected override void Awake()
        {
            base.Awake();

            gameEventBus.SetPlayerInputEnabled += (inputEnabled) => 
            {
                Controller.InputEnabled = inputEnabled;
            };
        }
    }
}

