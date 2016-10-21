using UnityEngine;
using System.Collections;

public class NormalPasserBy : TalkableCharacter {

	[SerializeField] NavMeshAgent m_agent;

	public void SetSpeed( float speed )
	{
		if (m_agent != null)
			m_agent.speed = speed;
	}

	public void SetTarget( Vector3 target )
	{
		if (m_agent != null) {
			Debug.Log ("Set des" + target);
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
