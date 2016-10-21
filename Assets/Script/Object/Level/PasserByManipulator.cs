using UnityEngine;
using System.Collections;

public class PasserByManipulator : MBehavior {

	[SerializeField] GameObject passerByPrefab;
	[SerializeField] MinMax createInterval;
	[SerializeField] Vector3 createArea;
	[SerializeField] MinMax speed;
	[SerializeField] Vector3 target;

	float timer = 0;
	protected override void MUpdate ()
	{
		base.MUpdate ();

		timer -= Time.deltaTime;
		if (timer <= 0) {
			timer = createInterval.RandomBetween;
			Create ();
		}
	}

	void Create()
	{
		GameObject passerBy = (GameObject)Instantiate (passerByPrefab);
		passerBy.transform.SetParent (transform);
		passerBy.transform.localPosition = 
			new Vector3 (Random.Range(-createArea.x,createArea.x) ,Random.Range(-createArea.y,createArea.y) ,Random.Range(-createArea.z,createArea.z) );

		NormalPasserBy passerByCom = passerBy.GetComponent<NormalPasserBy> ();
		passerByCom.SetSpeed (speed.RandomBetween);
		passerByCom.SetTarget (target);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube (transform.position, createArea * 2f );

		Gizmos.color = new Color (1f, 0.6f, 0);
		Gizmos.DrawSphere (target, 0.5f);
	}
}
