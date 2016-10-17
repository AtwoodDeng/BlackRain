using UnityEngine;
using System.Collections;

public class LogicManager : MonoBehaviour {

	static LogicManager m_Instance;
	public static LogicManager Instance{ get { return m_Instance;}}

	static MainCharacter m_MainCharacter;
	public static MainCharacter MainCharacter{ get { return m_MainCharacter; } }

	// Use this for initialization
	void Awake () {
		if (m_Instance == null)
			m_Instance = this;
		else
			Destroy (this);

		m_MainCharacter = FindObjectOfType<MainCharacter> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
