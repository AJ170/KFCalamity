using UnityEngine;

public class Bloodhound : MonoBehaviour
{
	public bool activate;

	private void Update()
	{
		if (activate)
		{
			Search();
			activate = false;
		}
	}

	private void Search()
	{
		UIPanel[] array = Object.FindObjectsOfType(typeof(UIPanel)) as UIPanel[];
		UIPanel[] array2 = array;
		foreach (UIPanel uIPanel in array2)
		{
			Debug.Log("x " + uIPanel.gameObject.name, uIPanel.gameObject);
		}
	}
}
