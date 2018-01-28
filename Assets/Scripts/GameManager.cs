using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public GameObject window, windowPause, pauseBtn;

	[SerializeField]
	GameObject victoryCapital, victoryCommunism;

	// Use this for initialization
	void Awake ()
	{
		Time.timeScale = 0; 
	}

	public void OnClickStart(){
		Time.timeScale = 1; 
		window.SetActive (false);

		ManageAudio.instance.PlayValidate();
		ManageAudio.instance.PlayMusic(0);

		foreach (var mov in FindObjectsOfType<PlayerController>())
		{
			mov.startGame();
		}
	}

	public void Victory(bool goRight)
	{
		if(goRight)
			victoryCapital.active = true;
		else
			victoryCommunism.active = true;

		ManageAudio.instance.PlayMusic(1);
			

		foreach (var mov in FindObjectsOfType<PlayerController>())
		{
			mov.enabled = false;
		}
	}

	public void OnClickExit(){
		Application.Quit (); 
	}

	public void OnClickRestart(){
		windowPause.SetActive (false); 
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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