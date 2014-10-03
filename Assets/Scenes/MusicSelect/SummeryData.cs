using UnityEngine;
using System.Collections;


[System.Serializable]
public class SummeryData{
	public int musicid;
	public string videoid;
	public string title;
	public string title_en;
	public string sentence;
	public string artist;
	public string authername;
	public string autherid;
	public int pv_count;
	public int playtime;
	public int lv;
	public int highScore;
	public string date;

	public static SummeryData sample(){
		SummeryData s = new SummeryData ();
		s.musicid = 0;
		s.videoid = "FLUC8aINF1c";
		s.title  = "Thunderclap";
		s.title_en  = "Thunderclap";
		s.artist = "Fear, and Loathing in Las Vegas";
		s.authername = "Kamasu";
		s.autherid = "FagkwR24g";
		s.pv_count = 0;
		s.playtime = 65;
		s.lv = 4;
		s.highScore = 0;
		s.date = ScreenUtil.dateConvert (System.DateTime.Now );
		return s;
	}
}
