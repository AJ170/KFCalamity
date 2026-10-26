public class ArmorBar : BarBase
{
	public override void Start()
	{
		base.Start();
		base.gameObject.SetActive(false);
	}

	public override void Update()
	{
		base.Update();
	}

	public override void SetBarStartValue(float normValue)
	{
		base.SetBarStartValue(normValue);
	}

	public override void SetBarScale(float normValue)
	{
		base.SetBarScale(normValue);
		if (normValue == 0f)
		{
			base.gameObject.SetActive(false);
		}
	}

	public override void SetBarTarget(float normValue)
	{
		base.SetBarTarget(normValue);
		base.gameObject.SetActive(true);
	}

	public override void ResetToFull()
	{
		base.ResetToFull();
	}

	public override void ResetToEmpty()
	{
		base.ResetToEmpty();
	}

	public override void OnBarTimeTick()
	{
		base.OnBarTimeTick();
	}

	public override void OnBarTimerFinished()
	{
		base.OnBarTimerFinished();
	}
}
