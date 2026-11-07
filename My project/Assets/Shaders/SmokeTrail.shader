Shader "Aviones/SmokeTrail"
{
    Properties
    {
        _Color ("Color humo", Color) = (0.85, 0.85, 0.85, 1)
        _NoiseScale ("Escala del ruido", Float) = 3
        _ScrollSpeed ("Velocidad del ruido", Float) = 0.6
        _NoiseAmount ("Cantidad de ruido", Range(0, 1)) = 0.7
        _Softness ("Suavidad de bordes", Range(0.2, 4)) = 1.5
        _Density ("Densidad", Range(0, 2)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _NoiseScale, _ScrollSpeed, _NoiseAmount, _Softness, _Density;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float hash(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float vnoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // v (uv.y) es el ancho de la estela: opaco en el centro, transparente en los bordes
                float edge = 1.0 - abs(i.uv.y * 2.0 - 1.0);
                edge = pow(saturate(edge), _Softness);

                // Dos capas de ruido que se desplazan para dar aspecto de humo turbulento
                float2 p = float2(i.uv.x * _NoiseScale * 4.0 - _Time.y * _ScrollSpeed, i.uv.y * _NoiseScale);
                float n = vnoise(p) * 0.65 + vnoise(p * 2.3 + 7.1) * 0.35;

                float alpha = edge * lerp(1.0, n * 1.6, _NoiseAmount) * _Density;
                alpha *= i.color.a;   // el fade a lo largo de la estela lo da el Gradient del TrailRenderer

                fixed3 rgb = _Color.rgb * i.color.rgb;
                return fixed4(rgb, saturate(alpha));
            }
            ENDCG
        }
    }
}