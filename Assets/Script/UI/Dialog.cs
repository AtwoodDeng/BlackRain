using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityStandardAssets.CrossPlatformInput;

public class Dialog : MBehavior {

	public TalkableCharacter character;
	[SerializeField] RectTransform dialogFrame;
	[SerializeField] RectTransform dialogArrow;
	[SerializeField] Image m_backImage;
	[SerializeField] Image m_arrow;
	[SerializeField] float showUpTime = 0.2f;
	[SerializeField] float disappearTime = 3f;

	NarrativeDialog.SpeakerType type;
	public Text m_text;

	[SerializeField] float backImageOriginalAlpha = 1f;

	public void Init( TalkableCharacter _char , NarrativeDialog dialog )
	{
		character = _char;
		type = dialog.type;

		if (m_text != null) {
			m_text.DOKill ();
			m_text.text =  dialog.word;
			// m_text.DOText( dialog.word , dialog.word.Length * 0.06f + 0.3f );
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

		UpdateDialogFramePosition ();
	}


	float timer = 0;
	protected override void MUpdate ()
	{
		base.MUpdate ();
		UpdateDialogArrow ();

		timer += Time.deltaTime;
		if (timer > disappearTime) {
			Disappear ();
		}

		if (CrossPlatformInputManager.GetButtonDown ("SkipDialog")) {
			Disappear ();
		}
	}

	bool isDisappeared = false;
	void Disappear()
	{
		if (!isDisappeared) {
			isDisappeared = true;
			if (m_text != null) {
				m_text.DOKill ();
				m_text.DOFade (0, showUpTime);
			}
			if (m_backImage != null) {
				m_backImage.DOKill ();
				m_backImage.DOFade (0, showUpTime).SetDelay (0.1f);
				m_backImage.transform.DOScale (0, showUpTime).OnComplete (delegate() {
					Destroy(gameObject);
				});
			}
			if (m_arrow != null) {
				m_arrow.DOKill ();
				m_arrow.DOFade (0, showUpTime * 0.2f);
			}

			LogicArg arg = new LogicArg (this);
			arg.AddMessage (M_Event.EVENT_END_DISPLAY_SENDER, character);
			M_Event.FireLogicEvent (LogicEvents.EndDisplayDialog, arg );
		}
	}


	void UpdateDialogFramePosition()
	{

		Vector3 characterPos = (type == NarrativeDialog.SpeakerType.MainCharacter) ? 
			MainCharacter.Instance.GetInteractiveCenter() :
			character.GetInteractCenter ();
		
		Vector3 screenPos = Camera.main.WorldToViewportPoint ( characterPos );

		RectTransform CanvasRect = UIManager.Instance.UICanvas.GetComponent<RectTransform> ();
		Vector2 WorldObject_ScreenPosition = new Vector2 (
			((screenPos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
			((screenPos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

		if (WorldObject_ScreenPosition.x < -500f)
			WorldObject_ScreenPosition.x = -500f;
		if (WorldObject_ScreenPosition.x > 500f)
			WorldObject_ScreenPosition.x = 500f;
		dialogFrame.anchoredPosition = WorldObject_ScreenPosition;

	}

	void UpdateDialogArrow()
	{
		
		Vector3 characterPos = (type == NarrativeDialog.SpeakerType.MainCharacter) ? 
			MainCharacter.Instance.GetInteractiveCenter() :
			character.GetInteractCenter ();

		Vector3 screenPos = Camera.main.WorldToViewportPoint ( characterPos - TalkableCharacter.InteractionPointOffset );

		RectTransform CanvasRect = UIManager.Instance.UICanvas.GetComponent<RectTransform> ();
		Vector2 targetPos = new Vector2 (
			((screenPos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
			((screenPos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

		float screenWidth = UIManager.Instance.UICanvas.pixelRect.width;
		float screenHeight = UIManager.Instance.UICanvas.pixelRect.height;

		if (targetPos.x > screenWidth / 2f || targetPos.x < -screenWidth / 2f || targetPos.y > screenHeight / 2f || targetPos.y < -screenHeight / 2f)
			Disappear ();


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


	}
}
