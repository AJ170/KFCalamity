using System;

public class AlphaLerp : Lerp01
{
	private float alpha;

	public float Alpha
	{
		get
		{
			return alpha;
		}
		set
		{
			alpha = value;
		}
	}

	public AlphaLerp(float alpha = 1f)
	{
		this.alpha = alpha;
	}

	public override void Update()
	{
		if (lerping)
		{
			base.Update();
			alpha = current;
		}
	}

	public bool Lerp(float goal, float time, Action callback, bool forceLerp = false)
	{
		return base.Lerp(alpha, goal, 1f / time, callback, forceLerp);
	}
}
