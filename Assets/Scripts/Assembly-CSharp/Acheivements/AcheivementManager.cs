using System.Collections.Generic;
using UnityEngine;

namespace Acheivements
{
	public class AcheivementManager : MonoBehaviour
	{
		private static AcheivementManager instance;

		private List<AcheivementBase> acheivements = new List<AcheivementBase>();

		private List<AcheivementBase> progression = new List<AcheivementBase>();

		public static AcheivementManager Instance
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

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		public static void Trigger(EventBase e)
		{
			if (!(instance != null) || e == null)
			{
				return;
			}
			foreach (AcheivementBase acheivement in instance.acheivements)
			{
				AcheivementStatus status = acheivement.Status;
				AcheivementStatus acheivementStatus = status;
				if (acheivement.Status != AcheivementStatus.Gold)
				{
					acheivement.Process(e);
					acheivementStatus = acheivement.Status;
				}
				if (status != acheivementStatus && !instance.progression.Contains(acheivement))
				{
					instance.progression.Add(acheivement);
				}
			}
		}

		public List<AcheivementBase> GetProgressionLogs()
		{
			return progression;
		}

		public static void RegisterAcheivement(AcheivementBase acheivement)
		{
			if (instance != null && acheivement != null)
			{
				instance.acheivements.Add(acheivement);
			}
		}
	}
}
