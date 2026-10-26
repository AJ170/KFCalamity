using UnityEngine;
#if UNITY_PSP2 && !UNITY_EDITOR
using UnityEngine.PSVita;
#endif

namespace Calamity.Input
{
	// PS Vita port: a touch in the centre of the rear touch pad (InputKey.RearTouchCenter). The edges are left out so
	// fingers holding the Vita don't trigger it. Pad coordinates: y = 0 at the top edge (1919 x 781 measured on hardware).
	public class VitaRearTouchWrapper : InputWrapper
	{
		private const float ZoneMinX = 0.3f;

		private const float ZoneMaxX = 0.7f;

		private const float ZoneMinY = 0.2f;

		private const float ZoneMaxY = 0.8f;

		private static int polledFrame = -1;

		private static bool held;

		private static bool wasHeld;

		private InputActionType type;

		public VitaRearTouchWrapper(InputActionType type)
		{
			this.type = type;
		}

		public override bool GetBool()
		{
			Poll();
			switch (type)
			{
			case InputActionType.Down:
				return held && !wasHeld;
			case InputActionType.Up:
				return !held && wasHeld;
			default:
				return held;
			}
		}

		public override int GetInt()
		{
			return GetBool() ? 1 : 0;
		}

		public override float GetFloat()
		{
			return (!GetBool()) ? 0f : 1f;
		}

		private static void Poll()
		{
			if (polledFrame == Time.frameCount)
			{
				return;
			}
			polledFrame = Time.frameCount;
			wasHeld = held;
			held = false;
#if UNITY_PSP2 && !UNITY_EDITOR
			float width = PSVitaInput.secondaryTouchWidth;
			float height = PSVitaInput.secondaryTouchHeight;
			int count = PSVitaInput.touchCountSecondary;
			for (int i = 0; i < count && !held; i++)
			{
				Vector2 position = PSVitaInput.GetSecondaryTouch(i).position;
				float x = position.x / width;
				float y = position.y / height;
				held = x >= ZoneMinX && x <= ZoneMaxX && y >= ZoneMinY && y <= ZoneMaxY;
			}
#endif
		}
	}
}
