using UnityEngine;

public class VirtualControls
{
	public bool leftDown;

	public bool upDown;

	public bool rightDown;

	public bool attackDown;

	public bool jumpDown;

	private float buttonWidth = 24f;

	private float buttonHeight = 30f;

	private TKAnyTouchRecognizer _leftRecognizer;

	private TKAnyTouchRecognizer _rightRecognizer;

	private TKAnyTouchRecognizer _upRecognizer;

	private TKButtonRecognizer _attackRecognizer;

	private TKButtonRecognizer _jumpRecognizer;

	public TKRect leftRect
	{
		get
		{
			return _leftRecognizer.boundaryFrame.Value;
		}
	}

	public TKRect rightRect
	{
		get
		{
			return _rightRecognizer.boundaryFrame.Value;
		}
	}

	public TKRect upRect
	{
		get
		{
			return _upRecognizer.boundaryFrame.Value;
		}
	}

	public TKRect attackRect
	{
		get
		{
			return _attackRecognizer.boundaryFrame.Value;
		}
	}

	public TKRect jumpRect
	{
		get
		{
			return _jumpRecognizer.boundaryFrame.Value;
		}
	}

	public VirtualControls()
	{
		if (Camera.main.aspect < 1.7f)
		{
			buttonHeight *= Camera.main.aspect / 1.7f;
			buttonWidth *= Camera.main.aspect / 1.7f;
		}
		setupRecognizers();
	}

	public void update()
	{
		leftDown = (upDown = (rightDown = (attackDown = (jumpDown = false))));
		TouchKit.updateTouches();
		leftDown = _leftRecognizer.state == TKGestureRecognizerState.RecognizedAndStillRecognizing;
		rightDown = _rightRecognizer.state == TKGestureRecognizerState.RecognizedAndStillRecognizing;
		upDown = _upRecognizer.state == TKGestureRecognizerState.RecognizedAndStillRecognizing;
		jumpDown = _jumpRecognizer.state == TKGestureRecognizerState.RecognizedAndStillRecognizing;
	}

	public void createDebugQuads()
	{
		createQuadButton(leftRect, Color.green);
		createQuadButton(rightRect, Color.green);
		createQuadButton(upRect, Color.white);
		createQuadButton(attackRect, Color.cyan);
		createQuadButton(jumpRect, Color.magenta);
	}

	private void createQuadButton(TKRect rect, Color color)
	{
		color.a = 0.2f;
		Vector3 position = Camera.main.ScreenToWorldPoint(rect.center);
		position.z = 0f;
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
		gameObject.transform.position = position;
		gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Sprites/Default");
		gameObject.GetComponent<Renderer>().material.color = color;
		gameObject.transform.localScale = new Vector3(TouchKit.instance.pixelsToUnityUnitsMultiplier.x * rect.width, TouchKit.instance.pixelsToUnityUnitsMultiplier.y * rect.height);
	}

	private void setupRecognizers()
	{
		_leftRecognizer = new TKAnyTouchRecognizer(new TKRect(0f, 0f, buttonWidth, buttonHeight));
		TouchKit.addGestureRecognizer(_leftRecognizer);
		_rightRecognizer = new TKAnyTouchRecognizer(new TKRect(buttonWidth + 1f, 0f, buttonWidth, buttonHeight));
		TouchKit.addGestureRecognizer(_rightRecognizer);
		_upRecognizer = new TKAnyTouchRecognizer(new TKRect(0f, buttonHeight * 0.7f, buttonWidth * 2f + 1f, 20f));
		TouchKit.addGestureRecognizer(_upRecognizer);
		_attackRecognizer = new TKButtonRecognizer(new TKRect(TouchKit.instance.designTimeResolution.x - buttonWidth * 2f, 0f, buttonWidth, buttonHeight), 0f);
		_attackRecognizer.onSelectedEvent += delegate
		{
			attackDown = true;
		};
		TouchKit.addGestureRecognizer(_attackRecognizer);
		_jumpRecognizer = new TKButtonRecognizer(new TKRect(TouchKit.instance.designTimeResolution.x - buttonWidth, 0f, buttonWidth, buttonHeight), 0f);
		TouchKit.addGestureRecognizer(_jumpRecognizer);
	}
}
