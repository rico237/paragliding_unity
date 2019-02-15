using SocketIO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Communication : MonoBehaviour
{
    public SocketIOComponent socket;
    public GameObject player;
    public UnityEngine.CharacterController controller;
    private JoyconManager joyconManager;
    List<Joycon> joycons;

    // Use this for initialization
    void Start()
    {
        joyconManager = JoyconManager.Instance;

        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;

        if (socket == null)
        {
            Debug.Log("Socket io null, Trying to assign it");
            socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        }
        if (controller == null)
        {
            Debug.Log("controller null, Trying to assign it");
            controller = GameObject.FindWithTag("Player").GetComponent<UnityEngine.CharacterController>();
        }
        if (player == null)
        {
            Debug.Log("player null, Trying to assign it");
            player = GameObject.FindWithTag("Player");
        }

        // Start server
        StartCoroutine(ConnectToServer());

        socket.On("USER_CONNECTED", OnUserConnected);
        socket.On("PLAY", OnUserPlay);
        socket.On("MOVE", OnUserMove);
        socket.On("JOYCON_UPDATE_LEFT", OnJoyconUpdate);
        socket.On("JOYCON_UPDATE_RIGHT", OnJoyconUpdate);

        socket.On("TEST", (SocketIOEvent e) => {
            Debug.Log(string.Format("[name: {0}, data: {1}]", e.name, e.data));
        });

    }

    private void OnJoyconUpdate(SocketIOEvent evt)
    {
        Debug.Log("On joycon update unity with data : " + evt.data);
    }

    private void OnUserConnected(SocketIOEvent evt)
    {
        Debug.Log("On user connected unity with data : " + evt.data);
    }

    private void OnUserPlay(SocketIOEvent evt)
    {
        Debug.Log("On user play unity with data : " + evt.data);
    }

    private void OnUserMove(SocketIOEvent evt)
    {
        Debug.Log("On user move unity with data : " + evt.data);
    }

    private IEnumerator ConnectToServer()
    {
        //yield return new WaitForSeconds(0.5f);
        socket.Emit("USER_CONNECT");



        // wait 1 seconds and continue
        //yield return new WaitForSeconds(1f);

        Dictionary<string, string> data = new Dictionary<string, string>();
        data["name"] = "Rico";
        Vector3 position = gameObject.transform.position;
        data["position"] = position.x + "," + position.y + "," + position.z;
        socket.Emit("PLAY", new JSONObject(data));

        // wait ONE FRAME and continue
        yield return null;
    }

   

    // Not used
    private void Update() { }
}
