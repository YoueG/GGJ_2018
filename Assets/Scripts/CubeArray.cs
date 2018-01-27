using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CubeArray : MonoBehaviour {
	bool[,] isCube;

	[SerializeField]
	int m_width, m_height;
	static int WIDTH, HEIGHT;

	// Use this for initialization
	void Awake () {
		isCube = new bool[m_width,m_height];
		updateArrayBool ();

		WIDTH = m_width;
		HEIGHT = m_height;
	}

	public static Vector3 getLeft()
	{
		return new Vector3(0, (HEIGHT/2));
	}

	public static Vector3 getRight()
	{
		return new Vector3(WIDTH, (HEIGHT/2));
	}

	//Update the cube array and return false if there is any intersection between two cubes
	public bool updateArrayBool(){
		isCube = new bool[m_width,m_height];

		bool withoutIntersection = true; 
		foreach (GameObject cube in GameObject.FindGameObjectsWithTag("Cube"))
		{
			int x = (int)cube.transform.position.x;
			int y = (int)cube.transform.position.y;

			if (x >= 0 && x < m_width/2 && y >= 0 && y < m_height)
			{
				bool cubeSetted = isCube [x, y]; 
				if (cubeSetted)
				{
					//Position in array is always setted
					withoutIntersection = false;
				}
				else
				{
					isCube [(int)cube.transform.position.x, (int)cube.transform.position.y] = true;
				}
			}
			else
			{
				//Position is out of range 
				withoutIntersection = false; 
			}
		}
		return withoutIntersection; 
	}


	public void checkForFullLine()
	{
		//Check if there is any full line 
		List<int> isFullLine = new List<int> (); 
		for (int i = 0; i < m_height; i++) {
			bool isFull = true; 
			for (int j = 0; j < m_width; j++) {
				if (!isCube [j, i])
					isFull = false; 	
			} 
			if (isFull)
				isFullLine.Add (i); 
		}

		//For each full line
		for(int i = 0; i < isFullLine.Count; i++){
			//Delete all cubes in a row
			foreach (GameObject cube in GameObject.FindGameObjectsWithTag("Cube")) {
				int y = (int)cube.transform.position.y;
				if(isFullLine[i] == y){
					//Heres goes a incredible cool explosion effect!!!
					Destroy (cube); 
				}
			}
			//Set down all cubes above the deleted row
			foreach (GameObject cube in GameObject.FindGameObjectsWithTag("Cube")) {
				int y = (int)cube.transform.position.y;
				if(isFullLine[i] < y){
					cube.transform.position += Vector3.down; 
				}
			}
			ManageAudio.instance.PlayFullLine (); 

			for (int j = 0; j < isFullLine.Count; j++) {
				isFullLine [j] -= 1;
			}
		}
			

	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(m_width/2, m_height/2, 1), new Vector3(m_width, m_height, 1));
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