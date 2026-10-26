using UnityEngine;

namespace Calamity.Input
{
	public class InputWrapper
	{
		protected bool requiresUpdate;

		public bool RequiresUpdate
		{
			get
			{
				return requiresUpdate;
			}
		}

		public bool MouseEnabled
		{
			get
			{
				return InputManager.MouseEnabled;
			}
		}

		public virtual void Update()
		{
		}

		public virtual void OnLevelLoaded()
		{
		}

		public virtual void OnGameObjectUpdated(GameObject gameObject)
		{
		}

		public virtual void InitializeIngame()
		{
		}

		public virtual bool GetBool()
		{
			return false;
		}

		public virtual Vector3 GetVector()
		{
			return Vector3.zero;
		}

		public virtual int GetInt()
		{
			return 0;
		}

		public virtual float GetFloat()
		{
			return 0f;
		}

		public virtual void Shutdown()
		{
		}
	}
}
