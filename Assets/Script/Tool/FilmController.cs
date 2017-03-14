using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Assertions;

public class FilmController : MonoBehaviour {

	public enum ShootPositionType
	{
		Normal,
		RelativeToMainCharacter,
		RelativeToThisCharacter,
		Original,
		NoMove,
		Path,
		FilmController
	}

	public enum ShootPerspectiveType
	{
		Normal,
		ToMainCharacter,
		ToThisCharacter,
		ToMiddle,
		Original,
		NoMove,
		Path,
		FilmController
	}

	public enum SoundTargetType
	{
		Others,
		MainCharacter,
		Girl,
	}

	[System.Serializable]
	public class Shoot{
		
		[SerializeField] ShootPositionType positionType;
		[SerializeField] ShootPerspectiveType perspectiveType;
		[SerializeField] public Camera targetCamera;
		[SerializeField] float duration;
		[SerializeField] float moveTime;
		[SerializeField] RotateMode rotateMode;
		[SerializeField] Ease easeType;
		[SerializeField] IconNarrativeDialog iconNarrative;
		[SerializeField] LogicEvents endEvent;
		[SerializeField] FilmController filmController;
		[ReadOnlyAttribute] public Vector3 originalPosition;
		[ReadOnlyAttribute] public Vector3 originalAngle;
		[ReadOnlyAttribute] public float originalFOV;
		[ReadOnlyAttribute] public Transform temCamera;
		public bool IsPath{
			get {
				return (positionType == ShootPositionType.Path) &&
				(perspectiveType == ShootPerspectiveType.Path) &&
					(targetCamera != null ) &&
				(targetCamera.GetComponent<DOTweenPath> () != null);
			}
		}
		public bool IsFilmController{
			get {
				return (positionType == ShootPositionType.FilmController) &&
				(perspectiveType == ShootPerspectiveType.FilmController) &&
				(filmController != null);
			}
		}

		public void DisplayIconNarrative( float delay , float duration , TalkableCharacter thisCharacter )
		{
			if (iconNarrative.icon != NarrativeIcon.None) {
				LogicArg arg = new LogicArg (this);
				if (iconNarrative.delay < 0)
					iconNarrative.delay = delay;
				if (iconNarrative.duration < 0)
					iconNarrative.duration = duration;
				if (iconNarrative.soundVolumn <= 0)
					iconNarrative.soundVolumn = 0.8f;
				if (iconNarrative.thisCharacter == null) {
					iconNarrative.thisCharacter = thisCharacter;
				}
//				Debug.Log ("Delay " + iconNarrative.delay + " duration " + iconNarrative.duration);
				arg.AddMessage (M_Event.EVENT_ICON_NARRATIV_DIALOG, iconNarrative);
				M_Event.FireLogicEvent (LogicEvents.DisplayIconDialog, arg);	
			}
		}

		public Vector3 GetTargetPosition( TalkableCharacter thisCharacter  )
		{
			temCamera.transform.position = Vector3.zero;
			if (targetCamera != null) {
				if (positionType == ShootPositionType.Normal) {
					temCamera.transform.position = targetCamera.transform.position;
				} else if (positionType == ShootPositionType.RelativeToMainCharacter) {
					temCamera.transform.position = targetCamera.transform.localPosition + MainCharacter.Instance.transform.position;
				} else if (positionType == ShootPositionType.RelativeToThisCharacter) {
					Assert.IsNotNull<TalkableCharacter> (thisCharacter);
					temCamera.transform.position = targetCamera.transform.localPosition + thisCharacter.transform.position;
				} 
			}
			if (positionType == ShootPositionType.Original) {
				temCamera.transform.position = originalPosition;
			} else if (positionType == ShootPositionType.NoMove) {
				temCamera.transform.position = Camera.main.transform.position;
			}
			return temCamera.transform.position;
		}

		public Vector3 GetTargetAngel( TalkableCharacter thisCharacter )
		{
			temCamera.transform.eulerAngles = Vector3.zero;
			if (targetCamera != null) {
				if (perspectiveType == ShootPerspectiveType.Normal) {
					temCamera.transform.eulerAngles = targetCamera.transform.eulerAngles;
				}  
			}
			if (perspectiveType == ShootPerspectiveType.ToMainCharacter) {
				temCamera.transform.LookAt (MainCharacter.Instance.GetViewCenter ());
			} else if (perspectiveType == ShootPerspectiveType.ToThisCharacter) {
				Assert.IsNotNull<TalkableCharacter> (thisCharacter);
//				if (thisCharacter != null)
//					temCamera.transform.LookAt (thisCharacter.GetViewCenter ());
//				else
				temCamera.transform.LookAt (thisCharacter.GetViewCenter());
			} else if (perspectiveType == ShootPerspectiveType.ToMiddle) {
				Assert.IsNotNull<TalkableCharacter> (thisCharacter);
//				if (thisCharacter != null)
//					temCamera.transform.LookAt ((thisCharacter.GetViewCenter () + MainCharacter.Instance.GetViewCenter ()) / 2f);
//				else
				temCamera.transform.LookAt ((thisCharacter.GetViewCenter() + MainCharacter.Instance.GetViewCenter ()) / 2f);
			}else if (perspectiveType == ShootPerspectiveType.Original) {
				temCamera.transform.eulerAngles = originalAngle;
			} else if (perspectiveType == ShootPerspectiveType.NoMove) {
				temCamera.transform.eulerAngles = Camera.main.transform.eulerAngles;
			}

			return temCamera.transform.eulerAngles;
		}

