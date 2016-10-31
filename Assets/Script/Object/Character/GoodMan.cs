using UnityEngine;
using System.Collections;

public class GoodMan : TalkableCharacter {

	[SerializeField] AudioClip showUpSound;
	[SerializeField] float ShowUpOffset = 2f ;
	[SerializeField] int hurtBeforeShowUp = 0;
	[SerializeField] NavMeshAgent agent;
	[SerializeField] Vector3 leadDes;
	[SerializeField] Vector3 goAwayDes;
	[SerializeField] NarrativePlotScriptableObject goAwayPlot;

	public enum GoodManState
	{
		Hide,
		ShowUp,
		Talk,
		Lead,
		LeadAfterTalk,
		GoodBye,
		Leave,
	}

	AStateMachine<GoodManState,LogicEvents> m_stateMachine = new AStateMachine<GoodManState, LogicEvents>();
	public GoodManState State{ get { return m_stateMachine.State; } }
	private bool isTalkedSub = false;

	protected override void MAwake ()
	{
		base.MAwake ();
		agent.enabled = false;

		InitStateMachine ();
	}


	void InitStateMachine()
	{

		m_stateMachine.State = GoodManState.Hide;

		m_stateMachine.AddEnter (GoodManState.ShowUp, ShowUp);
		m_stateMachine.AddEnter (GoodManState.Lead, delegate() {

			agent.enabled = true;
			agent.destination = leadDes;

			MainCharacter.Instance.transform.SetParent (transform);
		});

		m_stateMachine.BlindStateChangeEvent (LogicEvents.EndDisplayDialog, GoodManState.Lead, GoodManState.LeadAfterTalk);

		m_stateMachine.AddUpdate (GoodManState.LeadAfterTalk, delegate() {
			if ( ( transform.position -	leadDes ).magnitude < 0.2f )
			{
				m_stateMachine.State = GoodManState.GoodBye;
			}
		});


		m_stateMachine.AddEnter (GoodManState.GoodBye, delegate() {
			DisplayDialog(goAwayPlot);
		});

		m_stateMachine.BlindStateChangeEvent (LogicEvents.EndDisplayDialog, GoodManState.GoodBye, GoodManState.Leave);

		m_stateMachine.AddEnter (GoodManState.Leave, delegate() {

			agent.destination = goAwayDes;
			M_Event.FireLogicEvent(LogicEvents.GoodManLeave,new LogicArg(this));
			MainCharacter.Instance.transform.SetParent(null);
		});
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.BeginDamage] += OnBeginDamage;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.BeginDamage] -= OnBeginDamage;

	}

	static int hurtTime = 0;
	void OnBeginDamage( LogicArg arg )
	{
		if ((int)LogicManager.Instance.State >= (int)LogicManager.GameState.GoWithGoodMan) {
			hurtTime++;
		}
		if (hurtTime >= hurtBeforeShowUp && State == GoodManState.Hide ) {
			m_stateMachine.State = GoodManState.ShowUp;
		}
	}

	void ShowUp()
	{

		AudioSource source = gameObject.AddComponent<AudioSource> ();
		source.spatialBlend = 1f;
		source.clip = showUpSound;
		source.Play ();

		Vector3 pos = MainCharacter.Instance.transform.position + Vector3.ProjectOnPlane( Camera.main.transform.forward * -1f * ShowUpOffset , Vector3.up );
		transform.position = pos;
	}

	protected override void OnEndDisplayDialog (LogicArg arg)
	{
		if (isMainTalking) {
			if (State == GoodManState.ShowUp) {
				m_stateMachine.State = GoodManState.Lead;
			}
		}else if ( IsTalking ) {
			m_stateMachine.OnEvent (arg.type);
		}
		base.OnEndDisplayDialog (arg);
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		m_stateMachine.Update ();
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere (leadDes, 0.5f);

		Gizmos.color = Color.red;
		Gizmos.DrawSphere (goAwayDes, 0.5f);
	}

	void OnGUI()
	{
	}
}
