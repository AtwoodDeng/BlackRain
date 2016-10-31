using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PlaytestManager : MBehavior {

	private static PlaytestManager m_Instance;
	public static PlaytestManager Instance{
		get {
			if ( m_Instance == null )
				m_Instance = FindObjectOfType<PlaytestManager>();
			return m_Instance; }

	}

	string playtester;
	string thisFolder
	{
		get { return Application.dataPath + "/TestData/" + playtester; }
	}

	const string  LineSep = "\r\n";
	[SerializeField] float moveRecordInterval = 0.5f;

	public class StateData
	{
		public string State;
		public float time;
	}

	public class DialogData
	{
		public string word;
		public string character;
		public float time;
		public float lastTime;
	}

	public class DeathData
	{
		public string State;
		public float time;
		public float lastTime;
	}

	public class MoveIntensity
	{
		public float time;
		public float totalAngle;
		public float totalDistance;
		public Vector3 position;
		public Quaternion rotation;
	}

	private List<StateData> stateData = new List<StateData>();
	private List<DialogData> dialogData = new List<DialogData>();
	private List<DeathData> deathData = new List<DeathData>();
	private List<MoveIntensity> moveData = new List<MoveIntensity>();


	protected override void MStart ()
	{
		base.MStart ();
		playtester = System.DateTime.Now.ToString ("yy-MM-dd HH-mm");
		if (!Directory.Exists (thisFolder)) {
			DirectoryInfo info = Directory.CreateDirectory (thisFolder);
		}	
		LogicManager.Instance.RegisterStateChange (delegate(LogicManager.GameState fromState, LogicManager.GameState toState) {
			StateData st = new StateData();
			st.State = toState.ToString();
			st.time = Time.time;
			stateData.Add(st);
		});

		lastPosition = MainCharacter.Instance.transform.position;
		lastRotation = Camera.main.transform.rotation;
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents[(int)LogicEvents.EndGame] += OnEnd;
		M_Event.logicEvents[(int)LogicEvents.DisplayNextDialog] += OnDisplayNext;
		M_Event.logicEvents[(int)LogicEvents.EndDisplayDialog] += OnDisplayEnd;
		M_Event.logicEvents [(int)LogicEvents.Death] += OnDeath;
		M_Event.logicEvents [(int)LogicEvents.DeathEnd] += OnDeathEnd;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents[(int)LogicEvents.EndGame] -= OnEnd;
		M_Event.logicEvents[(int)LogicEvents.DisplayNextDialog] -= OnDisplayNext;
		M_Event.logicEvents[(int)LogicEvents.EndDisplayDialog] -= OnDisplayEnd;
		M_Event.logicEvents [(int)LogicEvents.Death] -= OnDeath;
		M_Event.logicEvents [(int)LogicEvents.DeathEnd] += OnDeathEnd;
	}

	DeathData temDeath;
	void OnDeath( LogicArg arg )
	{
		temDeath = new DeathData ();
		temDeath.time = Time.time;
		temDeath.State = LogicManager.Instance.State.ToString ();
	}

	void OnDeathEnd( LogicArg arg )
	{
		if (temDeath != null) {
			temDeath.lastTime = Time.time - temDeath.time;
			deathData.Add (temDeath);
		}
	}

	DialogData lastData = null;
	void OnDisplayNext( LogicArg arg )
	{
		if (lastData != null )
		{
			lastData.lastTime = Time.time - lastData.time;
			dialogData.Add (lastData);
			lastData = null;
		}
		lastData = new DialogData ();
		lastData.time = Time.time;
		lastData.word = (string)arg.GetMessage ("word");
		lastData.character = (string)arg.GetMessage ("character");
	}

	void OnDisplayEnd( LogicArg arg )
	{
		Debug.Log ("OnEndDisplay");
		if (lastData != null )
		{
			lastData.lastTime = Time.time - lastData.time;
			dialogData.Add (lastData);
			lastData = null;
		}
		
	}

	void OnEnd( LogicArg arg )
	{
		OutputState ();
		OutputDialog ();
		OutputMove ();
		OutputDeath ();
	}

	void OutputState()
	{
		StreamWriter file = new StreamWriter ( thisFolder + "/State" + thisFolder.Substring(thisFolder.Length-6) +".csv");
		file.WriteLine ("Time,State");
		for (int i = 0; i < stateData.Count; ++i) {
			StateData s = stateData [i];
			file.WriteLine (s.time + "," + s.State   );
		}
		file.Close ();
	}

	void OutputDialog()
	{
		StreamWriter file = new StreamWriter ( thisFolder + "/Dialog" + thisFolder.Substring(thisFolder.Length-6) + ".csv");
		file.WriteLine ("Time,Dialog,Character,LastTime,TimePreWord");
		for (int i = 0; i < dialogData.Count; ++i) {
			DialogData d = dialogData [i];
			string word = d.word;
			word = word.Replace (',', '-');
			file.WriteLine (d.time + "," + word + "," + d.character + "," +  d.lastTime + "," + ( d.lastTime / (d.word.Split(' ').Length + 1)) );
		}
		file.Close ();
	}
	void OutputMove()
	{
		StreamWriter file = new StreamWriter ( thisFolder + "/Move" + thisFolder.Substring(thisFolder.Length-6) + ".csv");
		file.WriteLine ("Time,Distance,angle,PosX,PosZ,AngleX,AngleY,AngleZ");
		for (int i = 0; i < moveData.Count; ++i) {
			MoveIntensity d = moveData [i];
			file.WriteLine (d.time + "," + d.totalDistance + "," + d.totalAngle + "," + d.position.x + "," + d.position.z + "," + 
				d.rotation.eulerAngles.x + "," + d.rotation.eulerAngles.y + "," + d.rotation.eulerAngles.z  );
		}
		file.Close ();
	}

	void OutputDeath()
	{
		StreamWriter file = new StreamWriter ( thisFolder + "/Death" + thisFolder.Substring(thisFolder.Length-6) + ".csv");
		file.WriteLine ("Time,State,LastTime");
		for (int i = 0; i < deathData.Count; ++i) {
			DeathData d = deathData [i];
			file.WriteLine (d.time + "," + d.State + "," + d.lastTime);
		}
		file.Close ();
	}

	float timer = 0;
	Vector3 lastPosition;
	Quaternion lastRotation;
	float accDistance = 0;
	float accAngle = 0;
	protected override void MUpdate ()
	{
		base.MUpdate ();

		timer -= Time.deltaTime;
		if (timer < 0) {
			timer = moveRecordInterval;
			MoveIntensity move = new MoveIntensity ();
			move.totalDistance = accDistance;
			accDistance = 0;
			move.totalAngle = accAngle;
			accAngle = 0;
			move.position = lastPosition;
			move.rotation = lastRotation;
			move.time = Time.time;
			moveData.Add (move);
		}

		accDistance += (MainCharacter.Instance.transform.position - lastPosition).magnitude;
		accAngle += Quaternion.Angle (Camera.main.transform.rotation, lastRotation);

		lastPosition = MainCharacter.Instance.transform.position;
		lastRotation = Camera.main.transform.rotation;
	}
}
