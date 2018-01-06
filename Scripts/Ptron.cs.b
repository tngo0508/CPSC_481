using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Ptron : MonoBehaviour {
	public float[] inputNeurons;
	public float[] hiddenNeurons;
	public float[] outputNeurons;
	private float[] weights;

	private int[] parr;

	int inputN, outputN, hiddenN, hiddenL;

	// Use this for initialization
	void Start () {
		parr = new int[400];
		outputN = 3;
		hiddenL = 2;
		hiddenN = 200;
	
		inputN = 400;
	
		int weightSize = inputN*hiddenN+(hiddenN*hiddenN*(hiddenL-1))+hiddenN*outputN;
		weights = new float[weightSize];
	
		inputNeurons = new float[inputN];
		hiddenNeurons = new float[hiddenN*hiddenL];
		outputNeurons = new float[outputN];

		int index = 0;

		for (int i = 0; i < weightSize; i++)
		{
			weights[index] = UnityEngine.Random.Range(-0.5f, 0.5f);
		}
	}

	int inputToHidden(int inp, int hid)
	{
		return inputN*hid+inp;
	}

	int hiddenToHidden(int toLayer, int fromHid, int toHid)
	{
		return inputN*hiddenN+((toLayer-2)*hiddenN*hiddenN)+hiddenN+fromHid+toHid;
	}

	int hiddenToOutput(int hid, int o)
	{
		return inputN*hiddenN + (hiddenL-1)*hiddenN*hiddenN + hid*outputN+o;
	}

	int hiddenAt(int layer, int hid)
	{
		return (layer-1)*hiddenN + hid;
	}

	float sigmoid(float x)
	{
		return (float)(1 / (1 + Math.Exp(-x)));
	}

	float sigmoidP(float x)
	{
		return x * (1 - x);
	}

	void populateNetwork()
	{
		GameObject[] canvas = GameObject.FindGameObjectsWithTag("Canvas");

		int[] arr = canvas[0].GetComponent<DrawCanvasScript>().getCanvas();

		
		for (int i = 0; i < 400; i++)
		{
			parr[i] =  (int)inputNeurons[i];
			inputNeurons[i] = (float)arr[i];
		}

		if (arr.SequenceEqual(parr))
		{
			print("Identical canvas");
		}
	}

	void calculateNetwork()
	{
		for (int hidden = 0; hidden < hiddenN; hidden++)
		{
			hiddenNeurons[hiddenAt(1,hidden)] = 0;
			for (int input = 0; input < inputN; input++)
			{
				hiddenNeurons[hiddenAt(1,hidden)] += inputNeurons[input]*weights[inputToHidden(input,hidden)];
			}
			hiddenNeurons[hiddenAt(1,hidden)] = sigmoid(hiddenNeurons[hiddenAt(1,hidden)]);
		}

		for (int i = 2; i <= hiddenL; i++)
		{
			for (int j = 0; j < hiddenN; j++)
			{
				hiddenNeurons[hiddenAt(i,j)] = 0;
				for (int k = 0; k < hiddenN; k++)
				{
					hiddenNeurons[hiddenAt(i,j)] += hiddenNeurons[hiddenAt(i-1,k)] * weights[hiddenToHidden(i,k,j)];
				}
				hiddenNeurons[hiddenAt(i,j)] = sigmoid(hiddenNeurons[hiddenAt(i,j)]);
			}
		}


		for (int i = 0; i < outputN; i++)
		{
			outputNeurons[i] = 0;
			for (int j = 0; j < hiddenN; j++)
			{
				outputNeurons[i] += hiddenNeurons[hiddenAt(hiddenL,j)] * weights[hiddenToOutput(j,i)]; 
			}
			print(outputNeurons[i]);
			outputNeurons[i] = sigmoid(outputNeurons[i]);
		}
		
	}

	public void trainNetwork(float teachingStep, float momentum, int trainingFiles, int maxEpochs)
	{
		int tCounter = 0;
		int epochs = 1;
		float error = 0.0f;

		int target;

		GameObject[] ts = GameObject.FindGameObjectsWithTag("TrainSet");

		TrainSetScript.TrainData[] set = ts[0].GetComponent<TrainSetScript>().getData();

		float[] odelta = new float[outputN];
		float[] hdelta = new float[hiddenN*hiddenL];

		float[] tempWeights = new float[inputN*hiddenN+(hiddenN*hiddenN*(hiddenL-1))+hiddenN*outputN];
		float[] prWeights = new float[inputN*hiddenN+(hiddenN*hiddenN*(hiddenL-1))+hiddenN*outputN];
		
		while (epochs < maxEpochs)
		{
			while (tCounter < trainingFiles)
			{
				
				for (int i = 0; i < inputN; i++)
				{
					inputNeurons[i] = (float)set[tCounter].data[i];
				}
				target = set[tCounter].type;
					
				print(target);
	
				calculateNetwork();
				
				
				for (int i = 0; i < outputN; i++)
				{
					if (i != target)
					{
						odelta[i] = (0.0f - outputNeurons[i])*sigmoidP(outputNeurons[i]);
						error += (0.0f - outputNeurons[i])*(0.0f-outputNeurons[i]);
					}
					else
					{
						odelta[i] = (1.0f - outputNeurons[i])*sigmoidP(outputNeurons[i]);
						error += (1.0f - outputNeurons[i])*(1.0f-outputNeurons[i]);
					}
				}
	
				for (int i = 0; i < hiddenN; i++)
				{
					hdelta[hiddenAt(hiddenL, i)] = 0;
					for (int j = 0; j < outputN; j++)
					{
						hdelta[hiddenAt(hiddenL, i)] += odelta[j]*weights[hiddenToOutput(i,j)];
					}
					hdelta[hiddenAt(hiddenL, i)] *= sigmoidP(hdelta[hiddenAt(hiddenL, i)]);
				}
	
				for (int i = hiddenL-1; i > 0; i--)
				{
					for (int j = 0; j < hiddenN; j++)
					{
						hdelta[hiddenAt(i,j)] = 0;
						for (int k = 0; k < hiddenN; k++)
						{
							hdelta[hiddenAt(i,j)] = hdelta[hiddenAt(i+1,k)] * weights[hiddenToHidden(i+1,j,k)];
						}
						hdelta[hiddenAt(i,j)] *= sigmoidP(hdelta[(i-1)*hiddenN + j]);
					}
				}
	
				tempWeights = weights;
	
				for (int i = 0; i < inputN; i++)
				{
					for (int j  = 0; j < hiddenN; j++)
					{
						weights[inputToHidden(i,j)] += momentum*(weights[inputToHidden(i,j)] - prWeights[inputToHidden(i,j)]) + teachingStep*hdelta[hiddenAt(1,j)]*inputNeurons[i];
					}
				}
	
				for (int i = 2; i <= hiddenL; i++)
				{
					for (int j = 0; j < hiddenN; j++)
					{
						for (int k = 0; k < hiddenN; k++)
						{
							weights[hiddenToHidden(i,j,k)] = momentum*(weights[hiddenToHidden(i,j,k)] - prWeights[hiddenToHidden(i,j,k)]) + teachingStep*hdelta[hiddenAt(i,k)] * hdelta[hiddenAt(i-1,j)];
						}
					}
				}
	
				for (int i = 0; i < outputN; i++)
				{
					for (int j = 0; j < hiddenN; j++)
					{
						weights[hiddenToOutput(j,i)] = momentum*(weights[hiddenToOutput(j,i)] - prWeights[hiddenToOutput(j,i)]) + teachingStep * odelta[i] * hiddenNeurons[hiddenAt(hiddenL, j)];
					}
				}
				
				prWeights = tempWeights;
	
				error = 0;
				tCounter++;
			}
			tCounter = 0;
			epochs++;
		}
	}

	public void recall()
	{
		Invoke("recallNetwork", 0.25f);	
	}

	public void recallNetwork()
	{
		populateNetwork();
		calculateNetwork();

		float winner = -1;
		int index = 0;
		
		for (int i = 0; i < outputN; i++)
		{
			if (outputNeurons[i] > winner)
			{
				winner = outputNeurons[i];
				index = i;
			}
		}
		print(index);
	}
/*
	bool populateInput()
	void calculateNetwork()
	bool trainNetwork()
	void recallNetwork()
*/

	// Update is called once per frame
	void Update () {
		
	}
}
