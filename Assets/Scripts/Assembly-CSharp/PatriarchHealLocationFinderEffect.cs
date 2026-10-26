using System.Collections.Generic;
using EventHelpers;
using Spawning;
using UnityEngine;

public class PatriarchHealLocationFinderEffect : EffectBaseListener
{
	public SpawnPoint spawnPoint;

	public float MinRange = 20f;

	public float MaxRange = 50f;

	private AITargetHandler targetHandler;

	private SpawnPointManager spawnPointManager;

	private bool waitingForRating;

	private float maxWaitTime = 0.5f;

	private float counter;

	protected override void Start()
	{
		base.Start();
		spawnPointManager = SpawnPointManager.Instance;
	}

	private void OnMounted(GameObject parent)
	{
		targetHandler = parent.GetComponent<AITargetHandler>();
	}

	private void Update()
	{
		if (!waitingForRating)
		{
			return;
		}
		counter -= Time.deltaTime;
		if (counter > 0f)
		{
			if (!spawnPointManager.IsBusy())
			{
				TriggerRating();
				waitingForRating = false;
			}
		}
		else
		{
			waitingForRating = false;
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && spawnPointManager != null)
		{
			if (spawnPointManager.IsBusy())
			{
				waitingForRating = true;
				counter = maxWaitTime;
			}
			else
			{
				TriggerRating();
			}
		}
	}

	private void TriggerRating()
	{
		RatingData ratingData = new RatingData(MinRange, MaxRange, new List<ZedType> { ZedType.Patriarch }, RatingCallback);
		ratingData.Method = RatingMethod.Boss;
		spawnPointManager.StartRating(ratingData);
	}

	private void RatingCallback(SpawnPoint spawnPoint, Vector3 direction)
	{
		if (spawnPoint != null)
		{
			this.spawnPoint = spawnPoint;
			if (targetHandler != null)
			{
				targetHandler.SetTarget(spawnPoint.gameObject);
			}
		}
		else if (targetHandler != null)
		{
			targetHandler.ClearTarget();
		}
	}
}
