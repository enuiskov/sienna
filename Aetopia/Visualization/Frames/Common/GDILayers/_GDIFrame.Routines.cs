using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sienna.Editor;
using WF = System.Windows.Forms;

namespace Sienna.BlackTorus
{
	public partial class GDIFrame : Frame
	{
		public struct Routines
		{
			public class Rendering
			{

				//public static void DrawBackground           (Viewport iViewport, Bitmap iBitmap, Color iColor, double iX, double iY, double iAngle, double iScale)
				//{

				//}
				//pub
				public static void Draw                     (Screen iViewport, GDIFrame iFrame)
				{
					if(iFrame.TexID == -1) iFrame.UpdateTexture();
					
					///iFrame.UpdateGraphics();
					//if(DateTime.Now.Millisecond > 990) iFrame.UpdateGraphics();


					//GL.Scale(iScale,iScale,1.0);
					var _B = iFrame.Bounds;

					
					GL.PushMatrix();
					GL.Translate(_B.X,_B.Y,0.0);
					{
						if(iFrame.Image != null) DrawBackground(iViewport, iFrame);
						if(iFrame.Image != null) DrawForeground(iViewport, iFrame);

						foreach(var cChildF in iFrame.Children)
						{
							Draw(iViewport, cChildF);
						}
					}
					GL.PopMatrix();
				}
				public static void DrawBackground           (Screen iViewport, GDIFrame iFrame)
				{
					//return;
					var _S = iFrame.Bounds.Size;

					GL.Begin(PrimitiveType.LineLoop);
					{
					    GL.Color4(iFrame.Palette.ForeColor);

					    GL.Vertex2(0.0,      0.0);
					    GL.Vertex2(_S.Width, 0.0);
					    GL.Vertex2(_S.Width, _S.Height);
					    GL.Vertex2(0.0,      _S.Height);
					}
					GL.End();

					GL.Begin(PrimitiveType.Quads);
					{
						//GL.Color4(Color.FromArgb((int)(255 * iFrame.Opacity), (iFrame.Parent != null ? iFrame.Parent.Palette.BackColor : iViewport.BackColor)));
						GL.Color4(iFrame.Palette.BackGradTopColor);
					    GL.Vertex2(0.0,      0.0);
					    GL.Vertex2(_S.Width, 0.0);

					    GL.Color4(iFrame.Palette.BackGradBottomColor);
					    GL.Vertex2(_S.Width, _S.Height);
					    GL.Vertex2(0.0,      _S.Height);
					}
					GL.End();
				}
				public static void DrawForeground           (Screen iViewport, GDIFrame iFrame)
				{
					if(iFrame.TexID == -1) throw new Exception();

					var   _TexSize = iFrame.TexSize;
					//var _RelTexPos    = new PointF((float)_AtlasRegion.X     / _AtlasSize, (float)_AtlasRegion.Y      / _AtlasSize);
					//var _RelTexSize   = new SizeF ((float)iFrame.Width / _TexSize, (float)iFrame.Height / _TexSize);
					//var _RelTexBounds = new RectangleF(_RelTexPos, _RelTexSize);
					int   _AbsW = iFrame.Width, _AbsH = iFrame.Height;
					float _RelW = (float)iFrame.Width  / _TexSize, _RelH = (float)iFrame.Height / _TexSize;

					//if(DateTime.Now.Millisecond < 100)
					//GL.Disable(EnableCap.Texture2D);
					//if(iViewport.RNG.NextDouble() > 0.9)
					//if(DateTime.Now.Millisecond % 100 > 90)
					{
					    //var _Region = iFrame.AtlasRegion;
					    var _Data = iFrame.Image.LockBits
					    (
					        new Rectangle(0, 0, _AbsW, _AbsH),
					        //new Rectangle(0, 0, 1, 1),
					        System.Drawing.Imaging.ImageLockMode.ReadOnly,
					        //System.Drawing.Imaging.PixelFormat.Format32bppArgb
					        System.Drawing.Imaging.PixelFormat.Format32bppArgb
					    );
						//GL.BindTexture(TextureTarget.Texture2D, iViewport.TexIDs[iFrame.TexID]);
						GL.BindTexture(TextureTarget.Texture2D, iFrame.TexID);
						
						//if(DateTime.Now.Millisecond < 100)
					    GL.TexSubImage2D
					    (
					        TextureTarget.Texture2D, 0,
					        0,0, _AbsW, _AbsH,
					        //iFrame.Image.Width, iFrame.Image.Height,
					        //PixelFormat.Bgra,
					        PixelFormat.Bgra,
					        //PixelFormat.Rgba,
					        PixelType.UnsignedByte, _Data.Scan0
					        //PixelType.UnsignedInt8888Reversed, _Data.Scan0
					    );
					    iFrame.Image.UnlockBits(_Data);
					}

					//var _S = iFrame.Bounds.Size;
					
					

					GL.Enable(EnableCap.Texture2D);
					GL.Begin(PrimitiveType.Quads);
					{
						GL.Color4(1.0,1.0,1.0,1.0);
						
						GL.TexCoord2(    0,     0); GL.Vertex2(0.0,     0.0);
						GL.TexCoord2(_RelW,     0); GL.Vertex2(_AbsW,   0.0);
						GL.TexCoord2(_RelW, _RelH); GL.Vertex2(_AbsW, _AbsH);
						GL.TexCoord2(    0, _RelH); GL.Vertex2(0.0,   _AbsH);
					}
					GL.End();
					GL.Disable(EnableCap.Texture2D);
				}

