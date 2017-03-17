using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingScreen : MBehavior {
	[ReadOnlyAttribute] public Text m_text;
	public string words;
	public float interval = 0.1f  ;
	public int maxWords = 14;

	public void SetWord( string word )
	{
		words = string.Copy (word);
		words += "    ";
		while( words.Length < maxWords ) {
			words += " ";
		}


	}

	protected override void MAwake ()
	{
		base.MAwake ();

//		InvokeRepeating ("UpdateWord", 99999f, interval);
		if (m_text == null)
			m_text = GetComponent<Text> ();
		if ( m_text == null )
			m_text = GetComponentInChildren<Text>();
		SetWord (" ");
	}

	float timer;
	protected override void MUpdate ()
	{
		timer -= Time.deltaTime;
		if (timer < 0) {
			UpdateWord ();
			timer = interval;
		}
		base.MUpdate ();
	}

	void UpdateWord()
	{
		words = words.Substring (1) + words [0];
		m_text.text = words.Substring(0,maxWords);
	}
}
