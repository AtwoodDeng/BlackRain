using UnityEngine;
using System.Collections;

public class MaterialToLight : MBehavior {

	[SerializeField] Light light;

	Material m_material;

	protected override void MStart ()
	{
		base.MStart ();

		MeshRenderer render = GetComponent<MeshRenderer> ();
		if (render != null) {
			m_material = new Material (render.material.shader);
			render.material = m_material;
		}
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		m_material.color = light.color;
	}
}
