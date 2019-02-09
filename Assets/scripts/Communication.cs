using SocketIO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class communication : MonoBehaviour {
    private SocketIOComponent socket;

    // Use this for initialization
    void Start () {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("chat", (SocketIOEvent e) => {
            Debug.Log("SAAAAAAAALUT");
            Debug.Log(string.Format("[name: {0}, data: {1}]", e.name, e.data));
        });

        /*Debug.Log(socket.IsConnected);
        Thread.Sleep(5000);
        Debug.Log(socket.IsConnected);

        socket.Emit("test", "connexion from unity");*/
        StartCoroutine("BeepBoop");

    }

    private IEnumerator BeepBoop()
    {
        // wait 1 seconds and continue
        yield return new WaitForSeconds(1);

        socket.Emit("test","Message 1");

        // wait 3 seconds and continue
        yield return new WaitForSeconds(3);

        socket.Emit("test", "Message 2");

        // wait 2 seconds and continue
        yield return new WaitForSeconds(2);

        socket.Emit("test", "Message 3");

        // wait ONE FRAME and continue
        yield return null;

        socket.Emit("test", "Message 4");
        socket.Emit("test", "Message 5");
    }

    // Update is called once per frame
    void Update () {
  
    }
}
