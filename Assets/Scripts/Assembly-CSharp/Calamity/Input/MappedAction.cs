namespace Calamity.Input
{
	public struct MappedAction
	{
		private InputAction action;

		private InputActionType type;

		private InputKey key;

		public InputAction Action
		{
			get
			{
				return action;
			}
		}

		public InputKey Key
		{
			get
			{
				return key;
			}
		}

		public InputActionType Type
		{
			get
			{
				return type;
			}
		}

		public MappedAction(InputAction action, InputKey key, InputActionType type = InputActionType.Normal)
		{
			this.action = action;
			this.key = key;
			this.type = type;
		}
	}
}
