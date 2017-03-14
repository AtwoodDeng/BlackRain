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
	[SerializeField] bool isFrom = false;
	[SerializeField] Ease easeType = Ease.Linear;
	[SerializeField] LogicEvents endEvents;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);
//		Debug.Log ("Move");
		if (isFrom) {
			target.DOMove (move, Time).From().SetRelative (relative).SetDelay(delay).SetEase(easeType).OnComplete (delegate() {
				if ( disableOnEnd )
					target.gameObject.SetActive( false );	
				if ( endEvents != LogicEvents.None )
					M_Event.FireLogicEvent(endEvents, new LogicArg(this) );
			});
		} else {
			target.DOMove (move, Time).SetRelative (relative).SetDelay (delay).SetEase (easeType).OnComplete (delegate() {
				if (disableOnEnd)
					target.gameObject.SetActive (false);	
				if ( endEvents != LogicEvents.None )
					M_Event.FireLogicEvent(endEvents, new LogicArg(this) );
			});
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (target.position, (relative? target.position : Vector3.zero ) + move);
	}
}
