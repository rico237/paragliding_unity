using UnityEngine;
using System.Collections;

public class GliderRenderer : MonoBehaviour {

	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();

	}
	
	// Update is called once per frame
	void Update () {


		RightBrakeFrame ();
		LeftBrakeFrame ();
	}

	private void LeftBrakeFrame(){
		animator.Play("LeftBrake", 1, Input.GetAxis("brakeL"));
	}

	private void RightBrakeFrame(){
		animator.Play("RightBrake", 2, Input.GetAxis("brakeR"));
	}
}
