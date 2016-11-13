using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu (fileName = "NarrativePlot", menuName = "EveryRainDrop/ThoughtList", order = 1)]
public class ThoughtScritableObject : ScriptableObject {

	public Thought mainThought;
	public List<Thought> thoughtList;

	/// <summary>
	/// Return the main thought if there is any
	/// then return the random thought if there is any
	/// </summary>
	/// <value>The thought.</value>
	public Thought Thought
	{
		get {	
			if (mainThought != null && mainThought.word.word != "" )
				return mainThought;
			return RandomThought;
		}
	}

	public Thought RandomThought{
		get {
			if( thoughtList.Count > 0 )
				return thoughtList [Random.Range (0,thoughtList.Count)];
			return new Thought ();
		}
	}

}


[System.Serializable]
public class Thought
{
	public LogicManager.GameState state;
	public MWord word;
	public string thought{ get { return word.word; } }

}