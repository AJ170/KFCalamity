// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs. The Unity 4 original was a BlinnPhong surface
// shader blending four layers by their height (alpha) weighted with the splat map; that maths is kept as compiled
// (weights normalised, sharpened towards the highest layer by _BlendSoft, layer normals blended the same way and scaled
// by _NScale; normals are plain RGB maps, unpacked as *2-1 like the original). Lighting and lightmaps come from Unity
// 2018's BlinnPhong (_Shininess = specular power / 128, _Spec = gloss), which matches Unity 4's with the upgraded,
// doubled light intensities.
Shader "Custom/splatmap" {
Properties {
 _Color ("Main Color", Color) = (0.8,0.8,0.8,0)
 _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
 _Spec ("Specular Power", Float) = 0.2
 _Shininess ("Glossiness", Float) = 0.15
 _BlendSoft ("Texture Blend Softness", Range(0,1)) = 0.1
 _NScale ("Normals Scale", Range(1,4)) = 2
 _Tex_Splat ("Splat map (RGBA)", 2D) = "white" {}
 _Tex_DiffuseR ("Splat Diffuse R (RGB), Height (A)", 2D) = "white" {}
 _Tex_DiffuseG ("Splat Diffuse G (RGB), Height (A)", 2D) = "white" {}
 _Tex_DiffuseB ("Splat Diffuse B (RGB), Height (A)", 2D) = "white" {}
 _Tex_DiffuseA ("Splat Diffuse A (RGB), Height (A)", 2D) = "white" {}
 _Tex_NormalR ("Splat Normal R", 2D) = "bump" {}
 _Tex_NormalG ("Splat Normal G", 2D) = "bump" {}
 _Tex_NormalB ("Splat Normal B", 2D) = "bump" {}
 _Tex_NormalA ("Splat Normal A", 2D) = "bump" {}
}
SubShader {
 Tags { "RenderType"="Opaque" }
 CGPROGRAM
 // the levels' lightmaps are non-directional (KFSceneLightmaps) and there is no real-time GI; the directional lightmap
 // variants also run out of interpolators on the PS Vita
 #pragma surface surf BlinnPhong nodirlightmap nodynlightmap
 #pragma target 3.0

 sampler2D _Tex_Splat;
 sampler2D _Tex_DiffuseR, _Tex_DiffuseG, _Tex_DiffuseB, _Tex_DiffuseA;
 sampler2D _Tex_NormalR, _Tex_NormalG, _Tex_NormalB, _Tex_NormalA;
 fixed4 _Color;
 half _Spec;
 half _Shininess;
 half _BlendSoft;
 half _NScale;

 struct Input
 {
 	float2 uv_Tex_Splat;
 	float2 uv_Tex_DiffuseR;
 	float2 uv_Tex_DiffuseG;
 	float2 uv_Tex_DiffuseB;
 	float2 uv_Tex_DiffuseA;
 };

 void surf (Input IN, inout SurfaceOutput o)
 {
 	float4 splat = tex2D(_Tex_Splat, IN.uv_Tex_Splat);
 	float4 dr = tex2D(_Tex_DiffuseR, IN.uv_Tex_DiffuseR);
 	float4 dg = tex2D(_Tex_DiffuseG, IN.uv_Tex_DiffuseG);
 	float4 db = tex2D(_Tex_DiffuseB, IN.uv_Tex_DiffuseB);
 	float4 da = tex2D(_Tex_DiffuseA, IN.uv_Tex_DiffuseA);
 	dr.w *= splat.x;
 	dg.w *= splat.y;
 	db.w *= splat.z;
 	da.w *= splat.w;
 	float sum = dr.w + dg.w + db.w + da.w;
 	if (sum > 0.0)
 	{
 		dr.w /= sum;
 		dg.w /= sum;
 		db.w /= sum;
 		da.w /= sum;
 	}
 	else
 	{
 		dr.w = 1.0;
 	}
 	half soft = saturate(_BlendSoft);
 	float r = lerp(dr.w > da.w ? 1.0 : 0.0, dr.w, soft);
 	float g = lerp(dg.w > da.w ? 1.0 : 0.0, dg.w, soft);
 	float b = lerp(db.w > da.w ? 1.0 : 0.0, db.w, soft);
 	float a = lerp(da.w > db.w ? 1.0 : 0.0, da.w, soft);
 	o.Albedo = saturate((dr.xyz * r + dg.xyz * g + db.xyz * b + da.xyz * a) * _Color.xyz);
 	float3 nr = tex2D(_Tex_NormalR, IN.uv_Tex_DiffuseR).xyz * 2.0 - 1.0;
 	float3 ng = tex2D(_Tex_NormalG, IN.uv_Tex_DiffuseG).xyz * 2.0 - 1.0;
 	float3 nb = tex2D(_Tex_NormalB, IN.uv_Tex_DiffuseB).xyz * 2.0 - 1.0;
 	float3 na = tex2D(_Tex_NormalA, IN.uv_Tex_DiffuseA).xyz * 2.0 - 1.0;
 	o.Normal = clamp((nr * r + ng * g + nb * b + na * a) * _NScale, -1.0, 1.0);
 	o.Specular = _Shininess;
 	o.Gloss = _Spec;
 	o.Alpha = 0.0;
 }
 ENDCG
}
Fallback "Legacy Shaders/Diffuse"
}
