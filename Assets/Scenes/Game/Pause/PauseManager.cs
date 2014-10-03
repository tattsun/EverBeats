using UnityEngine;
using System.Collections;

public class PauseManager : MonoBehaviour {

	bool endFrag;
	GameObject back;

	// Use this for initialization
	void Awake () {
		endFrag = false;

		back = ScreenUtil.findObject (transform , "pause_back" );
		ScreenUtil.fadeUI (back, 1, 0, 0, 1);

		float unit = 0.1f;
		float offset = 0f;
		ApplyAnimation ( "resume" , true ,  offset);
		ApplyAnimation ( "retry" , true , unit + offset);
		ApplyAnimation ( "back" , true ,  unit*2 +offset);

		ScreenUtil.fadeUI ( ScreenUtil.findObject (transform , "p_title" ) , 1, offset , 0, 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Back(){
		if (endFrag){
			return;
		}
		endFrag = true;

		GameManager.manager.saveAndExit ();
	}
	public void Resume(){
		if (endFrag){
			return;
		}
		endFrag = true;

		ScreenUtil.fadeUI (back, 1, 0, 1, 0);
		float unit = 0.1f;
		float offset = 0f;
		ApplyAnimation ( "resume" , false , offset);
		ApplyAnimation ( "retry" , false , unit + offset);
		ApplyAnimation ( "back" , false ,  unit*2 +offset);
		
		ScreenUtil.fadeUI ( ScreenUtil.findObject (transform , "p_title" ) , 1, offset , 1, 0);

		SimpleTimer.setTimer (1,()=>{
			Destroy(gameObject);
			GameManager.manager.OnPauseRestored ();
		});
	}
	public void Retry(){
		if (endFrag){
			return;
		}
		GameManager.manager.Retry ();
		Resume ();
	}

	private void ApplyAnimation( string name , bool isStart, float delay ){
		float interval = 0.7f;
		GameObject btn = ScreenUtil.findObject (transform , "p_btn_"+name );
		GameObject label = ScreenUtil.findObject (transform , "p_label_"+name );
		
		if (isStart) {
			ScreenUtil.moveUI (btn, new Vector2 (0, 0.1f), interval, ScreenUtil.CURVEMODE_EASEOUT, false, delay, true);
			ScreenUtil.fadeUI (btn, interval, delay, 0, 1);
			ScreenUtil.fadeUI (label, interval, delay, 0, 1);
		} else {
			ScreenUtil.moveUI (btn, new Vector2 (0, -0.1f), interval, ScreenUtil.CURVEMODE_EASEOUT, true, delay, false);
			ScreenUtil.fadeUI (btn, interval, delay, 1, 0);
			ScreenUtil.fadeUI (label, interval, delay, 1, 0);
		}

	}
}













