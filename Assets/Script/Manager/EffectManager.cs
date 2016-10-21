using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class EffectManager : MBehavior {

	[SerializeField] GameObject deathPrefab;
	[SerializeField] CameraFilterPack_AAA_WaterDropPro waterDropEffect;
	[SerializeField] BlurOptimized BlurEffect;
	[Range(0,1f)]
	[SerializeField] float damgeAffectThreshod = 0.1f;
	// Use this for initialization
	protected override void MStart ()
	{
		base.MStart ();
	}
	
	// Update is called once per frame
	protected override void MUpdate ()
	{
		if (Input.GetKeyDown (KeyCode.C) && Input.GetKey (KeyCode.LeftControl))
			M_Event.FireLogicEvent (LogicEvents.Death, new LogicArg (this));
		
		OnUpdateDamageEffect ();
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.Death] += OnDeath;
		M_Event.logicEvents [(int)LogicEvents.BeginDamage] += OnBeginDamage;
		M_Event.logicEvents [(int)LogicEvents.EndDamage] += OnEndDamge;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.Death] -= OnDeath;
		M_Event.logicEvents [(int)LogicEvents.BeginDamage] -= OnBeginDamage;
		M_Event.logicEvents [(int)LogicEvents.EndDamage] -= OnEndDamge;
	}

	void OnDeath(LogicArg arg )
	{
		if (deathPrefab != null) {
			GameObject deathObj = Instantiate (deathPrefab) as GameObject;
			Death death = deathObj.GetComponent<Death> ();
			death.InitDeath (LogicManager.MainCharacter.transform.position);

			if ( BlurEffect != null )
				BlurEffect.enabled = false;
			if ( waterDropEffect != null )
				waterDropEffect.enabled = false;
		}
	}

	void OnBeginDamage(LogicArg arg)
	{
		if ( waterDropEffect != null )
			waterDropEffect.enabled = true;
	}

	void OnEndDamge(LogicArg arg)
	{
		if ( waterDropEffect != null )
		waterDropEffect.enabled = false;
	}

	void OnUpdateDamageEffect()
	{
		if (waterDropEffect != null) {
//			if (MechanismManager.health.LostHealthRate < damgeAffectThreshod)
//				waterDropEffect.enabled = false;
//			else
			{
//				waterDropEffect.enabled = true;
				waterDropEffect.rate = Mathf.Sqrt( (MechanismManager.health.LostHealthRate - damgeAffectThreshod) / ( 1f - damgeAffectThreshod) + 0.5f ) ;
			}
		}
		if (BlurEffect != null) {
			if (MechanismManager.health.LostHealthRate < damgeAffectThreshod )
				BlurEffect.enabled = false;
			else if ( !(MechanismManager.Instance.DamageState == MechanismManager.DamageStateType.Dead) )
			{
				BlurEffect.enabled = true;
				BlurEffect.rate = (MechanismManager.health.LostHealthRate - damgeAffectThreshod) / ( 1f - damgeAffectThreshod ) ;
			}
		}
	}

}
