using UnityEngine;

namespace Acheivements
{
	public abstract class AcheivementBase : MonoBehaviour
	{
		protected AchievementId id;

		protected AcheivementStatus status;

		protected EventId eventId;

		public AchievementId ID
		{
			get
			{
				return id;
			}
		}

		public AcheivementStatus Status
		{
			get
			{
				return status;
			}
		}

		protected virtual void Awake()
		{
			AcheivementManager.RegisterAcheivement(this);
		}

		public void Process(EventBase e)
		{
			if (eventId != 0 && eventId == e.ID)
			{
				OnProcess(e);
			}
		}

		protected virtual void OnProcess(EventBase e)
		{
		}
	}
}
