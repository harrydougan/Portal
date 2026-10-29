using UnityEngine;
using UnityEngine.Events;


namespace PRTL
{
    [CreateAssetMenu(menuName = "PRTL/GameEventBus")]
    public class GameEventBus : ScriptableObject
    {
        public UnityAction<bool> SetPlayerInputEnabled;
    }
}

