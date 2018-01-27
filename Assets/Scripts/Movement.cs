using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using XboxCtrlrInput;

public class Movement : MonoBehaviour
{
	[SerializeField]
	XboxController m_controller;

	[SerializeField]
	bool m_goRight;
	Vector3 m_direction;

	public float initTimestep = 0.2F;
	float timestep; 
	float time; 

	CubeArray cA; 
	Vector3 m_startPos;
	[SerializeField]
	Transform m_InitPos;

	//The actual group which can rotate and will move down
	public Rotation actualGroup;

	GameObject preparedPiece;


	void Start(){
		cA = FindObjectOfType<CubeArray>(); 
		timestep = initTimestep;

		m_direction = m_goRight ? Vector3.right : Vector3.left;
		m_startPos = m_goRight ? CubeArray.getLeft() : CubeArray.getRight();
	}

	public void startGame()
	{
		prepareNext();

		if(m_goRight)
			actualGroup.rotateLeft (false);
		else
			actualGroup.rotateRight (false);

	}
	//Move down in interval of timestep
	void Update ()
	{
		time += Time.deltaTime; 
		if (time > timestep) {
			time = 0;
			move (m_direction); 
		}
		checkForInput (); 
	}

	bool m_canMoove = true;
	void checkForInput()
	{
		if (XCI.GetButtonDown(XboxButton.RightBumper, m_controller))
			actualGroup.rotateRight (false);
		else if (XCI.GetButtonDown(XboxButton.LeftBumper, m_controller))
			actualGroup.rotateLeft (false);
		

		if (XCI.GetAxis(XboxAxis.LeftStickY, m_controller) > 0)
		{
			if(m_canMoove)
				move (Vector3.up);

			m_canMoove = false;
		}
		else if (XCI.GetAxis(XboxAxis.LeftStickY, m_controller) < 0)
		{
			if(m_canMoove)
				move (Vector3.down);

			m_canMoove = false;
		}
		else
			m_canMoove = true;
		
		if (XCI.GetButton(XboxButton.A, m_controller))
			timestep = 0.05F; 
		else
			timestep = initTimestep;

		// cA.updateArrayBool (m_goRight);
	}

	[SerializeField]
	GameObject[] groups;

	void move(Vector3 dir)
	{
		if (actualGroup != null)
		{
			actualGroup.transform.position += dir; 

			if (!cA.updateArrayBool(m_goRight, actualGroup.transform))
			{
				actualGroup.transform.position -= dir; 
				ManageAudio.instance.playCantMove();

				if(dir == m_direction)
					spawnNew();
			}
		}
	}

	void prepareNext()
	{
		preparedPiece = Instantiate(groups[Random.Range(0, groups.Length)], m_InitPos.position, Quaternion.identity, cA.transform);
		preparedPiece.GetComponent<Rotation>().goRight = m_goRight;
	}

	//Handle spawning a new group and check if there is any intersection after spawning
	void spawnNew()
	{
		actualGroup.GetComponent<Rotation>().isActive = false; 

		actualGroup = preparedPiece.GetComponent<Rotation>();
		prepareNext();

		if(m_goRight)
			actualGroup.rotateLeft (false);
		else
			actualGroup.rotateRight (false);

		actualGroup.GetComponent<Rotation>().isActive = true;

		// GameOver
		if (!cA.updateArrayBool(m_goRight))
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		else
			cA.checkForFullLine (m_direction);
	}
}