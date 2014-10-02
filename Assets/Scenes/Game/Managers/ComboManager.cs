using UnityEngine;
using System.Collections;

public class ComboManager : MonoBehaviour {
	static public ComboManager instance;
	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public int GetCombo(MusicData.NoteData.NotePhase phase){
		return 14;
	}
}
