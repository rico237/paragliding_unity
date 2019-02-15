using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class JoyconController : MonoBehaviour
{
    public SocketIOComponent socket;
    // Calibrer les valeures de la magnitude pour detecter le mouvement
    public float borne_sup_magnitude = (float)1.018;

    public float coeffAcceleration = (float)0.07;

    // Calibrer les valeur de monté et descente pour detecter la monté et descente
    public float borne_monte = (float)-1.02;
    public float borne_descente = (float)-0.08;

    public JoyconManager.JoyconType joyconType;
    private float leftOrientationX;
    private float rightOrientationX;
    //public float[] stick;

    public Vector3 gyro = Vector3.zero;
    public float gyroMagnitude;

    public Vector3 accel = Vector3.zero;
    public float accelMagnitude;
    public Quaternion orientation;
    public Vector3 rotation;
    //public Joycon joycon;
    Vector3 rotationOffset = new Vector3(0, 180, 0);

    private enum Direction { Droite, Gauche, Arret }

    private Direction rightJoyconDirectionState;
    private Direction leftJoyconDirectionState;

    private Vector3 position;

    JoyconManager joyconManager;
    List<Joycon> joycons;

    void Start()
    {
        joyconManager = JoyconManager.Instance;
        //joycon = joyconManager.GetJoycon(joyconType);

        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
    }

    private void SetJoyconState()
    {
        foreach (Joycon joycon in joycons)
        {
            float magnitude = joycon.GetAccel().magnitude;
            Vector3 accelero = joycon.GetAccel();
            //Debug.Log(accelero);

            Quaternion orientation = joycon.GetVector();
            orientation = new Quaternion(orientation.x, orientation.z, orientation.y, orientation.w);

            Vector3 lastPosition = new Vector3(0, (float)-1.0);

            if (joycon.isLeft)
            {
                if (orientation.y >= -0.3 && orientation.y <= 0.3)//joycon face vers le haut
                {
                }
            }
            else
            {
                Debug.Log("right: " + orientation);

                if (orientation.x >= -0.3 && orientation.x <= 0.3)//joycon face vers le haut
                {
                }
            }

        }

    }

    /*private void SetJoyconState()
    {
        foreach (Joycon joycon in joycons)
        {
            float magnitude = joycon.GetAccel().magnitude;
            Vector3 accelero = joycon.GetAccel();
            //Debug.Log(accelero);

            Quaternion orientation = joycon.GetVector();
            orientation = new Quaternion(orientation.x, orientation.z, orientation.y, orientation.w);
            Debug.Log(orientation);
            

            Vector3 lastPosition = new Vector3(0, (float)-1.0);

            // Bouton shoulder appuye
            if (joycon.GetButton(Joycon.Button.SHOULDER_1))
            {
                float borne_inf_magnitude = 1 - (borne_sup_magnitude - 1);

                // Assez de magnitude sur l'accelerometre pour considerer un mouvement
                if (magnitude < borne_inf_magnitude || magnitude > borne_sup_magnitude)
                {
                    if (accelero.y > borne_monte || accelero.y < borne_descente)
                    {
                        //Debug.Log("Acceleration : " + accelero);
                        if (rotation.x >= 0 && rotation.x <= 90)
                        {
                            // Gauche pour Joy droite & inverssement
                            //Debug.Log("Gauche");
                            if (!joycon.isLeft)
                            {
                                rightJoyconDirectionState = Direction.Gauche;
                            }
                            else
                            {
                                leftJoyconDirectionState = Direction.Droite;
                            }
                        }
                        else if (rotation.x <= 360 && rotation.x >= 270)
                        {
                            //Debug.Log("Droite");
                            if (!joycon.isLeft)
                            {
                                rightJoyconDirectionState = Direction.Droite;
                            }
                            else
                            {
                                leftJoyconDirectionState = Direction.Gauche;
                            }
                        }
                        else
                            leftJoyconDirectionState = Direction.Arret; rightJoyconDirectionState = Direction.Arret;
                    }
                    else
                        leftJoyconDirectionState = Direction.Arret; rightJoyconDirectionState = Direction.Arret; 
                }
                else  
                    leftJoyconDirectionState = Direction.Arret; rightJoyconDirectionState = Direction.Arret;
            }
            else 
                leftJoyconDirectionState = Direction.Arret; rightJoyconDirectionState = Direction.Arret;
        }
    }*/




    void SendJoyconsData()
    {
        if (joycons.Count != 0)
        {
            foreach (Joycon joycon in joycons)
            {
                if (joycon != null)
                {
                    if (joycon.isLeft) socket.Emit("JOYCON_UPDATE_LEFT", new JSONObject(GetDictionnaryFromJoycon(joycon)));
                    else socket.Emit("JOYCON_UPDATE_RIGHT", new JSONObject(GetDictionnaryFromJoycon(joycon)));
                }
            }
        }
        else Debug.Log("PB avec les joycons (0 found)");
    }

    private Dictionary<string, string> GetDictionnaryFromJoycon(Joycon joycon)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        if (joycon.isLeft)
        {
            data["type"] = "Left";
        }
        else
        {
            data["type"] = "Right";
        }

        Quaternion orientation = joycon.GetVector();
        orientation = new Quaternion(orientation.x, orientation.z, orientation.y, orientation.w);
        Quaternion quat = Quaternion.Inverse(orientation);
        Vector3 rot = quat.eulerAngles;
        Vector3 rotationOffset = new Vector3(0, 180, 0);
        rot += rotationOffset;
        orientation = Quaternion.Euler(rot);
        Vector3 rotation = orientation.eulerAngles;

        data["gyro"] = WriteVectorProperly(joycon.GetGyro());
        data["gyro_magnitude"] = joycon.GetGyro().magnitude.ToString();
        data["accel"] = WriteVectorProperly(joycon.GetAccel()); ;
        data["accel_magnitude"] = joycon.GetAccel().magnitude.ToString();
        data["orientation"] = orientation.x + "," + orientation.y + "," + orientation.z + "," + orientation.w;
        data["rotation"] = WriteVectorProperly(rotation);

        if (joycon.GetButton(Joycon.Button.SHOULDER_1))
        {
            data["pushed_buttons"] = "SHOULDER_1";
        }
        else if (joycon.GetButton(Joycon.Button.SHOULDER_2))
        {
            data["pushed_buttons"] = "SHOULDER_2";
        }
        else
        {
            data["pushed_buttons"] = "";
        }

        if (joycon.isLeft)
        {
            //Debug.Log("left: " + orientation);
            if (orientation.y >= -0.3 && orientation.y <= 0.3)//joycon face vers le haut
            {
                data["isUp"] = "true";
            }
            else
            {
                data["isUp"] = "false";
            }
        }
        else
        {
            //Debug.Log("right: " + orientation);
            if (orientation.x >= -0.3 && orientation.x <= 0.3)//joycon face vers le haut
            {
                data["isUp"] = "true";
            }
            else
            {
                data["isUp"] = "false";
            }
        }

        return data;
    }

    private string WriteVectorProperly(Vector3 theV)
    {
        return theV.x + "," + theV.y + "," + theV.z;
    }

    // Update is called once per frame
    void Update()
    {

        foreach (Joycon joycon in joycons)
        {
            // make sure the Joycon only gets checked if attached
            if (joycon != null)
            {
                // GetButtonDown checks if a button has been pressed (not held)
                if (joycon.GetButtonDown(Joycon.Button.SHOULDER_1))
                {
                    // Joycon has no magnetometer, so it cannot accurately determine its yaw value. Joycon.Recenter allows the user to reset the yaw value.
                    joycon.Recenter();

                    if (joycon.isLeft)
                    {
                        // Joycon gauche
                        leftOrientationX = rotation.x;
                    }
                    else
                    {
                        // Joycon droit
                        rightOrientationX = rotation.x;
                    }
                }

                //stick = joycon.GetStick();

                // Gyro values: x, y, z axis values (in radians per second)
                gyro = joycon.GetGyro();
                gyroMagnitude = gyro.magnitude;

                // Accel values:  x, y, z axis values (in Gs)
                accel = joycon.GetAccel();
                accelMagnitude = accel.magnitude;

                // fix rotation
                orientation = joycon.GetVector();
                orientation = new Quaternion(orientation.x, orientation.z, orientation.y, orientation.w);
                Quaternion quat = Quaternion.Inverse(orientation);
                Vector3 rot = quat.eulerAngles;
                rot += rotationOffset;
                orientation = Quaternion.Euler(rot);
                rotation = orientation.eulerAngles;



                SendJoyconsData();
            }
        }
    }
}
