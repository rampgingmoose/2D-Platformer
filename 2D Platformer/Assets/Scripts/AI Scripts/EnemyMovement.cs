using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Rigidbody2D rigidBody2D;
    CapsuleCollider2D capsuleCollider2D;
    BoxCollider2D boxCollider2D;
    Animator animator;

    PlayerMovement playerPos;

    [SerializeField] float moveSpeed = 20.0f;
    [SerializeField] float attackRange = 5.0f;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        playerPos = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        HandleMovement();
        FlipSpriteOnTouchingPlayer();
        CheckDistanceToPlayer();
    }

    private void HandleMovement()
    {
        Vector2 enemyVelocity = new Vector2(moveSpeed, rigidBody2D.velocity.y);
        rigidBody2D.velocity = enemyVelocity;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        moveSpeed = -moveSpeed;
        FlipSprite();
    }

    private void FlipSpriteOnTouchingPlayer()
    {
        if (capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            moveSpeed = -moveSpeed;
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        transform.localScale = new Vector2(Mathf.Sign(moveSpeed), 1f);
    }

    private void CheckDistanceToPlayer()
    {
        Vector3 attackOffset = transform.position - playerPos.transform.position;
        float distanceToAttack = attackOffset.sqrMagnitude;

        if (distanceToAttack <= attackRange)
        {
            animator.SetBool("isAttacking", true);
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }
    }
}
