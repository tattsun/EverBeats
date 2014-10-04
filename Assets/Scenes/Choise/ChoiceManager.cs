using UnityEngine;
using System.Collections;

public class ChoiceManager : MonoBehaviour {
	private GameData gameData;
	int selected;
	bool EndFrag;

	// Use this for initialization
	void Start () {
		EndFrag = false;
		selected = loadDebugData ().Length - 1;
		gameData = loadDebugData ()[ selected ] ;
		GameData[] datas = loadDebugData ();
		if ( getDataFromDate( datas , "2000-01-01 00:00:00" ) == null){
			saveDebugData(GameData.gameDataWithString(((TextAsset)Resources.Load("note_sample")).text));
		}
		foreach (GameData g in datas){
			MusicData m = new MusicData(g.musicdata);
			if (g.summery.playtime < 5 && m.notes.Count > 50){
				g.summery.playtime = 600;
				g.summery.lv = m.GetLv( g.summery.playtime );
				if (!g.summery.title.EndsWith("[F]")){
					g.summery.title += "[F]"; 
				}
			}
		}
		replaceDebugDatas(datas);
		reflesh ();
	}
	void reflesh(){
		Debug.Log ("VIDEO ID : " + gameData.summery.videoid);
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
		if (EndFrag){
			return;
		}
		EndFrag = true;

		
		GameManager.gameData = gameData;
		GameManager.returnScene = "Result";
		ResultManager.returnScene = "Choice";
		NoteManager.isEditMode = false;
		BlackOuter.show(1);
		SimpleTimer.setTimer(1, ()=>{
			Application.LoadLevel ("Game");
		});

	}
	
	public void Make(){
		if (EndFrag){
			return;
		}
		EndFrag = true;
		string videoid = gameData.summery.videoid;
		gameData = new GameData ();
		gameData.summery.videoid = videoid;
		GameManager.gameData = gameData;
		GameManager.returnScene = "Choice";
		NoteManager.isEditMode = true;
		BlackOuter.show(1);
		SimpleTimer.setTimer(1, ()=>{
			Application.LoadLevel ("MusicSelect");
		});
	}
}
