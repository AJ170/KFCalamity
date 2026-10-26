using System;
using UnityEngine;

public class ActionSetObjectsHealth : TutorialSubStepBase
{
	[Serializable]
	public class DamageEntry
	{
		public GameObject Object;

		public string ObjectTag;

		public int NewHealth = 100;

		public bool Kill;
	}

	public DamageEntry[] Objects;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnStart()
	{
		DamageEntry[] objects = Objects;
		foreach (DamageEntry damageEntry in objects)
		{
			GameObject[] array = null;
			if (!string.IsNullOrEmpty(damageEntry.ObjectTag))
			{
				array = GameObject.FindGameObjectsWithTag(damageEntry.ObjectTag);
			}
			else if (damageEntry.Object != null)
			{
				array = new GameObject[1] { damageEntry.Object };
			}
			if (array == null)
			{
				continue;
			}
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				Health component = gameObject.GetComponent<Health>();
				if (damageEntry.Kill)
				{
					component.TakeDamage(component.CurrentHealth, DamageType.None, 1f, true);
				}
				else
				{
					component.SetHealth(damageEntry.NewHealth);
				}
			}
		}
		SetComplete(true);
	}
}
