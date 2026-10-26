using UnityEngine;

namespace Calamity.Input
{
	public class HybridInputWrapper : InputWrapper
	{
		protected InputWrapper inputWrapper1;

		protected InputWrapper inputWrapper2;

		public HybridInputWrapper(InputWrapper wrapper1, InputWrapper wrapper2)
		{
			inputWrapper1 = wrapper1;
			inputWrapper2 = wrapper2;
			requiresUpdate = wrapper1.RequiresUpdate || wrapper2.RequiresUpdate;
		}

		public override void OnLevelLoaded()
		{
			inputWrapper1.OnLevelLoaded();
			inputWrapper2.OnLevelLoaded();
		}

		public override void InitializeIngame()
		{
			inputWrapper1.InitializeIngame();
			inputWrapper2.InitializeIngame();
		}

		public override void Update()
		{
			inputWrapper1.Update();
			inputWrapper2.Update();
		}

		public override void OnGameObjectUpdated(GameObject gameObject)
		{
			inputWrapper1.OnGameObjectUpdated(gameObject);
			inputWrapper2.OnGameObjectUpdated(gameObject);
		}

		public override void Shutdown()
		{
			inputWrapper1.Shutdown();
			inputWrapper2.Shutdown();
		}

		public override bool GetBool()
		{
			return inputWrapper1.GetBool() || inputWrapper2.GetBool();
		}

		public override float GetFloat()
		{
			float @float = inputWrapper1.GetFloat();
			return (@float == 0f) ? inputWrapper2.GetFloat() : @float;
		}

		public override int GetInt()
		{
			int @int = inputWrapper1.GetInt();
			return (@int == 0) ? inputWrapper2.GetInt() : @int;
		}

		public override Vector3 GetVector()
		{
			Vector3 vector = inputWrapper1.GetVector();
			return (!(vector != Vector3.zero)) ? inputWrapper2.GetVector() : vector;
		}
	}
}
