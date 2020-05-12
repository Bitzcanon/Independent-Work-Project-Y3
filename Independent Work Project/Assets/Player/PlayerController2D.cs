using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    private Animator animator; //To switch between animations
    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;

    private float gravity;

    private float maxJumpVelocity;
    private float minJumpVelocity;

    //Public variables
    public Transform groundCheckC;
    public Transform groundCheckL;
    public Transform groundCheckR;

    public float runSpeed = 7f;
    public float maxJumpHeight = 2f;
    public float minJumpHeight = 0.3f;
    public float timeToJumpApex = 0.4f; //How long it takes for player to reach highest point

    void Start()
    {
        isGrounded = true;

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2f);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Math.Abs(gravity) * minJumpHeight);

        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() //For physics updating
    {
        if ((Physics2D.Linecast(transform.position, groundCheckC.position, 1 << LayerMask.NameToLayer("Ground"))) ||
            (Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Ground"))) ||
            (Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Ground")))) //If it hits a layer called ground
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            animator.Play("player_jump");
        }

        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            rb2D.velocity = new Vector2(runSpeed, rb2D.velocity.y);

            if (isGrounded)
                animator.Play("player_run");

            spriteRenderer.flipX = false;
        }
        else if (Input.GetKey("a") || Input.GetKey("left"))
        {
            rb2D.velocity = new Vector2(-runSpeed, rb2D.velocity.y);

            if (isGrounded)
                animator.Play("player_run");

            spriteRenderer.flipX = true;
        }
        else
        {
            if (isGrounded)
                animator.Play("player_idle");

            rb2D.velocity = new Vector2(0, rb2D.velocity.y); //Reset horizontal velocity
        }

        if (Input.GetKeyDown("space") && isGrounded)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, maxJumpVelocity);
            animator.Play("player_jump");
        }
        if (Input.GetKeyUp("space"))
        {
            if (rb2D.velocity.y > minJumpVelocity)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, minJumpVelocity);
            }
        }
    }

}
