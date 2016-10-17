using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class MPeople : MObject {

	private NavMeshAgent m_agent;
	public NavMeshAgent Agent{ get { 
			if (m_agent == null)
				m_agent = GetComponent<NavMeshAgent> ();
			return m_agent; } }

}
