using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LenFlareOnEvent : OnEventResponsor {

	[SerializeField] LensFlare LensFlare;
	[SerializeField] Ease easeType;
	[SerializeField] float brightnessTo = 0;
	[SerializeField] float duration = 0.2f;
	[SerializeField] int loopTime = 1;
	[SerializeField] LoopType loopType;
	[SerializeField] bool IsOnAwake = false;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (LensFlare == null)
			LensFlare = GetComponent<LensFlare> ();

		if (IsOnAwake)
			OnEvent (new LogicArg (this));
	}

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);
		DOTween.To (() => LensFlare.brightness, (x) => LensFlare.brightness = x, 0, duration).SetEase (easeType).SetLoops (loopTime, loopType);
	}
}
