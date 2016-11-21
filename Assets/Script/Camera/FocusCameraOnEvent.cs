using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FocusCameraOnEvent : OnEventResponsor {

	[SerializeField] Transform moveCameraTo;
	[SerializeField] float delayTime;
	[SerializeField] float moveTime;
	[SerializeField] float lastTime;
	[SerializeField] LogicEvents endEvent;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);
		M_Event.FireLogicEvent (LogicEvents.FocusCamera, new LogicArg (this));
		Debug.Log ("Focus");
		Sequence seq = DOTween.Sequence ();
		seq.AppendInterval (delayTime);
		seq.Append (Camera.main.transform.DOMove (moveCameraTo.position, moveTime));
		seq.Join (Camera.main.transform.DORotate (moveCameraTo.eulerAngles, moveTime));
		seq.AppendInterval (lastTime);
		seq.AppendCallback (delegate() {
			M_Event.FireLogicEvent(LogicEvents.UnfocusCamera , new LogicArg(this));	
			Debug.Log("Unfocus");
			if ( endEvent != LogicEvents.None )
				M_Event.FireLogicEvent(endEvent , new LogicArg( this ));
		});
	}
}
