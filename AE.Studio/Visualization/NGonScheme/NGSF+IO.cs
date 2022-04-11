using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using AE.Visualization.SchemeObjectModel;

//using System.Drawing;
//using System.Data;
////using System.Text;
//using System.Windows.Forms;
//using OpenTK;
//using OpenTK.Graphics;
//using OpenTK.Graphics.OpenGL;

namespace AE.Visualization
{
	public partial class NGonSchemeFrame : SchemeFrame<NGonNode>
	{
		public List<NGonNode> Inputs = new List<NGonNode>();
		public List<NGonNode> Outputs = new List<NGonNode>();
		public List<NGonNode> Scripts = new List<NGonNode>();

		public struct IO
		{
			public static Keys StringToKey(string iKeyString)
			{
				throw new WTFE();
			}
			public static void OnRender(NGonSchemeFrame iFrame)
			{
				///return;

				iFrame.Scheme.ResetHighlighting();


				foreach(var cInputNode in iFrame.Inputs)
				{
					var cKeyName = cInputNode.Type.Substring(1).Trim('[',']');
					var cKeyCode = (Keys)typeof(Keys).GetField(cKeyName).GetValue(null);
					//cNodeKey

					if(Keyboard.IsKeyDown(cKeyCode))
					{
						iFrame.ProcessPath(cInputNode, 1.0, true);
					}
					
					//if(cNodeKey == _InputKeyName)
					//{
						
					//    iFrame.ProcessPath(cInputNode, 1.0, true);
					//    ///cInputNode.IsHighlighting = true;
					//}
				}
			}

			public static void OnKeyDown(NGonSchemeFrame iFrame, KeyEventArgs iEvent)
			{
			//    return;


			//    //iFrame.Scheme.ResetHighlighting();



			//    var _InputKeyName = iEvent.KeyCode.ToString();

				

			//    foreach(var cInputNode in iFrame.Inputs)
			//    {
			//        var cNodeKey = cInputNode.Type.Substring(1).Trim('[',']');
					
			//        if(cNodeKey == _InputKeyName)
			//        {
						
			//            iFrame.ProcessPath(cInputNode, 1.0, true);
			//            ///cInputNode.IsHighlighting = true;
			//        }
			//    }
			//    //G.Console.Message("*OnKeyDown");

			//    //iFrame.Canvas.ParentForm.Is
			//    //System.Windows.Forms.Form.Is
			//    //System.Windows.I
			}
			public static void OnKeyUp(NGonSchemeFrame iFrame, KeyEventArgs iEvent)
			{
			//    ///iFrame.Scheme.ResetHighlighting();
			//    //G.Console.Message("*OnKeyUp");

			//    //System.Windows.Forms.Application.A
			}


		
		}
		
		//public struct IO
		//{
			
		//}
	}
}