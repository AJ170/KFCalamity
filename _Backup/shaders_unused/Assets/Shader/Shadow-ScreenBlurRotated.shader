// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Hidden/Shadow-ScreenBlurRotated" {
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
  sampler2D unity_RandomRotation16;
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
  	half2 tmpvar_1;
  	tmpvar_1 = v.texcoord.xy;
  	half2 tmpvar_2;
  	float2 tmpvar_3;
  	float2 inUV_4;
  	inUV_4 = tmpvar_1;
  	float4 tmpvar_5;
  	tmpvar_5.zw = float2(0.0, 0.0);
  	tmpvar_5.xy = inUV_4;
  	tmpvar_3 = (tmpvar_5).xy;
  	tmpvar_2 = tmpvar_3;
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = tmpvar_2;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	half4 theSample_7_2;
  	half4 theSample_6_3;
  	half4 theSample_5_4;
  	half4 theSample_4_5;
  	half4 theSample_3_6;
  	half4 theSample_2_7;
  	half4 theSample_1_8;
  	half4 theSample_9;
  	half diffTolerance_10;
  	half4 mask_11;
  	half4 rotation_12;
  	float4 coord_13;
  	half4 tmpvar_14;
  	tmpvar_14.zw = float2(0.0, 0.0);
  	tmpvar_14.xy = i.xlv_TEXCOORD0;
  	coord_13 = tmpvar_14;
  	float2 P_15;
  	P_15 = ((coord_13.xy * _ScreenParams.xy) / 16.0);
  	fixed4 tmpvar_16;
  	tmpvar_16 = ((2.0 * tex2D (unity_RandomRotation16, P_15)) - 1.0);
  	rotation_12 = tmpvar_16;
  	fixed4 tmpvar_17;
  	tmpvar_17 = tex2D (_MainTex, coord_13.xy);
  	mask_11 = tmpvar_17;
  	half tmpvar_18;
  	tmpvar_18 = (mask_11.z + (mask_11.w / 255.0));
  	half tmpvar_19;
  	float tmpvar_20;
  	tmpvar_20 = clamp ((unity_ShadowBlurParams.y / (1.0 - tmpvar_18)), 0.0, 1.0);
  	tmpvar_19 = tmpvar_20;
  	float tmpvar_21;
  	tmpvar_21 = unity_ShadowBlurParams.x;
  	diffTolerance_10 = tmpvar_21;
  	mask_11.xy = (mask_11.xy * diffTolerance_10);
  	float4 rotation_22;
  	rotation_22 = rotation_12;
  	float2 offset_23;
  	offset_23.x = dot (_BlurOffsets0.xy, rotation_22.xy);
  	offset_23.y = dot (_BlurOffsets0.xy, rotation_22.zw);
  	fixed4 tmpvar_24;
  	float2 P_25;
  	P_25 = (coord_13.xy + (tmpvar_19 * offset_23));
  	tmpvar_24 = tex2D (_MainTex, P_25);
  	theSample_9 = tmpvar_24;
  	mask_11.xy = (mask_11.xy + (clamp (
  	(diffTolerance_10 - abs((tmpvar_18 - (theSample_9.z +
  	(theSample_9.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_9.xy));
  	float4 rotation_26;
  	rotation_26 = rotation_12;
  	float2 offset_27;
  	offset_27.x = dot (_BlurOffsets1.xy, rotation_26.xy);
  	offset_27.y = dot (_BlurOffsets1.xy, rotation_26.zw);
  	fixed4 tmpvar_28;
  	float2 P_29;
  	P_29 = (coord_13.xy + (tmpvar_19 * offset_27));
  	tmpvar_28 = tex2D (_MainTex, P_29);
  	theSample_1_8 = tmpvar_28;
  	mask_11.xy = (mask_11.xy + (clamp (
  	(diffTolerance_10 - abs((tmpvar_18 - (theSample_1_8.z +
  	(theSample_1_8.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_1_8.xy));
  	float4 rotation_30;
  	rotation_30 = rotation_12;
  	float2 offset_31;
  	offset_31.x = dot (_BlurOffsets2.xy, rotation_30.xy);
  	offset_31.y = dot (_BlurOffsets2.xy, rotation_30.zw);
  	fixed4 tmpvar_32;
  	float2 P_33;
  	P_33 = (coord_13.xy + (tmpvar_19 * offset_31));
  	tmpvar_32 = tex2D (_MainTex, P_33);
  	theSample_2_7 = tmpvar_32;
  	mask_11.xy = (mask_11.xy + (clamp (
  	(diffTolerance_10 - abs((tmpvar_18 - (theSample_2_7.z +
  	(theSample_2_7.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_2_7.xy));
  	float4 rotation_34;
  	rotation_34 = rotation_12;
  	float2 offset_35;
  	offset_35.x = dot (_BlurOffsets3.xy, rotation_34.xy);
  	offset_35.y = dot (_BlurOffsets3.xy, rotation_34.zw);
  	fixed4 tmpvar_36;
  	float2 P_37;
  	P_37 = (coord_13.xy + (tmpvar_19 * offset_35));
  	tmpvar_36 = tex2D (_MainTex, P_37);
  	theSample_3_6 = tmpvar_36;
  	mask_11.xy = (mask_11.xy + (clamp (
  	(diffTolerance_10 - abs((tmpvar_18 - (theSample_3_6.z +
  	(theSample_3_6.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_3_6.xy));
  	float4 rotation_38;
  	rotation_38 = rotation_12;
  	float2 offset_39;
  	offset_39.x = dot (_BlurOffsets4.xy, rotation_38.xy);
  	offset_39.y = dot (_BlurOffsets4.xy, rotation_38.zw);
  	fixed4 tmpvar_40;
  	float2 P_41;
  	P_41 = (coord_13.xy + (tmpvar_19 * offset_39));
  	tmpvar_40 = tex2D (_MainTex, P_41);
  	theSample_4_5 = tmpvar_40;
  	mask_11.xy = (mask_11.xy + (clamp (
  	(diffTolerance_10 - abs((tmpvar_18 - (theSample_4_5.z +
  	(theSample_4_5.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_4_5.xy));
  	float4 rotation_42;
  	rotation_42 = rotation_12;
  	float2 offset_43;
  	offset_43.x = dot (_BlurOffsets5.xy, rotation_42.xy);
  	offset_43.y = dot (_BlurOffsets5.xy, rotation_42.zw);
  	fixed4 tmpvar_44;
  	float2 P_45;
  	P_45 = (coord_13.xy + (tmpvar_19 * offset_43));
  	tmpvar_44 = tex2D (_MainTex, P_45);
  	theSample_5_4 = tmpvar_44;
  	mask_11.xy = (mask_11.xy + (clamp (
  	(diffTolerance_10 - abs((tmpvar_18 - (theSample_5_4.z +
  	(theSample_5_4.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_5_4.xy));
  	float4 rotation_46;
  	rotation_46 = rotation_12;
  	float2 offset_47;
  	offset_47.x = dot (_BlurOffsets6.xy, rotation_46.xy);
  	offset_47.y = dot (_BlurOffsets6.xy, rotation_46.zw);
  	fixed4 tmpvar_48;
  	float2 P_49;
  	P_49 = (coord_13.xy + (tmpvar_19 * offset_47));
  	tmpvar_48 = tex2D (_MainTex, P_49);
  	theSample_6_3 = tmpvar_48;
  	mask_11.xy = (mask_11.xy + (clamp (
  	(diffTolerance_10 - abs((tmpvar_18 - (theSample_6_3.z +
  	(theSample_6_3.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_6_3.xy));
  	float4 rotation_50;
  	rotation_50 = rotation_12;
  	float2 offset_51;
  	offset_51.x = dot (_BlurOffsets7.xy, rotation_50.xy);
  	offset_51.y = dot (_BlurOffsets7.xy, rotation_50.zw);
  	fixed4 tmpvar_52;
  	float2 P_53;
  	P_53 = (coord_13.xy + (tmpvar_19 * offset_51));
  	tmpvar_52 = tex2D (_MainTex, P_53);
  	theSample_7_2 = tmpvar_52;
  	mask_11.xy = (mask_11.xy + (clamp (
  	(diffTolerance_10 - abs((tmpvar_18 - (theSample_7_2.z +
  	(theSample_7_2.w / 255.0)
  	))))
  	, 0.0, 1.0) * theSample_7_2.xy));
  	half4 tmpvar_54;
  	tmpvar_54 = ((float4)((mask_11.x / mask_11.y)));
  	tmpvar_1 = tmpvar_54;
  	bolOut = tmpvar_1;
  	return bolOut;
  }
  ENDCG
 }
}
Fallback Off
}