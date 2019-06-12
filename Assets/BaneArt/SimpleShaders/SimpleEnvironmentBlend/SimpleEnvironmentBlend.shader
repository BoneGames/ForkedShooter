// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SimpleEnvironmentBlend"
{
	Properties
	{
		_Float1("Float 1", Float) = 1
		_Top_Y("Top_Y", Color) = (0.7259277,0.7647059,0.06185123,0)
		_Gradient("Gradient", Range( 0 , 20)) = 6.4
		_Top_XZ("Top_XZ", Color) = (0.2569204,0.5525266,0.7279412,0)
		_Bot_XZ("Bot_XZ", Color) = (0.7058823,0.2024221,0.2024221,0)
		_Bot_Y("Bot_Y", Color) = (0.3877363,0.5955882,0.188311,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldNormal;
			float3 worldPos;
		};

		uniform float4 _Bot_XZ;
		uniform float4 _Bot_Y;
		uniform float4 _Top_XZ;
		uniform float4 _Top_Y;
		uniform float _Gradient;
		uniform float _Float1;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = i.worldNormal;
			float3 temp_output_19_0 = abs( ase_worldNormal );
			float YGradient22 = (( temp_output_19_0 * temp_output_19_0 )).y;
			float4 lerpResult16 = lerp( _Bot_XZ , _Bot_Y , YGradient22);
			float4 lerpResult15 = lerp( _Top_XZ , _Top_Y , YGradient22);
			float3 ase_worldPos = i.worldPos;
			float clampResult30 = clamp( ( ase_worldPos.y / _Gradient ) , 0.0 , 1.0 );
			float worldGradient32 = clampResult30;
			float4 lerpResult34 = lerp( lerpResult16 , lerpResult15 , worldGradient32);
			float4 outputGradient40 = lerpResult34;
			float4 temp_cast_0 = (0.0).xxxx;
			float4 temp_cast_1 = (_Float1).xxxx;
			float4 clampResult36 = clamp( outputGradient40 , temp_cast_0 , temp_cast_1 );
			o.Albedo = clampResult36.rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
234;92;1142;593;1601.145;55.7847;1.804623;True;True
Node;AmplifyShaderEditor.CommentaryNode;17;-2512.937,1557.141;Float;False;980.8558;289.1547;Get World Y Vector Mask;5;22;21;20;19;18;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;18;-2476.158,1644.252;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.AbsOpNode;19;-2277.188,1646.961;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;25;-2440.209,1858.47;Float;False;911.1976;457.6171;Create the world gradient;7;32;27;28;30;29;26;31;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-2416.767,2050.479;Float;False;Property;_Gradient;Gradient;2;0;Create;True;0;0;False;0;6.4;4.1;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;31;-2416.934,1903.411;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-2124.723,1654.148;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;29;-2125.389,1953.9;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-2289.121,2222.728;Float;False;Constant;_Float3;Float 3;-1;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-2285.757,2128.526;Float;False;Constant;_Float4;Float 4;-1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;21;-1959.173,1654.59;Float;False;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;-1749.725,1658.133;Float;False;YGradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;10;-2167.223,1061.537;Float;False;620.1286;475.2801;The same lerp for the Top Gradient Colors;4;11;15;12;24;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;9;-2169.554,655.6949;Float;False;627.3674;396.8867;Lerp the 2 Gradient Bottom Colors according to the above normals y vector;4;14;13;23;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ClampOpNode;30;-1959.849,1960.227;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;-1749.942,1956.441;Float;False;worldGradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;-1861.708,1413.147;Float;False;22;YGradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;33;-1418.069,865.5403;Float;False;763.2592;360.8531;;3;40;34;35;Blend the bottom and top colours according to world gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;11;-2105.208,1319.745;Float;False;Property;_Top_Y;Top_Y;1;0;Create;True;0;0;False;0;0.7259277,0.7647059,0.06185123,0;0.7259277,0.7647059,0.06185123,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;23;-1875.158,963.086;Float;False;22;YGradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-2122.532,873.0019;Float;False;Property;_Bot_Y;Bot_Y;5;0;Create;True;0;0;False;0;0.3877363,0.5955882,0.188311,0;0.3877363,0.5955882,0.188311,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;12;-2104.905,1145.347;Float;False;Property;_Top_XZ;Top_XZ;3;0;Create;True;0;0;False;0;0.2569204,0.5525266,0.7279412,0;0.2569204,0.5525266,0.7279412,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;-2122.231,705.8011;Float;False;Property;_Bot_XZ;Bot_XZ;4;0;Create;True;0;0;False;0;0.7058823,0.2024221,0.2024221,0;0.7058823,0.2024221,0.2024221,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;15;-1786.761,1113.014;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-1389.519,1135.021;Float;False;32;worldGradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;16;-1773.709,702.868;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;34;-1119.529,921.929;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-866.6628,916.907;Float;False;outputGradient;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-682.3069,554.8747;Float;False;Constant;_Float0;Float 0;-1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-685.6709,649.0768;Float;False;Property;_Float1;Float 1;0;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;-722.2937,462.1422;Float;False;40;outputGradient;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;36;-355.1277,464.2429;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SimpleEnvironmentBlend;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;0;18;0
WireConnection;20;0;19;0
WireConnection;20;1;19;0
WireConnection;29;0;31;2
WireConnection;29;1;26;0
WireConnection;21;0;20;0
WireConnection;22;0;21;0
WireConnection;30;0;29;0
WireConnection;30;1;28;0
WireConnection;30;2;27;0
WireConnection;32;0;30;0
WireConnection;15;0;12;0
WireConnection;15;1;11;0
WireConnection;15;2;24;0
WireConnection;16;0;14;0
WireConnection;16;1;13;0
WireConnection;16;2;23;0
WireConnection;34;0;16;0
WireConnection;34;1;15;0
WireConnection;34;2;35;0
WireConnection;40;0;34;0
WireConnection;36;0;41;0
WireConnection;36;1;38;0
WireConnection;36;2;39;0
WireConnection;0;0;36;0
ASEEND*/
//CHKSM=71F14136E0CC2EF4C1C5B519D9AD5DDEFCD7A45F