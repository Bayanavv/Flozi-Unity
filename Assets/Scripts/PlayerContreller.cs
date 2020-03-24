﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerContreller : MonoBehaviour
{

    //Start() variables
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    

    //FSM (finite state machine)
    private enum State { idle, running, jumping, falling, hurt};
    private State state = State.idle;
    
    //Inspector variables
    [SerializeField]private LayerMask ground;
    [SerializeField]private float speed = 5f;
    [SerializeField]private float jumpForce = 5f;
    [SerializeField]private int cherris = 0;
    [SerializeField]private Text cherrisText;
    [SerializeField]private float hurtForce = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();

    }

    private void Update()
    {
        if(state != State.hurt)
        {
            Movement();
        }
        Movement();
        AnimetionState();
        anim.SetInteger("state", (int)state);//sets animation based on Enumenator state
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            cherris += 1;
            cherrisText.text = cherris.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemie")
        {
            if (state == State.falling)
            {
                Destroy(other.gameObject);
                Jump();
            }
            else
            {
                state = State.hurt;
                if(other.gameObject.transform.position.x > transform.position.x)
                {
                    //Enemy is to my tight therefore I should be damaged and move left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    //Enamy is to my left therefore I should be damaged and move right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
            
        }
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");

        // Moving Left
        if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);//change the image of the player left
        }

        // Moving Right
        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);//change the image of the player right
        }
        // Jumping
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))//jump
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    private void AnimetionState()
    {
        if(state == State.jumping)
        {
            if(rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }
        else if(state == State.falling)
        {
            if(coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }

        else if(state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                state = State.idle;
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 2f)
        {

            //moving
            state = State.running;
        }
        else
        {
            state = State.idle;
        }
        
    }
        
}
