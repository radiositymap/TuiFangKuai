Shader "Unlit/Fade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HighlightCutOff("Highlight Cut-Off", Float) = 0.6
        _GradientMod ("Gradient Modulator", Float) = 4
    }
    SubShader
    {
        Tags { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 colour : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 colour : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Albedo;
            float _HighlightCutOff;
            float _GradientMod;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.colour = v.colour;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);

                // Highlight edges
                float distFromCtr = pow(abs(i.uv.x - 0.5) * 2, _GradientMod);
                if (distFromCtr > _HighlightCutOff)
                    distFromCtr = 1;
                float distFromCtr2 = pow(abs(i.uv.y - 0.5) * 2, _GradientMod);
                if (distFromCtr2 > _HighlightCutOff)
                    distFromCtr2 = 1;
                //fixed4 col = float4(i.colour.rgb, 1-distFromCtr);
                fixed4 col1 = (1-distFromCtr) * i.colour + distFromCtr * 3 * i.colour;
                fixed4 col2 = (1-distFromCtr2) * i.colour + distFromCtr2 * 3 * i.colour;
                fixed4 col = 0.6 * (col1 + col2);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
