using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;

    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidBody;
    Animator animator;
    List<RaycastHit2D> castCollisions = new();
    public bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        if (canMove) {
            // If movement is not 0, try to move
            if (movementInput != Vector2.zero) {
                bool success = TryMove(movementInput);
                if (!success && movementInput.x > 0) {
                    success = TryMove(new Vector2(movementInput.x, 0));
                }
                if (!success && movementInput.y > 0) {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
                animator.SetBool("isMoving", success);
            }
            else animator.SetBool("isMoving", false);

            if (movementInput.x < 0 ) {
                spriteRenderer.flipX = true;
            }
            else if (movementInput.x > 0 ) {
                spriteRenderer.flipX = false;
            }
        }
    }

    private bool TryMove(Vector2 direction) {
        if (direction != Vector2.zero) {
            // Check for potential collisions
            int count = rigidBody.Cast(
                direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
                movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
                castCollisions, // List of collisions to store the found collisions into after the Cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset // The amount to cast equal to the movement plus an offset
            );
            if (count == 0) {
                rigidBody.MovePosition(rigidBody.position + moveSpeed * Time.fixedDeltaTime * direction);
                return true;
            } else return false;
        } else return false;
    }

    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() {
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack() {
        LockMovement();
        if (spriteRenderer.flipX == true) swordAttack.AttackLeft();
        else swordAttack.AttackRight();
    }

    public void EndSwordAttack() {
        UnlockMovement();
        swordAttack.StopAttack();
    }

    public void LockMovement() {
        canMove = false;
    }

    public void UnlockMovement() {
        canMove = true;
    }
}
