using UnityEngine;
using System.Collections;
using System;

public class Swarm_Mover : MonoBehaviour
{
	public float speed;
	public float stiffness = 2.0f;
	public Vector3 dDirection;
	private Vector3 direction;

	void Start ()
	{
		direction = Vector3.back;
		dDirection = direction;
	}

	void Update()
	{
		float p = UnityEngine.Random.Range(0.0f, 1.0f);

		p = (float)Math.Pow(p,stiffness);

		if (p < .5)
		{
			p = p * -1 + 1;
		}

		direction = (p*direction + (1 - p)*dDirection);

		direction = direction.normalized;

		GetComponent<Rigidbody>().velocity = direction * speed;
	}



	
}
