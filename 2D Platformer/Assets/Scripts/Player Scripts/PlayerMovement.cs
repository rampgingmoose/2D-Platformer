using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Vector2 climbInput;
    Rigidbody2D rigidBody2D;
    Animator animator;
    CapsuleCollider2D playerCollider;
    BoxCollider2D feetCollider;
    PlayerInput playerInput;

    [SerializeField] float moveSpeed = 20f;
    [SerializeField] float climbSpeed = 15f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float defaultGravity;
    [SerializeField] float defaultAnimatorSpeed;

    [SerializeField] bool isGrounded;
    [SerializeField] bool canClimb;
    [SerializeField] bool isAlive = true;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        defaultGravity = rigidBody2D.gravityScale;
        defaultAnimatorSpeed = animator.speed;
        animator.SetBool("isAlive", isAlive);
    }

    private void Update()
    {
        if (!isAlive)
            return;

        HandleRun();
        HandleClimb();
        FlipSprite();
        HandleGroundDetection();
        HandleClimbableDetection();
    }

    private void OnMove(InputValue moveValue)
    {
        if (!isAlive)
            return;

        moveInput = moveValue.Get<Vector2>();
    }

    private void OnJump(InputValue jumpValue)
    {
        if (!isAlive)
            return;

        if (!isGrounded) { return; }

        if (jumpValue.isPressed)
        {
            rigidBody2D.velocity += new Vector2(0f, jumpForce);
        }
    }

    private void HandleClimb()
    {
        if (!isAlive)
            return;

        if (canClimb == true)
        {
            Vector2 climbVelocity = new Vector2(rigidBody2D.velocity.x, moveInput.y * climbSpeed);
            rigidBody2D.velocity = climbVelocity;
            rigidBody2D.gravityScale = 0f;
            bool HasVerticalMovement = Mathf.Abs(rigidBody2D.velocity.y) > Mathf.Epsilon;
            animator.SetBool("isClimbing", true);

            if (HasVerticalMovement == true)
            {
                animator.speed = defaultAnimatorSpeed;
            }
            else
            {
                animator.speed = 0f;
            }
        }
        else
        {
            rigidBody2D.gravityScale = defaultGravity;
            animator.SetBool("isClimbing", false);
            animator.speed = defaultAnimatorSpeed;
        }
    }

    private void HandleRun()
    {
        if (!isAlive)
            return;

        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, rigidBody2D.velocity.y);
        rigidBody2D.velocity = playerVelocity;

        bool HasHorizontalMovement = Mathf.Abs(rigidBody2D.velocity.x) > Mathf.Epsilon;

        animator.SetBool("isRunning", HasHorizontalMovement);
    }

    private void FlipSprite()
    {
        bool HasHorizontalMovement = Mathf.Abs(rigidBody2D.velocity.x) > Mathf.Epsilon;

        if (HasHorizontalMovement)
        {
            transform.localScale = new Vector2(Mathf.Sign(rigidBody2D.velocity.x), 1f);
        }
    }

    private void HandleGroundDetection()
    {
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void HandleClimbableDetection()
    {
        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Climable")))
        {
            canClimb = true;
        }
        else
        {
            canClimb = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            PlayerDeath();  
        }
    }

    private void PlayerDeath()
    {
        rigidBody2D.velocity += new Vector2(0f, jumpForce);
        isAlive = false;
        animator.SetBool("isRunning", false);
        animator.SetBool("isAlive", isAlive);
    }
}
