// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Effect/Gravity"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
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
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float actualPos : TEXCOORD4;
	};

	float4 _Color;
	sampler2D _MainTex;
	float4 _MainTex_ST;
	float3 center;
	uniform float rectBounds[10];

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.actualPos = v.vertex;
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		fixed4 col = _Color;
	col.a = .01f;
	float distance = center.x - i.actualPos.x;
	for (int j = 0; j < 10; j += 2)
	{
		float firstY = rectBounds[j];
		float secondY = rectBounds[j + 1];

		if (distance > firstY && distance < secondY)
		{
			float centerY = (firstY + secondY) / 2;
			float range = secondY - firstY;
			float result = ((centerY - distance) / range) / 2;
			if (result < 1)
			{
				result = -result;
			}

			col.a = result;
		}
	}

	return col;
	}
		ENDCG
	}
	}
}