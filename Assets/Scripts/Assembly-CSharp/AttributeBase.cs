public class AttributeBase
{
	private AttributeType type;

	private AttributeName name;

	public AttributeType Type
	{
		get
		{
			return type;
		}
	}

	public AttributeName Name
	{
		get
		{
			return name;
		}
	}

	public AttributeBase(AttributeType type, AttributeName name)
	{
		this.type = type;
		this.name = name;
	}
}
