using UnityEngine;
using System;

// 音符が流れてきた結果を受け取って，コンボ数を判定して
// コンボ数を表示して，そのコンボ数を呼び出し元に返します。
public class ComboManager : MonoBehaviour {

	internal static ComboManager instance;

	public GameObject numLabelPrehab;
	public GameObject comboLabelPrehab;
	public GameObject comboNoticeLabelPrehab;
	public GameObject beatSpritePrehab;
	public GameObject lightExpo1;
	public GameObject lightExpo2;
	public GameObject lightExpo3;
	public GameObject fullComboSpritePrehab;
	public Color[] comboColor;
	public Color badBeatColor;

	private GameObject numLabel; // コンボ数を表示する為のNGUI
	private GameObject comboLabel; // コンボ数のよこを表示する為のNGUI
	private GameObject comboNoticeLabel;
	private GameObject beatSprite; // GoodとかExcellentとかスプライト
	private GameObject fullComboSprite;

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
	public float noticeMoveDistance = 0.1f;
	private float defaultNoticeOffsetY;
	public float beatMoveDistance = 0.07f;
	private float defaultBeatOffsetY;
	private float defaultBeatScale;
	private float defaultFullComboScale;
	public float maxFullComboScale = 0.35f;
	private float defaultFullComboOffsetY;

	// 設定定数フィールド
	public float startAnimInterval = 0.5f;
	public float middleAnimInterval = 0.2f;
	public float endAnimInterval = 0.5f;
	public float lightAnimSpan = 0.3f;
	public float lightMinSize = 0.1f;
	public float lightMaxSize = 0.3f;
	public float beatMoveInterval = 0.3f;
	public float beatBiginHide = 1.0f;
	public float beatHideInterval = 0.1f;

	public int minComboNum = 5;

	private bool isStartAnimation = false;
	private bool isMiddleAnimation = false;
	private bool isEndAnimation = false;
	private bool isLightAnimation = false;
	private bool isComboNoticeAnimation = false;
	private bool isBeatAnimation = false;
	private bool isFullComboAnimation = false;

	private float startTimer = 0.0f;
	private float endTimer = 0.0f;
	private float noticeTimer = 0.0f;
	private float beatTimer = 0.0f;
	private float longJudgeTimer = 0.0f;
	private float fullComboTimer = 0.0f;

	private bool isLongNote = false;

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
		fullComboSprite = (GameObject)Instantiate(fullComboSpritePrehab);

		beatSprite.GetComponent<UISprite>().color = new Color(1,1,1,0);

