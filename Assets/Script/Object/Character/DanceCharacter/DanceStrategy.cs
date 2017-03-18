using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceStrategy : MBehavior {
	[ReadOnlyAttribute] public DanceCharacter parent;
	[SerializeField] DanceCharacter.DanceAnimation danceInitAnimation = DanceCharacter.DanceAnimation.None;
	[SerializeField] protected int beatFliter = 2;
	[SerializeField] protected int beatOffset = 0;
	public bool MoveOnBeat = true;
	[SerializeField] LogicEvents waitEvent = LogicEvents.None;
	[ReadOnlyAttribute] public int onEventCount = -1;

	[ReadOnlyAttribute] public bool shouldWaitForEvent = true;


	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (waitEvent, OnEvent);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (waitEvent, OnEvent);
	}

	void OnEvent( LogicArg arg )
	{
		shouldWaitForEvent = false;
		onEventCount = parent.countRecord;
//		Debug.Log ("Is get Des " + parent.IsGetDestination ());
	}

	virtual public void Init( DanceCharacter _p )
	{
		parent = _p;
		if (waitEvent == LogicEvents.None)
			shouldWaitForEvent = false;
	}

	public virtual void OnGotoDanceEnter ()
	{
		parent.m_agent.SetDestination (parent.OriginalPosition);
	}

	public virtual void OnGotoDanceUpdate()
	{
		if (parent.IsGetDestination () && !shouldWaitForEvent ) {
			Debug.Log ( name + "Start Dance " + transform.position + " " + parent.OriginalPosition + " " + parent.m_agent.destination);
			parent.SetStateFromTo (DanceCharacter.State.GotoDance, DanceCharacter.State.Dance);
		}
	}

	public virtual void OnDanceEnter() {
//		Debug.Log ("Enter Play " + danceInitAnimation);
		parent.SetTrigger (danceInitAnimation);
	}

	public virtual void OnDanceUpdate()
	{
	}

	public virtual void OnBeat( int count )
	{
		if  ( CanMoveOnBeat( count ) ) {
			OnBeatRhythm (( ( count - beatOffset) / beatFliter ) % 2 );
		}
	}

	public bool CanMoveOnBeat( int count )
	{
		return MoveOnBeat && GetBeatIndex (count) == 0;
	}

	public virtual void OnBeatRhythm( int index )
	{
//		Debug.Log(name + "on Beat Rhythm" );
	}

	public int GetBeatIndex( int count )
	{
		return (count - beatOffset) % beatFliter;
	}

}
