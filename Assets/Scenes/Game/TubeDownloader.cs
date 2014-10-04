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
		string url = null;
		if (!GameManager.gameData.summery.videoid.Equals (GameData.sampleData ().summery.videoid)) {
			LoaderManager.estimate_time = 30;
			Debug.Log ("[BGM] video id :" + GameManager.gameData.summery.videoid);
			Debug.LogWarning ("http://youtubeinmp3.com/fetch/?video=http://www.youtube.com/watch?v=" + GameManager.gameData.summery.videoid);
			WWW json = new WWW ("http://YoutubeInMP3.com/fetch/?api=advanced&format=JSON&video=http://www.youtube.com/watch?v=" + GameManager.gameData.summery.videoid);
			yield return json;
			Debug.Log ("[BGM] data fetched :" + json.text);
			JSONResponse r = null;
			try {
				r = LitJson.JsonMapper.ToObject<JSONResponse> (json.text);
				GameManager.gameData.summery.title = r.title;
				GameManager.gameData.summery.title_en = r.title;
				url = r.link + "&dammy=dammy.mp3";
			}catch{
				//Debug.LogWarning ("VIDEO-Info not found");
				//string url_2 = ScreenUtil.SearchAndGetString(json.text , "<a style='width:150px;height:30px;font-size:12px;margin-top:25px;' class='download' href='" , "'>Convert And<br /> Download MP3</a><br />") ; 
				//if ( url_2 != null ){
				//	Debug.Log ("VIDEO-URL found!");
				//	url = url_2 + "&dammy=dammy.mp3";
				//}else{
				Debug.LogWarning ("VIDEO-URL NOR found!");
				GameManager.gameData.summery.title = "Unknown";
				GameManager.gameData.summery.title_en = "Unknown";

				url = "http://youtubeinmp3.com/fetch/?video=http://www.youtube.com/watch?v="+GameManager.gameData.summery.videoid;
				//}
			}
		} else {
			LoaderManager.estimate_time = 10;
			url = "http://kamasu.jp/everbeats/musics/test_bgm.mp3";
		}
		Debug.LogWarning (url);
		WWW www = new WWW(url);
		//WWW www = new WWW("http://YouTubeInMP3.com/fetch/?video=http://www.youtube.com/watch?v="+GameManager.gameData.videoid+ "&dammy=dammy.mp3");
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
