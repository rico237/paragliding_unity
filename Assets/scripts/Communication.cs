using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communication : MonoBehaviour
{
    public SocketIOComponent socket;
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


        // Start server
        StartCoroutine(ConnectToServer());

        socket.On("USER_CONNECTED", OnUserConnected);
        socket.On("PLAYER_MOVE", OnUserMove);
        socket.On("JOYCON_UPDATE_LEFT", OnJoyconUpdate);
        socket.On("JOYCON_UPDATE_RIGHT", OnJoyconUpdate);

        //joyconManager = JoyconManager.Instance;

        // get the public Joycon array attached to the JoyconManager in scene
        //joycons = JoyconManager.Instance.j;

    }

    private void OnJoyconUpdate(SocketIOEvent evt)
    {
        Glider glider = GameObject.Find("Glider").GetComponent<Glider>();
        Player player = GameObject.Find("Player").GetComponent<Player>();

        if (evt.data.GetField("type").str == "Left")
        {
            if (evt.data.GetField("isUp").str == "true")
            {
                //print("JOY CON LEFT UP");
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
                //print("JOY CON RIGHT UP");
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
            //Debug.Log("GO HOME IMMIGRANT");
            player.goHome();
        }


        //Debug.Log("On joycon update unity with data : " + evt.data.GetField("accel_magnitude").str);
    }

    private void OnUserConnected(SocketIOEvent evt)
    {
        Debug.Log("On user connected unity with data : " + evt.data.GetField("name").str);
    }

    private void OnUserMove(SocketIOEvent evt)
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            //float speed = 1f * Time.deltaTime;
            float speed = 1f;

            Player player = GameObject.Find("Player").GetComponent<Player>();
            Vector3 position = Receiver.StringToVector3(evt.data.GetField("position").str);
            Quaternion rotation = Receiver.StringToQuaternion(evt.data.GetField("rotation").str);

            player.transform.position = Vector3.Lerp(player.transform.position, position, speed);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, rotation, speed);
        }
    }

    private IEnumerator ConnectToServer()
    {

        Dictionary<string, string> data = new Dictionary<string, string>();

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            data["name"] = "Mobile/tablette";
        } 
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            data["name"] = "PC";
        }
        else
        {
            data["name"] = "Other";
        }

        //Vector3 position = gameObject.transform.position;
        //data["position"] = position.x + "," + position.y + "," + position.z;
        socket.Emit("USER_CONNECT", new JSONObject(data));

        // wait ONE FRAME and continue
        yield return null;
    }

}
