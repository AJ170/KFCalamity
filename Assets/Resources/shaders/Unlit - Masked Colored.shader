// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Unlit/Masked Colored" {
Properties {
 _MainTex ("Base (RGB) Mask (A)", 2D) = "white" {}
 _Color ("Tint Color", Color) = (1,1,1,1)
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  ColorMask RGB
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  fixed4 _Color;
  sampler2D _MainTex;
  float4 _MainTex_ST;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	fixed4 color : COLOR;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	fixed4 xlv_COLOR : TEXCOORD0;
  	float2 xlv_TEXCOORD0 : TEXCOORD1;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_COLOR = v.color;
  	o.xlv_TEXCOORD0 = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	fixed4 tmpvar_1;
  	half4 col_2;
  	fixed4 tmpvar_3;
  	tmpvar_3 = (tex2D (_MainTex, i.xlv_TEXCOORD0) * i.xlv_COLOR);
  	col_2 = tmpvar_3;
  	half4 tmpvar_4;
  	tmpvar_4.xyz = lerp (col_2.xyz, (col_2.xyz * _Color.xyz), col_2.www);
  	tmpvar_4.w = col_2.w;
  	tmpvar_1 = tmpvar_4;
  	bolOut = tmpvar_1;
  	return bolOut;
  }
  ENDCG
 }
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  AlphaTest Greater 0.01
  ColorMask RGB
  ColorMaterial AmbientAndDiffuse
  SetTexture [_MainTex] { combine texture * primary }
  SetTexture [_MainTex] { ConstantColor [_Color] combine previous * constant }
 }
}
}