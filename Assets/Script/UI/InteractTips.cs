using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class InteractTips : MBehavior {

	[SerializeField] Image tipsButton;
	[SerializeField] Image tipsBackground;
	[SerializeField] Text tipsText;
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
//			Vector3 scale = tipsBackground.transform.localScale;
//			scale.x = tipsText.text.Length * 0.08f + 0.3f;
//			tipsBackground.transform.localScale = scale;
		}
	}

	public void Show( Interactable interact)
	{
		UpdateTips (interact);
		tipsButton.DOFade (1f, animateTime);
		tipsBackground.DOFade (backAlpha, animateTime);
		tipsText.DOFade (1f, animateTime);
	}

	public void Hide()
	{
		tipsButton.DOFade (0, animateTime);
		tipsBackground.DOFade (0, animateTime);
		tipsText.DOFade (0, animateTime);
	}
}
