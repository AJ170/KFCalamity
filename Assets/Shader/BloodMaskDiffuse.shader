// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs. The Unity 4 original was a Lambert surface shader;
// its albedo maths is kept as compiled (the blood mask is the base texture's alpha, thresholded by _BloodAmount and
// capped at 0.8, the hit flash is added to the albedo). Lighting and lightmaps come from Unity 2018's Lambert, which
// matches Unity 4's once the light intensities are doubled (Unity's upgrade does that).
Shader "Custom/BloodMaskDiffuse" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _BloodTexture ("Blood Texture", 2D) = "white" {}
 _HitColor ("Hit Color", Color) = (0,0,0,1)
 _BloodColor ("Blood Color", Color) = (0.5,0,0,1)
 _BloodAmount ("Blood Amount", Range(0,0.7)) = 0.7
}
SubShader {
 Tags { "RenderType"="Opaque" }
 LOD 200
 CGPROGRAM
 #pragma surface surf Lambert

 sampler2D _MainTex;
 sampler2D _BloodTexture;
 float _BloodAmount;
 float4 _HitColor;

 struct Input
 {
 	float2 uv_MainTex;
 	float2 uv_BloodTexture;
 };

 void surf (Input IN, inout SurfaceOutput o)
 {
 	half4 c = tex2D(_MainTex, IN.uv_MainTex);
 	half4 blood = tex2D(_BloodTexture, IN.uv_BloodTexture);
 	float mask = clamp(ceil(c.a - (1.0 - _BloodAmount)), 0.0, 0.8);
 	o.Albedo = c.rgb * (1.0 - mask) + blood.rgb * mask + _HitColor.rgb;
 	o.Alpha = 0.0;
 }
 ENDCG
}
Fallback "Legacy Shaders/VertexLit"
}
