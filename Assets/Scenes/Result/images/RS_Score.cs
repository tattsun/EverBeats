using UnityEngine;
using System.Collections;

public class RS_Score : MonoBehaviour {
	public float value;

	private UILabel v_label;
	private float startedTime;
	private float countUpInterval = 2;
	private float nowValue;

	// Use this for initialization
	void Start () {
		ScreenUtil.fadeUI (ScreenUtil.findObject (transform, "Score"), ResultManager.fadeDuration, 0, 0, 1);
		ScreenUtil.fadeUI (ScreenUtil.findObject (transform, "scorePoint"), ResultManager.fadeDuration, 0, 0, 1);
		v_label = ScreenUtil.findObject (transform, "scorePoint").GetComponent<UILabel> ();
		startedTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - startedTime < countUpInterval) {
			float x = (Time.time - startedTime) / countUpInterval;
			float easeOut = Mathf.Min (1 - Mathf.Exp(-6 * x), 1f);
			nowValue = easeOut * value;
		} else {
			nowValue = value;
		}
		v_label.text = ""+((int)nowValue);
	}
}
