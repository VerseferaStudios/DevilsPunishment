 
Shader "AbandonedGarden/PBR_Glass" {
 
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _SecTex ("Metallic(R) Smoothness(G)", 2D) = "white" {}
    _Normal ("Normal (RGB)", 2D) = "white" {}
}
 
	CGINCLUDE
    #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG
 
SubShader {
   Cull Off
CGPROGRAM
#pragma target 3.0
#include "UnityPBSLighting.cginc"
#pragma surface surf Standard alpha:premul
 
sampler2D _MainTex;
sampler2D _Normal;
sampler2D _SecTex;
 
struct Input {
    float2 uv_MainTex;
    float2 uv_Normal;
};
 
void surf (Input IN, inout SurfaceOutputStandard o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    fixed4 s = tex2D(_SecTex, IN.uv_MainTex);
    
    o.Albedo = c.rgb;
    o.Alpha = c.a;
    o.Normal = UnpackNormal( tex2D(_Normal, IN.uv_Normal) );
    
    o.Metallic = s.r;
    o.Smoothness = s.g;
}
ENDCG
}
 
Fallback "VertexLit"
}