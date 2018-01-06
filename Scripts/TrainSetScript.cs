using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainSetScript : MonoBehaviour {

	public struct TrainData
	{
		public int[] data;
		public int type;
	};

	private TrainData[] set;
	private int index;
	GameObject[] ptron;

	public TrainData[] getData()
	{
		return set;
	}

	// Use this for initialization
	void Start () {
		set = new TrainData[30];
		index = 0;
	}

	void trainStep()
	{
		ptron = GameObject.FindGameObjectsWithTag("Perceptron");
		GameObject[] canvas = GameObject.FindGameObjectsWithTag("Canvas");
		set[index].data = (int[])canvas[0].GetComponent<DrawCanvasScript>().getCanvas().Clone();
		set[index].type = index % 3;  
		print("Training symbol " + (index % 3) + " [" + (index + 1) + "/30]");
		index++;

		if (index == 30)
		{
			ptron[0].GetComponent<Ptron>().trainNetwork(0.2f, 0.3f, 30, 0.01f);
		}
	}

	// Update is called once per frame
	void Update () {
		if (index < 30)
		{
			if (Input.GetMouseButtonUp(0))
			{
				Invoke("trainStep", 0.25f);
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				ptron[0].GetComponent<Ptron>().save();
				print("Saved");
				Application.LoadLevel ("Menu");
			}

			if (Input.GetMouseButtonUp(0))
			{
				ptron[0].GetComponent<Ptron>().recall();
			}
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			Application.LoadLevel ("Menu");
		}
	}
}
