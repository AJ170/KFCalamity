// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Unlit/Depth Cutout" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Background" "IGNOREPROJECTOR"="true" }
 Pass {
  Tags { "QUEUE"="Background" "IGNOREPROJECTOR"="true" }
  ZTest Less
  Cull Off
  AlphaTest Greater 0.99
  ColorMask 0
  ColorMaterial AmbientAndDiffuse
  SetTexture [_MainTex] { combine texture * primary }
 }
}
}