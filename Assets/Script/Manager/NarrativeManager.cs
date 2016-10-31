using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class NarrativeManager : MBehavior {
	[SerializeField] float showUpTime=0.2f;
	[SerializeField] KeyCode nextDialogKey;

	private AudioSource mainCharacterSpeaker;
	private AudioSource temSpeaker;
	Text m_text;
	Image m_backImage;
	Image m_arrow;

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

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.DisplayDialog] += OnDisplayDialog;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.DisplayDialog] -= OnDisplayDialog;
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

	void OnDisplayDialog(LogicArg arg)
	{
		NarrativePlotScriptableObject tem_Plot = (NarrativePlotScriptableObject)arg.GetMessage (M_Event.EVENT_DISPLAY_DIALOG_PLOT);
		MonoBehaviour other =(MonoBehaviour) arg.sender;
		AudioSource otherSpeaker = AddSpeaker (other.gameObject);
		TalkableCharacter sender = (TalkableCharacter)arg.sender;
		if (tem_Plot != null) {
			PlotArray.Add (new DisplayPlot (tem_Plot,otherSpeaker,sender));
			if ( !IsDisplaying )
				NextDialog ();
		}
	}

	void DisplayDialog( NarrativeDialog dialog , DisplayPlot plot )
	{
		if (m_text != null) {
			m_text.DOKill ();
			m_text.text = dialog.word;
//			m_text.DOText( dialog.word , dialog.word.Length * 0.06f + 0.3f );
			m_text.DOFade (1f, showUpTime);
		}
		if (m_backImage != null) {
			m_backImage.transform.localScale = Vector3.one * 0.01f;
			m_backImage.transform.DOScale (1f, showUpTime);
			m_backImage.DOFade (backImageOriginalAlpha, showUpTime);
		}
		if (m_arrow != null) {
			m_arrow.DOFade (backImageOriginalAlpha, showUpTime);
		}

		if (temSpeaker != null)
			temSpeaker.Stop ();
		switch (dialog.type) {
		case NarrativeDialog.SpeakerType.ThisCharacter:
			temSpeaker = plot.otherSpeaker;
			break;
		case NarrativeDialog.SpeakerType.MainCharacter:
			temSpeaker = mainCharacterSpeaker;
			break;
		default:
			break;
		};

		if (dialog.clip != null) {
			temSpeaker.clip = dialog.clip;
			temSpeaker.Play ();
		}

		temDialog = dialog;

		LogicArg arg = new LogicArg (this);
		arg.AddMessage ("word", dialog.word);
		arg.AddMessage ("character", dialog.type.ToString ());
		M_Event.FireLogicEvent (LogicEvents.DisplayNextDialog, arg);

		UpdateDialogFramePosition ();
	}

	void NextDialog()
	{
		if (!m_isDisplaying) {
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
				PlotArray.RemoveAt (0);
				if (temPlot == null ) {
					EndDisplay ();
					return;
				}
				dialog = temPlot.NextDialog ();
			}
			DisplayDialog (dialog, temPlot );
		}		
	}

	void EndDisplay()
	{
		if (m_text != null)
			m_text.DOFade (0, showUpTime);
		if (m_backImage != null) {
			m_backImage.DOFade (0, showUpTime).SetDelay (0.1f);
			m_backImage.transform.DOScale (0, showUpTime);
		}
		if ( m_arrow != null )
			m_arrow.DOFade (0, showUpTime * 0.2f );
		temSpeaker = null;
		temDialog = null;

		m_isDisplaying = false;

		M_Event.FireLogicEvent (LogicEvents.EndDisplayDialog, new LogicArg (this));
	}


	protected override void MUpdate ()
	{
		base.MUpdate ();

		if ( (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && m_isDisplaying) {
			NextDialog ();
		}
//		if (temSpeaker != null && !temSpeaker.isPlaying) {
//			NextDialog ();
//		}
		UpdateDialogArrow();
	}

	void UpdateDialogArrow()
	{
		if (temDialog != null && temPlot != null) {

			if (temDialog.type == NarrativeDialog.SpeakerType.ThisCharacter) {

				Vector3 screenPos = Camera.main.WorldToViewportPoint ( temPlot.character.GetInteractCenter() - TalkableCharacter.InteractionPointOffset );

				RectTransform CanvasRect = UIManager.Instance.UICanvas.GetComponent<RectTransform> ();
				Vector2 targetPos = new Vector2 (
					((screenPos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
					((screenPos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));


				float width = dialogFrame.rect.width - 60f ;
				float height = dialogFrame.rect.height - 60f ;

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
				
			}

		}
	}

	void UpdateDialogFramePosition()
	{
		if (temDialog != null && temPlot != null ) {
			if (temDialog.type == NarrativeDialog.SpeakerType.ThisCharacter) {
				
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
				dialogArrow.gameObject.SetActive (true);
			} else {
				dialogArrow.gameObject.SetActive (false);
				dialogFrame.anchoredPosition = dialogFrameOriPos;

			}
		}
	}
}


class DisplayPlot
{
	public NarrativePlotScriptableObject plot;
	public int index;
	public AudioSource otherSpeaker;
	public TalkableCharacter character;

	public DisplayPlot(NarrativePlotScriptableObject _plot , AudioSource _other , TalkableCharacter _char )
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