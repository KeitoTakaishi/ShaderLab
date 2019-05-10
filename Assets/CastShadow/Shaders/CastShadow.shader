Shader "Custom/CastShadow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Pass{
            Name  "ShadowCaster"
            Tags  {"LightMode" = "ShadowCaster"}
            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma target 3.0

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster
            #include "UnityCG.cginc"
            struct VertexInput {
                float4 vertex : POSITION;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
            };

            VertexOutput vertShadowCaster(VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 fragShadowCaster(VertexOutput i) : SV_TARGET{
                return 0;
            }
            ENDCG
        }
    }
}
