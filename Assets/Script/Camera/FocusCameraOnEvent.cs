using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FocusCameraOnEvent : OnEventResponsor {

	[SerializeField] Transform moveCameraTo;
	[SerializeField] float delayTime;
	[SerializeField] float moveTime;
	[SerializeField] float lastTime;
	[SerializeField] LogicEvents endEvent;
	[SerializeField] Ease type = Ease.Linear;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);

		if (enabled) {
			M_Event.FireLogicEvent (LogicEvents.FocusCamera, new LogicArg (this));
			Debug.Log ("Focus");
			Camera toCam = moveCameraTo.gameObject.GetComponent<Camera> ();
			float camOriFOV = Camera.main.fieldOfView;
			Sequence seq = DOTween.Sequence ();
			seq.AppendInterval (delayTime);
			seq.Append (Camera.main.transform.DOMove (moveCameraTo.position, moveTime)).SetEase (type);
			seq.Join (Camera.main.transform.DORotate (moveCameraTo.eulerAngles, moveTime)).SetEase (type);
			if (toCam != null)
				seq.Join (Camera.main.DOFieldOfView (toCam.fieldOfView, moveTime).SetEase (type));
			seq.AppendInterval (lastTime);
			seq.AppendCallback (delegate() {
				Camera.main.fieldOfView = camOriFOV;
				M_Event.FireLogicEvent (LogicEvents.UnfocusCamera, new LogicArg (this));	
				if (endEvent != LogicEvents.None)
					M_Event.FireLogicEvent (endEvent, new LogicArg (this));
			});
		}
	}
}
