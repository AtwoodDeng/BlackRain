using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class InteractTips : MBehavior {

	[SerializeField] Image tipsButton;
	[SerializeField] Image tipsBackground;
	[SerializeField] Text tipsText;
	[SerializeField] Button confirmButton;
	[SerializeField] float animateTime = 0.5f;

	float backAlpha;
	protected override void MAwake ()
	{
		base.MAwake ();
		backAlpha = tipsBackground.color.a;

		Hide ();
	}

	public void UpdateTips( Interactable interact)
	{
		if (interact != null) {
			tipsText.text = interact.GetInteractTips ();
			tipsBackground.rectTransform.sizeDelta = new Vector2 ( Mathf.Max( 125f , tipsText.text.Length * 15f + 50f), 55f); 
		}
	}

	public void Show( Interactable interact)
	{
		UpdateTips (interact);
		tipsButton.DOFade (1f, animateTime);
		tipsBackground.DOFade (backAlpha, animateTime);
		tipsText.DOFade (1f, animateTime);
		confirmButton.image.DOFade (1f, animateTime);
	}

	public void Hide()
	{
		tipsButton.DOFade (0, animateTime);
		tipsBackground.DOFade (0, animateTime);
		tipsText.DOFade (0, animateTime);
		confirmButton.image.DOFade (0, animateTime);
	}
}
