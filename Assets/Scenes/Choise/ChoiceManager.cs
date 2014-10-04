using UnityEngine;
using System.Collections;

public class ChoiceManager : MonoBehaviour {
	private GameData gameData;
	int selected;

	// Use this for initialization
	void Start () {
		selected = loadDebugData ().Length - 1;
		gameData = loadDebugData ()[ selected ] ;
		if ( getDataFromDate( loadDebugData() , "2000-01-01 00:00:00" ) == null){
			saveDebugData(GameData.gameDataWithString(((TextAsset)Resources.Load("note_sample")).text));
		}
		reflesh ();
	}
	void reflesh(){
		MusicData.Level lv = new MusicData (gameData.musicdata).GetLvObj (gameData.summery.playtime);
		GameObject.Find ("mainlabel").GetComponent<UILabel> ().text = 
			"曲名　　　　:"+gameData.summery.title+"\n作成日　　　:"+gameData.summery.date+"\nハイスコア　:"+gameData.summery.highScore+"\n時間　　　　:"+gameData.summery.playtime/60+":"+gameData.summery.playtime%60+"\nレベル　　　:"+lv.lv+"\n密度ポイント:"+lv.pt_dens+"\n時間ポイント:"+lv.pt_time+"\n長打ポイント:"+lv.pt_long;
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
		DeleteData (gameData);
		selected = Mathf.Max (0, selected - 1);
		gameData = loadDebugData ()[selected];
		reflesh ();
	}
	public static void DeleteData( GameData d ){
		GameData[] ds = loadDebugData ();
		GameData target = getDataFromDate (ds , d.summery.date);
		GameData[] newds = new GameData [ds.Length - 1];
		int i = 0;
		foreach ( GameData d2 in ds ){
			if (d2 != target){
				newds[i] = d2; 
				i++;
			}
		}
		replaceDebugDatas(newds);
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
	public static void replaceDebugDatas( GameData[] datas ){
		string[] strs = new string[datas.Length];
		for ( int i= 0 ; i<datas.Length ; i++ ){
			strs[i] = datas[i].ToString();
		}
		PlayerPrefs.SetString ("GameData", ScreenUtil.Join(strs , "<!SEPALATOR!>"));
	}
	public static GameData getDataFromDate( GameData[] datas , string date){
		foreach ( GameData g in datas ){
			if (g.summery.date.Equals(date)){
				return g;
			}
		}
		Debug.LogWarning ("DATA NOT FOUND!");
		return null;
	}


	public void Play(){
		GameManager.gameData = gameData;
		GameManager.returnScene = "Result";
		ResultManager.returnScene = "Choice";
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
