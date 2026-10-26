// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs: Lambert, alpha cut out at _Cutoff, one light pass
// only (the original had no ForwardAdd pass), drawn slightly in front of the surface (Offset -1, -1).
Shader "Decal/Mobile/Cutout Diffuse" {
Properties {
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

 struct Input
 {
 	float2 uv_MainTex;
 };

 void surf (Input IN, inout SurfaceOutput o)
 {
 	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
 	o.Albedo = c.rgb;
 	o.Alpha = c.a;
 }
 ENDCG
}
Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
