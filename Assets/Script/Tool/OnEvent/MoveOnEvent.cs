using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MoveOnEvent : OnEventResponsor {
	[SerializeField] Transform target;
	[SerializeField] Vector3 move;
	[SerializeField] float Time;
	[SerializeField] float delay = 0;
	[SerializeField] bool relative;
	[SerializeField] bool disableOnEnd;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);
		Debug.Log ("Move");
		target.DOMove (move, Time).SetRelative (relative).SetDelay(delay).SetEase(Ease.Linear).OnComplete (delegate() {
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
