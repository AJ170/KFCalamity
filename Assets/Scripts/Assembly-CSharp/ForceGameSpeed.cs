using UnityEngine;

public class ForceGameSpeed : MonoBehaviour
{
	public float GameSpeed = 1f;

	private void Update()
	{
		GameSpeed = Mathf.Clamp01(GameSpeed);
		if (Time.timeScale != GameSpeed)
		{
			Debug.Log("altering time scale " + Time.timeScale + " " + GameSpeed);
			Time.timeScale = GameSpeed;
		}
	}
}
