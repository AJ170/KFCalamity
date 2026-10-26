// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Hidden/Camera-DepthNormalTexture" {
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
  	float3 normal : NORMAL;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float4 xlv_TEXCOORD0 : TEXCOORD0;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	float4 tmpvar_1;
  	float3x3 tmpvar_2;
  	tmpvar_2[0] = UNITY_MATRIX_IT_MV[0].xyz;
  	tmpvar_2[1] = UNITY_MATRIX_IT_MV[1].xyz;
  	tmpvar_2[2] = UNITY_MATRIX_IT_MV[2].xyz;
  	tmpvar_1.xyz = (tmpvar_2 * normalize(v.normal));
  	tmpvar_1.w = -(((mul(UNITY_MATRIX_MV, v.vertex)).z * _ProjectionParams.w));
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = tmpvar_1;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	float4 enc_2;
  	enc_2.xy = (((
  	(i.xlv_TEXCOORD0.xy / (i.xlv_TEXCOORD0.z + 1.0))
  	/ 1.7777) * 0.5) + 0.5);
  	float2 enc_3;
  	float2 tmpvar_4;
  	tmpvar_4 = frac((float2(1.0, 255.0) * i.xlv_TEXCOORD0.w));
  	enc_3.y = tmpvar_4.y;
  	enc_3.x = (tmpvar_4.x - (tmpvar_4.y * 0.00392157));
  	enc_2.zw = enc_3;
  	tmpvar_1 = enc_2;
  	bolOut = tmpvar_1;
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
  	float3 normal : NORMAL;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float2 xlv_TEXCOORD0 : TEXCOORD0;
  	float4 xlv_TEXCOORD1 : TEXCOORD1;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	float4 tmpvar_1;
  	float3x3 tmpvar_2;
  	tmpvar_2[0] = UNITY_MATRIX_IT_MV[0].xyz;
  	tmpvar_2[1] = UNITY_MATRIX_IT_MV[1].xyz;
  	tmpvar_2[2] = UNITY_MATRIX_IT_MV[2].xyz;
  	tmpvar_1.xyz = (tmpvar_2 * normalize(v.normal));
  	tmpvar_1.w = -(((mul(UNITY_MATRIX_MV, v.vertex)).z * _ProjectionParams.w));
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  	o.xlv_TEXCOORD1 = tmpvar_1;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	fixed x_2;
  	x_2 = ((tex2D (_MainTex, i.xlv_TEXCOORD0).w * _Color.w) - _Cutoff);
  	if ((x_2 < 0.0)) {
  	discard;
  	};
  	float4 enc_3;
  	enc_3.xy = (((
  	(i.xlv_TEXCOORD1.xy / (i.xlv_TEXCOORD1.z + 1.0))
  	/ 1.7777) * 0.5) + 0.5);
  	float2 enc_4;
  	float2 tmpvar_5;
  	tmpvar_5 = frac((float2(1.0, 255.0) * i.xlv_TEXCOORD1.w));
  	enc_4.y = tmpvar_5.y;
  	enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  	enc_3.zw = enc_4;
  	tmpvar_1 = enc_3;
  	bolOut = tmpvar_1;
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
  	float4 texcoord : TEXCOORD0;
  	float4 texcoord1 : TEXCOORD1;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float2 xlv_TEXCOORD0 : TEXCOORD0;
  	float4 xlv_TEXCOORD1 : TEXCOORD1;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	fixed4 tmpvar_1;
  	tmpvar_1 = v.color;
  	float4 tmpvar_2;
  	float4 tmpvar_3;
  	tmpvar_3.w = v.vertex.w;
  	tmpvar_3.xyz = (v.vertex.xyz * _Scale.xyz);
  	float4 tmpvar_4;
  	tmpvar_4.xy = tmpvar_1.xy;
  	tmpvar_4.zw = v.texcoord1.xy;
  	float4 pos_5;
  	pos_5.w = tmpvar_3.w;
  	float3 bend_6;
  	float4 v_7;
  	v_7.x = unity_ObjectToWorld[0].w;
  	v_7.y = unity_ObjectToWorld[1].w;
  	v_7.z = unity_ObjectToWorld[2].w;
  	v_7.w = unity_ObjectToWorld[3].w;
  	float tmpvar_8;
  	tmpvar_8 = (dot (v_7.xyz, float3(1.0, 1.0, 1.0)) + tmpvar_4.x);
  	float2 tmpvar_9;
  	tmpvar_9.x = dot (tmpvar_3.xyz, ((float3)((tmpvar_4.y + tmpvar_8))));
  	tmpvar_9.y = tmpvar_8;
  	float4 tmpvar_10;
  	tmpvar_10 = abs(((
  	frac((((
  	frac(((_Time.yy + tmpvar_9).xxyy * float4(1.975, 0.793, 0.375, 0.193)))
  	* 2.0) - 1.0) + 0.5))
  	* 2.0) - 1.0));
  	float4 tmpvar_11;
  	tmpvar_11 = ((tmpvar_10 * tmpvar_10) * (3.0 - (2.0 * tmpvar_10)));
  	float2 tmpvar_12;
  	tmpvar_12 = (tmpvar_11.xz + tmpvar_11.yw);
  	bend_6.xz = ((tmpvar_4.y * 0.1) * v.normal).xz;
  	bend_6.y = (v.texcoord1.y * 0.3);
  	pos_5.xyz = (tmpvar_3.xyz + ((
  	(tmpvar_12.xyx * bend_6)
  	+
  	((_Wind.xyz * tmpvar_12.y) * v.texcoord1.y)
  	) * _Wind.w));
  	pos_5.xyz = (pos_5.xyz + (v.texcoord1.x * _Wind.xyz));
  	float4 tmpvar_13;
  	tmpvar_13.w = 1.0;
  	tmpvar_13.xyz = lerp ((pos_5.xyz - (
  	(dot (_SquashPlaneNormal.xyz, pos_5.xyz) + _SquashPlaneNormal.w)
  	* _SquashPlaneNormal.xyz)), pos_5.xyz, ((float3)(_SquashAmount)));
  	tmpvar_3 = tmpvar_13;
  	float3x3 tmpvar_14;
  	tmpvar_14[0] = UNITY_MATRIX_IT_MV[0].xyz;
  	tmpvar_14[1] = UNITY_MATRIX_IT_MV[1].xyz;
  	tmpvar_14[2] = UNITY_MATRIX_IT_MV[2].xyz;
  	tmpvar_2.xyz = (tmpvar_14 * normalize(v.normal));
  	tmpvar_2.w = -(((mul(UNITY_MATRIX_MV, tmpvar_13)).z * _ProjectionParams.w));
  	o.pos = (mul(UNITY_MATRIX_MVP, tmpvar_13));
  	o.xlv_TEXCOORD0 = v.texcoord.xy;
  	o.xlv_TEXCOORD1 = tmpvar_2;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	float4 enc_2;
  	enc_2.xy = (((
  	(i.xlv_TEXCOORD1.xy / (i.xlv_TEXCOORD1.z + 1.0))
  	/ 1.7777) * 0.5) + 0.5);
  	float2 enc_3;
  	float2 tmpvar_4;
  	tmpvar_4 = frac((float2(1.0, 255.0) * i.xlv_TEXCOORD1.w));
  	enc_3.y = tmpvar_4.y;
  	enc_3.x = (tmpvar_4.x - (tmpvar_4.y * 0.00392157));
  	enc_2.zw = enc_3;
  	tmpvar_1 = enc_2;
  	bolOut = tmpvar_1;
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
  	float4 xlv_TEXCOORD1 : TEXCOORD1;
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
  	float4 tmpvar_4;
  	float tmpvar_5;
  	tmpvar_5 = (1.0 - abs(v.tangent.w));
  	float4 tmpvar_6;
  	tmpvar_6.w = 0.0;
  	tmpvar_6.xyz = tmpvar_1;
  	float4 tmpvar_7;
  	tmpvar_7.zw = float2(0.0, 0.0);
  	tmpvar_7.xy = tmpvar_1.xy;
  	float4 tmpvar_8;
  	tmpvar_8 = (v.vertex + ((tmpvar_7 * UNITY_MATRIX_IT_MV) * tmpvar_5));
  	float3 tmpvar_9;
  	tmpvar_9 = lerp (v.normal, normalize((tmpvar_6 * UNITY_MATRIX_IT_MV)).xyz, ((float3)(tmpvar_5)));
  	tmpvar_4.w = tmpvar_8.w;
  	tmpvar_4.xyz = (tmpvar_8.xyz * _Scale.xyz);
  	float4 tmpvar_10;
  	tmpvar_10.xy = tmpvar_2.xy;
  	tmpvar_10.zw = v.texcoord1.xy;
  	float4 pos_11;
  	pos_11.w = tmpvar_4.w;
  	float3 bend_12;
  	float4 v_13;
  	v_13.x = unity_ObjectToWorld[0].w;
  	v_13.y = unity_ObjectToWorld[1].w;
  	v_13.z = unity_ObjectToWorld[2].w;
  	v_13.w = unity_ObjectToWorld[3].w;
  	float tmpvar_14;
  	tmpvar_14 = (dot (v_13.xyz, float3(1.0, 1.0, 1.0)) + tmpvar_10.x);
  	float2 tmpvar_15;
  	tmpvar_15.x = dot (tmpvar_4.xyz, ((float3)((tmpvar_10.y + tmpvar_14))));
  	tmpvar_15.y = tmpvar_14;
  	float4 tmpvar_16;
  	tmpvar_16 = abs(((
  	frac((((
  	frac(((_Time.yy + tmpvar_15).xxyy * float4(1.975, 0.793, 0.375, 0.193)))
  	* 2.0) - 1.0) + 0.5))
  	* 2.0) - 1.0));
  	float4 tmpvar_17;
  	tmpvar_17 = ((tmpvar_16 * tmpvar_16) * (3.0 - (2.0 * tmpvar_16)));
  	float2 tmpvar_18;
  	tmpvar_18 = (tmpvar_17.xz + tmpvar_17.yw);
  	bend_12.xz = ((tmpvar_10.y * 0.1) * tmpvar_9).xz;
  	bend_12.y = (v.texcoord1.y * 0.3);
  	pos_11.xyz = (tmpvar_4.xyz + ((
  	(tmpvar_18.xyx * bend_12)
  	+
  	((_Wind.xyz * tmpvar_18.y) * v.texcoord1.y)
  	) * _Wind.w));
  	pos_11.xyz = (pos_11.xyz + (v.texcoord1.x * _Wind.xyz));
  	float4 tmpvar_19;
  	tmpvar_19.w = 1.0;
  	tmpvar_19.xyz = lerp ((pos_11.xyz - (
  	(dot (_SquashPlaneNormal.xyz, pos_11.xyz) + _SquashPlaneNormal.w)
  	* _SquashPlaneNormal.xyz)), pos_11.xyz, ((float3)(_SquashAmount)));
  	tmpvar_4 = tmpvar_19;
  	float3x3 tmpvar_20;
  	tmpvar_20[0] = UNITY_MATRIX_IT_MV[0].xyz;
  	tmpvar_20[1] = UNITY_MATRIX_IT_MV[1].xyz;
  	tmpvar_20[2] = UNITY_MATRIX_IT_MV[2].xyz;
  	tmpvar_3.xyz = (tmpvar_20 * normalize(tmpvar_9));
  	tmpvar_3.w = -(((mul(UNITY_MATRIX_MV, tmpvar_19)).z * _ProjectionParams.w));
  	o.pos = (mul(UNITY_MATRIX_MVP, tmpvar_19));
  	o.xlv_TEXCOORD0 = v.texcoord.xy;
  	o.xlv_TEXCOORD1 = tmpvar_3;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	half alpha_2;
  	fixed tmpvar_3;
  	tmpvar_3 = tex2D (_MainTex, i.xlv_TEXCOORD0).w;
  	alpha_2 = tmpvar_3;
  	half x_4;
  	x_4 = (alpha_2 - _Cutoff);
  	if ((x_4 < 0.0)) {
  	discard;
  	};
  	float4 enc_5;
  	enc_5.xy = (((
  	(i.xlv_TEXCOORD1.xy / (i.xlv_TEXCOORD1.z + 1.0))
  	/ 1.7777) * 0.5) + 0.5);
  	float2 enc_6;
  	float2 tmpvar_7;
  	tmpvar_7 = frac((float2(1.0, 255.0) * i.xlv_TEXCOORD1.w));
  	enc_6.y = tmpvar_7.y;
  	enc_6.x = (tmpvar_7.x - (tmpvar_7.y * 0.00392157));
  	enc_5.zw = enc_6;
  	tmpvar_1 = enc_5;
  	bolOut = tmpvar_1;
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
  	float3 normal : NORMAL;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float4 xlv_TEXCOORD0 : TEXCOORD0;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	fixed4 tmpvar_1;
  	tmpvar_1 = v.color;
  	float4 tmpvar_2;
  	float4 pos_3;
  	pos_3.w = v.vertex.w;
  	float alpha_4;
  	alpha_4 = tmpvar_1.w;
  	pos_3.xyz = (v.vertex.xyz * _Scale.xyz);
  	float4 tmpvar_5;
  	tmpvar_5.w = 0.0;
  	tmpvar_5.xyz = pos_3.xyz;
  	pos_3.xyz = lerp (pos_3.xyz, (mul(_TerrainEngineBendTree, tmpvar_5)).xyz, ((float3)(alpha_4)));
  	float4 tmpvar_6;
  	tmpvar_6.w = 1.0;
  	tmpvar_6.xyz = lerp ((pos_3.xyz - (
  	(dot (_SquashPlaneNormal.xyz, pos_3.xyz) + _SquashPlaneNormal.w)
  	* _SquashPlaneNormal.xyz)), pos_3.xyz, ((float3)(_SquashAmount)));
  	pos_3 = tmpvar_6;
  	float3x3 tmpvar_7;
  	tmpvar_7[0] = UNITY_MATRIX_IT_MV[0].xyz;
  	tmpvar_7[1] = UNITY_MATRIX_IT_MV[1].xyz;
  	tmpvar_7[2] = UNITY_MATRIX_IT_MV[2].xyz;
  	tmpvar_2.xyz = (tmpvar_7 * normalize(v.normal));
  	tmpvar_2.w = -(((mul(UNITY_MATRIX_MV, tmpvar_6)).z * _ProjectionParams.w));
  	o.pos = (mul(UNITY_MATRIX_MVP, tmpvar_6));
  	o.xlv_TEXCOORD0 = tmpvar_2;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	float4 enc_2;
  	enc_2.xy = (((
  	(i.xlv_TEXCOORD0.xy / (i.xlv_TEXCOORD0.z + 1.0))
  	/ 1.7777) * 0.5) + 0.5);
  	float2 enc_3;
  	float2 tmpvar_4;
  	tmpvar_4 = frac((float2(1.0, 255.0) * i.xlv_TEXCOORD0.w));
  	enc_3.y = tmpvar_4.y;
  	enc_3.x = (tmpvar_4.x - (tmpvar_4.y * 0.00392157));
  	enc_2.zw = enc_3;
  	tmpvar_1 = enc_2;
  	bolOut = tmpvar_1;
  	return bolOut;
  }
  ENDCG
 }
}
SubShader { 
 Tags { "RenderType"="TreeTransparentCutout" }
 Pass {
  Tags { "RenderType"="TreeTransparentCutout" }
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
  	float3 normal : NORMAL;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float2 xlv_TEXCOORD0 : TEXCOORD0;
  	float4 xlv_TEXCOORD1 : TEXCOORD1;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	fixed4 tmpvar_1;
  	tmpvar_1 = v.color;
  	float4 tmpvar_2;
  	float4 pos_3;
  	pos_3.w = v.vertex.w;
  	float alpha_4;
  	alpha_4 = tmpvar_1.w;
  	pos_3.xyz = (v.vertex.xyz * _Scale.xyz);
  	float4 tmpvar_5;
  	tmpvar_5.w = 0.0;
  	tmpvar_5.xyz = pos_3.xyz;
  	pos_3.xyz = lerp (pos_3.xyz, (mul(_TerrainEngineBendTree, tmpvar_5)).xyz, ((float3)(alpha_4)));
  	float4 tmpvar_6;
  	tmpvar_6.w = 1.0;
  	tmpvar_6.xyz = lerp ((pos_3.xyz - (
  	(dot (_SquashPlaneNormal.xyz, pos_3.xyz) + _SquashPlaneNormal.w)
  	* _SquashPlaneNormal.xyz)), pos_3.xyz, ((float3)(_SquashAmount)));
  	pos_3 = tmpvar_6;
  	float3x3 tmpvar_7;
  	tmpvar_7[0] = UNITY_MATRIX_IT_MV[0].xyz;
  	tmpvar_7[1] = UNITY_MATRIX_IT_MV[1].xyz;
  	tmpvar_7[2] = UNITY_MATRIX_IT_MV[2].xyz;
  	tmpvar_2.xyz = (tmpvar_7 * normalize(v.normal));
  	tmpvar_2.w = -(((mul(UNITY_MATRIX_MV, tmpvar_6)).z * _ProjectionParams.w));
  	o.pos = (mul(UNITY_MATRIX_MVP, tmpvar_6));
  	o.xlv_TEXCOORD0 = v.texcoord.xy;
  	o.xlv_TEXCOORD1 = tmpvar_2;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	half alpha_2;
  	fixed tmpvar_3;
  	tmpvar_3 = tex2D (_MainTex, i.xlv_TEXCOORD0).w;
  	alpha_2 = tmpvar_3;
  	half x_4;
  	x_4 = (alpha_2 - _Cutoff);
  	if ((x_4 < 0.0)) {
  	discard;
  	};
  	float4 enc_5;
  	enc_5.xy = (((
  	(i.xlv_TEXCOORD1.xy / (i.xlv_TEXCOORD1.z + 1.0))
  	/ 1.7777) * 0.5) + 0.5);
  	float2 enc_6;
  	float2 tmpvar_7;
  	tmpvar_7 = frac((float2(1.0, 255.0) * i.xlv_TEXCOORD1.w));
  	enc_6.y = tmpvar_7.y;
  	enc_6.x = (tmpvar_7.x - (tmpvar_7.y * 0.00392157));
  	enc_5.zw = enc_6;
  	tmpvar_1 = enc_5;
  	bolOut = tmpvar_1;
  	return bolOut;
  }
  ENDCG
 }
 Pass {
  Tags { "RenderType"="TreeTransparentCutout" }
  Cull Front
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
  	float3 normal : NORMAL;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float2 xlv_TEXCOORD0 : TEXCOORD0;
  	float4 xlv_TEXCOORD1 : TEXCOORD1;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	fixed4 tmpvar_1;
  	tmpvar_1 = v.color;
  	float4 tmpvar_2;
  	float4 pos_3;
  	pos_3.w = v.vertex.w;
  	float alpha_4;
  	alpha_4 = tmpvar_1.w;
  	pos_3.xyz = (v.vertex.xyz * _Scale.xyz);
  	float4 tmpvar_5;
  	tmpvar_5.w = 0.0;
  	tmpvar_5.xyz = pos_3.xyz;
  	pos_3.xyz = lerp (pos_3.xyz, (mul(_TerrainEngineBendTree, tmpvar_5)).xyz, ((float3)(alpha_4)));
  	float4 tmpvar_6;
  	tmpvar_6.w = 1.0;
  	tmpvar_6.xyz = lerp ((pos_3.xyz - (
  	(dot (_SquashPlaneNormal.xyz, pos_3.xyz) + _SquashPlaneNormal.w)
  	* _SquashPlaneNormal.xyz)), pos_3.xyz, ((float3)(_SquashAmount)));
  	pos_3 = tmpvar_6;
  	float3x3 tmpvar_7;
  	tmpvar_7[0] = UNITY_MATRIX_IT_MV[0].xyz;
  	tmpvar_7[1] = UNITY_MATRIX_IT_MV[1].xyz;
  	tmpvar_7[2] = UNITY_MATRIX_IT_MV[2].xyz;
  	tmpvar_2.xyz = -((tmpvar_7 * normalize(v.normal)));
  	tmpvar_2.w = -(((mul(UNITY_MATRIX_MV, tmpvar_6)).z * _ProjectionParams.w));
  	o.pos = (mul(UNITY_MATRIX_MVP, tmpvar_6));
  	o.xlv_TEXCOORD0 = v.texcoord.xy;
  	o.xlv_TEXCOORD1 = tmpvar_2;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	fixed x_2;
  	x_2 = (tex2D (_MainTex, i.xlv_TEXCOORD0).w - _Cutoff);
  	if ((x_2 < 0.0)) {
  	discard;
  	};
  	float4 enc_3;
  	enc_3.xy = (((
  	(i.xlv_TEXCOORD1.xy / (i.xlv_TEXCOORD1.z + 1.0))
  	/ 1.7777) * 0.5) + 0.5);
  	float2 enc_4;
  	float2 tmpvar_5;
  	tmpvar_5 = frac((float2(1.0, 255.0) * i.xlv_TEXCOORD1.w));
  	enc_4.y = tmpvar_5.y;
  	enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  	enc_3.zw = enc_4;
  	tmpvar_1 = enc_3;
  	bolOut = tmpvar_1;
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
  	float4 xlv_TEXCOORD1 : TEXCOORD1;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	float4 tmpvar_1;
  	tmpvar_1 = v.texcoord;
  	float2 tmpvar_2;
  	float4 tmpvar_3;
  	float4 pos_4;
  	pos_4 = v.vertex;
  	float2 offset_5;
  	offset_5 = v.texcoord1.xy;
  	float offsetz_6;
  	offsetz_6 = tmpvar_1.y;
  	float3 tmpvar_7;
  	tmpvar_7 = (v.vertex.xyz - _TreeBillboardCameraPos.xyz);
  	float tmpvar_8;
  	tmpvar_8 = dot (tmpvar_7, tmpvar_7);
  	if ((tmpvar_8 > _TreeBillboardDistances.x)) {
  	offsetz_6 = 0.0;
  	offset_5 = float2(0.0, 0.0);
  	};
  	pos_4.xyz = (v.vertex.xyz + (_TreeBillboardCameraRight * offset_5.x));
  	pos_4.xyz = (pos_4.xyz + (_TreeBillboardCameraUp.xyz * lerp (offset_5.y, offsetz_6, _TreeBillboardCameraPos.w)));
  	pos_4.xyz = (pos_4.xyz + ((_TreeBillboardCameraFront.xyz *
  	abs(offset_5.x)
  	) * _TreeBillboardCameraUp.w));
  	tmpvar_2.x = tmpvar_1.x;
  	tmpvar_2.y = float((v.texcoord.y > 0.0));
  	tmpvar_3.xyz = float3(0.0, 0.0, 1.0);
  	tmpvar_3.w = -(((mul(UNITY_MATRIX_MV, pos_4)).z * _ProjectionParams.w));
  	o.pos = (mul(UNITY_MATRIX_MVP, pos_4));
  	o.xlv_TEXCOORD0 = tmpvar_2;
  	o.xlv_TEXCOORD1 = tmpvar_3;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	fixed x_2;
  	x_2 = (tex2D (_MainTex, i.xlv_TEXCOORD0).w - 0.001);
  	if ((x_2 < 0.0)) {
  	discard;
  	};
  	float4 enc_3;
  	enc_3.xy = (((
  	(i.xlv_TEXCOORD1.xy / (i.xlv_TEXCOORD1.z + 1.0))
  	/ 1.7777) * 0.5) + 0.5);
  	float2 enc_4;
  	float2 tmpvar_5;
  	tmpvar_5 = frac((float2(1.0, 255.0) * i.xlv_TEXCOORD1.w));
  	enc_4.y = tmpvar_5.y;
  	enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  	enc_3.zw = enc_4;
  	tmpvar_1 = enc_3;
  	bolOut = tmpvar_1;
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
  	float3 normal : NORMAL;
  	float4 texcoord : TEXCOORD0;
  	float4 tangent : TANGENT;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	fixed4 xlv_COLOR : TEXCOORD0;
  	float2 xlv_TEXCOORD0 : TEXCOORD1;
  	float4 xlv_TEXCOORD1 : TEXCOORD2;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	float4 tmpvar_1;
  	tmpvar_1 = v.texcoord;
  	fixed4 tmpvar_2;
  	tmpvar_2 = v.color;
  	float4 tmpvar_3;
  	float4 pos_4;
  	pos_4 = v.vertex;
  	float2 offset_5;
  	offset_5 = v.tangent.xy;
  	float3 tmpvar_6;
  	tmpvar_6 = (v.vertex.xyz - _CameraPosition.xyz);
  	float tmpvar_7;
  	tmpvar_7 = dot (tmpvar_6, tmpvar_6);
  	if ((tmpvar_7 > _WaveAndDistance.w)) {
  	offset_5 = float2(0.0, 0.0);
  	};
  	pos_4.xyz = (v.vertex.xyz + (offset_5.x * _CameraRight));
  	pos_4.xyz = (pos_4.xyz + (offset_5.y * _CameraUp));
  	float4 vertex_8;
  	vertex_8.yw = pos_4.yw;
  	fixed4 color_9;
  	color_9.xyz = tmpvar_2.xyz;
  	fixed3 waveColor_10;
  	float3 waveMove_11;
  	float4 tmpvar_12;
  	tmpvar_12 = ((frac(
  	(((pos_4.x * (float4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos_4.z * (float4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * float4(1.2, 2.0, 1.6, 4.8)))
  	) * 6.40885) - 3.14159);
  	float4 tmpvar_13;
  	tmpvar_13 = (tmpvar_12 * tmpvar_12);
  	float4 tmpvar_14;
  	tmpvar_14 = (tmpvar_13 * tmpvar_12);
  	float4 tmpvar_15;
  	tmpvar_15 = (tmpvar_14 * tmpvar_13);
  	float4 tmpvar_16;
  	tmpvar_16 = (((tmpvar_12 +
  	(tmpvar_14 * -0.161616)
  	) + (tmpvar_15 * 0.0083333)) + ((tmpvar_15 * tmpvar_13) * -0.00019841));
  	float4 tmpvar_17;
  	tmpvar_17 = (tmpvar_16 * tmpvar_16);
  	float4 tmpvar_18;
  	tmpvar_18 = (tmpvar_17 * tmpvar_17);
  	float4 tmpvar_19;
  	tmpvar_19 = (tmpvar_18 * v.tangent.y);
  	waveMove_11.y = 0.0;
  	waveMove_11.x = dot (tmpvar_19, float4(0.024, 0.04, -0.12, 0.096));
  	waveMove_11.z = dot (tmpvar_19, float4(0.006, 0.02, -0.02, 0.1));
  	vertex_8.xz = (pos_4.xz - (waveMove_11.xz * _WaveAndDistance.z));
  	float3 tmpvar_20;
  	tmpvar_20 = lerp (float3(0.5, 0.5, 0.5), _WavingTint.xyz, ((float3)((dot (tmpvar_18, float4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7))));
  	waveColor_10 = tmpvar_20;
  	float3 tmpvar_21;
  	tmpvar_21 = (vertex_8.xyz - _CameraPosition.xyz);
  	float tmpvar_22;
  	tmpvar_22 = clamp (((2.0 *
  	(_WaveAndDistance.w - dot (tmpvar_21, tmpvar_21))
  	) * _CameraPosition.w), 0.0, 1.0);
  	color_9.w = tmpvar_22;
  	fixed4 tmpvar_23;
  	tmpvar_23.xyz = ((2.0 * waveColor_10) * v.color.xyz);
  	tmpvar_23.w = color_9.w;
  	float3x3 tmpvar_24;
  	tmpvar_24[0] = UNITY_MATRIX_IT_MV[0].xyz;
  	tmpvar_24[1] = UNITY_MATRIX_IT_MV[1].xyz;
  	tmpvar_24[2] = UNITY_MATRIX_IT_MV[2].xyz;
  	tmpvar_3.xyz = (tmpvar_24 * v.normal);
  	tmpvar_3.w = -(((mul(UNITY_MATRIX_MV, vertex_8)).z * _ProjectionParams.w));
  	o.pos = (mul(UNITY_MATRIX_MVP, vertex_8));
  	o.xlv_COLOR = tmpvar_23;
  	o.xlv_TEXCOORD0 = tmpvar_1.xy;
  	o.xlv_TEXCOORD1 = tmpvar_3;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	fixed x_2;
  	x_2 = ((tex2D (_MainTex, i.xlv_TEXCOORD0).w * i.xlv_COLOR.w) - _Cutoff);
  	if ((x_2 < 0.0)) {
  	discard;
  	};
  	float4 enc_3;
  	enc_3.xy = (((
  	(i.xlv_TEXCOORD1.xy / (i.xlv_TEXCOORD1.z + 1.0))
  	/ 1.7777) * 0.5) + 0.5);
  	float2 enc_4;
  	float2 tmpvar_5;
  	tmpvar_5 = frac((float2(1.0, 255.0) * i.xlv_TEXCOORD1.w));
  	enc_4.y = tmpvar_5.y;
  	enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  	enc_3.zw = enc_4;
  	tmpvar_1 = enc_3;
  	bolOut = tmpvar_1;
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
  	float3 normal : NORMAL;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	fixed4 xlv_COLOR : TEXCOORD0;
  	float2 xlv_TEXCOORD0 : TEXCOORD1;
  	float4 xlv_TEXCOORD1 : TEXCOORD2;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	float4 tmpvar_1;
  	float4 vertex_2;
  	vertex_2.yw = v.vertex.yw;
  	fixed4 color_3;
  	color_3.xyz = v.color.xyz;
  	fixed3 waveColor_4;
  	float3 waveMove_5;
  	float4 tmpvar_6;
  	tmpvar_6 = ((frac(
  	(((v.vertex.x * (float4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (v.vertex.z * (float4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * float4(1.2, 2.0, 1.6, 4.8)))
  	) * 6.40885) - 3.14159);
  	float4 tmpvar_7;
  	tmpvar_7 = (tmpvar_6 * tmpvar_6);
  	float4 tmpvar_8;
  	tmpvar_8 = (tmpvar_7 * tmpvar_6);
  	float4 tmpvar_9;
  	tmpvar_9 = (tmpvar_8 * tmpvar_7);
  	float4 tmpvar_10;
  	tmpvar_10 = (((tmpvar_6 +
  	(tmpvar_8 * -0.161616)
  	) + (tmpvar_9 * 0.0083333)) + ((tmpvar_9 * tmpvar_7) * -0.00019841));
  	float4 tmpvar_11;
  	tmpvar_11 = (tmpvar_10 * tmpvar_10);
  	float4 tmpvar_12;
  	tmpvar_12 = (tmpvar_11 * tmpvar_11);
  	float4 tmpvar_13;
  	tmpvar_13 = (tmpvar_12 * (v.color.w * _WaveAndDistance.z));
  	waveMove_5.y = 0.0;
  	waveMove_5.x = dot (tmpvar_13, float4(0.024, 0.04, -0.12, 0.096));
  	waveMove_5.z = dot (tmpvar_13, float4(0.006, 0.02, -0.02, 0.1));
  	vertex_2.xz = (v.vertex.xz - (waveMove_5.xz * _WaveAndDistance.z));
  	float3 tmpvar_14;
  	tmpvar_14 = lerp (float3(0.5, 0.5, 0.5), _WavingTint.xyz, ((float3)((dot (tmpvar_12, float4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7))));
  	waveColor_4 = tmpvar_14;
  	float3 tmpvar_15;
  	tmpvar_15 = (vertex_2.xyz - _CameraPosition.xyz);
  	float tmpvar_16;
  	tmpvar_16 = clamp (((2.0 *
  	(_WaveAndDistance.w - dot (tmpvar_15, tmpvar_15))
  	) * _CameraPosition.w), 0.0, 1.0);
  	color_3.w = tmpvar_16;
  	fixed4 tmpvar_17;
  	tmpvar_17.xyz = ((2.0 * waveColor_4) * v.color.xyz);
  	tmpvar_17.w = color_3.w;
  	float3x3 tmpvar_18;
  	tmpvar_18[0] = UNITY_MATRIX_IT_MV[0].xyz;
  	tmpvar_18[1] = UNITY_MATRIX_IT_MV[1].xyz;
  	tmpvar_18[2] = UNITY_MATRIX_IT_MV[2].xyz;
  	tmpvar_1.xyz = (tmpvar_18 * normalize(v.normal));
  	tmpvar_1.w = -(((mul(UNITY_MATRIX_MV, vertex_2)).z * _ProjectionParams.w));
  	o.pos = (mul(UNITY_MATRIX_MVP, vertex_2));
  	o.xlv_COLOR = tmpvar_17;
  	o.xlv_TEXCOORD0 = v.texcoord.xy;
  	o.xlv_TEXCOORD1 = tmpvar_1;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	fixed x_2;
  	x_2 = ((tex2D (_MainTex, i.xlv_TEXCOORD0).w * i.xlv_COLOR.w) - _Cutoff);
  	if ((x_2 < 0.0)) {
  	discard;
  	};
  	float4 enc_3;
  	enc_3.xy = (((
  	(i.xlv_TEXCOORD1.xy / (i.xlv_TEXCOORD1.z + 1.0))
  	/ 1.7777) * 0.5) + 0.5);
  	float2 enc_4;
  	float2 tmpvar_5;
  	tmpvar_5 = frac((float2(1.0, 255.0) * i.xlv_TEXCOORD1.w));
  	enc_4.y = tmpvar_5.y;
  	enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  	enc_3.zw = enc_4;
  	tmpvar_1 = enc_3;
  	bolOut = tmpvar_1;
  	return bolOut;
  }
  ENDCG
 }
}
Fallback Off
}