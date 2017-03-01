using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityStandardAssets.CrossPlatformInput;

public class NarrativeManager : MBehavior {


	static NarrativeManager m_Instance;
	public static NarrativeManager Instance{ 
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<NarrativeManager> ();
			return m_Instance;}}
	

	[SerializeField] float showUpTime=0.2f;
	[SerializeField] KeyCode nextDialogKey;
	[SerializeField] float autoSkipTime = 15f;
	[SerializeField] GameObject smallDialogPrefab;
	[SerializeField] GameObject iconDialogPrefab;
	[SerializeField] bool HideDialog;
	[SerializeField] bool HideIconDialog;

	private AudioSource mainCharacterSpeaker;
	private AudioSource temSpeaker;
	Text m_text;
	Image m_backImage;
	Image m_arrow;

	bool m_test_ShowDialog = true;

	private float backImageOriginalAlpha = 0;
	List<DisplayPlot> PlotArray = new List<DisplayPlot>();
	NarrativeDialog temDialog;
	DisplayPlot temPlot
	{
		get {
			if (PlotArray.Count > 0)
				return PlotArray [0];
			return null;
		}
	}

	private bool m_isDisplaying = false;
	public bool IsDisplaying{ get { return m_isDisplaying; } }

	private int index=0;

	RectTransform dialogFrame;
	RectTransform dialogArrow;
	Vector2 dialogFrameOriPos;

	Color mainDialogColor = Color.white;
	Color otherDialogColor = new Color( 0.65f , 0.65f , 0.65f , 1f );
	Color girlDialogColor = new Color (0.75f, 0.65f, 0.65f, 1f);

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.DisplayDialog] += OnDisplayDialog;
		M_Event.logicEvents [(int)LogicEvents.DisplayIconDialog] += OnDisplayIconDialog;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.DisplayDialog] -= OnDisplayDialog;
		M_Event.logicEvents [(int)LogicEvents.DisplayIconDialog] -= OnDisplayIconDialog;
	}

	protected override void MAwake ()
	{
		base.MAwake ();

		mainCharacterSpeaker = AddSpeaker (gameObject);

		m_text = UIManager.Instance.DialogText;
		m_backImage = UIManager.Instance.DialogBackImage;
		m_arrow = UIManager.Instance.DialogArrowImage;

		if (m_backImage != null) {
			backImageOriginalAlpha = m_backImage.color.a;
			m_backImage.DOFade (0, 0);
		}
		if (m_text != null) {
			m_text.DOFade (0, 0);
		}
		if (m_arrow != null) {
			m_arrow.DOFade (0, 0);
		}

		dialogFrame = UIManager.Instance.DialogFrame;
		dialogArrow = UIManager.Instance.DialogArrow;
		dialogFrameOriPos =  dialogFrame.anchoredPosition;
	}

	AudioSource AddSpeaker( GameObject obj )
	{
		AudioSource res = obj.AddComponent<AudioSource> ();
		res.spatialBlend = 1f;
		res.volume = 0.5f;
		res.loop = false;
		return res;
	}

	void OnDisplayIconDialog(LogicArg arg)
	{
		if (!HideIconDialog) {
			IconNarrativeDialog dialog = (IconNarrativeDialog)arg.GetMessage (M_Event.EVENT_ICON_NARRATIV_DIALOG);
			DisplayIconDialog (dialog);
		}
	}

	void OnDisplayDialog(LogicArg arg)
	{
		if (!HideDialog) {
			NarrativePlotScriptableObject tem_Plot = (NarrativePlotScriptableObject)arg.GetMessage (M_Event.EVENT_DISPLAY_DIALOG_PLOT);
			MonoBehaviour other = (MonoBehaviour)arg.sender;
			AudioSource otherSpeaker = AddSpeaker (other.gameObject);
			TalkableCharacter sender = (TalkableCharacter)arg.sender;

			if (tem_Plot != null) {
				if (!tem_Plot.important) {
					if (tem_Plot.dialogs != null && tem_Plot.dialogs.Count > 0)
						DisplayUnimportantDialog (tem_Plot.dialogs [0], new DisplayPlot (tem_Plot, otherSpeaker, sender));
				} else {
					PlotArray.Add (new DisplayPlot (tem_Plot, otherSpeaker, sender));
					if (!IsDisplaying)
						NextDialog ();
				}
			}
		}
	}

	void DisplayUnimportantDialog( NarrativeDialog dialog , DisplayPlot plot )
	{
		if (m_test_ShowDialog && !HideDialog ) {
			plot.otherSpeaker.clip = dialog.clip;
			plot.otherSpeaker.Play ();


			GameObject smallDialog = Instantiate (smallDialogPrefab) as GameObject;
			smallDialog.transform.SetParent (UIManager.Instance.transform);
			Dialog dialogCom = smallDialog.GetComponent<Dialog> ();

			if (dialogCom != null)
				dialogCom.Init (plot.character, dialog);
		}

	}

	void DisplayIconDialog( IconNarrativeDialog dialog  )
	{
//		if (m_test_ShowDialog && !HideDialog ) {
//			plot.otherSpeaker.clip = dialog.clip;
//			plot.otherSpeaker.Play ();
//
//
//			GameObject smallDialog = Instantiate (smallDialogPrefab) as GameObject;
//			smallDialog.transform.SetParent (UIManager.Instance.transform);
//			Dialog dialogCom = smallDialog.GetComponent<Dialog> ();
//
//			if (dialogCom != null)
//				dialogCom.Init (plot.character, dialog);
//		}
		if (!HideIconDialog ) {
			GameObject smallDialog = Instantiate (iconDialogPrefab) as GameObject;
			smallDialog.transform.SetParent (UIManager.Instance.transform);
			IconDialog dialogCom = smallDialog.GetComponent<IconDialog> ();

			if (dialogCom != null)
				dialogCom.Init (dialog);
		}
	}

	Color GetDialogColorFromType( NarrativeDialog.SpeakerType type )
	{
		if (type == NarrativeDialog.SpeakerType.MainCharacter)
			return mainDialogColor;
		else if (type == NarrativeDialog.SpeakerType.Girl)
			return girlDialogColor;
		else
			return otherDialogColor;
	}

	void DisplayDialog( NarrativeDialog dialog , DisplayPlot plot )
	{
		if (!HideDialog) {
			if (m_text != null) {
				m_text.DOKill ();
				m_text.text = dialog.word;
//			m_text.DOText( dialog.word , dialog.word.Length * 0.06f + 0.3f );
				m_text.DOFade (1f, showUpTime);
			}
			if (m_backImage != null) {
				m_backImage.DOKill ();
				m_backImage.color = GetDialogColorFromType (dialog.type);
				m_backImage.transform.localScale = Vector3.one * 0.01f;
				m_backImage.transform.DOScale (1f, showUpTime);
				m_backImage.DOFade (backImageOriginalAlpha, showUpTime);
			}
			if (m_arrow != null) {
				m_arrow.DOKill ();
				m_arrow.DOFade (backImageOriginalAlpha, showUpTime);
			}

			if (temSpeaker != null)
				temSpeaker.Stop ();
		
			switch (dialog.type) {
			case NarrativeDialog.SpeakerType.ThisCharacter:
			case NarrativeDialog.SpeakerType.Girl:
				temSpeaker = plot.otherSpeaker;
				break;
			case NarrativeDialog.SpeakerType.MainCharacter:
				temSpeaker = mainCharacterSpeaker;
				break;
			default:
				break;
			}
			;

			if (dialog.clip != null) {
				temSpeaker.clip = dialog.clip;
				temSpeaker.Play ();
			}

			temDialog = dialog;

			UpdateDialogFramePosition ();

			LogicArg arg = new LogicArg (this);
			arg.AddMessage ("word", dialog.word);
			arg.AddMessage ("character", dialog.type.ToString ());
			arg.AddMessage ("important", plot.plot.important);
			M_Event.FireLogicEvent (LogicEvents.DisplayNextDialog, arg);

			dialogTimer = 0;
		}
	}

	void NextDialog()
	{
		if (!m_isDisplaying) { // if not displaying 
			// show the text 
			if ( m_text != null )
				m_text.DOFade (0, showUpTime).OnComplete(SwitchToNext);
			if ( m_backImage != null )
				m_backImage.DOFade (0, showUpTime).SetDelay( 0.1f );
			if ( m_arrow != null )
				m_arrow.DOFade (0, showUpTime * 0.2f );
			m_isDisplaying = true;
		} else {
			SwitchToNext ();
		}

	}

	void SwitchToNext()
	{
		// if there is not plot to show
		if (temPlot == null ) {
			EndDisplay ();
		} else {
			NarrativeDialog dialog = temPlot.NextDialog ();
			if (dialog == null) {
				EndDisplayPlot (temPlot);
				PlotArray.Remove (temPlot);

				if (temPlot == null ) {
					EndDisplay ();
					return;
				}
				dialog = temPlot.NextDialog ();
			}
			DisplayDialog (dialog, temPlot );
		}		
	}

	void EndDisplayPlot ( DisplayPlot plot )
	{
		LogicArg arg = new LogicArg (this);
		arg.AddMessage (M_Event.EVENT_END_DISPLAY_SENDER, plot.character);
		arg.AddMessage (M_Event.EVENT_END_DISPLAY_FRAME, true);
		M_Event.FireLogicEvent (LogicEvents.EndDisplayDialog, arg );

		if (plot.plot.endPlotEvent != LogicEvents.None) {
			M_Event.FireLogicEvent (plot.plot.endPlotEvent, arg);
		}

	}

	void EndDisplay()
	{
		if (m_isDisplaying) {
			if (m_text != null)
				m_text.DOFade (0, showUpTime);
			if (m_backImage != null) {
				m_backImage.transform.DOScale (0, showUpTime);
			}
			if (m_arrow != null)
				m_arrow.DOFade (0, showUpTime * 0.5f);
			temSpeaker = null;
			temDialog = null;

			m_isDisplaying = false;

			Debug.Log ("End Display");

			dialogTimer = -1f;
		}
	}

	float dialogTimer = 0;

	protected override void MUpdate ()
	{
		
		base.MUpdate ();

		if ( dialogTimer > 0 )
			dialogTimer += Time.deltaTime;

		if (!HideDialog) {
			if (CrossPlatformInputManager.GetButtonDown ("SkipDialog") && m_isDisplaying) {
				NextDialog ();
			}

			if (dialogTimer > autoSkipTime && m_isDisplaying) {
				NextDialog ();
			}

			if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.L)) {
				m_test_ShowDialog = !m_test_ShowDialog;
			}
			
