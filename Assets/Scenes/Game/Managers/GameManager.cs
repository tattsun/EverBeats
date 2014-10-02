﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public bool isDebugMode = false;
	static public string returnScene;
	static public GameManager manager;
	static public int score;
	static public GameData gameData;


	// Use this for initialization
	void Awake(){
		if ( gameData == null) {
			gameData = GameData.sampleData();
		}
	}
	void Start () {
		score = 0;
		manager = this;
	}

	//call from loaderManager
	public void DoneLoading(){
		Camera.main.GetComponent<AudioSource> ().Play ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void saveAndExit(){
		if (NoteManager.isEditMode){
			gameData.musicdata = NoteManager.manager.exportMusicData().ToString();
			gameData.length = (int)NoteManager.manager.audio.time + 1;
			ChoiceManager.gameData = gameData;
		}
		Application.LoadLevel (returnScene);
	}

}
