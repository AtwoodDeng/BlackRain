using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
public class RotateStrategy : DanceStrategy {

	[SerializeField] public float radius;
	[SerializeField] public float initDegreed;
	[SerializeField] public float DegreedPerBeat;
	[SerializeField] public float DegreedSpeed;
	[SerializeField] Transform Center;
	[ReadOnlyAttribute] public float m_InnerDegreed;
	public float Degreed{
		get {
			return m_InnerDegreed;
		}
		set {
			if (Center != null) {
				targetPosition = Center.transform.position + new Vector3 (Mathf.Cos (value * Mathf.Deg2Rad), 0, Mathf.Sin (value * Mathf.Deg2Rad)) * radius; 
			}
			m_InnerDegreed = value;
		}
	}
	[ReadOnlyAttribute] public Vector3 targetPosition;
	[ReadOnlyAttribute] public float targetRadius;
	public bool updateInitPosition;

//	public override void OnBeatRhythm (int index)
//	{
//		Debug.Log ( name + "Rotate On Beat ");
//		targetRadius += DegreedPerBeat;
//	}

	public override void OnBeat (int count)
	{
		base.OnBeat (count);
		if (CanMoveOnBeat (count))
			targetRadius = initDegreed + ( count - onEventCount ) / beatFliter * DegreedPerBeat;
	}

	public override void OnGotoDanceUpdate ()
	{
		base.OnGotoDanceUpdate ();
		Degreed = initDegreed;
		parent.m_agent.SetDestination (targetPosition);
	}

	public override void OnDanceUpdate ()
	{
		base.OnDanceUpdate ();

		if ( DegreedPerBeat > 0 && Degreed < targetRadius)
			Degreed += DegreedSpeed * Time.deltaTime;

		if ( DegreedPerBeat < 0 && Degreed > targetRadius)
			Degreed += DegreedSpeed * Time.deltaTime;
		
		parent.m_agent.SetDestination (targetPosition);
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		if (updateInitPosition) {
			Degreed = initDegreed;
			transform.position = targetPosition;
			updateInitPosition = false;
		}
	}

	 
	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (Center.position, radius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (targetPosition, 0.1f);
	}
}
