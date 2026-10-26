// Rebuilt for Unity 2018 / PS Vita from the game's GLES program (Unity 4 Standard Assets simple water). Unity 4's
// unity_Scale maths is written in world space as Unity 5's own port of this shader does (same result unscaled).
Shader "FX/Water (simple)" {
Properties {
 _horizonColor ("Horizon color", Color) = (0.172,0.463,0.435,0)
 _WaveScale ("Wave scale", Range(0.02,0.15)) = 0.07
 _ColorControl ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
 _BumpMap ("Waves Normalmap ", 2D) = "" {}
 WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
 _MainTex ("Fallback texture", 2D) = "" {}
}
SubShader {
 Tags { "RenderType"="Opaque" }
 Pass {
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  float _WaveScale;
  float4 _WaveOffset;
  float4 _horizonColor;
  sampler2D _BumpMap;
  sampler2D _ColorControl;

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
  	float4 temp = wpos.xzxz * _WaveScale + _WaveOffset;
  	o.bumpuv0 = temp.xy * float2(0.4, 0.45);
  	o.bumpuv1 = temp.wz;
  	o.viewDir = normalize(_WorldSpaceCameraPos - wpos.xyz).xzy;
  	return o;
  }

  half4 frag (v2f i) : SV_Target
  {
  	half3 bump1 = tex2D(_BumpMap, i.bumpuv0).xyz * 2.0 - 1.0;
  	half3 bump2 = tex2D(_BumpMap, i.bumpuv1).xyz * 2.0 - 1.0;
  	half3 bump = (bump1 + bump2) * 0.5;
  	half fresnel = dot(i.viewDir, bump);
  	half4 water = tex2D(_ColorControl, float2(fresnel, fresnel));
  	half4 color;
  	color.rgb = lerp(water.rgb, _horizonColor.rgb, water.a);
  	color.a = _horizonColor.a;
  	return color;
  }
  ENDCG
 }
}
}