//		if (temSpeaker != null && !temSpeaker.isPlaying) {
//			NextDialog ();
//		}
			UpdateDialogArrow ();
		}
	}

	void UpdateDialogArrow()
	{
		if (temDialog != null && temPlot != null) {

			if (temDialog.type == NarrativeDialog.SpeakerType.ThisCharacter || temDialog.type == NarrativeDialog.SpeakerType.MainCharacter || temDialog.type == NarrativeDialog.SpeakerType.Girl ) {

				Vector3 characterPos = (temDialog.type == NarrativeDialog.SpeakerType.ThisCharacter || temDialog.type == NarrativeDialog.SpeakerType.Girl ) ?
					temPlot.character.GetInteractCenter () :
					MainCharacter.Instance.GetInteractiveCenter ();

				Vector3 screenPos = Camera.main.WorldToViewportPoint ( characterPos - TalkableCharacter.InteractionPointOffset );

				RectTransform CanvasRect = UIManager.Instance.UICanvas.GetComponent<RectTransform> ();
				Vector2 targetPos = new Vector2 (
					((screenPos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
					((screenPos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));


				float width = dialogFrame.rect.width - 60f ;
				float height = dialogFrame.rect.height - 40f ;

				Vector2 nowPos = dialogFrame.anchoredPosition;
				Vector2 arrowPos = Vector2.zero;

				arrowPos.x = Mathf.Clamp (targetPos.x, nowPos.x - width / 2f, nowPos.x + width / 2f);
				arrowPos.y = Mathf.Clamp (targetPos.y, nowPos.y - height / 2f, nowPos.y + height / 2f);

				if ((arrowPos.x > nowPos.x - width / 2f && arrowPos.x < nowPos.x + width / 2f) &&
				    (arrowPos.y > nowPos.y - height / 2f && arrowPos.y < nowPos.y + height / 2f)) {
					dialogArrow.gameObject.SetActive (false);
					return;
				}
				else
					dialogArrow.gameObject.SetActive (true);

				dialogArrow.anchoredPosition = arrowPos - dialogFrame.anchoredPosition;

				Vector2 forward = targetPos - arrowPos;
//				Debug.Log ("arrowPos " + arrowPos + " screen pos " + WorldObject_ScreenPosition);
				float angle = - Mathf.Atan (forward.x / forward.y) * Mathf.Rad2Deg;
				if (forward.y < 0)
					angle += 180f;
//				if (angle > 45f)
//					angle = 45f;
//				else if (angle < -45f)
//					angle = -45f;
//				Debug.Log ("angle " + angle);
				dialogArrow.rotation = Quaternion.Euler (0, 0, angle);

				m_arrow.color = GetDialogColorFromType (temDialog.type);
				
			}

		}
	}

	void UpdateDialogFramePosition()
	{
		if (temDialog != null && temPlot != null ) {
			if (temDialog.type == NarrativeDialog.SpeakerType.ThisCharacter || temDialog.type == NarrativeDialog.SpeakerType.Girl ) {
				
				Vector3 screenPos = Camera.main.WorldToViewportPoint ( temPlot.character.GetInteractCenter() );

				RectTransform CanvasRect = UIManager.Instance.UICanvas.GetComponent<RectTransform> ();
				Vector2 WorldObject_ScreenPosition = new Vector2 (
					                                   ((screenPos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
					                                   ((screenPos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
				
				if (WorldObject_ScreenPosition.x < -500f)
					WorldObject_ScreenPosition.x = -500f;
				if (WorldObject_ScreenPosition.x > 500f)
					WorldObject_ScreenPosition.x = 500f;
				dialogFrame.anchoredPosition = WorldObject_ScreenPosition;
			} else {
				dialogFrame.anchoredPosition = dialogFrameOriPos;

			}
		}
	}

	void OnGUI()
	{
//		GUILayout.Label ("");
//		GUILayout.Label ("");
//		GUILayout.Label ("is displaying " + Instance.IsDisplaying);
	}
}


class DisplayPlot
{
	public NarrativePlotScriptableObject plot;
	public int index;
	public AudioSource otherSpeaker;
	public TalkableCharacter character;

	public DisplayPlot(NarrativePlotScriptableObject _plot , AudioSource _other , TalkableCharacter _char  )
	{
		index = 0;
		plot = _plot;
		otherSpeaker = _other;
		character = _char;
	}

	public NarrativeDialog NextDialog()
	{
		if (plot.dialogs.Count > index) {
			return plot.dialogs [index++];
		}
		return null;
	}
}
