using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Shims for Unity 4 APIs that Unity 2018 removed. Lives in firstpass so every assembly can use it.
public static class KFCompat
{
	private static readonly Dictionary<string, Type> s_typeCache = new Dictionary<string, Type>();

	// Java bridges, Ouya IAP and GameAnalytics only work on the original Android build.
	// Runtime check rather than UNITY_ANDROID so the editor behaves like the Vita.
	public static bool IsAndroid
	{
		get { return Application.platform == RuntimePlatform.Android; }
	}

	// PS Vita controls (InputHelper Vita layout, controller HUD). Also on in the editor while the build target is
	// PS Vita, so the layout can be tried there: an Xbox pad's buttons 0-7 and right stick match the Vita's.
	public static bool IsVitaControls
	{
		get
		{
#if UNITY_EDITOR
			if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.PSP2)
			{
				return true;
			}
#endif
			return Application.platform == RuntimePlatform.PSP2;
		}
	}

	// Replacement for the removed GameObject.AddComponent(string) / GetComponent(string) name lookup.
	public static Type FindComponentType(string typeName)
	{
		Type type;
		if (s_typeCache.TryGetValue(typeName, out type))
		{
			return type;
		}
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length && type == null; i++)
		{
			type = assemblies[i].GetType(typeName, false) ?? assemblies[i].GetType("UnityEngine." + typeName, false);
			if (type != null && !typeof(Component).IsAssignableFrom(type))
			{
				type = null;
			}
		}
		s_typeCache[typeName] = type;
		return type;
	}

	public static Component AddComponent(GameObject gameObject, string typeName)
	{
		Type type = FindComponentType(typeName);
		if (type == null)
		{
			Debug.LogError("AddComponent: unknown component type '" + typeName + "'");
			return null;
		}
		return gameObject.AddComponent(type);
	}
}
