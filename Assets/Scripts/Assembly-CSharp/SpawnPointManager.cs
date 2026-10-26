using System;
using System.Collections.Generic;
using Spawning;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementTracker))]
public class SpawnPointManager : MonoBehaviour
{
	private enum InternalState
	{
		Passive = 0,
		PredictingPosition = 1,
		FilteringRange = 2,
		FilteringSpawnBlock = 3,
		FilteringLOS = 4,
		Rating = 5,
		Evaluating = 6
	}

	private static SpawnPointManager instance;

	public float MinRadius = 2f;

	public float MaxRadius = 20f;

	public float BossMinRadius = 20f;

	public float BossMaxRadius = 50f;

	public LayerMask SpawnBlockerMask;

	public LayerMask LOSMask;

	public float LOSHeightOffset = 1.8f;

	public float DirectionMinRadius = 6f;

	public bool AllowSpawnInsideCamera = true;

	public bool UseExpandedLineOfSightCheck = true;

	public bool DebugMode;

	private SpawnManager spawnManager;

	private PlayerMovementTracker playerTracker;

	private InternalState state;

	private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

	private RatingData ratingData;

	private PlayerPrediction prediction;

	private SpawnPoint[] validSpawnPoints;

	private Vector3 preferredDirection = Vector3.zero;

	private SpawnPoint bestSpawnPoint;

	private float bestRatingValue;

