using System;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField]
    private GameObject visuals;
    [SerializeField]
    private PlayerMovement myPlayerMovement;
    [SerializeField]
    private Animator myAnimator;

    private string jumping = "Jump";
    private string doubleJumping = "DoubleJump";
    private string Walk = "Walking";
    private string MaxHeight = "MaxJumpHeight";
    
    void OnEnable()
    {
        myPlayerMovement.OnDirectionChange += ChangeVisualDirection;
        myPlayerMovement.OnStartJump += Jump;
        myPlayerMovement.OnStartDoubleJump += DoubleJump;
        myPlayerMovement.OnMaxJumpHeight += MaxJumpHeight;
        myPlayerMovement.OnFinishJump += Walking;
    }

    void OnDisable()
    {
        myPlayerMovement.OnDirectionChange -= ChangeVisualDirection;
        myPlayerMovement.OnStartJump -= Jump;
        myPlayerMovement.OnStartDoubleJump -= DoubleJump;
        myPlayerMovement.OnMaxJumpHeight -= MaxJumpHeight;
        myPlayerMovement.OnFinishJump -= Walking;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeVisualDirection(myPlayerMovement.CurrentDirection);
    }

    private void ChangeVisualDirection(PlayerMovement.MovementDirection newDirection)
    {
        switch (newDirection)
        {
            case PlayerMovement.MovementDirection.Left:
                gameObject.transform.localScale = new Vector3(-1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                break;

            case PlayerMovement.MovementDirection.Right:
                gameObject.transform.localScale = new Vector3(1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                break;

            default:
                break;
        }
    }

    private void Jump()
    {
        myAnimator.SetBool(Walk, false);
        myAnimator.SetTrigger(jumping);
    }

    private void DoubleJump()
    {
        myAnimator.SetBool(Walk, false);
        myAnimator.SetTrigger(doubleJumping);
    }

    private void MaxJumpHeight()
    {
        myAnimator.SetBool(Walk, false);
        myAnimator.SetTrigger(MaxHeight);
    }

    private void Walking()
    {
        myAnimator.SetBool(Walk, true);   
    }
}
