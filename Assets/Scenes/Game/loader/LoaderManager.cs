using UnityEngine;
using System.Collections;

public class LoaderManager : MonoBehaviour {
	private GameObject black_mask;
	private GameObject black_f;
	private GameObject black_b;
	private GameObject loading_text;
	private bool forceDone;
	private bool finishFrag;
	private float startedTime;

	// Use this for initialization
	void Start () {
		black_mask = ScreenUtil.findObject (transform , "loadblack");
		loading_text = ScreenUtil.findObject (transform , "loading");
		black_b = ScreenUtil.findObject (transform , "black_b");
		black_f = ScreenUtil.findObject (transform , "black_f");


		finishFrag = false;
		forceDone = false;
		TubeDownloader.myDelegate = downLoadDone;
		startedTime = Time.time;
	}

	void Update () {
		if (finishFrag){
			loading_text.GetComponent<UIWidget>().alpha = 1;
			return;
		}
		loading_text.GetComponent<UIWidget>().alpha = Mathf.Abs(Mathf.Sin( Time.time - startedTime ));

		if (forceDone) {
			black_mask.GetComponent<UIAnchor>().relativeOffset += Vector2.right * Time.deltaTime * 0.7f;
		} else {
			black_mask.GetComponent<UIAnchor>().relativeOffset = new Vector2( ( Time.time - startedTime )/10.0f ,black_mask.GetComponent<UIAnchor>().relativeOffset.y);
		}
		if (forceDone && black_mask.GetComponent<UIAnchor>().relativeOffset.x > 1.05f){
			finishFrag = true;
			ScreenUtil.moveUI(loading_text , new Vector2(0.3f,0) , 1 , ScreenUtil.CURVEMODE_EASEOUT ,false ,0 , false);
			ScreenUtil.fadeUI(loading_text , 1 , 0 , 1 , 0 );
			ScreenUtil.fadeUI(black_f , 1 , 0 , 1 , 0 );
			ScreenUtil.fadeUI(black_b , 2 , 2 , 1 , 0 );

			SimpleTimer.setTimer(5,()=>{
				Destroy (gameObject);
				GameManager.manager.DoneLoading();
			});
		}


	}

	void downLoadDone(){
		forceDone = true;
	}
}
