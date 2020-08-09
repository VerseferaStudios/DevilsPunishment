 
Shader "Custom/PBR_Cutout_UV2AO" {
 
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _SecTex ("Metallic(R) Smoothness(G)", 2D) = "white" {}
    _AOTex ("AO(R)", 2D) = "white" {}
    _Normal ("Normal (RGB)", 2D) = "white" {}
}
 
	CGINCLUDE
    #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG
 
SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 300
   
CGPROGRAM
#pragma target 3.0
#include "UnityPBSLighting.cginc"
#pragma surface surf Standard
 
sampler2D _MainTex;
sampler2D _Normal;
sampler2D _SecTex;
sampler2D _AOTex;
 
struct Input {
    float2 uv_MainTex;
    float2 uv2_AOTex;
};
 
void surf (Input IN, inout SurfaceOutputStandard o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    fixed4 s = tex2D(_SecTex, IN.uv_MainTex);
    fixed4 se = tex2D(_AOTex, IN.uv2_AOTex);
    
    o.Albedo = c.rgb * se.r;
    o.Normal = UnpackNormal( tex2D(_Normal, IN.uv_MainTex) );
    
    o.Metallic = s.r;
    o.Smoothness = s.g;
    
}
ENDCG
}
 
Fallback "VertexLit"
}