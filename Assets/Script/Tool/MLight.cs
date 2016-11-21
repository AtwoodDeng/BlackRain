using UnityEngine;
using System.Collections;
using DG.Tweening;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class MLight : MBehavior {

	[SerializeField] Light m_light;
	[SerializeField] Color color;
	static Color lightColor = new Color( 83f / 255f , 219f / 255f , 255f / 255f );
	static Color oriangeColor = new Color (255f / 255f, 165f / 255f, 0 / 255f);
	float colorChangeInterval = 6f;

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (LogicEvents.EnterStreetColorful, EnterStreetColorful);
	}

	protected override void MOnDisable ()
	{
		base.MOnEnable ();
		M_Event.UnregisterEvent (LogicEvents.EnterStreetColorful, EnterStreetColorful);
	}

	void EnterStreetColorful( LogicArg arg)
	{
		Sequence seq = DOTween.Sequence ();
		seq.Append (m_light.DOColor (oriangeColor, colorChangeInterval));
		seq.Append (m_light.DOColor (Color.Lerp(oriangeColor,Color.black, 0.5f ), colorChangeInterval).SetLoops (999, LoopType.Yoyo).SetEase(Ease.InOutCirc) );
	}

	protected override void MStart ()
	{
		base.MStart ();
		if (m_light == null)
			m_light = gameObject.GetComponent<Light> ();
//		if (Application.isEditor && !Application.isPlaying) {
//			color = lightColor;
//			m_light.color = lightColor;
//		}
	}

	Color lastColor;
	protected override void MUpdate ()
	{
		if (Application.isEditor && !Application.isPlaying) {
//			if (lastColor != color) {
//				lightColor = color;
//			}
			m_light.color = lightColor;
//			color = lightColor;
//			lastColor = color;
		}
		base.MUpdate ();
	}
}
