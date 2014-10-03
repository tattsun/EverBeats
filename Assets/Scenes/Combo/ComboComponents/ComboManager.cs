﻿using UnityEngine;
using System;

// 音符が流れてきた結果を受け取って，コンボ数を判定して
// コンボ数を表示して，そのコンボ数を呼び出し元に返します。
public class ComboManager : MonoBehaviour {

	internal static ComboManager instance;

	public GameObject numLabelPrehab;
	public GameObject comboLabelPrehab;
	public GameObject comboNoticeLabelPrehab;
	public GameObject beatSpritePrehab;
	public GameObject lightPrehab;
	public GameObject lightExpo1;
	public GameObject lightExpo2;
	public Color combo0;
	public Color combo25;
	public Color combo50;
	public Color combo75;
	public Color combo100;

	private GameObject numLabel; // コンボ数を表示する為のNGUI
	private GameObject comboLabel; // コンボ数のよこを表示する為のNGUI
	private GameObject comboNoticeLabel;
	private GameObject beatSprite; // GoodとかExcellentとかスプライト
	private GameObject light;

	private int comboSum = 0;

	/* 定数フィールド */
	private float defaultTextScale;
	private float maxTextScale;
	private float defaultNumScale;
	private float maxNumScale;
	private float appearNumOffsetY;
	private float appearComboOffsetY;
	private float defaultNumOffsetY;
	private float defaultComboOffsetY;
	private Color nowComboColor = new Color(1,1,1,1);
	private float noticeMoveDistance = 0.1f;
	private float defaultNoticeOffsetY;

	// 設定定数フィールド
	public float startAnimInterval = 0.5f;
	public float middleAnimInterval = 0.2f;
	public float leaveAnimInterval = 2.0f;
	public float endAnimInterval = 0.5f;
	public float lightAnimSpan = 0.3f;
	public float lightMinSize = 0.1f;
	public float lightMaxSize = 0.3f;

	public int minComboNum = 5;

	private bool isStartAnimation = false;
	private bool isMiddleAnimation = false;
	private bool isEndAnimation = false;
	private bool isLightAnimation = false;
	private bool isComboNoticeAnimation = false;

	private float startTimer = 0.0f;
	private float lightTimer = 0.0f;
	private float noticeTimer = 0.0f;

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
		comboNoticeLabel = (GameObject)Instantiate(comboNoticeLabelPrehab);
		beatSprite = (GameObject)Instantiate(beatSpritePrehab);
		light = (GameObject)Instantiate(lightPrehab);

		beatSprite.GetComponent<UISprite>().color = new Color(1,1,1,0);
		light.GetComponent<UISprite>().color = new Color(1,1,1,0);

