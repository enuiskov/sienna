using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Text;
using System.Windows.Forms;
using OpenTK;
//using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using AE.Visualization.SchemeObjectModel;

namespace AE.Visualization
{
	public partial class ZoomableFrame : GLFrame
	{
		public partial struct Routines
		{
			public class Control
			{
				//public static T GetHoverObject(T iObj)
				//{
				//    if(iObj.IsPointerOver) return iObj;


				//    if(iObj is Node)
				//    {
				//        //var _Node = ;

				//        foreach(var cChildO in (iObj as Node).Children)
				//        {
				//            var cHoverO = GetHoverObject(cChildO);

				//            if(cHoverO != null)
				//            {
				//                return cHoverO;
				//            }
				//            else continue;
				//        }
				//    }
				//    return null;
				//    //for(var cChild in 
				//    //iViewport.Scheme.Children.ForEach(.IsMouseOver
				//    //throw new NotImplementedException();

				//}
				//public static void EnumObjects
			}
			public class Rendering
			{
				//public static void InitGL(SchemeFrame iViewport) 
				//{
				//    int _MaxTexSize = 512;//2048;//GL.GetInteger(GetPName.MaxTextureSize);

				//    iViewport.SchemeTextureID = GL.GenTexture();
				//    {
				//        GL.BindTexture  (TextureTarget.Texture2D, iViewport.SchemeTextureID);
				//        GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _MaxTexSize, _MaxTexSize, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
				//        //GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
				//        GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);

				//        //GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, (int)All.True);

				//        //glTexParameteri(GL_TEXTURE_2D, GL_GENERATE_MIPMAP, GL_TRUE );
				//    }
				//}
				
				public static void SetProjectionMatrix (ZoomableFrame iFrame) 
				{
					GL.MatrixMode(MatrixMode.Projection);
					//GL.LoadIdentity();

					var _Viewpoint = iFrame.Viewpoint.CurrentState;
					var _ProjMat = iFrame.IsPerspectiveMode ? _Viewpoint.PerspProjMatrix : _Viewpoint.OrthoProjMatrix;
					
					GL.LoadMatrix(ref _ProjMat);
					
					if(iFrame.IsPerspectiveMode)
					{
						var _LookAtMat = _Viewpoint.PerspLookAtMatrix;
						GL.MultMatrix(ref _LookAtMat);
						///GL.MultMatrix(ref _LookAtMat);
					}

					GL.MatrixMode(MatrixMode.Modelview);
					GL.LoadIdentity();
				}
				//public static void SetOrthographicMatrix (ZoomableFrame iFrame) 
				//{
				//    GL.MatrixMode(MatrixMode.Projection);
				//    GL.LoadIdentity();

				//    var _ProjMat = iFrame.Viewpoint.CurrentState.OrthoMatrix;
				//    GL.LoadMatrix(ref _ProjMat);
					
				//    GL.MatrixMode(MatrixMode.Modelview);
				//    GL.LoadIdentity();
				//}
				//public static void SetPerspectiveMatrix  (ZoomableFrame iFrame) 
				//{
				//    var _Viewpoint = iFrame.Viewpoint.CurrentState;
				//    var _ViewP     = _Viewpoint.Position;
				//    var _ViewI     = _Viewpoint.Inclination;
				//    var _ScaleF    = 1.0 / _ViewP.Z;// * 0.5;
				//    var _AspectR   = iFrame.AspectRatio;
					

				//    GL.MatrixMode(MatrixMode.Projection);
				//    {
				//        Matrix4 _PerspMat = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)iFrame.Width / iFrame.Height, 0.001f, 6400);
				//        GL.LoadMatrix(ref _PerspMat);
				//    }
				//    GL.MatrixMode(MatrixMode.Modelview);
				//    {
				//        var _Dist = 1.0 / _ViewP.Z;
				//        var _TgtPos = new Vector3d(_ViewP.X, _ViewP.Y, 0.0);
				//        var _EyePos = _TgtPos + new Vector3d(0.0,-_Dist,_Dist);
				//        //var _EyePos = new Vector3d(_ViewP.X;

