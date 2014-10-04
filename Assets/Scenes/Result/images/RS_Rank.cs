using UnityEngine;
using System.Collections;

public class RS_Rank : MonoBehaviour {
	public string value;

	// Use this for initialization
	void Start () {
		ScreenUtil.fadeUI (ScreenUtil.findObject (transform, "Rank"), ResultManager.fadeDuration, 0, 0, 1);
		ScreenUtil.fadeUI (ScreenUtil.findObject (transform, "rankPoint"), ResultManager.fadeDuration, 1, 0, 1);
		ScreenUtil.findObject (transform, "rankPoint").GetComponent<UILabel> ().text = value;
	}
}
