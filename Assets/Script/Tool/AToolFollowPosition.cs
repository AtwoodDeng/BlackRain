using UnityEngine;
using System.Collections;

public class AToolFollowPosition : MBehavior {

	[SerializeField] Transform target;
	[SerializeField] float closeRate = 30f;

	[SerializeField] bool followX;
	[SerializeField] bool followY;
	[SerializeField] bool followZ;

	[SerializeField] bool resetOnAwake;

	protected override void MAwake ()
	{
		base.MAwake ();

		if (resetOnAwake)
			transform.position = GetTargetPosition ();
	}

	Vector3 GetTargetPosition()
	{
		Vector3 res;
		res.x = followX ? target.position.x : transform.position.x;
		res.y = followY ? target.position.y : transform.position.y;
		res.z = followZ ? target.position.z : transform.position.z;
		return res;
	}

	// Update is called once per frame
	void Update () {
		Vector3 targetPos = GetTargetPosition ();
		transform.position = Vector3.Lerp (transform.position, targetPos, Mathf.Clamp01(closeRate * Time.deltaTime));
	}
}
