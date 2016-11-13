using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GirlBusStop : NormalPasserBy {

	[SerializeField] RangeTarget[] target;
	[SerializeField] NarrativePlotScriptableObject talkPointOne;
	[SerializeField] NarrativePlotScriptableObject talkPointTwo;
	[SerializeField] NarrativePlotScriptableObject wait;
	[SerializeField] NarrativePlotScriptableObject[] playMusic;
	[SerializeField] float waitSpeakInterval = 5f;
	[SerializeField] Rigidbody ropeTop;
	[SerializeField] Rigidbody ropeEnd;
//	[SerializeField] LogicManager.GameState startFollowState;
//	[SerializeField] LogicManager.GameState endFollowState;

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (LogicEvents.BustStopTalkPointOne, OnEvent);
		M_Event.RegisterEvent (LogicEvents.BustStopTalkPointTwo, OnEvent);
		M_Event.RegisterEvent (LogicEvents.ForceGirlLeave, OnEvent);
		M_Event.RegisterEvent (LogicEvents.PlayMusic, OnEvent);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (LogicEvents.BustStopTalkPointOne, OnEvent);
		M_Event.UnregisterEvent (LogicEvents.BustStopTalkPointTwo, OnEvent);
		M_Event.UnregisterEvent (LogicEvents.ForceGirlLeave, OnEvent);
		M_Event.UnregisterEvent (LogicEvents.PlayMusic, OnEvent);
	}


	void OnEvent(LogicArg arg)
	{
		if (arg.type == LogicEvents.BustStopTalkPointOne)
			DisplayDialog (talkPointOne);
		else if (arg.type == LogicEvents.BustStopTalkPointTwo)
			DisplayDialog (talkPointTwo);
		else if (arg.type == LogicEvents.ForceGirlLeave) {
			MechanismManager.health.SetHealthToMin ();
//			Leave ();
		} else if (arg.type == LogicEvents.PlayMusic) {
			ReactToMusic (arg);
		}
	}

	void ReactToMusic( LogicArg arg )
	{
		string musicName = (string)arg.GetMessage (M_Event.EVENT_PLAY_MUSIC_NAME);
		StartCoroutine( ReactToMusicDelay(Random.Range( 21f , 32f ) , musicName) );
	}

	void ReactToMusic( string musicName)
	{
		if (LogicManager.Instance.State == LogicManager.GameState.ListenToMusic) {

			if (musicName == "LadyAndBird") {
						DisplayDialog (playMusic [0]);
					} else
						DisplayDialog (playMusic [1]);
				}
	}

	IEnumerator ReactToMusicDelay( float time , string musicName)
	{
		yield return new WaitForSeconds (time);

		ReactToMusic (musicName);
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		if (LogicManager.Instance.State == LogicManager.GameState.DepartFromGirl) {
//			if (MechanismManager.Instance.DamageState == MechanismManager.DamageStateType.UnderDamage) {
				Vector3 girlToCamera = Camera.main.transform.position - transform.position;
				girlToCamera.y = 0;
				Vector3 forward = Camera.main.transform.forward;
				forward.y = 0;

				if (Vector3.Dot (girlToCamera.normalized, forward.normalized) > 0.1f ) {
					Leave ( );
				}
//			}
		}

		if (LogicManager.Instance.State < LogicManager.GameState.DepartFromGirl )
		{
			if ( (!IsPlayerIn && isInRain) || IsTalking ) {
				LockMove ();
			} else {
				RecoverMove ();
			}
		}else{
			Vector3 forward = Camera.main.transform.forward;
			forward.y = 0;

			m_agent.speed = 12f;
			m_agent.SetDestination (transform.position + forward * -5f);
		}

		if (!IsMainEnded && m_agent.speed > 0 )
			LockMove ();
	}


	void Leave ( )
	{
		Vector3 forward = Camera.main.transform.forward;
		forward.y = 0;
		LockMove ();
		transform.DOMove ( - forward.normalized * 4f, 0.2f).SetRelative(true).OnComplete (delegate() {
			gameObject.SetActive (false);
			M_Event.FireLogicEvent(LogicEvents.InvisibleFromPlayer, new LogicArg(this));
			M_Event.FireLogicEvent(LogicEvents.SwitchDefaultBGM ,new LogicArg(this));
		});
		
	}


	public override void LockMove ()
	{
		base.LockMove ();
		m_collider.radius = 0.45f;
	}

	public override void RecoverMove ()
	{
		base.RecoverMove ();
		if (LogicManager.Instance.State >= LogicManager.GameState.DepartFromGirl) {
			m_agent.speed = 3.5f;

		} else {
			m_agent.speed = MainCharacter.Instance.MoveSpeed * 0.88f ;
			m_collider.radius = 1.5f;
		}
	}

	IEnumerator StartWait()
	{
		float timer = waitSpeakInterval;
		while (true) {
			timer -= Time.deltaTime;

			if (m_agent.speed > 0)
				yield break;

			if (timer < 0) {
				DisplayDialog (wait);
				timer = waitSpeakInterval;
			}

			yield return new WaitForEndOfFrame ();
		}
	}

//	protected override void MFixedUpdate ()
//	{
//		base.MFixedUpdate ();
//
//		if (LogicManager.Instance.State >= startFollowState && LogicManager.Instance.State < endFollowState) {
//			Vector3 forward = MainCharacter.Instance.transform.position - transform.position;
//			Vector3 target = -forward.normalized * 0.75f + MainCharacter.Instance.transform.position;
//			m_agent.SetDestination ( target );
//		}
//	}


	protected override void MStart ()
	{
		base.MStart ();

		Vector3[] targetList = new Vector3[target.Length];
		for (int i = 0; i < targetList.Length; ++i) {
			targetList [i] = target[i].GetRangeTarget();
		}
		SetTarget (targetList);
		SetSpeed (m_agent.speed);
		LockMove ();

		LogicManager.Instance.RegisterStateChange (delegate(LogicManager.GameState fromState, LogicManager.GameState toState) {
			if ( toState == LogicManager.GameState.DepartFromGirl )
			{
				m_AISetting.type = Type.Normal;
			}
		});
	}


	protected override void OnEndDisplayDialog (LogicArg arg)
	{
		if (isMainTalking) {
			RecoverMove ();
		}
		base.OnEndDisplayDialog (arg);
	}

	protected override bool CheckIfInRain ()
	{
		
		return IsMainEnded;
	}


}
