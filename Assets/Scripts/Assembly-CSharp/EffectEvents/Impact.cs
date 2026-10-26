using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class Impact : TriggerBase
	{
		private Collider collider;

		private Vector3 position;

		private Vector3 fromDirection;

		public Collider Collider
		{
			get
			{
				return collider;
			}
		}

		public Vector3 Position
		{
			get
			{
				return position;
			}
		}

		public Vector3 FromDirection
		{
			get
			{
				return fromDirection;
			}
		}

		public Impact(Vector3 position)
			: base(TriggerId.Impact)
		{
			this.position = position;
		}

		public Impact(Vector3 position, Collider collider)
			: base(TriggerId.Impact)
		{
			this.position = position;
			this.collider = collider;
		}

		public Impact(Vector3 position, Collider collider, Vector3 fromDirection)
			: base(TriggerId.Impact)
		{
			this.position = position;
			this.collider = collider;
			this.fromDirection = fromDirection;
		}

		public Impact(string customName, Vector3 position)
			: base(TriggerId.Impact, customName)
		{
			this.position = position;
		}

		public Impact(string customName, Vector3 position, Collider collider)
			: base(TriggerId.Impact, customName)
		{
			this.position = position;
			this.collider = collider;
		}

		public Impact(string customName, Vector3 position, Collider collider, Vector3 fromDirection)
			: base(TriggerId.Impact, customName)
		{
			this.position = position;
			this.collider = collider;
			this.fromDirection = fromDirection;
		}
	}
}
