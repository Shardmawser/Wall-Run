using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("General")]
    [SerializeField] float playerHeight;

    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform feet;
    [SerializeField] Transform orientation;
    [SerializeField] WallRun wallRun;

    [Header("Camera")]
    [SerializeField] Camera cam;
    [SerializeField] float FOV;
    [SerializeField] float sprintFOV;
    [SerializeField] float sprintFOVTime;
    [SerializeField] float minusSprintFOVTime;
    [SerializeField] float crouchFOV;
    [SerializeField] float crouchFOVSpeed;


    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float movementMultiplier;
    [SerializeField] float airMultiplier;
    [SerializeField] float sprintSpeed;
    float walkSpeed;
    float verticalMovement;
    float horizontalMovement;
    bool isSprinting;
    bool isMoving;

    [Header("Sliding")]
    [SerializeField] float slideForce;
    bool isSliding = false;

    [Header("Crouching")]
    [SerializeField] float crouchSize;
    [SerializeField] float crouchMultiplier;
    [SerializeField] float crouchPositionOffset;
    public bool isCrouching;

    [Header("Drag")]
    [SerializeField] float groundDrag;
    [SerializeField] float airDrag;
    [SerializeField] float wallRunDrag;

    [Header("Jumping")]
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float groundDist;
    public bool isGrounded;
    [SerializeField] float jumpForce;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;


    Vector3 moveDir;
    Vector3 slopeMoveDir;

    RaycastHit slopeHit;

	private void Start()
	{
        walkSpeed = moveSpeed;
	}

	private bool OnSlope()
	{
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
		{
            if (slopeHit.normal != Vector3.up)
                return true;
            else
                return false;
		}
        return false;
	}

    void Update()
    {
        isGrounded = Physics.CheckSphere(feet.position, groundDist, whatIsGround);
        MyInput();
        ControlDrag();
        ControlFOV();
        ControlSpeed();
        ControlSound();
        if (Input.GetKeyDown(jumpKey) && isGrounded && !isCrouching)
        {
            Jump();
        } else if (Input.GetKeyDown(jumpKey) && isGrounded && isCrouching)
		{
            Crouch();
		}
		if (isSliding && isGrounded)
		{
			Slide();
		}


		slopeMoveDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);
    }

	private void ControlSound()
	{
        if(!isCrouching && !wallRun.wallLeft && isGrounded || !isCrouching && !wallRun.wallRight && isGrounded)
		{
		    if (isMoving && !isSprinting)
		    {
                AudioManager.Instance.Stop("Run");
                AudioManager.Instance.Play("Walk");
		    } else if (isSprinting && isMoving)
		    {
                AudioManager.Instance.Stop("Walk");
                AudioManager.Instance.Play("Run");
			}
		    else
		    {
                AudioManager.Instance.Stop("Walk");
                AudioManager.Instance.Stop("Run");
		    }

		} else if(wallRun.wallLeft || wallRun.wallRight)
		{
            AudioManager.Instance.Stop("Walk");
            AudioManager.Instance.Play("Run");
        }
        else
        {
            AudioManager.Instance.Stop("Walk");
            AudioManager.Instance.Stop("Run");
        }
    }

	void Slide()
	{
        /*AudioManager.Instance.Stop("Walk");
        AudioManager.Instance.Stop("Run");
        AudioManager.Instance.Play("Slide");*/

        transform.localScale = new Vector3(transform.localScale.x, crouchSize, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y - (crouchSize + crouchPositionOffset), transform.position.z);
        if (isGrounded)
		{
		    rb.AddForce(orientation.transform.forward * slideForce, ForceMode.Impulse);
		}
		else
		{
            return;
		}
		isSliding = false;
        Crouch();
	}

	void MyInput()
	{
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDir = orientation.transform.forward * verticalMovement + orientation.transform.right * horizontalMovement;

        if(Input.GetKeyDown(crouchKey) && isSprinting && !isCrouching)  
		{
            isSliding = true;
		}
        if (Input.GetKeyDown(crouchKey) && isGrounded && !isSliding)
        {
            Crouch();
        }

        if(horizontalMovement != 0 || verticalMovement != 0)
            isMoving = true;
		else
            isMoving = false;
    }

	private void FixedUpdate()
	{
        MovePlayer();
	}

    void Crouch()
    {
        if (!isCrouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchSize, transform.localScale.z);
            transform.position = new Vector3(transform.position.x, transform.position.y - (crouchSize + crouchPositionOffset), transform.position.z);
            isCrouching = true;
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, 1.71f, transform.localScale.z);
            transform.position = new Vector3(transform.position.x, transform.position.y + (crouchSize + crouchPositionOffset), transform.position.z);
            isCrouching = false;
        }
    }

    void MovePlayer()
	{

        if (isGrounded && !OnSlope() && !isCrouching)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope() && !isCrouching)
        {
            rb.AddForce(slopeMoveDir.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isCrouching)
		{
            rb.AddForce(slopeMoveDir.normalized * moveSpeed * crouchMultiplier, ForceMode.Acceleration);
		}
        else if (!isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }

    }

    

    void ControlDrag()
	{
        if (isGrounded)
            rb.drag = groundDrag;
        else if (!wallRun.isWallRunning)
            rb.drag = airDrag;
        else
            rb.drag = wallRunDrag;
	}

    void Jump()
	{
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
	}

    void ControlSpeed()
	{
		
        if (Input.GetKey(sprintKey) && isGrounded && !isCrouching)
		{
            moveSpeed = sprintSpeed;
            isSprinting = true;
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, sprintFOV, sprintFOVTime * Time.deltaTime);
        }
        else if (!Input.GetKey(sprintKey) && !isCrouching)
		{
			moveSpeed = walkSpeed;
            isSprinting = false;
            if (wallRun.wallLeft || wallRun.wallRight)
                return;
             cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, FOV, (sprintFOVTime - minusSprintFOVTime) * Time.deltaTime);
        }
        else if (isCrouching)
		{
            isSprinting = false;
		}
            
	}

    void ControlFOV()
	{
		if (isCrouching)
		{
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, crouchFOV, crouchFOVSpeed * Time.deltaTime);
		}
	}
}
