using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using DG.Tweening;

public class MechanismManager : MBehavior {
	static MechanismManager m_Instance;
	static public MechanismManager Instance
	{
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<MechanismManager> ();
			return m_Instance; }
	}
	[System.Serializable]
	public struct DamageSetting
	{
		/// <summary>
		/// The layer mask of the protection object
		/// </summary>
		[TooltipAttribute("the mask of the protection objects")]
		public LayerMask mask;
	
		[TooltipAttribute("The total time of the character can suvive in the rain.")]
		public float lastTime;
		/// <summary>
		/// The recover rate of the character per second
		/// </summary>
		[TooltipAttribute("The precentage of the recover per second.")]
		[RangeAttribute(0,1f)]
		public float recover;

		[TooltipAttribute("The precentage of the energy cost pre second.")]
		public float energyCost;

		[TooltipAttribute("The precentage of the energy recovered pre second.")]
		public float energyRecover;

		[TooltipAttribute("The precentage of the minHealth of the character.")]
		[RangeAttribute(0,1f)]
		public float minHealth;

		[TooltipAttribute("The duration between each sneeze")]
		public float sneezeDuration;
	}
	[SerializeField] DamageSetting damageSetting;

	public enum DamageStateType
	{
		None,
		UnderDamage,
		UnderProtect,
		Dead,
	}
	DamageStateType m_damageState = DamageStateType.UnderProtect;
	public DamageStateType DamageState
	{
		get { return m_damageState; }
		set {
			if (m_damageState != value) {
				if (value == DamageStateType.UnderDamage) {
					M_Event.FireLogicEvent (LogicEvents.BeginDamage, new LogicArg (this));
				} else if (value == DamageStateType.UnderProtect) {
					M_Event.FireLogicEvent (LogicEvents.EndDamage, new LogicArg (this));
				}
				m_damageState = value;
			}
		}
	}

	public class Health
	{
		float health;
		public const float FullHealth = 1f;
		public Health(){
			health = FullHealth;
			energy = FullEnergy;
			sneezeNum = 0;
		}

		private float energy;
		public const float FullEnergy = 1f;
		private bool isSpeedUp;
		private float minHealth = 0;
		private float sneezeDuration = 10f;
		public float inRainTimer;
		private int sneezeNum;


		/// <summary>
		/// Gets the health value.
		/// </summary>
		/// <value>The health value.</value>
		public float HealthValue{ get { return health; } }

		/// <summary>
		/// health / Full health
		/// </summary>
		/// <value>The health rate.</value>
		public float HealthRate{ get{ return health / FullHealth;}}
		/// <summary>
		/// Convert the health rate to (0 - 1f)
		/// </summary>
		/// <value>The health rate zero one.</value>
		public float HealthRateZeroOne{ get { return (health - minHealth) / (FullHealth - minHealth); } }
		/// <summary>
		/// 1 - healthRate
		/// </summary>
		/// <value>The lost health rate.</value>
		public float LostHealthRate { get { return 1f - HealthRate; } }

		public float SpeedUp { get { return isSpeedUp ? ( energy > 0 ? 1f : 0 ) : 0; } }// Mathf.Sqrt(( Mathf.Max( 0 , energy) / FullEnergy)) : 0 ; } }
		public float Energy { get { return energy; } }
		public float EnergyRate { get { return energy / FullEnergy; } }

		public bool IsMinHealth{ get { return health < FullHealth * minHealth; } }

		public void SetHealthToMin()
		{
			DOTween.To (() => health, (x) => health = x, minHealth, 0.5f);
		}

		public void OnDamage( float dmg )
		{
			if ( health > minHealth * FullHealth )
				health -= dmg * Time.deltaTime;
		}

		public void OnUpdateSneeze()
		{
			if ( !NarrativeManager.Instance.IsDisplaying && !MainCharacter.Instance.IsFocus )
				inRainTimer += Time.deltaTime;
			
			if (inRainTimer > (sneezeNum + 1) * sneezeDuration) {
				sneezeNum++;
				M_Event.FireLogicEvent (LogicEvents.Sneeze, new LogicArg (this));
				Debug.Log ("Sneeze" + sneezeNum);
			}
		}

		public void OnRecover( float recoverRate )
		{
			if ( health < FullHealth )
				health += (FullHealth - health) * recoverRate * Time.deltaTime;
		}

		public void Revive()
		{
			health = FullHealth;
		}
		public void OnSpeedUp( float energyLostRate )
		{
			if (energy > 0) {
				energy -= energyLostRate * Time.deltaTime;
				isSpeedUp = true;
				if (energy < 0) {
					isSpeedUp = false;
					energy = 0;
					M_Event.FireLogicEvent (LogicEvents.Breath, new LogicArg (this));
				}
			}
		}

		public void RecoverEnergy( float recoverRate )
		{
			if (energy < 1f) {
				energy += recoverRate * Time.deltaTime;
			}
			isSpeedUp = false;
		}

		public void InitHealth( float _minHealth , float _sneezeDuration )
		{
			minHealth = _minHealth;
			sneezeDuration = _sneezeDuration;
		}
	};

	Health m_health = new Health();
	static public Health health { get { return Instance.m_health;}}

	protected override void MAwake ()
	{
		base.MAwake ();
		m_health = new Health ();
		m_health.InitHealth (damageSetting.minHealth , damageSetting.sneezeDuration);
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

	protected override void MUpdate ()
	{
		base.MUpdate ();
		UpdateDamageState ();
		UpdateHealth ();
		UpdateEnergy ();
		UpdateSneeze ();
	}

	void UpdateSneeze()
	{
		if (DamageState == DamageStateType.UnderDamage)
			m_health.OnUpdateSneeze ();	
	}

	void UpdateEnergy()
	{
		if ( CrossPlatformInputManager.GetButton("SpeedUp") && MainCharacter.Instance.m_IsMoving ) {
			m_health.OnSpeedUp (damageSetting.energyCost);
		} else {
			m_health.RecoverEnergy (damageSetting.energyRecover);
		}

		if ( CrossPlatformInputManager.GetButtonDown("SpeedUp")) {
			M_Event.FireLogicEvent (LogicEvents.BeginRun, new LogicArg (this));
		}

		if ( CrossPlatformInputManager.GetButtonUp("SpeedUp")) {
			M_Event.FireLogicEvent (LogicEvents.EndRun, new LogicArg (this));
		}
	}

	void OnDeathEnd( LogicArg arg )
	{
		DamageState = DamageStateType.UnderProtect;
		m_health.Revive ();
	}

	void UpdateHealth()
	{
		if (DamageState == DamageStateType.UnderDamage) {	
			m_health.OnDamage (Health.FullHealth / damageSetting.lastTime);
		} else if ( DamageState == DamageStateType.UnderProtect) {
			m_health.OnRecover (damageSetting.recover);
		}
		if (m_health.HealthValue < 0 && DamageState == DamageStateType.UnderDamage) {
			DamageState = DamageStateType.Dead;
			M_Event.FireLogicEvent (LogicEvents.Death, new LogicArg (this));
		}
	}

	void UpdateDamageState()
	{
		if (DamageState != DamageStateType.Dead) {
			if (CheckUnderObject ()) {
				DamageState = DamageStateType.UnderProtect;
			} else {
				DamageState = DamageStateType.UnderDamage;
			}
		}
	}

	bool CheckUnderObject()
	{ 
		MainCharacter character = MainCharacter.Instance;

		RaycastHit hitInfo;
		if (Physics.Raycast (character.transform.position, Vector3.up, out hitInfo, 100f, damageSetting.mask.value)) {
			return true;
		}
		return false;
	}


	void OnGUI(){
		GUILayout.Label ("Sneeze Timer" + m_health.inRainTimer);
	}

}
