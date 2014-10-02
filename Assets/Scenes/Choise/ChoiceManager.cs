using UnityEngine;
using System.Collections;

public class ChoiceManager : MonoBehaviour {

	public static GameData gameData = null;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = -1;

		if (gameData == null) {
			string data = PlayerPrefs.GetString ("game-data");
			if (data == null) {
				gameData = new GameData ();
			} else {
				gameData = GameData.gameDataWithString (data);
			}
		} else {
			PlayerPrefs.SetString ("game-data",gameData.ToString());
		}
		GameObject.Find ("textbox").GetComponent<UIInput>().value = gameData.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play(){
		gameData =  GameData.gameDataWithString (GameObject.Find ("textbox").GetComponent<UIInput>().value);

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
