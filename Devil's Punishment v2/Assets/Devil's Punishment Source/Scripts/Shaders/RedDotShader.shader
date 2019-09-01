// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Cg/RedDotShader"
{
    // Credit to mouurusai: https://forum.unity.com/threads/red-dot-sight.146823/#post-1869299
    Properties
    {
            _MainTex ("Base (RGB)", 2D)="white"{}
            _ReticleSize("ReticleSize",float)=2.0
    }
    
    SubShader
    {
            Tags {"Queue"="Transparent""IgnoreProjector"="True""RenderType"="Transparent"}
            Blend SrcAlpha One
        
        
            Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                
                #include "UnityCG.cginc"

                struct appdata_t
                {
                            float4 vertex : POSITION;
                };

                struct v2f
                {
                            float4 vertex : SV_POSITION;
                            half2 texcoord : TEXCOORD0;
                };

                sampler2D _MainTex;
                float _ReticleSize;
                
                    v2f vert (appdata_t v)
                {
                            v2f o;
                            o.vertex= UnityObjectToClipPos(v.vertex);
                            o.texcoord= v.vertex.xy*_ReticleSize-mul((float3x3)unity_WorldToObject, _WorldSpaceCameraPos-mul(unity_ObjectToWorld, float4(0,0,0,1))).xy*_ReticleSize+float2(0.5,0.5);
                        return o;
                }
                
                    fixed4 frag (v2f i): COLOR
                {
                            fixed4 col = tex2D(_MainTex, i.texcoord);
                        return col;
                }
            ENDCG
        }
    }
}
