using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCanvasScript : MonoBehaviour {

	public GameObject cell;

	public int[] canvasState;
	private GameObject[] cells;

	public int[] getCanvas()
	{
		return canvasState;
	}
	
	public bool previous(int index)
	{
		return canvasState[index] == 1;
	}

	// Use this for initialization
	public void commit(int index, int val)
	{
		canvasState[index] = val;
	}

	void Start () {
	//	canvasState = new int[400];
	//	//cells = new GameObject[400];
	//	Bounds bounds = GetComponent<Renderer> ().bounds;
	//	float scaleX = bounds.size.x * 0.05f, scaleY = bounds.size.y * 0.05f;

	//	for (int x = 0; x < 20; x++) {
	//		for (int y = 0; y < 20; y++) {
	//			cells [x*20 + y] = Instantiate (cell, new Vector3(transform.position.x + x*scaleX, transform.position.y + y*scaleY, 0), Quaternion.identity, transform);
	//			cells [x*20 + y].transform.Rotate (-90, 0, 0);
	//			cells[x*20 + y].transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
	//			cells[x*20 + y].GetComponent<DrawCellScript>().setIndex(x*20 + y);
	//		}
	//	}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
