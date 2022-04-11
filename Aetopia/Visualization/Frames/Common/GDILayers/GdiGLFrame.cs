using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using WF = System.Windows.Forms;

namespace AE.Visualization
{
	public partial class GdiGLFrame : GLFrame
	{
		public GraphicsLayer Background;
		public GraphicsLayer Foreground;
		

		public GdiGLFrame()
		{
			this.Foreground = new GraphicsLayer();
			this.Background = new GraphicsLayer();

			//this.Resize      += new GenericEventHandler(GdiGLFrame_Resize);
			//this.ThemeUpdate += new GenericEventHandler(GdiGLFrame_ThemeUpdate);
		}

		
		//override 
		protected override void OnResize(GenericEventArgs iEvent)
		{
			base.OnResize(iEvent);

			this.Invalidate(true);
		}
		protected override void OnThemeUpdate(GenericEventArgs iEvent)
		{
			base.OnThemeUpdate(iEvent);

			this.Invalidate(true);
		}
		//void GdiGLFrame_Resize(GenericEventArgs iEvent)
		//{
		//    this.Invalidate(true);
		//}
		//void GdiGLFrame_ThemeUpdate(GenericEventArgs iEvent)
		//{
		//    this.Invalidate(true);
		//    //throw new NotImplementedException();
		//}

		public override void UpdateBounds()
		{
			var _PrevSize  = this.Bounds.Size;

			base.UpdateBounds();

			if(_PrevSize != this.Bounds.Size)
			{
				if(this.Background != null) this.Background.UpdateSize(this.Width, this.Height);
				if(this.Foreground != null) this.Foreground.UpdateSize(this.Width, this.Height);
			}

			//var _HasArea = this.Width != 0 && this.Height != 0;

			

			//if(this.Background != null)
			//{
			//    this.Background.Image.Dispose();

			//    if(_HasArea)
			//    {
			//        this.Background.Image = new Bitmap(this.Width,this.Height);
			//    }
			//}
			//this.Background.TexSize = (int)Math.Pow(2, Math.Ceiling(Math.Log(Math.Max(this.Width, this.Height),2)));

			//this.IsStateChanged = true;
			
			
			//Screen.Routines.Rendering.DeleteTexture(this.Screen, this);
			///this.Viewport.IsNeedUpdateBounds = true;

			//this.RegenerateAtl
		}

		//public void UpdateTexture()
		//{
		//    Screen.Routines.Rendering.DeleteTexture(this.Screen, this);
		//    Screen.Routines.Rendering.GenerateTexture(this.Screen, this);
		//    //this.TexID = 
		//}

		public          void Invalidate()
		{
			this.Invalidate(false);
		}
		public          void Invalidate(bool iDoBackToo)
		{
			this.Foreground.Invalidate();
			
			if(iDoBackToo)
			{
				this.Background.Invalidate();
			}
		}

		public override void Render()
		{
			//this.UpdateGraphics();
			//base.Render();
			
			this.UpdateGraphics();

			if(this.Background != null)
			{
			    if(this.Background.CurrentPhase == GraphicsLayer.Phase.Synchronization) this.Background.UpdateTextureData();
			    if(this.Background.CurrentPhase == GraphicsLayer.Phase.Ready)           this.RenderLayer(this.Background);
			}
			if(this.Foreground != null)
			{
				if(this.Foreground.CurrentPhase == GraphicsLayer.Phase.Synchronization) this.Foreground.UpdateTextureData();
				if(this.Foreground.CurrentPhase == GraphicsLayer.Phase.Ready)           this.RenderLayer(this.Foreground);
			}
		}
		//public         void UploadLayerData(GraphicsLayer iLayer)
		//{
		//    var _Data = iLayer.Image.LockBits
		//    (
		//        new Rectangle(0, 0, _AbsW, _AbsH),
		//        //new Rectangle(0, 0, 1, 1),
		//        System.Drawing.Imaging.ImageLockMode.ReadOnly,
		//        //System.Drawing.Imaging.PixelFormat.Format32bppArgb
		//        System.Drawing.Imaging.PixelFormat.Format32bppArgb
		//    );
		//    //GL.BindTexture(TextureTarget.Texture2D, iViewport.TexIDs[iFrame.TexID]);
		//    GL.BindTexture(TextureTarget.Texture2D, iLayer.TexID);
			
