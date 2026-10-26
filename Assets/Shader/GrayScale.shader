// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Unlit/GreyScale" {
Properties {
 _MainTex ("Texture", 2D) = "white" {}
}
SubShader { 
 Pass {
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	float2 xlv_TEXCOORD0 : TEXCOORD0;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	half4 texcol_1;
  	fixed4 tmpvar_2;
  	tmpvar_2 = tex2D (_MainTex, i.xlv_TEXCOORD0);
  	texcol_1 = tmpvar_2;
  	texcol_1.xyz = ((float3)(dot (texcol_1.xyz, float3(0.5, 0.05, 0.1))));
  	bolOut = texcol_1;
  	return bolOut;
  }
  ENDCG
 }
}
Fallback "VertexLit"
}