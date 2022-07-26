Shader "Custom/MotionEffect" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}                       // основная текстура
        _MoveDir("Move Direction", Vector) = (0,0,0,0)             // xyz: направление движения, w, интенсивность движения
        _MotionTintColor("Motion Tint Color", Color) = (1,1,1,1)   // мобильная раскраска
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
        sampler2D _MainTex;         // основная текстура
        float4 _MainTex_ST;         // tiling and offset
        float4 _MoveDir;            // Вектор движения, xyz: единичный вектор, направление, w: модуль
        fixed4 _MotionTintColor;    // Цвет хвоста индикатора движения
        float _InvMaxMotion;        // Обратное максимальное расстояние смещения, используемое для управления второй половиной замыкающего альфа
        float _alpha;               // общая альфа
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
            // Вершина сначала преобразуется в мировую систему координат
            float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
            // Нормаль конвертируется в мировые координаты
            float3 worldNormal = UnityObjectToWorldNormal(v.normal);
            // Находим вершину, которая находится «назад» к движению (направление хвоста), и находим ее согласно нормали вершины
            // Отрицательное число не требуется, поэтому минимальное значение ограничения равно 0 (это минимальное значение, ограниченное до 0)
            // MDDotN хранит изотропный коэффициент 0,0 ~ 1,0
            // Находим коэффициент обратной поверхности при MDDotN, противоположной направлению движения
            fixed MDDotN = max(0, dot(_MoveDir.xyz, worldNormal));
            // perlinoise возвращает -1 ~ 1, смещение на 0 ~ 1, как псевдослучайный, но очень плавный эффект
            // offsetFactor - коэффициент смещения вершины
            half offsetFactor = (perlin_noise(v.vertex.xy) * 0.5 + 0.5) * MDDotN;
            // Сила смещения = коэффициент смещения вершины * модуль вектора направления движения
            // Сохраняем силу смещения в normal.z, чтобы улучшить ограниченное использование регистров
            o.normal.z = (offsetFactor * _MoveDir.w);
            // движение смещено в соответствии с направлением модуля вектора _MoveDir.xyz * _MoveDir.w
            worldPos.xyz += _MoveDir.xyz * o.normal.z;
            // Давайте поговорим о мировых координатах с координатами клипа
            o.vertex = mul(unity_MatrixVP, worldPos);
            // uv размещается по смещению
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            // вершина нормальная
            o.normal.xyz = worldNormal;
            return o;
        }
        fixed4 frag(v2f i) : SV_Target{
            fixed4 col = tex2D(_MainTex, i.uv);
            #if MUTE_LIGHT // временно экранируем свет
            col.rgb *= _LightColor0.rgb;
            col.rgb *= dot(getLDir(), i.normal) * 0.5 + 0.5;    // half-lambert
            #endif
            return col;
        }
            fixed4 frag_motion(v2f_motion i) : SV_Target{
                fixed4 col = tex2D(_MainTex, i.uv);
                #if MUTE_LIGHT // временно экранируем свет
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
                // продано: объект
                Pass{
                    Name "sold"
                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    ENDCG
            }
                // испытание движения: отслеживание движения
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