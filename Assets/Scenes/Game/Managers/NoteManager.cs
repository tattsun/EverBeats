using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoteManager : MonoBehaviour {
	static private float REMOVE_OFFSET_TIME = 1;
	static private float OK_TIME 			= 0.1f;
	static private float GREAT_TIME 		= 0.05f;
	static private float OK_DISTANCE		= 0.2f;
	static public  float LONG_PUSH_TIME		= 0.1f;

	static private float SCORE_MULTIPLIER	= 100f;


	static public bool isEditMode = false;
	static public NoteManager manager;
	
	public float Interval = 1;


	public Transform left;
	public Transform right;
	public Transform generate;
	public GameObject notePrehab;
	public GameObject longNotePrehab;
	
	internal List< MusicData.NoteData> notes;
	internal AudioSource audio;
	internal MusicData music;
	
	private int index;

	// Use this for initialization
	void Start () {
		audio = Camera.main.GetComponent<AudioSource> ();
		music = new MusicData(GameManager.gameData.musicdata);
		index = 0;
		notes = new List< MusicData.NoteData > ();
		manager = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (!audio.isPlaying){
			return;
		}
		if (isEditMode) {
		} else {
			if (music.notes.Count > index){
				Generate ();
			}
			Move ();
		}

	}

	private void Generate(){
		MusicData.NoteData next = music.notes [index];
		while ( next.time - Interval < audio.time && music.notes.Count > index){
			GameObject myNote = (GameObject)Instantiate( next.isLong?longNotePrehab:notePrehab , new Vector3( 
			                                                                      left.position.x + (right.position.x - left.position.x) * next.offset ,
			                                                                      0 ,
			                                                                      generate.position.z ), Quaternion.identity );
			next.gameObject = myNote;
			myNote.GetComponent<Note>().data = next;

			notes.Add( next );
			index++;
			if (music.notes.Count <= index){
				break;
			}
			next = music.notes [index];
		}
	}

	private void Move(){
		foreach ( MusicData.NoteData note in notes ){
			Vector3 pos = note.gameObject.transform.position;
			if (note.phase == MusicData.NoteData.NotePhase.Normal){
				note.gameObject.transform.position = new Vector3(  pos.x , 0 , (( left.position.z - generate.position.z ) * (1 - (note.time - audio.time)/ Interval )  )  + generate.position.z);
			}else if (note.phase == MusicData.NoteData.NotePhase.Miss || note.phase == MusicData.NoteData.NotePhase.Bad){
				note.gameObject.transform.position = note.tappedPosition - Vector3.forward * ( audio.time - note.tappedTime) * 1;
			}else{
				/* do nothing */
			}
		}
		notes.RemoveAll ( ( MusicData.NoteData note )=>{
			if ( note.time + REMOVE_OFFSET_TIME < audio.time ){
				Destroy(note.gameObject);
				return true;
			}
			if ( note.time < audio.time && note.phase == MusicData.NoteData.NotePhase.Normal ){
				note.gameObject.GetComponent<Note>().missed();
				note.phase = MusicData.NoteData.NotePhase.Miss;
			}
			if ( note.time + OK_TIME < audio.time && note.phase == MusicData.NoteData.NotePhase.Miss){
				note.gameObject.GetComponent<Note>().failed();
				note.phase = MusicData.NoteData.NotePhase.Bad;
				ComboManager.instance.GetCombo(MusicData.NoteData.NotePhase.Bad);
			}
			return false;
		});
	}
	public void pushNote( float offset ){
		// get the note
		foreach ( MusicData.NoteData note in music.notes ){
			if (note.phase == MusicData.NoteData.NotePhase.Normal || note.phase == MusicData.NoteData.NotePhase.Miss){
				if ( Mathf.Abs(note.time - audio.time) < OK_TIME  ){
					if (  Mathf.Abs(note.offset - offset) < OK_DISTANCE ){
						float score = Mathf.Abs(OK_TIME - Mathf.Abs(note.offset - offset))*SCORE_MULTIPLIER;
						var type = MusicData.NoteData.NotePhase.Ok;
						if (Mathf.Abs(note.time - audio.time) < GREAT_TIME ){
							score *= 2;
							type = MusicData.NoteData.NotePhase.Great;
							GameManager.manager.result.great ++;
						}else{
							GameManager.manager.result.good ++;
						}
						note.gameObject.GetComponent<Note>().tapped( type );
						int combo = ComboManager.instance.GetCombo( type );
						GameManager.manager.result.maxCombo = Mathf.Max(GameManager.manager.result.maxCombo , combo);
						score *= (Mathf.Log10(combo) + 1 );
						if (note.isLong){
							score *= 0.2f;
						}
						GameManager.score += (int)score;
						ScoreManager.manager.leftPoint += (int)score;
						break;
					}
				}
			}
		}
	}
	public void holdNote( float offset ){
		foreach ( MusicData.NoteData note in music.notes ){
			if (!note.isLong){
				continue;
			}
			if (note.phase == MusicData.NoteData.NotePhase.Normal || note.phase == MusicData.NoteData.NotePhase.Miss){
				if ( Mathf.Abs(note.time - audio.time) < GREAT_TIME  ){
					if (  Mathf.Abs(note.offset - offset) < OK_DISTANCE ){
						var type = MusicData.NoteData.NotePhase.Great;
						note.gameObject.GetComponent<Note>().tapped( type );
						float score = OK_TIME*SCORE_MULTIPLIER * 2 * 0.2f * (Mathf.Log10(ComboManager.instance.GetCombo( type )) + 1 );;
						GameManager.score += (int)score;
						ScoreManager.manager.leftPoint += (int)score;
						break;
					}
				}
			}
		}
	}

	/*
	 * EditMode
	 */
	public MusicData.NoteData addNote (float offset , bool isLong = false){
		MusicData.NoteData n = new MusicData.NoteData ();
		n.time   = audio.time;
		n.offset = offset;
		n.isLong = isLong;
		notes.Add (n);
		return n;
	}
	public MusicData exportMusicData(){
		music.notes = FingerManager.removeWrongLongTaps(notes);
		return music;
	}
}