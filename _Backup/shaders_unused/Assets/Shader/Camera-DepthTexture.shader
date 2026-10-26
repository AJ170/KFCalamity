// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Hidden/Camera-DepthTexture" {
Properties {
 _MainTex ("", 2D) = "white" {}
 _Cutoff ("", Float) = 0.5
 _Color ("", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"


  struct kf_appdata
  {
  	float4 vertex : POSITION;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	bolOut = float4(0.0, 0.0, 0.0, 0.0);
  	return bolOut;
  }
  ENDCG
 }
}
SubShader { 
 Tags { "RenderType"="TransparentCutout" }
 Pass {
  Tags { "RenderType"="TransparentCutout" }
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  fixed4 _Color;
  fixed _Cutoff;
  sampler2D _MainTex;
  float4 _MainTex_ST;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float2 xlv_TEXCOORD0 : TEXCOORD0;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed x_1;
  	x_1 = ((tex2D (_MainTex, i.xlv_TEXCOORD0).w * _Color.w) - _Cutoff);
  	if ((x_1 < 0.0)) {
  	discard;
  	};
  	bolOut = float4(0.0, 0.0, 0.0, 0.0);
  	return bolOut;
  }
  ENDCG
 }
}
SubShader { 
 Tags { "RenderType"="TreeBark" }
 Pass {
  Tags { "RenderType"="TreeBark" }
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  float4 _Scale;
  float _SquashAmount;
  float4 _SquashPlaneNormal;
  float4 _Wind;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	fixed4 color : COLOR;
  	float3 normal : NORMAL;
  	float4 texcoord1 : TEXCOORD1;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	fixed4 tmpvar_1;
  	tmpvar_1 = v.color;
  	float4 tmpvar_2;
  	tmpvar_2.w = v.vertex.w;
  	tmpvar_2.xyz = (v.vertex.xyz * _Scale.xyz);
  	float4 tmpvar_3;
  	tmpvar_3.xy = tmpvar_1.xy;
  	tmpvar_3.zw = v.texcoord1.xy;
  	float4 pos_4;
  	pos_4.w = tmpvar_2.w;
  	float3 bend_5;
  	float4 v_6;
  	v_6.x = unity_ObjectToWorld[0].w;
  	v_6.y = unity_ObjectToWorld[1].w;
  	v_6.z = unity_ObjectToWorld[2].w;
  	v_6.w = unity_ObjectToWorld[3].w;
  	float tmpvar_7;
  	tmpvar_7 = (dot (v_6.xyz, float3(1.0, 1.0, 1.0)) + tmpvar_3.x);
  	float2 tmpvar_8;
  	tmpvar_8.x = dot (tmpvar_2.xyz, ((float3)((tmpvar_3.y + tmpvar_7))));
  	tmpvar_8.y = tmpvar_7;
  	float4 tmpvar_9;
  	tmpvar_9 = abs(((
  	frac((((
  	frac(((_Time.yy + tmpvar_8).xxyy * float4(1.975, 0.793, 0.375, 0.193)))
  	* 2.0) - 1.0) + 0.5))
  	* 2.0) - 1.0));
  	float4 tmpvar_10;
  	tmpvar_10 = ((tmpvar_9 * tmpvar_9) * (3.0 - (2.0 * tmpvar_9)));
  	float2 tmpvar_11;
  	tmpvar_11 = (tmpvar_10.xz + tmpvar_10.yw);
  	bend_5.xz = ((tmpvar_3.y * 0.1) * v.normal).xz;
  	bend_5.y = (v.texcoord1.y * 0.3);
  	pos_4.xyz = (tmpvar_2.xyz + ((
  	(tmpvar_11.xyx * bend_5)
  	+
  	((_Wind.xyz * tmpvar_11.y) * v.texcoord1.y)
  	) * _Wind.w));
  	pos_4.xyz = (pos_4.xyz + (v.texcoord1.x * _Wind.xyz));
  	float4 tmpvar_12;
  	tmpvar_12.w = 1.0;
  	tmpvar_12.xyz = lerp ((pos_4.xyz - (
  	(dot (_SquashPlaneNormal.xyz, pos_4.xyz) + _SquashPlaneNormal.w)
  	* _SquashPlaneNormal.xyz)), pos_4.xyz, ((float3)(_SquashAmount)));
  	tmpvar_2 = tmpvar_12;
  	o.pos = (mul(UNITY_MATRIX_MVP, tmpvar_12));
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	bolOut = float4(0.0, 0.0, 0.0, 0.0);
  	return bolOut;
  }
  ENDCG
 }
}
SubShader { 
 Tags { "RenderType"="TreeLeaf" }
 Pass {
  Tags { "RenderType"="TreeLeaf" }
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  fixed _Cutoff;
  sampler2D _MainTex;
  float4 _Scale;
  float _SquashAmount;
  float4 _SquashPlaneNormal;
  float4 _Wind;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	fixed4 color : COLOR;
  	float3 normal : NORMAL;
  	float4 texcoord : TEXCOORD0;
  	float4 texcoord1 : TEXCOORD1;
  	float4 tangent : TANGENT;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float2 xlv_TEXCOORD0 : TEXCOORD0;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	float3 tmpvar_1;
  	tmpvar_1 = v.normal;
  	fixed4 tmpvar_2;
  	tmpvar_2 = v.color;
  	float4 tmpvar_3;
  	float tmpvar_4;
  	tmpvar_4 = (1.0 - abs(v.tangent.w));
  	float4 tmpvar_5;
  	tmpvar_5.w = 0.0;
  	tmpvar_5.xyz = tmpvar_1;
  	float4 tmpvar_6;
  	tmpvar_6.zw = float2(0.0, 0.0);
  	tmpvar_6.xy = tmpvar_1.xy;
  	float4 tmpvar_7;
  	tmpvar_7 = (v.vertex + ((tmpvar_6 * UNITY_MATRIX_IT_MV) * tmpvar_4));
  	tmpvar_3.w = tmpvar_7.w;
  	tmpvar_3.xyz = (tmpvar_7.xyz * _Scale.xyz);
  	float4 tmpvar_8;
  	tmpvar_8.xy = tmpvar_2.xy;
  	tmpvar_8.zw = v.texcoord1.xy;
  	float4 pos_9;
  	pos_9.w = tmpvar_3.w;
  	float3 bend_10;
  	float4 v_11;
  	v_11.x = unity_ObjectToWorld[0].w;
  	v_11.y = unity_ObjectToWorld[1].w;
  	v_11.z = unity_ObjectToWorld[2].w;
  	v_11.w = unity_ObjectToWorld[3].w;
  	float tmpvar_12;
  	tmpvar_12 = (dot (v_11.xyz, float3(1.0, 1.0, 1.0)) + tmpvar_8.x);
  	float2 tmpvar_13;
  	tmpvar_13.x = dot (tmpvar_3.xyz, ((float3)((tmpvar_8.y + tmpvar_12))));
  	tmpvar_13.y = tmpvar_12;
  	float4 tmpvar_14;
  	tmpvar_14 = abs(((
  	frac((((
  	frac(((_Time.yy + tmpvar_13).xxyy * float4(1.975, 0.793, 0.375, 0.193)))
  	* 2.0) - 1.0) + 0.5))
  	* 2.0) - 1.0));
  	float4 tmpvar_15;
  	tmpvar_15 = ((tmpvar_14 * tmpvar_14) * (3.0 - (2.0 * tmpvar_14)));
  	float2 tmpvar_16;
  	tmpvar_16 = (tmpvar_15.xz + tmpvar_15.yw);
  	bend_10.xz = ((tmpvar_8.y * 0.1) * lerp (v.normal, normalize(
  	(tmpvar_5 * UNITY_MATRIX_IT_MV)
  	).xyz, ((float3)(tmpvar_4)))).xz;
  	bend_10.y = (v.texcoord1.y * 0.3);
  	pos_9.xyz = (tmpvar_3.xyz + ((
  	(tmpvar_16.xyx * bend_10)
  	+
  	((_Wind.xyz * tmpvar_16.y) * v.texcoord1.y)
  	) * _Wind.w));
  	pos_9.xyz = (pos_9.xyz + (v.texcoord1.x * _Wind.xyz));
  	float4 tmpvar_17;
  	tmpvar_17.w = 1.0;
  	tmpvar_17.xyz = lerp ((pos_9.xyz - (
  	(dot (_SquashPlaneNormal.xyz, pos_9.xyz) + _SquashPlaneNormal.w)
  	* _SquashPlaneNormal.xyz)), pos_9.xyz, ((float3)(_SquashAmount)));
  	tmpvar_3 = tmpvar_17;
  	o.pos = (mul(UNITY_MATRIX_MVP, tmpvar_17));
  	o.xlv_TEXCOORD0 = v.texcoord.xy;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	half alpha_1;
  	fixed tmpvar_2;
  	tmpvar_2 = tex2D (_MainTex, i.xlv_TEXCOORD0).w;
  	alpha_1 = tmpvar_2;
  	half x_3;
  	x_3 = (alpha_1 - _Cutoff);
  	if ((x_3 < 0.0)) {
  	discard;
  	};
  	bolOut = float4(0.0, 0.0, 0.0, 0.0);
  	return bolOut;
  }
  ENDCG
 }
}
SubShader { 
 Tags { "RenderType"="TreeOpaque" }
 Pass {
  Tags { "RenderType"="TreeOpaque" }
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  float4 _Scale;
  float _SquashAmount;
  float4 _SquashPlaneNormal;
  float4x4 _TerrainEngineBendTree;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	fixed4 color : COLOR;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	fixed4 tmpvar_1;
  	tmpvar_1 = v.color;
  	float4 pos_2;
  	pos_2.w = v.vertex.w;
  	float alpha_3;
  	alpha_3 = tmpvar_1.w;
  	pos_2.xyz = (v.vertex.xyz * _Scale.xyz);
  	float4 tmpvar_4;
  	tmpvar_4.w = 0.0;
  	tmpvar_4.xyz = pos_2.xyz;
  	pos_2.xyz = lerp (pos_2.xyz, (mul(_TerrainEngineBendTree, tmpvar_4)).xyz, ((float3)(alpha_3)));
  	float4 tmpvar_5;
  	tmpvar_5.w = 1.0;
  	tmpvar_5.xyz = lerp ((pos_2.xyz - (
  	(dot (_SquashPlaneNormal.xyz, pos_2.xyz) + _SquashPlaneNormal.w)
  	* _SquashPlaneNormal.xyz)), pos_2.xyz, ((float3)(_SquashAmount)));
  	pos_2 = tmpvar_5;
  	o.pos = (mul(UNITY_MATRIX_MVP, tmpvar_5));
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	bolOut = float4(0.0, 0.0, 0.0, 0.0);
  	return bolOut;
  }
  ENDCG
 }
}
SubShader { 
 Tags { "RenderType"="TreeTransparentCutout" }
 Pass {
  Tags { "RenderType"="TreeTransparentCutout" }
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  fixed _Cutoff;
  sampler2D _MainTex;
  float4 _Scale;
  float _SquashAmount;
  float4 _SquashPlaneNormal;
  float4x4 _TerrainEngineBendTree;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	fixed4 color : COLOR;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float2 xlv_TEXCOORD0 : TEXCOORD0;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	fixed4 tmpvar_1;
  	tmpvar_1 = v.color;
  	float4 pos_2;
  	pos_2.w = v.vertex.w;
  	float alpha_3;
  	alpha_3 = tmpvar_1.w;
  	pos_2.xyz = (v.vertex.xyz * _Scale.xyz);
  	float4 tmpvar_4;
  	tmpvar_4.w = 0.0;
  	tmpvar_4.xyz = pos_2.xyz;
  	pos_2.xyz = lerp (pos_2.xyz, (mul(_TerrainEngineBendTree, tmpvar_4)).xyz, ((float3)(alpha_3)));
  	float4 tmpvar_5;
  	tmpvar_5.w = 1.0;
  	tmpvar_5.xyz = lerp ((pos_2.xyz - (
  	(dot (_SquashPlaneNormal.xyz, pos_2.xyz) + _SquashPlaneNormal.w)
  	* _SquashPlaneNormal.xyz)), pos_2.xyz, ((float3)(_SquashAmount)));
  	pos_2 = tmpvar_5;
  	o.pos = (mul(UNITY_MATRIX_MVP, tmpvar_5));
  	o.xlv_TEXCOORD0 = v.texcoord.xy;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	half alpha_1;
  	fixed tmpvar_2;
  	tmpvar_2 = tex2D (_MainTex, i.xlv_TEXCOORD0).w;
  	alpha_1 = tmpvar_2;
  	half x_3;
  	x_3 = (alpha_1 - _Cutoff);
  	if ((x_3 < 0.0)) {
  	discard;
  	};
  	bolOut = float4(0.0, 0.0, 0.0, 0.0);
  	return bolOut;
  }
  ENDCG
 }
}
SubShader { 
 Tags { "RenderType"="TreeBillboard" }
 Pass {
  Tags { "RenderType"="TreeBillboard" }
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _TreeBillboardCameraFront;
  float4 _TreeBillboardCameraPos;
  float3 _TreeBillboardCameraRight;
  float4 _TreeBillboardCameraUp;
  float4 _TreeBillboardDistances;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  	float4 texcoord1 : TEXCOORD1;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float2 xlv_TEXCOORD0 : TEXCOORD0;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	float4 tmpvar_1;
  	tmpvar_1 = v.texcoord;
  	float2 tmpvar_2;
  	float4 pos_3;
  	pos_3 = v.vertex;
  	float2 offset_4;
  	offset_4 = v.texcoord1.xy;
  	float offsetz_5;
  	offsetz_5 = tmpvar_1.y;
  	float3 tmpvar_6;
  	tmpvar_6 = (v.vertex.xyz - _TreeBillboardCameraPos.xyz);
  	float tmpvar_7;
  	tmpvar_7 = dot (tmpvar_6, tmpvar_6);
  	if ((tmpvar_7 > _TreeBillboardDistances.x)) {
  	offsetz_5 = 0.0;
  	offset_4 = float2(0.0, 0.0);
  	};
  	pos_3.xyz = (v.vertex.xyz + (_TreeBillboardCameraRight * offset_4.x));
  	pos_3.xyz = (pos_3.xyz + (_TreeBillboardCameraUp.xyz * lerp (offset_4.y, offsetz_5, _TreeBillboardCameraPos.w)));
  	pos_3.xyz = (pos_3.xyz + ((_TreeBillboardCameraFront.xyz *
  	abs(offset_4.x)
  	) * _TreeBillboardCameraUp.w));
  	tmpvar_2.x = tmpvar_1.x;
  	tmpvar_2.y = float((v.texcoord.y > 0.0));
  	o.pos = (mul(UNITY_MATRIX_MVP, pos_3));
  	o.xlv_TEXCOORD0 = tmpvar_2;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed x_1;
  	x_1 = (tex2D (_MainTex, i.xlv_TEXCOORD0).w - 0.001);
  	if ((x_1 < 0.0)) {
  	discard;
  	};
  	bolOut = float4(0.0, 0.0, 0.0, 0.0);
  	return bolOut;
  }
  ENDCG
 }
}
SubShader { 
 Tags { "RenderType"="GrassBillboard" }
 Pass {
  Tags { "RenderType"="GrassBillboard" }
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  float4 _CameraPosition;
  float3 _CameraRight;
  float3 _CameraUp;
  fixed _Cutoff;
  sampler2D _MainTex;
  float4 _WaveAndDistance;
  fixed4 _WavingTint;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	fixed4 color : COLOR;
  	float4 texcoord : TEXCOORD0;
  	float4 tangent : TANGENT;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	fixed4 xlv_COLOR : TEXCOORD0;
  	float2 xlv_TEXCOORD0 : TEXCOORD1;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	float4 tmpvar_1;
  	tmpvar_1 = v.texcoord;
  	fixed4 tmpvar_2;
  	tmpvar_2 = v.color;
  	float4 pos_3;
  	pos_3 = v.vertex;
  	float2 offset_4;
  	offset_4 = v.tangent.xy;
  	float3 tmpvar_5;
  	tmpvar_5 = (v.vertex.xyz - _CameraPosition.xyz);
  	float tmpvar_6;
  	tmpvar_6 = dot (tmpvar_5, tmpvar_5);
  	if ((tmpvar_6 > _WaveAndDistance.w)) {
  	offset_4 = float2(0.0, 0.0);
  	};
  	pos_3.xyz = (v.vertex.xyz + (offset_4.x * _CameraRight));
  	pos_3.xyz = (pos_3.xyz + (offset_4.y * _CameraUp));
  	float4 vertex_7;
  	vertex_7.yw = pos_3.yw;
  	fixed4 color_8;
  	color_8.xyz = tmpvar_2.xyz;
  	fixed3 waveColor_9;
  	float3 waveMove_10;
  	float4 tmpvar_11;
  	tmpvar_11 = ((frac(
  	(((pos_3.x * (float4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos_3.z * (float4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * float4(1.2, 2.0, 1.6, 4.8)))
  	) * 6.40885) - 3.14159);
  	float4 tmpvar_12;
  	tmpvar_12 = (tmpvar_11 * tmpvar_11);
  	float4 tmpvar_13;
  	tmpvar_13 = (tmpvar_12 * tmpvar_11);
  	float4 tmpvar_14;
  	tmpvar_14 = (tmpvar_13 * tmpvar_12);
  	float4 tmpvar_15;
  	tmpvar_15 = (((tmpvar_11 +
  	(tmpvar_13 * -0.161616)
  	) + (tmpvar_14 * 0.0083333)) + ((tmpvar_14 * tmpvar_12) * -0.00019841));
  	float4 tmpvar_16;
  	tmpvar_16 = (tmpvar_15 * tmpvar_15);
  	float4 tmpvar_17;
  	tmpvar_17 = (tmpvar_16 * tmpvar_16);
  	float4 tmpvar_18;
  	tmpvar_18 = (tmpvar_17 * v.tangent.y);
  	waveMove_10.y = 0.0;
  	waveMove_10.x = dot (tmpvar_18, float4(0.024, 0.04, -0.12, 0.096));
  	waveMove_10.z = dot (tmpvar_18, float4(0.006, 0.02, -0.02, 0.1));
  	vertex_7.xz = (pos_3.xz - (waveMove_10.xz * _WaveAndDistance.z));
  	float3 tmpvar_19;
  	tmpvar_19 = lerp (float3(0.5, 0.5, 0.5), _WavingTint.xyz, ((float3)((dot (tmpvar_17, float4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7))));
  	waveColor_9 = tmpvar_19;
  	float3 tmpvar_20;
  	tmpvar_20 = (vertex_7.xyz - _CameraPosition.xyz);
  	float tmpvar_21;
  	tmpvar_21 = clamp (((2.0 *
  	(_WaveAndDistance.w - dot (tmpvar_20, tmpvar_20))
  	) * _CameraPosition.w), 0.0, 1.0);
  	color_8.w = tmpvar_21;
  	fixed4 tmpvar_22;
  	tmpvar_22.xyz = ((2.0 * waveColor_9) * v.color.xyz);
  	tmpvar_22.w = color_8.w;
  	o.pos = (mul(UNITY_MATRIX_MVP, vertex_7));
  	o.xlv_COLOR = tmpvar_22;
  	o.xlv_TEXCOORD0 = tmpvar_1.xy;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed x_1;
  	x_1 = ((tex2D (_MainTex, i.xlv_TEXCOORD0).w * i.xlv_COLOR.w) - _Cutoff);
  	if ((x_1 < 0.0)) {
  	discard;
  	};
  	bolOut = float4(0.0, 0.0, 0.0, 0.0);
  	return bolOut;
  }
  ENDCG
 }
}
SubShader { 
 Tags { "RenderType"="Grass" }
 Pass {
  Tags { "RenderType"="Grass" }
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  float4 _CameraPosition;
  fixed _Cutoff;
  sampler2D _MainTex;
  float4 _WaveAndDistance;
  fixed4 _WavingTint;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	fixed4 color : COLOR;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	fixed4 xlv_COLOR : TEXCOORD0;
  	float2 xlv_TEXCOORD0 : TEXCOORD1;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	float4 vertex_1;
  	vertex_1.yw = v.vertex.yw;
  	fixed4 color_2;
  	color_2.xyz = v.color.xyz;
  	fixed3 waveColor_3;
  	float3 waveMove_4;
  	float4 tmpvar_5;
  	tmpvar_5 = ((frac(
  	(((v.vertex.x * (float4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (v.vertex.z * (float4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * float4(1.2, 2.0, 1.6, 4.8)))
  	) * 6.40885) - 3.14159);
  	float4 tmpvar_6;
  	tmpvar_6 = (tmpvar_5 * tmpvar_5);
  	float4 tmpvar_7;
  	tmpvar_7 = (tmpvar_6 * tmpvar_5);
  	float4 tmpvar_8;
  	tmpvar_8 = (tmpvar_7 * tmpvar_6);
  	float4 tmpvar_9;
  	tmpvar_9 = (((tmpvar_5 +
  	(tmpvar_7 * -0.161616)
  	) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  	float4 tmpvar_10;
  	tmpvar_10 = (tmpvar_9 * tmpvar_9);
  	float4 tmpvar_11;
  	tmpvar_11 = (tmpvar_10 * tmpvar_10);
  	float4 tmpvar_12;
  	tmpvar_12 = (tmpvar_11 * (v.color.w * _WaveAndDistance.z));
  	waveMove_4.y = 0.0;
  	waveMove_4.x = dot (tmpvar_12, float4(0.024, 0.04, -0.12, 0.096));
  	waveMove_4.z = dot (tmpvar_12, float4(0.006, 0.02, -0.02, 0.1));
  	vertex_1.xz = (v.vertex.xz - (waveMove_4.xz * _WaveAndDistance.z));
  	float3 tmpvar_13;
  	tmpvar_13 = lerp (float3(0.5, 0.5, 0.5), _WavingTint.xyz, ((float3)((dot (tmpvar_11, float4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7))));
  	waveColor_3 = tmpvar_13;
  	float3 tmpvar_14;
  	tmpvar_14 = (vertex_1.xyz - _CameraPosition.xyz);
  	float tmpvar_15;
  	tmpvar_15 = clamp (((2.0 *
  	(_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))
  	) * _CameraPosition.w), 0.0, 1.0);
  	color_2.w = tmpvar_15;
  	fixed4 tmpvar_16;
  	tmpvar_16.xyz = ((2.0 * waveColor_3) * v.color.xyz);
  	tmpvar_16.w = color_2.w;
  	o.pos = (mul(UNITY_MATRIX_MVP, vertex_1));
  	o.xlv_COLOR = tmpvar_16;
  	o.xlv_TEXCOORD0 = v.texcoord.xy;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed x_1;
  	x_1 = ((tex2D (_MainTex, i.xlv_TEXCOORD0).w * i.xlv_COLOR.w) - _Cutoff);
  	if ((x_1 < 0.0)) {
  	discard;
  	};
  	bolOut = float4(0.0, 0.0, 0.0, 0.0);
  	return bolOut;
  }
  ENDCG
 }
}
Fallback Off
}