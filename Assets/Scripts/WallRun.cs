using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("General")]
    [SerializeField] float playerHeight;

    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Rigidbody rb;
    [SerializeField] PlayerMovement movement;


    [Header("Camera")]
    [SerializeField] Camera cam;
    [SerializeField] float fov;
    [SerializeField] float wallRunFOV;
    [SerializeField] float fovSpeed;
    [SerializeField] float camTilt;
    [SerializeField] float camTiltTime;
    [HideInInspector] public float tilt;

    bool decreaseFOV;

    [Header("Detection")]
    [SerializeField] float wallDist;
    [SerializeField] float minimumJumpHeight;

    [Header("Wall run")]
    [SerializeField] float wallRunGrav;
    [SerializeField] float wallRunJumpForce;
    [HideInInspector] public bool isWallRunning;

    [HideInInspector] public bool wallLeft = false;
    [HideInInspector] public bool wallRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

	private void Update()
	{
        CheckWall();
        if(wallLeft && CanWallRun())
		{
            isWallRunning = true;
            StartWallRun();
		}
        else if (wallRight && CanWallRun())
		{
            isWallRunning = true;
            StartWallRun();
		}
        else
		{
            isWallRunning = false;
            StopWallRun();
		}
	}

	void CheckWall()
	{
        wallLeft = Physics.Raycast(orientation.transform.position, -orientation.transform.right, out leftWallHit, wallDist);
        wallRight = Physics.Raycast(orientation.transform.position, orientation.transform.right, out rightWallHit, wallDist);
	}

    bool CanWallRun()
	{
        if (!movement.isGrounded && !movement.isCrouching)
        {
            return true;
        }
        return false;

    }

    void StartWallRun()
	{
        rb.useGravity = false;
        rb.AddForce(Vector3.down * wallRunGrav, ForceMode.Force);

        cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, wallRunFOV, fovSpeed * Time.deltaTime);

        if(wallLeft)
            tilt = Mathf.MoveTowards(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else if(wallRight)
            tilt = Mathf.MoveTowards(tilt, camTilt, camTiltTime * Time.deltaTime);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (wallLeft)
			{
                Vector3 wallRunJumpDir = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDir * wallRunJumpForce * 100, ForceMode.Force);
                decreaseFOV = true;
            } else if (wallRight)
			{
                Vector3 wallRunJumpDir = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDir * wallRunJumpForce * 100, ForceMode.Force);
                decreaseFOV = true;
			}
		}
	}

    void StopWallRun()
    {
        rb.useGravity = true;
        
		if (decreaseFOV)
		{
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, fov, fovSpeed * Time.deltaTime);
		}

        tilt = Mathf.MoveTowards(tilt, 0, camTiltTime * Time.deltaTime);

        if (cam.fieldOfView <= fov)
		{
            decreaseFOV = false;
		}
    }
}
