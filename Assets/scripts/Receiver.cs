using SocketIO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Receiver : MonoBehaviour {

    private SocketIOComponent socket;

    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        Thread.Sleep(5000);

        socket.On("chat", (SocketIOEvent e) => {
            Debug.Log(string.Format("[name: {0}, data: {1}]", e.name, e.data));
        });
    }

    public void TestBoop(SocketIOEvent e)
    {
        Debug.Log(string.Format("[name: {0}, data: {1}]", e.name, e.data));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
