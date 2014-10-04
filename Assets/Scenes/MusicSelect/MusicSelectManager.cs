using UnityEngine;
using System.Collections;

public class MusicSelectManager : MonoBehaviour {
	public void goGame(){
		if ( !GameObject.Find ("videoid").GetComponent<UIInput>().value.Equals("") ){
			GameManager.gameData.summery.videoid = GameObject.Find ("videoid").GetComponent<UIInput>().value;
		}
		Application.LoadLevel ("Game");
	}

	// Use this for initialization
	void Start () {
		try {
			Debug.Log ("VIDEO ID : " + GameManager.gameData.summery.videoid);
		}catch{
			Debug.LogWarning ("GAMEDATA NOT FOUND ");
			GameManager.gameData = GameData.sampleData();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
