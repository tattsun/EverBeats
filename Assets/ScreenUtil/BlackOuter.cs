using UnityEngine;
using System.Collections;

public class BlackOuter : MonoBehaviour {
	private static GameObject nowBlack;
	public float interval;
	public int depth;

	public static void show( float interval = 1 , int depth = 1000){
		GameObject black = (GameObject)Instantiate(Resources.Load("blackPrehab"));
		BlackOuter b = black.AddComponent<BlackOuter> ();
		b.interval = interval;
		b.depth = depth;
		nowBlack = black;
	}
	public static void dismiss(float interval = 0){
		nowBlack.GetComponent<BlackOuter> ().Dismiss (interval);
	}

	public void Dismiss( float duration ){
		if (duration == 0) {
			interval = duration;
		}
		ScreenUtil.fadeUI (gameObject , duration , 0 , 1 , 0 );
		SimpleTimer.setTimer (duration, () => { Destroy(gameObject); });
	}

	// Use this for initialization
	void Start () {
		GetComponent<UIWidget> ().depth = depth;
		ScreenUtil.fadeUI (gameObject , interval , 0 , 0 , 1 );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
