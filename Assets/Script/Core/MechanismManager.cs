using UnityEngine;
using System.Collections;

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
		}

		public float HealthValue{ get { return health; } }

		public float HealthRate{ get{ return health / FullHealth;}}
		public float LostHealthRate { get { return 1f - HealthRate; } }

		public void OnDamage( float dmg )
		{
			health -= dmg * Time.deltaTime;
		}

		public void OnRecover( float recoverRate )
		{
			health += (FullHealth - health) * recoverRate * Time.deltaTime;
		}
	};

	Health m_health = new Health();
	static public Health health { get { Instance.m_health }}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		UpdateDamageState ();
		UpdateHealth ();
	}

	void UpdateHealth()
	{
		if (DamageState == DamageStateType.UnderDamage) {	
			m_health.OnDamage (Health.FullHealth / damageSetting.lastTime);
		} else if ( DamageState == DamageStateType.UnderProtect) {
			m_health.OnRecover (damageSetting.recover);
		}
		if (HealthValue < 0 && DamageState == DamageStateType.UnderDamage) {
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
