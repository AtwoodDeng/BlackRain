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
		}

		private float energy;
		public const float FullEnergy = 1f;
		private bool isSpeedUp;
		private float minHealth = 0;


		public float HealthValue{ get { return health; } }

		public float HealthRate{ get{ return health / FullHealth;}}
		public float HealthRateZeroOne{ get { return (health - minHealth) / (FullHealth - minHealth); } }
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
			energy -= energyLostRate * Time.deltaTime;
			isSpeedUp = true;
			if (energy < 0) {
				isSpeedUp = false;
				energy = 0;
			}
		}

		public void RecoverEnergy( float recoverRate )
		{
			if (energy < 1f) {
				energy += recoverRate * Time.deltaTime;
			}
			isSpeedUp = false;
		}

		public void SetMinHealth( float min )
		{
			minHealth = min;
		}
	};

	Health m_health = new Health();
	static public Health health { get { return Instance.m_health;}}

	protected override void MAwake ()
	{
		base.MAwake ();
		m_health.SetMinHealth (damageSetting.minHealth);
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

}
