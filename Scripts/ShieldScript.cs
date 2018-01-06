using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour {

	GameObject[] player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectsWithTag("Player");
		Invoke("Death", 4.0f);
	}

	void Death ()
	{
		player[0].GetComponent<Done_PlayerController>().setInvuln(false);
		Destroy(transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = player[0].transform.position;
	}
}
