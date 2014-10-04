using UnityEngine;
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
	
	
	public int GetLv( float playtime ){
		return GetLvObj (playtime).lv;
	}
	public Level GetLvObj( float playtime ){
		return new Level( notes , playtime );
	}
	public class Level{
		public int lv;
		public float pt_dens;
		public float pt_time;
		public float pt_long;

		public Level ( List<NoteData> notes, float playtime) {
			/*  dencity */
			var copy = new List<NoteData>(notes);
			copy.RemoveAll (( NoteData n ) => {
				return n.isLong;
			});
			int long_num = notes.Count - copy.Count;
			
			float dens = ((float)copy.Count)/playtime;  //note per second
			pt_dens = Mathf.Max( 0,(dens*7.0f - 20 )*1.7f );
			Debug.Log ("[LEVEL] dens :"+dens + " ->point:" + pt_dens);
			
			/* playtime */
			pt_time = playtime/20;
			Debug.Log ("[LEVEL] playtime :"+playtime + " ->point:" + pt_time);
			
			/* long note num */
			pt_long = Mathf.Max(Mathf.Sqrt(long_num)/2 , 0);
			Debug.Log ("[LEVEL] long_num :"+long_num + " ->point:" + pt_long);
			
			/* SUM */
			lv = Mathf.CeilToInt (pt_dens + pt_time + pt_long);
			Debug.Log ("[LEVEL] lv :"+ lv );
		}
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
