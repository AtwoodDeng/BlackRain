using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacter : MBehavior {
	
	[System.Serializable]
	public class RenderSetting
	{
		[ReadOnlyAttribute] public Renderer umbrellaUp;
		[ReadOnlyAttribute] public Renderer umbrellaDown;
		[ReadOnlyAttribute] public Renderer umbrellaShadow;
		[ReadOnlyAttribute] public bool newUmbrellaMesh = false;
		[ReadOnlyAttribute] public Renderer head;
		[ReadOnlyAttribute] public Renderer body;
		[ReadOnlyAttribute] public bool newCharacterMesh = false;
		public Gradient UmbrellaColor;
		public Gradient bodyColor;
		public bool UseColorfulUmbrella;
		public bool UseColorfulSkin;
	}
	[SerializeField] RenderSetting renderSetting;
//	public Transform Umbrella;
//	public Transform WholeBody;
	public Animator m_animator;
	public Collider m_bodyCollider;
//	[SerializeField] public bool isShowShadow = false;
//	public GameObject fakeShadow;

	public void SetMaterial( Material skin , Material umbrella )
	{
		Renderer[] renders = GetComponentsInChildren<Renderer> ();

		if (renderSetting.umbrellaUp == null) {
			foreach ( Renderer r in renders) {	
				if (r.name.EndsWith ("top1"))
					renderSetting.umbrellaUp = r;
			}
		}

		if (renderSetting.umbrellaDown == null) {
			foreach ( Renderer r in renders) {	
				if (r.name.EndsWith ("top2"))
					renderSetting.umbrellaDown = r;
			}
		}

		renderSetting.umbrellaUp.material = umbrella;
		renderSetting.umbrellaDown.material = umbrella;

		if (renderSetting.umbrellaShadow == null) {
			foreach ( Renderer r in renders) {	
				if (r.name.EndsWith ("Shadow"))
					renderSetting.umbrellaShadow = r;
			}
		}


		if (renderSetting.head == null) {
			foreach ( Renderer r in renders) {	
				if (r.name.EndsWith ("Head"))
					renderSetting.head = r;
			}
		}

		if (renderSetting.body == null) {
			foreach ( Renderer r in renders) {	
				if (r.name.EndsWith ("body"))
					renderSetting.body = r;
			}
		}

		renderSetting.head.material = skin;
		renderSetting.body.material = skin;
	}

	protected override void MAwake ()
	{
		base.MAwake ();

		// set up render settings
//		Renderer[] renders = GetComponentsInChildren<Renderer> ();
//
//		if (renderSetting.umbrellaUp == null) {
//			foreach ( Renderer r in renders) {	
//				if (r.name.EndsWith ("top1"))
//					renderSetting.umbrellaUp = r;
//			}
//		}
//
//		if (renderSetting.umbrellaDown == null) {
//			foreach ( Renderer r in renders) {	
//				if (r.name.EndsWith ("top2"))
//					renderSetting.umbrellaDown = r;
//			}
//		}
//
//		if (renderSetting.umbrellaShadow == null) {
//			foreach ( Renderer r in renders) {	
//				if (r.name.EndsWith ("Shadow"))
//					renderSetting.umbrellaShadow = r;
//			}
//		}
//			
//		if (fakeShadow == null) {
//			SpriteRenderer[] sprites = gameObject.GetComponentsInChildren<SpriteRenderer> ();
//			foreach (SpriteRenderer s in sprites)
//				if (s.name.StartsWith ("FakeShadow")) {
//					fakeShadow = s.gameObject;
//					fakeShadow.gameObject.SetActive (false);
//				}
//		}
//
////		if (renderSetting.umbrellaShadow != null)
////			renderSetting.umbrellaShadow.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
//
//
//		if (renderSetting.head == null) {
//			foreach ( Renderer r in renders) {	
//				if (r.name.EndsWith ("Head"))
//					renderSetting.head = r;
//			}
//		}
//
//		if (renderSetting.body == null) {
//			foreach ( Renderer r in renders) {	
//				if (r.name.EndsWith ("body"))
//					renderSetting.body = r;
//			}
//		}
//
//		Transform[] transforms = gameObject.GetComponentsInChildren<Transform> ();
//		foreach (Transform trans in transforms) {
//			if (trans.name.StartsWith ("UMBRELLA:group"))
//				Umbrella = trans;
//			if (trans.name.EndsWith ("Character"))
//				WholeBody = trans;
//			if (trans.name.EndsWith ("body")) {
//				if (trans.GetComponent<Collider> () != null)
//					m_bodyCollider = trans.GetComponent<Collider> ();
//			}
//		}
//
//
//		UpdateColor ();

//		m_animator = gameObject.GetComponent<Animator> ();
//		if ( m_animator == null )
//			m_animator = gameObject.GetComponentInChildren<Animator> ();


//		if (isShowShadow && fakeShadow != null) {
//			fakeShadow.SetActive (true);
//			if (renderSetting.umbrellaShadow != null)
//				renderSetting.umbrellaShadow.enabled = false;
//		}
	}

	public void UpdateColor( Gradient _umbrellaColor = null , Gradient _bodyColor = null )
	{
		if (_umbrellaColor != null)
			renderSetting.UmbrellaColor = _umbrellaColor;
		if (_bodyColor != null)
			renderSetting.bodyColor = _bodyColor;
		
		if (renderSetting.UseColorfulUmbrella  && renderSetting.umbrellaUp != null) {
			Color umbrellaColor = renderSetting.UmbrellaColor.Evaluate (Random.Range (0, 1f));
			umbrellaColor.a = 0.3f;
//			if (!renderSetting.newUmbrellaMesh) {
//				renderSetting.umbrellaUp.material = new Material (renderSetting.umbrellaUp.material.shader);
//				renderSetting.newUmbrellaMesh = true;
//			}


			renderSetting.umbrellaUp.material.SetColor ("_Color", umbrellaColor);
			renderSetting.umbrellaDown.material = renderSetting.umbrellaUp.material;
		}

		if (renderSetting.UseColorfulSkin && renderSetting.head != null ) {
			Color bodyColor = renderSetting.bodyColor.Evaluate (Random.Range (0, 1f));// * Mathf.LinearToGammaSpace(0.35f);
//			if (!renderSetting.newCharacterMesh) {
//				renderSetting.head.material = new Material (renderSetting.head.material.shader);
//				renderSetting.newCharacterMesh = true;
//			}


//			if ( renderSetting.head.material.HasProperty( "_EmissionColor" ) )
//				renderSetting.head.material.SetColor ("_EmissionColor", bodyColor);
//			if ( renderSetting.head.material.HasProperty( "_MainColor" ) )
//				renderSetting.head.material.SetColor ("_MainColor", bodyColor);
//			if ( renderSetting.head.material.HasProperty( "_Color" ) )
				renderSetting.head.material.SetColor ("_Color", bodyColor);

			renderSetting.body.material = renderSetting.head.material;
		}

		
	}


	public void SetTrigger( string trigger )
	{
		m_animator.SetTrigger (trigger);
	}
}
