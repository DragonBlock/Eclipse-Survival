using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingMovement : MonoBehaviour
{
    // Michael's Edit:
    // Creating this class so it can be merged with the other movement script.
    // Still haven't decided if that's feasible or neccesary.

    [Header("Set in Inspector")]
    // May add different variables for walk/run for climbing areas 
    public float walkSpeed;
    public float runSpeed;
    public float climbSpeed;

    // Added variables for detection radius & jump height/power
    public float defaultRadius;
    public float walkRadius;
    public float runRadius;
    public float jumpPower;
    public float jumpCooldown;
    public float gravity;

    // For distinguishing circle colliders
    public GameObject detectCircle;

    [Header("Set Dynamically")]
    public Facing facing;
    private Rigidbody2D rb;
    private float moveSpeed; // Remove and replace with run detection (To do for code review)
    CircleCollider2D detectionCollider;
    // Collider for detecting ground/Jump
    CircleCollider2D collisionCollider;
    bool climbMode;
    bool onClimbable;
    bool climbing;
    // To Do: Implement ground detection
    bool onGround;
    float jumpTime;

    [HideInInspector]
    public bool OnClimbable { get{ return onClimbable; } set{ onClimbable = value; } }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facing = Facing.Left;
        collisionCollider = gameObject.GetComponent<CircleCollider2D>();

        detectionCollider = detectCircle.GetComponent<CircleCollider2D>();
        detectionCollider.radius = defaultRadius;

        // Set gravity here in inspector temporarily (Easier than going into settings every single time)
        Physics2D.gravity = new Vector2(0, -gravity);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && onGround)
        {
            rb.AddForce(new Vector2(0f, jumpPower));
            onGround = false;
            jumpTime = 0f;
            if (climbing)
            {
                climbing = false;
                Physics2D.gravity = new Vector2(0, -gravity);
            }
        }
        else if (!onGround)
        {
            jumpTime += Time.deltaTime;
            if (jumpCooldown <= jumpTime)
            {
                onGround = true;
            }
        }

        Vector2 pVel = rb.velocity;
        pVel.x = 0f;
        if(climbing) pVel.y = 0f;

        // Adding running
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = runSpeed;
            detectionCollider.radius = runRadius;
        }
        else if (moveSpeed != walkSpeed)
        {
            moveSpeed = walkSpeed;
            detectionCollider.radius = walkRadius;
        }

        // NOTE:
        // Climbing on climbable background enviroments was assumed
        // This can be easily changed if we want to climb on sidewalls or similar things later
        // If we do not want sideways movement/or only limited sideways movement that can be changed easily later too

        if (climbing && !onClimbable)
        {
            Physics2D.gravity = new Vector2(0, -gravity);
            climbing = false;
        }
                

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            // Place Ducking here if we want

            if (climbing)
            {
                pVel.y -= climbSpeed * Time.deltaTime;
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            pVel.x = moveSpeed * Time.deltaTime;

            if (facing != Facing.Right)
            {
                facing = Facing.Right;
                TurnXander();
            }
        }
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            if (onClimbable && !climbing)
            {
                climbing = true;
                Physics2D.gravity = new Vector2(0, 0f);
                facing = Facing.Up;
                TurnXander();
            }
            if (climbing)
            {
                pVel.y = climbSpeed * Time.deltaTime;
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            pVel.x = -moveSpeed * Time.deltaTime;

            if (facing != Facing.Left)
            {
                facing = Facing.Left;
                TurnXander();
            }
        }
        else
        {
                
        }


        // To Do: Cap speed so that nothing is crazy/either that or long falls kill/damage Xander

        rb.velocity = pVel;
        
    }

    private void TurnXander()
    {
        if (climbing)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 180);
            facing = Facing.Up;
            return;
        }

        switch (facing)
        {
            case Facing.Up:
                transform.localRotation = Quaternion.Euler(0, 0, 180);
                break;
            case Facing.Down:
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case Facing.Left:
                transform.localRotation = Quaternion.Euler(0, 0, -90);
                break;
            case Facing.Right:
                transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
        }
    }
}
