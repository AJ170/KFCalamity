public static class ObjectExtensions
{
	public static void Log(this object obj, params string[] strings)
	{
		Dbg.Log(strings);
	}
}
