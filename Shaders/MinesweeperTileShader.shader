Shader "Minesweeper/MinesweeperTileShader"
{
	Properties
	{
		_TextureAtlas ("Texture", 2D) = "white" {}
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
            #pragma multi_compile_instancing

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _TextureAtlas;
			float4 _TextureAtlas_ST;
			int _MinefieldSize;
			float4x4 _RootPosition;
			Buffer<uint> _TileStates;

			v2f vert(appdata v, uint instanceID : SV_InstanceID) {
				const float2 StateOffsets[] = {
					float2(0, 0),
					float2(1, 0),
					float2(2, 0),
					float2(3, 0),
					float2(0, 1),
					float2(1, 1),
					float2(2, 1),
					float2(3, 1),
					float2(0, 2),
					float2(1, 2),
					float2(2, 2),
					float2(3, 2),
					float2(0, 3),
				};

				v2f o;

				float4 offset = float4(0, 0, 0, 0);

				offset.xy = float2(
					instanceID % _MinefieldSize,
					instanceID / _MinefieldSize
				);

				o.vertex = mul(
					UNITY_MATRIX_VP,
					float4(
						mul(
							_RootPosition,
							float4((v.vertex + offset).xyz, 1.0)
						).xyz,
						1.0
					)
				);

				o.uv = (TRANSFORM_TEX(v.uv, _TextureAtlas) + StateOffsets[_TileStates[instanceID]]) * 0.25;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 col = tex2D(_TextureAtlas, i.uv);
				
				return col;
			}
			ENDCG
		}
	}
}
