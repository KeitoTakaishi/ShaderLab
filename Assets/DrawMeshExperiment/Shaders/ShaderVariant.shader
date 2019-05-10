//http://light11.hatenadiary.com/entry/2019/01/12/232533
Shader "Custom/ShaderVariant"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        [KeywordEnum(RED, GREEN, BLUE)] _("Color", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        Pass{
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ __RED __GREEN __BLUE
            #include "UnityCG.cginc"

            float4 vert(float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }

            fixed4 frag() : SV_Target
            {
                // それぞれのキーワードが定義されているかどうかで処理を分ける
            #ifdef __RED
                return fixed4(1, 0, 0, 1);
            #elif __GREEN
                return fixed4(0, 1, 0, 1);
            #elif __BLUE
                return fixed4(0, 0, 1, 1);
            #else
                return fixed4(1, 1, 1, 1);
            #endif
            }
            ENDCG
        }
    }
}
