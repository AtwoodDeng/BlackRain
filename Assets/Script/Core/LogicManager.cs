using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LogicManager : MBehavior {

	static LogicManager m_Instance;
	public static LogicManager Instance{ get { return m_Instance;}}

	static MainCharacter m_MainCharacter;
	public static MainCharacter MainCharacter{ get { return m_MainCharacter; } }

	public enum GameLanguage
	{
		English,
		Chinese,
	}
	[SerializeField] GameLanguage m_language;
	static public GameLanguage Language{
		get { return Instance.m_language; }
	}

	// Use this for initialization
	protected override void MAwake ()
	{
		if (m_Instance == null)
			m_Instance = this;
		else
			Destroy (this);

		m_MainCharacter = FindObjectOfType<MainCharacter> ();

		Cursor.visible = true;
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		// TODO: flexible input
		if (Input.GetKeyDown (KeyCode.E)) {
			M_Event.FireLogicEvent (LogicEvents.Interact, new LogicArg (this));
		}
	}
}
