Shader "Custom/MotionEffect" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}                       // �������� ��������
        _MoveDir("Move Direction", Vector) = (0,0,0,0)             // xyz: ����������� ��������, w, ������������� ��������
        _MotionTintColor("Motion Tint Color", Color) = (1,1,1,1)   // ��������� ���������
    }
        CGINCLUDE
#pragma multi_compile_fog
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"
#pragma multi_compile_fwdbase_fullshadows
#pragma fragmentoption ARB_precision_hint_fastest
            struct appdata {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
        };
        struct v2f {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : TEXCOORD1;
        };
        struct v2f_motion {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 normal : TEXCOORD1;
        };
        sampler2D _MainTex;         // �������� ��������
        float4 _MainTex_ST;         // tiling and offset
        float4 _MoveDir;            // ������ ��������, xyz: ��������� ������, �����������, w: ������
        fixed4 _MotionTintColor;    // ���� ������ ���������� ��������
        float _InvMaxMotion;        // �������� ������������ ���������� ��������, ������������ ��� ���������� ������ ��������� ����������� �����
        float _alpha;               // ����� �����
        float2 hash22(float2 p) {
            p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
            return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
        }
        float2 hash21(float2 p) {
            float h = dot(p, float2(127.1, 311.7));
            return -1.0 + 2.0 * frac(sin(h) * 43758.5453123);
        }
        //perlin
        float perlin_noise(float2 p) {
            float2 pi = floor(p);
            float2 pf = p - pi;
            float2 w = pf * pf * (3.0 - 2.0 * pf);
            return lerp(lerp(dot(hash22(pi + float2(0.0, 0.0)), pf - float2(0.0, 0.0)),
                dot(hash22(pi + float2(1.0, 0.0)), pf - float2(1.0, 0.0)), w.x),
                lerp(dot(hash22(pi + float2(0.0, 1.0)), pf - float2(0.0, 1.0)),
                    dot(hash22(pi + float2(1.0, 1.0)), pf - float2(1.0, 1.0)), w.x), w.y);
        }
        half3 getLDir() {
            return normalize(_WorldSpaceLightPos0.xyz);
        }
        v2f vert(appdata v) {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);          // clip pos
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);               // uv tiling and offset
            o.normal = UnityObjectToWorldNormal(v.normal);      // normal
            return o;
        }
        v2f_motion vert_motion(appdata v) {
            v2f_motion o;
            // ������� ������� ������������� � ������� ������� ���������
            float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
            // ������� �������������� � ������� ����������
            float3 worldNormal = UnityObjectToWorldNormal(v.normal);
            // ������� �������, ������� ��������� ������ � �������� (����������� ������), � ������� �� �������� ������� �������
            // ������������� ����� �� ���������, ������� ����������� �������� ����������� ����� 0 (��� ����������� ��������, ������������ �� 0)
            // MDDotN ������ ���������� ����������� 0,0 ~ 1,0
            // ������� ����������� �������� ����������� ��� MDDotN, ��������������� ����������� ��������
            fixed MDDotN = max(0, dot(_MoveDir.xyz, worldNormal));
            // perlinoise ���������� -1 ~ 1, �������� �� 0 ~ 1, ��� ���������������, �� ����� ������� ������
            // offsetFactor - ����������� �������� �������
            half offsetFactor = (perlin_noise(v.vertex.xy) * 0.5 + 0.5) * MDDotN;
            // ���� �������� = ����������� �������� ������� * ������ ������� ����������� ��������
            // ��������� ���� �������� � normal.z, ����� �������� ������������ ������������� ���������
            o.normal.z = (offsetFactor * _MoveDir.w);
            // �������� ������� � ������������ � ������������ ������ ������� _MoveDir.xyz * _MoveDir.w
            worldPos.xyz += _MoveDir.xyz * o.normal.z;
            // ������� ��������� � ������� ����������� � ������������ �����
            o.vertex = mul(unity_MatrixVP, worldPos);
            // uv ����������� �� ��������
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            // ������� ����������
            o.normal.xyz = worldNormal;
            return o;
        }
        fixed4 frag(v2f i) : SV_Target{
            fixed4 col = tex2D(_MainTex, i.uv);
            #if MUTE_LIGHT // �������� ���������� ����
            col.rgb *= _LightColor0.rgb;
            col.rgb *= dot(getLDir(), i.normal) * 0.5 + 0.5;    // half-lambert
            #endif
            return col;
        }
            fixed4 frag_motion(v2f_motion i) : SV_Target{
                fixed4 col = tex2D(_MainTex, i.uv);
                #if MUTE_LIGHT // �������� ���������� ����
        // col.rgb *= _LightColor0.rgb;
        // col.rgb *= dot(getLDir(), i.normal) * 0.5 + 0.5;    // half-lambert
        #endif
        col.rgb += _MotionTintColor.rgb * _alpha;
        col.a = _alpha * saturate(1 - (i.normal.z * _InvMaxMotion));
        return col;
        }
            ENDCG
            SubShader {
            Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
                LOD 100
                // �������: ������
                Pass{
                    Name "sold"
                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    ENDCG
            }
                // ��������� ��������: ������������ ��������
                pass {
                Name "motion trial"
                    // Tags { "Queue"="Transparent" "RenderType"="Transparent" }
                    ZWrite off Cull off
                    Blend SrcAlpha OneMinusSrcAlpha
                    CGPROGRAM
#pragma vertex vert_motion
#pragma fragment frag_motion
                    ENDCG
            }
        }
        Fallback "Diffuse"
}