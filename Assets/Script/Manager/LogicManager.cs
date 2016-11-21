using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LogicManager : MBehavior {

	static LogicManager m_Instance;
	public static LogicManager Instance{ 
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<LogicManager> ();
			return m_Instance;}}

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
		/// <summary>
		/// Level One
		/// </summary>
		Enter = 1,
		TalkWithManInCafe = 2,
		TryGoInRain = 3,
		TalkWithGirlInCafe = 4,
		WalkInStreetOne = 5,
		SeeBuilding = 6,
		BorrowUmbrella = 7,

		/// <summary>
		/// Level Two
		/// </summary>
		WalkInStreetTwo = 10,
		SeeTakePhoto = 11,
		SeeGirlStreetTwo = 12,
		FindGirlStreetTwo = 13,

		/// <summary>
		/// Level Three
		/// </summary>
		WalkInStreetThree = 20,
		WalkOutStreetThree = 28,

		/// <summary>
		/// Level Four
		/// </summary>
		InBusStop = 30,
		WalkWithGirl = 31,


		WalkInStreetFour = 40,
		ListenToMusic = 41,
		WalkOutStreetFour = 47,

		WalkIntoPeople = 50,
		WalkAcrossRoadWithGirl = 51,
		DepartFromGirl = 54,
		BeginShip = 56,


		WalkInStreetColorful = 70,

		BackToApartment = 80,

		PlayEndAnimation = 82,
		ShowCredit = 85,


		End = 99,
		EveryState = 100,

	}

	private AStateMachine<GameState,LogicEvents> m_stateMachine;
	public GameState State
	{
		get { return m_stateMachine.State; }
	}
	[SerializeField] GameState startState = GameState.Enter;


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
//		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(1) ) {
//			M_Event.FireLogicEvent (LogicEvents.Interact, new LogicArg (this));
//		}


		if ( CrossPlatformInputManager.GetButtonDown( "LockCam" ) ) {
			M_Event.FireLogicEvent (LogicEvents.UnlockCamera, new LogicArg (this));
		}
			
		if ( CrossPlatformInputManager.GetButtonUp( "LockCam" ) ) {
			M_Event.FireLogicEvent (LogicEvents.LockCamera, new LogicArg (this));
		}

		// for test
		if (Input.GetKeyDown (KeyCode.Q) && Input.GetKey(KeyCode.LeftControl) ) {
			m_stateMachine.State = GameState.End;
			M_Event.FireLogicEvent (LogicEvents.EndGame, new LogicArg (this));
		}

		// for test
		if (Input.GetKeyDown (KeyCode.G) && Input.GetKey(KeyCode.LeftControl) ) {
			m_stateMachine.State = GameState.WalkAcrossRoadWithGirl;
		}


		if (Input.GetKeyDown (KeyCode.C) && Input.GetKey(KeyCode.LeftControl) ) {
			m_stateMachine.State = GameState.ShowCredit;
		}


		if (Input.GetKeyDown (KeyCode.I) && Input.GetKey(KeyCode.LeftControl) ) {
			m_stateMachine.State = GameState.BeginShip;
			M_Event.FireLogicEvent (LogicEvents.InvisibleFromPlayer, new LogicArg (this));
		}

//		if (Input.GetKeyDown (KeyCode.Space)) {
//			M_Event.FireLogicEvent (LogicEvents.SwitchThoughtBox, new LogicArg (this));
//		}

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
		m_stateMachine =  new AStateMachine<GameState, LogicEvents>( GameState.None );

//		m_stateMachine.BlindTimeStateChange (GameState.Enter, GameState.TalkWithManInCafe, 1f);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.EndTalkManInCafe, GameState.Enter, GameState.TalkWithManInCafe);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.BeginDamage, GameState.TalkWithManInCafe, GameState.TryGoInRain);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.SeeGirlStreetTwo, GameState.SeeTakePhoto, GameState.SeeGirlStreetTwo);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.UnfocusCamera, GameState.SeeGirlStreetTwo, GameState.FindGirlStreetTwo);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.BusStopEndTalkGirl, GameState.InBusStop, GameState.WalkWithGirl);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.TrafficRedLight, GameState.WalkOutStreetFour, GameState.WalkIntoPeople);
		m_stateMachine.BlindTimeStateChange (GameState.WalkIntoPeople, GameState.WalkAcrossRoadWithGirl, 35f);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.ForceGirlLeave, GameState.WalkAcrossRoadWithGirl, GameState.DepartFromGirl);
//		m_stateMachine.BlindTimeStateChange (GameState.WalkAcrossRoadWithGirl, GameState.DepartFromGirl, 10f);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.InvisibleFromPlayer, GameState.DepartFromGirl, GameState.BeginShip);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.EnterEnd, GameState.WalkInStreetColorful, GameState.PlayEndAnimation);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.EndCredit, GameState.ShowCredit, GameState.End);


		m_stateMachine.BlindFromEveryState (LogicEvents.EndTalkWithGirl, GameState.TalkWithGirlInCafe);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetOne, GameState.WalkInStreetOne);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterRotateBuilding, GameState.SeeBuilding);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterBorrowUmbrella, GameState.BorrowUmbrella);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetTwo, GameState.WalkInStreetTwo);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterTakePhoto, GameState.SeeTakePhoto);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetThree, GameState.WalkInStreetThree);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterBusStop, GameState.InBusStop);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetThreeEnd, GameState.WalkOutStreetThree);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetFour, GameState.WalkInStreetFour);
		m_stateMachine.BlindFromEveryState (LogicEvents.GirlSayPlayMusic, GameState.ListenToMusic);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetFourEnd, GameState.WalkOutStreetFour);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetColorful, GameState.WalkInStreetColorful);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterApartment, GameState.BackToApartment);
		m_stateMachine.BlindFromEveryState (LogicEvents.WalkInApartment, GameState.PlayEndAnimation);
		m_stateMachine.BlindFromEveryState (LogicEvents.EndHitThree, GameState.ShowCredit);

		m_stateMachine.AddEnter (GameState.WalkAcrossRoadWithGirl, delegate() {
			M_Event.FireLogicEvent(LogicEvents.TrafficGreenLight,new LogicArg(this));
		});


		m_stateMachine.AddEnter (GameState.BeginShip, delegate() {
			M_Event.FireLogicEvent(LogicEvents.SwitchDefaultBGM , new LogicArg(this));	
		});


		m_stateMachine.AddEnter (GameState.BackToApartment, delegate() {
			M_Event.FireLogicEvent(LogicEvents.SwitchDefaultBGM , new LogicArg(this));	
		});

		m_stateMachine.AddEnter (GameState.End, delegate() {
			SceneManager.LoadScene("Title");
		});

		m_stateMachine.AddEnter (GameState.PlayEndAnimation, delegate() {
			m_MainCharacter.transform.DOMove(m_MainCharacter.transform.forward * m_MainCharacter.MoveSpeed * 4f , 4f ).SetRelative(true);
		});


		m_stateMachine.State = startState;
	}

	void OnGUI()
	{
		GUILayout.Label ("State " + State);
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

[System.Serializable]
public struct MWord
{
	public MWord( string eng , string chinese ){
		wordEng = eng;
		wordChinese = chinese;
	}
	public string word
	{
		get {
			if (LogicManager.Language == LogicManager.GameLanguage.Chinese)
				return wordChinese;
			return wordEng;
		}
	}

	public string wordEng;
	public string wordChinese;
}