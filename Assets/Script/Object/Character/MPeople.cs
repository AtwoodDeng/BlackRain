using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class MPeople : MObject {

	private UnityEngine.AI.NavMeshAgent m_agent;
	public UnityEngine.AI.NavMeshAgent Agent{ get { 
			if (m_agent == null)
				m_agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
			return m_agent; } }

}
