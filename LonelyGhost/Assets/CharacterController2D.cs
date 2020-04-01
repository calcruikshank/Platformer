using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{

    public Rigidbody2D rb;
    public Vector3 direction;
    public float dashSpeed = 40f;
    float horizontalMove = 0f;
    public ControllerExtension controller;
    public float runSpeed = 40f;
    bool jump = false;
    bool dashing = false;
    public float numOfDashes = 2f;
    public float timesDashed = 0f;

    private State state;
    private enum State
    {
        Normal,
        Dashing, 
        Jumping
    }

    void Awake()
    {
        state = State.Normal;
    }

    void Update()
    {

        switch (state)
        {
            case State.Normal:
                HandleMovement();
                HandleDash();
                break;
            case State.Dashing:
                HandleDashing();
                break;
            case State.Jumping:
                Jumping();
                break;
        }

        

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
    }


    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;

        if (dashing)
        {
            transform.position += direction * dashSpeed * Time.deltaTime;
        }
    }

    void HandleMovement()
    {
        if (Input.GetButtonDown("Jump"))
        {
            state = State.Jumping;
            
        }

        if (controller.m_Grounded)
        {
            timesDashed = 0;
        }
    }

    void Jumping()
    {
        jump = true;
        state = State.Normal;
    }

    void HandleDash()
    {
        if (Input.GetButtonDown("Fire2") && timesDashed < numOfDashes)
        {
            rb.velocity = new Vector2(0, 0); //zero out velcity
            state = State.Dashing;
            dashSpeed = 40;
            timesDashed++;

            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);


            direction = new Vector3(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);  //equal to the difference of current mouse position and transform
            direction = direction.normalized;
        }
    }

    void HandleDashing()
    {
        
        dashing = true;
        //Dash();
        dashSpeed -= dashSpeed * 7f * Time.deltaTime;


        if (dashSpeed < 4f)
        {
            state = State.Normal;
            dashing = false;
            
        }

    }
}
