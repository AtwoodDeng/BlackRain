using UnityEngine;
using System.Collections;

public class NormalPasserBy : TalkableCharacter {

	[SerializeField] NavMeshAgent m_agent;

	[System.Serializable]
	public struct RenderSetting
	{
		public MeshRenderer umbrellaUp;
		public MeshRenderer umbrellaDown;
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
				if (r.name.Equals ("UMBRELLA:top1"))
					renderSetting.umbrellaUp = r;
			}
		}

		if (renderSetting.umbrellaDown == null) {
			foreach ( MeshRenderer r in renders) {	
				if (r.name.Equals ("UMBRELLA:top2"))
					renderSetting.umbrellaDown = r;
			}
		}

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
			renderSetting.umbrellaUp.material = new Material (Shader.Find ("AlphaSelfIllum_NoFog"));
			renderSetting.umbrellaUp.material.SetColor ("_Color", umbrellaColor);
			renderSetting.umbrellaDown.material = renderSetting.umbrellaUp.material;
		}

		if (renderSetting.UseColorfulSkin) {
			Color bodyColor = renderSetting.bodyColor.Evaluate (Random.Range (0, 1f));// * Mathf.LinearToGammaSpace(0.35f);
			renderSetting.head.material = new Material (renderSetting.head.material.shader);
			renderSetting.head.material.SetColor ("_EmissionColor", bodyColor);
			renderSetting.body.material = renderSetting.head.material;
		}
	}

	public void SetSpeed( float speed )
	{
		if (m_agent != null)
			m_agent.speed = speed;
	}

	public void SetTarget( Vector3 target )
	{
		if (m_agent != null) {
			m_agent.destination = target;
		}
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		if ( m_agent == null )
		m_agent = GetComponent<NavMeshAgent> ();
	}

//	void OnDrawGizmosSelected()
//	{
//		Gizmos.color = Color.blue;
//		Vector3 toward = transform.position + velocity;
//		Gizmos.DrawLine (transform.position, toward);
//		Gizmos.DrawSphere (toward, 0.2f);
//	}
}
