// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/HearingShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}

	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		Tags { "RenderType"="Transparent" }
		LOD 200

		Pass
	{
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert
		#pragma fragment frag

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		StructuredBuffer<float4> soundPositions;
		StructuredBuffer<float> soundRadii;

		int numPositions;

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			float4 oldPos : TEXCOORD1;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
			float4 oldPos : TEXCOORD1;
		};

		fixed4 _Color;

		v2f vert(appdata v)
		{
			v2f o;
			o.oldPos = v.vertex;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		fixed4 frag(v2f IN) : SV_Target
		{
			fixed4 col;
			col = float4(0, 0, 0, 1);

			for (int i = 0; i < numPositions; i++)
			{
				float3 pos = mul(unity_ObjectToWorld, IN.oldPos).xyz;
				float dist = distance(pos, soundPositions[i]);

				if (dist < soundRadii[i])
				{
					col = float4(1, 1, 1, 1);
					break;
				}
			}
			
			return col;
		}
		ENDCG
		}
	}
	FallBack "Diffuse"
}
