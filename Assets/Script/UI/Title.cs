using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Title : MBehavior {

	[SerializeField] Text title;
	[SerializeField] Image white;
	[SerializeField] Image[] logos;
	[SerializeField] float logoShowTime = 5f;
	[SerializeField] float fadeTime = 2f;

	protected override void MStart ()
	{
		base.MStart ();
		foreach (Image logo in logos) {
			logo.DOFade (0, 0);
			logo.gameObject.SetActive (true);
		}
		white.gameObject.SetActive(true);
	}

	float timer = 0;
	int index = 0;
	protected override void MUpdate ()
	{
		base.MUpdate ();

		if (index <= logos.Length) {
			timer -= Time.deltaTime;
			if (timer < 0 || Input.GetMouseButtonUp (0) || Input.GetMouseButtonUp (1)) {
				timer = logoShowTime;
				if (index > 0)
					logos [index - 1].DOFade (0, fadeTime);
				if (index < logos.Length)
					logos [index].DOFade (1f, fadeTime);
				else
					white.DOFade (0, fadeTime);
				index++;
			}
		}
	}

	public void StartGame()
	{

		white.DOFade (1f, fadeTime).OnComplete (delegate {
			SceneManager.LoadSceneAsync ("Main");
		});
	}
}
