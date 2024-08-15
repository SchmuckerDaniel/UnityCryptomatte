
// -----------------------------------------------------------------------------
// This source file has been developed within the scope of the
// Technical Director course at Filmakademie Baden-Wuerttemberg.
// http://technicaldirector.de
//
// Written by Daniel Schmucker
// Copyright (c) 2020 Animationsinstitut of Filmakademie Baden-Wuerttemberg
// -----------------------------------------------------------------------------

Shader "Unlit/HDRP_CryptoColor"
{
    Properties
    {
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        // adding cutout transparency to allows alpha cards
		_AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.5
    }
    SubShader
    {
		
		Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

			UNITY_INSTANCING_BUFFER_START(PerInstance)
				UNITY_DEFINE_INSTANCED_PROP(float, _HashFloatCrypto) // per instance Cryptomatte Hashed Float Value
				UNITY_DEFINE_INSTANCED_PROP(float, _AlphaCutoffValue) // per instance Alpha Cutoff Value
			UNITY_INSTANCING_BUFFER_END(PerInstance)

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
			float _AlphaCutoff;

			float GetAlpha(v2f i) {
				return tex2D(_MainTex, i.uv.xy).a;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float alpha = GetAlpha(i);
				clip(alpha - _AlphaCutoff);
                fixed4 col = tex2D(_MainTex, i.uv);

                /*
                *    CRYPTOMATTE INSTANCE VALUES
                */
                float crypto_hash = UNITY_ACCESS_INSTANCED_PROP(PerInstance, _HashFloatCrypto);
                float alpha_cutoff = UNITY_ACCESS_INSTANCED_PROP(PerInstance, _AlphaCutoffValue);
                float4 color;
                if (alpha_cutoff < 1.0) {
                    float coverage = col.a; // coverage entspricht alpha der textur 
                    color = float4(crypto_hash, coverage, 0.0f, 1.0f);
                }
                else {
                    color = float4(crypto_hash, 1.0f, 0.0f, 1.0f);
                }
                /*
                *    CRYPTOMATTE END
                */

			return color;
            }
            ENDCG
        }
    }
}