		defaultFullComboOffsetY = fullComboSprite.GetComponent<UIAnchor>().relativeOffset.y;
		defaultFullComboScale = fullComboSprite.GetComponent<UIStretch>().relativeSize.x;
		defaultBeatScale = beatSprite.GetComponent<UIStretch>().relativeSize.x;
		defaultBeatOffsetY = beatSprite.GetComponent<UIAnchor>().relativeOffset.y;
		defaultNoticeOffsetY = comboNoticeLabel.GetComponent<UIAnchor>().relativeOffset.y;
		defaultTextScale = comboLabel.GetComponent<UIStretch>().relativeSize.y;
		defaultNumScale = numLabel.GetComponent<UIStretch>().relativeSize.y;
		maxNumScale = defaultNumScale * 1.5f;
		maxTextScale = defaultTextScale * 1.5f;
		defaultComboOffsetY = comboLabel.GetComponent<UIAnchor>().relativeOffset.y;
		defaultNumOffsetY = numLabel.GetComponent<UIAnchor>().relativeOffset.y;
		appearComboOffsetY = defaultComboOffsetY - 0.03f;
		appearNumOffsetY = defaultNumOffsetY - 0.03f;
		InitGUI();
	}

	void Update () {

		startTimer += Time.deltaTime;
		longJudgeTimer += Time.deltaTime;

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
			float nowTextScale = maxTextScale - GetTimeDuration(startTimer, startAnimInterval,"Shake")*Mathf.Abs(defaultTextScale - maxTextScale);
			float nowNumScale = maxNumScale - GetTimeDuration(startTimer, startAnimInterval)*Mathf.Abs(defaultNumScale - maxNumScale);

			comboLabel.GetComponent<UIStretch>().relativeSize.y = nowTextScale;
			//numLabel.GetComponent<UIStretch>().relativeSize.y = nowNumScale;

			if (Mathf.Abs(nowTextScale - defaultTextScale) < 0.0001f && Mathf.Abs(nowNumScale - defaultNumScale) < 0.0001f) {
				comboLabel.GetComponent<UIStretch>().relativeSize.y = defaultTextScale;
				numLabel.GetComponent<UIStretch>().relativeSize.y = defaultNumScale;
				endScale = true;
			}

			isStartAnimation = !(endScale && endAlpha);

			if (isMiddleAnimation) {
				isStartAnimation = endAlpha;
			}

			if (!isStartAnimation){
				startTimer = 0.0f;
				animationPhase = AnimationPhase.Middle;
			}
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
			endTimer += Time.deltaTime;
			/* 透明度のアニメーション */
			float alpha = 1.0f - GetTimeDuration(endTimer, endAnimInterval);

			comboLabel.GetComponent<UILabel>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, alpha);
			numLabel.GetComponent<UILabel>().color = new Color (nowComboColor.r, nowComboColor.g, nowComboColor.b, alpha);

			/* 位置のアニメーション */
			float numY = defaultNumOffsetY - (Mathf.Abs(defaultNumOffsetY - appearNumOffsetY) * GetTimeDuration(endTimer, endAnimInterval));
			float comboY = defaultComboOffsetY - (Mathf.Abs(defaultComboOffsetY - appearComboOffsetY) * GetTimeDuration(endTimer, endAnimInterval));

			comboLabel.GetComponent<UIAnchor>().relativeOffset.y = comboY;
			numLabel.GetComponent<UIAnchor>().relativeOffset.y = numY;

			if (Mathf.Abs(numY - appearNumOffsetY) < 0.01f && Mathf.Abs(comboY - appearComboOffsetY) < 0.01f) {
				endTimer = 0.0f;
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

		if (isBeatAnimation) {
			beatTimer += Time.deltaTime;
			if (beatTimer < beatMoveInterval) {
				float degree = GetTimeDuration(beatTimer, beatMoveInterval);

				beatSprite.GetComponent<UISprite>().color = new Color (1, 1, 1, GetTimeDuration(beatTimer, beatMoveInterval, "DecreasingCurve"));
				beatSprite.GetComponent<UIAnchor>().relativeOffset.y = defaultBeatOffsetY + beatMoveDistance - (degree * beatMoveDistance);
				beatSprite.GetComponent<UIStretch>().relativeSize.x = defaultBeatScale * GetTimeDuration(beatTimer, beatMoveInterval, "Shake");

			} else if (beatTimer > beatBiginHide) {
				float time = beatTimer - beatBiginHide;
				beatSprite.GetComponent<UISprite>().color = new Color (1, 1, 1, 1 - GetTimeDuration(time, beatHideInterval, "Linear"));
			}

			if (beatTimer > beatBiginHide + beatHideInterval) {
				beatTimer = 0.0f;
				isBeatAnimation = false;
			}
		}

		if (isFullComboAnimation) {
			fullComboTimer += Time.deltaTime;

			if (fullComboTimer <= 0.5f) {
				fullComboSprite.GetComponent<UISprite>().color = new Color(1,1,1,GetTimeDuration(fullComboTimer,0.3f));
				fullComboSprite.GetComponent<UIStretch>().relativeSize.x = maxFullComboScale - ((maxFullComboScale-defaultFullComboScale)*GetTimeDuration(fullComboTimer,0.3f,"Shake"));
			} else if (fullComboTimer >= 1.5f) {
				float time = fullComboTimer - 1.5f;
				fullComboSprite.GetComponent<UISprite>().color = new Color(1,1,1,1-GetTimeDuration(time, 0.5f));
				fullComboSprite.GetComponent<UIAnchor>().relativeOffset.y = defaultFullComboOffsetY - (GetTimeDuration(time, 0.5f)*0.05f);
			} else if (fullComboTimer > 2.0f) {
				isFullComboAnimation = false;
				fullComboTimer = 0.0f;
			}
		}

	}

	private bool isLongNotePressed () {
		if (longJudgeTimer > 0.25f) {
			return false;
		}
		return true;
	}

	private float GetTimeDuration(float time, float duration, string type = "Sin") {

		float x;
		switch(type) {
		case "Sin":
			if (time < duration)
				x = Mathf.Sin(Mathf.PI/2.0f/duration*time);
			else
				x = 1.0f;
			break;
			 
		case "Linear":
			if (time < duration) {
				x = (1.0f/duration)*time;
			} else {
				x = 1.0f;
			}
			break; 

		case "Shake":
			float t = time*3.3f/duration - 0.275f;
			x = Mathf.Exp(-2*t)*Mathf.Sin(Mathf.PI*t) + 1;
			break;
		
		case "IncreasingCurve":
			x = - (Mathf.Sqrt(1 - Mathf.Pow(time/duration,2))) + 1;
			break;

		case "DecreasingCurve":
			x = Mathf.Sqrt(1 - Mathf.Pow((time-duration)/duration,2));
			break;

		default:
			x = 0.0f;
			Debug.LogError("GetTimeDuration does not support type :" + type);
			break;
		}

		return x;
		// return (1-(1/(20*startTimer/duration+1)));
		// return (startTimer <= duration) ? startTimer / duration : duration;
	}

	public int GetCombo (MusicData.NoteData.NotePhase type){
		bool ok = true;

		//  Normal , Great , Ok , Bad , Miss
		switch (type){

			case MusicData.NoteData.NotePhase.Miss:
			beatSprite.GetComponent<UISprite>().spriteName = "";
			ok = false;
			break;

			case MusicData.NoteData.NotePhase.Bad:
			beatSprite.GetComponent<UISprite>().spriteName = "combo_bad";
			nowComboColor = badBeatColor;
			isBeatAnimation = true;
			ok = false;
			break;

			case MusicData.NoteData.NotePhase.Ok:
			beatSprite.GetComponent<UISprite>().spriteName = "combo_good";
			isBeatAnimation = true;
			ok = true;
			break;
			
			case MusicData.NoteData.NotePhase.Long:

			if (longJudgeTimer < NoteManager.LONG_PUSH_TIME + 0.1f) {
				animationPhase = AnimationPhase.Middle;
				longJudgeTimer = 0.0f;
				comboSum ++ ;
				SetComboNumToGUI();
				return comboSum;
			}
			isBeatAnimation = true;
			Debug.Log ("first long note");
			longJudgeTimer = 0.0f;
			beatSprite.GetComponent<UISprite>().spriteName = "combo_excellent";
			ok = true;

			break;

			case MusicData.NoteData.NotePhase.Great:
			beatSprite.GetComponent<UISprite>().spriteName = "combo_excellent";
			isBeatAnimation = true;
			ok = true;
			break;

			case MusicData.NoteData.NotePhase.Normal:
			ok = true;
			break;
		}	

		beatTimer = 0.0f;

		SwitchState (ok);

		return comboSum;
	}

	public void ResetCombo() {
		beatSprite.GetComponent<UISprite>().color = new Color(1,1,1,0);
		numLabel.GetComponent<UILabel>().color = new Color(1,1,1,0);
		comboLabel.GetComponent<UILabel>().color = new Color(1,1,1,0);
	}

	public void ShowFullCombo () {
		isFullComboAnimation = true;
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

		if (comboSum < minComboNum) {
			return;
		}
	
		if (comboSum == minComboNum)
			isStartAnimation = true;

		if (comboSum % 25 == 0) {
			Instantiate(lightExpo2);
			maxNumScale = defaultNumScale * 3.0f;
			comboNoticeLabel.GetComponent<UIAnchor>().relativeOffset.y = defaultNoticeOffsetY;
			comboNoticeLabel.GetComponent<UILabel>().spacingX = 0;
			comboNoticeLabel.GetComponent<UILabel>().text = "combo " + comboSum;
			isComboNoticeAnimation = true;
		} else {
			maxNumScale = defaultNumScale * 1.25f;
			Instantiate(lightExpo1);
		}

		if (50 <= comboSum && comboSum < 100) {
			nowComboColor = comboColor[1];
		} else if (100 <= comboSum && comboSum < 200) {
			nowComboColor = comboColor[2];
		} else if (200 <= comboSum && comboSum < 300) {
			nowComboColor = comboColor[3];
		} else if (300 <= comboSum) {
			nowComboColor = comboColor[4];
		} else if (comboSum < 50){
			nowComboColor = comboColor[0];
		} 

		numLabel.GetComponent<UILabel>().text = comboSum.ToString();
	} 

}
