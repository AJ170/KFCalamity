public class AttributeFlag : AttributeBase
{
	private bool flag;

	private bool defaultFlag;

	public bool Flag
	{
		get
		{
			return flag;
		}
	}

	public AttributeFlag(AttributeName name)
		: base(AttributeType.Flag, name)
	{
	}

	public AttributeFlag(AttributeName name, bool defaultFlag)
		: base(AttributeType.Flag, name)
	{
		this.defaultFlag = defaultFlag;
		flag = defaultFlag;
	}

	public void Alter(bool flag)
	{
		this.flag = flag;
	}

	public void Restore()
	{
		flag = defaultFlag;
	}
}
