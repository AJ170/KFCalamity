using System.Collections.Generic;
using UnityEngine;

public class HudCashScrollEntry : MonoBehaviour
{
	public UILabel Label;

	public UILabel SurfixLabel;

	public TweenPosition PositionTween;

	public List<TweenAlpha> AlphaTweens = new List<TweenAlpha>();
}
