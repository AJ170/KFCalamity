using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnManager : MonoBehaviour
{
	private class QueuedSpawn
	{
		public ZedType Unit;

		public Vector3 Position;

		public Quaternion Rotation;

		public QueuedSpawn(ZedType unit, Vector3 position, Quaternion rotation)
		{
			Unit = unit;
			Position = position;
			Rotation = rotation;
		}
	}

	public int FrameDelay = 2;

	private SpawnManager spawnManager;

	private Queue<QueuedSpawn> queue = new Queue<QueuedSpawn>();

	private int frameCounter;

	private void Start()
	{
		spawnManager = GetComponent<SpawnManager>();
	}

	private void Update()
	{
		frameCounter--;
		if (queue.Count > 0 && frameCounter <= 0)
		{
			SpawnNext();
			frameCounter = FrameDelay;
		}
	}

	public void AddSpawn(ZedType unit, Vector3 position, Quaternion rotation)
	{
		if (queue.Count == 0)
		{
			frameCounter = 1;
		}
		queue.Enqueue(new QueuedSpawn(unit, position, rotation));
	}

	public void AddSpawns(List<ZedType> units, Vector3 position, Quaternion rotation)
	{
		foreach (ZedType unit in units)
		{
			AddSpawn(unit, position, rotation);
		}
	}

	private void SpawnNext()
	{
		QueuedSpawn queuedSpawn = queue.Dequeue();
		if (queuedSpawn != null && GameUnitManager.Instance != null)
		{
			GameObject gameObject = GameUnitManager.Instance.CreateUnit(queuedSpawn.Unit, queuedSpawn.Position, queuedSpawn.Rotation);
			if (gameObject != null)
			{
				OnSpawn(gameObject, queuedSpawn.Unit);
			}
		}
	}

	private void OnSpawn(GameObject unit, ZedType type)
	{
		spawnManager.OnUnitSpawned(unit, type);
	}
}
