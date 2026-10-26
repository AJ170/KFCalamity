using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class ListExtensions
{
	public static void Shuffle<T>(this IList<T> list)
	{
		RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
		int num = list.Count;
		while (num > 1)
		{
			byte[] array = new byte[1];
			do
			{
				rNGCryptoServiceProvider.GetBytes(array);
			}
			while (array[0] >= num * (255 / num));
			int index = array[0] % num;
			num--;
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static void QuickShuffle<T>(this IList<T> list)
	{
		Random random = new Random();
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = random.Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static T LastItem<T>(this IList<T> list)
	{
		T result = default(T);
		if (list.Count > 0)
		{
			result = list[list.Count - 1];
		}
		return result;
	}
}
