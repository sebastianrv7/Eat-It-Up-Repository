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
    private string Falling = "Fall";
    private string Slide = "Sliding";

    void OnEnable()
    {
        myPlayerMovement.OnDirectionChange += ChangeVisualDirection;
        myPlayerMovement.OnStartJump += Jump;
        myPlayerMovement.OnStartDoubleJump += DoubleJump;
        myPlayerMovement.OnMaxJumpHeight += MaxJumpHeight;
        myPlayerMovement.OnFinishJump += Walking;
        myPlayerMovement.OnStartSlide += Sliding;
        myPlayerMovement.OnStopSlide += Sliding;
    }

    void OnDisable()
    {
        myPlayerMovement.OnDirectionChange -= ChangeVisualDirection;
        myPlayerMovement.OnStartJump -= Jump;
        myPlayerMovement.OnStartDoubleJump -= DoubleJump;
        myPlayerMovement.OnMaxJumpHeight -= MaxJumpHeight;
        myPlayerMovement.OnFinishJump -= Walking;
        myPlayerMovement.OnStartSlide -= Sliding;
        myPlayerMovement.OnStopSlide -= Sliding;
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
        myAnimator.SetBool(jumping, true);
        myAnimator.SetBool(Slide, false);
    }

    private void DoubleJump()
    {
        myAnimator.SetBool(Walk, false);
        myAnimator.SetBool(doubleJumping, true);
        myAnimator.SetBool(Slide, false);
    }

    private void MaxJumpHeight()
    {
        myAnimator.SetBool(Walk, false);
        myAnimator.SetBool(Falling, true);
        myAnimator.SetBool(Slide, false);
    }

    private void Walking()
    {
        myAnimator.SetBool(Walk, true);
        myAnimator.SetBool(jumping, false);
        myAnimator.SetBool(doubleJumping, false);
        myAnimator.SetBool(Falling, false);
        myAnimator.SetBool(Slide, false);

    }

    private void Sliding()
    {
        myAnimator.SetBool(Slide, true);
        myAnimator.SetBool(jumping, false);
        myAnimator.SetBool(doubleJumping, false);
        myAnimator.SetBool(Falling, false);

    }
}
