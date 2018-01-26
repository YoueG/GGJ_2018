using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageAudio : MonoBehaviour {
	static public ManageAudio instance;

	public AudioSource fullLine, cantMove;

	void Start()
	{
		instance = this;
	}

	public void playCantMove(){
		cantMove.Play (); 
	}

	public void PlayFullLine(){
		fullLine.Play (); 
	}
}
