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

    public bool rightUp = false;
    public bool leftUp = false;

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

    }

    private void OnJoyconUpdate(SocketIOEvent evt)
    {
        Glider glider = GameObject.Find("Glider").GetComponent<Glider>();
        Player player = GameObject.Find("Player").GetComponent<Player>();

        if (evt.data.GetField("type").str == "Left")
        {
            if (evt.data.GetField("isUp").str == "true")
            {
                print("JOY CON LEFT UP");
                leftUp = true;
                if (player.deployed)
                {
                    rotationLeft += 0.005f * Time.deltaTime;
                    glider.applyForceLeft(rotationLeft);
                }
                else rotationLeft = 0;
            }
            else leftUp = false;

            if(evt.data.GetField("pushed_buttons").str == "SHOULDER_2")
            {
                player.rotateLeft();
            }
        }

        if (evt.data.GetField("type").str == "Right")
        {
            if (evt.data.GetField("isUp").str == "true")
            {
                print("JOY CON RIGHT UP");
                rightUp = true;
                if (player.deployed)
                {
                    rotationRight += 0.005f * Time.deltaTime;
                    glider.applyForceRight(rotationRight);
                }
                else rotationRight = 0;
            }
            else rightUp = false;

            if (evt.data.GetField("pushed_buttons").str == "SHOULDER_2")
            {
                player.rotateRight();
            }
        }

        if(rightUp && leftUp && !player.deployed)
        {
            player.deploy();
        }

        if(System.Convert.ToDouble(evt.data.GetField("accel_magnitude").str) >= 2.0 && !player.flying)// Run
        {
            player.move();
        }

        if (evt.data.GetField("pushed_buttons").str == "HOME")
        {
            Debug.Log("GO HOME IMMIGRANT");
            player.goHome();
        }


        //Debug.Log("On joycon update unity with data : " + evt.data.GetField("accel_magnitude").str);
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
