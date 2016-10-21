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
	private float backImageOriginalAlpha = 0;
	List<DisplayPlot> tem_Plots = new List<DisplayPlot>();

	private bool m_isDisplaying = false;
	public bool IsDisplaying{ get { return m_isDisplaying; } }

	private int index=0;

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

		if (m_backImage != null) {
			backImageOriginalAlpha = m_backImage.color.a;
			m_backImage.DOFade (0, 0);
		}
		if (m_text != null) {
			m_text.DOFade (0, 0);
		}
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
		if (tem_Plot != null) {
			tem_Plots.Add (new DisplayPlot (tem_Plot,otherSpeaker));
			if ( !IsDisplaying )
				NextDialog ();
		}
	}

	void DisplayDialog( NarrativeDialog dialog , DisplayPlot plot )
	{
		if (m_text != null) {
			m_text.text = dialog.word;
			m_text.DOFade (1f, showUpTime);
		}
		if (m_backImage != null) {
			m_backImage.DOFade (backImageOriginalAlpha, showUpTime);
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

		temSpeaker.clip = dialog.clip;
		temSpeaker.Play ();
	}

	void NextDialog()
	{
		if (m_isDisplaying) {
			if ( m_text != null )
			m_text.DOFade (0, showUpTime).OnComplete(SwitchToNext);
			if ( m_backImage != null )
			m_backImage.DOFade (0, showUpTime);
		} else {
			m_isDisplaying = true;
			SwitchToNext ();
		}

	}

	void SwitchToNext()
	{
		// if there is not plot to show
		if (tem_Plots.Count <= 0) {
			EndDisplay ();
		} else {
			NarrativeDialog dialog = tem_Plots [0].NextDialog ();
			if (dialog == null) {
				tem_Plots.RemoveAt (0);
				if (tem_Plots.Count <= 0) {
					EndDisplay ();
				}
			} else {
				DisplayDialog (dialog,tem_Plots[0]);
			}
		}
	}

	void EndDisplay()
	{
		if (m_text != null)
			m_text.DOFade (0, showUpTime);
		if ( m_backImage != null )
			m_backImage.DOFade (0, showUpTime);
		temSpeaker = null;
		m_isDisplaying = false;

		M_Event.FireLogicEvent (LogicEvents.EndDisplayDialog, new LogicArg (this));
	}


	protected override void MUpdate ()
	{
		base.MUpdate ();

		if (Input.GetMouseButtonDown(0)) {
			NextDialog ();
		}
		if (temSpeaker != null && !temSpeaker.isPlaying) {
			NextDialog ();
		}
	}
}


class DisplayPlot
{
	public NarrativePlotScriptableObject plot;
	public int index;
	public AudioSource otherSpeaker;

	public DisplayPlot(NarrativePlotScriptableObject _plot , AudioSource _other)
	{
		index = 0;
		plot = _plot;
		otherSpeaker = _other;
	}

	public NarrativeDialog NextDialog()
	{
		if (plot.dialogs.Count > index) {
			return plot.dialogs [index++];
		}
		return null;
	}
}