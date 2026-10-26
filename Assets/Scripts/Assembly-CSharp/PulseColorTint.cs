using System;
using UnityEngine;

public class PulseColorTint
{
	private LerpColor lerpColor = new LerpColor();

	private Color baseColor = Color.white;

	private Color tintColor = Color.red;

	private bool pulsing;

	private int pulseCounter;

	private int numPulses = 1;

	private float speed = 5f;

	private Action callback;

	public Color CurrentColor
	{
		get
		{
			return lerpColor.CurrentColor;
		}
	}

	public bool Pulsing
	{
		get
		{
			return pulsing;
		}
	}

	public PulseColorTint()
	{
	}

	public PulseColorTint(Color baseColor, Color tintColor)
	{
		this.baseColor = baseColor;
		this.tintColor = tintColor;
	}

	public void Update()
	{
		if (pulsing)
		{
			lerpColor.Update();
		}
	}

	public bool Pulse(int numPulses, float speed, Action callback, bool forcePulse = false)
	{
		if (!pulsing || forcePulse)
		{
			this.numPulses = Mathf.Max(numPulses, 1);
			this.speed = speed;
			this.callback = callback;
			lerpColor.Lerp(baseColor, tintColor, speed, LerpTintDone, forcePulse);
			pulseCounter = 0;
			pulsing = true;
			return true;
		}
		return false;
	}

	public void Stop(bool triggerCallback = false)
	{
		if (pulsing)
		{
			pulsing = false;
			if (triggerCallback && callback != null)
			{
				callback();
			}
		}
	}

	private void LerpTintDone()
	{
		lerpColor.Lerp(tintColor, baseColor, speed, LerpBaseDone, true);
	}

	private void LerpBaseDone()
	{
		pulseCounter++;
		if (pulseCounter == numPulses)
		{
			pulsing = false;
			if (callback != null)
			{
				callback();
			}
		}
		else
		{
			lerpColor.Lerp(baseColor, tintColor, speed, LerpTintDone, true);
		}
	}

	public void SetBaseColor(Color baseColor)
	{
		this.baseColor = baseColor;
	}

	public void SetTintColor(Color tintColor)
	{
		this.tintColor = tintColor;
	}
}
