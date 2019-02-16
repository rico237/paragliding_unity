using Newtonsoft.Json.Linq;
using SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Receiver : MonoBehaviour
{

  public SocketIOComponent socket;
  private float position_x = 111.9f;
  private float position_y = 620f;
  private float position_z = 800f;
  public GameObject map;

  // Use this for initialization
  void Start()
  {
    Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(),map.GetComponent<Collider>());


    GameObject go = GameObject.Find("SocketIO");
    socket = go.GetComponent<SocketIOComponent>();
    Thread.Sleep(5000);

    socket.On("receiveposition", (SocketIOEvent e) =>
    {
          //Debug.Log(string.Format("[name: {0}, data: {1}]", e.name, e.data));
          position_x = float.Parse(e.data[0].ToString());
          position_y = float.Parse(e.data[1].ToString());
          position_z = float.Parse(e.data[2].ToString());
    });

  }

  public void TestBoop(SocketIOEvent e)
  {
    Debug.Log(string.Format("[name: {0}, data: {1}]", e.name, e.data));
  }

  // Update is called once per frame
  void Update()
  {
    Thread.Sleep(30);
    socket.Emit("getposition", "I want to get the new position");
    Move();
  }

  private void Move()
  {
    Vector3 position = new Vector3(position_x, position_y, position_z);
    this.gameObject.transform.position = position;
    //Debug.Log(this.gameObject.transform.position);
  }


}
