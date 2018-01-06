using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCellScript : MonoBehaviour {
	private float drawSpread;
	private bool drawing;
	private bool filled;
	public bool initialized;
	public bool initializedIndex;
	private Vector3 pMouse;
	public int index;
	// Use this for initialization

	void Start () {
		drawing = false;
		filled = false;
		initialized = false;
		Invoke("setSpread", 0.25f);
	}

	void setSpread()
	{
		drawSpread = GetComponent<Renderer>().bounds.size.x * 14;
	}
		
	public void setIndex (int i) {
		initializedIndex = true;
		index = i;
	}
	
	// Update is called once per frame
	void Update () {
		if (initialized == false)
		{
			if (!initializedIndex)
			{
				return;
			}
			initialized = true;
		}

		Vector3 mouse = Input.mousePosition;

		if (Input.GetMouseButton(0)) {
			if (drawing) {
				for (int i = 0; i < 10; i++) {
					if (GetComponent<Renderer>().bounds.IntersectRay(Camera.main.ScreenPointToRay(new Vector3(pMouse.x + 0.1f*i*(mouse.x - pMouse.x), pMouse.y + 0.1f*i*(mouse.y - pMouse.y), 0f)))){
						filled = true;
					}
					if (GetComponent<Renderer>().bounds.IntersectRay(Camera.main.ScreenPointToRay(new Vector3(pMouse.x+drawSpread + 0.1f*i*(mouse.x+drawSpread - pMouse.x+drawSpread), pMouse.y + 0.1f*i*(mouse.y - pMouse.y), 0f)))){
						filled = true;
					}
					if (GetComponent<Renderer>().bounds.IntersectRay(Camera.main.ScreenPointToRay(new Vector3(pMouse.x+drawSpread + 0.1f*i*(mouse.x+drawSpread - pMouse.x+drawSpread), pMouse.y+drawSpread + 0.1f*i*(mouse.y+drawSpread - pMouse.y+drawSpread), 0f)))){
						filled = true;
					}
					if (GetComponent<Renderer>().bounds.IntersectRay(Camera.main.ScreenPointToRay(new Vector3(pMouse.x + 0.1f*i*(mouse.x - pMouse.x), pMouse.y+drawSpread + 0.1f*i*(mouse.y+drawSpread - pMouse.y+drawSpread), 0f)))){
						filled = true;
					}
				}
			}else{
				if (GetComponent<Renderer> ().bounds.IntersectRay (Camera.main.ScreenPointToRay (mouse))) {
					filled = true;
				}
			}

			drawing = true;
			pMouse = mouse;
		}
		else if (drawing) 
		{
			transform.parent.gameObject.GetComponent<DrawCanvasScript>().commit(index, filled ? 1 : 0);
			filled = false;
			drawing = false;
		}

		Renderer rend = GetComponent<Renderer>();
		if (filled) {
			rend.material.SetColor ("_Color", Color.black);
		} else {
			if (transform.parent.gameObject.GetComponent<DrawCanvasScript>().previous(index))
			{
				rend.material.SetColor("_Color", Color.white);
			}
			else
			{
				rend.material.SetColor ("_Color", Color.white);
			}
		}
		pMouse = mouse;
	}
}
