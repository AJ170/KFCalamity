// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Unlit/Transparent Colored (Packed) (AlphaClip)" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  Offset -1, -1
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  half4 _MainTex_ST;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	fixed4 color : COLOR;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	half4 xlv_COLOR : TEXCOORD0;
  	float2 xlv_TEXCOORD0 : TEXCOORD1;
  	float2 xlv_TEXCOORD1 : TEXCOORD2;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_COLOR = v.color;
  	o.xlv_TEXCOORD0 = v.texcoord.xy;
  	o.xlv_TEXCOORD1 = ((v.vertex.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	half4 col_1;
  	half4 mask_2;
  	fixed4 tmpvar_3;
  	tmpvar_3 = tex2D (_MainTex, i.xlv_TEXCOORD0);
  	mask_2 = tmpvar_3;
  	half4 tmpvar_4;
  	tmpvar_4 = clamp (ceil((i.xlv_COLOR - 0.5)), 0.0, 1.0);
  	col_1 = clamp (((
  	(tmpvar_4 * 0.51)
  	- i.xlv_COLOR) / -0.49), 0.0, 1.0);
  	float2 tmpvar_5;
  	tmpvar_5 = abs(i.xlv_TEXCOORD1);
  	float tmpvar_6;
  	tmpvar_6 = (1.0 - max (tmpvar_5.x, tmpvar_5.y));
  	if ((tmpvar_6 < 0.0)) {
  	col_1.w = 0.0;
  	};
  	half4 tmpvar_7;
  	tmpvar_7 = (mask_2 * tmpvar_4);
  	mask_2 = tmpvar_7;
  	col_1.w = (col_1.w * ((
  	(tmpvar_7.x + tmpvar_7.y)
  	+ tmpvar_7.z) + tmpvar_7.w));
  	bolOut = col_1;
  	return bolOut;
  }
  ENDCG
 }
}
Fallback Off
}