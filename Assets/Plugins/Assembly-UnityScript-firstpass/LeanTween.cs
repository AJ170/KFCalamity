using System;
using System.Collections;
using Boo.Lang;
using Boo.Lang.Runtime;
using CompilerGenerated;
using UnityEngine;

[Serializable]
public class LeanTween : MonoBehaviour
{
	[NonSerialized]
	public static bool throwErrors = true;

	[NonSerialized]
	private static TweenDescr[] tweens;

	[NonSerialized]
	private static int tweenMaxSearch;

	[NonSerialized]
	private static int maxTweens = 400;

	[NonSerialized]
	private static int frameRendered = -1;

	[NonSerialized]
	private static GameObject tweenEmpty;

	[NonSerialized]
	private static float dtEstimated;

	[NonSerialized]
	private static float dt;

	[NonSerialized]
	private static float dtActual;

	[NonSerialized]
	private static TweenDescr tween;

	[NonSerialized]
	private static int i;

	[NonSerialized]
	private static int j;

	[NonSerialized]
	private static AnimationCurve punch = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.112586f, 0.9976035f), new Keyframe(0.3120486f, -0.1720615f), new Keyframe(0.4316337f, 0.07030682f), new Keyframe(0.5524869f, -0.03141804f), new Keyframe(0.6549395f, 0.003909959f), new Keyframe(0.770987f, -0.009817753f), new Keyframe(0.8838775f, 0.001939224f), new Keyframe(1f, 0f));

	[NonSerialized]
	private static Transform trans;

	[NonSerialized]
	private static Mesh mesh;

	[NonSerialized]
	private static Vector3[] vertices;

	[NonSerialized]
	private static Color32[] colors;

	[NonSerialized]
	private static float timeTotal;

	[NonSerialized]
	private static int tweenAction;

	[NonSerialized]
	private static Hashtable optionalItems;

	[NonSerialized]
	private static ICallable tweenFunc;

	[NonSerialized]
	private static AnimationCurve animationCurve;

	[NonSerialized]
	private static float ratioPassed;

	[NonSerialized]
	private static float val;

	[NonSerialized]
	private static Vector3 fromVect;

	[NonSerialized]
	private static Vector3 toVect;

	[NonSerialized]
	private static Vector3 newVect;

	[NonSerialized]
	private static bool isTweenFinished;

	[NonSerialized]
	private static GameObject target;

	[NonSerialized]
	private static GameObject customTarget;

	[NonSerialized]
	private static int startSearch;

	[NonSerialized]
	private static int lastMax;

	public static void init()
	{
		init(maxTweens);
	}

	public static void init(int maxSimultaneousTweens)
	{
		if (tweens == null)
		{
			maxTweens = maxSimultaneousTweens;
			tweens = new TweenDescr[maxTweens];
			tweenEmpty = new GameObject();
			tweenEmpty.name = "~LeanTween";
			tweenEmpty.AddComponent(typeof(LeanTween));
			tweenEmpty.isStatic = true;
			UnityEngine.Object.DontDestroyOnLoad(tweenEmpty);
			for (i = 0; i < maxTweens; i++)
			{
				tweens[i] = new TweenDescr();
			}
		}
	}

	public virtual void Update()
	{
		update();
	}

	public static void update()
	{
		if (frameRendered == Time.frameCount)
		{
			return;
		}
		init();
		dtEstimated = ((Application.targetFrameRate <= 0) ? (1f / 60f) : (1f / (float)Application.targetFrameRate));
		dtActual = Time.deltaTime * Time.timeScale;
		LeanTween.i = 0;
		while (LeanTween.i < tweenMaxSearch && LeanTween.i < maxTweens)
		{
			if (tweens[LeanTween.i].toggle)
			{
				tween = tweens[LeanTween.i];
				trans = tween.trans as Transform;
				timeTotal = tween.time;
				tweenAction = (int)tween.type;
				tweenFunc = tween.tweenFunc;
				animationCurve = tween.animationCurve;
				optionalItems = tween.optional;
				dt = dtActual;
				if (tween.useEstimatedTime)
				{
					dt = dtEstimated;
				}
				else if (tween.useFrames)
				{
					dt = 1f;
				}
				if (trans == null)
				{
					removeTween(LeanTween.i);
				}
				else
				{
					isTweenFinished = false;
					if (!(tween.passed + dt <= timeTotal))
					{
						isTweenFinished = true;
						tween.passed = timeTotal;
					}
					if (tween.passed == 0f && tweens[LeanTween.i].delay == 0f)
					{
						switch (tweenAction)
						{
						case 15:
							tween.from = trans.position;
							break;
						case 0:
							tween.from.x = trans.position.x;
							break;
						case 1:
							tween.from.x = trans.position.y;
							break;
						case 2:
							tween.from.x = trans.position.z;
							break;
						case 3:
							tweens[LeanTween.i].from.x = trans.localPosition.x;
							break;
						case 4:
							tweens[LeanTween.i].from.x = trans.localPosition.y;
							break;
						case 5:
							tweens[LeanTween.i].from.x = trans.localPosition.z;
							break;
						case 6:
							tween.from.x = trans.localScale.x;
							break;
						case 7:
							tween.from.x = trans.localScale.y;
							break;
						case 8:
							tween.from.x = trans.localScale.z;
							break;
						case 12:
							tween.from.x = trans.gameObject.GetComponent<Renderer>().material.color.a;
							break;
						case 13:
							tween.from.x = (int)((MeshFilter)trans.GetComponent(typeof(MeshFilter))).mesh.colors32[0].a;
							break;
						case 16:
							tween.from = trans.localPosition;
							break;
						case 17:
							tween.from = trans.eulerAngles;
							tween.to.x = closestRot(tween.from.x, tween.to.x);
							tween.to.y = closestRot(tween.from.y, tween.to.y);
							tween.to.z = closestRot(tween.from.z, tween.to.z);
							break;
						case 9:
							tween.from.x = trans.eulerAngles.x;
							tween.to.x = closestRot(tween.from.x, tween.to.x);
							break;
						case 10:
							tween.from.x = trans.eulerAngles.y;
							tween.to.x = closestRot(tween.from.x, tween.to.x);
							break;
						case 11:
							tween.from.x = trans.eulerAngles.z;
							tween.to.x = closestRot(tween.from.x, tween.to.x);
							break;
						case 18:
							tween.from = trans.localEulerAngles;
							tween.to.x = closestRot(tween.from.x, tween.to.x);
							tween.to.y = closestRot(tween.from.y, tween.to.y);
							tween.to.z = closestRot(tween.from.z, tween.to.z);
							break;
						case 19:
							tween.from = trans.localScale;
							break;
						case 20:
							tween.from = new Vector3(tween.ltRect.rect.x, tween.ltRect.rect.y, 0f);
							break;
						case 21:
							tween.from = new Vector3(tween.ltRect.rect.width, tween.ltRect.rect.height, 0f);
							break;
						}
						tween.diff.x = tween.to.x - tween.from.x;
						tween.diff.y = tween.to.y - tween.from.y;
						tween.diff.z = tween.to.z - tween.from.z;
					}
					if (!(tween.delay > 0f))
					{
						ratioPassed = tween.passed / timeTotal;
						if (!(ratioPassed <= 1f))
						{
							ratioPassed = 1f;
						}
						if (tweenAction >= 0 && tweenAction <= 14)
						{
							if (!RuntimeServices.EqualityOperator(animationCurve, null))
							{
								val = tweenOnCurve(tween, ratioPassed);
							}
							else
							{
								switch (tween.tweenType)
								{
								case LeanTweenType.linear:
									val = tween.from.x + tween.diff.x * ratioPassed;
									break;
								case LeanTweenType.easeOutQuad:
									val = easeOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
									break;
								case LeanTweenType.easeInQuad:
									val = easeInQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
									break;
								case LeanTweenType.easeInOutQuad:
									val = easeInOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
									break;
								case LeanTweenType.easeInCubic:
									val = easeInCubic(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeOutCubic:
									val = easeOutCubic(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInOutCubic:
									val = easeInOutCubic(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInQuart:
									val = easeInQuart(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeOutQuart:
									val = easeOutQuart(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInOutQuart:
									val = easeInOutQuart(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInQuint:
									val = easeInQuint(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeOutQuint:
									val = easeOutQuint(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInOutQuint:
									val = easeInOutQuint(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInSine:
									val = easeInSine(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeOutSine:
									val = easeOutSine(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInOutSine:
									val = easeInOutSine(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInExpo:
									val = easeInExpo(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeOutExpo:
									val = easeOutExpo(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInOutExpo:
									val = easeInOutExpo(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInCirc:
									val = easeInCirc(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeOutCirc:
									val = easeOutCirc(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInOutCirc:
									val = easeInOutCirc(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInBounce:
									val = easeInBounce(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeOutBounce:
									val = easeOutBounce(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInOutBounce:
									val = easeInOutBounce(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInBack:
									val = easeInBack(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeOutBack:
									val = easeOutBack(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInOutBack:
									val = easeInOutElastic(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInElastic:
									val = easeInElastic(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeOutElastic:
									val = easeOutElastic(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.easeInOutElastic:
									val = easeInOutElastic(tween.from.x, tween.to.x, ratioPassed);
									break;
								case LeanTweenType.punch:
									tween.animationCurve = punch;
									tween.to.x = tween.from.x + tween.to.x;
									val = tweenOnCurve(tween, ratioPassed);
									break;
								default:
									val = RuntimeServices.UnboxSingle(tweenFunc.Call(new object[3]
									{
										tween.from.x,
										tween.to.x,
										ratioPassed
									}));
									break;
								}
							}
							if (tweenAction == 0)
							{
								float x = val;
								Vector3 position = trans.position;
								float num = (position.x = x);
								Vector3 vector2 = (trans.position = position);
							}
							else if (tweenAction == 1)
							{
								float y = val;
								Vector3 position2 = trans.position;
								float num2 = (position2.y = y);
								Vector3 vector4 = (trans.position = position2);
							}
							else if (tweenAction == 2)
							{
								float z = val;
								Vector3 position3 = trans.position;
								float num3 = (position3.z = z);
								Vector3 vector6 = (trans.position = position3);
							}
							if (tweenAction == 3)
							{
								float x2 = val;
								Vector3 localPosition = trans.localPosition;
								float num4 = (localPosition.x = x2);
								Vector3 vector8 = (trans.localPosition = localPosition);
							}
							else if (tweenAction == 4)
							{
								float y2 = val;
								Vector3 localPosition2 = trans.localPosition;
								float num5 = (localPosition2.y = y2);
								Vector3 vector10 = (trans.localPosition = localPosition2);
							}
							else if (tweenAction == 5)
							{
								float z2 = val;
								Vector3 localPosition3 = trans.localPosition;
								float num6 = (localPosition3.z = z2);
								Vector3 vector12 = (trans.localPosition = localPosition3);
							}
							else if (tweenAction == 6)
							{
								float x3 = val;
								Vector3 localScale = trans.localScale;
								float num7 = (localScale.x = x3);
								Vector3 vector14 = (trans.localScale = localScale);
							}
							else if (tweenAction == 7)
							{
								float y3 = val;
								Vector3 localScale2 = trans.localScale;
								float num8 = (localScale2.y = y3);
								Vector3 vector16 = (trans.localScale = localScale2);
							}
							else if (tweenAction == 8)
							{
								float z3 = val;
								Vector3 localScale3 = trans.localScale;
								float num9 = (localScale3.z = z3);
								Vector3 vector18 = (trans.localScale = localScale3);
							}
							else if (tweenAction == 9)
							{
								float x4 = val;
								Vector3 eulerAngles = trans.eulerAngles;
								float num10 = (eulerAngles.x = x4);
								Vector3 vector20 = (trans.eulerAngles = eulerAngles);
							}
							else if (tweenAction == 10)
							{
								float y4 = val;
								Vector3 eulerAngles2 = trans.eulerAngles;
								float num11 = (eulerAngles2.y = y4);
								Vector3 vector22 = (trans.eulerAngles = eulerAngles2);
							}
							else if (tweenAction == 11)
							{
								float z4 = val;
								Vector3 eulerAngles3 = trans.eulerAngles;
								float num12 = (eulerAngles3.z = z4);
								Vector3 vector24 = (trans.eulerAngles = eulerAngles3);
							}
							else if (tweenAction == 12)
							{
								float a = val;
								Color color = trans.gameObject.GetComponent<Renderer>().material.color;
								float num13 = (color.a = a);
								Color color3 = (trans.gameObject.GetComponent<Renderer>().material.color = color);
							}
							else if (tweenAction == 13)
							{
								mesh = ((MeshFilter)trans.GetComponent(typeof(MeshFilter))).mesh;
								vertices = mesh.vertices;
								colors = new Color32[vertices.Length];
								Color32 color4 = mesh.colors32[0];
								color4.a = (byte)val;
								for (int i = 0; i < vertices.Length; i++)
								{
									colors[i] = color4;
								}
								mesh.colors32 = colors;
							}
						}
						else if (tweenAction >= 15)
						{
							if (!RuntimeServices.EqualityOperator(animationCurve, null))
							{
								newVect = tweenOnCurveVector(tween, ratioPassed);
							}
							else if (tween.tweenType == LeanTweenType.linear)
							{
								newVect.x = tween.from.x + tween.diff.x * ratioPassed;
								newVect.y = tween.from.y + tween.diff.y * ratioPassed;
								newVect.z = tween.from.z + tween.diff.z * ratioPassed;
							}
							else if (tween.tweenType >= LeanTweenType.linear)
							{
								switch (tween.tweenType)
								{
								case LeanTweenType.easeOutQuad:
									newVect = new Vector3(easeOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeOutQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeOutQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
									break;
								case LeanTweenType.easeInQuad:
									newVect = new Vector3(easeInQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeInQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeInQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
									break;
								case LeanTweenType.easeInOutQuad:
									newVect = new Vector3(easeInOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeInOutQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeInOutQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
									break;
								case LeanTweenType.easeInCubic:
									newVect = new Vector3(easeInCubic(tween.from.x, tween.to.x, ratioPassed), easeInCubic(tween.from.y, tween.to.y, ratioPassed), easeInCubic(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeOutCubic:
									newVect = new Vector3(easeOutCubic(tween.from.x, tween.to.x, ratioPassed), easeOutCubic(tween.from.y, tween.to.y, ratioPassed), easeOutCubic(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInOutCubic:
									newVect = new Vector3(easeInOutCubic(tween.from.x, tween.to.x, ratioPassed), easeInOutCubic(tween.from.y, tween.to.y, ratioPassed), easeInOutCubic(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInQuart:
									newVect = new Vector3(easeInQuart(tween.from.x, tween.to.x, ratioPassed), easeInQuart(tween.from.y, tween.to.y, ratioPassed), easeInQuart(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeOutQuart:
									newVect = new Vector3(easeOutQuart(tween.from.x, tween.to.x, ratioPassed), easeOutQuart(tween.from.y, tween.to.y, ratioPassed), easeOutQuart(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInOutQuart:
									newVect = new Vector3(easeInOutQuart(tween.from.x, tween.to.x, ratioPassed), easeInOutQuart(tween.from.y, tween.to.y, ratioPassed), easeInOutQuart(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInQuint:
									newVect = new Vector3(easeInQuint(tween.from.x, tween.to.x, ratioPassed), easeInQuint(tween.from.y, tween.to.y, ratioPassed), easeInQuint(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeOutQuint:
									newVect = new Vector3(easeOutQuint(tween.from.x, tween.to.x, ratioPassed), easeOutQuint(tween.from.y, tween.to.y, ratioPassed), easeOutQuint(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInOutQuint:
									newVect = new Vector3(easeInOutQuint(tween.from.x, tween.to.x, ratioPassed), easeInOutQuint(tween.from.y, tween.to.y, ratioPassed), easeInOutQuint(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInSine:
									newVect = new Vector3(easeInSine(tween.from.x, tween.to.x, ratioPassed), easeInSine(tween.from.y, tween.to.y, ratioPassed), easeInSine(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeOutSine:
									newVect = new Vector3(easeOutSine(tween.from.x, tween.to.x, ratioPassed), easeOutSine(tween.from.y, tween.to.y, ratioPassed), easeOutSine(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInOutSine:
									newVect = new Vector3(easeInOutSine(tween.from.x, tween.to.x, ratioPassed), easeInOutSine(tween.from.y, tween.to.y, ratioPassed), easeInOutSine(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInExpo:
									newVect = new Vector3(easeInExpo(tween.from.x, tween.to.x, ratioPassed), easeInExpo(tween.from.y, tween.to.y, ratioPassed), easeInExpo(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeOutExpo:
									newVect = new Vector3(easeOutExpo(tween.from.x, tween.to.x, ratioPassed), easeOutExpo(tween.from.y, tween.to.y, ratioPassed), easeOutExpo(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInOutExpo:
									newVect = new Vector3(easeInOutExpo(tween.from.x, tween.to.x, ratioPassed), easeInOutExpo(tween.from.y, tween.to.y, ratioPassed), easeInOutExpo(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInCirc:
									newVect = new Vector3(easeInCirc(tween.from.x, tween.to.x, ratioPassed), easeInCirc(tween.from.y, tween.to.y, ratioPassed), easeInCirc(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeOutCirc:
									newVect = new Vector3(easeOutCirc(tween.from.x, tween.to.x, ratioPassed), easeOutCirc(tween.from.y, tween.to.y, ratioPassed), easeOutCirc(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInOutCirc:
									newVect = new Vector3(easeInOutCirc(tween.from.x, tween.to.x, ratioPassed), easeInOutCirc(tween.from.y, tween.to.y, ratioPassed), easeInOutCirc(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInBounce:
									newVect = new Vector3(easeInBounce(tween.from.x, tween.to.x, ratioPassed), easeInBounce(tween.from.y, tween.to.y, ratioPassed), easeInBounce(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeOutBounce:
									newVect = new Vector3(easeOutBounce(tween.from.x, tween.to.x, ratioPassed), easeOutBounce(tween.from.y, tween.to.y, ratioPassed), easeOutBounce(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInOutBounce:
									newVect = new Vector3(easeInOutBounce(tween.from.x, tween.to.x, ratioPassed), easeInOutBounce(tween.from.y, tween.to.y, ratioPassed), easeInOutBounce(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInBack:
									newVect = new Vector3(easeInBack(tween.from.x, tween.to.x, ratioPassed), easeInBack(tween.from.y, tween.to.y, ratioPassed), easeInBack(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeOutBack:
									newVect = new Vector3(easeOutBack(tween.from.x, tween.to.x, ratioPassed), easeOutBack(tween.from.y, tween.to.y, ratioPassed), easeOutBack(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInOutBack:
									newVect = new Vector3(easeInOutBack(tween.from.x, tween.to.x, ratioPassed), easeInOutBack(tween.from.y, tween.to.y, ratioPassed), easeInOutBack(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInElastic:
									newVect = new Vector3(easeInElastic(tween.from.x, tween.to.x, ratioPassed), easeInElastic(tween.from.y, tween.to.y, ratioPassed), easeInElastic(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeOutElastic:
									newVect = new Vector3(easeOutElastic(tween.from.x, tween.to.x, ratioPassed), easeOutElastic(tween.from.y, tween.to.y, ratioPassed), easeOutElastic(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.easeInOutElastic:
									newVect = new Vector3(easeInOutElastic(tween.from.x, tween.to.x, ratioPassed), easeInOutElastic(tween.from.y, tween.to.y, ratioPassed), easeInOutElastic(tween.from.z, tween.to.z, ratioPassed));
									break;
								case LeanTweenType.punch:
									tween.animationCurve = punch;
									tween.to.x = tween.from.x + tween.to.x;
									tween.to.y = tween.from.y + tween.to.y;
									tween.to.z = tween.from.z + tween.to.z;
									if (tweenAction == 17 || tweenAction == 18)
									{
										tween.to.x = closestRot(tween.from.x, tween.to.x);
										tween.to.y = closestRot(tween.from.y, tween.to.y);
										tween.to.z = closestRot(tween.from.z, tween.to.z);
									}
									newVect = tweenOnCurveVector(tween, ratioPassed);
									break;
								}
							}
							else
							{
								fromVect = tween.from;
								toVect = tween.to;
								newVect.x = RuntimeServices.UnboxSingle(tweenFunc.Call(new object[3] { fromVect.x, toVect.x, ratioPassed }));
								newVect.y = RuntimeServices.UnboxSingle(tweenFunc.Call(new object[3] { fromVect.y, toVect.y, ratioPassed }));
								newVect.z = RuntimeServices.UnboxSingle(tweenFunc.Call(new object[3] { fromVect.z, toVect.z, ratioPassed }));
							}
							if (tweenAction == 15)
							{
								trans.position = newVect;
							}
							else if (tweenAction == 16)
							{
								trans.localPosition = newVect;
							}
							else if (tweenAction == 17)
							{
								trans.eulerAngles = newVect;
							}
							else if (tweenAction == 18)
							{
								trans.localEulerAngles = newVect;
							}
							else if (tweenAction == 19)
							{
								trans.localScale = newVect;
							}
							else if (tweenAction == 20)
							{
								tween.ltRect.rect.x = newVect.x;
								tween.ltRect.rect.y = newVect.y;
							}
							else if (tweenAction == 21)
							{
								tween.ltRect.rect.width = newVect.x;
								tween.ltRect.rect.height = newVect.y;
							}
						}
						if (!RuntimeServices.EqualityOperator(tween.optional, null))
						{
							object obj = optionalItems["onUpdate"];
							if (!RuntimeServices.EqualityOperator(obj, null))
							{
								object obj2 = optionalItems["onUpdateParam"];
								if (!(obj2 is Hashtable))
								{
									obj2 = RuntimeServices.Coerce(obj2, typeof(Hashtable));
								}
								Hashtable hashtable = (Hashtable)obj2;
								if (RuntimeServices.EqualityOperator(obj.GetType(), typeof(string)))
								{
									string methodName = obj as string;
									if (!RuntimeServices.EqualityOperator(optionalItems["onUpdateTarget"], null))
									{
										object obj3 = optionalItems["onUpdateTarget"];
										if (!(obj3 is GameObject))
										{
											obj3 = RuntimeServices.Coerce(obj3, typeof(GameObject));
										}
										customTarget = (GameObject)obj3;
										customTarget.BroadcastMessage(methodName, val);
									}
									else
									{
										trans.gameObject.BroadcastMessage(methodName, val);
									}
								}
								else
								{
									ICallable callable = obj as ICallable;
									if (!RuntimeServices.EqualityOperator(hashtable, null))
									{
										callable.Call(new object[2] { val, hashtable });
									}
									else
									{
										callable.Call(new object[1] { val });
									}
								}
							}
						}
					}
					if (isTweenFinished)
					{
						ICallable callable2 = null;
						string text = null;
						object obj4 = null;
						if (!RuntimeServices.EqualityOperator(tween.optional, null) && tween.trans != null)
						{
							if (!RuntimeServices.EqualityOperator(optionalItems["onComplete"], null))
							{
								if (RuntimeServices.EqualityOperator(optionalItems["onComplete"].GetType(), typeof(string)))
								{
									text = optionalItems["onComplete"] as string;
								}
								else
								{
									callable2 = optionalItems["onComplete"] as ICallable;
								}
							}
							obj4 = optionalItems["onCompleteParam"];
						}
						removeTween(LeanTween.i);
						if (!RuntimeServices.EqualityOperator(callable2, null))
						{
							if (RuntimeServices.ToBool(obj4))
							{
								callable2.Call(new object[1] { obj4 });
							}
							else
							{
								callable2.Call(new object[0]);
							}
						}
						else if (text != null)
						{
							if (!RuntimeServices.EqualityOperator(optionalItems["onCompleteTarget"], null))
							{
								object obj5 = optionalItems["onCompleteTarget"];
								if (!(obj5 is GameObject))
								{
									obj5 = RuntimeServices.Coerce(obj5, typeof(GameObject));
								}
								customTarget = (GameObject)obj5;
								if (!RuntimeServices.EqualityOperator(obj4, null))
								{
									customTarget.BroadcastMessage(text, obj4);
								}
								else
								{
									customTarget.BroadcastMessage(text);
								}
							}
							else if (!RuntimeServices.EqualityOperator(obj4, null))
							{
								trans.gameObject.BroadcastMessage(text, obj4);
							}
							else
							{
								trans.gameObject.BroadcastMessage(text);
							}
						}
					}
					else if (!(tween.delay > 0f))
					{
						tween.passed += dt;
					}
					else
					{
						tween.delay -= dt;
						if (!(tween.delay >= 0f))
						{
							tween.passed = 0f;
							tween.delay = 0f;
						}
					}
				}
			}
			LeanTween.i++;
		}
		frameRendered = Time.frameCount;
	}

	private static void removeTween(int i)
	{
		tweens[i].toggle = false;
		startSearch = i;
		if (i + 1 >= tweenMaxSearch)
		{
			startSearch = 0;
			tweenMaxSearch--;
		}
	}

	private static int pushNewTween(GameObject gameObject, Vector3 to, float time, TweenAction TweenAction, Hashtable optional)
	{
		init(maxTweens);
		int result;
		if (gameObject == null)
		{
			result = -1;
		}
		else
		{
			j = 0;
			i = startSearch;
			while (true)
			{
				if (j < maxTweens)
				{
					if (i >= maxTweens - 1)
					{
						i = 0;
					}
					if (tweens[i].toggle)
					{
						j++;
						if (j >= maxTweens)
						{
							string message = "LeanTween - You have run out of available spaces for tweening. To avoid this error increase the number of spaces to available for tweening when you initialize the LeanTween class ex: LeanTween.init( " + maxTweens * 2 + " );";
							if (throwErrors)
							{
								Debug.LogError(message);
							}
							else
							{
								Debug.Log(message);
							}
							result = -1;
							break;
						}
						i++;
						continue;
					}
					if (i + 1 > tweenMaxSearch)
					{
						tweenMaxSearch = i + 1;
					}
					startSearch = i + 1;
				}
				tween = tweens[i];
				tween.toggle = true;
				tween.trans = gameObject.transform;
				tween.to = to;
				tween.time = time;
				tween.passed = 0f;
				tween.type = TweenAction;
				tween.optional = optional;
				tween.delay = 0f;
				tween.id = i;
				tween.useEstimatedTime = false;
				tween.useFrames = false;
				tween.animationCurve = null;
				tween.tweenType = LeanTweenType.linear;
				if (!RuntimeServices.EqualityOperator(optional, null))
				{
					object obj = optional["ease"];
					int num = 0;
					if (!RuntimeServices.EqualityOperator(obj, null))
					{
						tween.tweenType = LeanTweenType.notUsed;
						if (RuntimeServices.EqualityOperator(obj.GetType(), typeof(LeanTweenType)))
						{
							tween.tweenType = (LeanTweenType)obj;
						}
						else if (RuntimeServices.EqualityOperator(obj.GetType(), typeof(AnimationCurve)))
						{
							tween.animationCurve = optional["ease"] as AnimationCurve;
						}
						else
						{
							tween.tweenFunc = optional["ease"] as ICallable;
							if (RuntimeServices.EqualityOperator(tween.tweenFunc, new __LeanTween_pushNewTween_0024callable0_0024792_63__(easeOutQuad)))
							{
								tween.tweenType = LeanTweenType.easeOutQuad;
							}
							else if (RuntimeServices.EqualityOperator(tween.tweenFunc, new __LeanTween_pushNewTween_0024callable0_0024792_63__(easeInQuad)))
							{
								tween.tweenType = LeanTweenType.easeInQuad;
							}
							else if (RuntimeServices.EqualityOperator(tween.tweenFunc, new __LeanTween_pushNewTween_0024callable0_0024792_63__(easeInOutQuad)))
							{
								tween.tweenType = LeanTweenType.easeInOutQuad;
							}
						}
						num++;
					}
					if (!RuntimeServices.EqualityOperator(optional["rect"], null))
					{
						TweenDescr tweenDescr = tween;
						object obj2 = optional["rect"];
						if (!(obj2 is LTRect))
						{
							obj2 = RuntimeServices.Coerce(obj2, typeof(LTRect));
						}
						tweenDescr.ltRect = (LTRect)obj2;
						num++;
					}
					if (!RuntimeServices.EqualityOperator(optional["delay"], null))
					{
						tween.delay = RuntimeServices.UnboxSingle(optional["delay"]);
						num++;
					}
					if (!RuntimeServices.EqualityOperator(optional["useEstimatedTime"], null))
					{
						tween.useEstimatedTime = RuntimeServices.UnboxBoolean(optional["useEstimatedTime"]);
						num++;
					}
					if (!RuntimeServices.EqualityOperator(optional["useFrames"], null))
					{
						tween.useFrames = RuntimeServices.UnboxBoolean(optional["useFrames"]);
						num++;
					}
					if (optional.Count <= num)
					{
						tween.optional = null;
					}
				}
				result = tweens[i].id;
				break;
			}
		}
		return result;
	}

	public static float closestRot(float from, float to)
	{
		float num = 0f - (360f - to);
		float num2 = 360f + to;
		float num3 = Mathf.Abs(to - from);
		float num4 = Mathf.Abs(num - from);
		float num5 = Mathf.Abs(num2 - from);
		return (!(num3 >= num4) && !(num3 >= num5)) ? to : ((num4 >= num5) ? num2 : num);
	}

	public static void cancel(GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if ((object)tweens[i].trans == transform)
			{
				removeTween(i);
			}
		}
	}

	public static void cancel(GameObject gameObject, int id)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if ((object)tweens[i].trans == transform && tweens[i].id == id)
			{
				removeTween(i);
			}
		}
	}

	public static bool isTweening(GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		i = 0;
		int result;
		while (true)
		{
			if (i < tweenMaxSearch)
			{
				if (tweens[i].toggle && (object)tweens[i].trans == transform)
				{
					result = 1;
					break;
				}
				i++;
				continue;
			}
			result = 0;
			break;
		}
		return (byte)result != 0;
	}

	public static bool isTweening(LTRect ltRect)
	{
		i = 0;
		int result;
		while (true)
		{
			if (i < tweenMaxSearch)
			{
				if (tweens[i].toggle && tweens[i].ltRect == ltRect)
				{
					result = 1;
					break;
				}
				i++;
				continue;
			}
			result = 0;
			break;
		}
		return (byte)result != 0;
	}

	public virtual int play(GameObject gameObject, int toFrame, int columns, int rows, Hashtable optional)
	{
		return 0;
	}

	public static int value(ICallable callOnUpdate, float from, float to, float time, Hashtable optional)
	{
		return value(tweenEmpty, callOnUpdate, from, to, time, optional);
	}

	public static int value(GameObject gameObject, string callOnUpdate, float from, float to, float time, Hashtable optional)
	{
		if (RuntimeServices.EqualityOperator(optional, null))
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, optional);
		tweens[num].from = new Vector3(from, 0f, 0f);
		return num;
	}

	public static int value(GameObject gameObject, ICallable callOnUpdate, float from, float to, float time)
	{
		return value(gameObject, callOnUpdate, from, to, time, null);
	}

	public static int value(GameObject gameObject, ICallable callOnUpdate, float from, float to, float time, Hashtable optional)
	{
		if (RuntimeServices.EqualityOperator(optional, null))
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, optional);
		tweens[num].from = new Vector3(from, 0f, 0f);
		return num;
	}

	public static int rotate(GameObject gameObject, Vector3 to, float time)
	{
		return rotate(gameObject, to, time, null);
	}

	public static int rotate(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.ROTATE, optional);
	}

	public static int rotateX(GameObject gameObject, float to, float time)
	{
		return rotateX(gameObject, to, time, null);
	}

	public static int rotateX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_X, optional);
	}

	public static int rotateY(GameObject gameObject, float to, float time)
	{
		return rotateY(gameObject, to, time, null);
	}

	public static int rotateY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_Y, optional);
	}

	public static int rotateZ(GameObject gameObject, float to, float time)
	{
		return rotateZ(gameObject, to, time, null);
	}

	public static int rotateZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_Z, optional);
	}

	public static int rotateLocal(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.ROTATE_LOCAL, optional);
	}

	public static int moveX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_X, optional);
	}

	public static int moveY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_Y, optional);
	}

	public static int moveZ(GameObject gameObject, float to, float time)
	{
		return moveZ(gameObject, to, time, null);
	}

	public static int moveZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_Z, optional);
	}

	public static int move(GameObject gameObject, Vector3 to, float time)
	{
		return move(gameObject, to, time, null);
	}

	public static int move(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.MOVE, optional);
	}

	public static int move(LTRect ltRect, Vector2 to, float time, Hashtable optional)
	{
		init();
		if (RuntimeServices.EqualityOperator(optional, null))
		{
			optional = new Hashtable();
		}
		optional["rect"] = ltRect;
		return pushNewTween(tweenEmpty, to, time, TweenAction.GUI_MOVE, optional);
	}

	public static int move(LTRect ltRect, Vector2 to, float time)
	{
		return move(ltRect, to, time, null);
	}

	public static int moveLocal(GameObject gameObject, Vector3 to, float time)
	{
		return moveLocal(gameObject, to, time, null);
	}

	public static int moveLocal(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.MOVE_LOCAL, optional);
	}

	public static int moveLocalX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_X, optional);
	}

	public static int moveLocalY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_Y, optional);
	}

	public static int moveLocalZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_Z, optional);
	}

	public static int scale(GameObject gameObject, Vector3 to, float time)
	{
		return scale(gameObject, to, time, null);
	}

	public static int scale(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.SCALE, optional);
	}

	public static int scale(LTRect ltRect, Vector2 to, float time, Hashtable optional)
	{
		init();
		if (RuntimeServices.EqualityOperator(optional, null))
		{
			optional = new Hashtable();
		}
		optional["rect"] = ltRect;
		return pushNewTween(tweenEmpty, to, time, TweenAction.GUI_SCALE, optional);
	}

	public static int scale(LTRect ltRect, Vector2 to, float time)
	{
		return scale(ltRect, to, time, null);
	}

	public static int scaleX(GameObject gameObject, float to, float time)
	{
		return scaleX(gameObject, to, time, null);
	}

	public static int scaleX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_X, optional);
	}

	public static int scaleY(GameObject gameObject, float to, float time)
	{
		return scaleY(gameObject, to, time, null);
	}

	public static int scaleY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_Y, optional);
	}

	public static int scaleZ(GameObject gameObject, float to, float time)
	{
		return scaleZ(gameObject, to, time, null);
	}

	public static int scaleZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_Z, optional);
	}

	public static int delayedCall(float delayTime, ICallable callback)
	{
		return delayedCall(tweenEmpty, delayTime, callback, null);
	}

	public static int delayedCall(float delayTime, ICallable callback, Hashtable optional)
	{
		return delayedCall(tweenEmpty, delayTime, callback, optional);
	}

	public static int delayedCall(GameObject gameObject, float delayTime, ICallable callback)
	{
		return delayedCall(gameObject, delayTime, callback, null);
	}

	public static int delayedCall(GameObject gameObject, float delayTime, ICallable callback, Hashtable optional)
	{
		if (RuntimeServices.EqualityOperator(optional, null))
		{
			optional = new Hashtable();
		}
		optional["onComplete"] = callback;
		return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional);
	}

	public static int delayedCall(GameObject gameObject, float delayTime, string callback)
	{
		return delayedCall(gameObject, delayTime, callback, null);
	}

	public static int delayedCall(GameObject gameObject, float delayTime, string callback, Hashtable optional)
	{
		if (RuntimeServices.EqualityOperator(optional, null))
		{
			optional = new Hashtable();
		}
		optional["onComplete"] = callback;
		return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional);
	}

	public static int alpha(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ALPHA, optional);
	}

	public static int alpha(GameObject gameObject, float to, float time)
	{
		return alpha(gameObject, to, time, null);
	}

	public static int alphaVertex(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ALPHA_VERTEX, optional);
	}

	public static int alphaVertex(GameObject gameObject, float to, float time)
	{
		return alphaVertex(gameObject, to, time, null);
	}

	private static float tweenOnCurve(TweenDescr tweenDescr, float ratioPassed)
	{
		return tweenDescr.from.x + (tweenDescr.to.x - tweenDescr.from.x) * tweenDescr.animationCurve.Evaluate(ratioPassed);
	}

	private static Vector3 tweenOnCurveVector(TweenDescr tweenDescr, float ratioPassed)
	{
		return new Vector3(tweenDescr.from.x + (tweenDescr.to.x - tweenDescr.from.x) * tweenDescr.animationCurve.Evaluate(ratioPassed), tweenDescr.from.y + (tweenDescr.to.y - tweenDescr.from.y) * tweenDescr.animationCurve.Evaluate(ratioPassed), tweenDescr.from.z + (tweenDescr.to.z - tweenDescr.from.z) * tweenDescr.animationCurve.Evaluate(ratioPassed));
	}

	public static float easeOutQuadOpt(float start, float diff, float ratioPassed)
	{
		return (0f - diff) * ratioPassed * (ratioPassed - 2f) + start;
	}

	public static float easeInQuadOpt(float start, float diff, float ratioPassed)
	{
		return diff * ratioPassed * ratioPassed + start;
	}

	public static float easeInOutQuadOpt(float start, float diff, float ratioPassed)
	{
		ratioPassed /= 0.5f;
		float result;
		if (!(ratioPassed >= 1f))
		{
			result = diff / 2f * ratioPassed * ratioPassed + start;
		}
		else
		{
			ratioPassed -= 1f;
			result = (0f - diff) / 2f * (ratioPassed * (ratioPassed - 2f) - 1f) + start;
		}
		return result;
	}

	public static float linear(float start, float end, float val)
	{
		return Mathf.Lerp(start, end, val);
	}

	public static float clerp(float start, float end, float val)
	{
		float num = 0f;
		float num2 = 360f;
		float num3 = Mathf.Abs((num2 - num) / 2f);
		float num4 = 0f;
		float num5 = 0f;
		if (!(end - start >= 0f - num3))
		{
			num5 = (num2 - start + end) * val;
			return start + num5;
		}
		if (!(end - start <= num3))
		{
			num5 = (0f - (num2 - end + start)) * val;
			return start + num5;
		}
		return start + (end - start) * val;
	}

	public static float spring(float start, float end, float val)
	{
		val = Mathf.Clamp01(val);
		val = (Mathf.Sin(val * (float)Math.PI * (0.2f + 2.5f * val * val * val)) * Mathf.Pow(1f - val, 2.2f) + val) * (1f + 1.2f * (1f - val));
		return start + (end - start) * val;
	}

	public static float easeInQuad(float start, float end, float val)
	{
		end -= start;
		return end * val * val + start;
	}

	public static float easeOutQuad(float start, float end, float val)
	{
		end -= start;
		return (0f - end) * val * (val - 2f) + start;
	}

	public static float easeInOutQuad(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		float result;
		if (!(val >= 1f))
		{
			result = end / 2f * val * val + start;
		}
		else
		{
			val -= 1f;
			result = (0f - end) / 2f * (val * (val - 2f) - 1f) + start;
		}
		return result;
	}

	public static float easeInCubic(float start, float end, float val)
	{
		end -= start;
		return end * val * val * val + start;
	}

	public static float easeOutCubic(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return end * (val * val * val + 1f) + start;
	}

	public static float easeInOutCubic(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		float result;
		if (!(val >= 1f))
		{
			result = end / 2f * val * val * val + start;
		}
		else
		{
			val -= 2f;
			result = end / 2f * (val * val * val + 2f) + start;
		}
		return result;
	}

	public static float easeInQuart(float start, float end, float val)
	{
		end -= start;
		return end * val * val * val * val + start;
	}

	public static float easeOutQuart(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return (0f - end) * (val * val * val * val - 1f) + start;
	}

	public static float easeInOutQuart(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		float result;
		if (!(val >= 1f))
		{
			result = end / 2f * val * val * val * val + start;
		}
		else
		{
			val -= 2f;
			result = (0f - end) / 2f * (val * val * val * val - 2f) + start;
		}
		return result;
	}

	public static float easeInQuint(float start, float end, float val)
	{
		end -= start;
		return end * val * val * val * val * val + start;
	}

	public static float easeOutQuint(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return end * (val * val * val * val * val + 1f) + start;
	}

	public static float easeInOutQuint(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		float result;
		if (!(val >= 1f))
		{
			result = end / 2f * val * val * val * val * val + start;
		}
		else
		{
			val -= 2f;
			result = end / 2f * (val * val * val * val * val + 2f) + start;
		}
		return result;
	}

	public static float easeInSine(float start, float end, float val)
	{
		end -= start;
		return (0f - end) * Mathf.Cos(val / 1f * ((float)Math.PI / 2f)) + end + start;
	}

	public static float easeOutSine(float start, float end, float val)
	{
		end -= start;
		return end * Mathf.Sin(val / 1f * ((float)Math.PI / 2f)) + start;
	}

	public static float easeInOutSine(float start, float end, float val)
	{
		end -= start;
		return (0f - end) / 2f * (Mathf.Cos((float)Math.PI * val / 1f) - 1f) + start;
	}

	public static float easeInExpo(float start, float end, float val)
	{
		end -= start;
		return end * Mathf.Pow(2f, 10f * (val / 1f - 1f)) + start;
	}

	public static float easeOutExpo(float start, float end, float val)
	{
		end -= start;
		return end * (0f - Mathf.Pow(2f, -10f * val / 1f) + 1f) + start;
	}

	public static float easeInOutExpo(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		float result;
		if (!(val >= 1f))
		{
			result = end / 2f * Mathf.Pow(2f, 10f * (val - 1f)) + start;
		}
		else
		{
			val -= 1f;
			result = end / 2f * (0f - Mathf.Pow(2f, -10f * val) + 2f) + start;
		}
		return result;
	}

	public static float easeInCirc(float start, float end, float val)
	{
		end -= start;
		return (0f - end) * (Mathf.Sqrt(1f - val * val) - 1f) + start;
	}

	public static float easeOutCirc(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return end * Mathf.Sqrt(1f - val * val) + start;
	}

	public static float easeInOutCirc(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		float result;
		if (!(val >= 1f))
		{
			result = (0f - end) / 2f * (Mathf.Sqrt(1f - val * val) - 1f) + start;
		}
		else
		{
			val -= 2f;
			result = end / 2f * (Mathf.Sqrt(1f - val * val) + 1f) + start;
		}
		return result;
	}

	public static float easeInBounce(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		return end - easeOutBounce(0f, end, num - val) + start;
	}

	public static float easeOutBounce(float start, float end, float val)
	{
		val /= 1f;
		end -= start;
		float result;
		if (!(val >= 0.36363637f))
		{
			result = end * (7.5625f * val * val) + start;
		}
		else if (!(val >= 0.72727275f))
		{
			val -= 0.54545456f;
			result = end * (7.5625f * val * val + 0.75f) + start;
		}
		else if (!(val >= 0.90909094f))
		{
			val -= 0.8181818f;
			result = end * (7.5625f * val * val + 0.9375f) + start;
		}
		else
		{
			val -= 21f / 22f;
			result = end * (7.5625f * val * val + 63f / 64f) + start;
		}
		return result;
	}

	public static float easeInOutBounce(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		return (val >= num / 2f) ? (easeOutBounce(0f, end, val * 2f - num) * 0.5f + end * 0.5f + start) : (easeInBounce(0f, end, val * 2f) * 0.5f + start);
	}

	public static float easeInBack(float start, float end, float val)
	{
		end -= start;
		val /= 1f;
		float num = 1.70158f;
		return end * val * val * ((num + 1f) * val - num) + start;
	}

	public static float easeOutBack(float start, float end, float val)
	{
		float num = 1.70158f;
		end -= start;
		val = val / 1f - 1f;
		return end * (val * val * ((num + 1f) * val + num) + 1f) + start;
	}

	public static float easeInOutBack(float start, float end, float val)
	{
		float num = 1.70158f;
		end -= start;
		val /= 0.5f;
		float result;
		if (!(val >= 1f))
		{
			num *= 1.525f;
			result = end / 2f * (val * val * ((num + 1f) * val - num)) + start;
		}
		else
		{
			val -= 2f;
			num *= 1.525f;
			result = end / 2f * (val * val * ((num + 1f) * val + num) + 2f) + start;
		}
		return result;
	}

	public static float easeInElastic(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		float num4 = 0f;
		float result;
		if (val == 0f)
		{
			result = start;
		}
		else
		{
			val /= num;
			if (val == 1f)
			{
				result = start + end;
			}
			else
			{
				if (num4 == 0f || !(num4 >= Mathf.Abs(end)))
				{
					num4 = end;
					num3 = num2 / 4f;
				}
				else
				{
					num3 = num2 / ((float)Math.PI * 2f) * Mathf.Asin(end / num4);
				}
				val -= 1f;
				result = 0f - num4 * Mathf.Pow(2f, 10f * val) * Mathf.Sin((val * num - num3) * ((float)Math.PI * 2f) / num2) + start;
			}
		}
		return result;
	}

	public static float easeOutElastic(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		float num4 = 0f;
		float result;
		if (val == 0f)
		{
			result = start;
		}
		else
		{
			val /= num;
			if (val == 1f)
			{
				result = start + end;
			}
			else
			{
				if (num4 == 0f || !(num4 >= Mathf.Abs(end)))
				{
					num4 = end;
					num3 = num2 / 4f;
				}
				else
				{
					num3 = num2 / ((float)Math.PI * 2f) * Mathf.Asin(end / num4);
				}
				result = num4 * Mathf.Pow(2f, -10f * val) * Mathf.Sin((val * num - num3) * ((float)Math.PI * 2f) / num2) + end + start;
			}
		}
		return result;
	}

	public static float easeInOutElastic(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		float num4 = 0f;
		float result;
		if (val == 0f)
		{
			result = start;
		}
		else
		{
			val /= num / 2f;
			if (val == 2f)
			{
				result = start + end;
			}
			else
			{
				if (num4 == 0f || !(num4 >= Mathf.Abs(end)))
				{
					num4 = end;
					num3 = num2 / 4f;
				}
				else
				{
					num3 = num2 / ((float)Math.PI * 2f) * Mathf.Asin(end / num4);
				}
				if (!(val >= 1f))
				{
					val -= 1f;
					result = -0.5f * (num4 * Mathf.Pow(2f, 10f * val) * Mathf.Sin((val * num - num3) * ((float)Math.PI * 2f) / num2)) + start;
				}
				else
				{
					val -= 1f;
					result = num4 * Mathf.Pow(2f, -10f * val) * Mathf.Sin((val * num - num3) * ((float)Math.PI * 2f) / num2) * 0.5f + end + start;
				}
			}
		}
		return result;
	}
}
