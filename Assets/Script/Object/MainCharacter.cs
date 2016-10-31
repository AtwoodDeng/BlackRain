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

	[SerializeField] Transform m_Cam;                  // A reference to the main camera in the scenes transform

	Rigidbody m_Rigidbody;
	CharacterController m_CharacterController;

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
		get { return m_MoveSpeedMultiplier * MechanismManager.health.HealthRate; }
	}
	[SerializeField] float m_RunSpeedMultiplier = 3f;
	public float RunSpeed
	{
		get { return m_RunSpeedMultiplier * MechanismManager.health.HealthRate; }
	}
	[SerializeField] CF.CameraBot.CameraBot cameraBot;
	private float oriCamBotSensity;
	SavePointData lastSavePoint;

	private float CameraSensity;

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


	[SerializeField] Animator m_animator;

	void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
		m_CharacterController = GetComponent<CharacterController> ();

		if (m_Cam == null && Camera.main != null)
		{
			m_Cam = Camera.main.transform;
		}
		CameraSensity = cameraBot.InputSetting.Sensitive;

		if ( m_animator != null )
			m_animator = GetComponentInChildren<Animator> ();

		oriCamBotSensity = cameraBot.InputSetting.BasicSensity;
	}

	void OnEnable()
	{
		M_Event.logicEvents [(int)LogicEvents.Death] += OnDeath;
		M_Event.logicEvents [(int)LogicEvents.DisplayDialog] += OnDisplay;
		M_Event.logicEvents [(int)LogicEvents.EndDisplayDialog] += OnEndDisplay;
		M_Event.logicEvents [(int)LogicEvents.EnterSavePoint] += OnEnterSavePoint;
		M_Event.logicEvents [(int)LogicEvents.LockCamera] += OnLockCamera;
		M_Event.logicEvents [(int)LogicEvents.UnlockCamera] += OnUnlockCamer;
	}

	void OnDisable()
	{
		M_Event.logicEvents [(int)LogicEvents.Death] -= OnDeath;
		M_Event.logicEvents [(int)LogicEvents.DisplayDialog] -= OnDisplay;
		M_Event.logicEvents [(int)LogicEvents.EndDisplayDialog] -= OnEndDisplay;
		M_Event.logicEvents [(int)LogicEvents.EnterSavePoint] -= OnEnterSavePoint;
		M_Event.logicEvents [(int)LogicEvents.LockCamera] -= OnLockCamera;
		M_Event.logicEvents [(int)LogicEvents.UnlockCamera] -= OnUnlockCamer;
	}

	void OnEnterSavePoint(LogicArg arg)
	{
		SavePointData data = (SavePointData)arg.GetMessage (M_Event.EVENT_SAVE_POINT);
		lastSavePoint = data;
	}

	void OnLockCamera( LogicArg arg )
	{
		cameraBot.InputSetting.BasicSensity = 0;
	}

	void OnUnlockCamer( LogicArg arg )
	{
		cameraBot.InputSetting.BasicSensity = oriCamBotSensity;
	}


	void OnDisplay( LogicArg arg )
	{
		NarrativePlotScriptableObject plot = (NarrativePlotScriptableObject)arg.GetMessage (M_Event.EVENT_DISPLAY_DIALOG_PLOT);
		if (plot != null && plot.lockCamera) {
			Moveable = false;
			cameraBot.InputSetting.NarrativeSensityMutiple = 0.03f;
		}
//		m_UseHeadBob = false;
//		m_Move = false;
//		m_CanJump = false;
	}

	void OnEndDisplay( LogicArg arg )
	{
		Moveable = true;
		cameraBot.InputSetting.NarrativeSensityMutiple = 1f;
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
		m_Cam.DOMoveY (deathSetting.deathFloatHeight , deathSetting.deathFloatTime).SetRelative (true).OnComplete(OnDeathEnd);
		m_Cam.DORotate (new Vector3 (75f, 0, 0), deathSetting.deathFloatTime / 3f ).SetEase(Ease.OutCubic);

	}

	void OnDeathEnd()
	{
		m_isOnDeath = false;
		Moveable = true;
		transform.DOKill ();
		transform.position = lastSavePoint.trans.position;
		transform.forward = lastSavePoint.trans.forward;

		cameraBot.enabled = true;
		cameraBot.InputSetting.DamageSensityMutiple = 1f;
		m_Cam.DOKill ();

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
	}


	Vector3 m_CamForward;
	Vector3 m_Move;

	Vector3 m_GroundNormal = Vector3.up;
	float m_TurnAmount;
	float m_ForwardAmount;

	void LateUpdate()
	{

		// read inputs
		float h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");
//		bool crouch = Input.GetKey(KeyCode.C);

		// calculate move direction to pass to character
		if (m_Cam != null)
		{
			// calculate camera relative direction to move:
			m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
			 m_Move = v * m_CamForward + h * m_Cam.right;
		}
		else
		{
			// we use world-relative directions in the case of no main camera
			m_Move = v*Vector3.forward + h*Vector3.right;
		}
			
		#if !MOBILE_INPUT
		// walk speed multiplier
//		if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
		#endif


		// pass all parameters to the character control script
		if ( Moveable )
			Move( m_Move );


		// set the animator 
		if (Moveable) {
			m_animator.SetBool ("isMoving", m_IsMoving );
		}

		// update the sensity
		if ( MechanismManager.Instance.DamageState == MechanismManager.DamageStateType.UnderDamage )
			cameraBot.InputSetting.DamageSensityMutiple = CameraSensity * ( MechanismManager.health.HealthRate * 0.7f + 0.3f );
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
}
