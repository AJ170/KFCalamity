Shader "Custom/splatmap" {
Properties {
 _Color ("Main Color", Color) = (0.8,0.8,0.8,0)
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
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}