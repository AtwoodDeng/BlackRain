using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class EffectManager : MBehavior {

	[SerializeField] GameObject deathPrefab;
	[SerializeField] CameraFilterPack_AAA_WaterDropPro waterDropEffect;
	[SerializeField] BlurOptimized BlurEffect;
	[SerializeField] CameraFilterPack_Color_BrightContrastSaturation saturationEffect;
	[SerializeField] CameraFilterPack_Colors_Adjust_PreFilters photoShopEffect;
	[SerializeField] CameraFilterPack_Blend2Camera_Blend blendToOld;
	[SerializeField] CameraFilterPack_Blend2Camera_Blend blendToDark;
	[Range(0,1f)]
	[SerializeField] float damgeAffectThreshod = 0.1f;
	[Range(0,1f)]
	[SerializeField] float illusionAffectThreshod = 0.3f;
	[SerializeField] float ToOldDuration = 2f;
	[SerializeField] Camera oldCamera;
	[SerializeField] Camera darkCamera;
	[SerializeField] Camera effectCamera;
	// Use this for initialization

	protected override void MAwake ()
	{
		base.MAwake ();
		if (waterDropEffect == null)
			waterDropEffect = Camera.main.GetComponentInChildren<CameraFilterPack_AAA_WaterDropPro> ();
		if (BlurEffect == null)
			BlurEffect = Camera.main.GetComponentInChildren<BlurOptimized> ();
		if (saturationEffect == null)
			saturationEffect = Camera.main.GetComponentInChildren<CameraFilterPack_Color_BrightContrastSaturation> ();
		if (photoShopEffect == null) {
			photoShopEffect = Camera.main.GetComponentInChildren<CameraFilterPack_Colors_Adjust_PreFilters> ();
			photoShopEffect.enabled = false;
		}
		if (blendToOld == null) {
			CameraFilterPack_Blend2Camera_Blend[] coms = Camera.main.GetComponentsInChildren<CameraFilterPack_Blend2Camera_Blend> ();
			foreach (CameraFilterPack_Blend2Camera_Blend com in coms)
				if (com.Camera2.name.StartsWith ("Old"))
					blendToOld = com;
			blendToOld.enabled = false;
		}
		if (blendToDark == null) {
			CameraFilterPack_Blend2Camera_Blend[] coms = Camera.main.GetComponentsInChildren<CameraFilterPack_Blend2Camera_Blend> ();
			foreach (CameraFilterPack_Blend2Camera_Blend com in coms)
				if (com.Camera2.name.StartsWith ("Dark"))
					blendToDark = com;
			blendToDark.enabled = false;
		}
		if (darkCamera == null) {
			Camera[] cameras = Camera.main.GetComponentsInChildren<Camera> ();
			foreach (Camera cam in cameras)
				if (cam.name.StartsWith ("Dark"))
					darkCamera = cam;
			darkCamera.enabled = false;
		}
		if (oldCamera == null) {
			Camera[] cameras = Camera.main.GetComponentsInChildren<Camera> ();
			foreach (Camera cam in cameras)
				if (cam.name.StartsWith ("Old"))
					oldCamera = cam;
			oldCamera.enabled = false;
		}
		if (effectCamera == null) {
			Camera[] cameras = Camera.main.GetComponentsInChildren<Camera> ();
			foreach (Camera cam in cameras)
				if (cam.name.StartsWith ("Effect"))
					effectCamera = cam;
			effectCamera.enabled = true;
		}
	}

	protected override void MStart ()
	{
		base.MStart ();

		LogicManager.Instance.RegisterStateChange (OnStateChange);
	}

	void OnStateChange( LogicManager.GameState fromState , LogicManager.GameState toState )
	{
//		Debug.Log("To state effect" + toState);
//		if ( toState == LogicManager.GameState.WalkInStreetColorful )
//		{
//			saturationEffect.enabled = true;
//			saturationEffect.Brightness = 1f;
//			saturationEffect.Contrast = 1f;
//			DOTween.To (() => saturationEffect.Saturation, (x) => saturationEffect.Saturation = x, 2f, 30f);
//		}

		Debug.Log("To state effect" + toState);
		if (toState == LogicManager.GameState.BeginShip) {
			photoShopEffect.enabled = true;
			DOTween.To (() => photoShopEffect.FadeFX, (x) => photoShopEffect.FadeFX = x, 0.3f, 15f);
		}
	}
	
	// Update is called once per frame
	protected override void MUpdate ()
	{
//		if (Input.GetKeyDown (KeyCode.C) && Input.GetKey (KeyCode.LeftControl))
//			M_Event.FireLogicEvent (LogicEvents.Death, new LogicArg (this));
		
		OnUpdateDamageEffect ();

		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.W)) {
			photoShopEffect.enabled = true;
			photoShopEffect.FadeFX = 0.3f;
		}
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.Death] += OnDeath;
		M_Event.logicEvents [(int)LogicEvents.BeginDamage] += OnBeginDamage;
		M_Event.logicEvents [(int)LogicEvents.EndDamage] += OnEndDamge;
		M_Event.logicEvents [(int)LogicEvents.DisPlayClimaxEffect] += OnClimax;
		M_Event.logicEvents [(int)LogicEvents.FocusCamera] += OnFocusCamera;
		M_Event.logicEvents [(int)LogicEvents.UnfocusCamera] += OnUnfocusCamera;
		M_Event.RegisterEvent (LogicEvents.ToOld, OnToOld);
		M_Event.RegisterEvent (LogicEvents.ToModern, OnToModern);
		M_Event.RegisterEvent (LogicEvents.ToDark, OnToDark);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.Death] -= OnDeath;
		M_Event.logicEvents [(int)LogicEvents.BeginDamage] -= OnBeginDamage;
		M_Event.logicEvents [(int)LogicEvents.EndDamage] -= OnEndDamge;
		M_Event.logicEvents [(int)LogicEvents.DisPlayClimaxEffect] -= OnClimax;
		M_Event.logicEvents [(int)LogicEvents.FocusCamera] -= OnFocusCamera;
		M_Event.logicEvents [(int)LogicEvents.UnfocusCamera] -= OnUnfocusCamera;
		M_Event.UnregisterEvent (LogicEvents.ToOld, OnToOld);
		M_Event.UnregisterEvent (LogicEvents.ToModern, OnToModern);
		M_Event.UnregisterEvent (LogicEvents.ToDark, OnToDark);
	}

	bool isCameraLocked = false;


	void OnToDark( LogicArg arg )
	{
		float delay = 0;
		float duration = ToOldDuration;
		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DELAY ) )
			delay = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DELAY);

		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DURATION ) )
			duration = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DURATION);

		blendToDark.enabled = true;
		darkCamera.enabled = true;
		if ( blendToDark != null )
			DOTween.To ( (x) => blendToDark.BlendFX = x, 0 , 1f, duration).SetDelay(delay);
	}

	void OnToOld( LogicArg arg )
	{
		float delay = 0;
		float duration = ToOldDuration;
		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DELAY ) )
			delay = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DELAY);

		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DURATION ) )
			duration = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DURATION);

		blendToOld.enabled = true;
		oldCamera.enabled = true;
		if ( blendToDark != null )
			DOTween.To ( (x) => blendToOld.BlendFX = x, 0 , 1f, duration).SetDelay(delay);
	}

	void OnToModern( LogicArg arg )
	{
		float delay = 0;
		float duration = ToOldDuration;
		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DELAY ) )
			delay = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DELAY);

		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DURATION ) )
			duration = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DURATION);
		if (oldCamera.enabled) {
			DOTween.To ((x) => blendToOld.BlendFX = x, 1f, 0f, duration).SetDelay (delay).OnComplete (delegate() {
				blendToOld.enabled = false;
				oldCamera.enabled = false;
				blendToDark.enabled = false;
				darkCamera.enabled = false;

			});
		}
		if (darkCamera.enabled) {
			DOTween.To ((x) => blendToDark.BlendFX = x, 1f , 0f, duration).SetDelay (delay).OnComplete (delegate() {
				blendToOld.enabled = false;
				oldCamera.enabled = false;
				blendToDark.enabled = false;
				darkCamera.enabled = false;

			});
		}
	}

	void OnFocusCamera( LogicArg arg )
	{
		Debug.Log ("On Lock Camera");
		isCameraLocked = true;

		if (waterDropEffect != null) {
			waterDropEffect.enabled = false;
		}
	}

	void OnUnfocusCamera( LogicArg arg )
	{
		Debug.Log ("On UnLock Camera");
		isCameraLocked = false;
		if (waterDropEffect != null && IsInDamage) {
			waterDropEffect.enabled = true;
		}
	}

	void OnClimax( LogicArg arg )
	{
		Debug.Log ("Effect Manager On Climax");
		photoShopEffect.enabled = true;
		DOTween.To (() => photoShopEffect.FadeFX, (x) => photoShopEffect.FadeFX = x, 0.3f, 20f);
	}


	void OnDeath(LogicArg arg )
	{
		// TODO : remake the on death

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

	bool IsInDamage = false;
	void OnBeginDamage(LogicArg arg)
	{
		if (waterDropEffect != null ) {
//			waterDropEffect.enabled = true;
			DOTween.To( () => waterDropEffect.rate , (x) => waterDropEffect.rate = x , 1f , 0.4f );
		}

		
	}

	void OnEndDamge(LogicArg arg)
	{
		if (waterDropEffect != null) {
			DOTween.To (() => waterDropEffect.rate, (x) => waterDropEffect.rate = x, 0f, 0.4f).OnComplete (delegate() {
				waterDropEffect.enabled = false;
			});
		}
		
	}

	void OnUpdateDamageEffect()
	{
//		if (waterDropEffect != null) {
////			if (MechanismManager.health.LostHealthRate < damgeAffectThreshod)
////				waterDropEffect.enabled = false;
////			else
//			{
////				waterDropEffect.enabled = true;
////				waterDropEffect.rate = Mathf.Sqrt( (MechanismManager.health.LostHealthRate - damgeAffectThreshod) / ( 1f - damgeAffectThreshod) + 0.5f ) ;
//			}
//
//		}
		if (waterDropEffect != null) {
			if (isCameraLocked || NarrativeManager.Instance.IsDisplaying) {
//				Debug.Log ("Water Drop false");
				waterDropEffect.enabled = false;
			}else
			{
				if (MechanismManager.Instance.DamageState == MechanismManager.DamageStateType.UnderDamage) {
					waterDropEffect.enabled = true;
				} else if ( MechanismManager.Instance.DamageState == MechanismManager.DamageStateType.UnderProtect) {
					if (waterDropEffect.rate == 0)
						waterDropEffect.enabled = false;
				}
			}
		}

		if (BlurEffect != null) {
			if (isCameraLocked || NarrativeManager.Instance.IsDisplaying ) {
				BlurEffect.enabled = false;
			}else if (MechanismManager.health.LostHealthRate < damgeAffectThreshod )
				BlurEffect.enabled = false;
			else if ( !(MechanismManager.Instance.DamageState == MechanismManager.DamageStateType.Dead) )
			{
				BlurEffect.enabled = true;
				BlurEffect.rate = (MechanismManager.health.LostHealthRate - damgeAffectThreshod) / ( 1f - damgeAffectThreshod ) ;
			}
		}
	}


}
