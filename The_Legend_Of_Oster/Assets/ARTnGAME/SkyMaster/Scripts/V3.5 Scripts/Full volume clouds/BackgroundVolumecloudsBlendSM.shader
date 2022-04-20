Shader "SkyMaster/BackgroundVolumecloudsBlendSM" {

     Properties {
         _MainTex ("Texture to blend", 2D) = "black" {}
     }
     SubShader {
         Tags { "Queue" = "Transparent" }
         Pass {
             Blend SrcAlpha OneMinusSrcAlpha
             SetTexture [_MainTex] { combine texture }
         }
     }
 }