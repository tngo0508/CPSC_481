using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameNN : MonoBehaviour {

	// Use this for initialization
	GameObject[] ptron;
	GameObject[] player;
	public GameObject explosion;
	public GameObject shield;
	int symbol;

	void Start () {
		Invoke("GetNN", 0.5f);
		Invoke("GetPlayer", 0.5f);
	}

	void GetNN() {
		ptron = GameObject.FindGameObjectsWithTag("Perceptron");
		ptron[0].GetComponent<Ptron>().load();
	}

	void GetPlayer()
	{
		player = GameObject.FindGameObjectsWithTag("Player");
	}


	void ExecNN()
	{
		ptron[0].GetComponent<Ptron>().recall();
	}

	void ProcessNN()
	{
		symbol = ptron[0].GetComponent<Ptron>().getAnswer();
		print("NNController Recieved " + symbol);

		if (symbol == 0)
		{
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			
			foreach (GameObject enemy in enemies)
			{
				Instantiate(explosion, enemy.transform.position, enemy.transform.rotation);
				Destroy(enemy);
			}
		}
		if (symbol == 1)
		{
			player[0].GetComponent<Done_PlayerController>().setInvuln(true);
		}
		if (symbol == 2)
		{
			player[0].GetComponent<Done_PlayerController>().spreadShot();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp(0))
		{
			ExecNN();
			ProcessNN();
		}
	}
}


