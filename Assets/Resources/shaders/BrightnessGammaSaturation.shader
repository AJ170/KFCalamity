// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Hidden/Brightness" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _SaturationAmount ("Saturation Amount", Range(0,1)) = 1
 _BrightnessAmount ("Brightness Amount", Range(0,1)) = 1
 _ContrastAmount ("Contrast Amount", Range(0,1)) = 1
}
SubShader { 
 Pass {
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  float _BrightnessAmount;
  float _ContrastAmount;
  sampler2D _MainTex;
  float _SaturationAmount;

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
  	float4 renderTex_1;
  	fixed4 tmpvar_2;
  	tmpvar_2 = tex2D (_MainTex, i.xlv_TEXCOORD0);
  	renderTex_1 = tmpvar_2;
  	float3 tmpvar_3;
  	tmpvar_3 = (renderTex_1.xyz * _BrightnessAmount);
  	renderTex_1.xyz = lerp (float3(0.5, 0.5, 0.5), lerp (((float3)(dot (tmpvar_3, float3(0.2125, 0.7154, 0.0721)))), tmpvar_3, ((float3)(_SaturationAmount))), ((float3)(_ContrastAmount)));
  	bolOut = renderTex_1;
  	return bolOut;
  }
  ENDCG
 }
}
}