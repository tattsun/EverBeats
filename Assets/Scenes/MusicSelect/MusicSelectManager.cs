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
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
