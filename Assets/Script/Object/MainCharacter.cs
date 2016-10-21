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

	private static MainCharacter m_Instance;
	public static MainCharacter Instance
	{
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<MainCharacter> ();
			return m_Instance;
		}
	}

	void Awake()
	{
	}
	void OnEnable()
	{
		M_Event.logicEvents [(int)LogicEvents.Death] += OnDeath;
		M_Event.logicEvents [(int)LogicEvents.DisplayDialog] += OnDisplay;
		M_Event.logicEvents [(int)LogicEvents.EndDisplayDialog] += OnEndDisplay;
	}

	void OnDisable()
	{
		M_Event.logicEvents [(int)LogicEvents.Death] -= OnDeath;
		M_Event.logicEvents [(int)LogicEvents.DisplayDialog] -= OnDisplay;
		M_Event.logicEvents [(int)LogicEvents.EndDisplayDialog] -= OnEndDisplay;
	}
	void OnDisplay( LogicArg arg )
	{
		m_UseHeadBob = false;
		m_Move = false;
		m_CanJump = false;
	}

	void OnEndDisplay( LogicArg arg )
	{
		m_UseHeadBob = true;
		m_Move = true;
		m_CanJump = true;
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
