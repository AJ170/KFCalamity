using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

// Compiles every project shader for the current build target (PS Vita) and lists errors and warnings
// (adapted from Borderlands' BOLShaderReport, which only saw import errors). Uses Unity's internal "compile and show
// code" (ShaderUtil.OpenCompiledShader, all variants), whose output lands in Temp/Compiled-*.shader.
// That call also opens each compiled file in the external code editor (it started one Rider per shader), so it
// only runs with -kfcompileshaders, and only with no external editor running; without it only import errors are listed.
// Writes Logs/shader_report.txt. Batch: -executeMethod KFShaderReport.Run
public static class KFShaderReport
{
	public static void Run()
	{
		const BindingFlags Any = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		MethodInfo hasError = typeof(ShaderUtil).GetMethod("ShaderHasError", Any, null, new[] { typeof(Shader) }, null);
		MethodInfo getMessages = typeof(ShaderUtil).GetMethod("GetShaderMessages", Any, null, new[] { typeof(Shader) }, null);
		MethodInfo openCompiled = Array.IndexOf(Environment.GetCommandLineArgs(), "-kfcompileshaders") >= 0 ? typeof(ShaderUtil).GetMethod("OpenCompiledShader", Any) : null;
		StringBuilder sb = new StringBuilder();
		int bad = 0, total = 0;
		foreach (string guid in AssetDatabase.FindAssets("t:Shader", new[] { "Assets" }))
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
			if (shader == null)
			{
				continue;
			}
			total++;
			StringBuilder detail = new StringBuilder();
			bool error = hasError != null && (bool)hasError.Invoke(null, new object[] { shader });
			if (getMessages != null)
			{
				Array messages = getMessages.Invoke(null, new object[] { shader }) as Array;
				foreach (object message in messages ?? new object[0])
				{
					Type t = message.GetType();
					detail.AppendFormat("    {0} line {1} ({2}): {3}\n", t.GetField("severity").GetValue(message), t.GetField("line").GetValue(message),
						t.GetField("platform").GetValue(message), t.GetField("message").GetValue(message));
				}
			}
			if (openCompiled != null)
			{
				foreach (string old in Directory.GetFiles("Temp", "Compiled-*.shader"))
				{
					File.Delete(old);
				}
				ParameterInfo[] ps = openCompiled.GetParameters();
				object[] args = new object[ps.Length];
				args[0] = shader;
				for (int i = 1; i < ps.Length; i++)
				{
					args[i] = ps[i].ParameterType == typeof(bool) ? (object)true : ps[i].ParameterType == typeof(int) ? (object)(ps[i].Name.Contains("ode") ? 1 : 0) : null;
				}
				try
				{
					openCompiled.Invoke(null, args);
				}
				catch (Exception e)
				{
					detail.AppendLine("    (compile call failed: " + (e.InnerException ?? e).Message + ")");
				}
				foreach (string compiled in Directory.GetFiles("Temp", "Compiled-*.shader"))
				{
					bool failed = false;
					foreach (string line in File.ReadAllLines(compiled))
					{
						if ((line.Contains("error") || line.Contains("Error")) && !line.Contains("Compile errors generating this shader"))
						{
							failed = true;
							detail.AppendLine("    " + line.Trim());
						}
						else if (line.Contains("Compile errors generating this shader"))
						{
							failed = true;
						}
					}
					if (failed)
					{
						error = true;
						File.Copy(compiled, "Logs/" + Path.GetFileName(compiled), true);
					}
				}
			}
			if (error)
			{
				bad++;
			}
			sb.AppendFormat("{0} {1} ({2})\n{3}", error ? "FAIL" : "ok  ", path, shader.name, detail);
		}
		sb.Insert(0, string.Format("{0} shaders, {1} failing (target {2})\n\n", total, bad, EditorUserBuildSettings.activeBuildTarget));
		Directory.CreateDirectory("Logs");
		File.WriteAllText("Logs/shader_report.txt", sb.ToString());
		Debug.Log("[KFShaderReport] " + total + " shaders, " + bad + " failing");
	}
}
