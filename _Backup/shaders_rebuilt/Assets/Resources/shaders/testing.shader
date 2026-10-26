Shader "Test: Player" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Overlay+1" }
 Pass {
  Tags { "QUEUE"="Overlay+1" }
  Color [_Color]
  ZTest Greater
  ZWrite Off
 }
 Pass {
  Tags { "QUEUE"="Overlay+1" }
  ZTest Less
  SetTexture [_MainTex] { combine texture }
 }
}
Fallback "Mobile/Diffuse"
}