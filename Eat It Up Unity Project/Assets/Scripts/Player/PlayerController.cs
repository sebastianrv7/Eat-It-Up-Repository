using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{    
    public InputAction jump;
    public InputAction pause;

    public delegate void InputActions();
    public event InputActions OnJump, OnPause;

    void OnEnable()
    {
        jump.performed += JumpPressed;
        pause.performed += PausePressed;
    }

    void OnDisable()
    {
        jump.performed -= JumpPressed;
        pause.performed -= PausePressed;
    }

    void Awake()
    {
        jump = InputSystem.actions.FindAction("Jump");
        pause = InputSystem.actions.FindAction("Pause");
        EnableController();
    }

    public void EnableController()
    {
        jump.Enable();
        pause.Enable();
    }

    public void DisableController()
    {
        jump.Disable();
        //pause.Disable();
    }

    private void PausePressed(InputAction.CallbackContext context)
    {
        OnPause?.Invoke();
    }

    private void JumpPressed(InputAction.CallbackContext context)
    {
        if (GameManager.GamePaused)
            return;
        OnJump?.Invoke();
    }

}