				//        Matrix4d _LookAtMat = Matrix4d.LookAt(_EyePos, _TgtPos, Vector3d.UnitZ);
				//        GL.LoadMatrix(ref _LookAtMat);
				//    }
				//}
				public static void Draw                (ZoomableFrame iFrame) 
				{	
					//SetOrthographicMatrix(iFrame);
					SetProjectionMatrix(iFrame);

					//GL.Enable(EnableCap.LineSmooth);
					GL.LineWidth(1);
					
					DrawUnitSpace(iFrame);
					DrawPropeller(iFrame);


					//if(
					//if(
					//DrawTerrain(iFrame);
				}
			

				public static void DrawTest1           (ZoomableFrame iFrame) 
				{
					Random _RNG = new Random(256);

					for(var cI = 0; cI < 4; cI ++)
					{
						var cColor = Color.FromArgb(128,Math.Max(200,200 + _RNG.Next(55)),Math.Max(200,200 + _RNG.Next(55)),Math.Max(200,200 + _RNG.Next(55)));

						GL.Translate(_RNG.NextDouble() * 100000, _RNG.NextDouble() * 100000, 0.0);
						GL.Rotate(90, new Vector3d(0,0,1));
						///DrawImage      (iViewport, iViewport.Image, cColor, _RNG.NextDouble() * 100000, _RNG.NextDouble() * 100000, 0, (100 - cI) * 1000);
					}
					//DrawImage      (iViewport, iViewport.Image, -1,-1,0,1);
					//DrawImage      (iViewport, iViewport.Image,  0,-1,0,1);
					//DrawImage      (iViewport, iViewport.Image,  0,0,0,1);
					//DrawImage      (iViewport, iViewport.Image, -1,0,0,1);

				}
				
				public static void DrawImage           (ZoomableFrame iFrame, Bitmap iBitmap, Color iColor, double iX, double iY, double iAngle, double iScale)
				{
					
					//var Data;
					//if(DateTime.Now.Millisecond > 950)
					{
						var _Data = iBitmap.LockBits
						(
							new Rectangle(0, 0, iBitmap.Width, iBitmap.Height),
							//new Rectangle(0, 0, 1, 1),
							System.Drawing.Imaging.ImageLockMode.ReadOnly,
							System.Drawing.Imaging.PixelFormat.Format32bppArgb
						);
						///GL.BindTexture(TextureTarget.Texture2D,iViewport.SchemeTextureID);
						//GL.BindTexture(TextureTarget.Texture2D, iViewport.FramesTextureID);
						
						GL.TexSubImage2D
						(
							TextureTarget.Texture2D, 0, 0, 0,
							iBitmap.Width, iBitmap.Height,
							PixelFormat.Bgra, PixelType.UnsignedByte, _Data.Scan0
						);
						iBitmap.UnlockBits(_Data);
					}
					//var _
					
					
					GL.Enable(EnableCap.Texture2D);
					GL.PushMatrix();
					
					GL.Translate(iX,iY,0.0);
					GL.Scale(iScale,iScale,1.0);
					
					GL.Begin(PrimitiveType.Quads);
					{
						GL.Color4(1.0,1.0,1.0,1.0);
						//GL.Color4(0.0,0.5,0.0,1.0);
						///GL.Color4(iColor);
						///GL.Color3(iColor);
						//GL.DrawP

					   GL.TexCoord2(0.0, 0.0); GL.Vertex2(0.0, 0.0);
					   GL.TexCoord2(1.0, 0.0); GL.Vertex2(1.0, 0.0);
					   GL.TexCoord2(1.0, 1.0); GL.Vertex2(1.0, 1.0);
					   GL.TexCoord2(0.0, 1.0); GL.Vertex2(0.0, 1.0);

					   // GL.TexCoord2(0.0, 0.0); GL.Vertex2(0.0, 0.0);
					   //GL.TexCoord2(1.0/8, 0.0); GL.Vertex2(1.0, 0.0);
					   //GL.TexCoord2(1.0/8, 1.0/8); GL.Vertex2(1.0, 1.0);
					   //GL.TexCoord2(0.0, 1.0/8); GL.Vertex2(0.0, 1.0);
					}
					GL.End();
					
					GL.PopMatrix();
					GL.Disable(EnableCap.Texture2D);
				}
				//public static void DrawOverlay      (SchemeViewport iViewport)                         
				//{
				//   System.Drawing.Imaging.BitmapData data = OverlayBitmap.LockBits
				//   (
				//      new System.Drawing.Rectangle(0, 0, OverlayBitmap.Width, OverlayBitmap.Height),
				//      System.Drawing.Imaging.ImageLockMode.ReadOnly,
				//      System.Drawing.Imaging.PixelFormat.Format32bppArgb
				//   );
				//   GL.TexSubImage2D
				//   (
				//      TextureTarget.Texture2D, 0, 0, 0,
				//      OverlayBitmap.Width, OverlayBitmap.Height,
				//      PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0
				//   );
				//   OverlayBitmap.UnlockBits(data);

