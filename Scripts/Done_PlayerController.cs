using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Done_Boundary 
{
	public float xMin, xMax, zMin, zMax;
}

public class Done_PlayerController : MonoBehaviour
{
	public float speed;
	public float tilt;
	public Done_Boundary boundary;

	public float dodgeDistance = 1.5f;
	public int target;
	public int dodge;
	public int baseZ = 0;
	public bool firing;
	public GameObject shot;
	public GameObject shield;
	public Transform shotSpawn;
	public float fireRate;
	 
	private float nextFire;

	public enum States {HUNT, EVADE};
	public States state = States.HUNT;
	public bool invuln;
	
	void seekClosestTarget()
	{
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject[] Player = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] Bolts = GameObject.FindGameObjectsWithTag("Enemy_Bolt");
		GameObject closest = null;
		float current, min = float.MaxValue;

		foreach (GameObject Bolt in Bolts)
		{
			if (dodgeDistance > Math.Sqrt(Math.Pow((Bolt.transform.position.x - Player[0].transform.position.x),2.0) +
				Math.Pow((Bolt.transform.position.z - 2 - Player[0].transform.position.z),2.0)))
			{
				state = States.EVADE;
				return;
			}
		}

		firing = false;
		if (.1f < Math.Abs(Player[0].transform.position.z - baseZ))
		{
			if (Player[0].transform.position.z > baseZ)
			{
				dodge = -1;
			}
			else
			{
				dodge = 1;
			}
		}
		else
		{
			dodge = 0;
		}

		if (Enemies.Length == 0)
		{
			target = 0;
			return;
		}

		foreach (GameObject Enemy in Enemies)
		{
			if (dodgeDistance < Math.Abs(Enemy.transform.position.y - Player[0].transform.position.y))
			{
				current = float.MaxValue;
			}
			else
			{
				current = (Enemy.transform.position - Player[0].transform.position).magnitude;
			}

			if (min > current)
			{
				if (dodgeDistance > current)
				{
					state = States.EVADE;
					return;
				}
				min = current;
				closest = Enemy;
			}
		}

		if (.5f > Math.Abs(closest.transform.position.x - Player[0].transform.position.x))
		{
			firing = true;
			target = 0;
		}
		else if (closest.transform.position.x == Player[0].transform.position.x)
		{
			target = 0;
		}
		else if (closest.transform.position.x > Player[0].transform.position.x)
		{
			target = 1;
		}
		else
		{
			target = -1;
		}
	}

	void avoidBolts()
	{
		GameObject[] Bolts = GameObject.FindGameObjectsWithTag("Enemy_Bolt");
		GameObject[] Player = GameObject.FindGameObjectsWithTag("Player");
		float current, min = dodgeDistance;
		GameObject closest = null;

		foreach (GameObject Bolt in Bolts)
		{
			
			current = (float)Math.Sqrt(Math.Pow((Bolt.transform.position.x - Player[0].transform.position.x),2.0) + Math.Pow((Bolt.transform.position.z - 2 - Player[0].transform.position.z),2.0));
			if (min > current)
			{
				min = current;
				closest = Bolt;
			}
		}

		if (closest == null)
		{
			state = States.HUNT;
			return;
		}

		if (closest.transform.position.x > Player[0].transform.position.x)
		{
			target = -1;
		}
		else
		{
			target = 1;
		}

		dodge = -1;

	}
	void Start()
	{
		Renderer r = GetComponent<Renderer>();
		foreach (Material m in r.materials)
		{
			Color color = m.color;
			color.a = 1.0f;
			m.color = color;
		}
		invuln = false;
	}

	public void setInvuln(bool b)
	{
		invuln = b;
		if (invuln)
		{
			Instantiate(shield, transform.position, transform.rotation);
		}
	}

	public void spreadShot()
	{
		Invoke("spreadShotInstance", 0.0f);
		Invoke("spreadShotInstance", 0.22f);
		Invoke("spreadShotInstance", 0.44f);
	}

	public void spreadShotInstance()
	{
		Quaternion[] angles = new Quaternion[5];
			
		angles[0] = Quaternion.Euler(shotSpawn.rotation.x, shotSpawn.rotation.y-16.0F, shotSpawn.rotation.z);
		angles[1] = Quaternion.Euler(shotSpawn.rotation.x, shotSpawn.rotation.y-8.0F, shotSpawn.rotation.z);
		angles[2] = Quaternion.Euler(shotSpawn.rotation.x, shotSpawn.rotation.y, shotSpawn.rotation.z);
		angles[3] = Quaternion.Euler(shotSpawn.rotation.x, shotSpawn.rotation.y+8.0F, shotSpawn.rotation.z);
		angles[4] = Quaternion.Euler(shotSpawn.rotation.x, shotSpawn.rotation.y+16.0F, shotSpawn.rotation.z);
		foreach (Quaternion q in angles)
		{
			Instantiate(shot, shotSpawn.position, q);
		}
		GetComponent<AudioSource>().Play ();
	}


	void Update ()
	{
		switch(state)
		{
			case States.HUNT :
				seekClosestTarget();
				break;
			case States.EVADE :
				avoidBolts();
				break;
		}

		if (/* Input.GetButton("Fire1") &&  */firing && Time.time > nextFire) 
		{
			nextFire = Time.time + fireRate;
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			GetComponent<AudioSource>().Play ();
		}
	}

	void FixedUpdate ()
	{
		
		float moveHorizontal = target;
		float moveVertical = dodge;

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		GetComponent<Rigidbody>().velocity = movement * speed;
		
		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
			0.0f, 
			Mathf.Clamp (GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
		);
		
		GetComponent<Rigidbody>().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
	}
}
