using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResultManager : MonoBehaviour {
	private static int ResultTypeBad 		 = 0;
	private static int ResultTypeExcellent	 = 1;
	private static int ResultTypeCombo		 = 2;
	private static int ResultTypeRatio		 = 3; 
	
	public static float fadeDuration = 1;

	public static ResultData result;
	public static string returnScene;
	public GameObject resultUnitPrehab;
	public GameObject RankPrehab;
	public GameObject ScorePrehab;
	public GameObject ButtonsPrehab;

	public List<ResultUnit> units;

	
	private float ResultStart = 1;
	private float unitStart = 1;
	private float scoreStart = -1;
	private float rankStart = 1;


	private float unitInterval = 1.2f;
	private float unitMoveStart = 0.8f;
	// Use this for initialization
	void Start () {
		//GameObject.Find ("result").GetComponent<UILabel> ().text = result.GetDebugString ();

		ScreenUtil.fadeUI (GameObject.Find ("result"), fadeDuration, ResultStart, 0, 1);

		if (result == null) {
			Debug.LogWarning ("NO RESULT");
			result = ResultData.sample();
		} else {
			if (GameManager.gameData.summery.highScore < result.score){
				//high score
				GameManager.gameData.summery.highScore = result.score;
				GameData[] ds = ChoiceManager.loadDebugData();
				GameData target = ChoiceManager.getDataFromDate(ds , GameManager.gameData.summery.date );
				target.summery.highScore = result.score;
				ChoiceManager.replaceDebugDatas(ds);
			}
		}

		units = new List<ResultUnit> ();


		for (int i = 0 ; i<4 ; i++){
			int index = i;
			SimpleTimer.setTimer ( unitStart + unitInterval*i, ()=>{
				if (units.Count > 0){
					units[units.Count - 1 ].PullUpStart();
				}
				addResults(index );
			} );
		}
		SimpleTimer.setTimer ( unitStart + unitInterval*4, ()=>{
			units[units.Count - 1 ].PullUpStart();
		});

		SimpleTimer.setTimer ( unitStart + unitInterval*4 + scoreStart, ()=>{
			RS_Score score = ((GameObject) Instantiate ( ScorePrehab )).GetComponent<RS_Score>();
			score.value = result.score;
		});
		SimpleTimer.setTimer ( unitStart + unitInterval*4 + scoreStart + rankStart, ()=>{
			RS_Rank rank = ((GameObject) Instantiate ( RankPrehab )).GetComponent<RS_Rank>();
			rank.value = result.rank;
		});
		SimpleTimer.setTimer ( unitStart + unitInterval*4 + scoreStart + rankStart + 1, ()=>{
			RS_Button button = ((GameObject) Instantiate ( ButtonsPrehab )).GetComponent<RS_Button>();
		});

	}

	void addResults ( int result_type  ){
		float value = new float[] { result.bad , result.great , result.maxCombo , result.ratio }[result_type];
		int max = new int[] { result.notenum , result.notenum , result.notenum , 100 }[result_type];
		bool percent = new bool[] { false, false , false, true }[result_type];
		string label_name = new string[] { "Bad","Excellent","Combo","Ratio"}[result_type];
		
		ResultUnit r = ((GameObject) Instantiate ( resultUnitPrehab )).GetComponent<ResultUnit>();
		r.isPercent = percent;
		r.label_name = label_name;
		r.maxvalue = max;
		r.value = value;
		ScreenUtil.moveUI (r.gameObject , GetVector( 0 , r ) , 0 , 0 , false , 0 , false );
		units.Add (r);

		int num = 0;
		for (int i = 1 ; i < 4-result_type ; i++){
			int toPos = i;
			SimpleTimer.setTimer( unitMoveStart + unitInterval * num,()=>{
				ScreenUtil.moveUI (r.gameObject , GetVector( toPos , r ) , unitInterval , ScreenUtil.CURVEMODE_EASEOUT_HARD , false , 0 , false );
			});
			num++;
		}
		r.transform.parent = GameObject.Find ("ResultContainer").transform;
	}

	Vector2 GetVector(int index , ResultUnit r){
		return GetPosition (index) - ScreenUtil.findObject (r.transform, "fore").GetComponent<UIAnchor> ().relativeOffset;
	}
	Vector2 GetPosition(int index){
		float margin = 0.1f;
		float unit = (1f - margin)/4;
		return new Vector2 ((unit * index + unit / 2 + margin / 2) - 0.5f, 0.02f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void back(){
		Application.LoadLevel(returnScene);
	}
}