				//   Matrix4 text_projection = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, -1, 1);
				//   GL.MatrixMode(MatrixMode.Projection);
				//   GL.LoadMatrix(ref text_projection);
				//   GL.MatrixMode(MatrixMode.Modelview);
				//   GL.LoadIdentity();

				//   //GL.Color4(Color4.White);
				//   GL.Color4(Color4.Bisque);
				//   GL.Enable(EnableCap.Texture2D);

				//   //GL.Translate(0,0,+2); ///~~!!!!!!!!;

				//   GL.Begin(PrimitiveType.Quads);
				//   {
				//      GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
				//      GL.TexCoord2(1, 0); GL.Vertex2(OverlayBitmap.Width, 0);
				//      GL.TexCoord2(1, 1); GL.Vertex2(OverlayBitmap.Width, OverlayBitmap.Height);
				//      GL.TexCoord2(0, 1); GL.Vertex2(0, OverlayBitmap.Height);
				//   }
				//   GL.End();
				//   GL.Disable(EnableCap.Texture2D);
				//}
				public static void DrawPropeller       (ZoomableFrame iFrame) 
				{
					//GL.MatrixMode(MatrixMode.Projection);
					//GL.LoadIdentity();
					//GL.Ortho(0.0, 1.0, 0.0, 1.0, 0.0, 4.0);
					GL.PushMatrix();
					//GL.Translate(0.7,0.5,0);
					//GL.Translate(0.7,0.5,0);
					GL.Rotate(-(float)DateTime.Now.Millisecond / 1000 * 360,0,0,1);
					
					//GL.Translate(0.0,0.5,0);

					GL.Begin(PrimitiveType.Quads);
					{
						
						//GL.Color4(Color.Transparent);
						
						//GL.Color4(iFrame.Palette.ForeColor);
						//GL.Vertex2(+ 0.0, + 0.01);
						/////GL.Color4(iFrame.Palette.ForeColor);
						//GL.Vertex2(+ 0.0, - 0.01);
						//GL.Vertex2(+ 1.0,  - 0.3);
						//GL.Color4(Color.Transparent);
						//GL.Vertex2(+ 0.5,  + 0.3);


						GL.Color4(iFrame.Palette.ForeColor);
						GL.Vertex2(+ 0.0, + 0.01);
						GL.Vertex2(+ 0.0, - 0.01);
						GL.Vertex2(+ 1.0, - 0.01);
						GL.Vertex2(+ 1.0, + 0.01);

						//GL.Vertex2(- 0.05, - 0.01);
						//GL.Vertex2(- 0.05, + 0.01);
						//GL.Vertex2(+ 0.3,  + 0.01);
						//GL.Vertex2(+ 0.3,  - 0.01);
					}
					GL.End();
					GL.PopMatrix();

					//throw new NotImplementedException();

				}
				public static void DrawUnitSpace       (ZoomableFrame iFrame) 
				{
					//GL.Enable(EnableCap.LineSmooth);
					GL.LineWidth(1);
					GL.Color3(iFrame.Palette.GlareColor);


					//GL.Begin(PrimitiveType.LineLoop);
					//{
					//    GL.Vertex2(-1.0, -1.0);
					//    GL.Vertex2(-1.0, +1.0);
					//    GL.Vertex2(+1.0, +1.0);
					//    GL.Vertex2(+1.0, -1.0);

					//    GL.Vertex2(-1.0, -1.0);
					//    GL.Vertex2(-1.0, +1.0);
					//    GL.Vertex2(+1.0, +1.0);
					//    GL.Vertex2(+1.0, -1.0);


					//    GL.End();
					//}
					GL.Begin(PrimitiveType.Lines);
					{
						//GL.Vertex2( 0.0, -0.9); GL.Vertex2( 0.0, +0.9);
						//GL.Vertex2(-0.9,  0.0); GL.Vertex2(+0.9,  0.0);

						//GL.Vertex2( 0.0, -1.0); GL.Vertex2( 0.0, -0.8);
						//GL.Vertex2(-1.0,  0.0); GL.Vertex2(-0.8,  0.0);

						
						GL.Color4(Color.Green); GL.Vertex2( 0.0, +1.0); GL.Vertex2( 0.0, +0.7);
						GL.Color4(Color.Red);   GL.Vertex2(+1.0,  0.0); GL.Vertex2(+0.7,  0.0);
						
						
					}
					GL.End();
					
					//GL.Disable(EnableCap.LineSmooth);
				}
				public static void DrawViewpoint       (ZoomableFrame iFrame) 
				{
					var _ViewP = iFrame.Viewpoint.CurrentState.Position;

					//GL.Enable(EnableCap.LineSmooth);
					GL.LineWidth(2);
					GL.Color3(Color.FromArgb(255, 0,200,200));

					//GL.Begin(PrimitiveType.LineLoop);
					//{
					//   GL.Vertex2(-1.0, -1.0);
					//   GL.Vertex2(-1.0, +1.0);
					//   GL.Vertex2(+1.0, +1.0);
					//   GL.Vertex2(+1.0, -1.0);

					//   GL.End();
					//}
					GL.PushMatrix();
					GL.Translate(_ViewP.X, _ViewP.Y, 0);

					GL.Begin(PrimitiveType.Lines);
					{
						//GL.Vertex2( 0.0, -0.9); GL.Vertex2( 0.0, +0.9);
						//GL.Vertex2(-0.9,  0.0); GL.Vertex2(+0.9,  0.0);

						GL.Vertex2(-0.1, -0.1); GL.Vertex2(+0.1, +0.1);
						GL.Vertex2(-0.1, +0.1); GL.Vertex2(+0.1, -0.1);
						
						GL.End();
					}

					GL.Begin(PrimitiveType.LineLoop);
					{
						for(double cA = 0; cA < Math.PI * 2; cA += Math.PI / 10)
						{
							GL.Vertex2(Math.Sin(cA),Math.Cos(cA));
						}
						GL.End();
					}

					GL.PopMatrix();
					//GL.Disable(EnableCap.LineSmooth);
				}
				public static void DrawGrid            (ZoomableFrame iFrame, double iX, double iY, double iGridSize, double iGridStep) 
				{
					///iViewport.AspectRatio
					var _HSize = iGridSize;
					var _VSize = iGridSize;

					var _HStep = iGridStep;
					var _VStep = iGridStep;
					//var _Grid  = iDimensions;
					

					GL.PushMatrix();
					GL.Translate(iX,iY,0.0);


					GL.Begin(PrimitiveType.Points);
					{
						for(var cY = 0.0; cY < iGridSize; cY += _VStep)
						{
							for(var cX = 0.0; cX < iGridSize; cX += _HStep)
							{
								if(cY != 0.0)
								{
									if(cX != 0.0) GL.Vertex2(+cX,+cY);
									if(true)      GL.Vertex2(-cX,+cY);
								}

								if(cX != 0.0) GL.Vertex2(+cX,-cY);
								if(true)      GL.Vertex2(-cX,-cY);
							}
						}
						GL.End();
					}
					GL.PopMatrix();
				}
				//public static void DrawGrid          (SchemeViewport iViewport, double iX, double iY, double iDimensions, double iTileSize) 
				//{
				//    var _HStep = iTileSize;
				//    //var _VStep = _HStep * 0.5773502691896257;
				//    var _VStep = _HStep * 0.8660254037844387;
				//    var _Grid  = iDimensions;
					