	public static SpawnPointManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		spawnManager = GetComponent<SpawnManager>();
		playerTracker = GetComponent<PlayerMovementTracker>();
	}

	private void OnLevelWasLoaded(int level)
	{
		SetupSpawnPoints();
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void Update()
	{
		if (state != 0)
		{
			Process();
		}
	}

	private void SetupSpawnPoints()
	{
		spawnPoints.Clear();
		GameObject[] array = GameObject.FindGameObjectsWithTag("SpawnPoint");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			spawnPoints.Add(gameObject.GetComponent<SpawnPoint>());
		}
	}

	public void StartRating(RatingData ratingData)
	{
		if (state == InternalState.Passive)
		{
			validSpawnPoints = spawnPoints.ToArray();
			SetupRatingData(ratingData);
			state = InternalState.PredictingPosition;
		}
	}

	private void SetupRatingData(RatingData ratingData)
	{
		if (ratingData.Method == RatingMethod.Normal && ratingData.Squad.Count > 0 && ratingData.Squad[0] == ZedType.Patriarch)
		{
			ratingData.Method = RatingMethod.Boss;
		}
		if (ratingData.MinRange == -1f)
		{
			float minRange;
			switch (ratingData.Method)
			{
			case RatingMethod.Normal:
				minRange = MinRadius;
				break;
			case RatingMethod.Boss:
				minRange = BossMinRadius;
				break;
			case RatingMethod.BossForced:
				minRange = MinRadius;
				break;
			default:
				minRange = MinRadius;
				break;
			}
			ratingData.MinRange = minRange;
		}
		if (ratingData.MaxRange == -1f)
		{
			float maxRange;
			switch (ratingData.Method)
			{
			case RatingMethod.Normal:
				maxRange = MaxRadius;
				break;
			case RatingMethod.Boss:
			case RatingMethod.BossForced:
				maxRange = BossMaxRadius;
				break;
			default:
				maxRange = MinRadius;
				break;
			}
			ratingData.MaxRange = maxRange;
		}
		this.ratingData = ratingData;
	}

	public void AbortRating()
	{
		if (state != 0)
		{
			Reset();
		}
	}

	private void Process()
	{
		RateProcess rateProcess = null;
		InternalState internalState;
		if (ratingData.Method == RatingMethod.Normal)
		{
			switch (state)
			{
			case InternalState.PredictingPosition:
				rateProcess = GetPredictedPosition;
				internalState = InternalState.FilteringRange;
				break;
			case InternalState.FilteringRange:
				rateProcess = FilterSpawnPointsRange;
				internalState = InternalState.FilteringSpawnBlock;
				break;
			case InternalState.FilteringSpawnBlock:
				rateProcess = FilterSpawnPointBlocker;
				internalState = InternalState.FilteringLOS;
				break;
			case InternalState.FilteringLOS:
				rateProcess = FilterSpawnPointsLOS;
				internalState = InternalState.Rating;
				break;
			case InternalState.Rating:
				rateProcess = ProcessRating;
				internalState = InternalState.Passive;
				break;
			default:
				internalState = InternalState.Passive;
				break;
			}
		}
		else if (ratingData.Method == RatingMethod.Boss || ratingData.Method == RatingMethod.BossForced)
		{
			switch (state)
			{
			case InternalState.PredictingPosition:
				rateProcess = GetPredictedPosition;
				internalState = InternalState.FilteringRange;
				break;
			case InternalState.FilteringRange:
				rateProcess = FilterSpawnPointsRange;
				internalState = InternalState.FilteringSpawnBlock;
				break;
			case InternalState.FilteringSpawnBlock:
				rateProcess = FilterSpawnPointBlocker;
				internalState = InternalState.Rating;
				break;
			case InternalState.Rating:
				rateProcess = ProcessRating;
				internalState = InternalState.Passive;
				break;
			default:
				internalState = InternalState.Passive;
				break;
			}
		}
		else
		{
			internalState = InternalState.Passive;
		}
		if (rateProcess != null && rateProcess())
		{
			state = internalState;
		}
		else
		{
			OnFail();
		}
	}

	private bool GetPredictedPosition()
	{
		if (playerTracker != null)
		{
			if (ratingData.Method == RatingMethod.Normal)
			{
				PlayerPrediction predictedPosition = playerTracker.GetPredictedPosition();
				if (predictedPosition != null)
				{
					prediction = predictedPosition;
					return true;
				}
			}
			else if (ratingData.Method == RatingMethod.Boss || ratingData.Method == RatingMethod.BossForced)
			{
				Vector3? cameraCenter = playerTracker.GetCameraCenter();
				if (cameraCenter.HasValue)
				{
					prediction = new PlayerPrediction(cameraCenter.Value, Vector3.zero, false);
					return true;
				}
			}
		}
		return false;
	}

	private bool FilterSpawnPointsRange()
	{
		if (prediction != null)
		{
			Vector3 position = prediction.Position;
			float num = ratingData.MinRange * ratingData.MinRange;
			float num2 = ratingData.MaxRange * ratingData.MaxRange;
			bool result = false;
			for (int i = 0; i < validSpawnPoints.Length; i++)
			{
				SpawnPoint spawnPoint = validSpawnPoints[i];
				if (!(spawnPoint != null))
				{
					continue;
				}
				if (spawnPoint.enabled)
				{
					float sqrMagnitude = (spawnPoint.Position - position).sqrMagnitude;
					if (!(num < sqrMagnitude) || !(sqrMagnitude < num2))
					{
						validSpawnPoints[i] = null;
					}
					else
					{
						result = true;
					}
				}
				else
				{
					validSpawnPoints[i] = null;
				}
			}
			return result;
		}
		return false;
	}

	private bool FilterSpawnPointBlocker()
	{
		if (prediction != null)
		{
			Vector3 position = prediction.Position;
			bool result = false;
			for (int i = 0; i < validSpawnPoints.Length; i++)
			{
				SpawnPoint spawnPoint = validSpawnPoints[i];
				if (spawnPoint != null)
				{
					if (BlockedBySpawnBlocker(validSpawnPoints[i].Position, position))
					{
						validSpawnPoints[i] = null;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}
		return true;
	}

	private bool FilterSpawnPointsLOS()
	{
		if (prediction != null)
		{
			List<LineOfSightSource> lineOfSightSources = null;
			if (LineOfSightManager.Instance != null)
			{
				lineOfSightSources = LineOfSightManager.Instance.GetSources();
			}
			bool result = false;
			for (int i = 0; i < validSpawnPoints.Length; i++)
			{
				SpawnPoint spawnPoint = validSpawnPoints[i];
				if (spawnPoint != null)
				{
					Vector3 position = validSpawnPoints[i].Position;
					if (WithinCameraBounds(position) && (!AllowSpawnInsideCamera || InLineOfSight(position, lineOfSightSources)))
					{
						validSpawnPoints[i] = null;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}
		return true;
	}

	private bool BlockedBySpawnBlocker(Vector3 target, Vector3 source)
	{
		return Physics.Linecast(source + Vector3.up * LOSHeightOffset, target + Vector3.up * LOSHeightOffset, SpawnBlockerMask.value);
	}

	private bool WithinCameraBounds(Vector3 position)
	{
		return playerTracker.WithinCameraBounds(position);
	}

	private bool InLineOfSight(Vector3 position, List<LineOfSightSource> lineOfSightSources)
	{
		if (lineOfSightSources != null)
		{
			foreach (LineOfSightSource lineOfSightSource in lineOfSightSources)
			{
				if (UseExpandedLineOfSightCheck)
				{
					Vector3 v = Vector3.Normalize(lineOfSightSource.Position - position);
					for (int i = 0; i < 3; i++)
					{
						Vector3 vector;
						switch (i)
						{
						case 0:
							vector = lineOfSightSource.Position;
							break;
						case 1:
							vector = lineOfSightSource.Position + v.RotateY((float)Math.PI / 2f);
							break;
						case 2:
							vector = lineOfSightSource.Position + v.RotateY(-(float)Math.PI / 2f);
							break;
						default:
							vector = lineOfSightSource.Position;
							break;
						}
						if (!Physics.Linecast(vector + Vector3.up * LOSHeightOffset, position + Vector3.up * LOSHeightOffset, LOSMask.value))
						{
							return true;
						}
					}
				}
				else if (!Physics.Linecast(lineOfSightSource.Position + Vector3.up * LOSHeightOffset, position + Vector3.up * LOSHeightOffset, LOSMask.value))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool ProcessRating()
	{
		PrepareRating();
		RateSpawnPoints();
		if (bestSpawnPoint != null)
		{
			OnRatingFinished();
			return true;
		}
		return false;
	}

	private void PrepareRating()
	{
		PreparePreferredDirection();
	}

	private void PreparePreferredDirection()
	{
		List<SpawnedUnit> spawnedObjectes = spawnManager.GetSpawnedObjectes();
		if (spawnedObjectes.Count > 0)
		{
			Vector3 position = prediction.Position;
			float num = DirectionMinRadius * DirectionMinRadius;
			Vector3 zero = Vector3.zero;
			foreach (SpawnedUnit item in spawnedObjectes)
			{
				GameObject gameObject = item.GameObject;
				if (gameObject != null)
				{
					Vector3 vector = position - gameObject.transform.position;
					vector.y = 0f;
					if (vector.sqrMagnitude > num)
					{
						zero += vector;
					}
				}
			}
			preferredDirection = zero.normalized;
		}
		else
		{
			preferredDirection = Vector3.zero;
		}
	}

	private void RateSpawnPoints()
	{
		ratingData.Position = prediction.Position;
		ratingData.Direction = preferredDirection;
		SpawnPoint[] array = validSpawnPoints;
		foreach (SpawnPoint spawnPoint in array)
		{
			if (spawnPoint != null)
			{
				float num = spawnPoint.Rate(ratingData);
				if (num > bestRatingValue)
				{
					bestSpawnPoint = spawnPoint;
					bestRatingValue = num;
				}
			}
		}
	}

	private void OnRatingFinished()
	{
		Vector3 direction;
		if (bestSpawnPoint != null)
		{
			bestSpawnPoint.TriggerCooldown();
			direction = prediction.Position - bestSpawnPoint.Position;
			direction.y = 0f;
			direction.Normalize();
		}
		else
		{
			direction = Vector3.zero;
		}
		if (ratingData.Callback != null)
		{
			ratingData.Callback(bestSpawnPoint, direction);
		}
		Reset();
	}

	private void OnFail()
	{
		if (this.ratingData.Method == RatingMethod.Boss)
		{
			RatingData ratingData = this.ratingData;
			ratingData.Method = RatingMethod.BossForced;
			ratingData.MinRange = -1f;
			ratingData.MaxRange = -1f;
			Reset();
			StartRating(ratingData);
			return;
		}
		Vector3 direction;
		if (this.ratingData.Method == RatingMethod.BossForced)
		{
			int index = UnityEngine.Random.Range(0, spawnPoints.Count);
			bestSpawnPoint = spawnPoints[index];
			if (bestSpawnPoint != null)
			{
				bestSpawnPoint.TriggerCooldown();
				direction = bestSpawnPoint.Position - prediction.Position;
			}
			else
			{
				direction = Vector3.zero;
			}
		}
		else
		{
			direction = Vector3.zero;
		}
		if (this.ratingData.Callback != null)
		{
			this.ratingData.Callback(bestSpawnPoint, direction);
		}
		Reset();
	}

	private void Reset()
	{
		ratingData = null;
		prediction = null;
		validSpawnPoints = null;
		preferredDirection = Vector3.zero;
		bestSpawnPoint = null;
		bestRatingValue = 0f;
		state = InternalState.Passive;
	}

	public bool IsBusy()
	{
		return state != InternalState.Passive;
	}
}
