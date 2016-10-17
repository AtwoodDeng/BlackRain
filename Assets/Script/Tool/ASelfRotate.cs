using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ASelfRotate : MBehavior {

	[SerializeField] bool ifRotateOnAwake = false;
	[SerializeField] Vector3 rotateDirection;
	[SerializeField] float rotateSpeed = 1;
	[SerializeField] int loopTime = 1;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (ifRotateOnAwake) {
			BeginRotate ();
		}
	}

	void BeginRotate()
	{
		transform.DOLocalRotate (rotateDirection, 1f / rotateSpeed).SetRelative(true).SetLoops (loopTime, LoopType.Incremental);
	}
}
