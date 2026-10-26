// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Hidden/BlurEffectConeTap" {
Properties {
 _MainTex ("", any) = "" {}
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  SetTexture [_MainTex] { ConstantColor (0,0,0,0.25) combine texture * constant alpha }
  SetTexture [_MainTex] { ConstantColor (0,0,0,0.25) combine texture * constant + previous }
  SetTexture [_MainTex] { ConstantColor (0,0,0,0.25) combine texture * constant + previous }
  SetTexture [_MainTex] { ConstantColor (0,0,0,0.25) combine texture * constant + previous }
 }
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  half4 _BlurOffsets;
  sampler2D _MainTex;
  half4 _MainTex_TexelSize;

  struct kf_appdata
  {
  	float4 vertex : POSITION;
  	float4 texcoord : TEXCOORD0;
  };

  struct kf_v2f
  {
  	float4 pos : SV_POSITION;
  	half2 xlv_TEXCOORD0 : TEXCOORD0;
  	half2 xlv_TEXCOORD1 : TEXCOORD1;
  	half2 xlv_TEXCOORD1_1 : TEXCOORD2;
  	half2 xlv_TEXCOORD1_2 : TEXCOORD3;
  	half2 xlv_TEXCOORD1_3 : TEXCOORD4;
  };

  kf_v2f vert (kf_appdata v)
  {
  	kf_v2f o;
  	UNITY_INITIALIZE_OUTPUT(kf_v2f, o);
  	half2 tmpvar_1;
  	tmpvar_1 = (v.texcoord.xy - (_BlurOffsets.xy * _MainTex_TexelSize.xy));
  	half2 cse_2;
  	cse_2 = (_MainTex_TexelSize.xy * _BlurOffsets.xy);
  	o.pos = (UnityObjectToClipPos(v.vertex));
  	o.xlv_TEXCOORD0 = tmpvar_1;
  	o.xlv_TEXCOORD1 = (tmpvar_1 + cse_2);
  	o.xlv_TEXCOORD1_1 = (tmpvar_1 - cse_2);
  	o.xlv_TEXCOORD1_2 = (tmpvar_1 + (cse_2 * float2(1.0, -1.0)));
  	o.xlv_TEXCOORD1_3 = (tmpvar_1 - (cse_2 * float2(1.0, -1.0)));
  	return o;
  }

  fixed4 frag (kf_v2f i) : SV_Target
  {
  	float4 bolOut;
  	half4 color_1;
  	fixed4 tmpvar_2;
  	tmpvar_2 = tex2D (_MainTex, i.xlv_TEXCOORD1);
  	color_1 = tmpvar_2;
  	fixed4 tmpvar_3;
  	tmpvar_3 = tex2D (_MainTex, i.xlv_TEXCOORD1_1);
  	fixed4 tmpvar_4;
  	tmpvar_4 = tex2D (_MainTex, i.xlv_TEXCOORD1_2);
  	fixed4 tmpvar_5;
  	tmpvar_5 = tex2D (_MainTex, i.xlv_TEXCOORD1_3);
  	half4 tmpvar_6;
  	tmpvar_6 = (((color_1 + tmpvar_3) + tmpvar_4) + tmpvar_5);
  	color_1 = tmpvar_6;
  	bolOut = (tmpvar_6 * 0.25);
  	return bolOut;
  }
  ENDCG
 }
}
Fallback Off
}