using System;
using UnityEngine;

namespace TutorialHelpers
{
	[Serializable]
	public class EventObject
	{
		public string Name;

		public ObjectType Type;

		public GameObject Object;

		public bool HasHealthbar;
	}
}
