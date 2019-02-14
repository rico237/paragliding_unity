using UnityEngine;
using System.Collections;

public class Harness : MonoBehaviour {

	private Player player;
	private MeshRenderer harnessRenderer;


	// Use this for initialization
	void Start () {

		//Player class object
		player = transform.parent.gameObject.GetComponent<Player> ();
		harnessRenderer = GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {

		//Show the harness if it's deployed
		harnessRenderer.enabled = player.getDeployed();

		if (player.getDeployed ()) {

			//Lock rotation of harness when deployed
			//transform.rotation = rotation;
		} else {

			//Keep rotation with player
			transform.rotation = transform.parent.rotation;
		}

	}
}
