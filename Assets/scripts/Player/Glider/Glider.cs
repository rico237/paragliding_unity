using UnityEngine;
using System.Collections;

public class Glider : MonoBehaviour, IBlowable{

	private Player player;
	private Rigidbody body;
	private WindController wind;
    private Thermal thermals;
	public float angleOfAttack;
	private Vector3 airVelocity;
	private GameObject gliderLines;
	private float originAngleOfAttack;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody> ();
		player = GameObject.Find ("Player").GetComponent<Player> ();
		wind = GameObject.Find ("Terrain").GetComponent<WindController> ();
        //thermals = GameObject.Find("Thermal").GetComponent<Thermal>();
		//gliderLines = transform.Find ("gliderLines").gameObject;
		Reference.blowables.Add (this);

		originAngleOfAttack = angleOfAttack;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (player.getDeployed ()) {
			fly ();
		} else {
			//Put the glider behind player when deploying.
			pushBack ();
		}

		HideGlider ();

		//print ("Relative speed: " + (body.velocity - wind.GetVelocity (this)));
	}

	private void fly(){ 
		 
		//The velocity relative to the air
        //TODO: MINUS THE THERMAL VELOCITY!!! 
		airVelocity = transform.InverseTransformDirection (body.velocity - wind.GetVelocity(this));

		//The drag from going forward
		//If the angle of attack is higher, the forward drag will increase
		Vector3 forwardDrag = Math.getDrag (airVelocity.z*Vector3.forward, Reference.DRAG_COEFFICIENT_FRONT, Reference.AIR_DENSITY_20,
			Reference.AREA_FRONT*angleOfAttack/originAngleOfAttack);

		float exposedUnder = Reference.AREA_UNDER/angleOfAttack*originAngleOfAttack;
		if (airVelocity.z <= Reference.STALL_LIMIT) {
			exposedUnder = 2;
		}
		//The drag from falling without lift (parachuting)
		//If the angle of attack is higher, the area under will decrease and thereby fall will increase
		Vector3 fallDrag = Math.getDrag (airVelocity.y * Vector3.up, Reference.DRAG_COEFFICIENT_UNDER, Reference.AIR_DENSITY_20,
			exposedUnder);

		//Drag from side-drifting. Speed is a stabilizer! THIS MAGICALLY CREATED CENTRIFUGAL FORCES!!
		Vector3 sideDrag = Math.getDrag (airVelocity.x * Vector3.right * Mathf.Abs(airVelocity.z), Reference.DRAG_COEFFICIENT_SIDE, Reference.AIR_DENSITY_20,
			                   Reference.AREA_SIDE);

		//The fall drag is ALWAYS the opposite of gravity, depending on how much of the
		//glider is exposed to the air
		body.AddForce (fallDrag);

		//Add the forces
		body.AddRelativeForce (getGlide(airVelocity.y, Reference.PLAYER_WEIGHT) + forwardDrag + getLift (airVelocity.z) + sideDrag);
		
		//print("Added force: " + (getGlide(airVelocity.y, Reference.PLAYER_WEIGHT) + forwardDrag + getLift (airVelocity.z)));

		//Do the braking/turning
		brake();

		if (Input.GetAxis ("speed") > 0) { //Cheap check to see if speeding is active
            //Debug.Log("FLA MOTHERFACKA");
			speed ();
		}
	}

	private Vector3 getLift(float relativeAirSpeed){

		//print ("Relative speed: " + relativeAirSpeed + " Actual speed: " + transform.InverseTransformDirection (body.velocity).z + " AoT: " + angleOfAttack + 
		//	" Sink: " + body.velocity.y);

		//Lift formula ~ relativeVel^2 * angleOfAttack
		//Angle of attack is faked here and should be affected only by braking or speeding
		if (relativeAirSpeed > Reference.STALL_LIMIT) { //Cheap stall check
			return (Vector3.up * Mathf.Pow(relativeAirSpeed,2)*angleOfAttack); //Speed makes lift
		} else {
			//print ("Stalling!");
			return Vector3.zero;
		}
	}

	private Vector3 getGlide(float fallVelocity, float mass){ 
		return Vector3.forward * -fallVelocity * mass; //Fall makes speed
	}

	public bool flyAble(){ //If glider is above head
		return transform.rotation.eulerAngles.x < Reference.FLYABLE_ANGLE &&
			transform.rotation.eulerAngles.x > -Reference.FLYABLE_ANGLE;
	}

	private void brake(){
        //Debug.Log("lets brake");
        //Debug.Log("brakeR: " + Input.GetAxis("brakeR"));
        //Debug.Log("brakeL: " + Input.GetAxis("brakeL"));

        //The drag gained when braking (pulling down small part of glider)
        Vector3 brakeDrag = Math.getDrag (body.velocity - wind.GetVelocity(this), Reference.DRAG_COEFFICIENT_UNDER, Reference.AIR_DENSITY_20, 
			               Reference.AREA_BRAKE);

        //Debug.Log("Brake drag: " + brakeDrag);

		Vector3 brakeRightPos = transform.TransformPoint (Vector3.right * 12 + Vector3.back);
		Vector3 brakeLeftPos = transform.TransformPoint (Vector3.left * 12 + Vector3.back);

		body.AddForceAtPosition (brakeDrag*Input.GetAxis("brakeR"), brakeRightPos);
		body.AddForceAtPosition (brakeDrag*Input.GetAxis("brakeL"), brakeLeftPos);

        //Debug.Log(Input.GetAxis("brakeR"));

		//An experiment in increasing AoT when braking.
		angleOfAttack = originAngleOfAttack + (Input.GetAxis("brakeR") + Input.GetAxis("brakeL"))*8;
	}

    public void applyForceLeft(float power) {
        Vector3 brakeDrag = Math.getDrag(body.velocity - wind.GetVelocity(this), Reference.DRAG_COEFFICIENT_UNDER, Reference.AIR_DENSITY_20,
                   Reference.AREA_BRAKE);
        Vector3 brakeLeftPos = transform.TransformPoint(Vector3.left * 12 + Vector3.back);
        body.AddForceAtPosition(brakeDrag * power, brakeLeftPos);
    }

    public void applyForceRight(float power)
    {
        Vector3 brakeDrag = Math.getDrag(body.velocity - wind.GetVelocity(this), Reference.DRAG_COEFFICIENT_UNDER, Reference.AIR_DENSITY_20,
           Reference.AREA_BRAKE);
        Vector3 brakeRightPos = transform.TransformPoint(Vector3.right * 12 + Vector3.back);
        body.AddForceAtPosition(brakeDrag * power, brakeRightPos);
    }


    private void pushBack(){
		//A cheap and hopefully temporary solution for placing glider behind player when deploying
		if (Input.GetKeyDown (KeyCode.Space) && !player.getDeployed()) {
			body.useGravity = false;
			body.AddRelativeForce(Vector3.back*10000);
			body.useGravity = true;
		}
	}

	private void speed(){
		float maxSpeedDegrees = originAngleOfAttack - Reference.SPEED_LIMIT;
		angleOfAttack = originAngleOfAttack - Input.GetAxis ("speed")*maxSpeedDegrees;
	}

	private void HideGlider(){
		gliderLines.SetActive (player.getDeployed ());
	}

	public void AddWind(Vector3 wind){ //Temporary solution. Area should change when wind is comming from different direction
        body.AddForce (Math.GetWindForce (wind - body.velocity, Reference.AREA_FRONT, Reference.DRAG_COEFFICIENT_FRONT));
}

	public Vector3 GetWorldPosition(){
		return body.position;
	}
}
