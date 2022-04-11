using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Text;
using WF = System.Windows.Forms;
using System.IO;


namespace AE.Visualization
{
	public partial class GLCanvasControl : OpenTK.GLControl, ICanvasControl
	{
		public Canvas Canvas {get;set;}
		//public float          GammaCorrection = 0.1f;
		//public ColorPalette   Palette;
		
		public static FontInfo       FontData;
		public static FontGlyphAtlas LowResFontAtlas;
		public static FontGlyphAtlas HighResFontAtlas;

		public static bool IsStaticInitialized = false;

		///public GLFrame        Frame;
		///public GLFrame        MainFrame;
		///public GLFrame        RecentNonMainFrame;
		///public GLFrame        ActiveFrame;
		
		///public MouseDragmeter Dragmeter;
		
		//public TextureAtlas   FramesAtlas;
		//public int            FramesTextureID    = -1;
		//public List<int>      TexIDs = new List<int>();
		//public Random         RNG = new Random();

		

		//public Screen() : this(true)
		//{}
		public VertexText TestText;

		public int TestTexSize;
		public int TestTexId;


		//static Canvas()
		//{
			
		//}
		public GLCanvasControl()
		{
			this.Canvas = new Canvas(this, new ColorPalette());
			///Workarounds.Screen = this;

			//this.Canvas.Palette = new ColorPalette();
			
			
			//this.Parent.client
			//this.MouseO
			//this.KeyPress
			//this.Frame = new RootFrame{Name = "RootFrame", Palette = new GdiColorPalette(), Screen = this, Dock = DockStyle.Fill};
			//{
				
			//}
			
		


			///this.FramesAtlas = new TextureAtlas();
			//this.BackColor   = Color.White;
			//this.Dragmeter = new MouseDragmeter();
			
		}

		public virtual void PrepareFrame(Frame iFrame)
		{
			Routines.Rendering.PrepareFrame(iFrame);
		}
		public virtual void RenderFrame(Frame iFrame)
		{
			Routines.Rendering.RenderFrame(iFrame);
		}
		//public virtual void SetActiveFrame(Frame iFrame)
		//{
		//    this.ActiveFrame = iFrame;
		//}
		public virtual string GetPrimaryStatusData()
		{
			var oStr = "";
			{
				oStr +=
				(
					"FPS: "      + this.Canvas.AverageFrameRate.ToString("F02") + " | " + 
					"ScrSize: "  + this.Width + "x" + this.Height + " | " +
					"Blending: " +
					((OpenTK.Graphics.OpenGL.BlendingFactorSrc)this.BlendingModes[this.SrcBlend]).ToString() + "," + 
					((OpenTK.Graphics.OpenGL.BlendingFactorDest)this.BlendingModes[this.DstBlend]).ToString()
				);


				//var _ = (G.Application as Studio);// as ZoomableFrame
				//if(Workarounds.NGonSchemeFrame != null)
				//{
				//    _StatusText += 
				//    (
				//        " | " + 

				//        "Viewpoint: " +
				//        "{" +
				//            Workarounds.NGonSchemeFrame.Viewpoint.CurrentState.Position.X.ToString("F03") + " " + 
				//            Workarounds.NGonSchemeFrame.Viewpoint.CurrentState.Position.Y.ToString("F03") + " " + 
				//            Workarounds.NGonSchemeFrame.Viewpoint.CurrentState.Position.Z.ToString("F03") + " " + 
				//        "}" +

				//        " | " + 

				//        "Pointer: " +
				//        "{" +
				//            Workarounds.NGonSchemeFrame.Scheme.Pointer.X.ToString("F03") + " " + 
				//            Workarounds.NGonSchemeFrame.Scheme.Pointer.Y.ToString("F03") + " " + 
				//            Workarounds.NGonSchemeFrame.Scheme.Pointer.Z.ToString("F03") + " " + 
				//        "}" 
				//    );
				//}
			}
			return oStr;
		}



		static GLCanvasControl()
		{
			FontData = new FontInfo();


			LowResFontAtlas  = new FontGlyphAtlas(null, 10f, 1024, 64);
			{
				LowResFontAtlas.CharWidth  = 8f;
				LowResFontAtlas.LineHeight = 15f;
				//this.LowResFontAtlas.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			}
			HighResFontAtlas = new FontGlyphAtlas(null, 48f, 1024, 16);
			{
				HighResFontAtlas.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				HighResFontAtlas.IsTexSmoothEnabled = true;
				HighResFontAtlas.IsMipmapEnabled = true;
			}
			///OnLoadStatic();
			
		}
		public static void OnLoadStatic()
		{
			if(GLCanvasControl.IsStaticInitialized) return;

			LowResFontAtlas.CreateImage();
			LowResFontAtlas.CreateTexture();
			
			HighResFontAtlas.CreateImage();
			HighResFontAtlas.CreateTexture();

			LowResFontAtlas.Image.Save("LowResFontAtlas.png");
			HighResFontAtlas.Image.Save("HighResFontAtlas.png");


			GLCanvasControl.IsStaticInitialized = true;

			//Screen.Routines.Rendering.InitGL(this);
		}
	}
	
}