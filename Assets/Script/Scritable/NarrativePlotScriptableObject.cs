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
	None,
	Umbrella,
	Happy,
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


}