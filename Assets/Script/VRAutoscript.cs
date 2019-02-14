using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAutoscript : MonoBehaviour {

    public GameObject audiows;
    public static float speed = 3.0F;
    public static bool moveforward;
    private CharacterController controller;
    private GvrViewer gvrviewer;
    private Transform vrHead;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        gvrviewer = transform.GetChild(0).GetComponent<GvrViewer>();
        //vrHead = Camera.main.transform;
      
    }

    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            moveforward = !moveforward;
      
        }
        if (moveforward)
        {
          

            speed = 3.0f;
            Vector3 forward = vrHead.TransformDirection(Vector3.forward);
            //controller.SimpleMove(forward * speed);
        }
    }
}
