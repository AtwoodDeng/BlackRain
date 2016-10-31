using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

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
	public Image targetBackground;
	public Text targetText;
	public Image screenBlack;
	public Text skipTips;
	public Image cursorImage;
	public Sprite ScanSprite;
	public Sprite normalSprite;



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

	protected override void MOnEnable ()
	{
		base.MOnEnable ();

		M_Event.logicEvents [(int)LogicEvents.Death] += OnDeath;
		M_Event.logicEvents [(int)LogicEvents.DeathEnd] += OnDeathEnd;
		M_Event.logicEvents [(int)LogicEvents.UnlockCamera] += OnUnlockCamera;
		M_Event.logicEvents [(int)LogicEvents.LockCamera] += OnLockCamera;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.Death] -= OnDeath;
		M_Event.logicEvents [(int)LogicEvents.DeathEnd] -= OnDeathEnd;
		M_Event.logicEvents [(int)LogicEvents.UnlockCamera] -= OnUnlockCamera;
		M_Event.logicEvents [(int)LogicEvents.LockCamera] -= OnLockCamera;
	}

	void OnUnlockCamera(LogicArg arg )
	{
		cursorImage.sprite = ScanSprite;
	}

	void OnLockCamera( LogicArg arg )
	{
		cursorImage.sprite = normalSprite;
	}

	void OnDeath( LogicArg arg )
	{
		skipTips.DOFade (1f, 1f);
	}

	void OnDeathEnd(LogicArg arg)
	{
		screenBlack.color = Color.white;
		screenBlack.DOFade (0, 2f).SetDelay(2f);
		skipTips.DOFade (0, 1f);
	}

	protected override void MStart ()
	{
		base.MStart ();
		LogicManager.Instance.RegisterStateChange ( OnStateChange );
		skipTips.DOFade (0, 0);

		Cursor.visible = false;

	}

	void OnStateChange( LogicManager.GameState fromState , LogicManager.GameState toState )
	{
		Debug.Log ("On Change" + toState);
		foreach (StateTargetPair pair in targetList) {
			if (toState == pair.state) {
				targetText.DOKill ();
				targetText.text = "";
				targetText.DOText (pair.target, 2f );

				return;
			}
		}
		targetText.text = "";
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		UpdateInteractTips ();
		UpdateCursor ();
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

		cursorImage.transform.position = InteractManager.FocusPoint;
	}

	public static Rect RectTransformToScreenSpace( RectTransform transform )
	{
		Vector2 size = Vector2.Scale (transform.rect.size, transform.lossyScale);
		return new Rect (transform.position.x, Screen.height - transform.position.y, size.x, size.y);
	}

}
