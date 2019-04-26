// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IntersectShader"
{
	Properties
	{
		_Diffuse("Diffuse", 2D) = "white" {}
		_Colour("Colour", Color) = (0,0,0,1)
		[Toggle]_TextureColourToggle("Texture/Colour Toggle", Float) = 1
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		_PanSpeed("Pan Speed", Vector) = (0,0,0,0)
		_IntersectIntensity("Intersect Intensity", Range( 0 , 1)) = 0.4479938
		_IntersectColor("Intersect Color", Color) = (1,0,0,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float _TextureColourToggle;
		uniform sampler2D _Diffuse;
		uniform float2 _PanSpeed;
		uniform float4 _Colour;
		uniform float4 _IntersectColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _IntersectIntensity;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 panner33 = ( 1.0 * _Time.y * _PanSpeed + i.uv_texcoord);
			float4 diffuse24 = lerp(tex2D( _Diffuse, panner33 ),_Colour,_TextureColourToggle);
			float4 temp_output_26_0 = diffuse24;
			o.Albedo = temp_output_26_0.rgb;
			float4 color22 = IsGammaSpace() ? float4(0,0,0,1) : float4(0,0,0,1);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth10 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth10 = abs( ( screenDepth10 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _IntersectIntensity ) );
			float clampResult14 = clamp( distanceDepth10 , 0.0 , 1.0 );
			float4 lerpResult17 = lerp( _IntersectColor , color22 , clampResult14);
			float4 Intersection31 = lerpResult17;
			o.Emission = ( diffuse24 + Intersection31 ).rgb;
			o.Alpha = _Opacity;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
1927;135;1172;827;870.0016;360.0286;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;1;-1779.126,-154.0935;Float;False;1092.073;631.3205;Mix of Pattern, Wave, Rim , Impact and adding intersection highlight;6;22;17;14;13;10;9;Shield Mix for Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-3672.989,-123.1458;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;35;-3597.989,-5.145782;Float;False;Property;_PanSpeed;Pan Speed;4;0;Create;True;0;0;False;0;0,0;0,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;9;-1752.888,374.7751;Float;False;Property;_IntersectIntensity;Intersect Intensity;5;0;Create;True;0;0;False;0;0.4479938;0.53;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;10;-1453.048,356.8683;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;33;-3356.18,-117.9095;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;14;-1170.595,340.096;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;29;-3072.166,69.09509;Float;False;Property;_Colour;Colour;1;0;Create;True;0;0;False;0;0,0,0,1;0.3490566,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;22;-1233.86,87.89163;Float;False;Constant;_Empty;Empty;5;0;Create;True;0;0;False;0;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;20;-3157.941,-121.4963;Float;True;Property;_Diffuse;Diffuse;0;0;Create;True;0;0;False;0;84508b93f15f2b64386ec07486afc7a3;84508b93f15f2b64386ec07486afc7a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;13;-1283.202,-104.0933;Float;False;Property;_IntersectColor;Intersect Color;6;0;Create;True;0;0;False;0;1,0,0,1;0,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;17;-922.8029,91.20604;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;23;-2803.191,-118.5488;Float;True;Property;_TextureColourToggle;Texture/Colour Toggle;2;0;Create;True;0;0;False;0;1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-661.3955,115.788;Float;False;Intersection;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-2503.725,-119.708;Float;False;diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;32;-425.0755,-32.1078;Float;False;31;Intersection;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;26;-404.757,-117.9019;Float;False;24;diffuse;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-192.4472,-48.21296;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;-179.0016,70.97141;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-320.312,246.1231;Float;False;Property;_Opacity;Opacity;3;0;Create;True;0;0;False;0;0.5;0.373;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;25.84922,-99.77617;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;IntersectShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;9;0
WireConnection;33;0;34;0
WireConnection;33;2;35;0
WireConnection;14;0;10;0
WireConnection;20;1;33;0
WireConnection;17;0;13;0
WireConnection;17;1;22;0
WireConnection;17;2;14;0
WireConnection;23;0;20;0
WireConnection;23;1;29;0
WireConnection;31;0;17;0
WireConnection;24;0;23;0
WireConnection;43;0;26;0
WireConnection;43;1;32;0
WireConnection;44;0;26;0
WireConnection;44;1;32;0
WireConnection;0;0;26;0
WireConnection;0;2;44;0
WireConnection;0;9;21;0
ASEEND*/
//CHKSM=38AC1A2F8DDB8054F010B787DAEB0D6306B90302