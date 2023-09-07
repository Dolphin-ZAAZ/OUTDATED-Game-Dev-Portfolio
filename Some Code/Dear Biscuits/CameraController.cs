using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform truck;

    [Range(0.05f, 10f)]public float sensitivity = 0.25f;

    private float mouseX;
    private float mouseY;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mouseX = 180;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetPosition();
        GetMouseInput();
        SetRotation();
    }

    
    private void SetPosition()
    {
        this.transform.position = new Vector3(truck.transform.position.x, truck.transform.position.y, truck.transform.position.z);
    }
    private void GetMouseInput()
    {
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivity;
        mouseY = Mathf.Clamp(mouseY, -25f, 65f);
    }
    private void SetRotation()
    { 
        this.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }

}
