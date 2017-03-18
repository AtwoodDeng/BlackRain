using UnityEngine;
using System.Collections;

public class PasserByManipulator : MBehavior {

	[SerializeField] GameObject[] passerByPrefab;
	[SerializeField] MinMax createInterval;
	[SerializeField] Vector3 createArea;
	[SerializeField] MinMax speed;
	[SerializeField] RangeTarget[] target;
	[SerializeField] LogicManager.GameState beginState = (LogicManager.GameState)0;
	[SerializeField] LogicManager.GameState endState = (LogicManager.GameState)99;
	[SerializeField] int MaxPeople = -1;
	[SerializeField] float tolarenceRange = 0.5f;

	float timer = 0;
	int totalPeople = 0;
	protected override void MUpdate ()
	{
		base.MUpdate ();

		if (LogicManager.Instance.State >= beginState && LogicManager.Instance.State < endState ) {
			if (MaxPeople < 0 || totalPeople < MaxPeople) {
				timer -= Time.deltaTime;
				if (timer <= 0) {
					timer = createInterval.RandomBetween;
					Create ();
				}
			}
		}
	}

	void Create()
	{
		GameObject passerBy = (GameObject)Instantiate (passerByPrefab[Random.Range(0,passerByPrefab.Length)]);
		passerBy.transform.SetParent (transform);
		passerBy.transform.position = transform.position +
			new Vector3 (Random.Range(-createArea.x,createArea.x) ,Random.Range(-createArea.y,createArea.y) ,Random.Range(-createArea.z,createArea.z) );
		Debug.Log ( " Create Passerby " + passerBy.name + " Set Position " + passerBy.transform.position);

		NormalPasserBy passerByCom = passerBy.GetComponent<NormalPasserBy> ();
		passerByCom.SetSpeed (speed.RandomBetween);
		passerByCom.targetToleranceRange = tolarenceRange;

		Vector3[] targetList = new Vector3[target.Length];
		for (int i = 0; i < targetList.Length; ++i) {
			targetList [i] = target [i].GetRangeTarget();
//			Debug.Log ("Target List " + i + " " + target [i].name);
		}
		passerByCom.SetTarget (targetList);

		totalPeople++;

	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube (transform.position, createArea * 2f );

//		Gizmos.color = new Color (1f, 0.6f, 0);
//		Gizmos.DrawSphere (target.position, 0.3f);
	}

}
