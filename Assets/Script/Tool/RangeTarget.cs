using UnityEngine;
using System.Collections;

public class RangeTarget : MBehavior {
	[SerializeField] float radius = 1f;

	public Vector3 GetRangeTarget()
	{
		float angle = Random.Range (0, Mathf.PI);
		Vector3 offset = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle));
		return transform.position + offset * Random.Range (0, radius);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawWireSphere (transform.position, radius);
	}
}
