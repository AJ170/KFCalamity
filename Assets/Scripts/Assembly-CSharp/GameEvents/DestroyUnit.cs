using EventHelpers;
using UnityEngine;

namespace GameEvents
{
	public class DestroyUnit : EventBase
	{
		private GameObject unitObject;

		public GameObject UnitObject
		{
			get
			{
				return unitObject;
			}
		}

		public DestroyUnit(GameObject obj)
			: base(EventId.DestroyUnit)
		{
			unitObject = obj;
		}
	}
}
