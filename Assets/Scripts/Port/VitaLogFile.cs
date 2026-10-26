using System;
using System.IO;
using UnityEngine;

// PS Vita: every Unity log message also goes to ux0:data/KFCalamity/log.txt (PS Vita port; from Borderlands' port).
// A retail Vita shows no console output; with the Vita's storage mounted over USB the file can be read on the PC.
// The previous run's log is kept as log_prev.txt.
// Lines are buffered and written at most every half second (a background thread), at once for errors, asserts and
// exceptions, and when the game quits: written line by line (AutoFlush) each one cost about 12 ms of the frame on a
// PS Vita's memory card, and the game logs several lines for a weapon switch.
public static class VitaLogFile
{
	private static StreamWriter s_writer;

	private static readonly object s_lock = new object();

	// messages can come from any thread, where Unity's clock may not be read
	private static readonly System.Diagnostics.Stopwatch s_clock = System.Diagnostics.Stopwatch.StartNew();

	private static bool s_dirty;

	private static string s_path;

	// a write that failed (the reason is written once the file is open again): logging used to stop for the rest of the
	// run without a word, which looked like the game hanging
	private static string s_failure;

	private static int s_reopenAt;

	private const int FlushMs = 500;


	private static void FlushLoop()
	{
		while (true)
		{
			System.Threading.Thread.Sleep(FlushMs);
			Reopen();
			Flush();
		}
	}

	private static void Failed(Exception e)
	{
		s_failure = e.GetType().Name + ": " + e.Message;
		try
		{
			s_writer.Dispose();
		}
		catch (Exception)
		{
		}
		s_writer = null;
	}

	// after a failure, the file is opened again (appending) every few seconds: lines logged in between are lost
	private static void Reopen()
	{
		lock (s_lock)
		{
			if (s_writer != null || s_path == null || s_failure == null || Environment.TickCount - s_reopenAt < 0)
			{
				return;
			}
			s_reopenAt = Environment.TickCount + 3000;
			try
			{
				s_writer = new StreamWriter(new FileStream(s_path, FileMode.Append, FileAccess.Write, FileShare.Read), new System.Text.UTF8Encoding(false), 16384);
				s_writer.AutoFlush = false;
				s_writer.WriteLine(string.Format("{0:0.00} [Log] [VitaLogFile] writing the log failed ({1}); lines since then are lost", s_clock.Elapsed.TotalSeconds, s_failure));
				s_writer.Flush();
				s_failure = null;
			}
			catch (Exception)
			{
				s_writer = null;
			}
		}
	}

	public static void Flush()
	{
		lock (s_lock)
		{
			if (s_writer == null || !s_dirty)
			{
				return;
			}
			try
			{
				s_writer.Flush();
				s_dirty = false;
			}
			catch (Exception e)
			{
				Failed(e);
			}
		}
	}

#if UNITY_PSP2
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Install()
	{
		if (Application.platform != RuntimePlatform.PSP2 || s_writer != null)
		{
			return;
		}
		// no stack trace taken for ordinary messages and warnings (the game logs a lot: each one walked the managed stack
		// and made garbage); errors, asserts and exceptions keep theirs
		Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
		Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
		// the second folder is only a last resort (the first one was not writable at all)
		foreach (string dir in new[] { "ux0:data/KFCalamity", "ux0:data" })
		{
			try
			{
				Directory.CreateDirectory(dir);
				string path = Path.Combine(dir, "log.txt");
				string prev = Path.Combine(dir, "log_prev.txt");
				// keep the previous run's log if possible; a failure here must not move the log elsewhere (File.Move
				// on ux0 made every run after the first log to another folder)
				try
				{
					if (File.Exists(path))
					{
						File.Copy(path, prev, true);
					}
				}
				catch (Exception)
				{
				}
				s_writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read), new System.Text.UTF8Encoding(false), 16384);
				s_writer.AutoFlush = false;
				s_writer.WriteLine("KF Calamity log " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (" + path + "), persistentDataPath " + Application.persistentDataPath);
				s_writer.Flush();
				s_path = path;
				break;
			}
			catch (Exception)
			{
				s_writer = null;
			}
		}
		if (s_writer != null)
		{
			Application.logMessageReceivedThreaded += OnLog;
			Application.quitting += Flush;
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
			System.Threading.Thread flusher = new System.Threading.Thread(FlushLoop);
			flusher.IsBackground = true;
			flusher.Name = "VitaLogFile flush";
			flusher.Start();
		}
	}

	// free memory after each scene load: the third Vita build ran out of main memory loading the tutorial (video memory
	// full, textures and meshes spilling into main memory)
	private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
	{
		Write(string.Format("[Scene] {0} loaded ({1}): free main {2:0.0} MB, video {3:0.0} MB", scene.name, mode,
			UnityEngine.PSVita.Diagnostics.GetFreeMemoryLPDDR() / 1048576.0, UnityEngine.PSVita.Diagnostics.GetFreeMemoryCDRAM() / 1048576.0));
	}
#endif

	// A line for the log file written directly: a release PS Vita player does not pass Debug.Log messages on (nothing
	// reaches logMessageReceived, the console or the in-game console), so the port's own diagnostics (memory, bundle
	// files) go through here. Elsewhere (editor, PC) it is an ordinary Debug.Log.
	public static void Write(string message)
	{
		if (s_writer == null)
		{
			UnityEngine.Debug.Log(message);
			return;
		}
		OnLog(message, null, LogType.Log);
	}

	private static void OnLog(string message, string stackTrace, LogType type)
	{
		lock (s_lock)
		{
			if (s_writer == null)
			{
				return;
			}
			try
			{
				s_writer.WriteLine(string.Format("{0:0.00} [{1}] {2}", s_clock.Elapsed.TotalSeconds, type, message));
				s_dirty = true;
				if (type == LogType.Exception || type == LogType.Error || type == LogType.Assert)
				{
					if (!string.IsNullOrEmpty(stackTrace))
					{
						s_writer.WriteLine(stackTrace.TrimEnd());
					}
					s_writer.Flush(); // (what led up to a crash is on the card)
					s_dirty = false;
				}
			}
			catch (Exception e)
			{
				// storage full or removed: no more lines until the file can be opened again (Reopen)
				Failed(e);
			}
		}
	}
}
