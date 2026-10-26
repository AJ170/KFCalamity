using System;
using System.Collections;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class TweenDescr
{
	public bool toggle;

	public Transform trans;

	public LTRect ltRect;

	public Vector3 from;

	public Vector3 to;

	public Vector3 diff;

	public float time;

	public bool useEstimatedTime;

	public bool useFrames;

	public float passed;

	public TweenAction type;

	public Hashtable optional;

	public float delay;

	public ICallable tweenFunc;

	public LeanTweenType tweenType;

	public AnimationCurve animationCurve;

	public int id;

	public override string ToString()
	{
		return "gameObject:" + trans.gameObject + " toggle:" + toggle + " passed:" + passed + " time:" + time + " delay:" + delay + " from:" + from + " to:" + to + " type:" + type + " useEstimatedTime:" + useEstimatedTime + " id:" + id;
	}
}
