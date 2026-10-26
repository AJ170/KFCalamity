// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Unlit/Dynamic Font (SoftClip)" {
Properties {
 _MainTex ("Alpha (A)", 2D) = "white" {}
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  Offset -1, -1
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  float2 _ClipSharpness;
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
  	float2 tmpvar_2;
  	tmpvar_2 = ((float2(1.0, 1.0) - abs(i.xlv_TEXCOORD1)) * _ClipSharpness);
  	col_1.xyz = i.xlv_COLOR.xyz;
  	fixed4 tmpvar_3;
  	tmpvar_3 = tex2D (_MainTex, i.xlv_TEXCOORD0);
  	col_1.w = (i.xlv_COLOR.w * tmpvar_3.w);
  	float tmpvar_4;
  	tmpvar_4 = (col_1.w * clamp (min (tmpvar_2.x, tmpvar_2.y), 0.0, 1.0));
  	col_1.w = tmpvar_4;
  	bolOut = col_1;
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
  Blend SrcAlpha OneMinusSrcAlpha
  AlphaTest Greater 0.01
  ColorMask RGB
  ColorMaterial AmbientAndDiffuse
  SetTexture [_MainTex] { combine texture * primary }
 }
}
}