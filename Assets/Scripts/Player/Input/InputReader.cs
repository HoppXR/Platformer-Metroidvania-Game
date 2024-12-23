using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, GameInput.IGameplayActions, GameInput.IUIActions
{
    private GameInput _gameInput;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();
            
            _gameInput.Gameplay.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);
            
            SetGameplay();
        }
    }

    public void SetGameplay()
    {
        _gameInput.Gameplay.Enable();
        _gameInput.UI.Disable();
    }

    public void SetUI()
    {
        _gameInput.UI.Enable();
        _gameInput.Gameplay.Disable();
    }
    
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;

    public event Action JumpEvent;
    public event Action JumpCancelledEvent;
    
    public event Action CrouchEvent;
    public event Action CrouchCancelledEvent;
    
    public event Action DashEvent;
    
    public event Action SwingEvent;
    public event Action SwingCancelledEvent;
    public event Action SwingShortenEvent;
    public event Action SwingExtendEvent;
    
    public event Action PauseEvent;
    public event Action ResumeEvent;
    
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        LookEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            JumpEvent?.Invoke();
        }

        if (context.phase == InputActionPhase.Performed)
        {
            SwingShortenEvent?.Invoke();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            JumpCancelledEvent?.Invoke();
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            CrouchEvent?.Invoke();
        }

        if (context.phase == InputActionPhase.Performed)
        {
            SwingExtendEvent?.Invoke();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            CrouchCancelledEvent?.Invoke();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            DashEvent?.Invoke();
        }
    }

    public void OnSwing(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            SwingEvent?.Invoke();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            SwingCancelledEvent?.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            PauseEvent?.Invoke();
        }
    }

    public void OnResume(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ResumeEvent?.Invoke();
        }
    }
}
