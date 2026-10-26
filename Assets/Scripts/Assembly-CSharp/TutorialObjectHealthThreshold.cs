using System;
using UnityEngine;

public class TutorialObjectHealthThreshold : TutorialEventBase
{
	public enum ThresholdType
	{
		MoreThan = 0,
		LessThan = 1,
		Equal = 2
	}

	public enum ThresholdValueType
	{
		Absolute = 0,
		Percentage = 1
	}

	[Serializable]
	public class Threshold
	{
		public ThresholdType ThresholdType;

		public ThresholdValueType ValueType;

		public int Value;

		public bool Compare(int currentHealth, int maxHealth)
		{
			float num = (float)currentHealth / (float)maxHealth;
			int num2 = ((ValueType != ThresholdValueType.Percentage) ? currentHealth : ((int)(num * 100f)));
			switch (ThresholdType)
			{
			case ThresholdType.MoreThan:
				return num2 > Value;
			case ThresholdType.LessThan:
				return num2 < Value;
			case ThresholdType.Equal:
				return num2 == Value;
			default:
				return false;
			}
		}
	}

	public GameObject Object;

	public Threshold[] Thresholds;

	private Health health;

	private void Start()
	{
		health = Object.GetComponent<Health>();
	}

	private void Update()
	{
		if (base.IsComplete)
		{
			return;
		}
		bool flag = true;
		Threshold[] thresholds = Thresholds;
		foreach (Threshold threshold in thresholds)
		{
			if (!threshold.Compare(health.CurrentHealth, health.MaxHealth))
			{
				flag = false;
			}
		}
		if (flag)
		{
			SetComplete(true);
		}
	}

	public override void OnStart()
	{
	}
}
