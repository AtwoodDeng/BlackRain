using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class BlockTrigger : MBehavior {

	[SerializeField]LogicManager.GameState beginState;
	[SerializeField]LogicManager.GameState endState;

	[SerializeField] ThoughtScritableObject blockThought;
	[SerializeField] ThoughtScritableObject enterThought;

	Collider m_collider;

	protected override void MAwake ()
	{
		base.MAwake ();

		gameObject.layer = LayerMask.NameToLayer ("PlayerTrigger");
	}

	protected override void MStart ()
	{
		base.MStart ();
		m_collider = GetComponent<Collider> ();
		m_collider.isTrigger = !( LogicManager.Instance.State >= beginState && LogicManager.Instance.State < endState ) ;


		LogicManager.Instance.RegisterStateChange (delegate(LogicManager.GameState fromState, LogicManager.GameState toState) {
			m_collider.isTrigger = !( toState >= beginState && toState < endState ) ;
		});
	}

	float lastSendThought = 0;
	void OnCollisionEnter( Collision col )
	{
		if (col.collider.tag == "Player") {
			if (blockThought != null && ( Time.time - lastSendThought ) > 5f ) {
				Debug.Log ("Block Send Event " + blockThought.Thought.word.word);
				ThoughtManager.Instance.SendThought (blockThought.Thought);
				lastSendThought = Time.time;
			}
		}

	}

	void OnTriggerEnter( Collider col )
	{
		if ( col.tag == "Player")
		{
			if ( enterThought != null) {
				ThoughtManager.Instance.SendThought (enterThought.Thought);
			}
		}
	}

}
