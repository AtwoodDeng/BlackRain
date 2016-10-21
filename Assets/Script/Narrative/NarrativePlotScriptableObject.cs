using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu (fileName = "NarrativePlot", menuName = "Narrative/Plot", order = 1)]
public class NarrativePlotScriptableObject : ScriptableObject {

	public List<NarrativeDialog> dialogs;

}



[System.Serializable]
public class NarrativeDialog
{
	/// <summary>
	/// The word of the dialog.
	/// </summary>
	public string word{
		get {
			if (LogicManager.Language == LogicManager.GameLanguage.English)
				return wordEng;
			if (LogicManager.Language == LogicManager.GameLanguage.Chinese)
				return wordChinese;
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