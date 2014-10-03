using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {
	public MusicData.NoteData data;
	public Material greatMaterial;
	public Material greatLongMaterial;
	public Material failedMaterial;
	public Material okMaterial;
	private GameObject effectPrehab;

	// Use this for initialization
	void Start () {
		effectPrehab = GameObject.Find ("effect");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void tapped(MusicData.NoteData.NotePhase phase){
		data.phase = phase;
		data.tappedTime = NoteManager.manager.audio.time;
		data.tappedPosition = transform.position;

		Vector3 correctPos = getCorrectPos ();
		((GameObject)Instantiate (effectPrehab , correctPos , Quaternion.Euler(new Vector3( 50.0f ,0 , 0)))).GetComponent<EffectManager>().enabled = true;

		if (ScreenUtil.findObject (transform, "flare") != null){
			Destroy(ScreenUtil.findObject (transform, "flare"));
		}
		if (!data.isLong) {
			if (phase == MusicData.NoteData.NotePhase.Great) {
				transform.position = correctPos;
				gameObject.renderer.material = greatMaterial;
				iTween.ScaleTo (gameObject, iTween.Hash ("x", 1.5f, "time", 0.5f));
			} else {
				gameObject.renderer.material = okMaterial;
			}
			iTween.FadeTo (gameObject, iTween.Hash ("alpha", 0, "time", 0.5f));
		} else {
			renderer.enabled = false;
		}

	}
	
	public void missed(){
		data.tappedTime = NoteManager.manager.audio.time;
		data.tappedPosition = getCorrectPos ();
		gameObject.renderer.material = failedMaterial;
		Destroy(ScreenUtil.findObject (transform, "flare"));
	}
	public void failed(){
		iTween.FadeTo(gameObject, iTween.Hash("alpha", 0, "time", 0.5f));
	}

	public Vector3 getCorrectPos(){
		return new Vector3 (transform.position.x, transform.position.y, NoteManager.manager.left.position.z);
	}
}
