using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceStrategy : MBehavior {
	[ReadOnlyAttribute] public DanceCharacter parent;
	[SerializeField] DanceCharacter.DanceAnimation danceInitAnimation = DanceCharacter.DanceAnimation.None;
	[SerializeField] protected int beatFliter = 2;

	virtual public void Init( DanceCharacter _p )
	{
		parent = _p;
	}

	public virtual void OnGotoDanceEnter ()
	{
		parent.m_agent.SetDestination (parent.OriginalPosition);
	}

	public virtual void OnGotoDanceUpdate()
	{
		if (parent.IsGetDestination ()) {
			parent.SetStateFromTo (DanceCharacter.State.GotoDance, DanceCharacter.State.Dance);
		}
	}

	public virtual void OnDanceEnter() {
		Debug.Log ("Enter Play " + danceInitAnimation);
		parent.SetTrigger (danceInitAnimation);
	}

	public virtual void OnDanceUpdate()
	{
	}

	public virtual void OnBeat( int count )
	{
		
	}


}