				#region Simple
				//public static void DrawForeground           (Viewport iViewport, Frame iFrame)
				//{
				//    if(iFrame.TexID == -1) throw new Exception();

				//    var   _TexSize = iFrame.TexSize;
				//    //var _RelTexPos    = new PointF((float)_AtlasRegion.X     / _AtlasSize, (float)_AtlasRegion.Y      / _AtlasSize);
				//    //var _RelTexSize   = new SizeF ((float)iFrame.Width / _TexSize, (float)iFrame.Height / _TexSize);
				//    //var _RelTexBounds = new RectangleF(_RelTexPos, _RelTexSize);
				//    int   _AbsW = iFrame.Width, _AbsH = iFrame.Height;
				//    float _RelW = (float)iFrame.Width  / _TexSize, _RelH = (float)iFrame.Height / _TexSize;

				//    //if(DateTime.Now.Millisecond < 100)
				//    //GL.Disable(EnableCap.Texture2D);
				//    //if(iViewport.RNG.NextDouble() > 0.9)
				//    //if(DateTime.Now.Millisecond % 100 > 90)
				//    {
				//        //var _Region = iFrame.AtlasRegion;
				//        var _Data = iFrame.Image.LockBits
				//        (
				//            new Rectangle(0, 0, _AbsW, _AbsH),
				//            //new Rectangle(0, 0, 1, 1),
				//            System.Drawing.Imaging.ImageLockMode.ReadOnly,
				//            //System.Drawing.Imaging.PixelFormat.Format32bppArgb
				//            System.Drawing.Imaging.PixelFormat.Format32bppArgb
				//        );
				//        //GL.BindTexture(TextureTarget.Texture2D, iViewport.TexIDs[iFrame.TexID]);
				//        GL.BindTexture(TextureTarget.Texture2D, iFrame.TexID);
						
				//        //if(DateTime.Now.Millisecond < 100)
				//        GL.TexSubImage2D
				//        (
				//            TextureTarget.Texture2D, 0,
				//            0,0, _AbsW, _AbsH,
				//            //iFrame.Image.Width, iFrame.Image.Height,
				//            //PixelFormat.Bgra,
				//            PixelFormat.Bgra,
				//            //PixelFormat.Rgba,
				//            PixelType.UnsignedByte, _Data.Scan0
				//            //PixelType.UnsignedInt8888Reversed, _Data.Scan0
				//        );
				//        iFrame.Image.UnlockBits(_Data);
				//    }

				//    //var _S = iFrame.Bounds.Size;
					
					

				//    GL.Enable(EnableCap.Texture2D);
				//    GL.Begin(PrimitiveType.Quads);
				//    {
				//        GL.Color4(1.0,1.0,1.0,1.0);
						
