using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMoveStrategy : DanceStrategy {

	Vector3 moveDirection{
		get {
			return new Vector3 (Mathf.Cos (angle * Mathf.Deg2Rad) * length * beatFliter, 0 ,
				Mathf.Sin (angle * Mathf.Deg2Rad) * length * beatFliter);
		}
	}
	[SerializeField] float angle;
	[SerializeField] float length;
//	[SerializeField] bool MoveOnBeat;
	Vector3 nowDirection;

	public override void Init (DanceCharacter _p)
	{
		base.Init (_p);
		nowDirection = moveDirection;
	}
	public override void OnDanceUpdate ()
	{
		base.OnDanceUpdate ();
		if ( !MoveOnBeat && parent.IsGetDestination ()) {
			parent.m_agent.SetDestination (GetDestination ());
		}
	}

	Vector3 GetDestinationIndex( int beatIndex )
	{
//		int beatIndex = ( ( count - beatOffset) / beatFliter ) % 2;
		nowDirection = (beatIndex == 0 ) ? moveDirection : Vector3.zero;

		return parent.OriginalPosition + nowDirection * beatFliter;
	}

	Vector3 GetDestination(  )
	{

		if (nowDirection.Equals (moveDirection))
			nowDirection = Vector3.zero;
		else
			nowDirection = moveDirection;
		return parent.OriginalPosition + nowDirection * beatFliter;
	}

	public override void OnBeatRhythm (int index)
	{
		parent.m_agent.SetDestination (GetDestinationIndex (index));
	}

//	public override void OnBeat ( int count )
//	{
//		base.OnBeat ( count );
//
//		if (MoveOnBeat && GetBeatIndex(count) == 0 ) {
////			Debug.Log ("Get Destination " + count);
//			parent.m_agent.SetDestination (GetDestinationCount ( count ));
//		}
//	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		if ( parent != null && parent.m_state == DanceCharacter.State.Dance) {
			Gizmos.DrawLine ( transform.position, GetDestination() );
		} else {
			Gizmos.DrawLine (transform.position, transform.position + moveDirection * beatFliter);	
		}

	}
}
