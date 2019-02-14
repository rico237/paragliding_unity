using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// The GameObject is made to bounce using the space key.
// Also the GameOject can be moved forward/backward and left/right.
// Add a Quad to the scene so this GameObject can collider with a floor.

public class controller : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float joyconMagn = 2.0f;

    JoyconManager joyconManager;
    List<Joycon> joycons;

    public Vector3 moveDirection = Vector3.zero;
    public UnityEngine.CharacterController ctrl;

    private Vector3 previous;
    private float velocity;

    void Start()
    {
        ctrl = GetComponent<UnityEngine.CharacterController>();

        joyconManager = JoyconManager.Instance;
        joycons = JoyconManager.Instance.j;

        // let the gameObject fall down
        //gameObject.transform.position = new Vector3(0, 5, 0);
    }

    void Update()
    {
        velocity = ((transform.position - previous).magnitude) / Time.deltaTime;
        previous = transform.position;

        if (joycons.Count != 0)
        {
            foreach (Joycon joycon in joycons)
            {
                if (joycon != null)
                {

                    if (ctrl.isGrounded)
                    {
                        // We are grounded, so recalculate
                        // move direction directly from axes

                        if (joycon.GetButton(Joycon.Button.SHOULDER_2) && joycon.GetAccel().magnitude >= joyconMagn)
                        {
                            // Joycon has no magnetometer, so it cannot accurately determine its yaw value. Joycon.Recenter allows the user to reset the yaw value.
                            joycon.Recenter();

                            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 1.0f);
                        }
                        else
                        {
                            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
                        }

                        moveDirection = transform.TransformDirection(moveDirection);
                        moveDirection = moveDirection * speed;

                        if (Input.GetButton("Jump"))
                        {
                            moveDirection.y = jumpSpeed;
                        }

                    }
                    else
                    {
                        Debug.Log(moveDirection.y);
                        moveDirection = new Vector3(Input.GetAxis("Horizontal") * (10 + Mathf.Abs(moveDirection.y)), moveDirection.y, moveDirection.z);
                    }

                }

            }
        }
        else
        {
            if (ctrl.isGrounded)
            {
                // We are grounded, so recalculate
                // move direction directly from axes

                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection = moveDirection * speed;

                if (Input.GetButton("Jump"))
                {
                    moveDirection.y = jumpSpeed;
                }

            }
            else
            {
                Debug.Log(moveDirection.y);
                moveDirection = new Vector3(Input.GetAxis("Horizontal") * (10 + Mathf.Abs(moveDirection.y)), moveDirection.y, moveDirection.z);
            }
        }

        // Apply gravity
        moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);

        // Move the controller
        ctrl.Move(moveDirection * Time.deltaTime);
    }
}