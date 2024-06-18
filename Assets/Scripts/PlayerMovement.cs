using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    // --- REFERENCES ---
    [HideInInspector]
    public Transform cam;
    Rigidbody rb;
    CapsuleCollider col;
    // --- REFERENCES ---


    // --- PARAMETERS ---
    [Header("Rotation")]
    public float sensitivity = 1;

    [Header("Movement")]
    public float speed = 5;
    [Range(0, 90)]
    public float maxGroundAngle = 45;
    public float jumpHeight = 1.5f;
    float jumpVelocity {
        get { return (0.009f + Mathf.Sqrt(0.00007225f + (0.20392f * jumpHeight))) / 0.10196f; }
    }
    public float airControl = 5;

    [Header("Collider")]
    public LayerMask groundLayers;
    // --- PARAMETERS ---


    // --- PLAYER STATE ---
    public Vector3 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }
    public Vector3 velocity
    {
        get { return rb.velocity; }
        set { rb.AddForce(value - velocity, ForceMode.VelocityChange); }
    }
    public Vector2 rotation
    {
        get { return cam.eulerAngles - new Vector3(((cam.eulerAngles.x > 90) ? 360 : 0), 0); }
        set
        {
            cam.localEulerAngles = new Vector3(Mathf.Clamp(value.x, -89.9f, 89.9f), 0, 0);
            transform.eulerAngles = new Vector3(0, value.y, 0);
        }
    }
    [HideInInspector] public bool isGrounded;
    Quaternion groundDir;
    public bool isRagdolled = false;
    // --- PLAYER STATE ---



    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            cam = transform.Find("Camera");
            rb = GetComponent<Rigidbody>();
            col = GetComponent<CapsuleCollider>();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Jump()
    {
        // Set velocity to jumpVelocity if player is moving slower than that vertically.
        velocity = new Vector3(velocity.x, Mathf.Max(velocity.y, jumpVelocity), velocity.z);
    }

    float value = 0;

    void Update()
    {
        if (isRagdolled)
            return;

        value += Time.deltaTime * 90;
        // Update rotation based on player's mouse movement.
        rotation += Inputs.mouseDelta * sensitivity;

    }

    void FixedUpdate()
    {
        if (Inputs.isJumping)
            isRagdolled = false;

        if (isRagdolled)
        {
            return;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Update grounded state and whether or not to use gravity on rigidbody.
        TestGrounded();

        Move();

        // Make player jump if they are trying to and are on the ground.
        if (isGrounded && Inputs.isJumping)
            Jump();
    }

    public void Move()
    {
        // Get target speed based on player's input and the player's speed value.
        Vector3 targetSpeed = (transform.rotation * new Vector3(Inputs.moveDir.x, 0, Inputs.moveDir.y)) * speed;

        // If player is grounded, align the target speed with the ground normal.
        if (isGrounded)
        {
            velocity = groundDir * targetSpeed;
        }
        // Only move the player while in the air if they are trying to move.
        else if (targetSpeed.magnitude > 0)
        {
            // Get the current horizontal velocity, clamped to a minimum of the speed value.
            float airSpeed = Mathf.Max(new Vector2(velocity.x, velocity.z).magnitude, speed);

            // Set the target horizontal speed to airSpeed.
            targetSpeed.y = 0;
            targetSpeed = targetSpeed.normalized * airSpeed;
            
            // Nullify target's y value to preserve vertical velocity.
            targetSpeed.y = velocity.y;

            // Move velocity towards the target speed based on air control.
            velocity = Vector3.MoveTowards(velocity, targetSpeed, speed * airControl * Time.fixedDeltaTime);
        }
    }

    void TestGrounded()
    {
        // Perform checks based on velocity and object detection.
        bool velocityCheck = velocity.y < speed;
        bool sphereCheck = Physics.SphereCast(position + (Vector3.up * col.radius), col.radius * 0.99f, Vector3.down, out RaycastHit hit, 0.2f, groundLayers);
        
        // Mark player as grounded if they pass the previous checks AND if the ground normal isn't too steep.
        isGrounded = velocityCheck && sphereCheck && (Vector3.Angle(Vector3.up, hit.normal) <= maxGroundAngle);

        // Update variables according to whether or not player is grounded.
        rb.useGravity = !isGrounded;

        if (isGrounded)
        {
            groundDir = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // If the player is on a rigidbody, apply force to said rigidbody to accomodate for no gravity.
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(Physics.gravity * rb.mass, hit.point, ForceMode.Force);
            }
        }
    }

    void Ragdoll()
    {
        isRagdolled = true;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
    }
}