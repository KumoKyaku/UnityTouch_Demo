// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Object Space Light Dir", "Forward Render", "Computes object space light direction (not normalized)" )]
	public sealed class ObjSpaceLightDirHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "ObjSpaceLightDir";
			m_inputPorts[ 0 ].Visible = false;
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_outputPorts[ 0 ].Name = "XYZ";
			m_useInternalPortData = false;
			m_previewShaderGUID = "c7852de24cec4a744b5358921e23feee";
			m_drawPreviewAsSphere = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			dataCollector.AddToIncludes( UniqueId, Constants.UnityCgLibFuncs );
			dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_POS ), true );

			string vertexPos = GeneratorUtils.GenerateVertexPosition( ref dataCollector, UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT4 );
			return GeneratorUtils.GenerateObjectLightDirection( ref dataCollector, UniqueId, m_currentPrecisionType, vertexPos );


			//			if ( m_outputPorts[ 0 ].IsLocalValue )
			//				return m_outputPorts[ 0 ].LocalValue;

			//			dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_POS ), true );
			//			dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );

			//#if UNITY_5_4_OR_NEWER
			//			string matrix = "unity_WorldToObject";
			//#else
			//				string matrix = "_World2Object";
			//#endif

			//			string value = "normalize( " + m_funcType + "( mul( " + matrix + ", float4( " + Constants.InputVarStr + ".worldPos , 1 ) ) ) )";
			//			RegisterLocalVariable( 0, value, ref dataCollector, "objectSpaceLightDir" + OutputId );
			//			return m_outputPorts[ 0 ].LocalValue;
		}
	}
}
