using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Movement : MonoBehaviour {

	[SerializeField]
	bool m_goRight;
	Vector3 m_direction;

	public float initTimestep = 0.2F;
	float timestep; 
	float time; 

	CubeArray cA; 

	//The actual group which can rotate and will move down
	public GameObject actualGroup; 


	void Start(){
		cA = FindObjectOfType<CubeArray>(); 
		timestep = initTimestep;

		m_direction = m_goRight ? Vector3.right : Vector3.left;
	}

	public void startGame(){
		actualGroup = this.gameObject.GetComponent<GroupSpawner> ().spawnNext ();
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

	void checkForInput(){
		if (Input.GetKeyDown (KeyCode.D)) {
			actualGroup.GetComponent<Rotation>().rotateRight (false); 
		} else if (Input.GetKeyDown (KeyCode.Q)) {
			actualGroup.GetComponent<Rotation>().rotateLeft (false); 
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			move (Vector3.left);
		} else if (Input.GetKeyDown (KeyCode.E)) {
			move (Vector3.right);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			timestep = 0.05F; 
		} else if (Input.GetKeyUp (KeyCode.Z)) {
			timestep = initTimestep;
		}
		cA.updateArrayBool ();
	}

	void move(Vector3 pos){
		if (actualGroup != null) {
			actualGroup.transform.position += pos; 
			if (!cA.updateArrayBool ()) {
				actualGroup.transform.position -= pos; 
				gameObject.GetComponent<ManageAudio> ().playCantMove (); 
				if(pos == m_direction){
					spawnNew (); 
				}
			}
		}
	}

	//Handle spawning a new group and check if there is any intersection after spawning
	private void spawnNew(){
		actualGroup.GetComponent<Rotation> ().isActive = false; 
		actualGroup = gameObject.GetComponent<GroupSpawner> ().spawnNext ();
		actualGroup.GetComponent<Rotation> ().isActive = true;
		if (!cA.updateArrayBool ()) {
			print ("GAME OVER!!!"); 
			//Theres a better way, but for now - keep it simple :) 
			Application.LoadLevel (Application.loadedLevelName); 
		} else {
			cA.checkForFullLine ();
		} 
	}
}