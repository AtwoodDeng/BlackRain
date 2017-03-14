using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartStrategy : DanceStrategy {
//	[SerializeField] MinMax range;
	[SerializeField] Vector3 offset;

	public override void OnGotoDanceEnter ()
	{
		base.OnGotoDanceEnter ();

//		Vector3 offset = Random.insideUnitSphere * range.RandomBetween;
//		offset.y = 0;
		parent.m_agent.SetDestination (MainCharacter.Instance.transform.position + offset);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine (transform.position, transform.position - offset);
//		Gizmos.DrawWireSphere (transform.position, range.min);
//		Gizmos.color = Color.Lerp (Color.red, Color.yellow, 0.5f);
//		Gizmos.DrawWireSphere (transform.position, range.max);
	}
}
