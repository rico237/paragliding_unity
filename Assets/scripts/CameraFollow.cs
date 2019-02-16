using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform playerTransform;
    public float smoothing = 5f;
    private Vector3 offset;

	void Start () 
    {
        offset = transform.position - playerTransform.position;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetCamPos = playerTransform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
