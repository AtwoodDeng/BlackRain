using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpinOnEvent : OnEventResponsor {
	[SerializeField] Vector3 spinAngle=Vector3.zero;
	[SerializeField] float duration=0;
	[SerializeField] bool isRelative=false;
	[SerializeField] float delay=0;
	[SerializeField] Ease easeType;
	[SerializeField] int loopTime=1;
	[SerializeField] LoopType loopType;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);
//		Debug.Log ("On Spin");
		transform.DORotate (spinAngle, duration).SetDelay (delay).SetEase (easeType).SetLoops (loopTime, loopType).SetRelative (isRelative);
	}
}
