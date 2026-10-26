using System.Collections.Generic;
using UnityEngine;

namespace Spawning
{
	public class QueuedSquad
	{
		public ZedType[] Units;

		public bool SpawnAsWhole;

		public bool DiscardOverflow;

		public QueuedSquad(ZedType[] units)
		{
			Units = units;
		}

		public QueuedSquad(ZedType[] units, bool spawnAsWhole, bool discardOverflow)
		{
			Units = units;
			SpawnAsWhole = spawnAsWhole;
			DiscardOverflow = discardOverflow;
		}

		public List<ZedType> GetUnits(int count)
		{
			int num = Units.Length - count;
			if (num > 0)
			{
				ZedType[] array = new ZedType[num];
				List<ZedType> list = new List<ZedType>(count);
				for (int i = 0; i < Units.Length; i++)
				{
					if (i < count)
					{
						list.Add(Units[i]);
					}
					else
					{
						array[i - count] = Units[i];
					}
				}
				Units = array;
				return list;
			}
			Debug.LogWarning("[QueuedSquad:GetUnits] Attempt to get more units than present in list");
			return null;
		}
	}
}
