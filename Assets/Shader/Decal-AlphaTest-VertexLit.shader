// Rebuilt for Unity 2018 / PS Vita. The Unity 4 original was a fixed-function vertex-lit cutout decal (texture *
// _Color, alpha cut out at _Cutoff, Offset -1, -1); this is the same as a Lambert surface shader (no level material
// uses it at the moment).
Shader "Decal/Cutout VertexLit" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
SubShader {
 Tags { "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="true" "RenderType"="TransparentCutout" }
 ColorMask RGB
 Offset -1, -1
 CGPROGRAM
 #pragma surface surf Lambert alphatest:_Cutoff noforwardadd

 sampler2D _MainTex;
 fixed4 _Color;

 struct Input
 {
 	float2 uv_MainTex;
 };

 void surf (Input IN, inout SurfaceOutput o)
 {
 	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
 	o.Albedo = c.rgb;
 	o.Alpha = c.a;
 }
 ENDCG
}
Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
