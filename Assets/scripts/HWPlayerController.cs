using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class HWPlayerController : MonoBehaviour
{
    public float speed = 2;
    public bool isLocalPlayer = true; //TODO: Switch to false when networking
    private SocketIOComponent socket;

    Vector3 oldPosition, currentPosition;
    Quaternion oldRotation, currentRotation;

    void Start()
    {
        if (socket == null) { socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>(); }

        // Get position and rotation from player
        oldPosition = transform.position; currentPosition = oldPosition;
        oldRotation = transform.rotation; currentRotation = oldRotation;
    }

    private void FixedUpdate()
    {
        // Update seulement si c'est le joueur local
        if (!isLocalPlayer) { return; }

        if (Input.GetKey(KeyCode.LeftShift)) { speed = 20; }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        // For game object
        transform.Rotate(0, moveHorizontal, 0);
        transform.Translate(0,0, moveVertical);

        // Update current position
        currentRotation = transform.rotation; currentPosition = transform.position;

        // Position & Rotation
        if (currentPosition != oldPosition || currentRotation != oldRotation)
        {
            JSONObject obj = new JSONObject(Receiver.GetDictionaryPostion(gameObject));
            socket.Emit("UPDATE_CAMERA", obj);
            oldPosition = currentPosition; oldRotation = currentRotation;
        }
    }
}