		//    //if(DateTime.Now.Millisecond < 100)
		//    //if(new Random().NextDouble() > 0.8)
		//    GL.TexSubImage2D
		//    (
		//        TextureTarget.Texture2D, 0,
		//        0,0, _AbsW, _AbsH,
		//        //iFrame.Image.Width, iFrame.Image.Height,
		//        //PixelFormat.Bgra,
		//        PixelFormat.Bgra,
		//        //PixelFormat.Rgba,
		//        PixelType.UnsignedByte, _Data.Scan0
		//        //PixelType.UnsignedInt8888Reversed, _Data.Scan0
		//    );
		//    iLayer.Image.UnlockBits(_Data);
		//}
		public virtual void RenderLayer(GraphicsLayer iLayer)
		{
			if(iLayer.TexID == -1) throw new Exception();

			//if(iLayer.CurrentPhase == GraphicsLayer.Phase.Synchronization)
			//{
				
			//}

			var   _TexSize = iLayer.TexSize;
			//var _RelTexPos    = new PointF((float)_AtlasRegion.X     / _AtlasSize, (float)_AtlasRegion.Y      / _AtlasSize);
			//var _RelTexSize   = new SizeF ((float)iFrame.Width / _TexSize, (float)iFrame.Height / _TexSize);
			//var _RelTexBounds = new RectangleF(_RelTexPos, _RelTexSize);
			int   _AbsW = iLayer.Image.Width, _AbsH = iLayer.Image.Height;
			float _RelW = (float)_AbsW  / _TexSize, _RelH = (float)_AbsH / _TexSize;

			//if(DateTime.Now.Millisecond < 100)
			//GL.Disable(EnableCap.Texture2D);
			//if(iViewport.RNG.NextDouble() > 0.9)
			//if(DateTime.Now.Millisecond % 100 > 90)
			//{
			//    //var _Region = iFrame.AtlasRegion;
			//    var _Data = iLayer.Image.LockBits
			//    (
			//        new Rectangle(0, 0, _AbsW, _AbsH),
			//        //new Rectangle(0, 0, 1, 1),
			//        System.Drawing.Imaging.ImageLockMode.ReadOnly,
			//        //System.Drawing.Imaging.PixelFormat.Format32bppArgb
			//        System.Drawing.Imaging.PixelFormat.Format32bppArgb
			//    );
			//    //GL.BindTexture(TextureTarget.Texture2D, iViewport.TexIDs[iFrame.TexID]);
			//    GL.BindTexture(TextureTarget.Texture2D, iLayer.TexID);
				
			//    //if(DateTime.Now.Millisecond < 100)
			//    //if(new Random().NextDouble() > 0.8)
			//    GL.TexSubImage2D
			//    (
			//        TextureTarget.Texture2D, 0,
			//        0,0, _AbsW, _AbsH,
			//        //iFrame.Image.Width, iFrame.Image.Height,
			//        //PixelFormat.Bgra,
			//        PixelFormat.Bgra,
			//        //PixelFormat.Rgba,
			//        PixelType.UnsignedByte, _Data.Scan0
			//        //PixelType.UnsignedInt8888Reversed, _Data.Scan0
			//    );
			//    iLayer.Image.UnlockBits(_Data);
			//}

			//var _S = iFrame.Bounds.Size;
			
			
			GL.Enable(EnableCap.Texture2D);
			GL.BindTexture(TextureTarget.Texture2D, iLayer.TexID);
			GL.Begin(PrimitiveType.Quads);
			{
				GL.Color4(1.0,1.0,1.0,this.Opacity);
				
				GL.TexCoord2(    0,     0); GL.Vertex2(0.0,     0.0);
				GL.TexCoord2(_RelW,     0); GL.Vertex2(_AbsW,   0.0);
				GL.TexCoord2(_RelW, _RelH); GL.Vertex2(_AbsW, _AbsH);
				GL.TexCoord2(    0, _RelH); GL.Vertex2(0.0,   _AbsH);

				//GL.TexCoord2(    0,     0); GL.Vertex2(0, 0);
				//GL.TexCoord2(_RelW,     0); GL.Vertex2(1, 0);
				//GL.TexCoord2(_RelW, _RelH); GL.Vertex2(1, 1);
				//GL.TexCoord2(    0, _RelH); GL.Vertex2(0, 1);
			}
			GL.End();
			GL.Disable(EnableCap.Texture2D);

			//GL.Begin(PrimitiveType.LineLoop);
			//{
			//    GL.Color4(0.0,1.0,0.0,1.0);
				
			//    GL.Vertex2(0.0,     0.0);
			//    GL.Vertex2(_AbsW,   0.0);
			//    GL.Vertex2(_AbsW, _AbsH);
			//    GL.Vertex2(0.0,   _AbsH);
			//}
			//GL.End();
		}
		public virtual void UpdateGraphics()
		{
			if(this.Background != null && this.Background.CurrentPhase == GraphicsLayer.Phase.Drawing)
			{
			    this.DrawBackground(new GraphicsContext(this.Background.Image, this.Palette));
			    this.Background.SetProcessingPhase(GraphicsLayer.Phase.Synchronization);
			}
			if(this.Foreground != null && this.Foreground.CurrentPhase == GraphicsLayer.Phase.Drawing)
			{
				this.DrawForeground(new GraphicsContext(this.Foreground.Image, this.Palette));
				this.Foreground.SetProcessingPhase(GraphicsLayer.Phase.Synchronization);
			}
			
			//if(this.Background != null)
			//{
			//    var _BackCtx = new GraphicsContext();
			//    {
			//        _BackCtx.BindImage(this.Background.Image);
			//        _BackCtx.BindPalette(this.Palette);
			//    }
			//    this.DrawBackground(_BackCtx);
			//}
			//if(this.Foreground != null)
			//{
			//    var _ForeCtx = new GraphicsContext();
			//    {
			//        _ForeCtx.BindImage(this.Foreground.Image);
			//        _ForeCtx.BindPalette(this.Palette);
			//    }
			//    this.DrawForeground(_ForeCtx);
			//}
		}

		
		public override string ToString()
		{
			return "GdiGLFrame '" + this.Name + "'";
		}
	}
}
