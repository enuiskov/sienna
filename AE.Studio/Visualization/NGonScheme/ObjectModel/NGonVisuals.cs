using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using AE.Visualization;

namespace AE.Visualization.SchemeObjectModel
{
	public class VisualObject
	{
		public NGonNode Owner;
		public virtual bool IsVisible {get{return true;}}
		

		public VisualObject(NGonNode iOwner)
		{
			this.Owner = iOwner;
		}

		public virtual void Draw()
		{
			GL.Begin(PrimitiveType.Lines);
			{
				GL.Vertex2(-0.2,-0.2);
				GL.Vertex2(+0.2,+0.2);
				GL.Vertex2(-0.2,+0.2);
				GL.Vertex2(+0.2,-0.2);
			}
			GL.End();
		}
	}
	public class TextVisualObject : VisualObject
	{
		private string Text_ = "Hello, World!";
		public  string Text {get{return this.Text_;} set{this.Text_ = value; this.IsTextUpdated = true;}}
		public  bool IsTextUpdated = true;
		public override bool IsVisible
		{
			get
			{
				return this.Owner.Value != 0;
				//(
				//    !String.IsNullOrEmpty(this.Owner.Name) || 
				//    !String.IsNullOrEmpty(this.Owner.Type) ||
				//    !String.IsNullOrEmpty(this.Owner.Description)
				//);
			}
		}

		public T2fC4ubV3fVertex[] VertexArray;

		public TextVisualObject(NGonNode iOwner) : base(iOwner)
		{
			
		}
		
		public void UpdateText()
		{
			
			this.IsTextUpdated = false;
		}
		public override void Draw()
		{
			var _TextObj = (this.Owner.Frame.Canvas.Control as GLCanvasControl).TestText;
			
			_TextObj.Value =
			(
				(!String.IsNullOrEmpty(this.Owner.Name) ? this.Owner.Name + "\r\n" : "") + 
				(!String.IsNullOrEmpty(this.Owner.Type) ? "<" + this.Owner.Type + ">" + "\r\n" : "") + 
				(!String.IsNullOrEmpty(this.Owner.Description) ? "'" + this.Owner.Description + "'" + "\r\n" : "") + 
				this.Owner.Value.ToString()
				//"Desc : Hello, World!\r\n"
			);
			_TextObj.Color = this.Owner.Color;
			_TextObj.UpdateVertexArray();
			_TextObj.Draw();
			//Screen.Routines.Drawing.DrawText(this.Text);
			//Screen.Routines.Drawing.DrawText(this.Text);
			///Screen.Routines.Drawing.DrawVertexArray(this.VertexArray, Workarounds.Screen.FontAtlas.TexID);
		}
	}
	//public struct Routines
	//{
	//    public class Rendering
	//    {
	//        public static void Render(VisualObject iVisual)
	//        {
	//            GL.Color4(iVisual.Owner.Palette.Colors[iVisual.Owner.Color]);
	//            GL.LineWidth(1f);
	//            GL.PointSize(3f);

	//            switch(iVisual.Type)
	//            {
	//                case "Clock" : Clock(iVisual); break;
	//                case "Cross" : Cross(iVisual); break;
	//                case "Text"  : Text(iVisual); break;

	//            }
	//        }
	//        public static void Cross(VisualObject iVisual)
	//        {
	//            GL.Begin(PrimitiveType.Lines);
	//            {
	//                GL.Vertex2(-0.2,-0.2);
	//                GL.Vertex2(+0.2,+0.2);
	//                GL.Vertex2(-0.2,+0.2);
	//                GL.Vertex2(+0.2,-0.2);
	//            }
	//            GL.End();
	//        }
	//        public static void Clock(VisualObject iVisual)
	//        {
	//            GL.PushMatrix();
	//            GL.Rotate(-DateTime.Now.Millisecond / 1000d * 360, Vector3d.UnitZ);
	//            GL.Begin(PrimitiveType.Lines);
	//            {
	//                GL.Vertex2(-0.0,-0.0);
	//                GL.Vertex2(+0.3,+0.0);
	//            }
	//            GL.End();
	//            GL.PopMatrix();
	//        }
	//        public static void Text(VisualObject iVisual)
	//        {
	//            GL.Enable(EnableCap.Texture2D);
	//            GL.Begin(PrimitiveType.Quads);
	//            {
	//                GL.TexCoord2(0,1); GL.Vertex2(-0.5,-0.5);
	//                GL.TexCoord2(1,1); GL.Vertex2(+0.5,-0.5);
	//                GL.TexCoord2(1,0); GL.Vertex2(+0.5,+0.5);
	//                GL.TexCoord2(0,0); GL.Vertex2(-0.5,+0.5);
	//            }
	//            GL.End();
	//            GL.Disable(EnableCap.Texture2D);
	//        }
	//    }	
	//}
}
