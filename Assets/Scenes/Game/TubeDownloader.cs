using UnityEngine;
using System.Collections;

public class TubeDownloader : MonoBehaviour {
	public delegate void  TubeDownloaderDelegate ();
	public bool isDebugMode = false;
	public GameObject white_gage;
	public GameObject score;
	public static TubeDownloaderDelegate myDelegate;
	public GameObject loaderPrehab;
	private GameObject loader;

	// Use this for initialization
	void Start () {
		score 		= GameObject.Find ("score");
		white_gage	= GameObject.Find ("gage_white");
		if (isDebugMode) {
			GameManager.manager.DoneLoading();
		} else {
			loader = (GameObject)Instantiate (loaderPrehab);
			StartCoroutine ("Load");
		}
	}
	
	// Update is called once per frame
	void Update () {
		float max = 1.05f;
		float min = 0.03f;
		float x = 0;
		if (NoteManager.isEditMode) {
			x = (max - min) * (NoteManager.manager.audio.time / NoteManager.manager.audio.clip.length) + min;
		} else {	
			x = (max - min )* (NoteManager.manager.audio.time / GameManager.gameData.summery.playtime) + min;
		}
		white_gage.GetComponent<UIAnchor>().relativeOffset = new Vector2(  x, white_gage.GetComponent<UIAnchor>().relativeOffset.y);
	}

	private IEnumerator Load()
	{
		Debug.Log("[BGM] LOAD STARTED..");
		//get JSON
/*
		WWW json = new WWW("http://youtubeinmp3.com/fetch/?api=advanced&format=JSON&video=http://www.youtube.com/watch?v="+GameManager.gameData.videoid);
		yield return json;
		JSONResponse r = LitJson.JsonMapper.ToObject<JSONResponse> (json.text);
		Debug.Log ("[BGN] data fetched :"+r.link);
		WWW www = new WWW(r.link + "&dammy=dammy.mp3");*/
		//WWW www = new WWW("http://YouTubeInMP3.com/fetch/?video=http://www.youtube.com/watch?v="+GameManager.gameData.videoid+ "&dammy=dammy.mp3");
		WWW www = new WWW("http://kamasu.jp/everbeats/musics/test_bgm.mp3");
		yield return www;
		Camera.main.GetComponent<AudioSource> ().clip = (www.audioClip);
		Debug.Log("[BGM] LOAD COMPLETE!");
		if (myDelegate != null){
			myDelegate();
		}
	}
	[System.Serializable]
	class JSONResponse {
		public string title;
		public string length;
		public string link;
	}
}
