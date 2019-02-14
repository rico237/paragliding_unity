using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {

	private Player player;

	// Use this for initialization
	void Start () {
		player = transform.parent.GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update () {

		float mouseY = Input.GetAxis ("mouseY");

		//Fixes the look limits. There are still few bugs: If you move mouse too fast it will pass the limit, so 'else' is needed for 
		//putting the rotation back to the limits when exceeded.
		if(((transform.rotation.eulerAngles.x < Reference.LOOK_DOWN_LIMIT || transform.rotation.eulerAngles.x >= Reference.LOOK_UP_LIMIT)
		    && mouseY < 0 ) || (mouseY > 0  && (transform.rotation.eulerAngles.x > Reference.LOOK_UP_LIMIT ||
		                                    transform.rotation.eulerAngles.x <= Reference.LOOK_DOWN_LIMIT))){
			//Rotate vertically around the local x-axis.
			transform.Rotate(mouseY*Time.deltaTime*-Reference.MOUSE_SENSITIVITY, 0,0, Space.Self);
		}

		if (player.getDeployed ()) { //Control only camera horizontal look when glider is deployed, do not move the actual player.

			//Relative to the player's y
			transform.RotateAround (transform.position, transform.parent.up, Input.GetAxis ("mouseX")
			                        * Time.deltaTime * Reference.MOUSE_SENSITIVITY);
		} else{ //Face camera same as player
			transform.eulerAngles = new Vector3(
				transform.eulerAngles.x, transform.parent.eulerAngles.y, transform.eulerAngles.z);
		}
	}
}
