using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ARSNetwork:MonoBehaviour
{
    /*
        This is class for ARS ML algorithm. Optimization is defined here, user can only set input size and
        output size to fit the problem he wants to optimize ( numOfinputs - how many sensored-data you want to feed
        , numOfOutputs - how many controll parameters does the Agent affect), and how many paths he wants to test 
        simultaneously and how many optimal ones he wants to take (numOfDirections and numOfBestDirections).


    */

    private float[] _inputs;
    private float[] _outputs;
    private float[][] _policy;
    private float[][] _randomPerturbation;
    private float _fitnessValue;
    private int _numOfSteps;
    private int _episodeLen;
    private float _learningRate;
    private int _numOfDirections;
    private int _numOfBestDirections;
    private float _noise;

    public ARSNetwork(int numOfInputs, int numOfOutputs, int numOfDirections, int numOfBestDirections)
    {
        /*
            Initialize ARS network with some constant learning parameters
        */
        this._numOfSteps = 1000;
        this._episodeLen = 1000;
        this._learningRate = 0.02f;
        this._numOfDirections = numOfDirections;
        this._numOfBestDirections = numOfBestDirections;
        this._noise = 0.03f;
        this._inputs = new float[numOfInputs];
        this._outputs = new float[numOfOutputs];

        InitPolicy();

    }

    private void InitPolicy()
    {
        List<float[]> matrixOfWeights = new List<float[]>();

        for (int i = 0; i < _outputs.Length; i++)
        {
            float[] singleInputWeigts = new float[_inputs.Length];

            for (int j = 0; j < _inputs.Length; j++)
            {
                singleInputWeigts[j] = 0;
            }

            matrixOfWeights.Add(singleInputWeigts);
        }
        _policy = matrixOfWeights.ToArray();
    }

    private void CopyPolicy(float[][] policy)
    {
        for (int i = 0; i < policy.Length; i++)
        {
            for (int j = 0; j < policy[i].Length; j++)
            {
                _policy[i][j] = policy[i][j];
            }       
        }
    }    

    public void GenerateRandomPerturbation(float[][] policy)
    {
        //Generate random perturbation to the policy
        List<float[]> randomPerturbation = new List<float[]>();
        for (int i = 0; i < policy.Length; i++)
        {
            float[] randomCol = new float[policy[i].Length];

            for (int j = 0; j < policy[i].Length; j++)
            {
                randomCol[j] = NormalDistr();
            }

            randomPerturbation.Add(randomCol);
        }
        _randomPerturbation = randomPerturbation.ToArray();

    }

    private float NormalDistr()
    {
        //Box - Muller transform
        float r1 = UnityEngine.Random.Range(0, 1);
        float r2 = UnityEngine.Random.Range(0, 1);
        float z = Mathf.Sqrt(- 2 * Mathf.Log(r1)) * Mathf.Cos(2*Mathf.PI * r2);

        return z;
    }

}


