using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using XboxCtrlrInput;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	XboxController m_controller;

	[SerializeField]
	bool m_goRight;
	Vector3 m_direction;

	float initTimestep = 0.5F;
	float timestep; 
	float time; 

	GridManager m_gridManager; 
	Vector3 m_startPos;
	[SerializeField]
	Transform m_InitPos;

	//The actual group which can rotate and will move down
	public Rotation actualGroup;

	Rotation preparedPiece;

	float m_verticalMovementDelay = .2f;
	float m_nextMoveTime;


	void Start(){
		m_gridManager = FindObjectOfType<GridManager>(); 
		timestep = initTimestep;

		m_direction = m_goRight ? Vector3.right : Vector3.left;
		m_startPos = m_goRight ? GridManager.getLeft() : GridManager.getRight();
	}

	public void startGame()
	{
		prepareNext();
		Invoke("spawnNew",1);
	}
	//Move down in interval of timestep
	void Update ()
	{
		time += Time.deltaTime; 
		if (time > timestep)
		{
			time = 0;
			move(m_direction); 
		}
		
		if (XCI.GetButtonDown(XboxButton.RightBumper, m_controller))
			actualGroup.rotateRight (false);
		else if (XCI.GetButtonDown(XboxButton.LeftBumper, m_controller))
			actualGroup.rotateLeft (false);

		if(XCI.GetAxis(XboxAxis.LeftStickY, m_controller) != 0)
		{
			if(m_nextMoveTime < Time.time)
			{
				if (XCI.GetAxis(XboxAxis.LeftStickY, m_controller) > 0)
					move(Vector3.up);
				else
					move(Vector3.down);

				m_nextMoveTime = Time.time + m_verticalMovementDelay;
			}
		}
		else
			m_nextMoveTime = 0;
		
		if (XCI.GetButton(XboxButton.A, m_controller))
			timestep = 0.05F; 
		else
			timestep = initTimestep;
	}

	[SerializeField]
	GameObject[] groups;

	void move(Vector3 dir)
	{
		if (actualGroup != null)
		{
			actualGroup.transform.position += dir; 

			if (!m_gridManager.updateArrayBool(m_goRight, actualGroup.transform))
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
		preparedPiece = Instantiate(groups[Random.Range(0, groups.Length)], m_InitPos.position, Quaternion.identity, m_gridManager.transform).GetComponent<Rotation>();
		preparedPiece.goRight = m_goRight;
		preparedPiece.GetComponent<Animation>().enabled = true;

		if(m_goRight)
			preparedPiece.rotateLeft(false);
		else
			preparedPiece.rotateRight(false);
	}

	//Handle spawning a new group and check if there is any intersection after spawning
	void spawnNew()
	{
		if(actualGroup)
			actualGroup.isActive = false; 

		actualGroup = preparedPiece;
		actualGroup.transform.position = m_startPos;

		bool needCorrection = true;

		foreach (Transform item in actualGroup.transform)
			if(item.position.x == (m_goRight ? 0 : 35))
				needCorrection = false;
		
		if(needCorrection)
			actualGroup.transform.position -= m_direction;

		prepareNext();

		// GameOver
		// if (!m_gridManager.updateArrayBool(m_goRight))
		// 	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		// else
			m_gridManager.checkForFullLine (m_direction);

		actualGroup.isActive = true;
		foreach (Transform cube in actualGroup.transform)
			cube.tag = "Cube";
	}
}