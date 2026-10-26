// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Hidden/Grayscale Effect" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _RampTex ("Base (RGB)", 2D) = "grayscaleRamp" {}
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

  sampler2D _MainTex;
  half _RampOffset;
  sampler2D _RampTex;

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
  	fixed4 xlat_varoutput_1;
  	fixed4 tmpvar_2;
  	tmpvar_2 = tex2D (_MainTex, i.xlv_TEXCOORD0);
  	fixed tmpvar_3;
  	tmpvar_3 = dot (tmpvar_2.xyz, float3(0.22, 0.707, 0.071));
  	half2 tmpvar_4;
  	tmpvar_4.y = 0.5;
  	tmpvar_4.x = (tmpvar_3 + _RampOffset);
  	xlat_varoutput_1.xyz = tex2D (_RampTex, tmpvar_4).xyz;
  	xlat_varoutput_1.w = tmpvar_2.w;
  	bolOut = xlat_varoutput_1;
  	return bolOut;
  }
  ENDCG
 }
}
Fallback Off
}