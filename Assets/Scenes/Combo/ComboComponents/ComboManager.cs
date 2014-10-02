using UnityEngine;
using System;

// 音符が流れてきた結果を受け取って，コンボ数を判定して
// コンボ数を表示して，そのコンボ数を呼び出し元に返します。
public class ComboManager : MonoBehaviour {

	internal ComboManager instance;

	public GameObject numLabelPrehab;
	public GameObject comboLabelPrehab;

	private GameObject numLabel; // コンボ数を表示する為のNGUI
	private GameObject comboLabel; // コンボ数のよこを表示する為のNGUI

	private int comboSum = 0;

	/* 定数フィールド */
	private float defaultTextScale;
	private float maxTextScale;
	private float defaultNumScale;
	private float maxNumScale;
	private float appearOffsetY;
	private float defaultOffsetY;

	// 設定定数フィールド
	public float startAnimInterval = 0.5f;
	public float middleAnimInterval = 0.2f;
	public float leaveAnimInterval = 2.0f;
	public float endAnimInterval = 0.5f;

	private int minComboNum = 5;

	private bool isStartAnimation = false;
	private bool isMiddleAnimation = false;
	private bool isEndAnimation = false;


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

		numLabel = (GameObject)Instantiate(numLabelPrehab);
		comboLabel = (GameObject)Instantiate(comboLabelPrehab);

		defaultTextScale = comboLabel.GetComponent<UIStretch>().relativeSize.y;
		defaultNumScale = numLabel.GetComponent<UIStretch>().relativeSize.y;
		maxNumScale = defaultNumScale * 3.0f;
		maxTextScale = defaultTextScale * 3.0f;
		defaultOffsetY = comboLabel.GetComponent<UIAnchor>().relativeOffset.y;
		appearOffsetY = defaultOffsetY - 0.1f;
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
			isEndAnimation = false;
			isStartAnimation = true;
			SetComboNumToGUI();
			break;

			case AnimationPhase.Middle:
			break;

			case AnimationPhase.End:
			isStartAnimation = false;
			break;
		}

		if (isStartAnimation) {
			/* 透明度のアニメーション */
			bool endAlpha = false;
			float alpha = GetTimeDuration(startAnimInterval);
			// Debug.Log("alpha: " + alpha);

			if (alpha > 0.99f){
				endAlpha = true;
			}

			comboLabel.GetComponent<UILabel>().color = new Color (1.0f, 1.0f, 1.0f, alpha);
			numLabel.GetComponent<UILabel>().color = new Color (1.0f, 1.0f, 1.0f, alpha);

			/* スケールのアニメーション */
			bool endScale = false;
			float nowTextScale = maxTextScale - GetTimeDuration(startAnimInterval)*Mathf.Abs(defaultTextScale - maxTextScale);
			float nowNumScale = maxNumScale - GetTimeDuration(startAnimInterval)*Mathf.Abs(defaultNumScale - maxNumScale);

			comboLabel.GetComponent<UIStretch>().relativeSize.y = nowTextScale;
			numLabel.GetComponent<UIStretch>().relativeSize.y = nowNumScale;

			if (Mathf.Abs(nowTextScale - defaultTextScale) < 0.0001f && Mathf.Abs(nowNumScale - defaultNumScale) < 0.0001f) {
				comboLabel.GetComponent<UIStretch>().relativeSize.y = defaultTextScale;
				numLabel.GetComponent<UIStretch>().relativeSize.y = defaultNumScale;
				endScale = true;
			}

			isStartAnimation = !(endScale && endAlpha);
			Debug.Log("isStartAnimation: " + isStartAnimation);

			if (!isStartAnimation){
				startTimer = 0.0f;
				animationPhase = AnimationPhase.Middle;
			}

			// Debug.Log("anima " + alpha + "," + nowTextScale + "," + nowNumScale);
		}

		if (isMiddleAnimation) {

			float nowNumScale = maxNumScale - GetTimeDuration(middleAnimInterval)*Mathf.Abs(defaultNumScale - maxNumScale);
			numLabel.GetComponent<UIStretch>().relativeSize.y = nowNumScale;
			if (Mathf.Abs(nowNumScale - defaultNumScale) < 0.0001f) {
				startTimer = 0.0f;
				isMiddleAnimation = false;
			}
		}

		if (isEndAnimation) {
			/* 透明度のアニメーション */
			float alpha = 1.0f - GetTimeDuration(endAnimInterval);

			comboLabel.GetComponent<UILabel>().color = new Color (1.0f, 1.0f, 1.0f, alpha);
			numLabel.GetComponent<UILabel>().color = new Color (1.0f, 1.0f, 1.0f, alpha);

			/* 位置のアニメーション */
			float posY = defaultOffsetY - (Mathf.Abs(defaultOffsetY - appearOffsetY) * GetTimeDuration(endAnimInterval));
			comboLabel.GetComponent<UIAnchor>().relativeOffset.y = posY;
			numLabel.GetComponent<UIAnchor>().relativeOffset.y = posY;

			if (Mathf.Abs(posY - appearOffsetY) < 0.01f) {
				isEndAnimation = false;
				animationPhase = AnimationPhase.Nothing;
			}
		}

		ApplyAnimation();

	}

	private float GetTimeDuration(float duration) {

		float x;
		if (startTimer < duration)
			x = Mathf.Sin(Mathf.PI/2.0f/duration*startTimer);
		else
			x = 1.0f;

		return x;
		// return (1-(1/(20*startTimer/duration+1)));
		// return (startTimer <= duration) ? startTimer / duration : duration;
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
			if ((animationPhase != AnimationPhase.End) && (animationPhase != AnimationPhase.Nothing)) {
				startTimer = 0.0f;
				DefaultenGUI();
				animationPhase = AnimationPhase.End;
				isEndAnimation = true;
			}
			return;
		}

		comboSum++;

		if (comboSum < minComboNum) {
			return;
		}

		// コンボ最低数以上に初めてなったとき
		if (animationPhase == AnimationPhase.Nothing) {
			animationPhase = AnimationPhase.Start;
			return;
		}

		if (animationPhase == AnimationPhase.Start) {
			startTimer = 0.0f;
			animationPhase = AnimationPhase.Middle;
			return;
		}

		if (animationPhase == AnimationPhase.Middle) {
			isMiddleAnimation = true;
			startTimer = 0.0f;
			SetComboNumToGUI();
			DefaultenGUI();
			return;
		}

	}

	private void DefaultenGUI () {
		comboLabel.GetComponent<UIStretch>().relativeSize.y = defaultTextScale;
		numLabel.GetComponent<UIStretch>().relativeSize.y = defaultNumScale;
		comboLabel.GetComponent<UILabel>().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		numLabel.GetComponent<UILabel>().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		comboLabel.GetComponent<UIAnchor>().relativeOffset.y = defaultOffsetY;
		numLabel.GetComponent<UIAnchor>().relativeOffset.y = defaultOffsetY;
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
		comboLabel.GetComponent<UIAnchor>().relativeOffset.y = defaultOffsetY;
		numLabel.GetComponent<UIAnchor>().relativeOffset.y = defaultOffsetY;
	}

	private void SetComboNumToGUI (){
		numLabel.GetComponent<UILabel>().text = comboSum.ToString();
	} 

}
