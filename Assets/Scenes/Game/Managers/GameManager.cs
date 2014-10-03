using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	static public string returnScene;
	static public GameManager manager;
	static public int score;
	static public GameData gameData;

	public ResultData result;
	bool endFrag = false;


	// Use this for initialization
	void Awake(){
		manager = this;
		if ( gameData == null) {
			gameData = GameData.sampleData();
		}
	}
	void Start () {
		score = 0;
		result = new ResultData ();
		GameObject.Find ("Title").GetComponent<UILabel> ().text = gameData.summery.title_en;
		GameObject.Find ("Lv").GetComponent<UILabel> ().text = "lv " + gameData.summery.lv;
	}

	//call from loaderManager
	public void DoneLoading(){
		Camera.main.GetComponent<AudioSource> ().Play ();
	}
	
	// Update is called once per frame
	void Update () {
		if (NoteManager.manager.audio.time > gameData.summery.playtime && !endFrag && !NoteManager.isEditMode) {
			SimpleTimer.setTimer(3, ()=>{
				saveAndExit();
			});
			endFrag = true;
		}
	}

	public void saveAndExit(){
		if (NoteManager.isEditMode) {
			MusicData music = NoteManager.manager.exportMusicData ();
			gameData.musicdata = music.ToString ();
			gameData.summery.playtime = (int)NoteManager.manager.audio.time + 1;
			gameData.summery.lv = music.GetLv (gameData.summery.playtime);
			gameData.summery.date = ScreenUtil.dateConvert (System.DateTime.Now);
			ChoiceManager.saveDebugData (gameData);
		} else {
			result.bad = NoteManager.manager.music.notes.Count - result.good - result.great;
			result.score = score;
			result.excute();
			ResultManager.result = result;
		}
		Application.LoadLevel (returnScene);
	}

}
