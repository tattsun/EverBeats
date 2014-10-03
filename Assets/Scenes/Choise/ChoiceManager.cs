using UnityEngine;
using System.Collections;

public class ChoiceManager : MonoBehaviour {
	private GameData gameData;
	int selected;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = -1;
		selected = loadDebugData ().Length - 1;
		gameData = loadDebugData ()[ selected ] ;
		reflesh ();
	}
	void reflesh(){
		MusicData.Level lv = new MusicData (gameData.musicdata).GetLvObj (gameData.summery.playtime);
		GameObject.Find ("mainlabel").GetComponent<UILabel> ().text = 
			"曲名　　　　:"+gameData.summery.title+"\n作成日　　　:"+gameData.summery.date+"\nレベル　　　:"+lv.lv+"\n密度ポイント:"+lv.pt_dens+"\n時間ポイント:"+lv.pt_time+"\n長打ポイント:"+lv.pt_long;
		GameObject.Find ("textbox").GetComponent<UIInput>().value = gameData.ToString ();
	}
	public void next (){
		selected = Mathf.Min (  loadDebugData ().Length - 1, selected + 1);
		gameData = loadDebugData ()[selected];
		reflesh ();
	}
	public void prev (){
		selected = Mathf.Max (0, selected - 1);
		gameData = loadDebugData ()[selected];
		reflesh ();
	}
	public void delete (){
		PlayerPrefs.SetString ("GameData" , PlayerPrefs.GetString("GameData").Replace( gameData.ToString() , "" ) );
		selected = Mathf.Max (0, selected - 1);
		gameData = loadDebugData ()[selected];
		reflesh ();
	}

	public static GameData[] loadDebugData(){
		GameData[] debug;
		try{
			string[] arr = PlayerPrefs.GetString ("GameData").Split(new string[] {"<!SEPALATOR!>"},System.StringSplitOptions.RemoveEmptyEntries);
			debug = new GameData[ arr.Length ];
			for ( int i=0 ; i < arr.Length ; i++ ){
				debug[i] = GameData.gameDataWithString( arr[i] );
			}
		}catch{
			return new GameData[]{new GameData ()};
		}
		return debug;
	}
	public static void saveDebugData( GameData data  ){
		string datas = PlayerPrefs.GetString("GameData");
		if (datas == null){
			datas = new GameData().ToString();
		}
		PlayerPrefs.SetString ("GameData", datas + "<!SEPALATOR!>" + data.ToString ());
	}

	public void Play(){
		GameManager.gameData = gameData;
		GameManager.returnScene = "Choice";
		NoteManager.isEditMode = false;
		Application.LoadLevel ("Game");
	}
	
	public void Make(){
		gameData = new GameData ();
		GameManager.gameData = gameData;
		GameManager.returnScene = "Choice";
		NoteManager.isEditMode = true;
		Application.LoadLevel ("Game");
	}
}
