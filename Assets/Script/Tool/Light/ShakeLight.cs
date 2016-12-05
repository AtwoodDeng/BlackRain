using UnityEngine;
using System.Collections;

public class ShakeLight : MBehavior {

	[SerializeField] Light m_light;
	[SerializeField] MinMax shakeLightIntense;
	// Use this for initialization
	protected override void MStart ()
	{
		base.MStart ();
		if (m_light == null)
			m_light = GetComponent<Light> ();
	}
	
	// Update is called once per frame
	protected override void MUpdate ()
	{
		base.MUpdate ();
		if (m_light != null) {
			m_light.intensity = Mathf.Lerp( m_light.intensity ,  shakeLightIntense.RandomBetween , 0.33f);
		}
	}
}
