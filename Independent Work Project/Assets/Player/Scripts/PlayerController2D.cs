using System;
using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    private PhotonView PV;

    private Animator animator; //To switch between animations
    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;

    //Jumping
    private bool isGrounded;
    private float jumpTimeCounter;
    private bool isJumping;

    private float jumpPressedRemember = 0;
    private float groundedRemember = 0;

    //Attacking
    private bool isAttacking = false;
    private float timeBetweenAttack;

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

    //Attacking
    public float startTimeBetweenAttack;
    public Transform strikeAttackPosition;
    public Transform jumpAttackPosition;
    public LayerMask whatIsEnemies;
    public float strikeAttackRangeX;
    public float strikeAttackRangeY;
    public float jumpAttackRange;
    public int damage;

    void Start()
    {
        PV = GetComponent<PhotonView>();

        isGrounded = true;

        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        if (timeBetweenAttack <= 0)
        {
            if (Input.GetButtonDown("Fire1") && !isAttacking)
            {
                isAttacking = true;

                PV.RPC("RPC_Attacking", RpcTarget.All);
                ////Animations
                //if (!isGrounded)
                //{
                //    animator.Play("knight_jumpattack");
                //    StartCoroutine(DoBasicAttack(0.4f, "jumpattack"));
                //}
                //else
                //{
                //    animator.Play("knight_strike");
                //    StartCoroutine(DoBasicAttack(0.3f, "strike"));
                //}
            }
        }
        else
            timeBetweenAttack -= Time.deltaTime;
    }

    [PunRPC]
    void RPC_Attacking()
    {
        //Animations
        if (!isGrounded)
        {
            animator.Play("knight_jumpattack");
            StartCoroutine(DoBasicAttack(0.4f, "jumpattack"));
        }
        else
        {
            animator.Play("knight_strike");
            StartCoroutine(DoBasicAttack(0.3f, "strike"));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(strikeAttackPosition.position, new Vector3(strikeAttackRangeX, strikeAttackRangeY, 1));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(jumpAttackPosition.position, jumpAttackRange);
    }

    IEnumerator DoBasicAttack(float attackDuration, string attackType)
    {
        //For hitting enemies

        if (attackType == "jumpattack")
        {
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(strikeAttackPosition.position, jumpAttackRange, whatIsEnemies);
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                enemiesToDamage[i].GetComponent<PlayerSetup>().TakeDamage(damage); //Reduce HP
            }
        }
        else if (attackType == "strike")
        {
            Collider2D[] enemiesToDamage = Physics2D.OverlapBoxAll(strikeAttackPosition.position, new Vector2(strikeAttackRangeX, strikeAttackRangeY), 0, whatIsEnemies);
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                enemiesToDamage[i].GetComponent<PlayerSetup>().TakeDamage(damage); //Reduce HP
            }
        }
        timeBetweenAttack = startTimeBetweenAttack;

        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    private void FixedUpdate() //For physics updating
    {
        if (PV.IsMine)
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

                if (!isAttacking)
                    animator.Play("knight_jump");
            }

            if (Input.GetKey("d") || Input.GetKey("right"))
            {
                rb2D.velocity = new Vector2(runSpeed, rb2D.velocity.y);

                if (isGrounded && !isAttacking)
                    animator.Play("knight_walk");

                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); //Turn whole player to the right
            }
            else if (Input.GetKey("a") || Input.GetKey("left"))
            {
                rb2D.velocity = new Vector2(-runSpeed, rb2D.velocity.y);

                if (isGrounded && !isAttacking)
                    animator.Play("knight_walk");

                transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f); //Turn whole player to the left
            }
            else if (isGrounded)
            {
                if (!isAttacking)
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
                //animator.Play("knight_jump");
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
}
