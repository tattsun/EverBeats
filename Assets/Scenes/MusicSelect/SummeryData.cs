using UnityEngine;
using System.Collections;


[System.Serializable]
public class SummeryData{
	public int musicid;
	public string videoid;
	public string title;
	public string sentence;
	public string artist;
	public string authername;
	public string autherid;
	public int pv_count;
	public int playtime;


	public static SummeryData sample(){
		SummeryData s = new SummeryData ();
		s.musicid = 0;
		s.videoid = "FLUC8aINF1c";
		s.title  = "Thunderclap";
		s.artist = "Fear, and Loathing in Las Vegas";
		s.authername = "Kamasu";
		s.autherid = "FagkwR24g";
		s.pv_count = 45;
		s.playtime = 65;
		return s;
	}
}
