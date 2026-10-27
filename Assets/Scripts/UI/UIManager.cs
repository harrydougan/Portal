using UnityEngine;
using UnityEngine.InputSystem;

namespace PRTL
{
    public class UIManager : MonoBehaviour
    {
        #region FSM
        enum EState
        {
            None,
            Play,
            Pause

        }

        EState currentState = EState.None;

        void ChangeState(EState newState)
        {
            OnStateExit(currentState);
            
            currentState = newState;

            OnStateEntry(currentState);
        }

        void OnStateEntry(EState state)
        {
            if (state == EState.Play)
            {
                SetCursorVisible(false);
            }
            else if (state == EState.Pause)
            {
                SetCursorVisible(true);
            }
        }

        void OnStateExit(EState state)
        {

        }
        #endregion

        #region Unity Methods

        private void Awake()
        {
            ChangeState(EState.Play);
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (currentState == EState.Play)
                {
                    ChangeState(EState.Pause);
                }
                else if (currentState == EState.Pause)
                {
                    ChangeState(EState.Play);
                }
            }
        }

        #endregion

        void SetCursorVisible(bool visible) // Locks cursor
        {
            if (visible)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}


