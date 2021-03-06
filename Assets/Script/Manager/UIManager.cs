﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MBehavior {

	private static UIManager m_Instance;
	public static UIManager Instance{
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<UIManager> ();
			return m_Instance;
		}
	}

	public Image DialogBackImage;
	public Image DialogArrowImage;
	public Text DialogText;
	public RectTransform DialogFrame
	{
		get { return DialogBackImage.rectTransform; }
	}
	public RectTransform DialogArrow
	{
		get { return DialogArrowImage.rectTransform; }
	}
	public Canvas UICanvas;

	public InteractTips interactableTips;
	public Image thoughtBackground;
	public Text thoughtText;
	public float thoughtShowTime = 5f;
//	public Image screenBlack;
	public Text skipTips;
	public Transform cursorTransform;
	public Image cursorImage;
	public Sprite ScanSprite;
	public Sprite normalSprite;
	public Sprite runSprite;

	public Image EnergyIcon;
	public Image EnergyBar;
	public Image EnergyFrame;

	public RectTransform ending;
	public Image endingBack;
	public RectTransform endingCredit;
	public Image white;

	public RectTransform musicPlayer;
	public Image normalMusicPlayer;
	public Image brokenMusicPlayer;
	[SerializeField] float musicHide = -300f;

	public RectTransform Menu;
	public Text languageText;
	public Text MenuMainMenuButtonText;
	public Text MenuBackButtonText;
	public Text MenuLanguageTips;


	[System.Serializable]
	public struct StateTargetPair
	{
		public LogicManager.GameState state;
		public string target{
			get {
				if (LogicManager.Language == LogicManager.GameLanguage.English)
					return targetEng;
				if (LogicManager.Language == LogicManager.GameLanguage.Chinese)
					return targetChinese;
				return targetEng;
			}
		}
		public string targetEng;
		public string targetChinese;
	}
	[SerializeField] StateTargetPair[] targetList;

	protected override void MAwake ()
	{
		base.MAwake ();
//		OnSwitchThought ();

		FadeEnergy (0, 0);
		if (white != null) {
			white.gameObject.SetActive (true);
			white.DOFade (0, 2f).OnComplete (delegate() {
				white.gameObject.SetActive (false);
			});
		}

		if ( normalMusicPlayer != null)
			normalMusicPlayer.gameObject.SetActive( true ) ;
		if ( brokenMusicPlayer != null)
			brokenMusicPlayer.gameObject.SetActive( false );
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();

//		M_Event.logicEvents [(int)LogicEvents.Death] += OnDeath;
//		M_Event.logicEvents [(int)LogicEvents.DeathEnd] += OnDeathEnd;
		M_Event.logicEvents [(int)LogicEvents.UnlockCamera] += OnUnlockCamera;
		M_Event.logicEvents [(int)LogicEvents.LockCamera] += OnLockCamera;
		M_Event.logicEvents [(int)LogicEvents.DisplayThought] += OnDisplayThought;
		M_Event.logicEvents [(int)LogicEvents.HideThought] += OnHideThought;
//		M_Event.logicEvents [(int)LogicEvents.SwitchThoughtBox] += OnSwitchThoughtBox;
		M_Event.RegisterEvent (LogicEvents.BeginRun, OnBeginRun);
		M_Event.RegisterEvent (LogicEvents.EndRun, OnEndRun);
		M_Event.RegisterEvent (LogicEvents.GirlSayPlayMusic, OnShowMusicPlayer);
		M_Event.RegisterEvent (LogicEvents.PickUpMusicPlayer, OnShowBrokenMusicPlayer);
//		M_Event.RegisterEvent (LogicEvents.InvisibleFromPlayer, OnHideMusicPlayer );
		M_Event.RegisterEvent (LogicEvents.WalkInApartment, OnHideMusicPlayer );
		M_Event.RegisterEvent (LogicEvents.PauseGame, OnPause);
		M_Event.RegisterEvent (LogicEvents.UnpauseGame, OnUnpause);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
//		M_Event.logicEvents [(int)LogicEvents.Death] -= OnDeath;
//		M_Event.logicEvents [(int)LogicEvents.DeathEnd] -= OnDeathEnd;
		M_Event.logicEvents [(int)LogicEvents.UnlockCamera] -= OnUnlockCamera;
		M_Event.logicEvents [(int)LogicEvents.LockCamera] -= OnLockCamera;
		M_Event.logicEvents [(int)LogicEvents.DisplayThought] -= OnDisplayThought;
		M_Event.logicEvents [(int)LogicEvents.HideThought] -= OnHideThought;
//		M_Event.logicEvents [(int)LogicEvents.SwitchThoughtBox] -= OnSwitchThoughtBox;
		M_Event.UnregisterEvent (LogicEvents.BeginRun, OnBeginRun);
		M_Event.UnregisterEvent (LogicEvents.EndRun, OnEndRun);
		M_Event.UnregisterEvent (LogicEvents.GirlSayPlayMusic, OnShowMusicPlayer);
		M_Event.UnregisterEvent (LogicEvents.PickUpMusicPlayer, OnShowBrokenMusicPlayer);
//		M_Event.UnregisterEvent (LogicEvents.InvisibleFromPlayer, OnHideMusicPlayer );
		M_Event.UnregisterEvent (LogicEvents.WalkInApartment, OnHideMusicPlayer );
		M_Event.UnregisterEvent (LogicEvents.PauseGame, OnPause);
		M_Event.UnregisterEvent (LogicEvents.UnpauseGame, OnUnpause);
	
	}

	void OnShowBrokenMusicPlayer( LogicArg arg )
	{

		if ( normalMusicPlayer != null)
			normalMusicPlayer.gameObject.SetActive( false ) ;
		if ( brokenMusicPlayer != null)
			brokenMusicPlayer.gameObject.SetActive( true );
		
		ShowMusicPlayer (1f);
	}

	void OnShowMusicPlayer( LogicArg arg )
	{
		ShowMusicPlayer (1f);
	}

	void OnHideMusicPlayer( LogicArg arg )
	{
		HideMusicPlayer (1f);
	}

	void OnBeginRun( LogicArg arg )
	{
//		FadeEnergy (1f, 0.2f);

		cursorImage.sprite = runSprite;

	}

	void OnEndRun( LogicArg arg )
	{
//		FadeEnergy (0, 2f);
		cursorImage.sprite = normalSprite;
	}

	void FadeEnergy( float to , float time )
	{
		EnergyBar.DOKill ();
		EnergyBar.DOFade (to, time);
		EnergyIcon.DOKill ();
		EnergyIcon.DOFade (to, time);
		EnergyFrame.DOKill ();
		EnergyFrame.DOFade (to, time);
	}

