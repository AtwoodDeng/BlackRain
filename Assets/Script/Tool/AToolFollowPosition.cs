using UnityEngine;
using System.Collections;

public class AToolFollowPosition : MBehavior {

	[SerializeField] Transform target;
	[SerializeField] float closeRate = 30f;

	[SerializeField] bool followX;
	[SerializeField] bool followY;
	[SerializeField] bool followZ;

	[SerializeField] bool UseOriginalOffest;

	[SerializeField] bool resetOnAwake;

	Vector3 originalOffset;

	protected override void MAwake ()
	{
		base.MAwake ();

		if (UseOriginalOffest) {
			originalOffset = transform.position - target.position;
		} else
			originalOffset = Vector3.zero;

		if (resetOnAwake)
			transform.position = GetTargetPosition ();
	}

	Vector3 GetTargetPosition()
	{
		if (target == null)
			return Vector3.zero;
		Vector3 res;
		Vector3 targetPosition = target.position + originalOffset;
		res.x = followX ? targetPosition.x : transform.position.x;
		res.y = followY ? targetPosition.y : transform.position.y;
		res.z = followZ ? targetPosition.z : transform.position.z;
		return res;
	}

	// Update is called once per frame
	void Update () {
		Vector3 targetPos = GetTargetPosition ();
		transform.position = Vector3.Lerp (transform.position, targetPos, Mathf.Clamp01(closeRate * Time.deltaTime));
	}
}