				//        GL.TexCoord2(    0,     0); GL.Vertex2(0.0,     0.0);
				//        GL.TexCoord2(_RelW,     0); GL.Vertex2(_AbsW,   0.0);
				//        GL.TexCoord2(_RelW, _RelH); GL.Vertex2(_AbsW, _AbsH);
				//        GL.TexCoord2(    0, _RelH); GL.Vertex2(0.0,   _AbsH);
				//    }
				//    GL.End();
				//    GL.Disable(EnableCap.Texture2D);
				//}
				#endregion
				#region ATLAS
				/**
				//public static void DrawForeground_ATLAS           (Viewport iViewport, Frame iFrame)
				//{
				//    if(iFrame.TexID       == -1) throw new Exception();
				//    if(iFrame.AtlasRegion == -1) return;

				//    var _AtlasRegion  = iViewport.FramesAtlas.Regions[iFrame.AtlasRegion].Bounds;
				//    var _AtlasSize    = iViewport.FramesAtlas.TestSize;
				//    var _RelTexPos    = new PointF((float)_AtlasRegion.X     / _AtlasSize, (float)_AtlasRegion.Y      / _AtlasSize);
				//    var _RelTexSize   = new SizeF ((float)_AtlasRegion.Width / _AtlasSize, (float)_AtlasRegion.Height / _AtlasSize);
				//    var _RelTexBounds = new RectangleF(_RelTexPos, _RelTexSize);
					

				//    //if(DateTime.Now.Millisecond < 100)
				//    //GL.Disable(EnableCap.Texture2D);
				//    //if(iViewport.RNG.NextDouble() > 0.9)
				//    //if(DateTime.Now.Millisecond % 100 > 90)
				//    {
				//        //var _Region = iFrame.AtlasRegion;
				//        var _Data = iFrame.Image.LockBits
				//        (
				//            new Rectangle(0, 0, iFrame.Image.Width, iFrame.Image.Height),
				//            //new Rectangle(0, 0, 1, 1),
				//            System.Drawing.Imaging.ImageLockMode.ReadOnly,
				//            //System.Drawing.Imaging.PixelFormat.Format32bppArgb
				//            System.Drawing.Imaging.PixelFormat.Format32bppArgb
				//        );
				//        //GL.BindTexture(TextureTarget.Texture2D, iViewport.TexIDs[iFrame.TexID]);
				//        GL.BindTexture(TextureTarget.Texture2D, iFrame.TexID);
						
				//        //if(DateTime.Now.Millisecond < 100)
				//        GL.TexSubImage2D
				//        (
				//            TextureTarget.Texture2D, 0,
				//            _AtlasRegion.X,     _AtlasRegion.Y,
				//            _AtlasRegion.Width, _AtlasRegion.Height,
				//            //iFrame.Image.Width, iFrame.Image.Height,
				//            //PixelFormat.Bgra,
				//            PixelFormat.Bgra,
				//            //PixelFormat.Rgba,
				//            PixelType.UnsignedByte, _Data.Scan0
				//            //PixelType.UnsignedInt8888Reversed, _Data.Scan0
				//        );
				//        iFrame.Image.UnlockBits(_Data);
				//    }

				//    var _S = iFrame.Bounds.Size;
					
					

				//    GL.Enable(EnableCap.Texture2D);
				//    GL.Begin(PrimitiveType.Quads);
				//    {
				//        GL.Color4(1.0,1.0,1.0,1.0);
						
				//        GL.TexCoord2(_RelTexBounds.Left,  _RelTexBounds.Top);    GL.Vertex2(0.0,      0.0);
				//        GL.TexCoord2(_RelTexBounds.Right, _RelTexBounds.Top);    GL.Vertex2(_S.Width, 0.0);
				//        GL.TexCoord2(_RelTexBounds.Right, _RelTexBounds.Bottom); GL.Vertex2(_S.Width, _S.Height);
				//        GL.TexCoord2(_RelTexBounds.Left,  _RelTexBounds.Bottom); GL.Vertex2(0.0,      _S.Height);
				//    }
				//    GL.End();
				//    GL.Disable(EnableCap.Texture2D);
				//}
				*/
				#endregion
			}
		}
	}
}
