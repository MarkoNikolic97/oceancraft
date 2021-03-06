// Crest Ocean System

// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

// Persistent foam sim
Shader "Hidden/Crest/Simulation/Update Foam"
{
	SubShader
	{
		Pass
		{
			Name "UpdateFoam"
			Blend Off
			ZWrite Off
			ZTest Always

			CGPROGRAM
			// For SV_VertexID
			#pragma target 3.5
			#pragma vertex Vert
			#pragma fragment Frag

			#include "UnityCG.cginc"
			#include "../OceanLODData.hlsl"
			#include "../FullScreenTriangle.hlsl"

			float _FoamFadeRate;
			float _WaveFoamStrength;
			float _WaveFoamCoverage;
			float _ShorelineFoamMaxDepth;
			float _ShorelineFoamStrength;
			float _SimDeltaTime;
			float _SimDeltaTimePrev;

			struct Attributes
			{
				uint vertexID : SV_VertexID;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float4 uv_uv_lastframe : TEXCOORD0;
				float2 positionWS_XZ : TEXCOORD1;
			};

			Varyings Vert(Attributes input)
			{
				Varyings output;

				output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
				float2 uv = GetFullScreenTriangleTexCoord(input.vertexID);

				output.uv_uv_lastframe.xy = uv;

				// lod data 1 is current frame, compute world pos from quad uv
				output.positionWS_XZ = LD_1_UVToWorld(uv);
				output.uv_uv_lastframe.zw = LD_0_WorldToUV(output.positionWS_XZ);

				return output;
			}

			half Frag(Varyings input) : SV_Target
			{
				float4 uv = float4(input.uv_uv_lastframe.xy, 0.0, 0.0);
				float4 uv_lastframe = float4(input.uv_uv_lastframe.zw, 0.0, 0.0);

				// #if _FLOW_ON
				half4 velocity = half4(tex2Dlod(_LD_Sampler_Flow_1, uv).xy, 0.0, 0.0);
				half foam = tex2Dlod(_LD_Sampler_Foam_0, uv_lastframe
					- ((_SimDeltaTime * _LD_Params_0.w) * velocity)
					).x;
				// #else
				// // sampler will clamp the uv currently
				// half foam = tex2Dlod(_LD_Sampler_Foam_0, uv_lastframe).x;
				// #endif

				half2 r = abs(uv_lastframe.xy - 0.5);
				if (max(r.x, r.y) > 0.5 - _LD_Params_0.w)
				{
					// no border wrap mode for RTs in unity it seems, so make any off-texture reads 0 manually
					foam = 0.0;
				}

				// fade
				foam *= max(0.0, 1.0 - _FoamFadeRate * _SimDeltaTime);

				// sample displacement texture and generate foam from it
				const float3 dd = float3(_LD_Params_1.w, 0.0, _LD_Params_1.x);
				half3 s = tex2Dlod(_LD_Sampler_AnimatedWaves_1, uv).xyz;
				half3 sx = tex2Dlod(_LD_Sampler_AnimatedWaves_1, uv + dd.xyyy).xyz;
				half3 sz = tex2Dlod(_LD_Sampler_AnimatedWaves_1, uv + dd.yxyy).xyz;
				float3 disp = s.xyz;
				float3 disp_x = dd.zyy + sx.xyz;
				float3 disp_z = dd.yyz + sz.xyz;
				// The determinant of the displacement Jacobian is a good measure for turbulence:
				// > 1: Stretch
				// < 1: Squash
				// < 0: Overlap
				float4 du = float4(disp_x.xz, disp_z.xz) - disp.xzxz;
				float det = (du.x * du.w - du.y * du.z) / (_LD_Params_1.x * _LD_Params_1.x);
				foam += 5.0 * _SimDeltaTime * _WaveFoamStrength * saturate(_WaveFoamCoverage - det);

				// add foam in shallow water. use the displaced position to ensure we add foam where world objects are.
				float4 uv_1_displaced = float4(LD_1_WorldToUV(input.positionWS_XZ + disp.xz), 0.0, 1.0);
				float waterDepth = tex2Dlod(_LD_Sampler_SeaFloorDepth_1, uv_1_displaced).x + disp.y;
				foam += _ShorelineFoamStrength * _SimDeltaTime * saturate(1.0 - waterDepth / _ShorelineFoamMaxDepth);

				return foam;
			}
			ENDCG
		}
	}
}
