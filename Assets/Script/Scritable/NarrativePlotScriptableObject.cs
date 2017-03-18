using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu (fileName = "NarrativePlot", menuName = "EveryRainDrop/NarrativePlot", order = 1)]
public class NarrativePlotScriptableObject : ScriptableObject {

	public List<NarrativeDialog> dialogs;

	public bool lockCamera = true;

	public bool important = false;

	public LogicEvents endPlotEvent = LogicEvents.None;
}



[System.Serializable]
public class NarrativeDialog
{
	/// <summary>
	/// The word of the dialog.
	/// </summary>
	public string word{
		get {
			string prefix="";
//			if (type == SpeakerType.Girl)
//				prefix = "(her)";
//			if (type == SpeakerType.MainCharacter)
//				prefix = "(me)";
//			if (type == SpeakerType.ThisCharacter)
//				prefix = "(them)";
//			prefix += "  ";
			if (LogicManager.Language == LogicManager.GameLanguage.English)
				return prefix + wordEng;
			if (LogicManager.Language == LogicManager.GameLanguage.Chinese)
				return prefix + wordChinese;
			return "";
		}
	}
	/// <summary>
	/// Dialog in English
	/// </summary>
	public string wordEng;
	/// <summary>
	/// Dialog in Chinese
	/// </summary>
	public string wordChinese;

	/// <summary>
	/// The clip of the dialog.
	/// </summary>
	public AudioClip clip;

	/// <summary>
	/// who says this dialog
	/// </summary>
	public enum SpeakerType
	{
		None,
		Narrator,
		ThisCharacter,
		MainCharacter,
		Girl,
	}
	public SpeakerType type;
}
	
public enum NarrativeIcon {
	None = 0,
	Happy = 1,
	Question = 2,
	Cry = 3,
	Shy = 4,
	Smirking = 5,
	LoveEyes = 6,
	Angry = 7,
	Exclamation = 8,
	DotDotDot = 10,
	Sad = 11,
	Nervous = 12,
	Consider = 13,
	Sad2 = 14,
	Confuse = 15,
	Unhappy = 16,

	Kappa = 31,
	Yao = 32,
	Crazy = 33,

	OK = 50,
	QuestionGirl = 51,
	Stop = 52,
	Music = 53,
	Ship = 54,
	Flower = 55,
	Firework = 56,
	RedLight = 57,
	Present = 58,

	Umbrella = 100,
	ShareUmbrella = 101,
	LostUmbrella = 102,
	Crow = 103,
	CrowInRainMC = 104,
	CrowInRainGirl = 105,
	Money = 110,
	Ladies = 111,
	Wine = 112,
	ShareUmbrellaMC = 113,
	ShareUmbrellaGirl = 114,

	HappyOld = 201,
	QuestionOld = 202,
	ShareUmbrellaOld = 203,
	MusicOld = 204,
}



[System.Serializable]
public class IconNarrativeDialog
{
	public NarrativeIcon icon;
	public float delay = -1f;
	public float duration = -1f;
	/// <summary>
	/// The clip of the dialog
	/// </summary>
	public AudioClip clip;
	public float soundVolumn = 0.8f;
	/// <summary>
	/// who says this dialog
	/// </summary>
	public enum SpeakerType
	{
		None,
		ThisCharacter,
		MainCharacter,
		Girl,
	}
	public SpeakerType type;

	/// <summary>
	/// Who send the dialog
	/// </summary>
	public TalkableCharacter thisCharacter;

	/// <summary>
	/// If set the dialog to a bigger one
	/// </summary>
	public bool IsBig = false;


}