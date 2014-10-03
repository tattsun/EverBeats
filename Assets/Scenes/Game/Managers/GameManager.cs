using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	static public string returnScene;
	static public GameManager manager;
	static public int score;
	static public GameData gameData;


	// Use this for initialization
	void Awake(){
		manager = this;
		if ( gameData == null) {
			gameData = GameData.sampleData();
		}
	}
	void Start () {
		score = 0;
		GameObject.Find ("Title").GetComponent<UILabel> ().text = gameData.summery.title_en;
		GameObject.Find ("Lv").GetComponent<UILabel> ().text = "lv " + gameData.summery.lv;
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
			MusicData music = NoteManager.manager.exportMusicData();
			gameData.musicdata = music.ToString();
			gameData.summery.playtime = (int)NoteManager.manager.audio.time + 1;
			gameData.summery.lv = music.GetLv(gameData.summery.playtime);
			gameData.summery.date = ScreenUtil.dateConvert (System.DateTime.Now );
			ChoiceManager.saveDebugData(gameData);
		}
		Application.LoadLevel (returnScene);
	}

}
