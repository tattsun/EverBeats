using UnityEngine;
using System.Collections;

public class ComboTester : MonoBehaviour {

	public GameObject comboManagerObject;
	private ComboManager comboManager;

	void Start () {
		comboManager = comboManagerObject.GetComponent<ComboManager>();
		comboManager.GetCombo(MusicData.NoteData.NotePhase.Ok);
	}

	bool test = true;
	float timer = 0.0f;
	int intTimer = 0;
	int prevTimer = 0;
	
	void Update () {
		/*
		timer += Time.deltaTime;
		intTimer = (int)timer;

		if (intTimer == prevTimer) 
			return;

		prevTimer = intTimer;

		if (intTimer > 1 && intTimer < 14) {
				comboManager.GetCombo(MusicData.NoteData.NotePhase.Ok);
		} else if (intTimer == 14) {
				comboManager.GetCombo(MusicData.NoteData.NotePhase.Miss);
		} 

		return;
		*/
		float value = Random.value;
		if (value > 0.90f){
			if (value < 0.905f) {
				comboManager.GetCombo(MusicData.NoteData.NotePhase.Miss);
			}
			else if (0.905f <= value && value < 0.98f) {
				comboManager.GetCombo(MusicData.NoteData.NotePhase.Ok);
			}
			else if (0.98 <= value) {
				comboManager.GetCombo(MusicData.NoteData.NotePhase.Great);
			}
		}
	}
}
