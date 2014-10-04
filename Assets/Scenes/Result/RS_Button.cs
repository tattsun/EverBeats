using UnityEngine;
using System.Collections;

public class RS_Button : MonoBehaviour {
	bool EndFrag;

	// Use this for initialization
	void Start () {
		EndFrag = false;
		ScreenUtil.moveUI (ScreenUtil.findObject (transform, "retryBtn"), new Vector2 (0, -0.22f), 1, ScreenUtil.CURVEMODE_EASEOUT, false, 0, true);
		ScreenUtil.moveUI (ScreenUtil.findObject (transform, "backBtn"), new Vector2 (0, -0.22f), 1, ScreenUtil.CURVEMODE_EASEOUT, false, 0.2f, true);
		ScreenUtil.fadeUI (ScreenUtil.findObject (transform, "retryBtn"), 0.1f, 0, 0, 1);
		ScreenUtil.fadeUI (ScreenUtil.findObject (transform, "backBtn"), 0.1f, 0, 0, 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void BackToHome(){
		if (EndFrag){
			return;
		}
		EndFrag = true;
		BlackOuter.show(1);
		SimpleTimer.setTimer(1, ()=>{
			Application.LoadLevel (ResultManager.returnScene);
		});
	}
	
	public void Retry(){
		if (EndFrag){
			return;
		}
		EndFrag = true;
		BlackOuter.show(1);
		SimpleTimer.setTimer(1, ()=>{
			Application.LoadLevel ("Game");
		});
	}
}
