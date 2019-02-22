using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HWCameraChange : MonoBehaviour
{
    Camera[] listOfCameras;
    int camPos = 0;
    // Start is called before the first frame update
    void Start()
    {
        listOfCameras = GetComponents<Camera>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void switchCamera()
    {

    }
}
