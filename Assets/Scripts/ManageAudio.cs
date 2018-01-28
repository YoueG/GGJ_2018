using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageAudio : MonoBehaviour {
	static public ManageAudio instance;

	public AudioSource music, fullLine, cantMove, collision, turn, validate;

	public AudioClip[] clip_music, clip_fullLine, clip_cantMove, clip_collision, clip_turn, clip_validate;

	void Start()
	{
		instance = this;
	}

	public void PlayMusic(int id)
	{
		music.clip = clip_music[id];
		music.Play (); 
	}

	public void PlayValidate()
	{
		validate.clip = clip_validate[Random.Range(0, clip_validate.Length)];
		validate.Play (); 
	}

	public void playTurn(){
		turn.clip = clip_turn[Random.Range(0, clip_turn.Length)];
		turn.Play (); 
	}

	public void blocsCollide(){
		collision.clip = clip_collision[Random.Range(0, clip_collision.Length)];
		collision.Play (); 
	}

	public void playCantMove(){
		cantMove.Play (); 
	}

	public void PlayFullLine(){
		fullLine.clip = clip_fullLine[Random.Range(0, clip_fullLine.Length)];
		fullLine.Play (); 
	}
}
