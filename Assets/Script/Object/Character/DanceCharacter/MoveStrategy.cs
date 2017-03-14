using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStrategy : DanceStrategy {

	[SerializeField] Transform[] targetList;
	[ReadOnlyAttribute] public int Index = 0;
	[SerializeField] bool MoveOnBeat;

	public override void Init (DanceCharacter _p)
	{
		base.Init (_p);
	}
	public override void OnDanceUpdate ()
	{
		base.OnGotoDanceUpdate ();
		if ( !MoveOnBeat && parent.IsGetDestination ()) {
			parent.m_agent.SetDestination (GetDestination ());
		}
	}

	Vector3 GetDestination()
	{
		return targetList [(Index++) % targetList.Length].position;
	}

	public override void OnBeat ( int count )
	{
		base.OnBeat ( count );

		if ( MoveOnBeat && (count % beatFliter == 0) )
			parent.m_agent.SetDestination (GetDestination ());
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		for (int i = 1; i < targetList.Length; ++i)
			Gizmos.DrawLine (targetList [i - 1].position, targetList [i].position);
	}
}
