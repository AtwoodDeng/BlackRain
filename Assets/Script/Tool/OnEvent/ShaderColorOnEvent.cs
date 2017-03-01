using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShaderColorOnEvent : OnEventResponsor {

	[SerializeField] Renderer targetRender;
	[SerializeField] Color setColorTo;
	[SerializeField] string property = "_MainColor";
	[SerializeField] float duration = 0 ;
	[SerializeField] float delay = 0;

	void Awake()
	{
		if (targetRender == null)
			targetRender = GetComponent<Renderer> ();
	}

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);

		if (targetRender != null) {
			if (string.IsNullOrEmpty (property))
				targetRender.material.DOColor (setColorTo, duration).SetDelay(delay);
			else
				targetRender.material.DOColor (setColorTo, property, duration).SetDelay(delay);
		}
	}
}
