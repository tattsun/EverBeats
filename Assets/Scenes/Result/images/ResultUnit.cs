using UnityEngine;
using System.Collections;

public class ResultUnit : MonoBehaviour {

	public bool isPercent = true;
	public int maxvalue = 100;
	public float value = 57.34f;
	public string label_name = "Ratio";

	private float countUpInterval = 2;
	private float pullUpInterval = 0.4f;
	private float pullUpStart = 0.6f;
	private float fadeInterval = 0.7f;
	private float LabelStart = 0f;
	private float LabelInterval = 1f;

	private float countUpSpeed;
	private float nowValue;

	private UILabel v_label;
	private UILabel m_label;
	private float startedTime;

	// Use this for initialization
	void Start () {
		startedTime = Time.time;
		v_label = ScreenUtil.findObject (transform, "point").GetComponent<UILabel> ();
		m_label = ScreenUtil.findObject (transform, "max").GetComponent<UILabel> ();
		ScreenUtil.findObject (transform, "name").GetComponent<UILabel> ().text = label_name;
	
		nowValue = 0;
		countUpSpeed = value / countUpInterval;
		ScreenUtil.findObject (transform, "percent").SetActive (isPercent);

		ScreenUtil.fadeUI (ScreenUtil.findObject (transform, "pointContainer"), fadeInterval , 0, 0, 1);
		ScreenUtil.fadeUI (ScreenUtil.findObject (transform, "fore"), fadeInterval , 0, 0, 1);
		/*
		ScreenUtil.moveUI (ScreenUtil.findObject(transform , "pointContainer") , new Vector2 (0, 0.03f), pullUpInterval, ScreenUtil.CURVEMODE_NONE, false, pullUpStart, true);
		ScreenUtil.moveUI (ScreenUtil.findObject(transform , "back") , new Vector2 (0, 0.12f), pullUpInterval, ScreenUtil.CURVEMODE_EASEOUT, false, pullUpStart, true);
		ScreenUtil.fadeUI (ScreenUtil.findObject (transform, "max"), pullUpInterval, pullUpStart, 0, 1);
		ScreenUtil.moveUI (ScreenUtil.findObject (transform, "max") , new Vector2 (0, 0.03f), pullUpInterval, ScreenUtil.CURVEMODE_NONE, false, pullUpStart, true);
		*/
		ScreenUtil.fadeUI (ScreenUtil.findObject (transform, "name"), LabelInterval , LabelStart, 0, 1);

		HidePullUp ();

		/*
		ScreenUtil.findObject (transform, "back").GetComponent<UIWidget> ().alpha = 0;
		SimpleTimer.setTimer (fadeInterval, () => {
			ScreenUtil.findObject (transform, "back").GetComponent<UIWidget> ().alpha = 1;
		});*/
	
	}

	// Update is called once per frame
	void Update () {
		if (Time.time - startedTime < countUpInterval) {
			float x = (Time.time - startedTime) / countUpInterval;
			//float easeOut = Mathf.Min (3.0f * x * x - 2.0f * x * x * x, 1f);
			float easeOut = Mathf.Min (1 - Mathf.Exp(-6 * x), 1f);
			nowValue = easeOut * value;
		} else {
			nowValue = value;
		}
		if (isPercent){
			v_label.text = nowValue.ToString("f2");
			m_label.text = "100%";
		}else{
			v_label.text = ""+((int)nowValue);
			m_label.text = ""+maxvalue;
		}
	}
	public void HidePullUp(){
		ScreenUtil.moveUI (ScreenUtil.findObject(transform , "pointContainer") , -new Vector2 (0, 0.03f), pullUpInterval, ScreenUtil.CURVEMODE_NONE, false, 0, false);
		ScreenUtil.moveUI (ScreenUtil.findObject(transform , "back") , -new Vector2 (0, 0.12f), pullUpInterval, ScreenUtil.CURVEMODE_EASEOUT, false, 0, false);
		ScreenUtil.moveUI (ScreenUtil.findObject (transform, "max") , -new Vector2 (0, 0.03f), pullUpInterval, ScreenUtil.CURVEMODE_NONE, false, 0, false);
		
		ScreenUtil.findObject (transform, "back").GetComponent<UIWidget> ().alpha = 0;
		ScreenUtil.findObject (transform, "max").GetComponent<UIWidget> ().alpha = 0;
	}
	public void PullUpStart(){
		ScreenUtil.moveUI (ScreenUtil.findObject(transform , "pointContainer") , new Vector2 (0, 0.03f), pullUpInterval, ScreenUtil.CURVEMODE_NONE, false, 0, false);
		ScreenUtil.moveUI (ScreenUtil.findObject(transform , "back") , new Vector2 (0, 0.12f), pullUpInterval, ScreenUtil.CURVEMODE_EASEOUT, false, 0, false);
		ScreenUtil.moveUI (ScreenUtil.findObject (transform, "max") , new Vector2 (0, 0.03f), pullUpInterval, ScreenUtil.CURVEMODE_NONE, false, 0, false);
		ScreenUtil.fadeUI (ScreenUtil.findObject (transform, "max"), pullUpInterval, 0, 0, 1);
		ScreenUtil.findObject (transform, "back").GetComponent<UIWidget> ().alpha = 1;
	}
}
