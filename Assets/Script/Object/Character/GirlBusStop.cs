using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GirlBusStop : TalkableCharacter {

	[SerializeField] RangeTarget[] target;
	[SerializeField] float targetToleranceRange = 0.3f;
	[SerializeField] NarrativePlotScriptableObject talkPointOne;
	[SerializeField] NarrativePlotScriptableObject talkPointTwo;
	[SerializeField] NarrativePlotScriptableObject stonePlot;
	[SerializeField] NarrativePlotScriptableObject wait;
//	[SerializeField] NarrativePlotScriptableObject[] playMusic;
	[SerializeField] float waitSpeakInterval = 5f;
	[SerializeField] Rigidbody ropeTop;
	[SerializeField] Rigidbody ropeEnd;
	[SerializeField] LayerMask damageMask;
	UnityEngine.AI.NavMeshAgent m_agent;
	[SerializeField] Animator m_animator;
	[SerializeField] GameObject Model;
	[System.Serializable]
	public struct SoundSetting
	{
		public AudioClip umbrellaWalk;
		public AudioClip umbrellaTakeOut;
		public AudioClip umbrellaTakeOff;
		//		public AnimationCurve curve;
	}
	[SerializeField] SoundSetting soundSetting;


	public enum GirlState
	{
		None,
		Init,
		Walk, // walk with out umbrella
		TalkWithOutUmbrella,
		WalkWithUmbrella, 
		TakeOutUmbrella,
		TakeOffUmbrella,
		WaitForPlayer,
		TalkWithPlayerInUmbrella,
		LeavePlayer,
		WalkAway,
	}

	AStateMachine<GirlState,LogicEvents> m_stateMachine;
	AudioSource m_umbrellaAudioSource;

//	[SerializeField] LogicManager.GameState startFollowState;
//	[SerializeField] LogicManager.GameState endFollowState;

	protected override void MAwake ()
	{
		base.MAwake ();
		m_stateMachine = new AStateMachine<GirlState, LogicEvents> (GirlState.None);
		m_agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
//		NextTarget ();
		m_animator = GetComponentInChildren<Animator> ();

		InitStateMachine ();


		{
			m_umbrellaAudioSource = gameObject.AddComponent<AudioSource> ();
			m_umbrellaAudioSource.volume = 0.7f;
			m_umbrellaAudioSource.spatialBlend = 1f;
			m_umbrellaAudioSource.playOnAwake = false;
			m_umbrellaAudioSource.minDistance = 1f;
			m_umbrellaAudioSource.maxDistance = 10f;
			//			AnimationCurve curve = new AnimationCurve ();
			//			curve.AddKey (new Keyframe (0, 1f));
			//			curve.AddKey (new Keyframe (1f, 0));
			//			m_umbrellaAudioSource.SetCustomCurve (AudioSourceCurveType.CustomRolloff , soundSetting.curve);
			m_umbrellaAudioSource.Stop ();
		}
	}

	float stateTimer = 0;
	void InitStateMachine()
	{
		m_stateMachine.AddEnter (GirlState.Init, delegate() {
			m_agent.speed = 0;
		});

		m_stateMachine.AddEnter (GirlState.Walk, delegate() {
			m_agent.speed = MainCharacter.Instance.FollowSpeed;
		});
		m_stateMachine.AddUpdate (GirlState.Walk, delegate() {
			if ( isInRain )
			{
				m_stateMachine.State = GirlState.TakeOutUmbrella;	
			}
			if ( m_realTalking )
			{
				m_stateMachine.State = GirlState.TalkWithOutUmbrella;
			}
		});

		m_stateMachine.AddEnter (GirlState.TakeOutUmbrella, delegate() {
			m_agent.speed = 0;
			m_animator.SetTrigger("TakeOut");

			if (m_umbrellaAudioSource != null) {
				m_umbrellaAudioSource.clip = soundSetting.umbrellaTakeOut;
				m_umbrellaAudioSource.loop = false;
				m_umbrellaAudioSource.Play ();
			}
		});
		m_stateMachine.AddUpdate (GirlState.TakeOutUmbrella, delegate {
			if (m_IsOpenUmbrella) {
				m_stateMachine.State = GirlState.WalkWithUmbrella;
			}
		});
		m_stateMachine.AddEnter (GirlState.TakeOffUmbrella, delegate() {
			m_agent.speed = 0;
			m_animator.SetTrigger("TakeOff");
			if (m_umbrellaAudioSource != null) {
				m_umbrellaAudioSource.clip = soundSetting.umbrellaTakeOff;
				m_umbrellaAudioSource.loop = false;
				m_umbrellaAudioSource.Play ();
			}
		});
		m_stateMachine.AddUpdate (GirlState.TakeOffUmbrella, delegate {
			if (!m_IsOpenUmbrella) {
				m_stateMachine.State = GirlState.Walk;
			}
		});

		m_stateMachine.AddEnter (GirlState.WalkWithUmbrella, delegate() {
			m_collider.radius = 1.5f;

			if (m_umbrellaAudioSource != null) {
				m_umbrellaAudioSource.clip = soundSetting.umbrellaWalk;
				m_umbrellaAudioSource.loop = true;
				m_umbrellaAudioSource.Play ();
			}
		});
		m_stateMachine.AddUpdate (GirlState.WalkWithUmbrella, delegate() {
			m_agent.speed = MainCharacter.Instance.MoveSpeed * 0.96f;

			if ( !m_IsPlayerIn && LogicManager.Instance.State < LogicManager.GameState.WalkOutStreetFour)
			{
				m_stateMachine.State = GirlState.WaitForPlayer;
			}

//			if ( IsTalking && !m_IsEndTalking )
			if ( m_realTalking )
			{
				m_stateMachine.State = GirlState.TalkWithPlayerInUmbrella;
			}

			if ( !isInRain )
			{
				m_stateMachine.State = GirlState.TakeOffUmbrella;
			}

		});

		m_stateMachine.AddEnter (GirlState.WaitForPlayer, delegate() {
			m_agent.speed = 0;	
			m_collider.radius = 0.45f;
		});
		m_stateMachine.AddUpdate (GirlState.WaitForPlayer, delegate() {
			if ( m_IsPlayerIn )
			{
				m_stateMachine.State = GirlState.WalkWithUmbrella;
			}

		});

		m_stateMachine.AddEnter (GirlState.TalkWithPlayerInUmbrella, delegate() {
			m_agent.speed = 0;
		});
		m_stateMachine.AddUpdate (GirlState.TalkWithPlayerInUmbrella, delegate() {
			if ( !m_realTalking )
			{
				m_stateMachine.State = GirlState.WalkWithUmbrella;
			}
		});

		m_stateMachine.AddEnter (GirlState.TalkWithOutUmbrella, delegate() {
			m_agent.speed = 0;	
		});
		m_stateMachine.AddUpdate (GirlState.TalkWithOutUmbrella, delegate() {
			if ( !m_realTalking )
			{
				m_stateMachine.State = GirlState.Walk;
			}
		});

		m_stateMachine.BlindFromEveryState (LogicEvents.ForceGirlLeave, GirlState.LeavePlayer);

		m_stateMachine.AddEnter (GirlState.LeavePlayer, delegate() {
			stateTimer = 0 ;		
		});

		m_stateMachine.AddUpdate (GirlState.LeavePlayer, delegate() {
			Vector3 forward = Camera.main.transform.forward;
			forward.y = 0;

			m_agent.speed = 12f;
			m_agent.SetDestination (transform.position + forward * -5f);	

			stateTimer += Time.deltaTime;

			if ( stateTimer > 2f )
				m_stateMachine.State = GirlState.WalkAway;
		});

		m_stateMachine.AddEnter (GirlState.WalkAway, delegate() {
			if ( Model != null )
				Model.SetActive(false);
			Model.SetActive( false );	
			M_Event.FireLogicEvent (LogicEvents.InvisibleFromPlayer, new LogicArg (this));
			M_Event.FireLogicEvent (LogicEvents.SwitchDefaultBGM, new LogicArg (this));
		});


		m_stateMachine.State = GirlState.Init;
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (LogicEvents.BustStopTalkPointOne, OnEvent);
		M_Event.RegisterEvent (LogicEvents.BustStopTalkPointTwo, OnEvent);
		M_Event.RegisterEvent (LogicEvents.EnterStone, OnEvent);
		M_Event.RegisterEvent (LogicEvents.ForceGirlLeave, OnEvent);
		M_Event.RegisterEvent (LogicEvents.PlayMusic, OnEvent);

	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (LogicEvents.BustStopTalkPointOne, OnEvent);
		M_Event.UnregisterEvent (LogicEvents.BustStopTalkPointTwo, OnEvent);
		M_Event.UnregisterEvent (LogicEvents.EnterStone, OnEvent);
		M_Event.UnregisterEvent (LogicEvents.ForceGirlLeave, OnEvent);
		M_Event.UnregisterEvent (LogicEvents.PlayMusic, OnEvent);
	}


	void OnEvent(LogicArg arg)
	{
		if (arg.type == LogicEvents.BustStopTalkPointOne) {
			DisplayDialog (talkPointOne);
		} else if (arg.type == LogicEvents.BustStopTalkPointTwo) {
			DisplayDialog (talkPointTwo);
		} else if (arg.type == LogicEvents.EnterStone) {
			DisplayDialog (stonePlot);
		} else if (arg.type == LogicEvents.ForceGirlLeave) {
//			MechanismManager.health.SetHealthToMin ();
//			Leave ();
		} else if (arg.type == LogicEvents.PlayMusic) {
//			ReactToMusic (arg);
		}

		m_stateMachine.OnEvent (arg.type);
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		UpdateSenseRain ();
		m_stateMachine.Update ();
		CheckTarget ();
	}

	void CheckTarget()
	{

		Vector3 offset = transform.position - m_agent.destination;
		offset.y = 0;
		if ( offset.magnitude < targetToleranceRange ) {
			NextTarget ();
		}
	}

	protected override void DisplayDialog (NarrativePlotScriptableObject plot)
	{
		base.DisplayDialog (plot);
		if ( plot != null )
		{
			if (plot.important)
				m_realTalking = true;
		}
	}


	protected bool isInRain = false;
	void UpdateSenseRain()
	{
//		bool isNowInRain = CheckIfInRain ();
//		if (isInRain != isNowInRain) {
//			if (isNowInRain) {
//				LockMove ();
//				m_animator.SetTrigger ("TakeOut");
//				//				Debug.Log ("Set TakeOut");
//			} else {
//				LockMove ();
//				m_animator.SetTrigger ("TakeOff");
//				//				Debug.Log ("Set TakeOff");
//			}
//		}
		isInRain = CheckIfInRain();
	}
	virtual protected bool CheckIfInRain()
	{

		if (Physics.Raycast (transform.position, Vector3.up, 100f, damageMask.value)) {
			return false;
		}
		return true;
	}

	int targetIndex = 0;
	virtual protected void NextTarget()
	{
		if (m_agent != null && m_agent.enabled && targetIndex < target.Length) {

			m_agent.destination = target [targetIndex].GetRangeTarget();

			targetIndex++;
		}
	}

	bool m_IsOpenUmbrella = false;
	public override void OnAnimationEnd (string info)
	{
		base.OnAnimationEnd (info);
		if (info == "TakeOut")
			m_IsOpenUmbrella = true;
		else if (info == "TakeOff")
			m_IsOpenUmbrella = false;
	}

	bool m_realTalking = false;
	protected override void OnEndDisplayDialog (LogicArg arg)
	{
		base.OnEndDisplayDialog (arg);
		TalkableCharacter character = (TalkableCharacter)arg.GetMessage (M_Event.EVENT_END_DISPLAY_SENDER);

		if (character == this) {
			if (m_stateMachine.State == GirlState.Init) {
				m_stateMachine.State = GirlState.Walk;
			}

			m_realTalking = false;
		}
	}


	bool m_IsPlayerIn;
	protected override void MOnTriggerEnter (Collider col)
	{
		base.MOnTriggerEnter (col);

		if (col.tag == "Player")
			m_IsPlayerIn = true;
	}

	protected override void MOnTriggerExit (Collider col)
	{
		base.MOnTriggerExit (col);
		if (col.tag == "Player")
			m_IsPlayerIn = false;
	}

	void OnGUI()
	{
//		GUILayout.Label ("");
//		GUILayout.Label ("GirlState" + m_stateMachine.State);
//		GUILayout.Label ("Girl is end talking " + m_realTalking);
	}

//	void ReactToMusic( LogicArg arg )
//	{
//		string musicName = (string)arg.GetMessage (M_Event.EVENT_PLAY_MUSIC_NAME);
//		StartCoroutine( ReactToMusicDelay(Random.Range( 21f , 32f ) , musicName) );
//	}
//
//
//	void ReactToMusic( string musicName)
//	{
//		if (LogicManager.Instance.State == LogicManager.GameState.ListenToMusic) {
//
//			if (musicName == "LadyAndBird") {
//				DisplayDialog (playMusic [0]);
//			} else if (musicName == "Parade") {
//				DisplayDialog (playMusic [1]);
//			} else if (musicName == "AnHeQiao") {
//				DisplayDialog (playMusic [2]);
//			}
//
//		}
//	}
//
//	IEnumerator ReactToMusicDelay( float time , string musicName)
//	{
//		yield return new WaitForSeconds (time);
//
//		if ( AudioManager.Instance.switchBGMName == musicName )
//			ReactToMusic (musicName);
//	}
//
//	protected override void MUpdate ()
//	{
//		base.MUpdate ();
//		if (LogicManager.Instance.State == LogicManager.GameState.DepartFromGirl) {
////			if (MechanismManager.Instance.DamageState == MechanismManager.DamageStateType.UnderDamage) {
//				Vector3 girlToCamera = Camera.main.transform.position - transform.position;
//				girlToCamera.y = 0;
//				Vector3 forward = Camera.main.transform.forward;
//				forward.y = 0;
//
//				if (Vector3.Dot (girlToCamera.normalized, forward.normalized) > 0.1f ) {
//					Leave ( );
//				}
////			}
//		}
//
//		if (LogicManager.Instance.State < LogicManager.GameState.WalkOutStreetFour) {
//			if ((!IsPlayerIn && isInRain) || IsTalking) {
//				LockMove ();
//			} else {
//				RecoverMove ();
//			}
//		} else if (LogicManager.Instance.State < LogicManager.GameState.DepartFromGirl) {
//
//			if (IsTalking) {
//				LockMove ();
//			} else {
//				RecoverMove ();
//			}
//		} else{
//			Vector3 forward = Camera.main.transform.forward;
//			forward.y = 0;
//
//			m_agent.speed = 12f;
//			m_agent.SetDestination (transform.position + forward * -5f);
//		}
//
//		if (!IsMainEnded && m_agent.speed > 0 )
//			LockMove ();
//	}
//
//
	void Leave ( )
	{
		Sequence seq = DOTween.Sequence ();
		seq.AppendInterval (2f);
		seq.AppendCallback (delegate() {
			Debug.Log("Leave Player");
			m_stateMachine.State = GirlState.WalkAway;
			M_Event.FireLogicEvent (LogicEvents.InvisibleFromPlayer, new LogicArg (this));
			M_Event.FireLogicEvent (LogicEvents.SwitchDefaultBGM, new LogicArg (this));
		});
//		Vector3 forward = Camera.main.transform.forward;
//		forward.y = 0;
//		transform.DOMove ( - forward.normalized * 4f, 0.2f).SetRelative(true).OnComplete (delegate() {
//			gameObject.SetActive (false);
//		});
		
	}
//
//
//	public override void LockMove ()
//	{
//		
//		base.LockMove ();
//		m_collider.radius = 0.5f;
//
//		if (waitCor == null) {
//			waitCor = StartCoroutine (StartWait());
//		}
//	}
//
//	public override void RecoverMove ()
//	{
//		base.RecoverMove ();
//		if (waitCor != null) {
//			StopCoroutine (waitCor);
//			waitCor = null;
//		}
//		if (LogicManager.Instance.State >= LogicManager.GameState.DepartFromGirl) {
//			m_agent.speed = 3.5f;
//
//		} else {
//			m_agent.speed = MainCharacter.Instance.MoveSpeed * 0.88f ;
//			m_collider.radius = 1.5f;
//		}
//	}
//
//	Coroutine waitCor = null ;
//	IEnumerator StartWait()
//	{
//		float timer = waitSpeakInterval * 0.3f ;
//		while (true) {
//			timer -= Time.deltaTime;
//
//			if (m_agent.speed > 0)
//				yield break;
//
//			if (LogicManager.Instance.State < LogicManager.GameState.InBusStop || LogicManager.Instance.State >= LogicManager.GameState.WalkOutStreetFour)
//				yield break;
//
//			if (timer < 0) {
//				DisplayDialog (wait);
//				timer = waitSpeakInterval;
//			}
//
//			yield return new WaitForEndOfFrame ();
//		}
//	}
//
////	protected override void MFixedUpdate ()
////	{
////		base.MFixedUpdate ();
////
////		if (LogicManager.Instance.State >= startFollowState && LogicManager.Instance.State < endFollowState) {
////			Vector3 forward = MainCharacter.Instance.transform.position - transform.position;
////			Vector3 target = -forward.normalized * 0.75f + MainCharacter.Instance.transform.position;
////			m_agent.SetDestination ( target );
////		}
////	}
//
//
//	protected override void MStart ()
//	{
//		base.MStart ();
//
//		Vector3[] targetList = new Vector3[target.Length];
//		for (int i = 0; i < targetList.Length; ++i) {
//			targetList [i] = target[i].GetRangeTarget();
//		}
//		SetTarget (targetList);
//		SetSpeed (m_agent.speed);
//		LockMove ();
//
//		LogicManager.Instance.RegisterStateChange (delegate(LogicManager.GameState fromState, LogicManager.GameState toState) {
//			if ( toState == LogicManager.GameState.DepartFromGirl )
//			{
//				m_AISetting.type = Type.Normal;
//			}
//		});
//	}
//
//
//	protected override void OnEndDisplayDialog (LogicArg arg)
//	{
//		if (isMainTalking) {
//			RecoverMove ();
//		}
//		base.OnEndDisplayDialog (arg);
//	}
//
//	protected override bool CheckIfInRain ()
//	{
//		
//		return IsMainEnded;
//	}


}
