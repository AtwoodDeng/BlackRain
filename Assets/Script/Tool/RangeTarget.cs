using UnityEngine;
using System.Collections;

public class RangeTarget : MBehavior {
	public enum Type
	{
		Sphere,
		Square,
	}
	[SerializeField] Type type;
	[SerializeField] float radius = 1f;
	[SerializeField] float length = 1f;
	[SerializeField] float width = 1f;

	public Vector3 GetRangeTarget()
	{
		if (type == Type.Sphere) {
//			float angle = Random.Range (0, Mathf.PI);
//			Vector3 offset = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle));
//			return transform.position + offset * Random.Range (0, radius);
			Vector3 offset = Random.insideUnitCircle * radius;
			offset.z = offset.y;
			offset.y = 0;
			return transform.position + offset;
		} else if (type == Type.Square) {
			Vector3 offset = new Vector3 (Random.Range (-length, length) / 2f, 0 , Random.Range (-width, width) / 2f);
			return transform.position + offset;
		}

		return transform.position;
	}

	void OnDrawGizmosSelected()
	{
		
		Gizmos.color = Color.red;
		if ( type == Type.Sphere )
			Gizmos.DrawWireSphere (transform.position, radius);
		else if ( type == Type.Square )
			Gizmos.DrawWireCube(transform.position , new Vector3( length ,1f , width ));
	}
}
