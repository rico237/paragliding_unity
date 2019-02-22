using UnityEngine;
using SocketIO;

public class HWGUI : MonoBehaviour
{
    private SocketIOComponent socket;

    private void Start()
    {
        socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
    }

    private void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(10, 110, 350, 100), "Socket connect : " + socket.url);
        }
    }
}
