using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MSceneManager : MBehavior {

	static MSceneManager m_Instance;
	public static MSceneManager Instance{ 
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<MSceneManager> ();
			return m_Instance;}
	}

	[System.Serializable]
	public class SceneInfo
	{
		public LogicManager.GameState startState;
		public LogicManager.GameState endState;
		public string sceneName;
		public string SceneStr{
			get{
				return sceneName;
			}
		}
		[ReadOnlyAttribute] public bool isLoaded;
	}
	public List<SceneInfo> sceneInfos = new List<SceneInfo>();

	protected override void MStart ()
	{
		base.MStart ();

		for (int i = 0; i < SceneManager.sceneCount; ++i) {
//			Debug.Log ("Scene " + SceneManager.GetSceneAt (i).name);
			foreach( SceneInfo info in sceneInfos ) {
				if ( info.SceneStr.Equals( SceneManager.GetSceneAt( i ).name )) {
					info.isLoaded = true;
				}
					}

		}

		UpdateScenes (LogicManager.GameState.None, LogicManager.Instance.State);
		LogicManager.Instance.RegisterStateChange (UpdateScenes);
	}

	public void UpdateScenes(LogicManager.GameState fromState, LogicManager.GameState toState) {
		foreach (SceneInfo info in sceneInfos) {
			
			if (info.startState <= toState && info.endState > toState ) {
				if (!info.isLoaded) {
//					Debug.Log ("Load Scene" + info.SceneStr);
					Debug.Log ("Load Scene from " + fromState + " to " + toState + " info " + info.SceneStr + info.startState + info.endState );
					info.isLoaded = true;
					StartCoroutine (LoadScene (info.SceneStr));
				}
			} else {
				if (info.isLoaded) {
					info.isLoaded = false;
					StartCoroutine (UnloadScene (info.SceneStr));
				}
			}

		}
	}

	IEnumerator LoadScene( string scene )
	{
		AsyncOperation AO = SceneManager.LoadSceneAsync (scene, LoadSceneMode.Additive);
		AO.allowSceneActivation = false;
		while (AO.progress < 0.9f) {
			Debug.Log ("Loading " + scene + " "  + AO.progress);
			yield return new WaitForEndOfFrame ();
		}

		AO.allowSceneActivation = true;
		yield return AO;
	}

	IEnumerator UnloadScene( string scene )
	{
		Debug.Log ("Unload Scene " + scene);
		AsyncOperation AO = SceneManager.UnloadSceneAsync (scene);
		yield return AO;
	}


}
