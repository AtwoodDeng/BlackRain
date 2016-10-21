using UnityEngine;
using System.Collections;
using DG.Tweening;

[ExecuteInEditMode]
public class ASelfRotate : MBehavior {

	[SerializeField] bool ifRotateOnAwake = true;
	[SerializeField] Vector3 rotateDirection;
	[Tooltip("The cycle Time to rotate 360 degree")]
	[SerializeField] float rotateCycleTime = 1f;
	[SerializeField] bool playOnEditor = true;

	protected override void MUpdate ()
	{
		base.MUpdate ();

		if ( !(Application.isEditor && !playOnEditor) ) {
			Vector3 rotateAngle = rotateDirection.normalized * (360f / rotateCycleTime);
			transform.Rotate (rotateAngle * Time.deltaTime);
		}
	}

}
