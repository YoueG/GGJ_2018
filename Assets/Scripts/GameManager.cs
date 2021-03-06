﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using XboxCtrlrInput;

public class GameManager : MonoBehaviour {
	public GameObject window, windowPause, pauseBtn, windowRestart, credit;

	[SerializeField]
	GameObject victoryCapital, victoryCommunism;

	// Use this for initialization
	void Awake ()
	{
		Time.timeScale = 0; 
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape) && XCI.GetButtonDown(XboxButton.Start))
			Application.Quit();
	}

	public void OnClickStart(){
		Time.timeScale = 1; 
		window.SetActive (false);

		ManageAudio.instance.PlayMusic(0);

		foreach (var mov in FindObjectsOfType<PlayerController>())
		{
			mov.startGame();
		}
	}

	public void Victory(bool goRight)
	{
		if(goRight)
		{
			ManageAudio.instance.PlayMusic(3);
			victoryCapital.active = true;
		}
			
		else
		{
			ManageAudio.instance.PlayMusic(2);
			victoryCommunism.active = true;
		}

		windowRestart.SetActive (true);
		FindObjectOfType<EventSystem>().SetSelectedGameObject(windowRestart.GetComponentInChildren<Button>().gameObject);

		foreach (var mov in FindObjectsOfType<PlayerController>())
		{
			mov.enabled = false;
		}
	}

	public void OnClickExit(){
		Application.Quit (); 
	}

	public void OnClickRestart(){
		windowRestart.SetActive (false);
		victoryCapital.active = false;
		victoryCommunism.active = false;

		foreach (Transform child in FindObjectOfType<GridManager>().transform)
			Destroy(child.gameObject);
		
		foreach (var mov in FindObjectsOfType<PlayerController>())
		{
			mov.startGame();
			mov.enabled = true;
		}

		ManageAudio.instance.PlayMusic(0);
	}

	public void OnClickPause(){
		pauseBtn.GetComponent<Button> ().interactable = false; 
		Time.timeScale = 0F;
		windowPause.SetActive (true); 
	}

	public void OnClickContinue(){
		pauseBtn.GetComponent<Button> ().interactable = true; 
		Time.timeScale = 1F;
		windowPause.SetActive (false); 
	}
}