using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FingerManager : MonoBehaviour {
	static bool isEditerMode = false;
	Dictionary<string , MusicData.NoteData> lastnote;

	// Use this for initialization
	void Start () {
		lastnote = new Dictionary<string , MusicData.NoteData> ();
	}
	void Update(){
		if (Input.touchCount == 0 && Input.GetMouseButtonDown (0)) {
			isEditerMode = true;
		}

		if (NoteManager.isEditMode) {
			/* EditMode */
			if (isEditerMode){
				if (Input.GetMouseButtonDown(0)) {
					int tapCount = 0;
					if (lastnote.ContainsKey("0")){
						tapCount = lastnote["0"].tapCount+1;
					}
					makeNote(Input.mousePosition , 0 , tapCount);
				}else{
					if (Input.GetMouseButton(0) && lastnote["0"].time + NoteManager.LONG_PUSH_TIME < NoteManager.manager.audio.time  ) {
						//long note
						lastnote["0"].isLong = true;
						makeNote(Input.mousePosition , 0 , lastnote["0"].tapCount , true);
					}
				}
			}else{
				for ( int i = 0 ; i<Input.touchCount ; i++){
					Touch t = Input.GetTouch(i);
					if (t.phase == TouchPhase.Began){
						int tapCount = 0;
						if (lastnote.ContainsKey(""+t.fingerId)){
							tapCount = lastnote[""+t.fingerId].tapCount+1;
						}
						makeNote(t.position , t.fingerId , tapCount);
					} else if ((t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved)
					           && lastnote[""+t.fingerId].time + NoteManager.LONG_PUSH_TIME < NoteManager.manager.audio.time){
						lastnote[""+t.fingerId].isLong = true;
						makeNote(t.position , t.fingerId , lastnote[""+t.fingerId].tapCount , true);
					}
				}
			}
		} else {
			/* Normal */
			if (isEditerMode){
				if (Input.GetMouseButtonDown(0)) {
					tapped(Input.mousePosition);
				}else if (Input.GetMouseButton(0)  ) {
					//long note
					hold(Input.mousePosition);
				}
			}else{
				for ( int i = 0 ; i<Input.touchCount ; i++){
					Touch t = Input.GetTouch(i);
					if (t.phase == TouchPhase.Began){
						tapped(t.position);
					} else if (t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved){
						hold(t.position);
					}
				}
			}
		}
	}

	static public List<MusicData.NoteData> removeWrongLongTaps( List<MusicData.NoteData> notes){
		var copy = new List<MusicData.NoteData> (notes);
		var result = new List<MusicData.NoteData> (notes);
		while (copy.Count > 0){
			int fingerid = copy[0].fingerId;
			int tapCount = copy[0].tapCount;
			int sum = 0;

			MusicData.NoteData temp = null;
			foreach (MusicData.NoteData n in copy){
				if (n.fingerId == fingerid && n.tapCount == tapCount ){
					sum++;
					if (temp == null){
						temp = n;
					}
				}
			}
			if (sum < 3 && copy[0].isLong){
				Debug.Log("[EXPORT DATA] wrong long tap detected.");
				result.RemoveAll( (MusicData.NoteData n)=>{
					return n.fingerId == fingerid && n.tapCount == tapCount && temp != n;
				} );
				temp.isLong = false;
			}
			copy.RemoveAll( (MusicData.NoteData n)=>{
				return n.fingerId == fingerid && n.tapCount == tapCount;
			} );
		}
		return result;
	}

	private void makeNote(Vector2 point , int fingerId , int tapCount , bool isLong = false){
		if (lastnote.ContainsKey(""+fingerId) ){
			lastnote.Remove(""+fingerId);
		}
		MusicData.NoteData note = NoteManager.manager.addNote ( getOffset(point) , isLong );
		lastnote.Add ( ""+fingerId,note);
		note.tapCount = tapCount;
		note.fingerId = fingerId;
	}

	private void tapped(Vector2 point){
		NoteManager.manager.pushNote ( getOffset(point) );
	}
	private void hold(Vector2 point){
		NoteManager.manager.holdNote ( getOffset(point) );
	}

	private float getOffset( Vector2 point ){
		float offset = 0.14f;
		float x = point.x / Screen.width;
		x = Mathf.Max (offset, Mathf.Min (1.0f - offset, x));
		return (x - offset) / (1.0f - offset * 2);
	}

}
