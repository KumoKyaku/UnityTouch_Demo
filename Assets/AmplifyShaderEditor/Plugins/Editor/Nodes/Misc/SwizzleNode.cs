// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Swizzle 
// Donated by Tobias Pott - @ Tobias Pott
// www.tobiaspott.de

using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Swizzle", "Misc", "swizzle components of vector types " )]
	public sealed class SwizzleNode : SingleInputOp
	{

		private const string OutputTypeStr = "Output type";

		[SerializeField]
		private WirePortDataType m_selectedOutputType = WirePortDataType.FLOAT4;

		[SerializeField]
		private int m_selectedOutputTypeInt = 4;
		[SerializeField]
		private int[] m_selectedOutputSwizzleTypes = new int[] { 0, 1, 2, 3 };


		private readonly string[] SwizzleVectorChannels = { "x", "y", "z", "w" };
		private readonly string[] SwizzleColorChannels = { "r", "g", "b", "a" };
		private readonly string[] SwizzleChannelLabels = { "Channel 0", "Channel 1", "Channel 2", "Channel 3" };

		private readonly string[] _outputValueTypes ={  "Float",
														"Vector2",
														"Vector3",
														"Vector4"};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].CreatePortRestrictions( WirePortDataType.FLOAT,
														WirePortDataType.FLOAT2,
														WirePortDataType.FLOAT3,
														WirePortDataType.FLOAT4,
														WirePortDataType.COLOR,
														WirePortDataType.INT );


			m_inputPorts[ 0 ].DataType = WirePortDataType.FLOAT4;
			m_outputPorts[ 0 ].DataType = m_selectedOutputType;
			m_textLabelWidth = 90;
			m_autoWrapProperties = true;
			m_autoUpdateOutputPort = false;
		}


		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			Rect rect = m_globalPosition;
			rect.x = rect.x + ( Constants.NodeButtonDeltaX - 1 ) * drawInfo.InvertedZoom + 1;
			rect.y = rect.y + Constants.NodeButtonDeltaY * drawInfo.InvertedZoom;
			rect.width = Constants.NodeButtonSizeX * drawInfo.InvertedZoom;
			rect.height = Constants.NodeButtonSizeY * drawInfo.InvertedZoom;
			EditorGUI.BeginChangeCheck();
			m_selectedOutputTypeInt = EditorGUIPopup( rect,m_selectedOutputTypeInt, _outputValueTypes, UIUtils.PropertyPopUp );
			if ( EditorGUI.EndChangeCheck() )
			{
				switch ( m_selectedOutputTypeInt )
				{
					case 0: m_selectedOutputType = WirePortDataType.FLOAT; break;
					case 1: m_selectedOutputType = WirePortDataType.FLOAT2; break;
					case 2: m_selectedOutputType = WirePortDataType.FLOAT3; break;
					case 3: m_selectedOutputType = WirePortDataType.FLOAT4; break;
				}

				UpdatePorts();
			}
		}

		public override void DrawProperties()
		{

			EditorGUILayout.BeginVertical();
			EditorGUI.BeginChangeCheck();
			m_selectedOutputTypeInt = EditorGUILayoutPopup( OutputTypeStr, m_selectedOutputTypeInt, _outputValueTypes );
			if ( EditorGUI.EndChangeCheck() )
			{
				switch ( m_selectedOutputTypeInt )
				{
					case 0: m_selectedOutputType = WirePortDataType.FLOAT; break;
					case 1: m_selectedOutputType = WirePortDataType.FLOAT2; break;
					case 2: m_selectedOutputType = WirePortDataType.FLOAT3; break;
					case 3: m_selectedOutputType = WirePortDataType.FLOAT4; break;
				}

				UpdatePorts();
			}
			EditorGUILayout.EndVertical();

			// Draw base properties
			base.DrawProperties();

			EditorGUILayout.BeginVertical();

			int count = 0;

			switch ( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				count = 4;
				break;
				case WirePortDataType.FLOAT3:
				count = 3;
				break;
				case WirePortDataType.FLOAT2:
				count = 2;
				break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				count = 1;
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				break;
			}

			int inputMaxChannelId = 0;
			switch ( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				inputMaxChannelId = 3;
				break;
				case WirePortDataType.FLOAT3:
				inputMaxChannelId = 2;
				break;
				case WirePortDataType.FLOAT2:
				inputMaxChannelId = 1;
				break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				inputMaxChannelId = 0;
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				break;
			}

			EditorGUI.BeginChangeCheck();
			if ( m_inputPorts[ 0 ].DataType == WirePortDataType.COLOR )
			{
				for ( int i = 0; i < count; i++ )
				{
					m_selectedOutputSwizzleTypes[ i ] = Mathf.Clamp( EditorGUILayoutPopup( SwizzleChannelLabels[ i ], m_selectedOutputSwizzleTypes[ i ], SwizzleColorChannels ), 0, inputMaxChannelId );
				}
			}
			else
			{
				for ( int i = 0; i < count; i++ )
				{
					m_selectedOutputSwizzleTypes[ i ] = Mathf.Clamp( EditorGUILayoutPopup( SwizzleChannelLabels[ i ], m_selectedOutputSwizzleTypes[ i ], SwizzleVectorChannels ), 0, inputMaxChannelId );
				}
			}
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
			}

			EditorGUILayout.EndVertical();
		}

		void UpdatePorts()
		{
			m_sizeIsDirty = true;
			ChangeOutputType( m_selectedOutputType, false );
		}

		public string GetSwizzleComponentForChannel( int channel )
		{
			if ( m_inputPorts[ 0 ].DataType == WirePortDataType.COLOR )
			{
				return SwizzleColorChannels[ channel ];
			}
			else
			{
				return SwizzleVectorChannels[ channel ];
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;
			
			string value = string.Format( "({0}).", m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) );
			int count = 0;
			switch ( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				count = 4;
				break;
				case WirePortDataType.FLOAT3:
				count = 3;
				break;
				case WirePortDataType.FLOAT2:
				count = 2;
				break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				count = 1;
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				break;
			}
			for ( int i = 0; i < count; i++ )
			{
				value += GetSwizzleComponentForChannel( m_selectedOutputSwizzleTypes[ i ] );
			}

			return CreateOutputLocalVariable( 0, value, ref dataCollector );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedOutputType = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
			switch ( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT: m_selectedOutputTypeInt = 0; break;
				case WirePortDataType.FLOAT2: m_selectedOutputTypeInt = 1; break;
				case WirePortDataType.FLOAT3: m_selectedOutputTypeInt = 2; break;
				case WirePortDataType.COLOR:
				case WirePortDataType.FLOAT4: m_selectedOutputTypeInt = 3; break;
			}
			for ( int i = 0; i < m_selectedOutputSwizzleTypes.Length; i++ )
			{
				m_selectedOutputSwizzleTypes[ i ] = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}

			UpdatePorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedOutputType );
			for ( int i = 0; i < m_selectedOutputSwizzleTypes.Length; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedOutputSwizzleTypes[ i ] );
			}
		}
	}
}
