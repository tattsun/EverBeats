using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MemoryManager : MonoBehaviour {

	// Use this for initialization
	List<GameObject> pool;
	void Start () {
		pool = new List<GameObject> ();
		float unit = 0.2f;

		for (int time = 0 ; time <3 ; time ++){

			SimpleTimer.setTimer(3.0f*unit*time  , ()=>{
				for ( int i = 0 ; i< 30 ; i++){
					pool.Add (instantiate("Note_normal"));
					pool.Add (instantiate("Note_longtap"));
				}
			});
			SimpleTimer.setTimer(3.0f*unit*time + unit, ()=>{
				pool.RemoveAll( (GameObject g)=>{
					g.GetComponent<Note>().data = new MusicData.NoteData();
					if (Random.value < 0.25f){
						g.GetComponent<Note>().tapped(MusicData.NoteData.NotePhase.Great);
						ComboManager.instance.GetCombo(MusicData.NoteData.NotePhase.Great);
					}else if (Random.value < 0.5f){
						g.GetComponent<Note>().tapped(MusicData.NoteData.NotePhase.Ok);
						ComboManager.instance.GetCombo(MusicData.NoteData.NotePhase.Ok);
					}else if (Random.value < 0.75f){
						g.GetComponent<Note>().missed();
						ComboManager.instance.GetCombo(MusicData.NoteData.NotePhase.Miss);
					}else{
						g.GetComponent<Note>().failed();
						ComboManager.instance.GetCombo(MusicData.NoteData.NotePhase.Bad);
					}
					return false;
				});
			});
			SimpleTimer.setTimer(3.0f*unit*time + 2.0f*unit, ()=>{
				pool.RemoveAll( (GameObject g)=>{
					Destroy(g);
					return true;
				});
				ComboManager.instance.GetCombo(MusicData.NoteData.NotePhase.Miss);
			});

		}
	}
	GameObject instantiate(string name){
		return (GameObject)Instantiate (GameObject.Find(name) , new Vector3(Random.value*6 - 3,Random.value*6 - 3,-5 + Random.value*6 - 3) , Quaternion.identity);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
