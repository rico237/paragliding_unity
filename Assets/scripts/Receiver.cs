using SocketIO;
using System.Collections;
using UnityEngine;

public class Receiver : MonoBehaviour
{

    public SocketIOComponent socket;
    public GameObject map;
    public GameObject plyer;

    private float position_x = 111.9f;
    private float position_y = 620f;
    private float position_z = 800f;


    // Use this for initialization
    void Start()
    {

        if(plyer == null)
        {
            plyer = GameObject.FindWithTag("playa").GetComponent<GameObject>();
        }

        //Physics.IgnoreCollision(plyer.GetComponent<Collider>(), map.GetComponent<Collider>());

        if (socket == null)
        {
            socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        }



        StartCoroutine(ConnectToServer());

        socket.On("UPDATE_CAMERA", OnCameraUpdate);
        socket.On("USER_CONNECTED", OnConnect);
    }
    // Update is called once per frame
    void Update() { }

    private void OnConnect(SocketIOEvent e)
    {
        Debug.Log("User connect with id : "+ e.data["id"]);
    }

    private void OnCameraUpdate (SocketIOEvent e)
    {
        Debug.Log(e.data);
        position_x = float.Parse(e.data["x"].ToString());
        position_y = float.Parse(e.data["y"].ToString());
        position_z = float.Parse(e.data["z"].ToString());


        Vector3 position = new Vector3(position_x, position_y, position_z);
        gameObject.transform.position = position;
    }

    private IEnumerator ConnectToServer()
    {
        //yield return new WaitForSeconds(0.5f);
        socket.Emit("USER_CONNECT");

        // wait 1 seconds and continue
        //yield return new WaitForSeconds(1f);

        //Dictionary<string, string> data = new Dictionary<string, string>();
        //data["name"] = "Rico";
        //Vector3 position = gameObject.transform.position;
        //data["position"] = position.x + "," + position.y + "," + position.z;
        //socket.Emit("PLAY", new JSONObject(data));

        // wait ONE FRAME and continue
        yield return null;
    }

}
