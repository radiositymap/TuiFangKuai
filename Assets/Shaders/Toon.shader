Shader "Unlit/Toon"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _NormalDepth("Normal Depth", Float) = 1
        _Albedo ("Albedo", Color) = (1,1,1,1)
        _ShadowBrightness("Shadow Brightness", Float) = 0.4
        //_Palette0("Palette Colour 0", Color) = (1,1,1,1)
        //_Palette1("Palette Colour 1", Color) = (1,1,1,1)
        //_Palette2("Palette Colour 2", Color) = (1,1,1,1)
        //_Palette3("Palette Colour 3", Color) = (1,1,1,1)
        [Toggle] _UseSpecular("Use Specular", Float) = 1
        _SpecularColour("Specular Colour", Color) = (1,1,1,1)
        _Shininess("Specular Coefficient", Float) = 16
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
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : NORMAL;
                float3 worldTangent : TANGENT;
                float4 worldPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                SHADOW_COORDS(3) // shadow data in TEXCOORD2
                float4 pos : SV_POSITION;
            };

            //uniform float4 _LightColor0;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            //float4 _BumpMap_ST;
            float _NormalDepth;
            uniform float4 _Albedo;
            float _ShadowBrightness;
            //uniform float4 _Palette0;
            //uniform float4 _Palette1;
            //uniform float4 _Palette2;
            //uniform float4 _Palette3;
            float _UseSpecular;
            uniform float4 _SpecularColour;
            uniform float _Shininess;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = mul(unity_ObjectToWorld, v.normal).xyz;
                o.worldTangent = mul(unity_ObjectToWorld, v.tangent).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldNormal = normalize(i.worldNormal);
                // use normal map
                half3 normal = UnpackNormal(tex2D(_BumpMap, i.uv));
                float3 worldTangent = normalize(i.worldTangent);
                float3 binormal = cross(i.worldNormal, i.worldTangent);
                worldNormal =
                    (normal.x * i.worldTangent) +
                    (normal.y * binormal) +
                    (normal.z * i.worldNormal * _NormalDepth);
                //return float4(normal.r, normal.g, normal.b, 1.0);
                float3 worldEye =
                    normalize(_WorldSpaceCameraPos - i.worldPos.xyz);
                float3 lightDir = _WorldSpaceLightPos0.w ?
                    _WorldSpaceLightPos0.xyz - i.worldPos.xyz : // point light
                    _WorldSpaceLightPos0.xyz; // directional light - already negative light direction

                // first parameter is incidence vector
                float3 reflection = reflect(-lightDir, worldNormal);

                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

                float intensity = saturate(dot(worldNormal, lightDir));
                if (intensity < _ShadowBrightness)
                    intensity = _ShadowBrightness;
                else
                    intensity = 1;
                float3 diffuse = _LightColor0.rgb * _Albedo.rgb * intensity;
                fixed shadow = SHADOW_ATTENUATION(i);
                if (shadow < _ShadowBrightness)
                    diffuse *= _ShadowBrightness;

                // add light from ambient and light probes
                diffuse.rgb += ShadeSH9(half4(worldNormal, 1));

                float4 col = float4(ambient + diffuse, 1.0);

                if (_UseSpecular > 0) {
                    float specIntensity =
                        pow(saturate(dot(reflection, worldEye)), _Shininess);
                    if (specIntensity > 0.5)
                        col = float4(_LightColor0.rgb*_SpecularColour.rgb, 1.0);
                }
                /*
                // pick closest colour in palette
                float4 palette[] = {_Palette0, _Palette1, _Palette2, _Palette3};
                float nearestDist = distance(col, palette[0]);
                float currDist = nearestDist;
                float4 closestColour = palette[0];
                for (int i=1; i<4; i++) {
                    currDist = distance(col, palette[i]);
                    if (currDist < nearestDist) {
                        nearestDist = currDist;
                        closestColour = palette[i];
                    }
                }
                col = closestColour;
                */

                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
