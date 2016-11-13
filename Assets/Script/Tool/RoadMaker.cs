using UnityEngine;
using System.Collections;
//using UnityEditor;

[ExecuteInEditMode]
public class RoadMaker : MonoBehaviour {

//	[SerializeField] GameObject roadPrefab;
//
//	[SerializeField] bool Reset = false;
//	Vector3 lastPoint = Vector3.zero;
//	int index = 0;
//
//	void OnEnable() {
//		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
//	}
//
//	void OnDisable() {
//		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
//	}
//
//	void OnSceneGUI(SceneView sceneView) {
//		Handles.BeginGUI();
//
//		if (Reset == true) {
//			lastPoint = Vector3.zero;
//			index = 0;
//			Reset = false;
//		}
//
//		Event e = Event.current;
//		if (e.type == EventType.MouseDown && e.button == 0 && e.control ) {
//			Ray r = Camera.current.ScreenPointToRay (new Vector3 (e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
//
//			RaycastHit hitInfo;
//
//			if (Physics.Raycast(r,out hitInfo)) {
//
//				Vector3 toPos = hitInfo.point;
//				if (lastPoint != Vector3.zero) {
//					if (roadPrefab != null) {
//						GameObject newRoad = Instantiate (roadPrefab) as GameObject;
//						newRoad.name = "Road" + index.ToString ();
//						index++;
//						newRoad.transform.SetParent (transform);
//						Vector3 pos = (lastPoint + toPos) / 2f;
//						pos.y = 0;
//						newRoad.transform.position = pos;
//						Vector3 scale = newRoad.transform.localScale;
//						scale.x = (lastPoint - toPos).magnitude + 0.5f;
//						newRoad.transform.localScale = scale;
//						Vector3 toward = (toPos - lastPoint).normalized;
//						newRoad.transform.right = toward;
//
//					}
//				}
//
//				lastPoint = toPos;
//			
//			}
//		}
//
//		Handles.EndGUI();    
//	}
//
//	void OnDrawGizmosSelected()
//	{
//		Gizmos.color = Color.green;
//		Gizmos.DrawWireSphere (lastPoint,1f);
//	}
}
