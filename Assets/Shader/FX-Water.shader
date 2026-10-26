// Rebuilt for Unity 2018 / PS Vita from the game's GLES WATER_SIMPLE program (Unity 4 Standard Assets water).
// Only the simple mode is kept: the "WaterMode"="Simple" tag makes Water.cs skip its reflection and refraction cameras,
// which would render the level two more times every frame. Unity 4's unity_Scale maths is written in world space as
// Unity 5's own port of this shader does (same result for unscaled water).
Shader "FX/Water" {
Properties {
 _WaveScale ("Wave scale", Range(0.02,0.15)) = 0.063
 _ReflDistort ("Reflection distort", Range(0,1.5)) = 0.44
 _RefrDistort ("Refraction distort", Range(0,1.5)) = 0.4
 _RefrColor ("Refraction color", Color) = (0.34,0.85,0.92,1)
 _Fresnel ("Fresnel (A) ", 2D) = "gray" {}
 _BumpMap ("Normalmap ", 2D) = "bump" {}
 WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
 _ReflectiveColor ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
 _HorizonColor ("Simple water horizon color", Color) = (0.172,0.463,0.435,1)
 _MainTex ("Fallback texture", 2D) = "" {}
 _ReflectionTex ("Internal Reflection", 2D) = "" {}
 _RefractionTex ("Internal Refraction", 2D) = "" {}
}
SubShader {
 Tags { "RenderType"="Opaque" "WaterMode"="Simple" }
 Pass {
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  float4 _WaveScale4;
  float4 _WaveOffset;
  sampler2D _BumpMap;
  sampler2D _ReflectiveColor;
  float4 _HorizonColor;

  struct v2f
  {
  	float4 pos : SV_POSITION;
  	float2 bumpuv0 : TEXCOORD0;
  	float2 bumpuv1 : TEXCOORD1;
  	float3 viewDir : TEXCOORD2;
  };

  v2f vert (float4 vertex : POSITION)
  {
  	v2f o;
  	o.pos = UnityObjectToClipPos(vertex);
  	float4 wpos = mul(unity_ObjectToWorld, vertex);
  	float4 temp = wpos.xzxz * _WaveScale4 + _WaveOffset;
  	o.bumpuv0 = temp.xy;
  	o.bumpuv1 = temp.wz;
  	o.viewDir = (_WorldSpaceCameraPos - wpos.xyz).xzy;
  	return o;
  }

  half4 frag (v2f i) : SV_Target
  {
  	half3 bump1 = tex2D(_BumpMap, i.bumpuv0).xyz * 2.0 - 1.0;
  	half3 bump2 = tex2D(_BumpMap, i.bumpuv1).xyz * 2.0 - 1.0;
  	half3 bump = (bump1 + bump2) * 0.5;
  	half fresnel = dot(normalize(i.viewDir), bump);
  	half4 water = tex2D(_ReflectiveColor, float2(fresnel, fresnel));
  	half4 color;
  	color.rgb = lerp(water.rgb, _HorizonColor.rgb, water.a);
  	color.a = _HorizonColor.a;
  	return color;
  }
  ENDCG
 }
}
}
