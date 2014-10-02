using UnityEngine;
using System.Collections;

public class ComboTester : MonoBehaviour {

	public GameObject comboManagerObject;
	private ComboManager comboManager;

	void Start () {
		comboManager = comboManagerObject.GetComponent<ComboManager>();
		//comboManager.GetCombo(MusicData.NoteData.NotePhase.Ok);
	}
	
	void Update () {
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
