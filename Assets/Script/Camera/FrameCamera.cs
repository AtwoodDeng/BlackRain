using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FrameCamera : MBehavior {

	[ReadOnlyAttribute] public Camera m_camera;
	[SerializeField] GirlBusStop girl;
	public enum Type
	{
		Up,
		Down

	}
	public Type type;
	[SerializeField] float fadeDuration;
	protected override void MAwake ()
	{
		base.MAwake ();

		m_camera = GetComponent<Camera> ();
		Hide (0);

	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (LogicEvents.HideFrameCamera, OnHide);
		M_Event.RegisterEvent (LogicEvents.ShowFrameCamera, OnShow);
		M_Event.RegisterEvent (LogicEvents.CompleteFrameCamera, OnComplete);

	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (LogicEvents.HideFrameCamera, OnHide);
		M_Event.UnregisterEvent (LogicEvents.ShowFrameCamera, OnShow);
		M_Event.UnregisterEvent (LogicEvents.CompleteFrameCamera, OnComplete);

	}


	void OnComplete (LogicArg arg)
	{
		Complete (fadeDuration);
	}

	void Complete (float duration)
	{
		m_camera.enabled = true;
		if (type == Type.Up) {

			DOTween.To ( () => m_camera.rect.y , delegate(float x) {
				Rect r = m_camera.rect; r.y = x; m_camera.rect = r;
			} , 0.5f, duration).SetEase (Ease.InOutCubic);
			DOTween.To ( () => m_camera.rect.height , delegate(float x) {
				Rect r = m_camera.rect; r.height = x; m_camera.rect = r;
			} , 0.5f, duration).SetEase (Ease.InOutCubic);

		}

		if (type == Type.Down) {
			DOTween.To(  () => m_camera.rect.height , delegate(float x) {
				Rect r = m_camera.rect; r.height = x; m_camera.rect = r;
			}  , 0.5f , duration).SetEase(Ease.InOutCubic);
		}
	}

	void OnShow (LogicArg arg)
	{
		Show (fadeDuration);
	}

	void Show (float duration)
	{
		m_camera.enabled = true;
		if (type == Type.Up) {
			
			DOTween.To ( () => m_camera.rect.y , delegate(float x) {
				Rect r = m_camera.rect; r.y = x; m_camera.rect = r;
			} , 0.7f, duration).SetEase (Ease.InOutCubic);
			DOTween.To ( () => m_camera.rect.height , delegate(float x) {
				Rect r = m_camera.rect; r.height = x; m_camera.rect = r;
			} , 0.3f, duration).SetEase (Ease.InOutCubic);

		}

		if (type == Type.Down) {
			DOTween.To(  () => m_camera.rect.height , delegate(float x) {
				Rect r = m_camera.rect; r.height = x; m_camera.rect = r;
			}  , 0.3f , duration).SetEase(Ease.InOutCubic);
		}
	}

	void OnHide( LogicArg arg )
	{
		Hide (fadeDuration);
	}

	void Hide (float duration)
	{
		if (type == Type.Up) {
			DOTween.To ( () => m_camera.rect.y , delegate(float x) {
				Rect r = m_camera.rect; r.y = x; m_camera.rect = r;
			} , 0.99f, duration).SetEase (Ease.InOutCubic).OnComplete( delegate() {
				m_camera.enabled = false;});
			DOTween.To ( () => m_camera.rect.height , delegate(float x) {
				Rect r = m_camera.rect; r.height = x; m_camera.rect = r;
			} , 0.01f, duration).SetEase (Ease.InOutCubic);

		}

		if (type == Type.Down) {
			DOTween.To(  () => m_camera.rect.height , delegate(float x) {
				Rect r = m_camera.rect; r.height = x; m_camera.rect = r;
			}  , 0.01f , duration).SetEase(Ease.InOutCubic).OnComplete(delegate() {
				m_camera.enabled = false;	
			});
		}
	}

	[SerializeField] float degreedSpeed = 30f;
	[ReadOnlyAttribute] public float temDegreed = 0;
	[SerializeField] public float radius = 12f;
	Vector3 velocity;
	protected override void MUpdate ()
	{
		base.MUpdate ();

		if ( ( LogicManager.Instance.State == LogicManager.GameState.WalkWithGirlFrame || 
			LogicManager.Instance.State == LogicManager.GameState.WalkWithGirlInDark ) &&
			type == Type.Down )
		{
			Vector3 target = girl.transform.position + new Vector3 (-Mathf.Cos (temDegreed * Mathf.Deg2Rad) * radius , 0 , Mathf.Sin (temDegreed * Mathf.Deg2Rad) * radius / 3f );
			target.y = 1.5f; // + Mathf.Sin (temDegreed * Mathf.Deg2Rad + Mathf.PI / 4f ) * 0.5f ;
			transform.position = Vector3.SmoothDamp (transform.position, target, ref velocity, 0.1f);
			transform.LookAt ( girl.GetViewCenter() );
			temDegreed += degreedSpeed * Time.deltaTime;
		}
	}
}
