﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicData {

	public List<NoteData> notes;

	static public MusicData testnotes(){
		return new MusicData( ((TextAsset)Resources.Load("note_sample")).text );
	}

	
	public MusicData (){
		notes = new List<NoteData>();
	}
	public MusicData (string data){
		notes = new List<NoteData> ();
		if (data.Equals("")){
			return;
		}
		string[] arr = ScreenUtil.sepalateByEnter (data);
		foreach (string s in arr ){
			notes.Add (new NoteData(s) );
		}
	}
	public string ToString(){
		ArrayList arr = new ArrayList();
		foreach ( MusicData.NoteData n in notes ){
			arr.Add( n.ToString() );
		}
		return ScreenUtil.Join( (string[])arr.ToArray( typeof(string) ) , "\n" );
	}

	public class NoteData {
		public enum NotePhase { Normal , Great , Ok , Bad , Miss};

		public bool isLong;
		public float time;
		public float offset;

		/* modifiable */
		public GameObject gameObject;
		public NotePhase phase;
		public Vector3 tappedPosition;
		public float tappedTime;
		/* edit */
		public int tapCount;
		public int fingerId;


		public NoteData (){
			phase = NotePhase.Normal;
		}
		public NoteData (string data){
			string[] arr = data.Split(new string[]{ "," } , System.StringSplitOptions.None);
			isLong = arr[0].Equals("y");
			time = float.Parse(arr[1]);
			offset = float.Parse(arr[2]);
			phase = NotePhase.Normal;
		}
		public string ToString(){
			return (isLong?"y":"n") + "," + time + "," + offset;
		}
	}
}
