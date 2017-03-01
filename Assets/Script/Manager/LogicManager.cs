using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LogicManager : MonoBehaviour {

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
	static GameLanguage m_language ;
	static public GameLanguage Language{
		get { return  m_language; }
	}
	static public void ChangeLanguageTo( GameLanguage language )
	{
		m_language = language;
	}
	[SerializeField] float soundVolume;

	public static bool isGamePaused
	{
		get {
			return m_isGamePaused;
		}
	}
	static bool m_isGamePaused=false;


	public enum GameState
	{
		None=0,
		/// <summary>
		/// Level One
		/// </summary>
		Enter = 1,
		CafeReadyToGo = 2,
		TalkWithGirlInCafe = 4,
		WalkInStreetOne = 5,
		StreetOneToOld = 6,
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
		PickUpMusicPlayer = 59,


		WalkInStreetColorful = 70,

		BackToApartment = 80,
		TalkWithFakeGirl = 81,

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
	void Awake()
	{
		if (m_Instance == null)
			m_Instance = this;
		else
			Destroy (this);

		m_MainCharacter = FindObjectOfType<MainCharacter> ();


		InitStateMachine ();

//		PlaySoundOnEvent[] s = FindObjectsOfType<PlaySoundOnEvent> ();
//		foreach (PlaySoundOnEvent e in s)
//			Debug.Log (e.name);

	}


	void Start()
	{

		M_Event.FireLogicEvent (LogicEvents.LockCamera, new LogicArg (this));
	}

//Control + Q to exit the gameObject
//Control + G to turn on the green light
//Control + C to Show Credit
//Control + I to bring in the ship
//Control + E to toggle the effect manager
//Alt + number to select the camera
//Control + K to toggle the main Character
//Control + M to toggle the music player
//Control + L to toggle the UI

	void Update()
	{

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

		if (CrossPlatformInputManager.GetButtonDown ("PauseGame")) {
			if (isGamePaused)
				OnUnpauseGame ( true );
			else
				OnPauseGame ();
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

		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.E)) {
			EffectManager effectManager = FindObjectOfType<EffectManager> ();
			effectManager.enabled = !effectManager.isActiveAndEnabled;
		}

		if (Input.GetKey (KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) ) {
			if (Input.GetKeyDown (KeyCode.Alpha1)) {
				Debug.Log ("Press One ALPHA");
				M_Event.FireLogicEvent (LogicEvents.FilmShotOne, new LogicArg (this));
				m_stateMachine.State = GameState.BackToApartment;
			}
			if (Input.GetKeyDown (KeyCode.Alpha2)) {
				M_Event.FireLogicEvent (LogicEvents.FilmShotTwo, new LogicArg (this));
				m_stateMachine.State = GameState.WalkInStreetTwo;
			}
			if (Input.GetKeyDown (KeyCode.Alpha3)) {
				M_Event.FireLogicEvent (LogicEvents.FilmShotThree, new LogicArg (this));
				m_stateMachine.State = GameState.WalkInStreetTwo;
			}
			if (Input.GetKeyDown (KeyCode.Alpha4)) {
				M_Event.FireLogicEvent (LogicEvents.FilmShotFour, new LogicArg (this));
				m_stateMachine.State = GameState.WalkInStreetThree;
				Camera.main.fieldOfView = 60f;
			}
			if (Input.GetKeyDown (KeyCode.Alpha5)) {
				M_Event.FireLogicEvent (LogicEvents.FilmShotFive, new LogicArg (this));
				m_stateMachine.State = GameState.InBusStop;
				Camera.main.fieldOfView = 20f;
			}
			if (Input.GetKeyDown (KeyCode.Alpha6)) {
				M_Event.FireLogicEvent (LogicEvents.FilmShotSix, new LogicArg (this));
				m_stateMachine.State = GameState.BackToApartment;
			}
			if (Input.GetKeyDown (KeyCode.Alpha7)) {
				M_Event.FireLogicEvent (LogicEvents.FilmShotSeven, new LogicArg (this));
			}
			if (Input.GetKeyDown (KeyCode.Alpha8))
				M_Event.FireLogicEvent (LogicEvents.FilmShotEight, new LogicArg (this));
			if (Input.GetKeyDown (KeyCode.Alpha9))
				M_Event.FireLogicEvent (LogicEvents.FilmShotNine, new LogicArg (this));
		}

	

		m_stateMachine.Update ();
	}

	void OnEnable()
	{
		M_Event.RegisterAll (OnEvent);
	}

	void OnDisable()
	{
		M_Event.RegisterAll (OnEvent);
	}

	void OnEvent(LogicArg arg)
	{
		m_stateMachine.OnEvent (arg.type);
		if (arg.type == LogicEvents.UnpauseGame) {
			OnUnpauseGame (false);
		}
	}

	void OnPauseGame()
	{
		m_isGamePaused = true;
		MBehavior.isPaused = true;
		DOTween.PauseAll ();
		M_Event.FireLogicEvent (LogicEvents.PauseGame, new LogicArg (this));
	}

	void OnUnpauseGame( bool isFire )
	{
		m_isGamePaused = false;
		MBehavior.isPaused = false;
		DOTween.PlayAll ();
		if ( isFire )
		M_Event.FireLogicEvent (LogicEvents.UnpauseGame, new LogicArg (this));
	}

	void InitStateMachine()
	{
		m_stateMachine =  new AStateMachine<GameState, LogicEvents>( GameState.None );

//		m_stateMachine.BlindTimeStateChange (GameState.Enter, GameState.TalkWithManInCafe, 1f);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.CafeEndPackUp, GameState.Enter, GameState.CafeReadyToGo);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.StreetOneWatchCorw, GameState.WalkInStreetOne, GameState.StreetOneToOld);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.SeeGirlStreetTwo, GameState.SeeTakePhoto, GameState.SeeGirlStreetTwo);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.UnfocusCamera, GameState.SeeGirlStreetTwo, GameState.FindGirlStreetTwo);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.BusStopEndTalkGirl, GameState.InBusStop, GameState.WalkWithGirl);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.TrafficRedLight, GameState.WalkOutStreetFour, GameState.WalkIntoPeople);
		m_stateMachine.BlindTimeStateChange (GameState.WalkIntoPeople, GameState.WalkAcrossRoadWithGirl, 35f);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.ForceGirlLeave, GameState.WalkAcrossRoadWithGirl, GameState.DepartFromGirl);
//		m_stateMachine.BlindTimeStateChange (GameState.WalkAcrossRoadWithGirl, GameState.DepartFromGirl, 10f);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.InvisibleFromPlayer, GameState.DepartFromGirl, GameState.BeginShip);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.EnterEnd, GameState.WalkInStreetColorful, GameState.PlayEndAnimation);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.EndTalkWithFakeGirl, GameState.BackToApartment, GameState.TalkWithFakeGirl);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.EndCredit, GameState.ShowCredit, GameState.End);


		m_stateMachine.BlindFromEveryState (LogicEvents.EndTalkWithGirl, GameState.TalkWithGirlInCafe);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetOne, GameState.WalkInStreetOne);
