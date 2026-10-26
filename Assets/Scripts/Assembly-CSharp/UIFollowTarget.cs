using UnityEngine;

[AddComponentMenu("NGUI/Examples/Follow Target")]
public class UIFollowTarget : MonoBehaviour
{
	public float OffsetX = 0.15f;

	public float OffsetY = 0.15f;

	public Transform target;

	public Vector3 TargetPosition = Vector3.zero;

	public Camera gameCamera;

	public Camera uiCamera;

	public bool disableIfInvisible = true;

	public Vector3 PixelPositionOffest;

	private Vector3 lastPosition = Vector3.zero;

	public bool hurb;

	private Transform mTrans;

	private void Awake()
	{
		mTrans = base.transform;
	}

	private void Start()
	{
		if (target != null)
		{
			if (FindCameras())
			{
				SetVisible(true);
			}
		}
		else
		{
			Debug.LogError("Expected to have 'target' set to a valid transform", this);
			base.enabled = false;
		}
	}

	private void SetVisible(bool val)
	{
		int i = 0;
		for (int childCount = mTrans.childCount; i < childCount; i++)
		{
			NGUITools.SetActive(mTrans.GetChild(i).gameObject, val);
		}
	}

	private void LateUpdate()
	{
		if (CheckCamerasFound())
		{
			Vector3 position = ((!(target != null)) ? TargetPosition : target.position);
			Vector3 vector = gameCamera.WorldToViewportPoint(position);
			float num = (vector.x - 0.5f) * -1f;
			float num2 = (vector.y - 0.5f) * -1f;
			vector.x += num * OffsetX;
			vector.y += num2 * OffsetY;
			vector += PixelPositionOffest;
			bool flag = true;
			bool flag2 = vector != lastPosition;
			lastPosition = vector;
			if (flag2 && flag)
			{
				base.transform.position = uiCamera.ViewportToWorldPoint(vector);
				vector = mTrans.localPosition;
				vector.x = Mathf.FloorToInt(vector.x);
				vector.y = Mathf.FloorToInt(vector.y);
				vector.z = 0f;
				mTrans.localPosition = vector;
			}
			OnUpdate(flag);
		}
		else if (FindCameras())
		{
			SetVisible(true);
		}
	}

	private bool CheckCamerasFound()
	{
		return gameCamera != null && uiCamera != null;
	}

	private bool FindCameras()
	{
		if (target != null && gameCamera == null)
		{
			gameCamera = NGUITools.FindCameraForLayer(target.gameObject.layer);
		}
		if (uiCamera == null)
		{
			uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		return CheckCamerasFound();
	}

	protected virtual void OnUpdate(bool isVisible)
	{
	}
}
