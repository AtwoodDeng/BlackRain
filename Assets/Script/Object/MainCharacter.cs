using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using DG.Tweening;
using UnityStandardAssets.CrossPlatformInput;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterController))]
public class MainCharacter : MonoBehaviour {

	[System.Serializable]
	public struct DeathSetting
	{
		public float deathFloatHeight;
		public float deathFloatTime;
	};
	[SerializeField]  DeathSetting deathSetting;

	private static MainCharacter m_Instance;
	public static MainCharacter Instance
	{
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<MainCharacter> ();
			return m_Instance;
		}
	}

	[SerializeField] Transform m_CamTransform;                  // A reference to the main camera in the scenes transform
	Camera m_MainCamera;

	Rigidbody m_Rigidbody;
	CharacterController m_CharacterController;
	[SerializeField] Transform model;

	[SerializeField] float m_StationaryTurnSpeed = 180;
	public float StationaryTurnSpeed
	{
		get { return m_StationaryTurnSpeed * MechanismManager.health.HealthRate; }
	}
	[SerializeField] float m_MovingTurnSpeed = 360;
	public float MovingTurnSpeed
	{
		get { return m_MovingTurnSpeed * MechanismManager.health.HealthRate; }
	}
	[SerializeField] float m_MoveSpeedMultiplier = 1f;
	public float MoveSpeed
	{
//		get { return m_MoveSpeedMultiplier * MechanismManager.health.HealthRate; }
		get { return m_MoveSpeedMultiplier ; }
	}
	/// <summary>
	/// The speed for the passerby and girl to follow
	/// </summary>
	/// <value>The follow speed.</value>
	public float FollowSpeed
	{
		get { return MoveSpeed * 0.96f; }
	}
	[SerializeField] float m_RunSpeedMultiplier = 3f;
	public float RunSpeed
	{
//		get { return m_RunSpeedMultiplier * ( 0.6f + MechanismManager.health.HealthRate * 0.4f ) ; }
		get { return m_RunSpeedMultiplier; }
	}
	[SerializeField] CF.CameraBot.CameraBot cameraBot;
	private float oriCamBotSensity;
	SavePointData lastSavePoint;
	[SerializeField] float CameraSensity = 3.3f;
	private float CameraMechanismSensity = 1f;
	private float CameraNarrativeSensity = 1f;

	[SerializeField] Transform headTransform;
	[SerializeField] Transform umbrellaTransform;
	public Vector3 GetViewCenter() {
		return headTransform.position;
	}
	public Vector3 GetInteractiveCenter() {
		return headTransform.position ;
	}
	public Vector3 GetShareUmbrellaCenter() {
		Vector3 toward = umbrellaTransform.position - transform.position;
		toward.y = 0;
		return transform.position + toward * 1.5f; 
	}

	private bool m_Moveable = true;
	public bool Moveable
	{
		get { return m_Moveable; }
		set {
			if (m_Moveable != value) {
				if (value == false) {
					m_Rigidbody.velocity = Vector3.zero;
				}
			}

			m_Moveable = value;
		}
	}

	public bool m_IsMoving
	{
		get {
			return CrossPlatformInputManager.GetButton ("Horizontal") || CrossPlatformInputManager.GetButton ("Vertical");
		}
	}
	public bool m_IsRunning
	{
		get {
			return MechanismManager.health.SpeedUp > 0;
		}
	}

	public bool IsFocus{
		get { return m_isFocus; }
	}
	bool m_isFocus = false;
	bool m_isDisplay = false;
	bool m_isSneeze = false;
	bool m_isBreath = false;


	[SerializeField] Animator m_animator;

	void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
		m_CharacterController = GetComponent<CharacterController> ();

		if (m_CamTransform == null && Camera.main != null)
		{
			m_CamTransform = Camera.main.transform;
		}

		if (m_CamTransform != null)
			m_MainCamera = m_CamTransform.GetComponent<Camera> ();
		
		if (cameraBot == null)
			cameraBot = FindObjectOfType<CF.CameraBot.CameraBot> ();
		
		if ( m_animator != null )
			m_animator = GetComponentInChildren<Animator> ();

		cameraBot.InputSetting.Sensitive = CameraSensity;
		oriCamBotSensity = CameraSensity;
	}

	void OnEnable()
	{
		M_Event.logicEvents [(int)LogicEvents.Death] += OnDeath;
		M_Event.logicEvents [(int)LogicEvents.DisplayDialog] += OnDisplay;
		M_Event.logicEvents [(int)LogicEvents.EndDisplayDialog] += OnEndDisplay;
		M_Event.logicEvents [(int)LogicEvents.EnterSavePoint] += OnEnterSavePoint;
		M_Event.logicEvents [(int)LogicEvents.LockCamera] += OnLockCamera;
		M_Event.logicEvents [(int)LogicEvents.UnlockCamera] += OnUnlockCamer;
		M_Event.logicEvents [(int)LogicEvents.FocusCamera] += OnFocusCamera;
		M_Event.logicEvents [(int)LogicEvents.UnfocusCamera] += OnUnfocusCamera;
		M_Event.logicEvents [(int)LogicEvents.Sneeze] += OnSneeze;
		M_Event.logicEvents [(int)LogicEvents.Breath] += OnBreath;
		M_Event.RegisterEvent (LogicEvents.ToOld, OnToOld);
		M_Event.RegisterEvent (LogicEvents.ToModern, OnToModern);
	}

	void OnDisable()
	{
		M_Event.logicEvents [(int)LogicEvents.Death] -= OnDeath;
		M_Event.logicEvents [(int)LogicEvents.DisplayDialog] -= OnDisplay;
		M_Event.logicEvents [(int)LogicEvents.EndDisplayDialog] -= OnEndDisplay;
		M_Event.logicEvents [(int)LogicEvents.EnterSavePoint] -= OnEnterSavePoint;
		M_Event.logicEvents [(int)LogicEvents.LockCamera] -= OnLockCamera;
		M_Event.logicEvents [(int)LogicEvents.UnlockCamera] -= OnUnlockCamer;
		M_Event.logicEvents [(int)LogicEvents.FocusCamera] -= OnFocusCamera;
		M_Event.logicEvents [(int)LogicEvents.UnfocusCamera] -= OnUnfocusCamera;
		M_Event.logicEvents [(int)LogicEvents.Sneeze] -= OnSneeze;
		M_Event.logicEvents [(int)LogicEvents.Breath] -= OnBreath;
		M_Event.UnregisterEvent (LogicEvents.ToOld, OnToOld);
		M_Event.UnregisterEvent (LogicEvents.ToModern, OnToModern);
	}

	void OnToOld( LogicArg arg )
	{

		float delay = 0;
		float duration = 0;
		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DELAY ) )
			delay = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DELAY);

		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DURATION ) )
			duration = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DURATION);

		StartCoroutine (SetTriggerDelay ("Old", delay));

	}

	void OnToModern( LogicArg arg )
	{
		float delay = 0;
		float duration = 0;
		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DELAY ) )
			delay = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DELAY);

		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DURATION ) )
			duration = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DURATION);


		StartCoroutine (SetTriggerDelay ("Morden", delay));
	}

	IEnumerator SetTriggerDelay( string trigger , float delay )
	{
		yield return new WaitForSeconds (delay);
		if ( m_animator != null )
			m_animator.SetTrigger (trigger);
	}

	void OnBreath( LogicArg arg )
	{
		m_animator.SetTrigger ("Breath");
		Moveable = false;
		m_isBreath = true;
		Sequence seq = DOTween.Sequence ();
		seq.AppendInterval (1.5f);
		seq.AppendCallback (delegate() {
			if ( !m_isFocus && !m_isDisplay) {
				Moveable = true;
				m_isBreath = false;
			}
		});

	}

	void OnSneeze( LogicArg arg )
	{
		m_animator.SetTrigger ("Sneeze");
		Moveable = false;
		m_isSneeze = true;
		Sequence seq = DOTween.Sequence ();
		seq.AppendInterval (2f);
		seq.AppendCallback (delegate() {
			if ( !m_isFocus && !m_isDisplay) {
				Moveable = true;
				m_isSneeze = false;
			}
		});
	}



	void OnFocusCamera( LogicArg arg )
	{
		m_isFocus = true;
		cameraBot.enabled = false;
		Moveable = false;
	}

	void OnUnfocusCamera( LogicArg arg )
	{
		m_isFocus = false;
		cameraBot.enabled = true;
		Moveable = true;
	}


	void OnEnterSavePoint(LogicArg arg)
	{
		SavePointData data = (SavePointData)arg.GetMessage (M_Event.EVENT_SAVE_POINT);
		lastSavePoint = data;
	}

	void OnLockCamera( LogicArg arg )
	{
		CameraSensity = 0;
	}

	void OnUnlockCamer( LogicArg arg )
	{
		CameraSensity = oriCamBotSensity;
	}


	void OnDisplay( LogicArg arg )
	{
		TalkableCharacter character = (TalkableCharacter)arg.sender;
		NarrativePlotScriptableObject plot = (NarrativePlotScriptableObject)arg.GetMessage (M_Event.EVENT_DISPLAY_DIALOG_PLOT);

		if (plot.important) {
			if (character != null && character.NeedMoveCamera ()) {
				character.MoveCamera (m_MainCamera);
				cameraBot.enabled = false;
				Moveable = false;
				m_isDisplay = true;
			} else {
			
				if (plot != null && plot.lockCamera) {
					Moveable = false;
					m_isDisplay = true;
					CameraNarrativeSensity = 0.03f;

				}
			}
		}
//		m_UseHeadBob = false;
//		m_Move = false;
//		m_CanJump = false;
	}

	void OnEndDisplay( LogicArg arg )
	{
//		Debug.Log ("Camerabot enable");
		if (!m_isFocus) {
			Moveable = true;
			cameraBot.enabled = true;
			m_isDisplay = false;
		}
		CameraNarrativeSensity = 1f;
//		m_UseHeadBob = true;
//		m_Move = true;
//		m_CanJump = true;
	}

	private bool m_isOnDeath = false;
	void OnDeath( LogicArg arg )
	{
		m_isOnDeath = true;
		Moveable = false;
		transform.DORotate (new Vector3 (90f, 0, 0) , 2f ).SetRelative (true).SetEase(Ease.InCubic);
//		m_CharacterController.enabled = false;
//		m_MouseLook.enable = false;
//		m_FovKick.enable = false;
//		m_UseHeadBob = false;
		cameraBot.enabled = false;
		m_CamTransform.DOMoveY (deathSetting.deathFloatHeight , deathSetting.deathFloatTime).SetRelative (true).OnComplete(OnDeathEnd);
		m_CamTransform.DORotate (new Vector3 (75f, 0, 0), deathSetting.deathFloatTime / 3f ).SetEase(Ease.OutCubic);

	}

	void OnDeathEnd()
	{
		m_isOnDeath = false;
		Moveable = true;
		transform.DOKill ();
		transform.position = lastSavePoint.trans.position;
		transform.forward = lastSavePoint.trans.forward;

		Debug.Log ("Came enable death");
		cameraBot.enabled = true;
		CameraMechanismSensity = 1f;
		m_CamTransform.DOKill ();

		M_Event.FireLogicEvent (LogicEvents.DeathEnd, new LogicArg (this));
	}

	void Update()
	{
//		if (m_isOnDeath) {
//			if (Input.GetKeyDown (KeyCode.Escape)) {
//				m_Cam.DOKill ();
//				OnDeathEnd ();
//			}
//		}

		if (Input.GetKey (KeyCode.LeftControl)  && 
			Input.GetKeyDown (KeyCode.K)) {
			if (model != null) {
				Debug.Log ("Set model");
				model.gameObject.SetActive(!model.gameObject.activeSelf);
			}
		}
	}


	Vector3 m_CamForward;
	Vector3 m_Move;

	Vector3 m_GroundNormal = Vector3.up;
	float m_TurnAmount;
	float m_ForwardAmount;

	void LateUpdate()
	{

		m_Move = Vector3.zero;

		if (Moveable) {
			// read inputs
			float h = CrossPlatformInputManager.GetAxis ("Horizontal");
			float v = CrossPlatformInputManager.GetAxis ("Vertical");
//		bool crouch = Input.GetKey(KeyCode.C);

			// calculate move direction to pass to character
			if (m_CamTransform != null) {
				// calculate camera relative direction to move:
				m_CamForward = Vector3.Scale (m_CamTransform.forward, new Vector3 (1, 0, 1)).normalized;
				m_Move = v * m_CamForward + h * m_CamTransform.right;
			} else {
				// we use world-relative directions in the case of no main camera
				m_Move = v * Vector3.forward + h * Vector3.right;
			}
		}
		#if !MOBILE_INPUT
		// walk speed multiplier
//		if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
		#endif


		Move (m_Move);

		// set the animator 
		m_animator.SetBool ("isMoving", m_IsMoving);

		// update the sensity
		if (MechanismManager.Instance.DamageState == MechanismManager.DamageStateType.UnderDamage) {
			CameraMechanismSensity = (MechanismManager.health.HealthRateZeroOne * 0.2f + 0.8f);
		}
		else
			CameraMechanismSensity = 1f;

		cameraBot.InputSetting.Sensitive = CameraSensity * CameraNarrativeSensity * CameraMechanismSensity;
	}

	void Move( Vector3 move )
	{
		// convert the world relative moveInput vector into a local-relative
		// turn amount and forward amount required to head in the desired
		// direction.
		if (move.magnitude > 1f) move.Normalize();
		move = transform.InverseTransformDirection(move);

		move = Vector3.ProjectOnPlane(move, m_GroundNormal);
		m_TurnAmount = Mathf.Atan2(move.x, move.z);
		m_ForwardAmount = move.z;

		ApplyExtraTurnRotation();

		HandleGroundedMovement( move );
	}

	void HandleGroundedMovement( Vector3 move )
	{
		// check whether conditions are right to allow a jump:
		m_Rigidbody.velocity = transform.forward * move.magnitude * Mathf.Lerp( MoveSpeed , RunSpeed , MechanismManager.health.SpeedUp ) ;
//		m_CharacterController.SimpleMove( transform.forward * move.magnitude * m_MoveSpeedMultiplier );
	}

	void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(StationaryTurnSpeed, MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
	}

	public void Kick( Vector3 direction)
	{
		StartCoroutine (BeKicked (direction));
	}

	IEnumerator BeKicked(Vector3 direction)
	{
		Moveable = false;
		m_Rigidbody.velocity = direction * m_MoveSpeedMultiplier;
		yield return new WaitForSeconds (0.5f);

		Moveable = true;
	}

	public static Camera MainCameara
	{
		get { return Instance.m_MainCamera; }
	}
}
