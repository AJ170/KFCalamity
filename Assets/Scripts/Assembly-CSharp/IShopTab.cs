using UnityEngine;

public interface IShopTab
{
	Rect TabRect { get; }

	void Update();

	void Display();

	void RemoveSelection();

	void ResetUnableTint();

	void SetSelectedButton(int direction);

	void HandleButtonPressed();

	void HandleTouch(Vector2 location);

	void SetSizeBasedOnResolution();

	void OnForwardPressed();

	void OnBackwardPressed();

	void Start();

	void Stop();

	void SetTabPosX(float x);

	void SetInitialSelection();
}
