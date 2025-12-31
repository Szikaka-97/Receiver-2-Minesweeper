Shader "Minesweeper/Minesweeper Highlight Shader"
{
	Properties
	{
		_Color ("Color", color) = (1, 1, 0, 1)
		_CrossSize ("Cross Size", float) = 1
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

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			fixed4 _Color;
			half _CrossSize;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target {
				const float border_width = 1.0 / 16.0;

				bool draw = false;

				draw = draw || (i.uv.x < border_width || i.uv.x > 1.0 - border_width || i.uv.y < border_width || i.uv.y > 1.0 - border_width);
				
				float2 c_uv = i.uv - 0.5;

				float2 pos_from_center = (_ScreenParams.xy * 0.5 - i.vertex.xy) / i.vertex.z / _CrossSize;

				draw = draw || (abs(pos_from_center.x) < 100) && (abs(pos_from_center.y) < 400);
				draw = draw || (abs(pos_from_center.x) < 400) && (abs(pos_from_center.y) < 100);

				if (!draw) {
					discard;
				}

				return _Color;
			}
			ENDCG
		}
	}
}