				//    GL.PushMatrix();
				//    GL.Translate(iX,iY,0.0);


				//    GL.Begin(PrimitiveType.Points);
				//    {
				//        var cIsEvenRow = true; for(var cY = 0.0; cY < _Grid; cY += _VStep, cIsEvenRow =! cIsEvenRow)
				//        {
				//            for(var cX = cIsEvenRow ? 0.0 : _HStep / 2.0; cX < _Grid; cX += _HStep)
				//            {
				//                if(cY != 0.0)
				//                {
				//                    if(cX != 0.0) GL.Vertex2(+cX,+cY);
				//                    if(true)      GL.Vertex2(-cX,+cY);
				//                }

				//                if(cX != 0.0) GL.Vertex2(+cX,-cY);
				//                if(true)      GL.Vertex2(-cX,-cY);
				//            }
				//        }
				//        GL.End();
				//    }
				//    GL.PopMatrix();
				//}
				public static void DrawStaticGrid      (ZoomableFrame iViewport) 
				{
					GL.PointSize(2f);
					GL.Color4(0.0,0.0,0.0, 1.0);

					double _OffsX = 32.0, _OffsY = _OffsX;
					double _Size = _OffsX;// * 2;

					DrawGrid(iViewport, _OffsX, _OffsY, _Size,1.0);
				}
				//public static void DrawDynamicGrid     (SchemeViewport iViewport) 
				//{
				//    var _ViewP = new Vector2d(iViewport.Viewpoint.X,iViewport.Viewpoint.Y);

