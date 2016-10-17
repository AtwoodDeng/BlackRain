using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using DG.Tweening;

public class MainCharacter : FirstPersonController {

	[System.Serializable]
	public struct DeathSetting
	{
		public float deathFloatHeight;
		public float deathFloatTime;
		public GameObject walk;
		public GameObject death;
	};
	[SerializeField]  DeathSetting deathSetting;

	void Awake()
	{
	}
	void OnEnable()
	{
		M_Event.logicEvents [(int)LogicEvents.Death] += OnDeath;
	}

	void OnDisable()
	{
		M_Event.logicEvents [(int)LogicEvents.Death] -= OnDeath;
	}

	void OnDeath( LogicArg arg )
	{
		m_CharacterController.enabled = false;
		m_MouseLook.enable = false;
		m_FovKick.enable = false;
		m_UseHeadBob = false;
		m_Camera.transform.DOMoveY (deathSetting.deathFloatHeight , deathSetting.deathFloatTime).SetRelative (true);
		m_Camera.transform.DORotate (new Vector3 (90f, 0, 0), deathSetting.deathFloatTime / 3f ).SetEase(Ease.OutCubic);

		deathSetting.walk.SetActive (false);
		deathSetting.death.SetActive (true);
	}

}
