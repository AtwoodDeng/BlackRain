using UnityEngine;
using System.Collections;

public class main : MonoBehaviour {

	LazyClouds ls = null;
	GameObject mainLight = null;
	float rotation = 0;
	void OnGUI () {
		if (ls==null)
			return;

		if (mainLight==null)
			return;

		float dy = 25;
		float y = 1;
		float x2 = 200;

		GUI.Label(new Rect(25,dy*y,100,30), "Cloud scattering");
		ls.LS_CloudScattering = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), ls.LS_CloudScattering, 0.0f, 2.0f);

		GUI.Label(new Rect(25,dy*y,100,30), "Cloud thickness");
		ls.LS_CloudThickness = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), ls.LS_CloudThickness, 0.0f, 2.0f);

		GUI.Label(new Rect(25,dy*y,100,30), "Cloud sharpness");
		ls.LS_CloudSharpness = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), ls.LS_CloudSharpness, 0.0f, 4.0f);

		GUI.Label(new Rect(25,dy*y,100,30), "Shadow depth");
		ls.LS_ShadowScale = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), ls.LS_ShadowScale, 0.0f, 2.0f);

		GUI.Label(new Rect(25,dy*y,100,30), "Cloud intensity");
		ls.LS_CloudIntensity = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), ls.LS_CloudIntensity, 0.0f, 10.0f);

		GUI.Label(new Rect(25,dy*y,100,30), "Cloud scale");
		ls.LS_CloudScale = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), ls.LS_CloudScale, 0.0f, 15.0f);

		GUI.Label(new Rect(25,dy*y,100,30), "Cloud speed");
		ls.LS_CloudTimeScale = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), ls.LS_CloudTimeScale, 0.0f, 100.0f);

		GUI.Label(new Rect(25,dy*y,100,30), "Distance scale");
		ls.LS_DistScale = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), ls.LS_DistScale, 0.0f, 20.0f);
		
		GUI.Label(new Rect(25,dy*y,100,30), "Light rotation");
		rotation = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), rotation, -1, 1);
		mainLight.transform.Rotate (new Vector3(rotation,0,0));
		rotation*=0.95f;
		//mainLight.transform.eulerAngles = e;

	}

	void Update () {
		if (Camera.current != null)
			Camera.current.transform.RotateAround( Camera.current.transform.position, new Vector3(0,1,0), Time.deltaTime*2f);

	}

	void Start () {
		ls = GameObject.Find ("CloudSphere").GetComponent<LazyClouds>();
		mainLight = GameObject.Find ("Sun");
		Debug.Log (ls + " ," + mainLight);
	}
	


}
