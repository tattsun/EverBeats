using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {
	public static ScoreManager manager;
	private float sumPoint;
	public float leftPoint;

	// Use this for initialization
	void Start () {
		sumPoint = 0;
		leftPoint = 0;
		manager = this;
	}
	
	// Update is called once per frame
	void Update () {
		// leftPointが０になるまで数字は連続的にふえる様なアニメーション
		if (leftPoint > 1000){
			leftPoint -= 100;
			sumPoint += 100;
		}else if (leftPoint > 100){
			leftPoint -= 10;
			sumPoint += 10;
		}else if (leftPoint > 50){
			leftPoint -= 5;
			sumPoint += 5;
		}else if (leftPoint > 0){
			leftPoint--;
			sumPoint++;
		}
		GetComponent<UILabel>().text = string.Format("{0:0000000}", sumPoint);
	}

}
