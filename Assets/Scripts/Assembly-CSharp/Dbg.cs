using System.Text;
using UnityEngine;

public static class Dbg
{
	public static void Log(params string[] strings)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string text in strings)
		{
			stringBuilder.Append(text + "\t");
		}
		Debug.Log(stringBuilder.ToString());
	}
}
