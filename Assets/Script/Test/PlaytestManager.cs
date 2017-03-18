using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Analytics;

public class PlaytestManager : MBehavior {
	[SerializeField] bool isUseAnalystic = true;
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

	public enum Choice{
		None,
		Piano,
		Violin,
		Guitar,

	}

	public class StateData
	{
		public string State;
		public float endTime;
		public float duration;
		public float inRainTime;
		public int sneezeTime;
		public float minHealth;
		public float freezeTime;

		public override string ToString ()
		{
			return string.Format ("[StateData] State:{0}, endTime:{1}, duration:{2}, inRainTime:{3}, sneezeTime:{4}, minHealth:{5}, freezeTime:{6}" , State,endTime,duration,inRainTime,sneezeTime,minHealth,freezeTime);
		}
	}



	public class ContentData
	{
		public LogicManager.MusicChoice choice;
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
	ContentData contentDate = new ContentData();


	protected override void MStart ()
	{
		base.MStart ();
		playtester = System.DateTime.Now.ToString ("yy-MM-dd HH-mm");
		LogicManager.Instance.RegisterStateChange (delegate(LogicManager.GameState fromState, LogicManager.GameState toState) {
			if ( fromState != LogicManager.GameState.None )
			{
				StateData st = new StateData();
				st.State = fromState.ToString();
				st.endTime = Time.time;
				st.duration = state_duration;
				st.inRainTime = state_inRainTime;
				st.freezeTime = state_freezeTime;
				st.sneezeTime = state_sneezeTime;
				st.minHealth = state_minHealth;
				Debug.Log(st);
				if ( isUseAnalystic )
				{
					Analytics.CustomEvent("State" , new Dictionary<string,object>
						{
							{"StateName" , st.State },
							{"GameTime" , st.endTime },
							{"Duration" , st.duration },
							{"InRainTime" , st.inRainTime },
							{"SneezeTime" , st.sneezeTime },
							{"minHealth" , st.minHealth},
								
						});
				}
				stateData.Add(st);
			}
			ResetState();
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
		M_Event.logicEvents [(int)LogicEvents.Sneeze] += OnSneeze;
		M_Event.logicEvents [(int)LogicEvents.PlayMusic] += OnPlayMusic;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents[(int)LogicEvents.EndGame] -= OnEnd;
		M_Event.logicEvents[(int)LogicEvents.DisplayNextDialog] -= OnDisplayNext;
		M_Event.logicEvents[(int)LogicEvents.EndDisplayDialog] -= OnDisplayEnd;
		M_Event.logicEvents [(int)LogicEvents.Death] -= OnDeath;
		M_Event.logicEvents [(int)LogicEvents.DeathEnd] -= OnDeathEnd;
		M_Event.logicEvents [(int)LogicEvents.Sneeze] -= OnSneeze;
		M_Event.logicEvents [(int)LogicEvents.PlayMusic] -= OnPlayMusic;
	}

	void OnPlayMusic( LogicArg arg )
	{
		string musicName = (string)arg.GetMessage (M_Event.EVENT_PLAY_MUSIC_NAME);
		if (musicName.StartsWith("Piano"))
			contentDate.choice = LogicManager.MusicChoice.Piano;
		if (musicName.StartsWith("Guitar"))
			contentDate.choice = LogicManager.MusicChoice.Guitar;
		if (musicName.StartsWith("Violin"))
			contentDate.choice = LogicManager.MusicChoice.Violin;

		if (isUseAnalystic) {
			Analytics.CustomEvent("MusicChoice" , new Dictionary<string,object>
				{
					{"Choice" , (int)contentDate.choice },

				});
		}
		
	}

	void OnSneeze( LogicArg arg )
	{
		state_sneezeTime += 1;
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
		bool important = (bool)arg.GetMessage ("important");
		if (important) {
			if (lastData != null) {
				lastData.lastTime = Time.time - lastData.time;
				dialogData.Add (lastData);
				lastData = null;
			}
			lastData = new DialogData ();
			lastData.time = Time.time;
			lastData.word = (string)arg.GetMessage ("word");
			lastData.character = (string)arg.GetMessage ("character");
		}
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

		if (!Directory.Exists (thisFolder)) {
			DirectoryInfo info = Directory.CreateDirectory (thisFolder);
		}

		OutputState ();
//		OutputDialog ();
		OutputMove ();
//		OutputDeath ();
		OutputContent();
	}

	void UpdateContendDate()
	{
		contentDate.choice = LogicManager.Instance.storyData.musicChoice;
	}

	void OutputContent()
	{
		StreamWriter file = new StreamWriter ( thisFolder + "/Content" + thisFolder.Substring(thisFolder.Length-6) +".csv");
		file.WriteLine ("MusicChoice");
		UpdateContendDate ();
		file.WriteLine (contentDate.choice);
		file.Close ();
		
	}


	void OutputState()
	{
		StreamWriter file = new StreamWriter ( thisFolder + "/State" + thisFolder.Substring(thisFolder.Length-6) +".csv");
		file.WriteLine ("State,EndTime,Duration,InRainTime,FreezeTime,SneezeTime,minHealth");
		for (int i = 0; i < stateData.Count; ++i) {
			StateData s = stateData [i];
			file.WriteLine ( s.State + "," + s.endTime + "," + s.duration + ","  + s.inRainTime + ","+ s.freezeTime + ","+ s.sneezeTime + ","+ s.minHealth);
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
	public void UpdateMove()
	{
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

	float state_duration;
	float state_freezeTime;
	float state_minHealth = 1f ;
	int state_sneezeTime;
	float state_inRainTime;

	public void UpdateState(){
		state_duration += Time.deltaTime;
		if (MechanismManager.Instance.DamageState == MechanismManager.DamageStateType.UnderDamage)
			state_inRainTime += Time.deltaTime;
		if (MechanismManager.health.HealthRate < state_minHealth)
			state_minHealth = MechanismManager.health.HealthRate;
		if (MainCharacter.Instance.IsFocus || NarrativeManager.Instance.IsDisplaying)
			state_freezeTime += Time.deltaTime;
	}

	public void ResetState(){
		state_duration = 0;
		state_freezeTime = 0;
		state_minHealth = 1f;
		state_sneezeTime = 0;
		state_inRainTime = 0;
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		UpdateMove ();
		UpdateState ();
	}
}
