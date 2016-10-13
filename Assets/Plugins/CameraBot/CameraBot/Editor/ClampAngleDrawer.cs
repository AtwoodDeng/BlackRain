#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Kit.Extend;

namespace CF.CameraBot
{
    [CustomPropertyDrawer(typeof(ClampAngle))]
    public class ClampAngleDrawer : PropertyDrawer
    {
  //      private static Texture _angle;
  //      private static Texture Angle { get { if (_angle == null) _angle = Resources.Load("angle") as Texture; return _angle; } }

  //      private GUIContent ClampAngleLabel = new GUIContent("Clamp Angle", Angle, "Camera can only move within those angle.");
  //      private GUIContent ForwardReferenceLabel = new GUIContent("Forward Reference", "Override global forward reference.");
  //      private GUIContent RotationUpwardMethodLabel = new GUIContent("Rotation Reference", "Calculate camera upward, World = Alwary point to sky, Local = Based on camera relative upward, Custom = developer design.");
  //      private GUIContent CustomTransformLabel = new GUIContent("Upward Reference", "Using Custom Transform's upward as camera rotation reference.");
  //      private GUIContent LeftRightLabel = new GUIContent("Left/Right range");
  //      private GUIContent UpDownLabel = new GUIContent("Up/Down range");

  //      private GUIContent ReboundLabel = new GUIContent("Enable Rebound", "Drag camera back to initial position, related to target forward direction.");
  //      private GUIContent ReboundDelayLabel = new GUIContent("Rebound Delay", "Delay second after player stop input.");
  //      private GUIContent ReboundPeriodLabel = new GUIContent("Rebound Period", "A fixed constant time to drag back camera to initial position.");
		//private GUIContent ReboundCurveLabel = new GUIContent("Rebound Curve", "Animation curve for rebound speed.");

		//readonly float lineH = EditorGUIUtility.singleLineHeight, lineS = EditorGUIUtility.standardVerticalSpacing;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Pro version
        }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return 0f;
		}
	}
}
#endif