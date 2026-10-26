using UnityEngine;

public struct TKRect
{
	public float x;

	public float y;

	public float width;

	public float height;

	public float xMin
	{
		get
		{
			return x;
		}
	}

	public float xMax
	{
		get
		{
			return x + width;
		}
	}

	public float yMin
	{
		get
		{
			return y;
		}
	}

	public float yMax
	{
		get
		{
			return y + height;
		}
	}

	public Vector2 center
	{
		get
		{
			return new Vector2(x + width / 2f, y + height / 2f);
		}
	}

	public TKRect(float x, float y, float width, float height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
		updateRectWithRuntimeScaleModifier();
	}

	public TKRect(float width, float height, Vector2 center)
	{
		this.width = width;
		this.height = height;
		x = center.x - width / 2f;
		y = center.y - height / 2f;
		updateRectWithRuntimeScaleModifier();
	}

	private void updateRectWithRuntimeScaleModifier()
	{
		Vector2 runtimeScaleModifier = TouchKit.instance.runtimeScaleModifier;
		x *= runtimeScaleModifier.x;
		y *= runtimeScaleModifier.y;
		width *= runtimeScaleModifier.x;
		height *= runtimeScaleModifier.y;
	}

	public TKRect copyWithExpansion(float allSidesExpansion)
	{
		return copyWithExpansion(allSidesExpansion, allSidesExpansion);
	}

	public TKRect copyWithExpansion(float xExpansion, float yExpansion)
	{
		xExpansion *= TouchKit.instance.runtimeScaleModifier.x;
		yExpansion *= TouchKit.instance.runtimeScaleModifier.y;
		TKRect result = default(TKRect);
		result.x = x - xExpansion;
		result.y = y - yExpansion;
		result.width = width + xExpansion * 2f;
		result.height = height + yExpansion * 2f;
		return result;
	}

	public bool contains(Vector2 point)
	{
		if (x <= point.x && y <= point.y && xMax >= point.x && yMax >= point.y)
		{
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		return string.Format("TKRect: x: {0}, xMax: {1}, y: {2}, yMax: {3}, width: {4}, height: {5}, center: {6}", x, xMax, y, yMax, width, height, center);
	}
}
