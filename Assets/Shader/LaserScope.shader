// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "AngryBots/FX/LaserScope" {
Properties {
 _MainTex ("MainTex", 2D) = "white" {}
 _NoiseTex ("NoiseTex", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" "Reflection"="LaserScope" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Transparent" "Reflection"="LaserScope" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha One
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  half4 _MainTex_ST;
  sampler2D _NoiseTex;
  half4 _NoiseTex_ST;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	half4 xlv_TEXCOORD0 : TEXCOORD0;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	half4 tmpvar_1;
  	float2 tmpvar_2;
  	tmpvar_2 = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  	tmpvar_1.xy = tmpvar_2;
  	float2 tmpvar_3;
  	tmpvar_3 = ((v.texcoord.xy * _NoiseTex_ST.xy) + _NoiseTex_ST.zw);
  	tmpvar_1.zw = tmpvar_3;
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = tmpvar_1;
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	tmpvar_1 = (tex2D (_MainTex, i.xlv_TEXCOORD0.xy) * tex2D (_NoiseTex, i.xlv_TEXCOORD0.zw));
  	bolOut = tmpvar_1;
  	return bolOut;
  }
  ENDCG
 }
}
Fallback Off
}