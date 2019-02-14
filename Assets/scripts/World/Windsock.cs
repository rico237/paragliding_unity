using UnityEngine;
using System.Collections;

public class Windsock : MonoBehaviour, IBlowable {

	private Rigidbody sockBody;

	// Use this for initialization
	void Start () {
		Reference.blowables.Add (this);
		sockBody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate(){

	}

	public void AddWind(Vector3 wind){
		sockBody.AddForce (wind);
	}

	public Vector3 GetWorldPosition(){
		return sockBody.position;
	}
}
