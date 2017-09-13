// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Indirect Diffuse Light", "Light", "Indirect Lighting", NodeAvailabilityFlags = ( int ) NodeAvailability.CustomLighting )]
	public sealed class IndirectDiffuseLighting : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddOutputPort( WirePortDataType.FLOAT3, "Out" );
			m_errorMessageTypeIsError = NodeMessageType.Warning;
			m_errorMessageTooltip = "This node only returns correct information using a custom light model, otherwise returns 0";
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData , ref dataCollector );
			if ( m_inputPorts[ 0 ].IsConnected )
				dataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting )
				return "float3(0,0,0)";

			string normal = string.Empty;
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
				dataCollector.AddToInput( UniqueId, Constants.InternalData, false );
				dataCollector.ForceNormal = true;

				normal = "WorldNormalVector( " + Constants.InputVarStr + " , " + m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar ) + " )";
			}
			else
			{
				normal = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
				dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
			}

			return GeneratorUtils.GenerateIndirectLighting( ref dataCollector, UniqueId, m_currentPrecisionType, normal );
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if ( ContainerGraph.CurrentStandardSurface != null && ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting )
				m_showErrorMessage = true;
			else
				m_showErrorMessage = false;
		}
	}
}
