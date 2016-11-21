using UnityEngine;
using System.Collections;

public class ColorfulStreetSitPerson : TalkableCharacter {
	[SerializeField] NarrativePlotScriptableObject[] reactInteractPlot;

	public void ReactToInteract()
	{

		if (reactInteractPlot != null)
			StartCoroutine (DisplayDelay (Random.Range (0, 0.5f)));
	}

	IEnumerator DisplayDelay( float delay )
	{
		yield return new WaitForSeconds (delay);
		DisplayDialog (reactInteractPlot[Random.Range(0,reactInteractPlot.Length)]);
	}
}
