using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using System.IO;

public class Ptron : MonoBehaviour {
	public float[] inputNeurons;
	public float[] hiddenNeurons;
	public float[] outputNeurons;
	//private float[] weights;

	private float[,] weightsHI;
	private float[,] weightsOH;

	private int[] parr;
	private int answer;

	private int inputN, outputN, hiddenN, hiddenL;

	// Use this for initialization
	void Start () {
		parr = new int[400];
		outputN = 3;
		hiddenL = 1;
		hiddenN = 150;
	
		inputN = 400;
	
		weightsHI = new float[hiddenN, inputN];
		weightsOH = new float[outputN, hiddenN];
		
		inputNeurons = new float[inputN];
		hiddenNeurons = new float[hiddenN*hiddenL];
		outputNeurons = new float[outputN];

		for (int hidden = 0; hidden < hiddenN; hidden++)
		{
			for (int input = 0; input < inputN; input++)
			{
				weightsHI[hidden,input] = UnityEngine.Random.Range(-0.5f, 0.5f);
			}
		}

		for (int output = 0; output < outputN; output++)
		{
			for (int hidden = 0; hidden < hiddenN; hidden++)
			{
				weightsOH[output,hidden] = UnityEngine.Random.Range(-0.5f, 0.5f);
			}
		}
	}

	public int getAnswer()
	{
		return answer;
	}

	public void save()
	{
		string path = "Assets/_Complete-Game/weights";
		StreamWriter writer = new StreamWriter(path, true);
		for (int hidden = 0; hidden < hiddenN; hidden++)
		{
			for (int input = 0; input < inputN; input++)
			{
				writer.WriteLine(weightsHI[hidden, input].ToString());
			}
		}
		for (int output = 0; output< outputN; output++)
		{
			for (int hidden = 0; hidden < hiddenN; hidden++)
			{
				writer.WriteLine(weightsOH[output, hidden].ToString());
			}
		}
		writer.Close();
	}	

	public void load()
	{
		string path = "Assets/_Complete-Game/weights";
		StreamReader reader = new StreamReader(path);
		for (int hidden = 0; hidden < hiddenN; hidden++)
		{
			for (int input = 0; input < inputN; input++)
			{
				weightsHI[hidden, input] = float.Parse(reader.ReadLine());
			}
		}
		for (int output = 0; output< outputN; output++)
		{
			for (int hidden = 0; hidden < hiddenN; hidden++)
			{
				weightsOH[output, hidden] = float.Parse(reader.ReadLine());
			}
		}
		reader.Close();
	}

	float sigmoid(float x)
	{
		return (float)(1.0f / (1 + Math.Exp(-x)));
	}

	float sigmoidP(float x)
	{
		float s = sigmoid(x);

		return s * (1.0f - s);
	}

	void populateNetwork()
	{
		GameObject[] canvas = GameObject.FindGameObjectsWithTag("Canvas");

		int[] arr = (int[])canvas[0].GetComponent<DrawCanvasScript>().getCanvas().Clone();

		
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
			hiddenNeurons[hidden] = 0;
			for (int input = 0; input < inputN; input++)
			{
				hiddenNeurons[hidden] += inputNeurons[input]*weightsHI[hidden, input];
			}
			hiddenNeurons[hidden] = sigmoid(hiddenNeurons[hidden]);
		}

