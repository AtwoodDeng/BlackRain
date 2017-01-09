using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class NormalPasserBy : TalkableCharacter {

	[SerializeField] protected UnityEngine.AI.NavMeshAgent m_agent;
	private float MAgentSpeed;
	[SerializeField] Animator m_animator;
	[SerializeField] NarrativePlotScriptableObject[] enterUmbrellaPlot;
	[SerializeField] float enterPlotInterval = 5f;
	[SerializeField] List<Vector3> targetList;
	private int targetIndex = 0;
	[SerializeField] bool dieOnLastTarget = true;
	public float targetToleranceRange = 0.5f;
	bool isWaittingForGreen = false;
//	public bool loop = false;

	[System.Serializable]
	public struct RenderSetting
	{
		public MeshRenderer umbrellaUp;
		public MeshRenderer umbrellaDown;
		public MeshRenderer umbrellaShadow;
		public MeshRenderer head;
		public MeshRenderer body;
		public Gradient UmbrellaColor;
		public Gradient bodyColor;
		public bool UseColorfulUmbrella;
		public bool UseColorfulSkin;
	}
	[SerializeField] RenderSetting renderSetting;

	[System.Serializable]
	public struct SoundSetting
	{
		public AudioClip umbrellaWalk;
		public AudioClip umbrellaTakeOut;
		public AudioClip umbrellaTakeOff;
//		public AnimationCurve curve;
	}
	[SerializeField] SoundSetting soundSetting;

	AudioSource m_umbrellaAudioSource;


	public enum Type
	{
		Normal,
		Friendly,
		Unfriendly,
		ExtremFriendly,
	}

	[System.Serializable]
	public struct AISetting
	{
		public LayerMask damageMask;
		public Type type;

	};
	[SerializeField] protected AISetting m_AISetting;

	bool m_IsPlayerIn;
	public bool IsPlayerIn{
		get { return m_IsPlayerIn; }
	}


	bool m_isOpenUmbrella;
	public bool IsOpenUmbrella{
		get { return m_isOpenUmbrella; }
	}

	[SerializeField] float waitDuration = 5f;

	protected override void MAwake ()
	{
		base.MAwake ();
		InitRender ();
		if (m_animator == null)
			m_animator = GetComponentInChildren<Animator> ();
		if (m_agent == null) {
			m_agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		}
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

	static int shipTalkTotal = 0;
	protected override void MStart ()
	{
		base.MStart ();
		m_agent.enabled = true;
		NextTarget ();

		LogicManager.Instance.RegisterStateChange (delegate(LogicManager.GameState fromState, LogicManager.GameState toState) {

			if ( toState == LogicManager.GameState.BeginShip  && shipTalkTotal < 5 )
			{
				Vector3 toward = (MainCharacter.Instance.transform.position - transform.position );
				if ( toward.magnitude < 20f && Vector3.Dot( toward , Camera.main.transform.forward ) > 0.1f )
				{
					NarrativePlotScriptableObject shipCome = (NarrativePlotScriptableObject)Resources.Load("NarrativeDyn/ShipComes");
					Sequence seq = DOTween.Sequence();
					seq.AppendInterval( Random.Range( 0 , 5f ));
					seq.OnComplete(delegate() {
						DisplayDialog( shipCome );
					});
				}
				shipTalkTotal ++;
			}

//			if ( toState == LogicManager.GameState.BeginShip && m_AISetting.type == Type.Unfriendly)
//			{
//				NarrativePlotScriptableObject plot1 = (NarrativePlotScriptableObject)Resources.Load("NarrativeDyn/FindGrilNarrative1");
//				NarrativePlotScriptableObject plot2 = (NarrativePlotScriptableObject)Resources.Load("NarrativeDyn/FindGrilNarrative2");
//				subPlots = new NarrativePlotScriptableObject[2];
//				subPlots[0] = plot1;
//				subPlots[1] = plot2;
//			}
		});
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (LogicEvents.TrafficGreenLight, OnTrafficLightGreen);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (LogicEvents.TrafficGreenLight, OnTrafficLightGreen);
	}

	void OnTrafficLightGreen( LogicArg arg )
	{
		if (isWaittingForGreen) {
			RecoverMove ();
			isWaittingForGreen = false;
			gameObject.tag = "PasserBy";
		}
	}

	public void InitRender()
	{
		MeshRenderer[] renders = GetComponentsInChildren<MeshRenderer> ();

		if (renderSetting.umbrellaUp == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.Equals ("UMBRELLA:top1"))
					renderSetting.umbrellaUp = r;
			}
		}
		if (renderSetting.umbrellaUp != null) {
			if (renderSetting.umbrellaUp.GetComponent<Collider> () == null)
				renderSetting.umbrellaUp.gameObject.AddComponent<MeshCollider> ();
			renderSetting.umbrellaUp.gameObject.layer = LayerMask.NameToLayer ("Umbrella");
		}

		if (renderSetting.umbrellaDown == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.Equals ("UMBRELLA:top2"))
					renderSetting.umbrellaDown = r;
			}
		}
		if (renderSetting.umbrellaDown != null) {
			renderSetting.umbrellaDown.enabled = false;
			if (renderSetting.umbrellaDown.GetComponent<Collider> () == null)
				renderSetting.umbrellaDown.gameObject.AddComponent<MeshCollider> ();
			renderSetting.umbrellaDown.gameObject.layer = LayerMask.NameToLayer ("Umbrella");
		}

		if (renderSetting.umbrellaShadow == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.Equals ("UMBRELLA:TopShadow"))
					renderSetting.umbrellaShadow = r;
			}
		}
		if (renderSetting.umbrellaShadow != null)
			renderSetting.umbrellaShadow.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;


		if (renderSetting.head == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.Equals ("Head"))
					renderSetting.head = r;
			}
		}

		if (renderSetting.body == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.Equals ("body"))
					renderSetting.body = r;
			}
		}


		if (renderSetting.UseColorfulUmbrella) {
			Color umbrellaColor = renderSetting.UmbrellaColor.Evaluate (Random.Range (0, 1f));
			umbrellaColor.a = 0.3f;
			renderSetting.umbrellaUp.material = new Material (Shader.Find ("AlphaSelfIllum_NoFog"));
			renderSetting.umbrellaUp.material.SetColor ("_Color", umbrellaColor);

		}

		if (renderSetting.UseColorfulSkin) {
			Color bodyColor = renderSetting.bodyColor.Evaluate (Random.Range (0, 1f));// * Mathf.LinearToGammaSpace(0.35f);
			renderSetting.head.material = new Material (renderSetting.head.material.shader);
			renderSetting.head.material.SetColor ("_EmissionColor", bodyColor);
			renderSetting.body.material = renderSetting.head.material;
		}
	}

	public void SetSpeed( float speed )
	{
		MAgentSpeed = speed;
		if (m_agent != null)
			m_agent.speed = MAgentSpeed;
	}

	public void SetTarget( Vector3[] target )
	{
		targetList.AddRange (target);
	}

	float lastPlayEnterDialogTime = 0;


	static int shipComingCount = 0;
	protected override void MOnTriggerEnter ( Collider  col)
	{
		base.MOnTriggerEnter (col);
		if (isWaittingForGreen && col.tag == "TrafficObstacle" && LogicManager.Instance.State < LogicManager.GameState.WalkAcrossRoadWithGirl) {
//			Debug.Log ("Meet Traffic Obstcale ");
			LockMove ();
			gameObject.tag = "TrafficObstacle";
		}

		if ( col.tag == "Player")
		{
			m_IsPlayerIn = true;

			if (IsOpenUmbrella && enterUmbrellaPlot != null && enterUmbrellaPlot.Length > 0 && ( Time.time - lastPlayEnterDialogTime ) > enterPlotInterval ) {
				if (shipComingCount < 5) {
					DisplayDialog (enterUmbrellaPlot [Random.Range (0, enterUmbrellaPlot.Length)]);	
					lastPlayEnterDialogTime = Time.time;
					shipComingCount++;
				}
			}

			if (m_AISetting.type == Type.Friendly || m_AISetting.type == Type.ExtremFriendly ) {
				m_agent.speed = MainCharacter.Instance.FollowSpeed;
			}

			if (m_AISetting.type == Type.Unfriendly) {
				m_agent.speed = MAgentSpeed * 3f;
			}
		}else if (col.tag == "WaitForGreen") {
			isWaittingForGreen = true;
//			Debug.Log ("Wait for green ");
		}
	}

	void OnTriggerExit(Collider col )
	{
		if (col.tag == "Player") {
			m_IsPlayerIn = false;

			if ( m_AISetting.type == Type.ExtremFriendly )  {
				LockMove();
			}

			if ( ( m_AISetting.type == Type.Friendly ) && IsOpenUmbrella ) {
				LockMove ();
				Sequence seq = DOTween.Sequence ();
				seq.AppendInterval (waitDuration);
				seq.AppendCallback (delegate() {
					if (!m_IsPlayerIn)
						RecoverMove ();
				});
			}
			if (m_AISetting.type == Type.Unfriendly && IsOpenUmbrella) {
				Sequence seq = DOTween.Sequence ();
				seq.AppendInterval (waitDuration);
				seq.AppendCallback (delegate() {
					if (!m_IsPlayerIn)
						RecoverMove ();
				});
			}

		}


	}

	public override void Interact ()
	{
		base.Interact ();

	}
	protected override void DisplayDialog (NarrativePlotScriptableObject plot)
	{
		base.DisplayDialog (plot);

		if ( plot != null && plot.important )
			LockMove ();
	}

		
	protected override void OnEndDisplayDialog (LogicArg arg)
	{
		if (IsTalking) {
			Debug.Log ("End DisplayDialog " + IsPlayerIn + m_AISetting.type);
			RecoverMove ();
		}
		base.OnEndDisplayDialog (arg);
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		UpdateSenseRain ();

		CheckLeave ();

		if ( isWaittingForGreen )
			LimitSpeed ();
	}

	void LimitSpeed()
	{
		if ( (m_AISetting.type == Type.Friendly || m_AISetting.type == Type.Normal) && m_IsPlayerIn && m_isOpenUmbrella ) {
			m_agent.speed = MainCharacter.Instance.MoveSpeed * 0.88f;
		}
		if ( m_agent.velocity.magnitude > m_agent.speed * 2f && m_agent.speed > 0  ) {
			Debug.Log ("Exceed limit speed");
			LockMove ();
		}
	}

	virtual protected void CheckLeave()
	{
		Vector3 offset = transform.position - m_agent.destination;
		offset.y = 0;
		if ( offset.magnitude < targetToleranceRange ) {
			NextTarget ();
		}
	}

	virtual protected void NextTarget()
	{
		if (m_agent != null && m_agent.enabled) {
			
			if (targetIndex >= targetList.Count) {
				if ( dieOnLastTarget )
					gameObject.SetActive (false);
				return;
			}
			
			m_agent.destination = targetList [targetIndex];

			targetIndex++;
		}
	}

	protected bool isInRain = false;
	void UpdateSenseRain()
	{
		bool isNowInRain = CheckIfInRain ();
		if (isInRain != isNowInRain) {
			if (isNowInRain) {
				LockMove ();
				m_animator.SetTrigger ("TakeOut");
				if (m_umbrellaAudioSource != null) {
					m_umbrellaAudioSource.clip = soundSetting.umbrellaTakeOut;
					m_umbrellaAudioSource.loop = false;
					m_umbrellaAudioSource.Play ();
				}
//				Debug.Log ("Set TakeOut");
			} else {
				LockMove ();
				m_animator.SetTrigger ("TakeOff");
				if (m_umbrellaAudioSource != null) {
					m_umbrellaAudioSource.clip = soundSetting.umbrellaTakeOff;
					m_umbrellaAudioSource.loop = false;
					m_umbrellaAudioSource.Play ();
				}
//				Debug.Log ("Set TakeOff");
			}
		}
		isInRain = isNowInRain;
	}

	virtual public void LockMove()
	{
//		Debug.Log ("Lock Move");
		m_agent.speed = 0;
	}

	virtual public void RecoverMove()
	{
//		Debug.Log ("Recover Move " + MAgentSpeed );

		if ( (m_AISetting.type == Type.ExtremFriendly || m_AISetting.type == Type.Friendly) ){
			if ( IsPlayerIn )
				m_agent.speed = MainCharacter.Instance.FollowSpeed;
		} else {
			m_agent.speed = MAgentSpeed;
		}
	}

	virtual protected bool CheckIfInRain()
	{
		
		if (Physics.Raycast (transform.position, Vector3.up, 100f, m_AISetting.damageMask.value)) {
			return false;
		}
		return true;
	}

	public override void OnAnimationEnd (string info)
	{
		base.OnAnimationEnd (info);
		if (info == "TakeOff" || info == "TakeOut") {
			// for the one first meet in front of the building
			if (m_AISetting.type == Type.ExtremFriendly) {
				if (IsPlayerIn)
					RecoverMove ();
			} else {
				RecoverMove ();
			}
		}

		if (info == "TakeOff")
			m_isOpenUmbrella = false;
		else if (info == "TakeOut") {
			m_isOpenUmbrella = true;
			if (m_umbrellaAudioSource != null) {
				m_umbrellaAudioSource.clip = soundSetting.umbrellaWalk;
				m_umbrellaAudioSource.loop = true;
				m_umbrellaAudioSource.Play ();
			}
		}
	}

//	void OnDrawGizmosSelected()
//	{
//		Gizmos.color = Color.blue;
//		Vector3 toward = transform.position + velocity;
//		Gizmos.DrawLine (transform.position, toward);
//		Gizmos.DrawSphere (toward, 0.2f);
//	}
}
