using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HWCameraChange : MonoBehaviour
{
    int camPos = 0;
    public List<GameObject> listOfCameras;
    public int current = 0;
    // Start is called before the first frame update
    void Start()
    {
        //listOfCameras = GameObject.Find("CameraList"):
         foreach (Transform child in GameObject.Find("CamerasList").transform)
         {
            listOfCameras.Add(child.gameObject);
         }

    }


    public void switchCamera()
    {
        listOfCameras[current].SetActive(false);
        if (current >= listOfCameras.Count-1)
        {
            current = 0;
        }
        else current++;
        listOfCameras[current % 5].SetActive(true);



    }
}
