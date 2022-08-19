Shader "Unlit/Portal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Albedo ("Albedo", Color) = (1,1,1,1)
        _Highlight ("Highlight", Color) = (1,1,1,1)
        _HighlightCutOff("Highlight Cut-Off", Float) = 0.6
        _GradientMod ("Gradient Modulator", Float) = 4
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
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Albedo;
            float4 _Highlight;
            float _HighlightCutOff;
            float _GradientMod;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(unity_ObjectToWorld, v.normal).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float random (float2 uv)
            {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Highlight edges
                float distFromCtr = pow(abs(i.uv.x - 0.5) * 2, _GradientMod);
                if (distFromCtr > _HighlightCutOff)
                    distFromCtr = 1;
                float distFromCtr2 = pow(abs(i.uv.y - 0.5) * 2, _GradientMod);
                if (distFromCtr2 > _HighlightCutOff)
                    distFromCtr2 = 1;
                fixed4 col1 = (1-distFromCtr) * _Albedo + distFromCtr * _Highlight;
                fixed4 col2 = (1-distFromCtr2) * _Albedo + distFromCtr2 * _Highlight;
                fixed4 col = 0.8 * (col1 + col2);

                // add some spots
                float2 spots[15];
                for (int j=0; j<15; j++) {
                    spots[j] = 0.7 * float2(
                        random(float2(0.01*j, i.normal.x*j)),
                        random(float2(0.02*j, 0.05*j))) + float2(0.2, 0.2);
                }

                // vary blinking by face using normal vals
                for (int j=0; j<15; j++) {
                    float randVal = random(spots[j]);
                    float spotSize = 0.02 * randVal;
                    float blinkRate = randVal + i.normal.z;
                    float blinkOffSet = randVal + i.normal.y + i.normal.z;
                    float sinTime = sin(blinkOffSet + blinkRate * _Time.y);

                    // use sine squared for 0-1 range
                    if (distance(spots[j], i.uv) < spotSize)
                        col = lerp(col, fixed4(1,1,1,1), sinTime * sinTime);
                }

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
