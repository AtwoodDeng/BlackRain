using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FilmController : MonoBehaviour {

	public enum ShootType
	{
		Normal,
		RelativeToMainCharacter,
		RelativeToThisCharacter,
	}

	public enum ShootPerspectiveType
	{
		Normal,
		ToMainCharacter,
		ToThisCharacter,
		ToMiddle,
	}

	public enum SoundTargetType
	{
		Others,
		MainCharacter,
		Girl,
	}

	[System.Serializable]
	public class Shoot{
		
		[SerializeField] ShootType type;
		[SerializeField] ShootPerspectiveType perspectiveType;
		[SerializeField] Camera targetCamera;
		[SerializeField] float duration;
		[SerializeField] float moveTime;
//		[SerializeField] AudioClip sound;
//		[SerializeField] GameObject soundTarget;
//		[SerializeField] SoundTargetType soundTargetType;
//		[SerializeField] float soundDelay = -1f;
		[SerializeField] IconNarrativeDialog iconNarrative;
		[SerializeField] LogicEvents endEvent;

		public void PlaySoundOnTarget()
		{
//			if (sound != null) {
//				if (soundTargetType == SoundTargetType.MainCharacter)
//					soundTarget = MainCharacter.Instance.gameObject;
//				if (soundTarget == null)
//					soundTarget = MainCharacter.Instance.gameObject;
//				if (soundDelay < 0)
//					soundDelay = moveTime;
//				AudioManager.PlaySoundOn (sound, soundTarget, soundDelay);
//			}
		}

		public void DisplayIconNarrative( float delay , float duration )
		{
			if (iconNarrative.icon != NarrativeIcon.None) {
				LogicArg arg = new LogicArg (this);
				if (iconNarrative.delay < 0)
					iconNarrative.delay = delay;
				if (iconNarrative.duration < 0)
					iconNarrative.duration = duration;
				arg.AddMessage (M_Event.EVENT_ICON_NARRATIV_DIALOG, iconNarrative);
				M_Event.FireLogicEvent (LogicEvents.DisplayIconDialog, arg);	
			}
		}

		public Vector3 GetTargetPosition( GameObject thisCharacter  )
		{
			if (targetCamera != null) {
				if (type == ShootType.Normal) {
					return targetCamera.transform.position;
				} else if (type == ShootType.RelativeToMainCharacter) {
					targetCamera.transform.position = targetCamera.transform.localPosition + MainCharacter.Instance.transform.position;
					return targetCamera.transform.position;
				} else if (type == ShootType.RelativeToThisCharacter) {
					targetCamera.transform.position = targetCamera.transform.localPosition + thisCharacter.transform.position;
					return targetCamera.transform.position;
				}
			}
			return Vector3.zero;
		}

		public Vector3 GetTargetAngel( GameObject thisCharacter )
		{
			if (targetCamera != null) {
				if (perspectiveType == ShootPerspectiveType.Normal) {
					return targetCamera.transform.eulerAngles;
				} else if (perspectiveType == ShootPerspectiveType.ToMainCharacter) {
					targetCamera.transform.LookAt (MainCharacter.Instance.transform.position);
					return targetCamera.transform.eulerAngles;
				} else if (perspectiveType == ShootPerspectiveType.ToThisCharacter) {
					targetCamera.transform.LookAt (thisCharacter.transform.position);
					return targetCamera.transform.eulerAngles;
				} else if (perspectiveType == ShootPerspectiveType.ToMiddle) {
					targetCamera.transform.LookAt ( (thisCharacter.transform.position + MainCharacter.Instance.transform.position) / 2f);
					return targetCamera.transform.eulerAngles;
				}
			}

			return Vector3.zero;
		}

		public float GetTargetFOV()
		{

			if (type == ShootType.Normal && targetCamera != null)
				return targetCamera.fieldOfView;

			return 60f;
		}

		public float GetMoveTime()
		{
			return moveTime;
		}

		public float GetDuration()
		{
			return duration;
		}

		public LogicEvents GetEndEvent()
		{
			return endEvent;
		}


	}
	[SerializeField] LogicEvents startEvent;
	[SerializeField] List<Shoot> shootList = new List<Shoot>();
	[SerializeField] float backTime = -1f;
	[ReadOnlyAttribute] Vector3 originalPos;
	[ReadOnlyAttribute] Vector3 originalRotation;
	[ReadOnlyAttribute] float originalFOV;

	public void Work()
	{
		// make the camera focus
		M_Event.FireLogicEvent (LogicEvents.FocusCamera, new LogicArg (this));
		if (startEvent != LogicEvents.None)
			M_Event.FireLogicEvent (startEvent, new LogicArg (this));

		originalPos = MainCharacter.MainCameara.transform.position;
		originalRotation = MainCharacter.MainCameara.transform.eulerAngles;
		originalFOV = MainCharacter.MainCameara.fieldOfView;
		Sequence seq = DOTween.Sequence ();
		for (int i = 0; i < shootList.Count; ++i) {
			Shoot s = shootList [i];
			seq.AppendCallback (delegate() {
				s.DisplayIconNarrative(s.GetMoveTime() , s.GetDuration() );
			});
			seq.Append (MainCharacter.MainCameara.transform.DOMove (s.GetTargetPosition(gameObject), s.GetMoveTime()));
			seq.Join (MainCharacter.MainCameara.transform.DORotate (s.GetTargetAngel(gameObject), s.GetMoveTime()));
			seq.Join (MainCharacter.MainCameara.DOFieldOfView (s.GetTargetFOV(), s.GetMoveTime()));
			seq.AppendInterval (s.GetDuration());
			if (s.GetEndEvent() != LogicEvents.None) {
				seq.AppendCallback (delegate {
					M_Event.FireLogicEvent(s.GetEndEvent(),new LogicArg(this));	
				});
			}
		}

		if (backTime > 0) {
			seq.Append (MainCharacter.MainCameara.transform.DOMove (originalPos, backTime));
			seq.Join (MainCharacter.MainCameara.transform.DORotate (originalRotation , backTime));
			seq.Join (MainCharacter.MainCameara.DOFieldOfView (originalFOV,backTime));
		}
		seq.AppendCallback (delegate {
			M_Event.FireLogicEvent(LogicEvents.UnfocusCamera, new LogicArg(this));
		});
	}

}
