using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkPassStreetNormalStrategy : NormalStrategy {

	[SerializeField] LogicEvents startWalkEvent = LogicEvents.AcrossStreetEndTalkGirl;
	[SerializeField] RangeTarget walkTo;

	public override void OnNormalUpdate ()
	{
		base.OnNormalUpdate ();

	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (startWalkEvent, OnStartWalk);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (startWalkEvent, OnStartWalk);
	}

	void OnStartWalk( LogicArg arg )
	{
		if ( parent.m_state == DanceCharacter.State.Normal)
			parent.m_agent.SetDestination (walkTo.GetRangeTarget ());
	}
}
