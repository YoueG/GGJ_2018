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
	Vector3 m_spawn;

	//The actual group which can rotate and will move down
	public GameObject actualGroup; 


	void Start(){
		cA = FindObjectOfType<CubeArray>(); 
		timestep = initTimestep;

		m_direction = m_goRight ? Vector3.right : Vector3.left;
		m_spawn = m_goRight ? CubeArray.getLeft() : CubeArray.getRight();
	}

	public void startGame(){
		actualGroup = spawnNext ();

		if(m_goRight)
			actualGroup.GetComponent<Rotation>().rotateLeft (false);
		else
			actualGroup.GetComponent<Rotation>().rotateRight (false);

	}
	//Move down in interval of timestep
	void Update () {
		time += Time.deltaTime; 
		if (time > timestep) {
			time = 0;
			move (m_direction); 
		}
		checkForInput (); 
	}

	bool m_canMoove = true;
	void checkForInput(){
		if (XCI.GetButtonDown(XboxButton.RightBumper, m_controller))
			actualGroup.GetComponent<Rotation>().rotateRight (false);
		else if (XCI.GetButtonDown(XboxButton.LeftBumper, m_controller))
			actualGroup.GetComponent<Rotation>().rotateLeft (false);
		

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

	//Spawn the next group
	GameObject spawnNext()
	{
		int i = Random.Range(0, groups.Length);

		GameObject next = Instantiate(groups[i], m_spawn, Quaternion.identity, cA.transform);
		next.GetComponent<Rotation>().goRight = m_goRight;

		return next;
	}

	void move(Vector3 dir)
	{
		if (actualGroup != null)
		{
			actualGroup.transform.position += dir; 

			if (!cA.updateArrayBool(m_goRight, actualGroup))
			{
				actualGroup.transform.position -= dir; 
				ManageAudio.instance.playCantMove();

				if(dir == m_direction)
					spawnNew();
			}
		}
	}

	//Handle spawning a new group and check if there is any intersection after spawning
	private void spawnNew()
	{
		actualGroup.GetComponent<Rotation> ().isActive = false; 

		actualGroup = spawnNext();

		if(m_goRight)
			actualGroup.GetComponent<Rotation>().rotateLeft (false);
		else
			actualGroup.GetComponent<Rotation>().rotateRight (false);

		actualGroup.GetComponent<Rotation> ().isActive = true;

		// GameOver
		if (!cA.updateArrayBool(m_goRight))
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		else
			cA.checkForFullLine (m_direction);
	}
}