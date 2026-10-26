using EventHelpers;
using UnityEngine;

namespace GameEvents
{
	public class CreateUnit : EventBase
	{
		private GameObject unitObject;

		public GameObject UnitObject
		{
			get
			{
				return unitObject;
			}
		}

		public CreateUnit(GameObject obj)
			: base(EventId.CreateUnit)
		{
			unitObject = obj;
		}
	}
}
