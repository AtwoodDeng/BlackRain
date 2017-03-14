using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOnEvent : OnEventResponsor {
	[SerializeField] string startTrigger;
	[SerializeField] string eventTrigger;
	 public Animator m_animator;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);
		if (m_animator != null && !string.IsNullOrEmpty (eventTrigger))
			m_animator.SetTrigger (eventTrigger);
	}

	protected override void MAwake ()
	{
		base.MAwake ();
		if (m_animator == null)
			m_animator = GetComponent<Animator> ();
		if (m_animator == null)
			m_animator = GetComponentInChildren<Animator> ();
	}

	protected override void MStart ()
	{
		base.MStart ();
		if (m_animator != null && !string.IsNullOrEmpty (startTrigger))
			m_animator.SetTrigger (startTrigger);
	}
}
