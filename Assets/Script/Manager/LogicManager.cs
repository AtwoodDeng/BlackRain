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



	public enum GameState
	{
		None=0,
		Enter=1,
		BuyCoffee=2,

		TryGoHome=3,
		GoWithGoodMan=4,
		AskForUmbrella = 5,

		End = 99,

	}

	private AStateMachine<GameState,LogicEvents> m_stateMachine;
	public GameState State
	{
		get { return m_stateMachine.State; }
	}

	public void RegisterStateChange(AStateMachine<GameState,LogicEvents>.StateChangeHandler func)
	{
		m_stateMachine.AddOnChange (func);
	}

	// Use this for initialization
	protected override void MAwake ()
	{
		if (m_Instance == null)
			m_Instance = this;
		else
			Destroy (this);

		m_MainCharacter = FindObjectOfType<MainCharacter> ();


		InitStateMachine ();


	}

	protected override void MStart ()
	{
		base.MStart ();

		M_Event.FireLogicEvent (LogicEvents.LockCamera, new LogicArg (this));
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		// TODO: flexible input
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(1) ) {
			M_Event.FireLogicEvent (LogicEvents.Interact, new LogicArg (this));
		}

		if (Input.GetKeyDown (KeyCode.E) || Input.GetMouseButtonDown(1) ) {
			M_Event.FireLogicEvent (LogicEvents.UnlockCamera, new LogicArg (this));
		}

		if (Input.GetKeyUp (KeyCode.E)  || Input.GetMouseButtonUp(1)  ) {
			M_Event.FireLogicEvent (LogicEvents.LockCamera, new LogicArg (this));
		}

		if (Input.GetKeyDown (KeyCode.Q) && Input.GetKey(KeyCode.LeftControl) ) {
			m_stateMachine.State = GameState.End;
			M_Event.FireLogicEvent (LogicEvents.EndGame, new LogicArg (this));
		}

		m_stateMachine.Update ();
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		for (int i = 0; i < M_Event.logicEvents.Length; ++i)
			M_Event.logicEvents [i] += OnEvent;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		for (int i = 0; i < M_Event.logicEvents.Length; ++i)
			M_Event.logicEvents [i] -= OnEvent;
	}

	void OnEvent(LogicArg arg)
	{
		m_stateMachine.OnEvent (arg.type);
	}

	void InitStateMachine()
	{
		m_stateMachine = new AStateMachine<GameState, LogicEvents> ();

		m_stateMachine.BlindTimeStateChange (GameState.Enter, GameState.BuyCoffee, 1f);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.EndTalkWaiter, GameState.BuyCoffee, GameState.TryGoHome);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.DeathEnd, GameState.TryGoHome, GameState.GoWithGoodMan);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.GoodManLeave, GameState.GoWithGoodMan, GameState.AskForUmbrella);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.EndGame, GameState.AskForUmbrella, GameState.End);

			
		m_stateMachine.State = GameState.Enter;
	}


	
}

[System.Serializable]
public struct MinMax
{
	public float min;
	public float max;

	public float RandomBetween{
		get {
			return Random.Range (min, max);
			}
	}
}