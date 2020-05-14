using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    private Animator animator; //To switch between animations
    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;

    //Jumping
    private bool isGrounded;
    private float jumpTimeCounter;
    private bool isJumping;

    private float jumpPressedRemember = 0;
    private float groundedRemember = 0;

    //Public variables
    public Transform groundCheckC;
    public Transform groundCheckL;
    public Transform groundCheckR;

    //Movement
    public float runSpeed = 7f;
    public float jumpForce = 5f;
    public float jumpTime;

    //Player slack
    public float jumpSlackTimer = 0.3f; //Allow player slack in jump timings for smoother controls
    public float groundedSlackTimer = 0.15f; //Allow player slack in jumping off platforms

    void Start()
    {
        isGrounded = true;

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
            animator.Play("knight_jump");
        }

        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            rb2D.velocity = new Vector2(runSpeed, rb2D.velocity.y);

            if (isGrounded)
                animator.Play("knight_walk");

            spriteRenderer.flipX = false;
        }
        else if (Input.GetKey("a") || Input.GetKey("left"))
        {
            rb2D.velocity = new Vector2(-runSpeed, rb2D.velocity.y);

            if (isGrounded)
                animator.Play("knight_walk");

            spriteRenderer.flipX = true;
        }
        else
        {
            if (isGrounded)
                animator.Play("knight_idle");

            rb2D.velocity = new Vector2(0, rb2D.velocity.y); //Reset horizontal velocity
        }

        jumpPressedRemember -= Time.deltaTime;
        if (Input.GetKeyDown("space")) //Setting the jump recently float value
        {
            jumpPressedRemember = jumpSlackTimer;
        }

        groundedRemember -= Time.deltaTime;
        if (isGrounded)
        {
            groundedRemember = groundedSlackTimer;
        }

        if ((jumpPressedRemember >= 0f) && (groundedRemember >= 0f)) //Recently jumped or recently grounded checks before jumping
        {
            jumpPressedRemember = 0f;
            groundedRemember = 0f;

            isJumping = true;
            jumpTimeCounter = jumpTime;

            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
            animator.Play("knight_jump");
        }
        //For variable jump height
        if (Input.GetKey("space") && isJumping) //Prevent double jump with isJumping check
        {
           if (jumpTimeCounter > 0)
           {
                rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
           }
           else
           {
                isJumping = false;
           }
        }

        if (Input.GetKeyUp("space"))
        {
            isJumping = false;
        }
    }
}
