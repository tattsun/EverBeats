using UnityEngine;
using System;

// 音符が流れてきた結果を受け取って，コンボ数を判定して
// コンボ数を表示して，そのコンボ数を呼び出し元に返します。
public class ComboManager : MonoBehaviour {

	internal ComboManager instance;

	public GameObject numLabelPrehab;
	public GameObject comboLabelPrehab;

	// TODO ここは最終的には消します。最後のプレハブ化のときに
	public GameObject numLabel; // コンボ数を表示する為のNGUI
	public GameObject comboLabel; // コンボ数のよこを表示する為のNGUI

	private int comboSum = 0;

	/* 定数フィールド */
	private float defaultTextScale;
	private float maxTextScale;
	private float defaultNumScale;
	private float maxNumScale;
	private Vector2 startOffset;

	// 設定定数フィールド
	public float showLabelInterval = 1.0f;

	private float alphaPerSec = 0.01f;
	private float textScalePerSec = 0.01f;
	private float numScalePerSec = 0.01f;
	private int minComboNum = 5;

	private bool isStartAnimation = false;

	private float startTimer = 0.0f;

	enum AnimationPhase{
		Nothing, // 何もないとき
		Start, // comboの文字と数字がスケールダウンででてくる
		Middle, // comboの文字は変化無し，数字だけスケールダウンで新しくなる
		End // 横長にふっと消える
	}
	private AnimationPhase animationPhase = AnimationPhase.Nothing;

	void Start () {
		instance = this;
		defaultTextScale = comboLabel.GetComponent<UIStretch>().relativeSize.y;
		defaultNumScale = numLabel.GetComponent<UIStretch>().relativeSize.y;
		maxNumScale = defaultNumScale * 3.0f;
		maxTextScale = defaultTextScale * 1.5f;

		alphaPerSec = 1.0f / showLabelInterval;
		textScalePerSec = (maxTextScale - defaultTextScale) / showLabelInterval;
		numScalePerSec = (maxNumScale - defaultNumScale) / showLabelInterval;
		InitGUI();

	}


	void Update () {

		startTimer += Time.deltaTime;

		Debug.Log("Now Phase: <color=green>" + animationPhase + "</color>");

		switch (animationPhase) {
			case AnimationPhase.Nothing:
			InitGUI();
			break;

			case AnimationPhase.Start:
			isStartAnimation = true;
			setComboNumToGUI();
			break;

			case AnimationPhase.Middle:
			setComboNumToGUI();
			break;

			case AnimationPhase.End:
			InitGUI();
			setComboNumToGUI();
			break;
		}

		if (isStartAnimation) {
			/* 透明度のアニメーション */
			bool endAlpha = false;
			float alpha = (alphaPerSec * Time.deltaTime) + comboLabel.GetComponent<UILabel>().color.a;
			// Debug.Log("alpha: " + alpha);

			if (alpha > 1.0f){
				alpha = 1.0f;
				endAlpha = true;
			}

			comboLabel.GetComponent<UILabel>().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			numLabel.GetComponent<UILabel>().color = new Color (1.0f, 1.0f, 1.0f, alpha);

			/* スケールのアニメーション */
			bool endScale = false;
			// float nowTextScale = (comboLabel.GetComponent<UIStretch>().relativeSize.y + defaultTextScale)/2.0f;
			// float nowNumScale = (numLabel.GetComponent<UIStretch>().relativeSize.y + defaultNumScale)/2.0f;
			float nowTextScale = maxTextScale - GetTimeDuration(3.0f)*Mathf.Abs(defaultTextScale - maxTextScale);
			float nowNumScale = maxNumScale - GetTimeDuration(3.0f)*Mathf.Abs(defaultNumScale - maxNumScale);

			comboLabel.GetComponent<UIStretch>().relativeSize.y = nowTextScale;
			numLabel.GetComponent<UIStretch>().relativeSize.y = nowNumScale;

			if (Mathf.Abs(nowTextScale - defaultTextScale) < 0.0001f && Mathf.Abs(nowNumScale - defaultNumScale) < 0.0001f) {
				comboLabel.GetComponent<UIStretch>().relativeSize.y = defaultTextScale;
				numLabel.GetComponent<UIStretch>().relativeSize.y = defaultNumScale;
				endScale = true;
			}

			isStartAnimation = !(endScale && endAlpha);

			if (!isStartAnimation){
				startTimer = 0.0f;
			}

			// Debug.Log("anima " + alpha + "," + nowTextScale + "," + nowNumScale);
		}

		ApplyAnimation();

	}

	private float GetTimeDuration(float duration) {
		return (1-(1/(20*startTimer/duration+1)));
		
	}

	public int GetCombo (MusicData.NoteData.NotePhase type){

		Debug.Log("Get Combo type: " + type);
		bool ok = true;

		//  Normal , Great , Ok , Bad , Miss
		switch (type){
			case MusicData.NoteData.NotePhase.Miss:
			ok = false;
			break;

			case MusicData.NoteData.NotePhase.Bad:
			ok = false;
			break;

			case MusicData.NoteData.NotePhase.Ok:
			ok = true;
			break;

			case MusicData.NoteData.NotePhase.Great:
			ok = true;
			break;

			case MusicData.NoteData.NotePhase.Normal:
			ok = true;
			break;
		}	

		SwitchState (ok);

		return comboSum;
	}

	// miss, badのときfalse, それ以外true (成功判定)
	private void SwitchState (bool ok){

		if (!ok) {
			comboSum = 0;
			InitGUI();
			animationPhase = AnimationPhase.Nothing;
			return;
		}

		comboSum++;

		// コンボ最低数以上に初めてなったとき
		if (animationPhase == AnimationPhase.Nothing) {
			animationPhase = AnimationPhase.Start;
			return;
		}

		if (animationPhase == AnimationPhase.Start) {
			animationPhase = AnimationPhase.Middle;
			return;
		}

	}

	/*
	timerとanimationPhaseで文字にアニメーションを適応させる。
	*/
	private void ApplyAnimation (){
		switch (animationPhase){
			case AnimationPhase.Start:
			


			break;
		}
	}

	private void InitGUI (){
		isStartAnimation=false;
		comboLabel.GetComponent<UIStretch>().relativeSize.y = maxTextScale;
		numLabel.GetComponent<UIStretch>().relativeSize.y = maxNumScale;
		comboLabel.GetComponent<UILabel>().color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
		numLabel.GetComponent<UILabel>().color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
	}

	private void setComboNumToGUI (){
		if (comboSum < minComboNum){
			return;
		}
		numLabel.GetComponent<UILabel>().text = comboSum.ToString();
	} 

}
