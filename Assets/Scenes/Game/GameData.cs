using UnityEngine;
using System.Collections;
using LitJson;

[System.Serializable]
public class GameData{
	public string musicdata;
	public SummeryData summery;


	public GameData (){
		musicdata = "";
		summery = SummeryData.sample ();
	}
	public static GameData sampleData(){
		return ChoiceManager.loadDebugData () [0];
	}

	public static GameData gameDataWithString(string str){
		return JsonMapper.ToObject<GameData>(str);
	}
	// Use this for initialization
	public string ToString(){
		return JsonMapper.ToJson (this);
	}

}