				//    GL.PointSize(5f);
				//    GL.Color4(0.0,0.7,1.0, 1.0);


				//    //_ViewP.X
				//    var _XStep = 1.0 / iViewport.TileScale;
				//    var _YStep = 1.0 / iViewport.TileScale;// * 0.5773502691896257;

				//    var _X = Math.Round(_ViewP.X * _XStep) / _XStep;
				//    var _Y = Math.Round(_ViewP.Y * _YStep) / _YStep;
				//    //var _X = Math.Round(_ViewP.X * _XStep) / _XStep;


				//    //var _LogZoom = Math.Floor(Math.Log(1.0 / iViewport.Viewpoint.Z, 10));//10;
				//    //var _Step    = (1.0 / 81) / Math.Pow(10, Math.Floor(_LogZoom));
				//    //    var _Length  = _Step * 100.0;
						


				//    var _GridSize =  Math.Floor(Math.Log(iViewport.Viewpoint.Z * 100.0, 10));
				//    var _GridStep = _GridSize / 10.0;//Math.Round(_Dimensions * 1.0) / 1.0;

				//    DrawGrid(iViewport, _X,_Y, _GridSize, _GridStep);
				//}
				//public static void DrawDynamicUnitGrid (SchemeViewport iViewport) 
				//{
				//    var _ViewP = new Vector2d(iViewport.Viewpoint.X,iViewport.Viewpoint.Y);

				//    GL.PointSize(5f);
				//    GL.Color4(0.0,0.7,1.0, 1.0);


				//    //_ViewP.X
				//    var _XStep = 1.0 / iViewport.TileScale;
				//    var _YStep = 1.0 / iViewport.TileScale;// * 0.5773502691896257;

				//    var _X = Math.Round(_ViewP.X * _XStep) / _XStep;
				//    var _Y = Math.Round(_ViewP.Y * _YStep) / _YStep;
				//    //var _X = Math.Round(_ViewP.X * _XStep) / _XStep;


				//    //var _LogZoom = Math.Floor(Math.Log(1.0 / iViewport.Viewpoint.Z, 10));//10;
				//    //var _Step    = (1.0 / 81) / Math.Pow(10, Math.Floor(_LogZoom));
				//    //    var _Length  = _Step * 100.0;
						


				//    //var _GridSize =  Math.Floor(Math.Log(iViewport.Viewpoint.Z * 100.0, 10));
				//    //var _GridStep = _GridSize / 10.0;//Math.Round(_Dimensions * 1.0) / 1.0;

				//    DrawGrid(iViewport, _X,_Y, 10.0, 1.0);
				//}
				
				//public static void DrawQuadCursor    (SchemeViewport iViewport) 
				//{
				//    var _HvrCell = iViewport.HoverCell;
				//    var _Size = 1.0;

				//    var _X = (double)_HvrCell.X;
				//    var _Y = (double)_HvrCell.Y;
					
					

				//    GL.Begin(PrimitiveType.LineLoop);
				//    {
				//        GL.Vertex2(_X,         _Y       );
				//        GL.Vertex2(_X,         _Y +_Size);
				//        GL.Vertex2(_X + _Size, _Y +_Size);
				//        GL.Vertex2(_X + _Size, _Y       );


				//        GL.End();
				//    }
				//}
				//public static void DrawCursor          (SchemeViewport iViewport) 
				//{
				//    var _MouP = new Vector2d(iViewport.Mouse.SX,iViewport.Mouse.SY);

