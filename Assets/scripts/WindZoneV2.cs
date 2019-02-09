using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZoneV2 : MonoBehaviour {
    public List<CharacterController> rigidbodies = new List<CharacterController>();
    public Vector3 windDirection = Vector3.right;
    public float windStrenght = 5;


    private void OnTriggerEnter(Collider other)
    {
        CharacterController obj = other.gameObject.GetComponent<CharacterController>();
        if(obj != null)
        {
            Debug.Log("Salut");
            rigidbodies.Add(obj);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterController obj = other.gameObject.GetComponent<CharacterController>();
        if (obj != null)
        {
            Debug.Log("a plus");
            rigidbodies.Remove(obj);
        }
    }

    private void FixedUpdate()
    {
        foreach (CharacterController elem in rigidbodies)
        {
            //elem.controller.Move(elem.moveDirection + (windDirection * windStrenght * Time.deltaTime));
            elem.moveDirection = elem.moveDirection + (windDirection * windStrenght * Time.deltaTime);
        }
    }
}
