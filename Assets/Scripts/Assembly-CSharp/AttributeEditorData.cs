using System;

[Serializable]
public class AttributeEditorData
{
	public AttributeType Type;

	public AttributeName Name;

	public bool Flag;

	public int Fixed;

	public float Multiplier = 1f;
}
