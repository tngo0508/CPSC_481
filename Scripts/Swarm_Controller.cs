using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class Swarm_Controller : MonoBehaviour
{
	public Vector3 gBest;
	public int _SwarmAmount = 5;
	public GameObject prefab;
	private int SwarmAmount;
	private GameObject[] SC;


	void Start()
	{
		gBest = Vector3.forward * 100;
		SwarmAmount = _SwarmAmount;
		SC = GameObject.FindGameObjectsWithTag("Swarm_Controller");
		if (GetComponent<Transform>().position.x < 0)
		{
			GetComponent<Rigidbody>().velocity = Vector3.right * 5;
		}
		else
		{
			GetComponent<Rigidbody>().velocity = Vector3.left * 5;
		}
			
		Spawn();
	}

	void Update()
	{
		GameObject[] Particles = GameObject.FindGameObjectsWithTag("Enemy_Swarm");
		GameObject[] Player = GameObject.FindGameObjectsWithTag("Player");

		InvokeRepeating("Spawn", .5f, .5f);

		if (Particles.Length == 0)
		{
			Destroy(SC[0]);
		}

		Vector3 current;

		foreach (GameObject Particle in Particles)
		{
			current = (Player[0].transform.position - Particle.transform.position);
			if (gBest.magnitude > current.magnitude)
			{
				gBest = current;
			}
		}

		foreach (GameObject Particle in Particles)
		{
			Swarm_Mover sm = Particle.GetComponent<Swarm_Mover>();

			sm.dDirection = gBest - Particle.transform.position;	
		}
	}

	void Spawn()
	{
		if (SwarmAmount == 0)
		{
			return;
		}
		else
		{
			SwarmAmount--;
			Instantiate(prefab,SC[0].transform.position, Quaternion.identity);
		}
	}
	
}
