namespace EventHelpers
{
	public class TriggerBase
	{
		private TriggerId id;

		private string name = string.Empty;

		public TriggerId Id
		{
			get
			{
				return id;
			}
		}

		public string CustomName
		{
			get
			{
				return name;
			}
		}

		public bool IsCustom
		{
			get
			{
				return !string.IsNullOrEmpty(name);
			}
		}

		public TriggerBase(TriggerId triggerId)
		{
			id = triggerId;
		}

		public TriggerBase(TriggerId triggerId, string customName)
		{
			id = triggerId;
			name = customName;
		}
	}
}
