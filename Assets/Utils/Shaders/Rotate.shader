Shader "Unlit/Rotate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            //https://en.wikipedia.org/wiki/Rodrigues%27_rotation_formula
             
            float3 rotate3D(float3 p, float3 rotation) 
            {
                float3 n = normalize(rotation);
                float theta = length(rotation);
                float s = sin(theta);
                float c = cos(theta);
                float r = 1.0 - c;

                //Row Major!
                float3x3 m = float3x3(
                    c + n.x*n.x*r,
                    n.x*n.y*r + n.z*s,
                    n.z*n.x*r - n.y*s,
                    
                    n.x*n.y*r - n.z*s,
                    c + n.y*n.y*r,
                    n.y*n.z*r + n.x*s,

                    n.z*n.x*r + n.y*s,
                    n.y*n.z*r - n.x*s,
                    c + n.z*n.z*r
                );
                return mul(m, p);
            }

            v2f vert (appdata v)
            {
                v2f o;
                float3 rot = float3(_Time.y, 0.0, 0.0);
                //float3 localPos = rotate3D(v.vertex.xyz, rot);
                float3 localPos = v.vertex.xyz * float3(1.0, 2.0, 1.0);
                localPos = rotate3D(v.vertex.xyz, rot);

                o.vertex = UnityObjectToClipPos(float4(float3(localPos), 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
