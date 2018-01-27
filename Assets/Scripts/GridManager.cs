﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GridManager : MonoBehaviour {
	bool[,] isCube;

	[SerializeField]
	int m_width, m_height;
	static int WIDTH, HEIGHT, MIDDLE;

	[SerializeField]
	GameObject m_particles;

	// Use this for initialization
	void Awake () {
		isCube = new bool[m_width,m_height];

		WIDTH = m_width;
		HEIGHT = m_height;

		MIDDLE = m_width/2;
	}

	public static Vector3 getLeft()
	{
		return new Vector3(0, (HEIGHT/2));
	}

	public static Vector3 getRight()
	{
		return new Vector3(WIDTH-1 - 2, (HEIGHT/2));
	}

	//Update the cube array and return false if there is any intersection between two cubes
	public bool updateArrayBool(bool goRight, Transform actualGroup = null)
	{
		isCube = new bool[m_width,m_height];

		bool notTouching = true;

		foreach (GameObject cube in GameObject.FindGameObjectsWithTag("Cube"))
		{
			int x = (int)cube.transform.position.x;
			int y = (int)cube.transform.position.y;

			if((goRight && x <= MIDDLE) || (!goRight && x >= MIDDLE-1))
			{
				if(y >= 0 && y < m_height)
				{
					// On line
					if(goRight && x == MIDDLE && actualGroup && cube.transform.parent == actualGroup)
						notTouching = false;
					else if(!goRight && x == MIDDLE-1 && actualGroup && cube.transform.parent == actualGroup)
						notTouching = false;
					else
					{
						if (isCube [x, y])
						{
							notTouching = false;							
							Destroy(Instantiate(m_particles, cube.transform.position, goRight ?  Quaternion.Euler(0,0,0) : Quaternion.Euler(0,180,0)), 3);
						}
						else
							isCube [(int)cube.transform.position.x, (int)cube.transform.position.y] = true;
					}
				}
				else
				{
					//Position is out of range 
					notTouching = false;
				}
			}
		}

		return notTouching;
	}

	[SerializeField]
	float m_shakeAmount;
	public void checkForFullLine(Vector3 direction)
	{
		//Check if there is any full line 
		List<int> isFullLine = new List<int> ();

		for (int x = 0; x < m_width; x++)
		{
			bool isFull = true; 
			for (int y = 0; y < m_height; y++)
				if (!isCube [x, y])
					isFull = false;

			if (isFull)
				isFullLine.Add (x);
		}

		GameObject[] cubeArray = GameObject.FindGameObjectsWithTag("Cube");

		//For each full line
		for(int i = 0; i < isFullLine.Count; i++)
		{
			foreach (GameObject cube in cubeArray)
			{
				//Delete line
				if(isFullLine[i] == (int)cube.transform.position.x)
					Destroy (cube);
				//Displace
				else
				{
					if(direction == Vector3.right)
						cube.transform.position += cube.transform.position.x <= isFullLine[i] ? 2*direction : direction;
					else
						cube.transform.position += cube.transform.position.x >= isFullLine[i] ? 2*direction : direction;					
				}
			}

			ManageAudio.instance.PlayFullLine();
			Shake.Value += m_shakeAmount;

			for (int j = 0; j < isFullLine.Count; j++)
				isFullLine [j] -= 1;
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(MIDDLE - MIDDLE/2, m_height/2, 1) - Vector3.one/2, new Vector3(MIDDLE, m_height, 1));
		Gizmos.DrawWireCube(new Vector3(MIDDLE + MIDDLE/2, m_height/2, 1) - Vector3.one/2, new Vector3(MIDDLE, m_height, 1));
    }
#endif
}

/*
 * 	//Only for debug purposes 
private void printArray(){
	int rowLength = isCube.GetLength(0);
	int colLength = isCube.GetLength(1);

	for (int i = 0; i < rowLength; i++)
	{
		string line = ""; 
		for (int j = 0; j < colLength; j++)
		{
			line += isCube [i, j].ToString (); 
		}
		print (line); 
	}
	Console.ReadLine();
}
*/