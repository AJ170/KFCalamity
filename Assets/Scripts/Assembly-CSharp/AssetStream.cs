using System;
using System.IO;
using UnityEngine;

public class AssetStream : MonoBehaviour
{
	public static void GetFileOffsetLength(string fileName, out long offset, out long length)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				using (AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getAssets", new object[0]))
				{
					using (AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("openFd", new object[1] { fileName }))
					{
						if (AndroidJNI.ExceptionOccurred() != IntPtr.Zero)
						{
							AndroidJNI.ExceptionDescribe();
							AndroidJNI.ExceptionClear();
							throw new IOException(fileName + " not found");
						}
						offset = androidJavaObject3.Call<long>("getStartOffset", new object[0]);
						length = androidJavaObject3.Call<long>("getLength", new object[0]);
					}
				}
			}
		}
	}

	public static string GetApkPath()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				return androidJavaObject.Call<string>("getPackageCodePath", new object[0]);
			}
		}
	}
}
