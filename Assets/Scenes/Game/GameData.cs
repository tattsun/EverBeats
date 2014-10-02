using UnityEngine;
using System.Collections;
using LitJson;

[System.Serializable]
public class GameData{
	public string musicdata;
	public string title;
	public int lv;
	public int length;
	public string videoid;

	public GameData (){
		musicdata = "";
		title = "Thunderclap";
		lv = 11;
		length = 100;
		videoid = "FLUC8aINF1c";
	}
	public static GameData sampleData(){
		GameData g = new GameData();
		g.musicdata = MusicData.testnotes ().ToString ();
		return g;
	}

	public static GameData gameDataWithString(string str){
		return JsonMapper.ToObject<GameData>(str);
	}
	// Use this for initialization
	public string ToString(){
		return JsonMapper.ToJson (this);
	}

}