//	void OnSwitchThoughtBox( LogicArg arg )
//	{
//		OnSwitchThought ();
//	}

	void OnUnlockCamera(LogicArg arg )
	{
		cursorImage.sprite = ScanSprite;
	}

	void OnLockCamera( LogicArg arg )
	{
		cursorImage.sprite = normalSprite;
	}

//	void OnDeath( LogicArg arg )
//	{
//		skipTips.DOFade (1f, 1f);
//	}
//
//	void OnDeathEnd(LogicArg arg)
//	{
//		screenBlack.color = Color.white;
//		screenBlack.DOFade (0, 2f).SetDelay(2f);
//		skipTips.DOFade (0, 1f);
//	}


	Vector3 musicPlayerOriPlace;

	protected override void MStart ()
	{
		base.MStart ();
		LogicManager.Instance.RegisterStateChange ( OnStateChange );
		skipTips.DOFade (0, 0);

		Cursor.visible = false;

		ending.gameObject.SetActive (false);

		musicPlayerOriPlace = musicPlayer.position;

		HideThought ();

		HideMusicPlayer ( 0 );

		Menu.gameObject.SetActive (false);
	}

	bool isThoughtOn = true;
	Sequence thoughtSequence;
	void OnDisplayThought( LogicArg arg )
	{
		Thought thought = (Thought)arg.GetMessage (M_Event.EVENT_THOUGHT);
		if ( thought.thought != null && thought.thought.Length > 1 ) {
			thoughtText.text = thought.thought;
			thoughtBackground.transform.DOKill ();
			thoughtBackground.transform.localScale = Vector3.one * 0.001f;
			thoughtSequence = DOTween.Sequence ();
			thoughtSequence.Append (thoughtBackground.transform.DOScale (1f, 0.3f));
			thoughtSequence.AppendCallback(delegate() {
				isThoughtOn = true;
			});
			thoughtSequence.AppendInterval (thoughtShowTime);
			thoughtSequence.Append (thoughtBackground.transform.DOScale (0, 0.3f));
			thoughtSequence.AppendCallback (delegate() {
				isThoughtOn = false;	
			});
		}
	}

	void OnHideThought( LogicArg arg )
	{
		HideThought ();
	}

	void HideThought()
	{
		if (isThoughtOn == true) {
			thoughtSequence.Kill ();
			thoughtBackground.transform.DOKill ();
			thoughtBackground.transform.DOScale (0, 0.3f);
			isThoughtOn = false;
		}
	}

	bool isMusicPlayerShown = false;
	public void OnSwitchMusicPlayer()
	{
		if (isMusicPlayerShown)
			HideMusicPlayer (1f);
		else
			ShowMusicPlayer (1f);
	}

	public void ShowMusicPlayer( float time )
	{
		musicPlayer.DOMove (musicPlayerOriPlace, time).SetEase (Ease.OutCubic);
		isMusicPlayerShown = true;
	}

	public void HideMusicPlayer( float time )
	{
		musicPlayer.DOMove (musicPlayerOriPlace + Vector3.up * musicHide , time ).SetEase (Ease.OutCubic);
		isMusicPlayerShown = false;
	}

