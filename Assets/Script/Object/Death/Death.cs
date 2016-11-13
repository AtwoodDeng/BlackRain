using UnityEngine;
using System.Collections;

public class Death : MObject {
	[SerializeField] GameObject deathPeoplePrefab;

	[SerializeField] float circleRadius = 3f;
	[SerializeField] float createRadius = 5f;
	[SerializeField] int deathPeopleNumber = 10;
	[SerializeField] float createInterval = 0.2f;
	[SerializeField] LogicManager.GameState beginState;
	[SerializeField] LogicManager.GameState endState;
	[SerializeField] bool isStartOnAwake = false;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (isStartOnAwake)
			StartDeath ();
	}

	protected override void MStart ()
	{
		base.MStart ();
		LogicManager.Instance.RegisterStateChange ( OnStateChange );
	}

	void OnStateChange( LogicManager.GameState fromState , LogicManager.GameState toState )
	{
		Debug.Log("Death  " + toState );
		if ( toState == beginState )
		{
			StartDeath();	
		}else if ( toState == endState )
		{
			gameObject.SetActive( false );
		}
		
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.DeathEnd] += OnDeathEnd;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.DeathEnd] -= OnDeathEnd;
	}

	void OnDeathEnd(LogicArg arg )
	{
		Destroy (gameObject);
	}

	public void InitDeath(Vector3 position)
	{
		Debug.Log ("Init Death");
		Vector3 myPos = position;
		myPos.y = 0;
		transform.position = myPos;
		StartDeath ();
	}

	void StartDeath()
	{
		for (int i = 0; i < deathPeopleNumber; ++i) {
			GameObject people = Instantiate (deathPeoplePrefab, transform, false) as GameObject;

			float toAngle = Random.Range (-180f, 0f ) * Mathf.Deg2Rad;
			float range = circleRadius * Random.Range (0.9f, 1.1f);
			Vector3 toPos = new Vector3 (Mathf.Cos (toAngle), 0.13f , Mathf.Sin (toAngle)) * range ;
			people.transform.localPosition = toPos;
			DeathPeople dp = people.GetComponent<DeathPeople> ();
			dp.Init ( transform.position , transform ,  range + 0.1f);
		}

		for (int i = 0; i < deathPeopleNumber / 5 ; ++i) {
			GameObject people = Instantiate (deathPeoplePrefab, transform, false) as GameObject;

			float toAngle = Random.Range (0f, 270f) * Mathf.Deg2Rad;
			float range = circleRadius * Random.Range (0.9f, 1.1f);
			Vector3 toPos = new Vector3 (Mathf.Cos (toAngle), 0.13f , Mathf.Sin (toAngle)) * range ;
			people.transform.localPosition = toPos;
			DeathPeople dp = people.GetComponent<DeathPeople> ();
			dp.Init ( transform.position , transform ,  range + 0.1f);
		}
		
	}
	
//	void StartDeath()
//	{
//		StartCoroutine (CreatePeople());
//	}

	IEnumerator CreatePeople()
	{
		for (int i = 0; i < deathPeopleNumber; ++i) {
			Random.InitState ((int)(Time.time * 777));

			GameObject people = Instantiate (deathPeoplePrefab, transform, false) as GameObject;
			float angle = Random.Range (-100f, 100f);
			Vector3 pos = new Vector3 (Mathf.Cos (angle) * createRadius, 0 , Mathf.Sin (angle) * createRadius);
			people.transform.localPosition = pos;

			float toAngle = angle + Random.Range (-30f , 30f) * Mathf.Deg2Rad;
			float range = circleRadius * Random.Range (0.9f, 1.1f);
			Vector3 toPos = new Vector3 (Mathf.Cos (toAngle) * range, 0 , Mathf.Sin (toAngle) * range) ;
			DeathPeople dp = people.GetComponent<DeathPeople> ();
			dp.Init ( transform.position , transform ,  range);

			yield return new WaitForSeconds (createInterval);
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color (1f, 0.7f, 0);
		Gizmos.DrawWireSphere (transform.position, circleRadius);

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (transform.position,  createRadius);
	}
}