		defaultNoticeOffsetY = comboNoticeLabel.GetComponent<UIAnchor>().relativeOffset.y;
		defaultTextScale = comboLabel.GetComponent<UIStretch>().relativeSize.y;
		defaultNumScale = numLabel.GetComponent<UIStretch>().relativeSize.y;
		maxNumScale = defaultNumScale * 3.0f;
		maxTextScale = defaultTextScale * 3.0f;
		defaultComboOffsetY = comboLabel.GetComponent<UIAnchor>().relativeOffset.y;
		defaultNumOffsetY = numLabel.GetComponent<UIAnchor>().relativeOffset.y;
		appearComboOffsetY = defaultComboOffsetY - 0.1f;
		appearNumOffsetY = defaultNumOffsetY - 0.1f;
		InitGUI();
	}

	void Update () {

		startTimer += Time.deltaTime;
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
			float alpha = GetTimeDuration(startTimer, startAnimInterval);

			if (alpha > 0.99f) {
				endAlpha = true;
			}
			comboLabel.GetComponent<UILabel>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, alpha);
			numLabel.GetComponent<UILabel>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, alpha);

			/* スケールのアニメーション */
			bool endScale = false;
			float nowTextScale = maxTextScale - GetTimeDuration(startTimer, startAnimInterval)*Mathf.Abs(defaultTextScale - maxTextScale);
			float nowNumScale = maxNumScale - GetTimeDuration(startTimer, startAnimInterval)*Mathf.Abs(defaultNumScale - maxNumScale);

			//comboLabel.GetComponent<UIStretch>().relativeSize.y = nowTextScale;
			//numLabel.GetComponent<UIStretch>().relativeSize.y = nowNumScale;

			if (Mathf.Abs(nowTextScale - defaultTextScale) < 0.0001f && Mathf.Abs(nowNumScale - defaultNumScale) < 0.0001f) {
				comboLabel.GetComponent<UIStretch>().relativeSize.y = defaultTextScale;
				numLabel.GetComponent<UIStretch>().relativeSize.y = defaultNumScale;
				endScale = true;
			}

			isStartAnimation = !(endScale && endAlpha);

			if (!isStartAnimation){
				startTimer = 0.0f;
				animationPhase = AnimationPhase.Middle;
			}

			// Debug.Log("anima " + alpha + "," + nowTextScale + "," + nowNumScale);
		}

		if (isMiddleAnimation) {

			float nowNumScale = maxNumScale - GetTimeDuration(startTimer, middleAnimInterval)*Mathf.Abs(defaultNumScale - maxNumScale);
			numLabel.GetComponent<UIStretch>().relativeSize.y = nowNumScale;
			if (Mathf.Abs(nowNumScale - defaultNumScale) < 0.0001f) {
				startTimer = 0.0f;
				isMiddleAnimation = false;
			}
		}

		if (isEndAnimation) {
			/* 透明度のアニメーション */
			float alpha = 1.0f - GetTimeDuration(startTimer, endAnimInterval);

			comboLabel.GetComponent<UILabel>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, alpha);
			numLabel.GetComponent<UILabel>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, alpha);

			/* 位置のアニメーション */
			float numY = defaultNumOffsetY - (Mathf.Abs(defaultNumOffsetY - appearNumOffsetY) * GetTimeDuration(startTimer, endAnimInterval));
			float comboY = defaultComboOffsetY - (Mathf.Abs(defaultComboOffsetY - appearComboOffsetY) * GetTimeDuration(startTimer, endAnimInterval));

			comboLabel.GetComponent<UIAnchor>().relativeOffset.y = comboY;
			numLabel.GetComponent<UIAnchor>().relativeOffset.y = numY;

			if (Mathf.Abs(numY - appearNumOffsetY) < 0.01f && Mathf.Abs(comboY - appearComboOffsetY) < 0.01f) {
				isEndAnimation = false;
				animationPhase = AnimationPhase.Nothing;
			}
		}

		if (isComboNoticeAnimation) {
			noticeTimer += Time.deltaTime;
			Color color = comboNoticeLabel.GetComponent<UILabel>().color;
			
			if (noticeTimer < 1.0f) {
				float degree = GetTimeDuration(noticeTimer,1.0f);
				comboNoticeLabel.GetComponent<UILabel>().color = new Color (color.r, color.g, color.b, degree);
				comboNoticeLabel.GetComponent<UIAnchor>().relativeOffset.y = defaultNoticeOffsetY + (degree * noticeMoveDistance);
			} else if (2.0f <= noticeTimer && noticeTimer < 3.0f) {
				float nowTimer = noticeTimer - 2.0f;
				comboNoticeLabel.GetComponent<UILabel>().spacingX = (int)(GetTimeDuration(nowTimer,1.0f)*50);
				comboNoticeLabel.GetComponent<UILabel>().color = new Color (color.r, color.g, color.b, 1.0f - GetTimeDuration(nowTimer,1.0f));
			} else if (noticeTimer >= 3.0f) {
				noticeTimer = 0.0f;
				isComboNoticeAnimation = false;
			}
		}

		/*
		if (isLightAnimation) {
			lightTimer += Time.deltaTime;

			if (lightTimer < (lightAnimSpan/2)) {
				light.GetComponent<UISprite>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, GetTimeDuration(lightTimer,lightAnimSpan/2));
				light.GetComponent<UIStretch>().relativeSize.x = lightMinSize + ((lightMaxSize - lightMinSize)*GetTimeDuration(lightTimer,lightAnimSpan/2));
			} else {
				light.GetComponent<UISprite>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, 1.0f - GetTimeDuration(lightTimer-lightAnimSpan/2,lightAnimSpan/2));
				light.GetComponent<UIStretch>().relativeSize.x = lightMinSize + ((lightMaxSize - lightMinSize)*(1.0f - GetTimeDuration(lightTimer-lightAnimSpan/2,lightAnimSpan/2)));
			}

			if (lightTimer >= lightAnimSpan) {
				lightTimer = 0.0f;
				isLightAnimation = false;
			}
		}
		*/

	}

	private float GetTimeDuration(float time, float duration) {

		float x;
		if (time < duration)
			x = Mathf.Sin(Mathf.PI/2.0f/duration*time);
		else
			x = 1.0f;

		return x;
		// return (1-(1/(20*startTimer/duration+1)));
		// return (startTimer <= duration) ? startTimer / duration : duration;
	}

	public int GetCombo (MusicData.NoteData.NotePhase type){
		bool ok = true;

		//  Normal , Great , Ok , Bad , Miss
		switch (type){
			case MusicData.NoteData.NotePhase.Miss:
			beatSprite.GetComponent<UISprite>().spriteName = "combo_miss";
			ok = false;
			break;

			case MusicData.NoteData.NotePhase.Bad:
			beatSprite.GetComponent<UISprite>().spriteName = "combo_bad";
			ok = false;
			break;

			case MusicData.NoteData.NotePhase.Ok:
			beatSprite.GetComponent<UISprite>().spriteName = "combo_good";
			ok = true;
			break;

			case MusicData.NoteData.NotePhase.Great:
			beatSprite.GetComponent<UISprite>().spriteName = "combo_excellent";
			ok = true;
			break;

			case MusicData.NoteData.NotePhase.Normal:
			ok = true;
			break;
		}	

		beatSprite.GetComponent<UISprite>().color = new Color(1,1,1,1);

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
		comboLabel.GetComponent<UILabel>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, 1.0f);
		numLabel.GetComponent<UILabel>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, 1.0f);
		comboLabel.GetComponent<UIAnchor>().relativeOffset.y = defaultComboOffsetY;
		numLabel.GetComponent<UIAnchor>().relativeOffset.y = defaultNumOffsetY;
	}

	private void InitGUI (){
		isStartAnimation=false;
		comboLabel.GetComponent<UIStretch>().relativeSize.y = maxTextScale;
		numLabel.GetComponent<UIStretch>().relativeSize.y = maxNumScale;
		comboLabel.GetComponent<UILabel>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, 0.0f);
		numLabel.GetComponent<UILabel>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, 0.0f);
		comboLabel.GetComponent<UIAnchor>().relativeOffset.y = defaultComboOffsetY;
		numLabel.GetComponent<UIAnchor>().relativeOffset.y = defaultNumOffsetY;
	}

	private void SetComboNumToGUI (){

		if (isLightAnimation) {
			if (lightTimer >= (lightAnimSpan/2.0f)) {
				lightTimer = lightAnimSpan/2.0f - (lightTimer - lightAnimSpan/2.0f);
			}
		} else {
			isLightAnimation = true;
		}

		if (comboSum % 25 == 0) {
			Instantiate(lightExpo2);
			comboNoticeLabel.GetComponent<UIAnchor>().relativeOffset.y = defaultNoticeOffsetY;
			comboNoticeLabel.GetComponent<UILabel>().spacingX = 0;
			comboNoticeLabel.GetComponent<UILabel>().text = "combo " + comboSum;
			isComboNoticeAnimation = true;
		} else {
			Instantiate(lightExpo1);
		}

		if (comboSum == 25) {
			nowComboColor = combo25;
		} else if (comboSum == 50) {
			nowComboColor = combo50;
		} else if (comboSum == 75) {
			nowComboColor = combo75;
		} else if (comboSum == 100) {
			nowComboColor = combo100;
		} else if (comboSum == 0){
			nowComboColor = combo0;
		}

		numLabel.GetComponent<UILabel>().text = comboSum.ToString();
	} 

}
