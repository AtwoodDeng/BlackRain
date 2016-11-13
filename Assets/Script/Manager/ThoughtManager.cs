using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class ThoughtManager : MBehavior {

	static ThoughtManager m_Instance;
	public static ThoughtManager Instance{ 
		get { 
			if (m_Instance == null)
				m_Instance = FindObjectOfType<ThoughtManager> ();
			return m_Instance;}}


	[SerializeField] private ThoughtScritableObject preloadThoughtList;
	[SerializeField] private ThoughtScritableObject enterStateThoughtList;
	[SerializeField] private ThoughtScritableObject damageThoughtList;
	[SerializeField] private float tiredTime = 30f;
	private float tiredTimer = 0;

	List<Thought> thoughtsForState = new List<Thought>();


	protected override void MStart ()
	{
		LogicManager.Instance.RegisterStateChange (OnStateChange);
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterAll (OnEvent);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnRegisterAll (OnEvent);
	}

	bool isDamaged = false;
	void OnEvent( LogicArg arg )
	{
		if (arg.type == LogicEvents.BeginDamage) {
			if (LogicManager.Instance.State <= LogicManager.GameState.WalkInStreetTwo) {
				if (!isDamaged) {
					isDamaged = true;
					SendThought (damageThoughtList.mainThought);
				} else {
					SendThought (damageThoughtList.RandomThought);
				}
			}
		}
	}

	void OnResetTiredTime ( LogicArg arg )
	{
		ResetTiredTime ();
	}

	void OnStateChange( LogicManager.GameState fromState , LogicManager.GameState toState )
	{
//		Debug.Log ("To state " + toState);
		thoughtsForState.Clear ();
		foreach (Thought thought in preloadThoughtList.thoughtList) {
			if (thought.state == toState || thought.state == LogicManager.GameState.EveryState) {
				thoughtsForState.Add (thought);
			}
		}


		foreach (Thought thought in enterStateThoughtList.thoughtList) {
			if (thought.state == toState ) {
				SendThought (thought);
			}
		}

		ResetTiredTime ();
	}

	public Thought GetTempThought()
	{
		if ( thoughtsForState.Count > 0 )
			return thoughtsForState[Random.Range(0,thoughtsForState.Count)];
		return new Thought ();
	}

	public void SendThought( Thought thought )
	{
		if ( thought.thought != "" ) {
			LogicArg arg = new LogicArg (this);
			arg.AddMessage (M_Event.EVENT_THOUGHT, thought);
//			Debug.Log ("Send thought " + thought.thought);
			M_Event.FireLogicEvent (LogicEvents.DisplayThought, arg);
		}
	}

	public void ResetTiredTime()
	{
		tiredTimer = 0;
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		tiredTimer += Time.deltaTime;

		if (CrossPlatformInputManager.GetButtonDown ("SkipThought")) {
			M_Event.FireLogicEvent (LogicEvents.HideThought, new LogicArg (this));
		}

		if (tiredTimer > tiredTime || ( Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T)) ) {
			SendThought (GetTempThought ());
			ResetTiredTime ();
		}
	}



}