				//    GL.PointSize(10f);
				//    GL.Color4(0.0,0.5,1.0, 1.0);


				//    var _GridSize = 1.0 / iViewport.TileScale;
				//    var _HalfT    = _GridSize / 2.0;
				//    //_ViewP.X
				//    //var _XBias = _YStep;
				//    var _XStep = 1.0 / _GridSize;
				//    var _YStep = 1.0 / _GridSize;

				//    //var _YStep = 2.0 / iViewport.TileScale * 0.5773502691896257;
				//    ///var _YStep = 1.0 / iViewport.TileScale * 0.5773502691896257;
					

				//    var _X = Math.Round(_MouP.X * _XStep) / _XStep;
				//    var _Y = Math.Round(_MouP.Y * _YStep) / _YStep;
				//    var _P = false;
				//    {
				//        //if(_MouP.X - _MouP.Y
				//        //if(_Y % 2.0 < 1.0)
				//        //{
				//        //    _X += 0.5;//Math_Y
				//        //}
				//    }
					



				//    //var _X = Math.Round(_ViewP.X * _XStep) / _XStep;


				//    ///DrawGrid(iViewport, _X,_Y, 2.0, iViewport.TileScale);

				//    GL.PointSize(10f);
				//    GL.Begin(PrimitiveType.Points);
				//    {
				//        GL.Vertex2(_X,_Y);
				//    }
				//    GL.End();


				//    GL.Begin(PrimitiveType.LineLoop);
				//    {
				//        GL.Vertex2(_X -_HalfT,_Y -_HalfT);
				//        GL.Vertex2(_X +_HalfT,_Y -_HalfT);
				//        GL.Vertex2(_X +_HalfT,_Y +_HalfT);
				//        GL.Vertex2(_X -_HalfT,_Y +_HalfT);

				//        GL.End();
				//    }

				//    GL.Color4(1.0,0.0,0.0,1.0);
				//    GL.Begin(PrimitiveType.LineLoop);
				//    {
				//        if(true)
				//        {
				//            GL.Vertex2(_X -_HalfT,_Y -_HalfT);
				//            GL.Vertex2(_X +_HalfT,_Y -_HalfT);
				//            GL.Vertex2(_X -_HalfT,_Y +_HalfT);
				//        }
				//        else
				//        {
				//            GL.Vertex2(_X +_HalfT,_Y +_HalfT);
				//            GL.Vertex2(_X -_HalfT,_Y +_HalfT);
				//            GL.Vertex2(_X +_HalfT,_Y -_HalfT);
				//        }

				//        GL.End();
				//    }
				//}
				public static void DrawMap             (ZoomableFrame iFrame) 
				{
					var _GridSize = 1.0;// / iViewport.TileScale;
					var _HalfT    = _GridSize / 2.0;
					

					GL.Begin(PrimitiveType.Quads);
					{
						for(var cY = -32; cY < 32; cY++)
						{
							for(var cX = -32; cX < 32; cX++)
							{
								//if(iViewport.
								//GL.Color4(255,0,0,255);

								GL.Vertex2(cX -_HalfT,cY -_HalfT);
								GL.Vertex2(cX +_HalfT,cY -_HalfT);
								GL.Vertex2(cX +_HalfT,cY +_HalfT);
								GL.Vertex2(cX -_HalfT,cY +_HalfT);

								
							}
						}
						GL.End();
					}

					//GL.Color4(1.0,0.0,0.0,1.0);
					//GL.Begin(PrimitiveType.LineLoop);
					//{
					//    if(true)
					//    {
					//        GL.Vertex2(_X -_HalfT,_Y -_HalfT);
					//        GL.Vertex2(_X +_HalfT,_Y -_HalfT);
					//        GL.Vertex2(_X -_HalfT,_Y +_HalfT);
					//    }
					//    else
					//    {
					//        GL.Vertex2(_X +_HalfT,_Y +_HalfT);
					//        GL.Vertex2(_X -_HalfT,_Y +_HalfT);
					//        GL.Vertex2(_X +_HalfT,_Y -_HalfT);
					//    }

					//    GL.End();
					//}
				}
				
