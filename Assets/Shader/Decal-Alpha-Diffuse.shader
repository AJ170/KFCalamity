// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs: Lambert, texture * _Color, alpha blended, drawn
// slightly in front of the surface it lies on (Offset -1, -1). Lighting and lightmaps from Unity 2018's Lambert.
Shader "Decal/Transparent Diffuse" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
SubShader {
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 ZWrite Off
 ColorMask RGB
 Offset -1, -1
 CGPROGRAM
 #pragma surface surf Lambert alpha

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
Fallback "Legacy Shaders/Transparent/VertexLit"
}