//	public void OnSwitchThought()
//	{
//		if (isThoughtOn) {
//			thoughtBackground.transform.DOKill ();
//			thoughtBackground.transform.DOScale (0, 0.3f).OnComplete (delegate() {
//				isThoughtOn = false;
//			});
//		} else {
//			thoughtBackground.transform.DOKill ();
//			thoughtBackground.transform.DOScale (1f, 0.3f).OnComplete (delegate() {
//				isThoughtOn = true;
//			});
//			
//		}
//	}

	void OnStateChange( LogicManager.GameState fromState , LogicManager.GameState toState )
	{
		if (toState == LogicManager.GameState.ShowCredit) {
//			ending.DOScale (0.01f, 1f).From ();
			endingBack.DOFade (0.45f, 2f);
			endingBack.DOFade (1f, 5f).SetDelay (25f);
			endingCredit.transform.DOMoveY (1600f, 30f).OnComplete( delegate() {
				M_Event.FireLogicEvent(LogicEvents.EndCredit, new LogicArg(this));	
			});
			ending.gameObject.SetActive (true);
		} else if (toState == LogicManager.GameState.BeginShip) {
			HideMusicPlayer (0.3f);
		}
//		Debug.Log ("On Change" + toState);
//		foreach (StateTargetPair pair in targetList) {
//			if (toState == pair.state) {
//				targetText.DOKill ();
//				targetText.text = "";
//				targetText.DOText (pair.target, 2f );
//
//				return;
//			}
//		}
//		targetText.text = "";
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		UpdateInteractTips ();
		UpdateCursor ();
//		UpdateEnergy ();
		UpdateThought ();

		if (Input.GetKeyDown (KeyCode.M) && Input.GetKey (KeyCode.LeftControl)) {
//			ShowMusicPlayer (1f);
			OnSwitchMusicPlayer();
		}

		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.L)) {
			Debug.Log (cursorImage.gameObject.activeSelf);
			cursorImage.gameObject.SetActive (!cursorImage.gameObject.activeSelf);
			EnergyFrame.gameObject.SetActive (!EnergyFrame.gameObject.activeSelf);
			interactableTips.gameObject.SetActive (!interactableTips.gameObject.activeSelf);
		}
	}

	void UpdateThought()
	{
		Vector3 characterPos = MainCharacter.Instance.GetInteractiveCenter () ;

		Vector3 screenPos = Camera.main.WorldToViewportPoint ( characterPos - 2f * TalkableCharacter.InteractionPointOffset );

		RectTransform CanvasRect = UIManager.Instance.UICanvas.GetComponent<RectTransform> ();
		Vector2 targetPos = new Vector2 (
			((screenPos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
			((screenPos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

		thoughtBackground.rectTransform.anchoredPosition = targetPos + new Vector2( 66f , 66f );
	}

	void UpdateEnergy()
	{
		EnergyBar.fillAmount = MechanismManager.health.EnergyRate;
	}

	void UpdateInteractTips()
	{
		Interactable interact = InteractManager.Instance.TempInteractable;
		if (interact != null) {
			interactableTips.Show (interact);

			Vector3 screenPos = Camera.main.WorldToViewportPoint (interact.GetInteractCenter ());

			RectTransform CanvasRect = UICanvas.GetComponent<RectTransform> ();
			Vector2 WorldObject_ScreenPosition=new Vector2(
				((screenPos.x*CanvasRect.sizeDelta.x)-(CanvasRect.sizeDelta.x*0.5f)),
				((screenPos.y*CanvasRect.sizeDelta.y)-(CanvasRect.sizeDelta.y*0.5f)));

			interactableTips.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
		} else {
			interactableTips.Hide ();
		}
	}

	public void UpdateCursor()
	{
		Vector2 pos;
		// RectTransformUtility.ScreenPointToLocalPointInRectangle (UICanvas.GetComponent<RectTransform>(), InteractManager.FocusPoint, Camera.main, out pos);

		cursorTransform.position = InteractManager.FocusPoint;
	}

	public static Rect RectTransformToScreenSpace( RectTransform transform )
	{
		Vector2 size = Vector2.Scale (transform.rect.size, transform.lossyScale);
		return new Rect (transform.position.x, Screen.height - transform.position.y, size.x, size.y);
	}


	public void OnInteract()
	{
		M_Event.FireLogicEvent (LogicEvents.Interact, new LogicArg (this));
	}

	public void OnPlayMusic( string musicName )
	{
		LogicArg arg = new LogicArg (this);
		arg.AddMessage (M_Event.EVENT_PLAY_MUSIC_NAME, musicName);
		M_Event.FireLogicEvent (LogicEvents.PlayMusic, arg);
	}

	public void EndGame()
	{
		M_Event.FireLogicEvent (LogicEvents.EndGame, new LogicArg (this));
	}

	public void OnPause( LogicArg arg )
	{
		InitMenu ();
	}


	public void InitMenu( )
	{
		Menu.gameObject.SetActive (true);
		UpdateLanguageText ();
	}
	public void UpdateLanguageText()
	{
		languageText.text = (LogicManager.Language == LogicManager.GameLanguage.English) ? "简体中文" : "English";
		MenuMainMenuButtonText.text = (LogicManager.Language == LogicManager.GameLanguage.English) ? "MAIN MENU" : "主菜单";
		MenuBackButtonText.text = (LogicManager.Language == LogicManager.GameLanguage.English) ? "BACK" : "返回";
		MenuLanguageTips.text = (LogicManager.Language == LogicManager.GameLanguage.English) ? "LANGUAGE" : "语言";
	}

	public void BackToTitle()
	{
		SceneManager.LoadScene ("Title");
	}


	public void OnUnpause( LogicArg arg )
	{
		CloseMenu ();
	}

	public void BackToGame()
	{
		M_Event.FireLogicEvent (LogicEvents.UnpauseGame, new LogicArg(this));
	}

	public void ChangeLanguage()
	{
		if (LogicManager.Language == LogicManager.GameLanguage.English) {
			LogicManager.ChangeLanguageTo (LogicManager.GameLanguage.Chinese);
		} else if (LogicManager.Language == LogicManager.GameLanguage.Chinese) {
			LogicManager.ChangeLanguageTo (LogicManager.GameLanguage.English);
		}

		UpdateLanguageText ();
	}

	public void CloseMenu(  )
	{
		Menu.gameObject.SetActive (false);
	}
}
