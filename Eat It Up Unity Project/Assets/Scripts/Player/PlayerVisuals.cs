using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField]
    private GameObject visuals;
    [SerializeField]
    private PlayerMovement myPlayerMovement;
    [SerializeField]
    private PlayerHealth myPlayerHealth;
    [SerializeField]
    private Animator myAnimator;
    [SerializeField]
    private ParticleSystem spawnParticle;
    [SerializeField]
    private ParticleSystem deathParticle;
    [SerializeField]
    private ParticleSystem movementParticle;
    [SerializeField]
    private ParticleSystem slideParticle;
    [SerializeField]
    private ParticleSystem doubleJumpParticle;
    [SerializeField]
    private Vector3 slideParticleOffset;
    [SerializeField]
    private GameObject jumpParticleParent;
    [SerializeField]
    private List<ParticleSystem> jumpParticles;

    private Coroutine movementParticleCoroutine;
    private Coroutine slideParticleCoroutine;
    private PlayerMovement.MovementDirection currentDirection;

    private string jumping = "Jump";
    private string doubleJumping = "DoubleJump";
    private string Walk = "Walking";
    private string Falling = "Fall";
    private string Slide = "Sliding";
    private string Died = "Death";

    void OnEnable()
    {
        myPlayerMovement.OnDirectionChange += ChangeVisualDirection;
        myPlayerMovement.OnStartJump += Jump;
        myPlayerMovement.OnStartDoubleJump += DoubleJump;
        myPlayerMovement.OnMaxJumpHeight += MaxJumpHeight;
        myPlayerMovement.OnFinishJump += Walking;
        myPlayerMovement.OnStartSlide += Sliding;
        myPlayerMovement.OnStopSlide += StopSliding;
        myPlayerHealth.OnDeath += Death;
    }

    void OnDisable()
    {
        myPlayerMovement.OnDirectionChange -= ChangeVisualDirection;
        myPlayerMovement.OnStartJump -= Jump;
        myPlayerMovement.OnStartDoubleJump -= DoubleJump;
        myPlayerMovement.OnMaxJumpHeight -= MaxJumpHeight;
        myPlayerMovement.OnFinishJump -= Walking;
        myPlayerMovement.OnStartSlide -= Sliding;
        myPlayerMovement.OnStopSlide -= StopSliding;
        myPlayerHealth.OnDeath -= Death;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeVisualDirection(myPlayerMovement.CurrentDirection);
        movementParticle.transform.parent = null;
        movementParticleCoroutine = StartCoroutine(PlayMovementParticle());
        jumpParticleParent.transform.parent = null;
        doubleJumpParticle.transform.parent = null;
        slideParticle.transform.parent = null;
        SpawnPlayer();
    }

    private void ChangeVisualDirection(PlayerMovement.MovementDirection newDirection)
    {
        currentDirection = newDirection;
        switch (newDirection)
        {
            case PlayerMovement.MovementDirection.Left:
                gameObject.transform.localScale = new Vector3(-1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                movementParticle.transform.localScale = new Vector3(1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                break;

            case PlayerMovement.MovementDirection.Right:
                gameObject.transform.localScale = new Vector3(1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                movementParticle.transform.localScale = new Vector3(-1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                break;

            default:
                break;
        }
    }

    private void Jump()
    {
        if (movementParticleCoroutine != null)
            StopCoroutine(movementParticleCoroutine);

        myAnimator.SetBool(Walk, false);
        myAnimator.SetBool(jumping, true);
        myAnimator.SetBool(Slide, false);
        myAnimator.SetBool(Falling, false);
        PlayJumpParticle();
    }

    private void DoubleJump()
    {
        myAnimator.SetBool(Walk, false);
        myAnimator.SetBool(doubleJumping, true);
        myAnimator.SetBool(Slide, false);
        myAnimator.SetBool(Falling, false);
        PlayDoubleJumpParticle();
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
        if (movementParticleCoroutine != null)
            StopCoroutine(movementParticleCoroutine);
        movementParticleCoroutine = StartCoroutine(PlayMovementParticle());
        slideParticle.Stop();
    }

    private void Sliding()
    {
        myAnimator.SetBool(Slide, true);
        myAnimator.SetBool(jumping, false);
        myAnimator.SetBool(doubleJumping, false);
        myAnimator.SetBool(Falling, false);
        myAnimator.SetBool(Walk, false);
        slideParticleCoroutine = StartCoroutine(FollowSlideParticles());
        if(movementParticleCoroutine != null)
            StopCoroutine(movementParticleCoroutine);
        slideParticle.Play();
    }

    private void StopSliding()
    {
        myAnimator.SetBool(Slide, false);
        if(slideParticle != null)
            slideParticle.Stop();
        if(slideParticleCoroutine != null)
            StopCoroutine(slideParticleCoroutine);
    }

    private void Death()
    {
        myAnimator.SetTrigger(Died);
        StartCoroutine(DeathAnimation());
    }

    private void SpawnPlayer()
    {
        spawnParticle.Play();
        visuals.SetActive(true);
    }

    private IEnumerator PlayMovementParticle()
    {
        while (true)
        {
            yield return new WaitForSeconds(.75f);
            movementParticle.transform.position = gameObject.transform.position + new Vector3(0f, -0.5f, 0f);
            movementParticle.Emit(1);
        }
    }

    private void PlayJumpParticle()
    {
        jumpParticleParent.transform.position = gameObject.transform.position;
        foreach (ParticleSystem particle in jumpParticles)
        {
            particle.Play();
        }
    }

    private void PlayDoubleJumpParticle()
    {
        doubleJumpParticle.transform.position = gameObject.transform.position;
        doubleJumpParticle.Play();
    }
    private IEnumerator DeathAnimation()
    {
        yield return new WaitForSeconds(0.25f);
        visuals.SetActive(false);
        deathParticle.Play();
    }

    private IEnumerator FollowSlideParticles()
    {
        while (true)
        {
            Vector3 offset = new Vector3();
            if (PlayerMovement.MovementDirection.Left == currentDirection)
            {
                offset = new Vector3(-slideParticleOffset.x, slideParticleOffset.y, slideParticleOffset.z);
            }
            else if (PlayerMovement.MovementDirection.Right == currentDirection)
            {
                offset = new Vector3(slideParticleOffset.x, slideParticleOffset.y, slideParticleOffset.z);
            }
            slideParticle.transform.position = gameObject.transform.position + offset;
            yield return new WaitForEndOfFrame();
        }

    }
}