				public static void DrawPointer         (ZoomableFrame iFrame) 
				{
					var _X = iFrame.Pointer.X;
					var _Y = iFrame.Pointer.Y;
				
					GL.PointSize(5f);
					GL.Color4(0.0,0.5,1.0, 1.0);

					GL.Begin(PrimitiveType.Lines);
					{
						GL.Vertex2(-10.0, _Y);
						GL.Vertex2(+10.0, _Y);

						GL.Vertex2(_X, -10.0);
						GL.Vertex2(_X, +10.0);
					}
					GL.End();

					//GL.Begin(PrimitiveType.LineLoop);
					//{
					//    var _HStep   = 1.0;
					//    var _HOffs   = _HStep * 0.25;
					//    var _HMiddle = _HStep * 0.5;

					//    var _VStep   = _HStep * 2.0 * 0.8660254037844387;// *2.0;
					//    var _VOffs   = _VStep * 0.0;//25;
					//    var _VMiddle = _VStep * 0.5;//5;
		
					//    var _StepA   = 2.0943951023931953;  //pi / 1.5
					//    var _MaxA    = 6.283185307179586;   //pi * 2.0
					//    var _Radius  = 0.5773502691896257;  //0.5 / cos(30); tan(30)
					//    var _InvA    = 3.141592653589793;   //pi
						
						

					//    var _IsInvX = (_HOffs + Math.Abs(_X)) % _HStep > _HMiddle;

					//    var _IsInvY = (_VOffs + Math.Abs(_Y)) % _VStep > _VMiddle;
					//    //var _IsInvY = false;///(_VOffs + Math.Abs(_Y)) % _VStep > _VMiddle;
					//    var _BiasA  = _IsInvX ^ _IsInvY ? _InvA : 0.0; //pi / 3;

					//    for(double cA = 0.0; cA < _MaxA; cA += _StepA)
					//    {
					//        GL.Vertex2
					//        (
					//            _X - (Math.Sin(cA + _BiasA) * _Radius),//(0.5 * Math.Sin(_TriaA)),
					//            _Y - (Math.Cos(cA + _BiasA) * _Radius)// * (0.5 * Math.Sin(_TriaA))
					//        );
					//    }
					//}
					//GL.End();

					
				}
				//public static void DrawPointer         (ZoomableFrame iFrame) 
				//{
				//    var _X = iFrame.Pointer.X;
				//    var _Y = iFrame.Pointer.Y;
				
				//    GL.PointSize(5f);
				//    GL.Color4(0.0,0.5,1.0, 1.0);

				//    ///DrawGrid(iFrame, _X,_Y, 5.0,1.0);

				//    ///return;
				//    //GL.MatrixMode(MatrixMode.Projection);
				//    //GL.LoadIdentity();
				//    //GL.Ortho(0.0, 1.0, 0.0, 1.0, 0.0, 4.0);
				//    //GL.Translate(0,0,+0.1);
				//    GL.Begin(PrimitiveType.Lines);
				//    {
				//        GL.Vertex2(-10.0, _Y);
				//        GL.Vertex2(+10.0, _Y);

				//        GL.Vertex2(_X, -10.0);
				//        GL.Vertex2(_X, +10.0);
				//    }
				//    GL.End();

				//    GL.Begin(PrimitiveType.LineLoop);
				//    {
				//        var _HStep   = 1.0;
				//        var _HOffs   = _HStep * 0.25;
				//        var _HMiddle = _HStep * 0.5;

				//        var _VStep   = _HStep * 2.0 * 0.8660254037844387;// *2.0;
				//        var _VOffs   = _VStep * 0.0;//25;
				//        var _VMiddle = _VStep * 0.5;//5;
		
				//        var _StepA   = 2.0943951023931953;  //pi / 1.5
				//        var _MaxA    = 6.283185307179586;   //pi * 2.0
				//        var _Radius  = 0.5773502691896257;  //0.5 / cos(30); tan(30)
				//        var _InvA    = 3.141592653589793;   //pi
						
						

				//        var _IsInvX = (_HOffs + Math.Abs(_X)) % _HStep > _HMiddle;

				//        var _IsInvY = (_VOffs + Math.Abs(_Y)) % _VStep > _VMiddle;
				//        //var _IsInvY = false;///(_VOffs + Math.Abs(_Y)) % _VStep > _VMiddle;
				//        var _BiasA  = _IsInvX ^ _IsInvY ? _InvA : 0.0; //pi / 3;

