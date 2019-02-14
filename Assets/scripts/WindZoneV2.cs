using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZoneV2 : MonoBehaviour {
    public List<controller> rigidbodies = new List<controller>();
    public Vector3 windDirection = Vector3.right;
    public float windStrenght = 5;


    private void OnTriggerEnter(Collider other)
    {
        controller obj = other.gameObject.GetComponent<controller>();
        if(obj != null)
        {
            Debug.Log("Salut");
            rigidbodies.Add(obj);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        controller obj = other.gameObject.GetComponent<controller>();
        if (obj != null)
        {
            Debug.Log("a plus");
            rigidbodies.Remove(obj);
        }
    }

    private void FixedUpdate()
    {
        foreach (controller elem in rigidbodies)
        {
            //elem.controller.Move(elem.moveDirection + (windDirection * windStrenght * Time.deltaTime));
            elem.moveDirection = elem.moveDirection + (windDirection * windStrenght * Time.deltaTime);
        }
    }
}
