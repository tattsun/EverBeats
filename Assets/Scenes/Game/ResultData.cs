using UnityEngine;
using System.Collections;

[System.Serializable]
public class ResultData{

	public int score;
	public int maxCombo;
	public int great;
	public int good;
	public int bad;

	public bool isNormCleared;
	public float ratio;
	public string rank;
	public int notenum;

	
	public static ResultData sample(){
		ResultData r = new ResultData ();
		r.great = 120;
		r.good = 12;
		r.bad = 5;
		r.maxCombo = 34;
		r.score = 12415;
		r.excute ();
		return r;
	}


	public ResultData(){
		great = 0;
		good = 0;
		bad = 0;
		maxCombo = 0;
	}
	public string GetDebugString(){
		return "SCORE: " + score + "\n"
				+ "RATIO: " + ratio.ToString ("f2") + "%\n"
				+ "(GREAT " + great + "/GOOD " + good + "/BAD " + bad + ")\n"
				+ "MAX-COMBO: " + maxCombo + "\n"
				+ "RANK: " + rank + "\n"
				+ "NORM: " + (isNormCleared ? "CLEAR" : " - ") + "\n";
	}
	public void excute(){
		notenum = great + good + bad;
		ratio = ((float)(great *10 + good*9)) / ((float)(notenum*10)) * 100;

		if ( good == 0 && bad == 0 ){
			rank = "SS";
		}else if (ratio > 95f){
			rank = "S";
		}else if (ratio > 90f){
			rank = "A";
		}else if (ratio > 80f){
			rank = "B";
		}else if (ratio > 65f){
			rank = "C";
		}else if (ratio > 50f){
			rank = "D";
		}else{
			rank = "E";
		}

		if (ratio > 85f) {
			isNormCleared = true;
		}
	}
}
