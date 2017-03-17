using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using UnityEngine;
using DG.Tweening;

public class BlurOnEvent : OnEventResponsor {
	[SerializeField] BlurOptimized blurEffect;
	[SerializeField] float duration = 3f;
	[SerializeField] float delay;
	[SerializeField] Ease easeType;
	[SerializeField] float blurRateTo = 1f;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);

		DOTween.To (() => blurEffect.rate, (x) => blurEffect.rate = x, blurRateTo, duration).SetDelay (delay).SetEase (easeType);

	}
}
