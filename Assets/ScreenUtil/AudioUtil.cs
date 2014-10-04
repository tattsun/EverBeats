using UnityEngine;
using System.Collections;

public class AudioUtil : MonoBehaviour {
	static private GameObject _audio1;
	static private GameObject _audio2;
	static private float _interval;
	static float _originalVolume;


	float startedTime;
	// Use this for initialization
	void Start () {
		startedTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (_interval < Time.time - startedTime ){
			_audio1.GetComponent<AudioSource>().Stop();
			if (_audio2 != null){
				_audio2.GetComponent<AudioSource>().volume = 1;
			}
			Destroy(gameObject);
			return;
		}
		_audio1.GetComponent<AudioSource>().volume = _originalVolume * (1.0f - (Time.time - startedTime) / _interval);
		if (_audio2 != null){
		_audio2.GetComponent<AudioSource>().volume = (Time.time - startedTime) / _interval;
		}
	}

	static public void crossfade( float interval , GameObject audio1 , GameObject audio2 = null ){
		if (!audio1.GetComponent<AudioSource>().isPlaying){
			if (audio2 != null){
				audio2.GetComponent<AudioSource> ().Play();
			}
			return;
		}
		(new GameObject().AddComponent<AudioUtil>()).gameObject.name = "AudioUtil";
		_audio1 = audio1;
		_audio2 = audio2;
		_interval = interval; 
		_originalVolume = audio1.GetComponent<AudioSource> ().volume;
		if (audio2 != null){
			audio2.GetComponent<AudioSource> ().Play();
			audio2.GetComponent<AudioSource> ().volume = 0;
		}
	}


}
