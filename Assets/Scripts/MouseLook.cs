using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera cam;
    [SerializeField] Transform orientation;
    [SerializeField] WallRun wallRun;

    [Header("Input")]
    float mouseX;
    float mouseY;

    [Header("Sensitivity")]
    [SerializeField] float sensX;
    [SerializeField] float sensY;

    float xRot;
    float yRot;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRot += mouseX * sensX;
        xRot -= mouseY * sensY;

        xRot = Mathf.Clamp(xRot, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRot, yRot, wallRun.tilt);
        orientation.transform.localRotation = Quaternion.Euler(0, yRot, 0);
    }
}
