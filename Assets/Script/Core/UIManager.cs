using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	public Text DialogText;

	public InteractTips interactableTips;

	protected override void MUpdate ()
	{
		base.MUpdate ();
		UpdateInteractTips ();
	}

	void UpdateInteractTips()
	{
		Interactable interact = InteractManager.Instance.TempInteractable;
		if (interact != null) {
			interactableTips.Show (interact);

			Vector3 screenPos = Camera.main.WorldToViewportPoint (interact.GetInteractCenter ());

			RectTransform CanvasRect = GetComponent<Canvas> ().GetComponent<RectTransform> ();
			Vector2 WorldObject_ScreenPosition=new Vector2(
				((screenPos.x*CanvasRect.sizeDelta.x)-(CanvasRect.sizeDelta.x*0.5f)),
				((screenPos.y*CanvasRect.sizeDelta.y)-(CanvasRect.sizeDelta.y*0.5f)));

			interactableTips.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
		} else {
			interactableTips.Hide ();
		}
	}

	public static Rect RectTransformToScreenSpace( RectTransform transform )
	{
		Vector2 size = Vector2.Scale (transform.rect.size, transform.lossyScale);
		return new Rect (transform.position.x, Screen.height - transform.position.y, size.x, size.y);
	}

}
