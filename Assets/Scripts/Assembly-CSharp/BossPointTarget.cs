using UnityEngine;

public class BossPointTarget : MonoBehaviour
{
	public UISprite Arrow;

	public Transform target;

	public Camera gameCamera;

	public Camera uiCamera;

	public bool disableIfInvisible = true;

	public Vector3 PixelPositionOffest;

	private Transform mTrans;

	private bool mIsVisible;

	private void Awake()
	{
		mTrans = base.transform;
	}

	private void Start()
	{
		if (target != null)
		{
			if (gameCamera == null)
			{
				gameCamera = NGUITools.FindCameraForLayer(target.gameObject.layer);
			}
			if (uiCamera == null)
			{
				uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
			}
			SetVisible(false);
		}
		else
		{
			Debug.LogError("Expected to have 'target' set to a valid transform", this);
			base.enabled = false;
		}
	}

	private void SetVisible(bool val)
	{
		mIsVisible = val;
		int i = 0;
		for (int childCount = mTrans.childCount; i < childCount; i++)
		{
			NGUITools.SetActive(mTrans.GetChild(i).gameObject, val);
		}
	}

	private void LateUpdate()
	{
		Vector3 position = target.position + PixelPositionOffest;
		Vector3 position2 = gameCamera.WorldToViewportPoint(position);
		bool flag = true;
		if (mIsVisible != flag)
		{
			SetVisible(flag);
		}
		if (flag)
		{
			position2.x = Mathf.Clamp01(position2.x);
			position2.y = Mathf.Clamp01(position2.y);
			position2.z = 0f;
			position = uiCamera.ViewportToWorldPoint(position2);
			position.z = 0f;
			if (position != mTrans.position)
			{
				base.transform.position = position;
			}
			if (!(Arrow != null))
			{
			}
		}
		OnUpdate(flag);
	}

	protected virtual void OnUpdate(bool isVisible)
	{
	}
}
