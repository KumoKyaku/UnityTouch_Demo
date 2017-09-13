// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public sealed class ShaderEditorModeWindow : MenuParent
	{
		private static readonly Color OverallColorOn = new Color( 1f, 1f, 1f, 0.9f );
		private static readonly Color OverallColorOff = new Color( 1f, 1f, 1f, 0.3f );
		private static readonly Color FontColorOff = new Color( 1f, 1f, 1f, 0.4f );
		private const float DeltaY = 15;
		private const float DeltaX = 10;

		private const float CollSizeX = 180;
		private const float CollSizeY = 70;

		//private static string MatFormat = "<size=20>MATERIAL</size>\n{0}";
		//private static string ShaderFormat = "<size=20>SHADER</size>\n{0}";
		//private const string CurrMatStr = "MATERIAL";
		//private const string CurrShaderStr = "SHADER";

		private const string NoMaterialStr = "No Material";
		private const string NoShaderStr = "No Shader";

		private bool m_init = true;
		//private GUIStyle m_materialLabelStyle;
		//private GUIStyle m_shaderLabelStyle;

		//private GUIStyle m_rightStyle;
		//private Texture2D m_shaderOnTexture;
		//private Texture2D m_materialOnTexture;

		//private GUIContent m_materialContent;
		//private GUIContent m_shaderContent;

		private Vector2 m_auxVector2;
		private GUIContent m_auxContent;
		private GUIStyle m_leftButtonStyle = null;
		private GUIStyle m_rightButtonStyle = null;
		private Rect m_leftButtonRect;
		private Rect m_rightButtonRect;

		public ShaderEditorModeWindow( AmplifyShaderEditorWindow parentWindow ) : base( parentWindow, 0, 0, 0, 0, "ShaderEditorModeWindow", MenuAnchor.BOTTOM_CENTER, MenuAutoSize.NONE ) { }

		public void ConfigStyle( GUIStyle style )
		{
			style.normal.textColor = FontColorOff;
			style.hover.textColor = FontColorOff;
			style.active.textColor = FontColorOff;
			style.focused.textColor = FontColorOff;

			style.onNormal.textColor = FontColorOff;
			style.onHover.textColor = FontColorOff;
			style.onActive.textColor = FontColorOff;
			style.onFocused.textColor = FontColorOff;
		}


		public void Draw( Rect graphArea, Vector2 mousePos, Shader currentShader, Material currentMaterial, float usableArea, float leftPos, float rightPos /*, bool showLastSelection*/ )
		{
			if ( m_init )
			{
				m_init = false;
				GUIStyle shaderModeTitle = UIUtils.GetCustomStyle( CustomStyle.ShaderModeTitle );
				GUIStyle shaderModeNoShader = UIUtils.GetCustomStyle( CustomStyle.ShaderModeNoShader );
				GUIStyle materialModeTitle = UIUtils.GetCustomStyle( CustomStyle.MaterialModeTitle );
				GUIStyle shaderNoMaterialModeTitle = UIUtils.GetCustomStyle( CustomStyle.ShaderNoMaterialModeTitle );

				ConfigStyle( shaderModeTitle );
				ConfigStyle( shaderModeNoShader );
				ConfigStyle( materialModeTitle );
				ConfigStyle( shaderNoMaterialModeTitle );
			}
			Color buffereredColor = GUI.color;

			MasterNode currentMasterNode = ParentWindow.CurrentGraph.CurrentMasterNode;
			// Shader Mode
			if ( currentMasterNode != null )
			{
				m_leftButtonStyle = UIUtils.GetCustomStyle( currentShader == null ? CustomStyle.ShaderModeNoShader : CustomStyle.ShaderModeTitle );
				m_leftButtonRect = graphArea;
				m_leftButtonRect.x = 10 + leftPos;
				m_leftButtonRect.y += m_leftButtonRect.height - 38 - 15;
				string shaderName = ( currentShader != null ) ? ( currentShader.name ) : NoShaderStr;
				m_auxContent = new GUIContent( "<size=20>SHADER</size>\n" + shaderName );
				m_auxVector2 = m_leftButtonStyle.CalcSize( m_auxContent );
				m_leftButtonRect.width = m_auxVector2.x + 30 + 4;
				m_leftButtonRect.height = 38;

				GUI.color = m_leftButtonRect.Contains( mousePos ) ? OverallColorOn : OverallColorOff;

				if ( GUI.Button( m_leftButtonRect, m_auxContent, m_leftButtonStyle ) && currentShader != null )
				{
					Selection.activeObject = currentShader;
					EditorGUIUtility.PingObject( Selection.activeObject );
				}

				// Material Mode
				if( currentMaterial != null )
				{
					m_rightButtonStyle = UIUtils.GetCustomStyle( CustomStyle.MaterialModeTitle );
					m_rightButtonRect = graphArea;
					string matName = ( currentMaterial != null ) ? ( currentMaterial.name ) : NoMaterialStr;
					m_auxContent = new GUIContent( "<size=20>MATERIAL</size>\n" + matName );
					m_auxVector2 = m_rightButtonStyle.CalcSize( m_auxContent );
					m_rightButtonRect.width = m_auxVector2.x + 30 + 4;
					m_rightButtonRect.height = 38;

					m_rightButtonRect.x = graphArea.xMax - m_rightButtonRect.width - rightPos - 10;
					m_rightButtonRect.y = graphArea.yMax - 38 - 15;

					GUI.color = m_rightButtonRect.Contains( mousePos ) ? OverallColorOn : OverallColorOff;

					if ( GUI.Button( m_rightButtonRect, m_auxContent, m_rightButtonStyle ) )
					{
						Selection.activeObject = currentMaterial;
						EditorGUIUtility.PingObject( Selection.activeObject );
					}
				}
			}
			
			// Shader Function
			else if( currentMasterNode == null && ParentWindow.CurrentGraph.CurrentOutputNode != null)
			{
				m_leftButtonStyle = UIUtils.GetCustomStyle( CustomStyle.ShaderFunctionMode );
				m_leftButtonRect = graphArea;
				m_leftButtonRect.x = 10 + leftPos;
				m_leftButtonRect.y += m_leftButtonRect.height - 38 - 15;
				string functionName = ( ParentWindow.CurrentGraph.CurrentShaderFunction != null ) ? ( ParentWindow.CurrentGraph.CurrentShaderFunction.name ) : "No Shader Function";
				m_auxContent = new GUIContent( "<size=20>SHADER FUNCTION</size>\n"+ functionName );
				m_auxVector2 = m_leftButtonStyle.CalcSize( m_auxContent );
				m_leftButtonRect.width = m_auxVector2.x + 30 + 4;
				m_leftButtonRect.height = 38;

				GUI.color = m_leftButtonRect.Contains( mousePos ) ? OverallColorOn : OverallColorOff;

				if ( GUI.Button( m_leftButtonRect, m_auxContent, m_leftButtonStyle ) && ParentWindow.CurrentGraph.CurrentShaderFunction != null)
				{
					Selection.activeObject = ParentWindow.CurrentGraph.CurrentShaderFunction;
					EditorGUIUtility.PingObject( Selection.activeObject );
				}
			}

			GUI.color = buffereredColor;
		}

		public override void Destroy()
		{
			base.Destroy();
			//m_materialLabelStyle = null;
			//m_shaderLabelStyle = null;
			m_leftButtonStyle = null;
			m_rightButtonStyle = null;
		}
	}
}
