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
    public float rotationLeft = 0;
    public float rotationRight = 0;

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
        socket.On("JOYCON_UPDATE_LEFT", OnJoyconUpdate);
        socket.On("JOYCON_UPDATE_RIGHT", OnJoyconUpdate);
        socket.On("PLAYER_MOVE", OnPlayerMove);

    }

    private void OnJoyconUpdate(SocketIOEvent evt)
    {
        Glider glider = GameObject.Find("Glider").GetComponent<Glider>();

        if (evt.data.GetField("type").str == "Left")
        {
            if (evt.data.GetField("isUp").str == "true")
            {
                print("JOY CON LEFT UP");
                rotationLeft += 0.005f * Time.deltaTime;
                glider.applyForceLeft(rotationLeft);

            }
            else rotationLeft = 0;
        }

        if (evt.data.GetField("type").str == "Right")
        {
            if (evt.data.GetField("isUp").str == "true")
            {
                print("JOY CON RIGHT UP");
                rotationRight += 0.005f * Time.deltaTime;
                glider.applyForceRight(rotationRight);
            }
            else rotationRight = 0;
        }
        //Debug.Log("On joycon update unity with data : " + evt.data);
    }

    private void OnUserConnected(SocketIOEvent evt)
    {
        Debug.Log("On user connected unity with data : " + evt.data);
    }

    private void OnPlayerMove(SocketIOEvent evt)
    {
        string position = evt.data.GetField("position").str;
        string rotation = evt.data.GetField("rotation").str;

        float speed = 1f;

        player.transform.position = Vector3.Lerp(player.transform.position, Receiver.StringToVector3(position), speed * Time.deltaTime);
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Receiver.StringToQuaternion(rotation), speed * Time.deltaTime);
    }


    private IEnumerator ConnectToServer()
    {
        yield return null;
        socket.Emit("USER_CONNECT");
        // wait ONE FRAME and continue
        yield return null;
    }

   

    // Not used
    private void Update() { }
}
