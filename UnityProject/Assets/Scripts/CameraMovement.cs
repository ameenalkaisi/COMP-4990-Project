using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float slowMove = 50f;
    public float normalMove = 100f;
    public float fastMove = 250f;
    public float scrollSpeed = 10f;

    private float currentMoveSpeed;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            currentMoveSpeed = fastMove;
        else if (Input.GetKey(KeyCode.LeftControl))
            currentMoveSpeed = slowMove;
        else currentMoveSpeed = normalMove;

        // WASD controls, SHIFT for fast, CTRL for slow
        if(Input.GetKey(KeyCode.W))
            transform.position += new Vector3(0f, currentMoveSpeed, 0f) * Time.deltaTime;

        if(Input.GetKey(KeyCode.A))
            transform.position -= new Vector3(currentMoveSpeed, 0f, 0f) * Time.deltaTime;

        if(Input.GetKey(KeyCode.S))
            transform.position -= new Vector3(0f, currentMoveSpeed, 0f) * Time.deltaTime;

        if(Input.GetKey(KeyCode.D))
            transform.position += new Vector3(currentMoveSpeed, 0f, 0f) * Time.deltaTime;

        // Zooming controls
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            GetComponent<Camera>().orthographicSize -= scrollSpeed;
        
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            GetComponent<Camera>().orthographicSize += scrollSpeed;
    }
}