//		m_stateMachine.BlindFromEveryState (LogicEvents.EnterRotateBuilding, GameState.SeeBuilding);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterBorrowUmbrella, GameState.BorrowUmbrella);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetTwo, GameState.WalkInStreetTwo);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterTakePhoto, GameState.SeeTakePhoto);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetThree, GameState.WalkInStreetThree);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterBusStop, GameState.InBusStop);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetThreeEnd, GameState.WalkOutStreetThree);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetFour, GameState.WalkInStreetFour);
		m_stateMachine.BlindFromEveryState (LogicEvents.GirlSayPlayMusic, GameState.ListenToMusic);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetFourEnd, GameState.WalkOutStreetFour);
		m_stateMachine.BlindFromEveryState (LogicEvents.PickUpMusicPlayer, GameState.PickUpMusicPlayer);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterStreetColorful, GameState.WalkInStreetColorful);
		m_stateMachine.BlindFromEveryState (LogicEvents.EnterApartment, GameState.BackToApartment);
		m_stateMachine.BlindFromEveryState (LogicEvents.WalkInApartment, GameState.PlayEndAnimation);
		m_stateMachine.BlindFromEveryState (LogicEvents.EndHitThree, GameState.ShowCredit);

		m_stateMachine.AddEnter (GameState.StreetOneToOld, delegate() {
			M_Event.FireLogicEvent(LogicEvents.ToOld, new LogicArg(this));	
		});

		m_stateMachine.AddEnter (GameState.WalkAcrossRoadWithGirl, delegate() {
			M_Event.FireLogicEvent(LogicEvents.TrafficGreenLight,new LogicArg(this));
		});

		m_stateMachine.AddEnter (GameState.BeginShip, delegate() {
			LogicArg arg = new LogicArg(this);
			arg.AddMessage(M_Event.EVENT_BGM_FADE_TIME,0);
			M_Event.FireLogicEvent(LogicEvents.SwitchDefaultBGM , arg );	
		});

		m_stateMachine.AddEnter (GameState.TalkWithFakeGirl, delegate() {
			LogicArg arg = new LogicArg(this);
			arg.AddMessage(M_Event.EVENT_BGM_FADE_TIME,5f);
			M_Event.FireLogicEvent(LogicEvents.SwitchDefaultBGM , arg );	
		});

		m_stateMachine.AddEnter (GameState.End, delegate() {
			SceneManager.LoadScene("Title");
		});

		m_stateMachine.AddEnter (GameState.PlayEndAnimation, delegate() {
			m_MainCharacter.transform.DOMove(m_MainCharacter.transform.forward * m_MainCharacter.MoveSpeed * 2f , 2f ).SetRelative(true);
		});


		m_stateMachine.State = startState;
	}



	void OnGUI()
	{
//		 GUILayout.Label ("State " + State);
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