using UnityEngine;

public class TimerBar : BarBase
{
	public Transform Flash;

	private UISprite flashSprite;

	private TweenColor flashTween;

	private float timeLeft;

	private float totalTime;

	public override void Start()
	{
		base.Start();
		flashSprite = Flash.GetComponent<UISprite>();
		flashTween = Flash.GetComponent<TweenColor>();
		targetNormalizedWidth = 1f;
	}

	public override void Update()
	{
		if (CalculateBar)
		{
			OnBarTimeTick();
		}
	}

	private void startFlash()
	{
		flashSprite.gameObject.SetActive(true);
		flashTween.Play(true);
	}

	public void OnFlashEnd()
	{
		flashTween.Reset();
		flashSprite.gameObject.SetActive(false);
	}

	public override void SetBarScale(float normValue)
	{
		slider.sliderValue = normValue;
	}

	public override void OnBarTimerFinished()
	{
		base.OnBarTimerFinished();
	}

	public override void OnBarTimeTick()
	{
		timeLeft += Time.deltaTime / totalTime;
		float barScale = Mathf.Lerp(currentNormalizedWidth, targetNormalizedWidth, timeLeft);
		if (timeLeft >= 1f)
		{
			SetBarScale(0f);
			StopTimer();
			startFlash();
		}
		else
		{
			SetBarScale(barScale);
		}
	}

	public void SetBarTime(float time)
	{
		CalculateBar = true;
		currentNormalizedWidth = 0f;
		timeLeft = 0f;
		totalTime = time;
	}

	public void StopTimer()
	{
		SetBarScale(0f);
		CalculateBar = false;
		currentNormalizedWidth = 0f;
	}
}