		for (int output = 0; output < outputN; output++)
		{
			outputNeurons[output] = 0;
			for (int hidden = 0; hidden < hiddenN; hidden++)
			{
				outputNeurons[output] += hiddenNeurons[hidden]*weightsOH[output, hidden]; 
			}
			outputNeurons[output] = sigmoid(outputNeurons[output]);
		}
	}

	public void trainNetwork(float teachingStep, float momentum, int trainingFiles, float leastMeanSquaredError)
	{
		float meanSquaredError = 999.0f;
		float error = 0.0f;
		int tCounter = 0;
		int epochs = 0;
		int maxEpochs = 1000;
		int target;

		GameObject[] ts = (GameObject[])GameObject.FindGameObjectsWithTag("TrainSet").Clone();

		TrainSetScript.TrainData[] set = (TrainSetScript.TrainData[])ts[0].GetComponent<TrainSetScript>().getData().Clone();


		float[] odelta = new float[outputN];
		float[] hdelta = new float[hiddenN*hiddenL];

		float[,] tempWeightsHI;
		float[,] tempWeightsOH;
		float[,] prWeightsHI = (float[,])weightsHI.Clone();
		float[,] prWeightsOH = (float[,])weightsOH.Clone();
		
		while (epochs < maxEpochs && Math.Abs(meanSquaredError - leastMeanSquaredError) > 0.0001)
		{
			meanSquaredError = 0;
			while (tCounter < trainingFiles)
			{
				
				for (int input = 0; input < inputN; input++)
				{
					inputNeurons[input] = (float)set[tCounter].data[input];
				}
				target = set[tCounter].type;

				calculateNetwork();
				
				//Backpropagation

				//Calculate Output Deltas
				for (int output = 0; output < outputN; output++)
				{
					if (output != target)
					{
						odelta[output] = (0.0f - outputNeurons[output])*sigmoidP(outputNeurons[output]);
						error += (0.0f - outputNeurons[output])*(0.0f-outputNeurons[output]);
					}
					else
					{
						odelta[output] = (1.0f - outputNeurons[output])*sigmoidP(outputNeurons[output]);
						error += (1.0f - outputNeurons[output])*(1.0f-outputNeurons[output]);
					}
				}

				//Calculate Hidden Deltas
				for (int hidden = 0; hidden < hiddenN; hidden++)
				{
					hdelta[hidden] = 0;
					for (int output = 0; output < outputN; output++)
					{
						hdelta[hidden] += odelta[output]*weightsOH[output, hidden];
					}
					hdelta[hidden] *= sigmoidP(hdelta[hidden]);
				}


				// Propagate Forward: Weights
				tempWeightsHI = (float[,])weightsHI.Clone();
				tempWeightsOH = (float[,])weightsOH.Clone();
	
				for (int input = 0; input < inputN; input++)
				{
					for (int hidden  = 0; hidden < hiddenN; hidden++)
					{
						weightsHI[hidden, input] += momentum*(weightsHI[hidden, input] - prWeightsHI[hidden, input]) + teachingStep*hdelta[hidden]*inputNeurons[input];
					}
				}
	
				for (int output = 0; output < outputN; output++)
				{
					for (int hidden = 0; hidden < hiddenN; hidden++)
					{
						weightsOH[output, hidden] += momentum*(weightsOH[output, hidden] - prWeightsOH[output, hidden]) + teachingStep * odelta[output] * hiddenNeurons[hidden];
					}
				}
				
				//Store previous weights
				prWeightsHI = (float[,])tempWeightsHI.Clone();
				prWeightsOH = (float[,])tempWeightsOH.Clone();
	
				meanSquaredError += error/(outputN+1);

				error = 0;
				tCounter++;
			}

			print("epoch " + epochs + ", mse " + meanSquaredError);
			tCounter = 0;
			epochs++;
		}

		print("Completed training in " + epochs + " epochs");
	}

	public void recall()
	{
		recallNetwork();
		//Invoke("recallNetwork", 0.25f);
	}

	public void recallNetwork()
	{
		populateNetwork();
		calculateNetwork();

		float winner = -1;
		int index = 0;
		
		for (int output = 0; output < outputN; output++)
		{
			print(outputNeurons[output]);
			if (outputNeurons[output] > winner)
			{
				winner = outputNeurons[output];
				index = output;
			}
		}
		print(index);
		if (outputNeurons[index] > .9f)
		{
			answer = index;
		}
		else
		{
			answer = -1;
		}
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
