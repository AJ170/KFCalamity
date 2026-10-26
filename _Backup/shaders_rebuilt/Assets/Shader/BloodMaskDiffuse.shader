Shader "Custom/BloodMaskDiffuse" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _BloodTexture ("Blood Texture", 2D) = "white" {}
 _HitColor ("Hit Color", Color) = (0,0,0,1)
 _BloodColor ("Blood Color", Color) = (0.5,0,0,1)
 _BloodAmount ("Blood Amount", Range(0,0.7)) = 0.7
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