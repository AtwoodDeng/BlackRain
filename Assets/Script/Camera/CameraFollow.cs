using UnityEngine;
using System.Collections;

public class CameraFollow : MBehavior {

	private Vector3 targetPos;
	private Vector3 targetVelocity;
	[SerializeField] Transform target;
	[SerializeField] float distance = 10f;
	[SerializeField] float angle = 45f;
	[Range(0,0.1f)]
	[SerializeField] float positionCloseRate = 0.05f;
	[Range(0,60f)]
	[SerializeField] float rotateMaxAngle = 1f;

	private Vector3 ToPos;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (target != null) {
			targetPos = target.position;
		}
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		UpdateTarget ();
		UpdatePosition ();
	}

	void UpdateTarget()
	{
		if (target != null) {
			// update target position
			Vector3 lastPos = targetPos;
			targetPos = target.position;
			// update target velocity
			if ( ( targetPos - lastPos ).magnitude > Mathf.Epsilon)
				targetVelocity = (targetPos - lastPos) / Time.deltaTime;

			Vector3 InverseVel = -targetVelocity.normalized;
			Vector3 offset = (InverseVel + Vector3.up * Mathf.Tan (angle * Mathf.Deg2Rad)).normalized * distance;
			ToPos = offset + targetPos;
		}
	}

	void UpdatePosition()
	{
		transform.position = Vector3.Lerp (transform.position, ToPos, positionCloseRate);

		Vector3 targetDir = target.position - transform.position;
		float step = rotateMaxAngle * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotateMaxAngle , 0.0F);
		transform.rotation = Quaternion.LookRotation (newDir);
	}
}
