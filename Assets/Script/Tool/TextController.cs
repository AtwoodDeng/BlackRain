using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MBehavior {
	[ReadOnlyAttribute] public Text m_text;
	[SerializeField] MWord word;
	protected override void MAwake ()
	{
		base.MAwake ();
		m_text = GetComponent<Text> ();
		if (m_text == null)
			m_text = GetComponentInChildren<Text> ();
		m_text.text = word.word;

	}
}
