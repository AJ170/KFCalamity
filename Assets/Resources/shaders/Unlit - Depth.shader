// Rebuilt for Unity 2018 / PS Vita from the game's GLES programs (_Tools/gles2cg.py).
Shader "Unlit/Depth" {
SubShader { 
 LOD 100
 Tags { "QUEUE"="Geometry+1" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Geometry+1" "RenderType"="Opaque" }
  ColorMask 0
 }
}
}