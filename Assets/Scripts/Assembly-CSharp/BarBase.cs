using UnityEngine;

public class BarBase : MonoBehaviour
{
	public enum BarType
	{
		PlayerHealth = 1,
		EnemyHealth = 2,
		Armor = 3,
		Reload = 4,
		CastBar = 5
	}

	protected UISlider slider;

	protected float maxWidth;

	protected float currentNormalizedWidth;

	protected float targetNormalizedWidth;

	protected float barSpeed;

	protected Vector3 barScaleVector;

	public bool CalculateBar;

	protected int playerNumber;

	protected Texture background;

	protected Texture bar;

	protected Texture frame;

	public virtual void Start()
	{
		slider = GetComponent<UISlider>();
		if (slider == null)
		{
			Debug.Log("<BarBase> - Could not find UISlider component.");
		}
		barSpeed = 5f;
	}

	public virtual void Update()
	{
		if (CalculateBar)
		{
			OnBarTimeTick();
		}
	}

	public virtual void ResetToFull()
	{
		if (slider == null)
		{
			slider = GetComponent<UISlider>();
		}
		CalculateBar = false;
		slider.sliderValue = 1f;
		currentNormalizedWidth = 1f;
	}

	public virtual void ResetToEmpty()
	{
		if (slider == null)
		{
			slider = GetComponent<UISlider>();
		}
		CalculateBar = false;
		slider.sliderValue = 0.01f;
		currentNormalizedWidth = 0.01f;
	}

	public virtual void SetBarStartValue(float normValue)
	{
		if (slider == null)
		{
			slider = GetComponent<UISlider>();
		}
		CalculateBar = false;
		slider.sliderValue = normValue;
		currentNormalizedWidth = normValue;
	}

	public virtual void SetBarScale(float normValue)
	{
		if (slider == null)
		{
			slider = GetComponent<UISlider>();
		}
		slider.sliderValue = normValue;
		currentNormalizedWidth = normValue;
	}

	public virtual void SetBarTarget(float normValue)
	{
		targetNormalizedWidth = normValue;
		CalculateBar = true;
	}

	public virtual void OnBarTimeTick()
	{
		bool flag = ((targetNormalizedWidth >= currentNormalizedWidth) ? true : false);
		float t = Time.deltaTime * barSpeed;
		float num = Mathf.Lerp(currentNormalizedWidth, targetNormalizedWidth, t);
		if (!flag)
		{
			if (num <= 0.02f)
			{
				SetBarScale(0f);
				currentNormalizedWidth = targetNormalizedWidth;
				CalculateBar = false;
				OnBarTimerFinished();
			}
			else
			{
				SetBarScale(num);
			}
		}
		else if (num >= 0.98f)
		{
			SetBarScale(1f);
			currentNormalizedWidth = targetNormalizedWidth;
			CalculateBar = false;
			OnBarTimerFinished();
		}
		else
		{
			SetBarScale(num);
		}
	}

	public virtual void OnBarTimerFinished()
	{
	}
}
