using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlCamera : MonoBehaviour {

  private float distance = 40f;
  private Vector2 oldPosition1;
  private Vector2 oldPosition2;
  private bool isRotate;
  public GameObject player;
  private float rotateSpeed = 20f;

  // Use this for initialization
  void Start () {
	}

  
  public void Enlarge()
  {
    if (Camera.main.fieldOfView <= 150)
      this.GetComponent<Camera>().fieldOfView += 4f;

    if (Camera.main.orthographicSize <= 20)
      this.GetComponent<Camera>().orthographicSize += 0.5f;

  }

  public void Reduce()
  {
    if (this.GetComponent<Camera>().fieldOfView > 20)
      this.GetComponent<Camera>().fieldOfView -= 4f;
    if (this.GetComponent<Camera>().orthographicSize >= 1)
      this.GetComponent<Camera>().orthographicSize -= 0.5f;
  }


  //determine enlarge or not
  bool isEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
  {
    
    var leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
    var leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
    if (leng1 < leng2)
    {
      
      return true;
    }
    else
    {
      
      return false;
    }
  }

  void RotateView()
  {
    if (Input.GetMouseButtonDown(1))
      isRotate = true;
    if (Input.GetMouseButtonUp(1))
      isRotate = false;
    if(Input.touchCount == 1)
    {
      rotateSpeed = 7f;
      if (Input.touches[0].phase == TouchPhase.Moved)
        isRotate = true;
      else
        isRotate = false;
    }
    //if (Input.touches[0].phase == TouchPhase.Ended)
      //isRotate = false;

    if (isRotate)
    {
      Vector3 originalPosition = this.transform.position;
      Quaternion originalRotation = this.transform.rotation;
      transform.RotateAround(player.transform.position, Vector3.up, rotateSpeed * Input.GetAxis("Mouse X"));
      transform.RotateAround(player.transform.position, transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));
      float x = transform.eulerAngles.x;
      if(x < 10 || x > 70)
      {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
      }
      //Debug.Log(transform.position);
    }
  }


  // Update is called once per frame
  void Update()
  {
    RotateView();

    if (Input.GetAxis("Mouse ScrollWheel") < 0)
        Enlarge();

    if (Input.GetAxis("Mouse ScrollWheel") > 0)
        Reduce();

    //multiple finger touch
    if (Input.touchCount > 1 && Input.touches[0].phase != TouchPhase.Stationary && Input.touches[1].phase != TouchPhase.Stationary)
    {
      // two fingers move touch
      if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
      {
        // get the position of finger
        Vector2 tempPosition1 = Input.GetTouch(0).position;
        Vector2 tempPosition2 = Input.GetTouch(1).position;
        
        if (isEnlarge(oldPosition1, oldPosition2, tempPosition1, tempPosition2))
        {

          distance -= 20f * Time.deltaTime;
          if (distance <= 20)
          {
            distance = 20;

          }
          Camera.main.fieldOfView = distance;
        }
        else
        {

          distance += 20f * Time.deltaTime;

          if (distance >= 150f)
          {
            distance = 150;
          }
          Camera.main.fieldOfView = distance;

        }
        
        oldPosition1 = tempPosition1;
        oldPosition2 = tempPosition2;

      }
    }

    
  }

}
