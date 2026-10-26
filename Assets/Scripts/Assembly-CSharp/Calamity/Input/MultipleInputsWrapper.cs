using System;
using System.Collections.Generic;
using UnityEngine;

namespace Calamity.Input
{
	public class MultipleInputsWrapper : InputWrapper
	{
		private InputWrapper[] wrappers;

		private List<InputWrapper> updateWrappers = new List<InputWrapper>();

		public MultipleInputsWrapper(params InputWrapper[] wrappers)
		{
			this.wrappers = wrappers;
			updateWrappers = new List<InputWrapper>();
			updateWrappers.AddRange(Array.FindAll(wrappers, (InputWrapper a) => a.RequiresUpdate));
			if (updateWrappers.Count > 0)
			{
				requiresUpdate = true;
			}
		}

		public override void InitializeIngame()
		{
			InputWrapper[] array = wrappers;
			foreach (InputWrapper inputWrapper in array)
			{
				inputWrapper.InitializeIngame();
			}
		}

		public override void Update()
		{
			foreach (InputWrapper updateWrapper in updateWrappers)
			{
				updateWrapper.Update();
			}
		}

		public override void Shutdown()
		{
			InputWrapper[] array = wrappers;
			foreach (InputWrapper inputWrapper in array)
			{
				inputWrapper.Shutdown();
			}
		}

		public override void OnGameObjectUpdated(GameObject gameObject)
		{
			InputWrapper[] array = wrappers;
			foreach (InputWrapper inputWrapper in array)
			{
				inputWrapper.OnGameObjectUpdated(gameObject);
			}
		}

		public override void OnLevelLoaded()
		{
			InputWrapper[] array = wrappers;
			foreach (InputWrapper inputWrapper in array)
			{
				inputWrapper.OnLevelLoaded();
			}
		}

		public override bool GetBool()
		{
			InputWrapper[] array = wrappers;
			foreach (InputWrapper inputWrapper in array)
			{
				if (inputWrapper.GetBool())
				{
					return true;
				}
			}
			return false;
		}

		public override int GetInt()
		{
			int num = 0;
			InputWrapper[] array = wrappers;
			foreach (InputWrapper inputWrapper in array)
			{
				num = inputWrapper.GetInt();
				if (num != 0)
				{
					break;
				}
			}
			return num;
		}

		public override float GetFloat()
		{
			float num = 0f;
			InputWrapper[] array = wrappers;
			foreach (InputWrapper inputWrapper in array)
			{
				num = inputWrapper.GetFloat();
				if (num != 0f)
				{
					break;
				}
			}
			return num;
		}

		public override Vector3 GetVector()
		{
			Vector3 vector = Vector3.zero;
			InputWrapper[] array = wrappers;
			foreach (InputWrapper inputWrapper in array)
			{
				vector = inputWrapper.GetVector();
				if (vector != Vector3.zero)
				{
					break;
				}
			}
			return vector;
		}
	}
}
