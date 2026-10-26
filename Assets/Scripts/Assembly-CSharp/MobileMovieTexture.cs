using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class MobileMovieTexture : MonoBehaviour
{
	public delegate void OnFinished(MobileMovieTexture sender);

	private const int CHANNELS = 3;

	private const string PLATFORM_DLL = "theorawrapper";

	[SerializeField]
	private string m_path;

	[SerializeField]
	private Material m_movieMaterial;

	[SerializeField]
	private bool m_playAutomatically = true;

	[SerializeField]
	private bool m_loop = true;

	private IntPtr m_nativeContext = IntPtr.Zero;

	private int m_picWidth;

	private int m_picHeight;

	private int m_yStride;

	private int m_yHeight;

	private int m_uvStride;

	private int m_uvHeight;

	private ChannelTexture[] m_ChannelTextures;

	private bool m_advance = true;

	private double m_elapsedTime;

	private double m_nextFrameTime;

	private bool m_reallocTextures;

	private bool m_hasFinished = true;

	public string Path
	{
		get
		{
			return m_path;
		}
	}

	public bool playAutomatically
	{
		set
		{
			m_playAutomatically = value;
		}
	}

	public int width
	{
		get
		{
			return m_picWidth;
		}
	}

	public int height
	{
		get
		{
			return m_picHeight;
		}
	}

	public float aspectRatio
	{
		get
		{
			if (m_nativeContext != IntPtr.Zero)
			{
				return GetAspectRatio(m_nativeContext);
			}
			return 1f;
		}
	}

	public bool isPlaying
	{
		get
		{
			return m_nativeContext != IntPtr.Zero && !m_hasFinished && m_advance;
		}
	}

	public bool pause
	{
		get
		{
			return !m_advance;
		}
		set
		{
			m_advance = !value;
		}
	}

	[method: MethodImpl(32)]
	public event OnFinished onFinished;

	[DllImport("theorawrapper")]
	private static extern IntPtr OpenStreamDemo(string path, int offset, int size, bool loop, bool pot);

	[DllImport("theorawrapper")]
	private static extern int GetPicWidth(IntPtr context);

	[DllImport("theorawrapper")]
	private static extern int GetPicHeight(IntPtr context);

	[DllImport("theorawrapper")]
	private static extern bool DecodeFrame(IntPtr context);

	[DllImport("theorawrapper")]
	private static extern int GetYStride(IntPtr context);

	[DllImport("theorawrapper")]
	private static extern int GetYHeight(IntPtr context);

	[DllImport("theorawrapper")]
	private static extern int GetUVStride(IntPtr context);

	[DllImport("theorawrapper")]
	private static extern int GetUVHeight(IntPtr context);

	[DllImport("theorawrapper")]
	private static extern bool DecodeAndUpload(IntPtr context, int yHandle, int uHandle, int vHandle);

	[DllImport("theorawrapper")]
	private static extern void GetPlane(IntPtr context, int planeIndex, IntPtr buffer);

	[DllImport("theorawrapper")]
	private static extern double GetNextFrameTime(IntPtr context);

	[DllImport("theorawrapper")]
	private static extern void CloseStream(IntPtr context);

	[DllImport("theorawrapper")]
	private static extern float GetAspectRatio(IntPtr context);

	private void Start()
	{
		if (m_playAutomatically)
		{
			Play();
		}
	}

	public void Play()
	{
		Close();
		m_elapsedTime = 0.0;
		m_nextFrameTime = 0.0;
		Open();
		m_advance = true;
		m_hasFinished = false;
	}

	public void Stop()
	{
		Close();
	}

	private void Open()
	{
		long offset;
		long length;
		AssetStream.GetFileOffsetLength(m_path, out offset, out length);
		bool pot = false;
		m_nativeContext = OpenStreamDemo(AssetStream.GetApkPath(), (int)offset, (int)length, m_loop, pot);
		if (m_nativeContext != IntPtr.Zero)
		{
			m_picWidth = GetPicWidth(m_nativeContext);
			m_picHeight = GetPicHeight(m_nativeContext);
			m_yStride = GetYStride(m_nativeContext);
			m_yHeight = GetYHeight(m_nativeContext);
			m_uvStride = GetUVStride(m_nativeContext);
			m_uvHeight = GetUVHeight(m_nativeContext);
			AllocateTexures();
			Decode();
			m_nextFrameTime = GetNextFrameTime(m_nativeContext);
			return;
		}
		throw new Exception("problem opening movie");
	}

	private void AllocateTexures()
	{
		m_ChannelTextures = new ChannelTexture[3];
		TextureFormat format = TextureFormat.RGBA32;
		m_ChannelTextures[0] = new ChannelTexture(m_yStride, m_yHeight, format);
		m_ChannelTextures[1] = new ChannelTexture(m_uvStride, m_uvHeight, format);
		m_ChannelTextures[2] = new ChannelTexture(m_uvStride, m_uvHeight, format);
		Vector2 scale = new Vector2((float)m_picWidth / (float)m_yStride, 0f - (float)m_picHeight / (float)m_yHeight);
		Vector2 offset = new Vector2(0f, (float)m_picHeight / (float)m_yHeight);
		m_movieMaterial.SetTexture("_YTex", m_ChannelTextures[0].tex);
		m_movieMaterial.SetTexture("_CbTex", m_ChannelTextures[1].tex);
		m_movieMaterial.SetTexture("_CrTex", m_ChannelTextures[2].tex);
		m_movieMaterial.SetTextureScale("_YTex", scale);
		m_movieMaterial.SetTextureOffset("_YTex", offset);
	}

	private void DestroyTextures()
	{
		m_reallocTextures = false;
		m_ChannelTextures[0].Destroy();
		m_ChannelTextures[1].Destroy();
		m_ChannelTextures[2].Destroy();
		m_ChannelTextures = null;
	}

	private void Close()
	{
		if (m_nativeContext != IntPtr.Zero)
		{
			CloseStream(m_nativeContext);
			m_nativeContext = IntPtr.Zero;
		}
	}

	private void OnDestroy()
	{
		Close();
		DestroyTextures();
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			m_reallocTextures = true;
		}
	}

	private void Update()
	{
		if (m_reallocTextures && m_ChannelTextures != null)
		{
			for (int i = 0; i < 3; i++)
			{
				m_ChannelTextures[i].ReAlloc();
			}
			m_reallocTextures = false;
		}
		if (!(m_nativeContext != IntPtr.Zero) || m_hasFinished || !m_advance)
		{
			return;
		}
		m_elapsedTime += Time.deltaTime;
		if (!(m_elapsedTime > m_nextFrameTime))
		{
			return;
		}
		Decode();
		double nextFrameTime = GetNextFrameTime(m_nativeContext);
		if (nextFrameTime == m_nextFrameTime)
		{
			m_hasFinished = true;
			if (this.onFinished != null)
			{
				this.onFinished(this);
			}
		}
		m_nextFrameTime = nextFrameTime;
	}

	private void Decode()
	{
		DecodeAndUpload(m_nativeContext, m_ChannelTextures[0].tex.GetNativeTextureID(), m_ChannelTextures[1].tex.GetNativeTextureID(), m_ChannelTextures[2].tex.GetNativeTextureID());
	}
}
