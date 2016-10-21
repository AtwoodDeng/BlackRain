using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class EffectManager : MBehavior {

	[SerializeField] GameObject deathPrefab;
	[SerializeField] GlitchEffect glitchEffect;
	[SerializeField] BlurOptimized BlurEffect;
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
			if ( glitchEffect != null )
			glitchEffect.enabled = false;
		}
	}

	void OnBeginDamage(LogicArg arg)
	{
		if (glitchEffect != null)
			glitchEffect.enabled = true;
	}

	void OnEndDamge(LogicArg arg)
	{
		if (glitchEffect != null) {
			glitchEffect.enabled = false;
		}
	}

	void OnUpdateDamageEffect()
	{
		if (glitchEffect != null)
			glitchEffect.intensity = MechanismManager.health.LostHealthRate * 2f + 0.5f;
		if (BlurEffect != null)
			BlurEffect.rate = ( MechanismManager.health.LostHealthRate > 0.1f)? ( MechanismManager.health.LostHealthRate - 0.1f ) / 0.9f : 0 ;
	}

}
