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
 _ReflectiveColorCube ("Reflective color cube (RGB) fresnel (A)", CUBE) = "" { TexGen CubeReflect }
 _HorizonColor ("Simple water horizon color", Color) = (0.172,0.463,0.435,1)
 _MainTex ("Fallback texture", 2D) = "" {}
 _ReflectionTex ("Internal Reflection", 2D) = "" {}
 _RefractionTex ("Internal Refraction", 2D) = "" {}
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