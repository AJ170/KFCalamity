// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Hidden/Shadow-ScreenBlur" {
Properties {
 _MainTex ("Base", 2D) = "white" {}
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  float4 _BlurOffsets0;
  float4 _BlurOffsets1;
  float4 _BlurOffsets2;
  float4 _BlurOffsets3;
  float4 _BlurOffsets4;
  float4 _BlurOffsets5;
  float4 _BlurOffsets6;
  float4 _BlurOffsets7;
  sampler2D _MainTex;
  float4 unity_ShadowBlurParams;

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
  	float4 theSample_7_2;
  	float4 theSample_6_3;
  	float4 theSample_5_4;
  	float4 theSample_4_5;
  	float4 theSample_3_6;
  	float4 theSample_2_7;
  	float4 theSample_1_8;
  	float4 theSample_9;
  	float4 mask_10;
  	float4 coord_11;
  	half4 tmpvar_12;
  	tmpvar_12.zw = float2(0.0, 0.0);
  	tmpvar_12.xy = i.xlv_TEXCOORD0;
  	coord_11 = tmpvar_12;
  	fixed4 tmpvar_13;
  	tmpvar_13 = tex2D (_MainTex, coord_11.xy);
  	mask_10 = tmpvar_13;
  	float tmpvar_14;
  	tmpvar_14 = (mask_10.z + (mask_10.w / 255.0));
  	float tmpvar_15;
  	tmpvar_15 = clamp ((unity_ShadowBlurParams.y / (1.0 - tmpvar_14)), 0.0, 1.0);
  	mask_10.xy = (mask_10.xy * unity_ShadowBlurParams.x);
  	fixed4 tmpvar_16;
  	float2 P_17;
  	P_17 = (coord_11 + (tmpvar_15 * _BlurOffsets0)).xy;
  	tmpvar_16 = tex2D (_MainTex, P_17);
  	theSample_9 = tmpvar_16;
  	mask_10.xy = (mask_10.xy + (clamp (
  	(unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_9.z +
  	(theSample_9.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_9.xy));
  	fixed4 tmpvar_18;
  	float2 P_19;
  	P_19 = (coord_11 + (tmpvar_15 * _BlurOffsets1)).xy;
  	tmpvar_18 = tex2D (_MainTex, P_19);
  	theSample_1_8 = tmpvar_18;
  	mask_10.xy = (mask_10.xy + (clamp (
  	(unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_1_8.z +
  	(theSample_1_8.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_1_8.xy));
  	fixed4 tmpvar_20;
  	float2 P_21;
  	P_21 = (coord_11 + (tmpvar_15 * _BlurOffsets2)).xy;
  	tmpvar_20 = tex2D (_MainTex, P_21);
  	theSample_2_7 = tmpvar_20;
  	mask_10.xy = (mask_10.xy + (clamp (
  	(unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_2_7.z +
  	(theSample_2_7.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_2_7.xy));
  	fixed4 tmpvar_22;
  	float2 P_23;
  	P_23 = (coord_11 + (tmpvar_15 * _BlurOffsets3)).xy;
  	tmpvar_22 = tex2D (_MainTex, P_23);
  	theSample_3_6 = tmpvar_22;
  	mask_10.xy = (mask_10.xy + (clamp (
  	(unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_3_6.z +
  	(theSample_3_6.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_3_6.xy));
  	fixed4 tmpvar_24;
  	float2 P_25;
  	P_25 = (coord_11 + (tmpvar_15 * _BlurOffsets4)).xy;
  	tmpvar_24 = tex2D (_MainTex, P_25);
  	theSample_4_5 = tmpvar_24;
  	mask_10.xy = (mask_10.xy + (clamp (
  	(unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_4_5.z +
  	(theSample_4_5.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_4_5.xy));
  	fixed4 tmpvar_26;
  	float2 P_27;
  	P_27 = (coord_11 + (tmpvar_15 * _BlurOffsets5)).xy;
  	tmpvar_26 = tex2D (_MainTex, P_27);
  	theSample_5_4 = tmpvar_26;
  	mask_10.xy = (mask_10.xy + (clamp (
  	(unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_5_4.z +
  	(theSample_5_4.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_5_4.xy));
  	fixed4 tmpvar_28;
  	float2 P_29;
  	P_29 = (coord_11 + (tmpvar_15 * _BlurOffsets6)).xy;
  	tmpvar_28 = tex2D (_MainTex, P_29);
  	theSample_6_3 = tmpvar_28;
  	mask_10.xy = (mask_10.xy + (clamp (
  	(unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_6_3.z +
  	(theSample_6_3.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_6_3.xy));
  	fixed4 tmpvar_30;
  	float2 P_31;
  	P_31 = (coord_11 + (tmpvar_15 * _BlurOffsets7)).xy;
  	tmpvar_30 = tex2D (_MainTex, P_31);
  	theSample_7_2 = tmpvar_30;
  	mask_10.xy = (mask_10.xy + (clamp (
  	(unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_7_2.z +
  	(theSample_7_2.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_7_2.xy));
  	float4 tmpvar_32;
  	tmpvar_32 = ((float4)((mask_10.x / mask_10.y)));
  	tmpvar_1 = tmpvar_32;
  	bolOut = tmpvar_1;
  	return bolOut;
  }
  ENDCG
 }
}
Fallback Off
}