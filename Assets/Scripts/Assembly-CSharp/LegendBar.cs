using System;
using UnityEngine;

[Serializable]
public class LegendBar
{
	public TweenPosition PositionTween;

	public UILabel O_label;

	public UILabel U_label;

	public UILabel Y_label;

	public UILabel A_label;

	public string Okey;

	public string Ukey;

	public string Ykey;

	public string Akey;

	public GameObject Parent;

	public void Show()
	{
		if (!(Parent != null))
		{
		}
	}

	public void Hide()
	{
		if (Parent != null)
		{
			Parent.SetActive(false);
		}
	}
}
