using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacter : MBehavior {
	
	[System.Serializable]
	public class RenderSetting
	{
		[ReadOnlyAttribute] public MeshRenderer umbrellaUp;
		[ReadOnlyAttribute] public MeshRenderer umbrellaDown;
		[ReadOnlyAttribute] public MeshRenderer umbrellaShadow;
		[ReadOnlyAttribute] public MeshRenderer head;
		[ReadOnlyAttribute] public MeshRenderer body;
		public Gradient UmbrellaColor;
		public Gradient bodyColor;
		public bool UseColorfulUmbrella;
		public bool UseColorfulSkin;
	}
	[SerializeField] RenderSetting renderSetting;
	[ReadOnlyAttribute] public Transform Umbrella;
	[ReadOnlyAttribute] public Transform WholeBody;
	[ReadOnlyAttribute] public Animator m_animator;
	[ReadOnlyAttribute] public Collider m_bodyCollider;

	protected override void MAwake ()
	{
		base.MAwake ();

		// set up render settings
		MeshRenderer[] renders = GetComponentsInChildren<MeshRenderer> ();

		if (renderSetting.umbrellaUp == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.EndsWith ("top1"))
					renderSetting.umbrellaUp = r;
			}
		}

		if (renderSetting.umbrellaDown == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.EndsWith ("top2"))
					renderSetting.umbrellaDown = r;
			}
		}

		if (renderSetting.umbrellaShadow == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.EndsWith ("Shadow"))
					renderSetting.umbrellaShadow = r;
			}
		}

//		if (renderSetting.umbrellaShadow != null)
//			renderSetting.umbrellaShadow.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;


		if (renderSetting.head == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.Equals ("Head"))
					renderSetting.head = r;
			}
		}

		if (renderSetting.body == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.Equals ("body"))
					renderSetting.body = r;
			}
		}

		if (renderSetting.UseColorfulUmbrella) {
			Color umbrellaColor = renderSetting.UmbrellaColor.Evaluate (Random.Range (0, 1f));
			umbrellaColor.a = 0.3f;
			renderSetting.umbrellaUp.material = new Material (renderSetting.umbrellaUp.material.shader);
			renderSetting.umbrellaUp.material.SetColor ("_Color", umbrellaColor);
			renderSetting.umbrellaDown.material = renderSetting.umbrellaUp.material;
		}

		if (renderSetting.UseColorfulSkin) {
			Color bodyColor = renderSetting.bodyColor.Evaluate (Random.Range (0, 1f));// * Mathf.LinearToGammaSpace(0.35f);
			renderSetting.head.material = new Material (renderSetting.head.material.shader);
			if ( renderSetting.head.material.HasProperty( "_EmissionColor" ) )
				renderSetting.head.material.SetColor ("_EmissionColor", bodyColor);
			if ( renderSetting.head.material.HasProperty( "_MainColor" ) )
				renderSetting.head.material.SetColor ("_MainColor", bodyColor);
			if ( renderSetting.head.material.HasProperty( "_Color" ) )
				renderSetting.head.material.SetColor ("_Color", bodyColor);
			
			renderSetting.body.material = renderSetting.head.material;
		}

		Transform[] transforms = gameObject.GetComponentsInChildren<Transform> ();
		foreach (Transform trans in transforms) {
			if (trans.name.StartsWith ("UMBRELLA:group"))
				Umbrella = trans;
			if (trans.name.EndsWith ("Character"))
				WholeBody = trans;
			if (trans.name.EndsWith ("body")) {
				if (trans.GetComponent<Collider> () != null)
					m_bodyCollider = trans.GetComponent<Collider> ();
			}
		}

		m_animator = gameObject.GetComponent<Animator> ();
		if ( m_animator == null )
			m_animator = gameObject.GetComponentInChildren<Animator> ();
	}


	public void SetTrigger( string trigger )
	{
		m_animator.SetTrigger (trigger);
	}
}
