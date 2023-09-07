using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 1f;
    float tilt = 0f;
    float tiltx = 0f;

    PhotonView view;
    // Start is called before the first frame update
    void Start()
    {
        //playerBody = GameObject.Find("Player(Clone)").GetComponent<Transform>();
        //view = GetComponentInParent<PhotonView>();
        //if (view.IsMine)
        //{
            Cursor.lockState = CursorLockMode.Locked;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //if (view.IsMine)
        //{
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            tilt -= mouseY;
            tiltx += mouseX;
            tilt = Mathf.Clamp(tilt, -90, 90);
            //playerBody.Rotate(Vector3.up * mouseX, Space.Self);
            //this.transform.Rotate(Vector3.up * mouseX, Space.Self);
            this.transform.localRotation = Quaternion.Euler(tilt, 0, 0);
        //}
    }
}
