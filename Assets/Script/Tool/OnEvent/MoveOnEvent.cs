using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MoveOnEvent : OnEventResponsor {
	[SerializeField] Transform target;
	[SerializeField] Vector3 move;
	[SerializeField] float Time;
	[SerializeField] bool relative;
	[SerializeField] bool disableOnEnd;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);
		target.DOMove (move, Time).SetRelative (relative).OnComplete (delegate() {
			if ( disableOnEnd )
				target.gameObject.SetActive( false );	
		});
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (target.position, (relative? target.position : Vector3.zero ) + move);
	}
}
