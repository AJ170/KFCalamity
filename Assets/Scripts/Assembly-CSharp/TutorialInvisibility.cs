using UnityEngine;

public class TutorialInvisibility : MonoBehaviour
{
	public float InvisibilityTimer;

	public bool StartsAsInvisible = true;

	private Health health;

	private float timer;

	private void Awake()
	{
		health = GetComponent<Health>();
		health.Invulnerable = StartsAsInvisible;
	}

	private void Start()
	{
		StartTimer();
	}

	private void Update()
	{
		if (timer > 0f)
		{
			timer = Mathf.Max(0f, timer - Time.deltaTime);
			if (timer == 0f)
			{
				ToggleInvisibilty();
				timer = -1f;
			}
		}
	}

	public void ToggleInvisibilty()
	{
		health.Invulnerable = !health.Invulnerable;
	}

	public void StartTimer()
	{
		timer = InvisibilityTimer;
	}
}
