using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{    
    public InputAction jump;


    public delegate void Jump();
    public event Jump OnJump;

    void OnEnable()
    {
        jump.performed += JumpPressed;
    }

    private void JumpPressed(InputAction.CallbackContext context)
    {
        OnJump();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        jump = InputSystem.actions.FindAction("Jump");
        jump.Enable();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