		public float GetTargetFOV()
		{

			if (positionType == ShootPositionType.Normal && targetCamera != null)
				return targetCamera.fieldOfView;

			return 60f;
		}

		public float GetMoveTime()
		{
			if (IsPath) {
				return targetCamera.GetComponent<DOTweenPath> ().duration;
			}
			return moveTime;
		}

		public float GetDuration()
		{
			if (IsFilmController)
				return GetFilmControllDuration ();
			return duration;
		}

		public LogicEvents GetEndEvent()
		{
			return endEvent;
		}

		public Ease GetEaseType()
		{
			return easeType;
		}
		public RotateMode GetRotateMode()
		{
			return rotateMode;
		}

		public void SetOriginal( Vector3 pos , Vector3 angle , float FOV , Transform temCam)
		{
			originalPosition = pos;
			originalAngle = angle;
			originalFOV = FOV;
			temCamera = temCam;
		}

		public float GetFilmControllDuration()
		{
			if (filmController != null)
				return filmController.GetTotalTime ();
			return 0;
		}

		public void PlayFilmController()
		{
			if (filmController != null)
				filmController.Work ();
		}

	}
	[SerializeField] LogicEvents startEvent;
	[SerializeField] public TalkableCharacter character;
	[SerializeField] List<Shoot> shootList = new List<Shoot>();
	[SerializeField] float backTime = -1f;
	[ReadOnlyAttribute] Vector3 originalPos;
	[ReadOnlyAttribute] Vector3 originalRotation;
	[ReadOnlyAttribute] float originalFOV;

	public float GetTotalTime()
	{
		float totalTime = 0;
		foreach (Shoot shoot in shootList)
			totalTime += shoot.GetDuration () + shoot.GetMoveTime ();
		return totalTime;
	}

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
		if ( character == null )
			character = GetComponent<TalkableCharacter> ();

		GameObject temCamera = new GameObject ();
		for (int i = 0; i < shootList.Count; ++i) {
			Shoot s = shootList [i];
			s.SetOriginal (originalPos, originalRotation, originalFOV, temCamera.transform);
			seq.AppendCallback (delegate() {
				s.DisplayIconNarrative(s.GetMoveTime() , s.GetDuration() , character );
			});
			if (s.IsPath) {
				DOTweenPath path = s.targetCamera.GetComponent<DOTweenPath> ();
				seq.AppendCallback (delegate {
					s.targetCamera.enabled = true;
					Debug.Log ("Path " + path.name);
				});
				path.DOPlay ();
				seq.AppendInterval (path.duration);
				seq.AppendCallback (delegate {
					MainCharacter.MainCameara.transform.position = s.targetCamera.transform.position;
					MainCharacter.MainCameara.transform.rotation = s.targetCamera.transform.rotation;
					MainCharacter.MainCameara.fieldOfView = s.targetCamera.fieldOfView;
					s.targetCamera.enabled = false;	
				});

			} else if (s.IsFilmController) {
				seq.AppendCallback (delegate() {
					s.PlayFilmController ();
					Debug.Log("duration " + s.GetDuration());
					}
				);
			}else {
				seq.Append (MainCharacter.MainCameara.transform.DOMove (s.GetTargetPosition (character), s.GetMoveTime ()).SetEase (s.GetEaseType ()));
				seq.Join (MainCharacter.MainCameara.transform.DORotate (s.GetTargetAngel (character), s.GetMoveTime (), s.GetRotateMode ()).SetEase (s.GetEaseType ()));
				seq.Join (MainCharacter.MainCameara.DOFieldOfView (s.GetTargetFOV (), s.GetMoveTime ()).SetEase (s.GetEaseType ()));
			}
			seq.AppendInterval (s.GetDuration());
			if (s.GetEndEvent() != LogicEvents.None) {
				seq.AppendCallback (delegate {
					M_Event.FireLogicEvent(s.GetEndEvent(),new LogicArg(this));	
				});
			}
		}

		if (backTime > 0) {
			seq.Append (MainCharacter.MainCameara.transform.DOMove (originalPos, backTime));
			seq.Join (MainCharacter.MainCameara.transform.DORotate (originalRotation, backTime));
			seq.Join (MainCharacter.MainCameara.DOFieldOfView (originalFOV, backTime));
		}
		seq.AppendCallback (delegate {
			M_Event.FireLogicEvent(LogicEvents.UnfocusCamera, new LogicArg(this));
			M_Event.FireLogicEvent(LogicEvents.EndFilmControl , new LogicArg(this));
		});
	}

}
