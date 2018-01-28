using System.Collections;
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
	GameObject m_particles, m_lineEffect;

	// Use this for initialization
	void Awake () {
		isCube = new bool[m_width,m_height];

		WIDTH = m_width;
		HEIGHT = m_height;

		MIDDLE = m_width/2;

		m_gameManager = FindObjectOfType<GameManager>();
	}

	public static Vector3 getLeft()
	{
		return new Vector3(0, (HEIGHT/2));
	}

	public static Vector3 getRight()
	{
		return new Vector3(WIDTH-1 - 2, (HEIGHT/2));
	}

	GameManager m_gameManager;
	//Update the cube array and return false if there is any intersection between two cubes
	public bool updateArrayBool(bool goRight, Transform actualGroup = null)
	{
		bool result = true;

		if(actualGroup)
		{
			int pieceNb = actualGroup.childCount;

			Vector3[] temp = new Vector3[pieceNb];
			Vector3 decal = Vector3.zero;

			for (int i = 0; i < pieceNb; i++)
			{
				int y = (int)actualGroup.GetChild(i).position.y;
				int x = (int)actualGroup.GetChild(i).position.x;

				temp[i] = new Vector3(x, y);

				if(y < 0)
					decal = Vector3.up;
				else if (y >= m_height)
					decal = Vector3.down;
				else if(x < 0)
					decal = Vector3.right;
				else if (x >= m_width)
					decal = Vector3.left;
			}
			
			actualGroup.position += decal;
			for (int i = 0; i < pieceNb; i++)
				temp[i] += decal;

			isCube = new bool[m_width,m_height];

			foreach (GameObject cube in GameObject.FindGameObjectsWithTag("Cube"))
				if(cube.transform.parent != actualGroup)
					isCube [(int)cube.transform.position.x, (int)cube.transform.position.y] = true;
					
			for (int i = 0; i < pieceNb; i++)
			{
				if((int)temp[i].x <= 0 || (int)temp[i].x >= m_width-1)
				{
					m_gameManager.Victory(goRight);
					return false;			
				}
				else if(isCube[(int)temp[i].x, (int)temp[i].y])
				{
					ManageAudio.instance.blocsCollide();

					if((int)temp[i].x <= 1 || (int)temp[i].x >= m_width-2)
					{
						m_gameManager.Victory(!goRight);
						return false;
					}

					Destroy(Instantiate(m_particles, actualGroup.GetChild(i).position, goRight ?  Quaternion.Euler(0,0,0) : Quaternion.Euler(0,180,0)), 3);
					result = false;
				}
				
				isCube[(int)temp[i].x, (int)temp[i].y] = true;
			}
		}
		
		return result;
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
			{
				if (!isCube [x, y])
					isFull = false;
			}

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
				{
					Destroy (cube);
				}
				//Displace
				else
				{
					if(direction == Vector3.right)
						cube.transform.position += cube.transform.position.x <= isFullLine[i] ? 2*direction : direction;
					else
						cube.transform.position += cube.transform.position.x >= isFullLine[i] ? 2*direction : direction;					
				}
			}

			Destroy(Instantiate(m_lineEffect, Vector3.right * isFullLine[i], Quaternion.identity), 1);
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

		// DEBUG
		// for (int x = 0; x < m_width; x++)
		// 	for (int y = 0; y < m_height; y++)
		// 		if(isCube[x, y])
		// 			Gizmos.DrawCube(new Vector3(x, y), Vector3.one);
    }
#endif
}