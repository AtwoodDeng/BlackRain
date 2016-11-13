using UnityEngine;
using System.Collections;

public class StreetFourPeople : TalkableCharacter {


	[System.Serializable]
	public struct RenderSetting
	{
		public MeshRenderer umbrellaUp;
		public MeshRenderer umbrellaDown;
		public MeshRenderer umbrellaShadow;
		public MeshRenderer head;
		public MeshRenderer body;
		public Gradient UmbrellaColor;
		public Gradient bodyColor;
		public bool UseColorfulUmbrella;
		public bool UseColorfulSkin;
	}
	[SerializeField] RenderSetting renderSetting;

	protected override void MAwake ()
	{
		base.MAwake ();

		InitRender ();
	}

	public void InitRender()
	{
		MeshRenderer[] renders = GetComponentsInChildren<MeshRenderer> ();

		if (renderSetting.umbrellaUp == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.StartsWith ("UMBRELLA:top1"))
					renderSetting.umbrellaUp = r;
			}
		}
		if (renderSetting.umbrellaUp != null) {
//			if (renderSetting.umbrellaUp.GetComponent<Collider> () == null)
//				renderSetting.umbrellaUp.gameObject.AddComponent<MeshCollider> ();
			renderSetting.umbrellaUp.gameObject.layer = LayerMask.NameToLayer ("Umbrella");
		}

		if (renderSetting.umbrellaDown == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.StartsWith ("UMBRELLA:top2"))
					renderSetting.umbrellaDown = r;
			}
		}

		if (renderSetting.umbrellaDown != null) {
			renderSetting.umbrellaDown.enabled = false;
			if (renderSetting.umbrellaDown.GetComponent<Collider> () == null)
				renderSetting.umbrellaDown.gameObject.AddComponent<MeshCollider> ();
			renderSetting.umbrellaDown.gameObject.layer = LayerMask.NameToLayer ("Umbrella");
		}

		if (renderSetting.umbrellaShadow == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.StartsWith ("UMBRELLA:TopShadow"))
					renderSetting.umbrellaShadow = r;
			}
		}

		if (renderSetting.umbrellaShadow != null)
			renderSetting.umbrellaShadow.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

		if (renderSetting.head == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.StartsWith ("Head"))
					renderSetting.head = r;
			}
		}

		if (renderSetting.body == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.StartsWith ("body"))
					renderSetting.body = r;
			}
		}

		if (renderSetting.UseColorfulUmbrella && renderSetting.umbrellaUp != null ) {
			Color umbrellaColor = renderSetting.UmbrellaColor.Evaluate (Random.Range (0, 1f));
			umbrellaColor.a = 0.3f;
			renderSetting.umbrellaUp.material = new Material (Shader.Find ("AlphaSelfIllum_NoFog"));
			renderSetting.umbrellaUp.material.SetColor ("_Color", umbrellaColor);

		}

		if (renderSetting.UseColorfulSkin && renderSetting.head != null && renderSetting.body != null ) {
			Color bodyColor = renderSetting.bodyColor.Evaluate (Random.Range (0, 1f));// * Mathf.LinearToGammaSpace(0.35f);
			renderSetting.head.material = new Material (renderSetting.head.material.shader);
			renderSetting.head.material.SetColor ("_EmissionColor", bodyColor);
			renderSetting.body.material = renderSetting.head.material;
		}
	}
}
