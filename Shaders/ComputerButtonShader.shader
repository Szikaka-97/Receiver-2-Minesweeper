Shader "Minesweeper/ComputerButtonShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Offset -1, -1

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _PressedAmount;

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);

				float2 pos_from_center = _ScreenParams.xy * 0.5 - i.vertex.xy;

				// if (length(pos_from_center) < 200 * i.vertex.z) {
				if (
					pos_from_center.x < -pos_from_center.y * 0.1
					&&
					-pos_from_center.x < pos_from_center.y
					&&
					pos_from_center.x > 2 * pos_from_center.y - 3000 * i.vertex.z
				) {
					col.xyz = float3(0, 1, 0);
				}

				return col + _PressedAmount;
			}
			ENDCG
		}
	}
}