				//        for(double cA = 0.0; cA < _MaxA; cA += _StepA)
				//        {
				//            GL.Vertex2
				//            (
				//                _X - (Math.Sin(cA + _BiasA) * _Radius),//(0.5 * Math.Sin(_TriaA)),
				//                _Y - (Math.Cos(cA + _BiasA) * _Radius)// * (0.5 * Math.Sin(_TriaA))
				//            );
				//        }
				//    }
				//    GL.End();

					
				//}
				//public static void DrawObject       (SchemeViewport iViewport, SchemeObject iObject) 
				//{
				//   GL.PushMatrix();
				//   GL.Translate(iObject.Transform.X,iObject.Transform.Y,0);
				//   //GL.Rotate(-(float)DateTime.Now.Millisecond / 1000 * 1800,0,0,1);
				//   GL.Rotate(iObject.Transform.Angle,0,0,1);
				//   GL.Scale(iObject.Transform.Scale);

				//   GL.Begin(PrimitiveType.Quads);
				//   {
				//      GL.Color4(Color.FromArgb(128, 0,128,0));

				//      GL.Vertex2(+ 1.0, + 1.0);
				//      GL.Vertex2(+ 1.0, - 1.0);
				//      GL.Vertex2(- 1.0, - 1.0);
				//      GL.Vertex2(- 1.0, + 1.0);
				//   }
				//   GL.End();
				//   GL.PopMatrix();

				//   //throw new NotImplementedException();

				//}
				

				//public static void DrawRootObject      (ZoomableFrame iFrame)
				//{
				//    ///iViewport.Scheme.MousePosition = new Vector2d(iViewport.Mouse.SX, iViewport.Mouse.SY);

				//    GL.Enable(EnableCap.LineSmooth);
				//    GL.LineWidth(1);


				//    iFrame.RootObject.Draw();
				//}
				public static void DrawContactPoints   (ZoomableFrame iFrame)
				{
					

					//GL.PointSize(5f);
					//GL.Begin(PrimitiveType.Points);
					//{
					//    GL.Color4(iViewport.Palette.ForeColor);
					//    //GL.Color4(Screen.Colors[iNode.Color]);

					//    foreach(var cVertex in iViewport.ContactPoints)
					//    {
					//        GL.Vertex3(cVertex);
					//    }
					//}
					//GL.End();
				}
				
			}
			public class Tests
			{
				public static void Atlas(ZoomableFrame iFrame)
				{
					//var _SVP   = iViewport; if(_SVP == null) return;
					//var _RNG   = new Random();
					//var _Sizes = new List<Size>();
					//{
					//    //for(int _Count = (int)((DateTime.Now.Second / 60.0) * 200), cI = 0; cI < _Count; cI++)
					//    for(int _Count = 100, cI = 0; cI < _Count; cI++)
					//    {
					//        //var cSize = new Size(_RNG.Next(1,64), _RNG.Next(1,64));
					//        var cPowOf2 = (int)Math.Pow(2, _RNG.Next(0, 4));
					//        var cSize = new Size(cPowOf2,cPowOf2);//_RNG.Next(1,64), _RNG.Next(1,64));


					//        _Sizes.Add(cSize);
					//    }

					//    //_Sizes.Sort(new Comparison<Size>(this.Atlas.CompareAreas));
					//}
					//iViewport.SchemeAtlas.AllocateRegions(_Sizes.ToArray(), 256);

					//var _Grx = Graphics.FromImage((_SVP.Scheme.Children[0] as SchemeObjectModel.PixelMap).Image);
					//{
					//    _Grx.Clear(Color.Transparent);
					//    foreach(var cRegion in _SVP.SchemeAtlas.Regions)
					//    {
					//        _Grx.FillRectangle(new SolidBrush(Color.FromArgb(128, (int)(_RNG.NextDouble() * 255), (int)(_RNG.NextDouble() * 255), (int)(_RNG.NextDouble() * 255))), cRegion.Bounds);
					//    }
					//}
					//_Grx.DrawString("Processed: " + _SVP.SchemeAtlas.Regions.Length + "/" + _Sizes.Count + "(" + Math.Floor((float)_SVP.SchemeAtlas.Regions.Length / _Sizes.Count * 100) +  "%)", new Font(FontFamily.GenericSansSerif, 30), Brushes.Red, 10,10);

					////_Grx.
				}
			}
		}
	}
}