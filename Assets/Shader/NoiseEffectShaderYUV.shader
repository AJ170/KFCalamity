// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Hidden/Noise Shader YUV" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _GrainTex ("Base (RGB)", 2D) = "gray" {}
 _ScratchTex ("Base (RGB)", 2D) = "gray" {}
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

  float4 _GrainOffsetScale;
  sampler2D _GrainTex;
  fixed4 _Intensity;
  sampler2D _MainTex;
  float4 _ScratchOffsetScale;
  sampler2D _ScratchTex;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float2 xlv_TEXCOORD0 : TEXCOORD0;
  	float2 xlv_TEXCOORD1 : TEXCOORD1;
  	float2 xlv_TEXCOORD2 : TEXCOORD2;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	half2 tmpvar_1;
  	tmpvar_1 = v.texcoord.xy;
  	float2 inUV_2;
  	inUV_2 = tmpvar_1;
  	float4 tmpvar_3;
  	tmpvar_3.zw = float2(0.0, 0.0);
  	tmpvar_3.xy = inUV_2;
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = (tmpvar_3).xy;
  	o.xlv_TEXCOORD1 = ((v.texcoord.xy * _GrainOffsetScale.zw) + _GrainOffsetScale.xy);
  	o.xlv_TEXCOORD2 = ((v.texcoord.xy * _ScratchOffsetScale.zw) + _ScratchOffsetScale.xy);
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed3 yuv_1;
  	fixed4 col_2;
  	fixed4 tmpvar_3;
  	tmpvar_3 = tex2D (_MainTex, i.xlv_TEXCOORD0);
  	col_2.w = tmpvar_3.w;
  	yuv_1.x = dot (tmpvar_3.xyz, float3(0.299, 0.587, 0.114));
  	yuv_1.y = ((tmpvar_3.z - yuv_1.x) * 0.492);
  	yuv_1.z = ((tmpvar_3.x - yuv_1.x) * 0.877);
  	yuv_1 = (yuv_1 + ((
  	(tex2D (_GrainTex, i.xlv_TEXCOORD1).xyz * 2.0)
  	- 1.0) * _Intensity.x));
  	col_2.x = ((yuv_1.z * 1.14) + yuv_1.x);
  	col_2.y = (((yuv_1.z * -0.581) + (yuv_1.y * -0.395)) + yuv_1.x);
  	col_2.z = ((yuv_1.y * 2.032) + yuv_1.x);
  	col_2.xyz = (col_2.xyz + ((
  	(tex2D (_ScratchTex, i.xlv_TEXCOORD2).xyz * 2.0)
  	- 1.0) * _Intensity.y));
  	bolOut = col_2;
  	return bolOut;
  }
  ENDCG
 }
}
Fallback Off
}