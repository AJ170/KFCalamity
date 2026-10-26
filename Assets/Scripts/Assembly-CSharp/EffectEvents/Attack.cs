using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class Attack : TriggerBase
	{
		private bool isAiming;

		private bool isAimingAtHead;

		private GameObject aimTarget;

		public bool IsAiming
		{
			get
			{
				return isAiming;
			}
		}

		public bool IsAimingAtHead
		{
			get
			{
				return isAimingAtHead;
			}
		}

		public GameObject AimTarget
		{
			get
			{
				return aimTarget;
			}
		}

		public Attack()
			: base(TriggerId.Attack)
		{
		}

		public Attack(string customName)
			: base(TriggerId.Attack, customName)
		{
		}

		public Attack(bool isAiming, bool isAimingAtHead, GameObject aimTarget)
			: base(TriggerId.Attack)
		{
			this.isAiming = isAiming;
			this.isAimingAtHead = isAimingAtHead;
			this.aimTarget = aimTarget;
		}

		public Attack(string customName, bool isAiming, bool isAimingAtHead, GameObject aimTarget)
			: base(TriggerId.Attack, customName)
		{
			this.isAiming = isAiming;
			this.isAimingAtHead = isAimingAtHead;
			this.aimTarget = aimTarget;
		}
	}
}
