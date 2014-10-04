using UnityEngine;
using System.Collections;

public class debugLabel : MonoBehaviour {

	UILabel label;
	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel> ();
	}
	
	// Update is called once per frame
	void Update () {
		label.text = "FPS :" + (1 / Time.deltaTime).ToString("f1");

	}
}
