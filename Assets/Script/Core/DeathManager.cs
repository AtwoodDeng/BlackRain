using UnityEngine;
using System.Collections;

public class DeathManager : MBehavior {

	[SerializeField] GameObject deathPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.C) && Input.GetKey (KeyCode.LeftControl))
			M_Event.FireLogicEvent (LogicEvents.Death, new LogicArg (this));
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.Death] += OnDeath;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.Death] -= OnDeath;
	}

	void OnDeath(LogicArg arg )
	{
		if (deathPrefab != null) {
			GameObject deathObj = Instantiate (deathPrefab) as GameObject;
			Death death = deathObj.GetComponent<Death> ();
			death.InitDeath (LogicManager.MainCharacter.transform.position);
		}
	}

}
