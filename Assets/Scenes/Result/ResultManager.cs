using UnityEngine;
using System.Collections;

public class ResultManager : MonoBehaviour {
	public static ResultData result;
	public static string returnScene;

	// Use this for initialization
	void Start () {
		GameObject.Find ("result").GetComponent<UILabel> ().text = result.GetDebugString ();
		if (GameManager.gameData.summery.highScore < result.score){
			GameManager.gameData.summery.highScore = result.score;
			GameData[] ds = ChoiceManager.loadDebugData();
			GameData target = ChoiceManager.getDataFromDate(ds , GameManager.gameData.summery.date );
			target.summery.highScore = result.score;
			ChoiceManager.replaceDebugDatas(ds);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void back(){
		Application.LoadLevel(returnScene);
	}
}
