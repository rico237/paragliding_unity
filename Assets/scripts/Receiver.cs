using SocketIO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    public static Receiver instance;
    public SocketIOComponent socket;
    public GameObject map;

    public GameObject sphere;
    public Transform voile;

    private Vector3 offset;

    private float position_x = 111.9f;
    private float position_y = 620f;
    private float position_z = 800f;

    public static Receiver Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        //if (instance == null)
        //    instance = this;
        //else if (instance != this)
        //    Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);
        if (instance != null) Destroy(gameObject);
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        //Physics.IgnoreCollision(plyer.GetComponent<Collider>(), map.GetComponent<Collider>());
        if (sphere == null) {Debug.Log("Please do connections. sphere"); /*plyer = GameObject.Find("Player").GetComponent<GameObject>();*/}
        if (voile  == null) {Debug.Log("Please do connections. Voile");}
        if (socket == null) {socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();}

        // Start server
        StartCoroutine(ConnectToServer());

        // Listen to events
        socket.On("UPDATE_CAMERA", OnBallMove);
        socket.On("USER_CONNECTED", OnConnect);
    }

    private void FixedUpdate()
    {
        //Player playa = GameObject.Find("Player").GetComponent<Player>();
        //Debug.Log(WriteVector3(followThis.transform.position));
        //if (sphere != null)
            //socket.Emit("UPDATE_CAMERA", new JSONObject(GetDictionaryPostion(sphere)));
    }

    #region Listening

    private void OnBallMove(SocketIOEvent e)
    {
        Vector3 v = StringToVector3(e.data["rotation"] + "");
        Quaternion q = StringToQuaternion(e.data["rotation"] + "");

        //if (sphere != null)
        //    offset = sphere.transform.position - v;
        //else
        //    offset = v;
        //Vector3 targetCamPos = sphere.transform.position + offset;

        //voile.transform.position = targetCamPos;
        voile.transform.position = v;
        voile.transform.rotation = q;
    }

    private void OnConnect(SocketIOEvent e)
    {
        Debug.Log("User connect with id : "+ e.data["id"]);
    }

    #endregion

    #region Commands

    private IEnumerator ConnectToServer()
    {
        yield return null;
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

    #endregion

    #region JSON_Functions

    public static string WriteVector3(Vector3 v)
    {
        return v.x + "," + v.y + "," + v.z;
    }

    public static string WriteQuaternion(Quaternion v)
    {
        return v.x + "," + v.y + "," + v.z + "," + v.w;
    }

    public static Dictionary<string, string> GetDictionaryPostion(GameObject obj)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["position"] = WriteVector3(obj.transform.position);
        data["rotation"] = WriteQuaternion(obj.transform.rotation);
        return data;
    }

    public static Quaternion StringToQuaternion(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            sVector = sVector.Substring(1, sVector.Length - 2);
        if (sVector.StartsWith("\"") && sVector.EndsWith("\""))
            sVector = sVector.Substring(1, sVector.Length - 2);

        string[] sArray = sVector.Split(',');
        Quaternion result = new Quaternion(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]),
            float.Parse(sArray[3]));
        return result;
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            sVector = sVector.Substring(1, sVector.Length - 2);
        if (sVector.StartsWith("\"") && sVector.EndsWith("\""))        
            sVector = sVector.Substring(1, sVector.Length - 2);

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    #endregion
}
