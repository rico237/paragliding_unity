using UnityEngine;
using System.Collections;
using SocketIO;

public class Player : MonoBehaviour, IBlowable {

	private Vector3 moveDirection = Vector3.zero;
	private CharacterController controller;
	private float speed = 6;
	public bool deployed;
	public bool flying;
    private SocketIOComponent socket;
    private Rigidbody flyingBody;
	private CapsuleCollider flyingCollider;
	private Quaternion startRotation;
	public Vector3 relativeVelocityAir;

    public GameObject objectToCopy;
    public Transform home;

    private Rigidbody siegeBody;

    Vector3 offsetSiegeFromVoile;
    Vector3 oldPosition, currentPosition;
    Quaternion oldRotation, currentRotation;

	private const float maxWindVelocity = 20;

	// Use this for initialization
	void Start () {
        if (socket == null) { socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>(); }
        controller = GetComponent<CharacterController> ();
		flyingBody = GetComponent<Rigidbody> ();
		startRotation = flyingBody.rotation;
		Reference.blowables.Add(this);

        siegeBody = GameObject.FindWithTag("siege").GetComponent<Rigidbody>();
        offsetSiegeFromVoile = siegeBody.position - flyingBody.position;

        if (home == null)
            home = gameObject.transform;

        // Get position and rotation from player
        oldPosition = transform.position; currentPosition = oldPosition;
        oldRotation = transform.rotation; currentRotation = oldRotation;

        //Turn of the cursor while in fps
        //Cursor.visible = false;
		//Cursor.lockState = CursorLockMode.Locked;
	}

	void Update(){
		//if (Input.GetKey (KeyCode.Escape)) {
		//	Application.Quit ();
		//}

		if (flying) {
			WeightShift();
		}

    }

	void FixedUpdate () {
		playerControl ();

        // Update current position
        currentRotation = transform.rotation; currentPosition = transform.position;

        // Position & Rotation
        if (currentPosition != oldPosition || currentRotation != oldRotation)
        {
            JSONObject obj = new JSONObject(Receiver.GetDictionaryPostion(objectToCopy));
            socket.Emit("UPDATE_CAMERA", obj);
            socket.Emit("PLAYER_MOVE", obj);
            oldPosition = currentPosition; oldRotation = currentRotation;
        }

        //moveSiegeBody();
    }
    

    void moveSiegeBody()
    {
        GameObject siege = GameObject.FindWithTag("siege");
        siege.transform.position = flyingBody.position + offsetSiegeFromVoile;

    }

    void OnTriggerEnter(Collider collider){ //When hitting ground
		if (collider.gameObject.name == "Terrain") {
			setFlying (false);
            //Debug.Log("not flying");
		}
	}

	void OnTriggerExit(Collider collider){ //When leaving ground
		if (collider.gameObject.name == "Terrain" && deployed) {
			setFlying (true);
            //Debug.Log("flying");
        }
	}
	public bool getDeployed(){
		return deployed;
	}

	public float GetMass(){
		return flyingBody.mass;
	}

	public Vector3 GetRelativeVelocityAir(){
		return relativeVelocityAir;
	}

	public void AddWind(Vector3 wind){ //Temporary solution. Area should change when wind is comming from different direction
		relativeVelocityAir = transform.InverseTransformVector (flyingBody.velocity - wind);
		flyingBody.AddForce(Math.GetWindForce(wind - flyingBody.velocity, Reference.AREA_PLAYER_FRONT, Reference.DRAG_COEFFICIENT_PLAYER_FRONT));
	}

	public Vector3 GetWorldPosition(){
		return flyingBody.position;
	}


	private float GetWindPitch(float velocityAir){
		float startPitchVelocity = 8;
		if (velocityAir > startPitchVelocity) {
			return velocityAir / startPitchVelocity;
		}
		return 1;
	}

    public void move()
    {
        if (controller.isGrounded)
        {
            //Sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = 12;
            }
            else
            {
                speed = 8;
            }

            //Add the user inputs. X is sideways, Z is forward/backward.
            moveDirection = new Vector3(0, 0, 1.0f);

            //Translate the direction to world space
            moveDirection = transform.TransformDirection(moveDirection);

            //siegeBody.constraints = RigidbodyConstraints.FreezeRotationZ;

            //Apply the speed! 
            moveDirection *= speed;

        }
        else
        {//In the air
            moveDirection.y -= Reference.GRAVITY;
        }

        //Apply movement
        controller.Move(moveDirection * Time.deltaTime);
    }

	private void unDeployedControl(){ //All the code for player control
		
		//If the player is on ground and no active glider
		if (controller.isGrounded) {
			//Sprint
			if (Input.GetKey (KeyCode.LeftShift)) {
				speed = 12;
			} else {
				speed = 6;
			}
			
			//Add the user inputs. X is sideways, Z is forward/backward.
			moveDirection = new Vector3 (Input.GetAxis ("sideways"), 0, Input.GetAxis ("forward"));
			
			//Translate the direction to world space
			moveDirection = transform.TransformDirection (moveDirection);

            //Apply the speed! 
            moveDirection *= speed;
			
		} else {//In the air
			moveDirection.y -= Reference.GRAVITY;
		}
		
		//Apply movement
		controller.Move (moveDirection * Time.deltaTime);

    }
	
    public void deploy()
    {
        if(flyingBody.velocity.y < 0.5f && flyingBody.velocity.y > -0.5f && !flying)
        {
            setDeployed(!deployed);
            deployedControl();
        }
    }

	private void deployedControl(){
		//Walk forward
		//This should be modified so that when the running speed is to high, force is reduced
		flyingBody.AddRelativeForce (Vector3.forward * Input.GetAxis ("forward")*800);
	}
	
	private void playerControl(){
		if (!flying) {
			//Listen for deploy input
			if (Input.GetKeyUp (KeyCode.Space) && flyingBody.velocity.y < 0.5f && flyingBody.velocity.y > -0.5f) { //Deploy the glider. Kind of lags. 
                //Debug.Log("DEPLOY");
				setDeployed(!deployed);
			}
			if (deployed) {
				deployedControl ();
			} else {
				unDeployedControl ();
			}
		}
	}

	private void WeightShift(){

		//Move the player's center of mass. Not quite working though. It does not really turn! Very good for efficient turning
		flyingBody.centerOfMass = Vector3.right * Input.GetAxis ("leanRight")*3;
	}
	
	private void setFlying(bool flying){//Sets the mode to flying or not
		
		//Switch flight physics
		this.flying = flying;
		flyingBody.freezeRotation = !flying;
		if(flyingBody.freezeRotation){//Rotate the player right when landing
			flyingBody.rotation = Quaternion.Euler (startRotation.eulerAngles.x, 
			                                        flyingBody.rotation.eulerAngles.y, startRotation.eulerAngles.z);
		}
	}
	
	private void setDeployed(bool deployed){ //Sets the mode to deployed or not
		
		//Swith ground handling physics
		this.deployed = deployed;
		controller.enabled = !deployed;
		flyingBody.useGravity = deployed;
		flyingBody.isKinematic = !deployed;
	}

    public void rotateLeft()
    {
        if(!flying)
            transform.Rotate(-Vector3.up * 16 * Time.deltaTime);
    }

    public void rotateRight()
    {
        if(!flying)
            transform.Rotate(Vector3.up * 16 * Time.deltaTime);
    }

    public void goHome()
    {
        deployed = false;
        setDeployed(deployed);
        transform.position = home.position;
    }
}
