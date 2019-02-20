using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach this script to a GameObject with a Rigidbody. Press the up and down keys to move the Rigidbody up and down.
//Press the space key to freeze all rotations, but notice the positions still change.

public class HWRotationSiege : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    Vector3 m_YAxis;
    float m_Speed;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //Set up vector for moving the Rigidbody in the y axis
        m_YAxis = new Vector3(0, 5, 0);
        m_Speed = 20.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Press space to add constraints on the RigidBody (freeze all rotations)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Freeze all rotations
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
