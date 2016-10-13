// #define TRIGGER_LOG
using UnityEngine;
using Kit.Extend;

namespace CF.CameraBot
{
	public partial class CameraBot : MonoBehaviour
	{
		#region Core
		private void UpdateOrbitCoordinateData(float vertical, float horizontal, float amount, Preset preset)
		{
			// convert input valus into angle
			amount *= Mathf.Rad2Deg;
			preset.Cache.m_YawClampAngleDelta += horizontal * amount;
			preset.Cache.m_PitchClampAngleDelta += vertical * amount;
		}

		/// <summary>In order to remind angle within range, generate reverse input based on target rotation.</summary>
		private void RelativeTargetRotation()
		{
			// do nothing if user need a static local angle related to chase target.
			if (!activePreset.m_Method.m_IsRelatedAngle)
			{
				// try to remind lastFramePosition, find angle difference.
				Quaternion
					qt1 = activePreset.Cache.m_LastFrameTargetRotation,
					qt2 = activePreset.transform.rotation,
					qt3 = qt1.Inverse() * qt2;
				
				Vector3 diff = qt3.eulerAngles;
				diff.x = ToSemiCircleSigned(diff.x);
				diff.y = ToSemiCircleSigned(diff.y);
				diff.z = ToSemiCircleSigned(diff.z);
				diff *= (activePreset.m_VirtualPosition.m_Camera.m_OppositePolar ? -1f : 1f);
				activePreset.Cache.m_YawClampAngleDelta += diff.y;
				activePreset.Cache.m_PitchClampAngleDelta += diff.x;
			}
		}

		private void UpdateInstanceTransform()
		{
			foreach (Preset preset in PresetList)
			{

				float
					flipCP = (preset.m_VirtualPosition.m_Camera.m_OppositePolar ? -1f : 1f),
					flipCE = (preset.m_VirtualPosition.m_Camera.m_OppositeElevation ? -1f : 1f),
					flipLP = (preset.m_VirtualPosition.m_LookTarget.m_OppositePolar == preset.m_VirtualPosition.m_Camera.m_OppositePolar ? 1f : -1f),
					flipLE = (preset.m_VirtualPosition.m_LookTarget.m_OppositeElevation == preset.m_VirtualPosition.m_Camera.m_OppositeElevation ? 1f : -1f),
					deltaYaw = (preset.Cache.m_YawClampAngleDelta * flipCP),
					deltaPitch = (preset.Cache.m_PitchClampAngleDelta * flipCE);

				preset.Cache.m_YawClampAngleDelta = preset.Cache.m_PitchClampAngleDelta = 0f;

				// camera pivot
				preset.Instance.YawDegree = preset.m_VirtualPosition.m_Camera.m_Coordinates.Yaw -= deltaYaw;
				preset.Instance.PitchDegree = preset.m_VirtualPosition.m_Camera.m_Coordinates.Pitch -= deltaPitch;

				// Look target
				preset.Instance.YawLookAtDegree = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Yaw -= deltaYaw * flipLP;
				preset.Instance.PitchLookAtDegree = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Pitch -= deltaPitch * flipLE;
			}
		}

		/// <summary>Repersent circle degree in +/- 180 degree.</summary>
		/// <remarks>left = 0 ~ -180, right = 0 ~ 180</remarks>
		/// <param name="degree"></param>
		/// <returns></returns>
		public static float ToSemiCircleSigned(float degree)
		{
			// An angle more then half circle, left + right = 180+180=360 degree.
			if (Mathf.Abs(degree) > 180f)
			{
				degree = Mathf.Repeat(180f - degree, 180f) * Mathf.Sign(-degree);
			}
			return degree;
		}
		#endregion
    }
}
