// Amplify Shader Editor - Advanced Bloom Post-Effect for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Billboard", "Vertex Data", "Calculates new Vertex positions and normals to achieve a billboard effect." )]
	public sealed class BillboardNode : ParentNode
	{
		private const string ErrorMessage = "Billboard node should only be connected to vertex ports.";
		private const string WarningMessage = "This nodes is a bit different from all others as it injects the necessary code into the vertex body and writes directly on the vertex position and normal.\nIt outputs a value of 0 so it can be connected directly to a vertex port.\n[Only if that port is a relative vertex offset].";

		[SerializeField]
		private BillboardType m_billboardType = BillboardType.Cylindrical;

		[SerializeField]
		private bool m_rotationIndependent = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT3, Constants.EmptyPortValue );
			m_textLabelWidth = 115;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, () =>
			 {
				 m_billboardType = ( BillboardType ) EditorGUILayoutEnumPopup( BillboardOpHelper.BillboardTypeStr, m_billboardType );
				 m_rotationIndependent = EditorGUILayoutToggle( BillboardOpHelper.BillboardRotIndStr, m_rotationIndependent );
			 } );
			EditorGUILayout.HelpBox( WarningMessage, MessageType.Warning );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.IsFragmentCategory )
			{
				UIUtils.ShowMessage( ErrorMessage );
				return "0";
			}
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return "0";

			m_outputPorts[ 0 ].SetLocalValue( "0" );
			string vertexPosValue = "v.vertex";
			string vertexNormalValue = "v.normal";
			BillboardOpHelper.FillDataCollector( ref dataCollector, m_billboardType, m_rotationIndependent, vertexPosValue, vertexNormalValue );

			return "0";
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_billboardType = ( BillboardType ) Enum.Parse( typeof( BillboardType ), GetCurrentParam( ref nodeParams ) );
			m_rotationIndependent = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_billboardType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_rotationIndependent );
		}
	}
}
