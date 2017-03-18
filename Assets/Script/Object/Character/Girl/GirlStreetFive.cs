using System.Collections;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GirlStreetFive : TalkableCharacter {

	[SerializeField] RangeTarget[] target;
	[SerializeField] float waitSpeakInterval = 5f;
	[SerializeField] LayerMask damageMask;
	UnityEngine.AI.NavMeshAgent m_agent;
	[SerializeField] Animator m_animator;
	[SerializeField] GameObject Model;
	[SerializeField] GirlState initState;
	[SerializeField] LogicManager.GameState prepareState = LogicManager.GameState.WalkWithGirlModern;
	[SerializeField] LogicEvents arriveEndEvent = LogicEvents.StreetFiveGirlArriveEnd;
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
		Prepare,
		Walk, // walk with out umbrella
		TalkWithOutUmbrella,
		WalkWithUmbrella, 
		TakeOutUmbrella,
		TakeOffUmbrella,
		WaitForPlayer,
		TalkWithPlayerInUmbrella,
		ArriveEnd,
		Disappear,
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

		m_stateMachine.BlindStateChangeEvent (LogicEvents.StreetFiveGirlEndTalk, GirlState.Prepare, GirlState.WalkWithUmbrella);

		m_stateMachine.AddEnter (GirlState.Init, delegate() {
			Model.SetActive(false);
			m_collider.enabled = false;
			m_agent.enabled = false;
		});

		m_stateMachine.AddUpdate (GirlState.Init, delegate() {
			if ( LogicManager.Instance.State == prepareState )
			{
				m_stateMachine.State = GirlState.Prepare;
			}
		});

		m_stateMachine.AddExit (GirlState.Init, delegate() {
			Model.SetActive(true);
			m_collider.enabled = true;
			m_agent.enabled = true;
		});

		m_stateMachine.AddEnter (GirlState.Prepare, delegate() {
			m_animator.SetTrigger("WalkInRain");
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

			if ( CheckTarget () )
				NextTarget();
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
			m_collider.radius = 0.7f;

			if (m_umbrellaAudioSource != null) {
				m_umbrellaAudioSource.clip = soundSetting.umbrellaWalk;
				m_umbrellaAudioSource.loop = true;
				m_umbrellaAudioSource.Play ();
			}
		});
		m_stateMachine.AddUpdate (GirlState.WalkWithUmbrella, delegate() {
			m_agent.speed = MainCharacter.Instance.MoveSpeed * 0.96f;

//			if ( LogicManager.Instance.State < LogicManager.GameState.WalkWithGirlFrame )
			{
				if (!m_IsPlayerIn ) 	
					m_stateMachine.State = GirlState.WaitForPlayer;
			}

			if ( LogicManager.Instance.State < LogicManager.GameState.WalkWithGirlModern )
			{
				// if behind player
				if ( transform.position.x < MainCharacter.Instance.transform.position.x )
				{

				}else {
					m_agent.speed = MainCharacter.Instance.MoveSpeed * 0.5f;
				}
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
			if ( CheckTarget () )
				NextTarget();

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

		m_stateMachine.AddExit (GirlState.WaitForPlayer, delegate {

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

		m_stateMachine.AddEnter (GirlState.ArriveEnd, delegate {

			M_Event.FireLogicEvent(arriveEndEvent, new LogicArg(this));
			m_stateMachine.State = GirlState.Disappear;
		});

		m_stateMachine.AddEnter (GirlState.Disappear, delegate {
			Model.SetActive(false);
			m_collider.enabled = false;
			m_agent.enabled = false;
		});

		if (Application.isEditor)
			m_stateMachine.State = initState;
		else
			m_stateMachine.State = GirlState.Init;
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
//		M_Event.RegisterAll (OnEvent);

		M_Event.RegisterEvent (LogicEvents.StreetFiveGirlEndTalk, OnEvent);
		//		M_Event.RegisterEvent (LogicEvents.BusStopTalkPointOne, OnEvent);
		//		M_Event.RegisterEvent (LogicEvents.BusStopTalkPointTwo, OnEvent);
		//		M_Event.RegisterEvent (LogicEvents.EnterStone, OnEvent);
		//		M_Event.RegisterEvent (LogicEvents.ForceGirlLeave, OnEvent);
		//		M_Event.RegisterEvent (LogicEvents.PlayMusic, OnEvent);

	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		//		M_Event.RegisterAll (OnEvent);
		M_Event.UnregisterEvent (LogicEvents.StreetFiveGirlEndTalk, OnEvent);
		//		M_Event.UnregisterEvent (LogicEvents.BusStopTalkPointOne, OnEvent);
		//		M_Event.UnregisterEvent (LogicEvents.BusStopTalkPointTwo, OnEvent);
		//		M_Event.UnregisterEvent (LogicEvents.EnterStone, OnEvent);
		//		M_Event.UnregisterEvent (LogicEvents.ForceGirlLeave, OnEvent);
		//		M_Event.UnregisterEvent (LogicEvents.PlayMusic, OnEvent);
	}


	void OnEvent(LogicArg arg)
	{

		m_stateMachine.OnEvent (arg.type);
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		UpdateSenseRain ();
		m_stateMachine.Update ();
	}

	public bool CheckTarget()
	{
		return (m_agent.remainingDistance <= 0.15f ) &&
			( !m_agent.hasPath || m_agent.velocity.magnitude == 0);
		//		Vector3 offset = transform.position - m_agent.destination;
		//		offset.y = 0;
		//		if ( offset.magnitude < targetToleranceRange ) {
		//			NextTarget ();
		//		}
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
		isInRain = CheckIfInRain();
	}
	virtual protected bool CheckIfInRain()
	{

		if (Physics.Raycast (transform.position, Vector3.up, 100f, damageMask.value)) {
			return false;
		}
		return true;
	}

	[ReadOnlyAttribute] public int targetIndex = 0;
	virtual protected void NextTarget()
	{
		if (m_agent != null && m_agent.enabled ) {

			if (targetIndex < target.Length) {

				m_agent.destination = target [targetIndex].GetRangeTarget ();

				targetIndex++;

				Debug.Log ("Next Target");
			} else {

				m_stateMachine.State = GirlState.ArriveEnd;
			}
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

	protected override void OnEndFilmControl (LogicArg arg)
	{
		base.OnEndFilmControl (arg);

		m_realTalking = false;
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

	public override bool IsInteractable ()
	{
		if (m_stateMachine.State == GirlState.Prepare)
			return true;
		
		return false;
	}


	void OnGUI()
	{
		if (m_stateMachine.State != GirlState.Init && m_stateMachine.State != GirlState.Disappear ) {
			GUILayout.Label ("");
			GUILayout.Label ("GirlStreetFiveState " + m_stateMachine.State);
			GUILayout.Label ("Girl is end talking " + m_realTalking);
		}
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
//	void Leave ( )
//	{
//		Sequence seq = DOTween.Sequence ();
//		seq.AppendInterval (2f);
//		seq.AppendCallback (delegate() {
//			Debug.Log("Leave Player");
//			m_stateMachine.State = GirlState.WalkAway;
//			M_Event.FireLogicEvent (LogicEvents.InvisibleFromPlayer, new LogicArg (this));
//			M_Event.FireLogicEvent (LogicEvents.SwitchDefaultBGM, new LogicArg (this));
//		});
		//		Vector3 forward = Camera.main.transform.forward;
		//		forward.y = 0;
		//		transform.DOMove ( - forward.normalized * 4f, 0.2f).SetRelative(true).OnComplete (delegate() {
		//			gameObject.SetActive (false);
		//		});

//	}
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
