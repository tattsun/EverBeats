using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	static public string returnScene;
	static public GameManager manager;
	static public int score;
	static public GameData gameData;

	public GameObject pausePrehab;
	internal ResultData result;

	GameObject pause_btn;
	bool nowPausing;
	bool endFrag;


	// Use this for initialization
	void Awake(){
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;

		manager = this;
		if ( gameData == null) {
			gameData = GameData.sampleData();
		}
	}
	void Start () {
		score = 0;
		nowPausing = false;
		endFrag = false;
		result = new ResultData ();
		GameObject.Find ("Title").GetComponent<UILabel> ().text = gameData.summery.title_en;
		GameObject.Find ("Lv").GetComponent<UILabel> ().text = "lv " + gameData.summery.lv;
		pause_btn = GameObject.Find ("pause_btn");
		pause_btn.SetActive (false);
	}

	//call from loaderManager
	public void DoneLoading(){
		Camera.main.GetComponent<AudioSource> ().Play ();
		pause_btn.SetActive (true);
		ScreenUtil.moveUI(pause_btn, new Vector2(-0.1f , 0) , 0.5f, ScreenUtil.CURVEMODE_EASEOUT , false , 0 , true);
		ScreenUtil.fadeUI(pause_btn, 0.5f , 0 , 0 , 1 );
		
		Debug.Log("MUSIC LENGTH : "+(int)NoteManager.manager.audio.clip.length);
	}
	
	// Update is called once per frame
	void Update () {
		if (( NoteManager.manager.audio.time > gameData.summery.playtime || NoteManager.manager.audio.time > NoteManager.manager.audio.clip.length - 1 ) && !endFrag && !NoteManager.isEditMode ) {
			endFrag = true;
			bool isFullCombo = (NoteManager.manager.music.notes.Count == result.good + result.great);
			if (isFullCombo){
				SimpleTimer.setTimer(1,()=>{
					ComboManager.instance.ShowFullCombo();
					setGameQuit(2);
				});
			}else{
				setGameQuit(2);
			}
		}
		if (NoteManager.manager.audio.time > NoteManager.manager.audio.clip.length-1 && !endFrag && NoteManager.isEditMode) {
			endFrag = true;
			setGameQuit(3);
		}
	}
	void setGameQuit(float delay){
		ScreenUtil.moveUI(pause_btn, new Vector2(0.1f , 0) , 0.5f, ScreenUtil.CURVEMODE_EASEOUT , true , 0 , false);
		ScreenUtil.fadeUI(pause_btn, 0.5f , 0 , 1 , 0 );
		nowPausing = true;
		SimpleTimer.setTimer(delay, ()=>{
			_gameQuit();
		});
	}

	public void Pause(){
		if (!nowPausing){
			ScreenUtil.moveUI(pause_btn, new Vector2(0.1f , 0) , 0.5f, ScreenUtil.CURVEMODE_EASEOUT , true , 0 , false);
			ScreenUtil.fadeUI(pause_btn, 0.5f , 0 , 1 , 0 );
			nowPausing = true;
			Instantiate (pausePrehab);
			NoteManager.manager.audio.Pause();
		}
	}
	public void OnPauseRestored(){
		if (nowPausing){
			pause_btn.SetActive(true);
			ScreenUtil.moveUI(pause_btn, new Vector2(-0.1f , 0) , 0.5f, ScreenUtil.CURVEMODE_EASEOUT , false , 0 , true);
			ScreenUtil.fadeUI(pause_btn, 0.5f , 0 , 0 , 1 );
			nowPausing = false;
			NoteManager.manager.audio.Play();
		}
	}

	public void Retry(){
		endFrag = false;
		NoteManager.manager.audio.time = 0;
		NoteManager.manager.notes.RemoveAll ((MusicData.NoteData n) => {
			Destroy(n.gameObject);
			return true;
		});
		ComboManager.instance.GetCombo (MusicData.NoteData.NotePhase.Miss);
		NoteManager.manager.music = new MusicData (gameData.musicdata);
		NoteManager.manager.index = 0;

		result = new ResultData ();
		score = 0;
	}
	
	public void gameQuit(){
		_gameQuit ();
	}
	private void _gameQuit(){
		BlackOuter.show(2);
		AudioUtil.crossfade ( 2 , Camera.main.gameObject);
		SimpleTimer.setTimer(3, ()=>{
			saveAndExit();
		});
	}
	public void saveAndExit(){
		if (NoteManager.isEditMode) {
			MusicData music = NoteManager.manager.exportMusicData ();
			gameData.musicdata = music.ToString ();
			if (!endFrag){
				Debug.Log("no max time ");
				gameData.summery.playtime = (int)NoteManager.manager.audio.time + 1;
			}else{
				Debug.Log("max time ");
				gameData.summery.playtime = (int)NoteManager.manager.audio.clip.length;
			}
			gameData.summery.lv = music.GetLv (gameData.summery.playtime);
			gameData.summery.date = ScreenUtil.dateConvert (System.DateTime.Now);
			ChoiceManager.saveDebugData (gameData);
		} else {
			result.bad = NoteManager.manager.music.notes.Count - result.good - result.great;
			result.score = score;
			result.excute();
			ResultManager.result = result;
		}
		if (returnScene == null) {
			Debug.LogWarning ("[QUIT GAME] ReturnScene is null");
			Application.LoadLevel ("Result");
		} else {
			Application.LoadLevel (returnScene);
		}
	}

}
