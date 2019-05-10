// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

 Shader "InstancingPlayGround/InstancedShader" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader {


        Pass {

            Tags {"LightMode"="ForwardBase"}
            Cull Front
            ZWrite On
            ZTest LEqual

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma target 4.5

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;

            struct InsObj{
                float3 pos;
                float3 vel;
                float3 acc;
                float3 rot;
                float3 angVel;
                float3 scale;
                float4 col;
            };


        #if SHADER_TARGET >= 45
            StructuredBuffer<InsObj> InsObjs;
        #endif

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv_MainTex : TEXCOORD0;
                float3 ambient : TEXCOORD1;
                float3 diffuse : TEXCOORD2;
                float3 color : TEXCOORD3;
                SHADOW_COORDS(4)
            };

            // rotate関数 [参考](http://tips.hecomi.com/entry/2016/05/08/160626)
            float3 rotate(float3 p, float3 rotation){
                float3 a = normalize(rotation);
                float angle = length(rotation);
                float s = sin(angle);
                float c = cos(angle);
                float r = 1.0 - c;
                float3x3 m = float3x3(
                    a.x * a.x * r + c,
                    a.y * a.x * r + a.z * s,
                    a.z * a.x * r - a.y * s,
                    a.x * a.y * r - a.z * s,
                    a.y * a.y * r + c,
                    a.z * a.y * r + a.x * s,
                    a.x * a.z * r + a.y * s,
                    a.y * a.z * r - a.x * s,
                    a.z * a.z * r + c
                );
                return mul(m, p);
            }

            v2f vert (appdata_full v, uint instanceID : SV_InstanceID)
            {
                InsObj obj;
            #if SHADER_TARGET >= 45
                obj = InsObjs[instanceID];
            #endif


                
                float3 rot = obj.rot;
                float4 temp = mul(unity_ObjectToWorld, v.vertex);
                float t = _Time.y * 0.4;
                float offSet = .5;

                float3 localPosition = 
                    rotate(v.vertex.xyz * (obj.scale.xyz * 
                        float3(sin(t + temp.z*offSet), sin(t + temp.z*offSet), sin(t + temp.z*offSet))), rot);
                float3 worldPosition = obj.pos.xyz + localPosition;

                //float3 normal = rotate(v.normal,rot);
                //half3 worldNormal = UnityObjectToWorldNormal(normal);
                half3 worldNormal = rotate(v.normal, rot);

                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                float3 ambient = ShadeSH9(float4(worldNormal, 1.0f));
                float3 diffuse = (nl * _LightColor0.rgb);

                v2f o;
                o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
                o.uv_MainTex = v.texcoord;
                o.ambient = ambient;
                o.diffuse = diffuse;
                //o.color = v.color;
                // ほぼほぼベース色でちょっとだけobj.col乗せる
                o.color = v.color*0.9 + (obj.col*0.0);

                TRANSFER_SHADOW(o)
                    return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed shadow = SHADOW_ATTENUATION(i);
                fixed4 albedo = tex2D(_MainTex, i.uv_MainTex);
                float3 lighting = i.diffuse * shadow + i.ambient;
                fixed4 output = fixed4(albedo.rgb * i.color * lighting, albedo.w);
                UNITY_APPLY_FOG(i.fogCoord, output);
                return output;
            }

            ENDCG
        }
    }
}