using UnityEngine;
using System.Collections;

public class Variometer : MonoBehaviour {
	private AudioSource audioSrc;
	private Rigidbody player;

	// Use this for initialization
	void Start () {
		audioSrc = GetComponent<AudioSource> ();
		player = GameObject.Find ("Player").GetComponent<Rigidbody> ();
		audioSrc.Play ();
		audioSrc.pitch = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){
		if (player.velocity.y > 0 && player.velocity.y < 1000) {
			audioSrc.pitch = player.velocity.y / 5;
		} else {
			audioSrc.pitch = 0;
		}
		//audio.Play ();
	}
}
