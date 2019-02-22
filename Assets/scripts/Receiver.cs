using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    public SocketIOComponent socket;    // Sokcet du serveur
    public GameObject map;              // Terrain si jamais besoin de gerer les collisions
    public Transform voile;             // Position & rotation de la voile

    // Use this for initialization
    void Start()
    {
        //Physics.IgnoreCollision(plyer.GetComponent<Collider>(), map.GetComponent<Collider>());

        if (voile  == null) {Debug.Log("Please do connections. Null object voile.");}
        if (socket == null) {socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();}

        // Start server
        StartCoroutine(ConnectToServer());

        // Listen to events
        socket.On("UPDATE_CAMERA", OnUpdateMove);
        socket.On("USER_CONNECTED", OnConnect);
    }

    #region Listening

    private void OnUpdateMove(SocketIOEvent evt)
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            float speed = 1f * Time.deltaTime;

            //Debug.Log("On user move unity with data : " + evt.data);

            Vector3 position = StringToVector3(evt.data.GetField("position").str);
            Quaternion rotation = StringToQuaternion(evt.data.GetField("rotation").str);

            voile.transform.position = Vector3.Lerp(voile.transform.position, position, speed);
            voile.transform.rotation = Quaternion.Lerp(voile.transform.rotation, rotation, speed);
        }
    }

    private void OnConnect(SocketIOEvent e)
    {
        Debug.Log("User connect with id : "+ e.data["name"]);
    }

    #endregion

    #region Commands

    private IEnumerator ConnectToServer()
    {
        // wait ONE FRAME and continue
        yield return null;

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

    #endregion

    #region JSON_Functions

    public static Dictionary<string, string> GetDictionaryPostion(GameObject obj)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["position"] = WriteVector3(obj.transform.position);
        data["rotation"] = WriteQuaternion(obj.transform.rotation);
        return data;
    }

    public static string WriteVector3(Vector3 v)
    {
        return v.x + "," + v.y + "," + v.z;
    }

    public static string WriteQuaternion(Quaternion v)
    {
        return v.x + "," + v.y + "," + v.z + "," + v.w;
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
