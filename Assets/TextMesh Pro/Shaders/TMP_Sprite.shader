Shader "TextMeshPro/Sprite"
{
	Properties
	{
<<<<<<< Updated upstream
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		
		_CullMode ("Cull Mode", Float) = 0
		_ColorMask ("Color Mask", Float) = 15
		_ClipRect ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
=======
        _MainTex            ("Sprite Texture", 2D) = "white" {}
		_Color              ("Tint", Color) = (1,1,1,1)

		_StencilComp        ("Stencil Comparison", Float) = 8
		_Stencil            ("Stencil ID", Float) = 0
		_StencilOp          ("Stencil Operation", Float) = 0
		_StencilWriteMask   ("Stencil Write Mask", Float) = 255
		_StencilReadMask    ("Stencil Read Mask", Float) = 255

		_CullMode           ("Cull Mode", Float) = 0
		_ColorMask          ("Color Mask", Float) = 15
		_ClipRect           ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
>>>>>>> Stashed changes

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
<<<<<<< Updated upstream
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
=======
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

>>>>>>> Stashed changes
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
<<<<<<< Updated upstream
			Pass [_StencilOp] 
=======
			Pass [_StencilOp]
>>>>>>> Stashed changes
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull [_CullMode]
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
            Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
<<<<<<< Updated upstream
			
=======

>>>>>>> Stashed changes
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
<<<<<<< Updated upstream
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
			};
			
=======
				float4 vertex			: SV_POSITION;
				fixed4 color			: COLOR;
                float2 texcoord			: TEXCOORD0;
				float4 worldPosition	: TEXCOORD1;
				float4 mask				: TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
			};

>>>>>>> Stashed changes
            sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
            float4 _MainTex_ST;
<<<<<<< Updated upstream
=======
		    float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            int _UIVertexColorAlwaysGammaSpace;
>>>>>>> Stashed changes

            v2f vert(appdata_t v)
			{
				v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
<<<<<<< Updated upstream
                OUT.worldPosition = v.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				
=======
				float4 vPosition = UnityObjectToClipPos(v.vertex);
            	OUT.worldPosition = v.vertex;
				OUT.vertex = vPosition;

            	float2 pixelSize = vPosition.w;
                pixelSize /= abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

				float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                if (_UIVertexColorAlwaysGammaSpace && !IsGammaSpace())
                {
                    v.color.rgb = UIGammaToLinear(v.color.rgb);
                }
>>>>>>> Stashed changes
                OUT.color = v.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
<<<<<<< Updated upstream
				
                #ifdef UNITY_UI_CLIP_RECT
					color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
=======

                #if UNITY_UI_CLIP_RECT
				half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
				color *= m.x * m.y;
>>>>>>> Stashed changes
				#endif

				#ifdef UNITY_UI_ALPHACLIP
					clip (color.a - 0.001);
				#endif

				return color;
			}
<<<<<<< Updated upstream
		ENDCG
=======
		    ENDCG
>>>>>>> Stashed changes
		}
	}
}
