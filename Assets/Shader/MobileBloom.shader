// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Hidden/FastBloom" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Bloom ("Bloom (RGB)", 2D) = "black" {}
}
SubShader { 
 Pass {
  ZTest False
  ZWrite Off
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _Bloom;
  sampler2D _MainTex;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	half2 xlv_TEXCOORD0 : TEXCOORD0;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = v.texcoord.xy;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	tmpvar_1 = (tex2D (_MainTex, i.xlv_TEXCOORD0) + tex2D (_Bloom, i.xlv_TEXCOORD0));
  	bolOut = tmpvar_1;
  	return bolOut;
  }
  ENDCG
 }
 Pass {
  ZTest False
  ZWrite Off
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  half4 _MainTex_TexelSize;
  half4 _Parameter;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	half2 xlv_TEXCOORD0 : TEXCOORD0;
  	half2 xlv_TEXCOORD1 : TEXCOORD1;
  	half2 xlv_TEXCOORD2 : TEXCOORD2;
  	half2 xlv_TEXCOORD3 : TEXCOORD3;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = (v.texcoord.xy + _MainTex_TexelSize.xy);
  	o.xlv_TEXCOORD1 = (v.texcoord.xy + (_MainTex_TexelSize.xy * float2(-0.5, -0.5)));
  	o.xlv_TEXCOORD2 = (v.texcoord.xy + (_MainTex_TexelSize.xy * float2(0.5, -0.5)));
  	o.xlv_TEXCOORD3 = (v.texcoord.xy + (_MainTex_TexelSize.xy * float2(-0.5, 0.5)));
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	fixed4 tmpvar_2;
  	tmpvar_2 = (((tex2D (_MainTex, i.xlv_TEXCOORD0) + tex2D (_MainTex, i.xlv_TEXCOORD1)) + tex2D (_MainTex, i.xlv_TEXCOORD2)) + tex2D (_MainTex, i.xlv_TEXCOORD3));
  	half4 tmpvar_3;
  	tmpvar_3 = max (((tmpvar_2 / 4.0) - _Parameter.z), float4(0.0, 0.0, 0.0, 0.0));
  	tmpvar_1 = (tmpvar_3 * _Parameter.w);
  	bolOut = tmpvar_1;
  	return bolOut;
  }
  ENDCG
 }
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  half4 _MainTex_TexelSize;
  half4 _Parameter;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	half4 xlv_TEXCOORD0 : TEXCOORD0;
  	half2 xlv_TEXCOORD1 : TEXCOORD1;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	half4 tmpvar_1;
  	tmpvar_1.zw = float2(1.0, 1.0);
  	tmpvar_1.xy = v.texcoord.xy;
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = tmpvar_1;
  	o.xlv_TEXCOORD1 = ((_MainTex_TexelSize.xy * float2(0.0, 1.0)) * _Parameter.x);
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	half4 color_1;
  	half2 coords_2;
  	coords_2 = (i.xlv_TEXCOORD0.xy - (i.xlv_TEXCOORD1 * 3.0));
  	half4 tap_3;
  	fixed4 tmpvar_4;
  	tmpvar_4 = tex2D (_MainTex, coords_2);
  	tap_3 = tmpvar_4;
  	color_1 = (tap_3 * float4(0.0205, 0.0205, 0.0205, 0.0));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_5;
  	fixed4 tmpvar_6;
  	tmpvar_6 = tex2D (_MainTex, coords_2);
  	tap_5 = tmpvar_6;
  	color_1 = (color_1 + (tap_5 * float4(0.0855, 0.0855, 0.0855, 0.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_7;
  	fixed4 tmpvar_8;
  	tmpvar_8 = tex2D (_MainTex, coords_2);
  	tap_7 = tmpvar_8;
  	color_1 = (color_1 + (tap_7 * float4(0.232, 0.232, 0.232, 0.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_9;
  	fixed4 tmpvar_10;
  	tmpvar_10 = tex2D (_MainTex, coords_2);
  	tap_9 = tmpvar_10;
  	color_1 = (color_1 + (tap_9 * float4(0.324, 0.324, 0.324, 1.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_11;
  	fixed4 tmpvar_12;
  	tmpvar_12 = tex2D (_MainTex, coords_2);
  	tap_11 = tmpvar_12;
  	color_1 = (color_1 + (tap_11 * float4(0.232, 0.232, 0.232, 0.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_13;
  	fixed4 tmpvar_14;
  	tmpvar_14 = tex2D (_MainTex, coords_2);
  	tap_13 = tmpvar_14;
  	color_1 = (color_1 + (tap_13 * float4(0.0855, 0.0855, 0.0855, 0.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_15;
  	fixed4 tmpvar_16;
  	tmpvar_16 = tex2D (_MainTex, coords_2);
  	tap_15 = tmpvar_16;
  	color_1 = (color_1 + (tap_15 * float4(0.0205, 0.0205, 0.0205, 0.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	bolOut = color_1;
  	return bolOut;
  }
  ENDCG
 }
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  half4 _MainTex_TexelSize;
  half4 _Parameter;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	half4 xlv_TEXCOORD0 : TEXCOORD0;
  	half2 xlv_TEXCOORD1 : TEXCOORD1;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	half4 tmpvar_1;
  	tmpvar_1.zw = float2(1.0, 1.0);
  	tmpvar_1.xy = v.texcoord.xy;
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = tmpvar_1;
  	o.xlv_TEXCOORD1 = ((_MainTex_TexelSize.xy * float2(1.0, 0.0)) * _Parameter.x);
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	half4 color_1;
  	half2 coords_2;
  	coords_2 = (i.xlv_TEXCOORD0.xy - (i.xlv_TEXCOORD1 * 3.0));
  	half4 tap_3;
  	fixed4 tmpvar_4;
  	tmpvar_4 = tex2D (_MainTex, coords_2);
  	tap_3 = tmpvar_4;
  	color_1 = (tap_3 * float4(0.0205, 0.0205, 0.0205, 0.0));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_5;
  	fixed4 tmpvar_6;
  	tmpvar_6 = tex2D (_MainTex, coords_2);
  	tap_5 = tmpvar_6;
  	color_1 = (color_1 + (tap_5 * float4(0.0855, 0.0855, 0.0855, 0.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_7;
  	fixed4 tmpvar_8;
  	tmpvar_8 = tex2D (_MainTex, coords_2);
  	tap_7 = tmpvar_8;
  	color_1 = (color_1 + (tap_7 * float4(0.232, 0.232, 0.232, 0.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_9;
  	fixed4 tmpvar_10;
  	tmpvar_10 = tex2D (_MainTex, coords_2);
  	tap_9 = tmpvar_10;
  	color_1 = (color_1 + (tap_9 * float4(0.324, 0.324, 0.324, 1.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_11;
  	fixed4 tmpvar_12;
  	tmpvar_12 = tex2D (_MainTex, coords_2);
  	tap_11 = tmpvar_12;
  	color_1 = (color_1 + (tap_11 * float4(0.232, 0.232, 0.232, 0.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_13;
  	fixed4 tmpvar_14;
  	tmpvar_14 = tex2D (_MainTex, coords_2);
  	tap_13 = tmpvar_14;
  	color_1 = (color_1 + (tap_13 * float4(0.0855, 0.0855, 0.0855, 0.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	half4 tap_15;
  	fixed4 tmpvar_16;
  	tmpvar_16 = tex2D (_MainTex, coords_2);
  	tap_15 = tmpvar_16;
  	color_1 = (color_1 + (tap_15 * float4(0.0205, 0.0205, 0.0205, 0.0)));
  	coords_2 = (coords_2 + i.xlv_TEXCOORD1);
  	bolOut = color_1;
  	return bolOut;
  }
  ENDCG
 }
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  half4 _MainTex_TexelSize;
  half4 _Parameter;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	half2 xlv_TEXCOORD0 : TEXCOORD0;
  	half4 xlv_TEXCOORD1 : TEXCOORD1;
  	half4 xlv_TEXCOORD1_1 : TEXCOORD2;
  	half4 xlv_TEXCOORD1_2 : TEXCOORD3;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	half4 tmpvar_1;
  	tmpvar_1.zw = float2(1.0, 1.0);
  	tmpvar_1.xy = v.texcoord.xy;
  	half2 tmpvar_2;
  	tmpvar_2 = ((_MainTex_TexelSize.xy * float2(0.0, 1.0)) * _Parameter.x);
  	half4 tmpvar_3;
  	tmpvar_3 = (-(tmpvar_2.xyxy) * 3.0);
  	half4 tmpvar_4;
  	tmpvar_4 = (tmpvar_3 + tmpvar_2.xyxy);
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = tmpvar_1.xy;
  	o.xlv_TEXCOORD1 = (v.texcoord.xyxy + (tmpvar_3 * float4(1.0, 1.0, -1.0, -1.0)));
  	o.xlv_TEXCOORD1_1 = (v.texcoord.xyxy + (tmpvar_4 * float4(1.0, 1.0, -1.0, -1.0)));
  	o.xlv_TEXCOORD1_2 = (v.texcoord.xyxy + ((tmpvar_4 + tmpvar_2.xyxy) * float4(1.0, 1.0, -1.0, -1.0)));
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	half4 color_1;
  	fixed4 tmpvar_2;
  	tmpvar_2 = tex2D (_MainTex, i.xlv_TEXCOORD0);
  	color_1 = (tmpvar_2 * float4(0.324, 0.324, 0.324, 1.0));
  	half4 tapB_3;
  	half4 tapA_4;
  	fixed4 tmpvar_5;
  	tmpvar_5 = tex2D (_MainTex, i.xlv_TEXCOORD1.xy);
  	tapA_4 = tmpvar_5;
  	fixed4 tmpvar_6;
  	tmpvar_6 = tex2D (_MainTex, i.xlv_TEXCOORD1.zw);
  	tapB_3 = tmpvar_6;
  	color_1 = (color_1 + ((tapA_4 + tapB_3) * float4(0.0205, 0.0205, 0.0205, 0.0)));
  	half4 tapB_7;
  	half4 tapA_8;
  	fixed4 tmpvar_9;
  	tmpvar_9 = tex2D (_MainTex, i.xlv_TEXCOORD1_1.xy);
  	tapA_8 = tmpvar_9;
  	fixed4 tmpvar_10;
  	tmpvar_10 = tex2D (_MainTex, i.xlv_TEXCOORD1_1.zw);
  	tapB_7 = tmpvar_10;
  	color_1 = (color_1 + ((tapA_8 + tapB_7) * float4(0.0855, 0.0855, 0.0855, 0.0)));
  	half4 tapB_11;
  	half4 tapA_12;
  	fixed4 tmpvar_13;
  	tmpvar_13 = tex2D (_MainTex, i.xlv_TEXCOORD1_2.xy);
  	tapA_12 = tmpvar_13;
  	fixed4 tmpvar_14;
  	tmpvar_14 = tex2D (_MainTex, i.xlv_TEXCOORD1_2.zw);
  	tapB_11 = tmpvar_14;
  	color_1 = (color_1 + ((tapA_12 + tapB_11) * float4(0.232, 0.232, 0.232, 0.0)));
  	bolOut = color_1;
  	return bolOut;
  }
  ENDCG
 }
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  half4 _MainTex_TexelSize;
  half4 _Parameter;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	half2 xlv_TEXCOORD0 : TEXCOORD0;
  	half4 xlv_TEXCOORD1 : TEXCOORD1;
  	half4 xlv_TEXCOORD1_1 : TEXCOORD2;
  	half4 xlv_TEXCOORD1_2 : TEXCOORD3;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	half2 tmpvar_1;
  	tmpvar_1 = ((_MainTex_TexelSize.xy * float2(1.0, 0.0)) * _Parameter.x);
  	half4 tmpvar_2;
  	tmpvar_2 = (-(tmpvar_1.xyxy) * 3.0);
  	half4 tmpvar_3;
  	tmpvar_3 = (tmpvar_2 + tmpvar_1.xyxy);
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = v.texcoord.xy;
  	o.xlv_TEXCOORD1 = (v.texcoord.xyxy + (tmpvar_2 * float4(1.0, 1.0, -1.0, -1.0)));
  	o.xlv_TEXCOORD1_1 = (v.texcoord.xyxy + (tmpvar_3 * float4(1.0, 1.0, -1.0, -1.0)));
  	o.xlv_TEXCOORD1_2 = (v.texcoord.xyxy + ((tmpvar_3 + tmpvar_1.xyxy) * float4(1.0, 1.0, -1.0, -1.0)));
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	half4 color_1;
  	fixed4 tmpvar_2;
  	tmpvar_2 = tex2D (_MainTex, i.xlv_TEXCOORD0);
  	color_1 = (tmpvar_2 * float4(0.324, 0.324, 0.324, 1.0));
  	half4 tapB_3;
  	half4 tapA_4;
  	fixed4 tmpvar_5;
  	tmpvar_5 = tex2D (_MainTex, i.xlv_TEXCOORD1.xy);
  	tapA_4 = tmpvar_5;
  	fixed4 tmpvar_6;
  	tmpvar_6 = tex2D (_MainTex, i.xlv_TEXCOORD1.zw);
  	tapB_3 = tmpvar_6;
  	color_1 = (color_1 + ((tapA_4 + tapB_3) * float4(0.0205, 0.0205, 0.0205, 0.0)));
  	half4 tapB_7;
  	half4 tapA_8;
  	fixed4 tmpvar_9;
  	tmpvar_9 = tex2D (_MainTex, i.xlv_TEXCOORD1_1.xy);
  	tapA_8 = tmpvar_9;
  	fixed4 tmpvar_10;
  	tmpvar_10 = tex2D (_MainTex, i.xlv_TEXCOORD1_1.zw);
  	tapB_7 = tmpvar_10;
  	color_1 = (color_1 + ((tapA_8 + tapB_7) * float4(0.0855, 0.0855, 0.0855, 0.0)));
  	half4 tapB_11;
  	half4 tapA_12;
  	fixed4 tmpvar_13;
  	tmpvar_13 = tex2D (_MainTex, i.xlv_TEXCOORD1_2.xy);
  	tapA_12 = tmpvar_13;
  	fixed4 tmpvar_14;
  	tmpvar_14 = tex2D (_MainTex, i.xlv_TEXCOORD1_2.zw);
  	tapB_11 = tmpvar_14;
  	color_1 = (color_1 + ((tapA_12 + tapB_11) * float4(0.232, 0.232, 0.232, 0.0)));
  	bolOut = color_1;
  	return bolOut;
  }
  ENDCG
 }
}
Fallback Off
}