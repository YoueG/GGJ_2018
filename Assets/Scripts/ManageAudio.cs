using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageAudio : MonoBehaviour {
	static public ManageAudio instance;

	public AudioSource fullLine, cantMove, collision, move, turn;

	void Start()
	{
		instance = this;
	}

	public void playTurn(){
		turn.Play (); 
	}

	public void playMove(){
		move.Play (); 
	}

	public void blocsCollide(){
		collision.Play (); 
	}

	public void playCantMove(){
		cantMove.Play (); 
	}

	public void PlayFullLine(){
		fullLine.Play (); 
	}
}
