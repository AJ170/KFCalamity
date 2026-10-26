Shader "FX/Water (simple)" {
Properties {
 _horizonColor ("Horizon color", Color) = (0.172,0.463,0.435,0)
 _WaveScale ("Wave scale", Range(0.02,0.15)) = 0.07
 _ColorControl ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
 _ColorControlCube ("Reflective color cube (RGB) fresnel (A) ", CUBE) = "" { TexGen CubeReflect }
 _BumpMap ("Waves Normalmap ", 2D) = "" {}
 WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
 _MainTex ("Fallback texture", 2D) = "" {}
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