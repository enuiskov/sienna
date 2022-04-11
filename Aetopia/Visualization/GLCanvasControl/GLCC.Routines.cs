using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
//using System.Text;
using System.Windows.Forms;
using WF = System.Windows.Forms;
using OpenTK;
//using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public partial class GLCanvasControl : OpenTK.GLControl
	{
		

		//public DateTime      LastRenderTime = DateTime.MinValue;
		//public float         AverageFrameRate    = 0.0f;
		//public float FrameOpacity = 
		
		public int           SrcBlend     = 8;//4;
		public int           DstBlend     = 0;//5;


		public int           BgColor      = 0;
		public int           TextColor    = 1;
		public int           DrawingColor = 1;

		public int[]         BlendingModes = new int[]{0,1,768,769,770,771,772,773,774,775,776,32769,32770,32771,32772,34185,35065,35066,35067};
		
		

		
		public partial struct Routines
		{
			
			public class Drawing
			{
				public static void DrawRectangle (PrimitiveType iPrimType, Color iColor, float iX, float iY, float iWidth, float iHeight)
				{
					DrawRectangle(iPrimType, iColor, new RectangleF(iX,iY,iWidth,iHeight));
				}
				public static void DrawRectangle (PrimitiveType iPrimType, Color iColor, RectangleF iRect)
				{
					DrawGradient(iPrimType, iRect, iColor, iColor, true);
				}
				public static void DrawGradient  (PrimitiveType iPrimType, RectangleF iRect, Color iFrColor, Color iToColor, bool iIsTopDown)
				{
					var _IsTex = GL.GetBoolean(GetPName.Texture2D);

					if(_IsTex) GL.Disable(EnableCap.Texture2D);

					GL.LineWidth(1);

					GL.Begin(iPrimType);
					{
						GL.Color4(             iFrColor           ); GL.Vertex2(iRect.Left, iRect.Top);
						GL.Color4(iIsTopDown ? iFrColor : iToColor); GL.Vertex2(iRect.Right,iRect.Top);
						GL.Color4(             iToColor           ); GL.Vertex2(iRect.Right,iRect.Bottom);
						GL.Color4(iIsTopDown ? iToColor : iFrColor); GL.Vertex2(iRect.Left, iRect.Bottom);
					}
					GL.End();

					if(_IsTex) GL.Enable(EnableCap.Texture2D);
				}

				public static void DrawText      (string iText)
				{
					
				}
				public static void DrawVertexArray(T2fC4ubV3fVertex[] iVertexArray, int iTexID)
				{
					unsafe
					{
						fixed (T2fC4ubV3fVertex* _VertArrPtr = iVertexArray)
						{
							GL.InterleavedArrays(OpenTK.Graphics.OpenGL.InterleavedArrayFormat.T2fC4ubV3f,0,(IntPtr)_VertArrPtr); 

							GL.BindTexture(TextureTarget.Texture2D, iTexID);

							GL.Enable(EnableCap.Texture2D);
							GL.DrawArrays(PrimitiveType.Quads, 0, iVertexArray.Length);
							GL.Disable(EnableCap.Texture2D);
							
							GL.Finish();
						}
					}
				}
			}
			public class Rendering
			{
				//public static void GenerateTextures    (Viewport iCanvas) 
				//{
				//    //return;

				//    var _TexSize = 256;

				//    for(var cI = 0; cI < 10; cI++)
				//    {
				//        var cTexID = GL.GenTexture();
				//        {
				//            GL.BindTexture  (TextureTarget.Texture2D, cTexID);
				//            GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _TexSize, _TexSize, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
				//            GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
				//            GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
				//        }
				//        iCanvas.TexIDs.Add(cTexID);
				//    }
				//}
				//public static void DeleteTexture(Canvas iCanvas, GdiGLFrame iFrame) 
				//{
				//    if(iFrame.TexID != -1)
				//    {
				//        GL.DeleteTexture(iFrame.TexID);
				//        iFrame.TexID = -1;
				//    }
				//}
				//public static void GenerateTexture    (Canvas iCanvas, GdiGLFrame iFrame) 
				//{
				//    //Math.Max(iFrame.Width, iFrame.Height);
				//    //var _MaxSize = Math.Max(iFrame.Width, iFrame.Height);
				//    //var _TexSize = (int)Math.Pow(2, Math.Ceiling(Math.Log(_MaxSize,2)));
				//    //return;

				//    //var _TexSize = 256;
				//    iFrame.TexID = GL.GenTexture();
				//    {
				//        GL.BindTexture  (TextureTarget.Texture2D, iFrame.TexID);
				//        GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, iFrame.TexSize, iFrame.TexSize, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
				//        GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
				//        GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
				//    }
				//}


				public static void GenerateFrameTextures   (GLCanvasControl iCanvas) 
				{
					//foreach(var cFrame in iCanvas.Frame.GetAllChildFrames())
					//{
					//    cFrame.UpdateTexture();
					//}
				}
				public static void CreateFrameLayers   (Frame iFrame, bool iIsRecursive) 
				{
					///iFrame.Layers = new GLGraphicsLayer[]{new GLGraphicsLayer(),new GLGraphicsLayer()};

					for(var cLi = 0; cLi < iFrame.Layers.Length; cLi++)
					{
						iFrame.Layers[cLi] = new GLGraphicsLayer(iFrame);
					}

					foreach(var cLayer in iFrame.Layers)
					{
						cLayer.UpdateSize(iFrame.Width, iFrame.Height);
					}

					if(iIsRecursive) foreach(var cFrame in iFrame.Children)
					{
						CreateFrameLayers(cFrame, iIsRecursive);
					}
				}
				public static void ResizeFrameLayers   (Frame iFrame, bool iIsRecursive) 
				{
					//var _Control = iFrame.Canvas.Control as GLCanvasControl;

					
					foreach(GLGraphicsLayer cLayer in iFrame.Layers)
					{
						if(cLayer == null) continue;

						cLayer.UpdateSize(iFrame.Width, iFrame.Height);
					}

					if(iIsRecursive) foreach(var cFrame in iFrame.Children)
					{
						ResizeFrameLayers(cFrame, iIsRecursive);
					}
				}
				public static void InitGL                  (GLCanvasControl iCanvas) 
				{
					iCanvas.TestTexSize = 128;
					iCanvas.TestTexId   = GL.GenTexture();
					{
						GL.BindTexture  (TextureTarget.Texture2D, iCanvas.TestTexId);
						///GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, iCanvas.TestTexSize, iCanvas.TestTexSize, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
						GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, iCanvas.TestTexSize, iCanvas.TestTexSize, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
						GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
						GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
					}
					




					////GL.ClearColor(Color.Gainsboro);

					//GL.Enable(EnableCap.Texture2D);
					///GL.Enable(EnableCap.Blend);
					//GL.Enable(EnableCap.AlphaTest);

					//GL.Disable(EnableCap.DepthTest);
					//GL.Disable(EnableCap.DepthClamp);
					
					////GL.AlphaFunc(AlphaFunction.Greater, 0.5f);
					/////GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcColor);
					////GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
					////GL.BlendFunc(( BlendingFactorSrc)this.SrcBlend, BlendingFactorDest.SrcColor);

					////GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.SrcColor);


					//GL.BlendColor(Color4.Black);
					////GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcColor);

					/**
					int _MaxTexSize = iCanvas.FramesAtlas.TestSize;//. 512;//2048;//GL.GetInteger(GetPName.MaxTextureSize);

					iCanvas.FramesTextureID = GL.GenTexture();
					{
						GL.BindTexture  (TextureTarget.Texture2D, iCanvas.FramesTextureID);
						GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _MaxTexSize, _MaxTexSize, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
						GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
						GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
					}
					*/
					
					//GenerateFrameTextures(iCanvas);
					//GenerateTextures(iCanvas);
					


					////throw new NotImplementedException();
				}
				
				//public static void Resize                  (Canvas iCanvas) 
				//{
				//    GL.Viewport(0, 0, iCanvas.Width, iCanvas.Height);
				//    //GL.Viewport(iCanvas.Width / 2, iCanvas.Height / 2, iCanvas.Width / 2,iCanvas.Height / 2);
				//}
				public static void PrepareGL               (GLCanvasControl iCanvas) 
				{
					GL.Viewport(0, 0, iCanvas.Width, iCanvas.Height);
					//GL.BlendFunc((BlendingFactorSrc)iCanvas.BlendingModes[iCanvas.SrcBlend], (BlendingFactorDest)iCanvas.BlendingModes[iCanvas.DstBlend]);
					//GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);


					
					//GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

					
					//GL.Viewport(0,0,iCanvas.Width,iCanvas.Height);
					GL.ClearColor(Color.FromArgb(0,iCanvas.Canvas.Frame.Palette.ShadeColor));
					//GL.Clear(ClearBufferMask.ColorBufferBit);
					///GL.ClearColor(Color.Transparent);
					//GL.Clear(ClearBufferMask.ColorBufferBit);//iCanvas.Frame.Palette.ShadeColor);

					///GL.BlendFunc((BlendingFactorSrc)iCanvas.BlendingModes[iCanvas.SrcBlend], (BlendingFactorDest)iCanvas.BlendingModes[iCanvas.DstBlend]);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
					//GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

					///GL.LineWidth(1);
					///GL.Enable(EnableCap.LineSmooth);//.LineS
					
					///SetFrameMatrix(iCanvas);
					//GL.Clear(ClearBufferMask.ColorBufferBit);

					//GL.ClearColor(Color.FromArgb(50,0,0,0));//this.Colors[this.BgColor]);
					//GL.MatrixMode(MatrixMode.Projection);
					//GL.LoadIdentity();
					//GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
					//GL.Ortho(0.0, 1.0, 0.0, 1.0, 0.0, 4.0);

					//GL.Clear(ClearBufferMask.ColorBufferBit);
					
				}
				public static void SetCanvasMatrix         (GLCanvasControl iCanvas) 
				{
					GL.MatrixMode(MatrixMode.Projection);
					GL.LoadIdentity();
					
					//GL.Ortho(0.0,iCanvas.Width, 0.0,iCanvas.Height, 0.0, 4.0);
					GL.Ortho(0.0, iCanvas.Width,iCanvas.Height, 0.0, 0.0, 4.0);

					GL.MatrixMode(MatrixMode.Modelview);
					GL.LoadIdentity();
				}
				//public static void SetOrthographicMatrix   (Canvas iCanvas) 
				//{
				//    GL.MatrixMode(MatrixMode.Projection);
				//    GL.LoadIdentity();
				
				//    GL.Ortho(0.0,1.0, 1.0,0.0, 0.0, 4.0);
				//}
				public static void SetFrameMatrix          (GLCanvasControl iCanvas) 
				{
					GL.MatrixMode(MatrixMode.Projection);
					GL.LoadIdentity();
					
					//GL.Ortho(0.0,1.0, 0.0,1.0, 0.0, 4.0);
					GL.Ortho(0.0,1.0, 1.0,0.0, 0.0, 4.0);

					GL.MatrixMode(MatrixMode.Modelview);
					GL.LoadIdentity();
				}
				//public static void SetFrameMatrix          (Canvas iCanvas) 
				//{
				//    GL.MatrixMode(MatrixMode.Projection);
				//    GL.LoadIdentity();
					
				//    GL.Ortho(0.0,1.0, 0.0,1.0, 0.0, 4.0);

				//    //GL.MatrixMode(
				//    //GL.Ortho(0.0,1.0, 1.0,0.0, 0.0, 4.0);
				//}
				
				

				public static void BeginTest (GLCanvasControl iCanvas) 
				{
					//GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

					//GL.Hint(HintTarget.LineSmoothHint, HintMode.Fastest);
					//GL.BlendFunc((BlendingFactorSrc)iCanvas.BlendingModes[iCanvas.SrcBlend], (BlendingFactorDest)iCanvas.BlendingModes[iCanvas.DstBlend]);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

					GL.ClearColor(Color.FromArgb(255,iCanvas.Canvas.Frame.Palette.ShadeColor));
					///GL.ClearColor(Color.FromArgb(0,Color.Black));
					///GL.ClearColor(Color.FromArgb(0,Color.Black));
					//GL.ClearColor(Color.FromArgb(255,iCanvas.Frame.Palette.ShadeColor));

					GL.Clear(ClearBufferMask.ColorBufferBit);
					{
						var _Size = iCanvas.TestTexSize * 0.8;/// 0.2 * iCanvas.AspectRatio;

						GL.Begin(PrimitiveType.Quads);
						{
							//GL.Color4(255,255,255,255);
							//GL.Color4(1.0,1.0,1.0,1.0);
							var _Color1 = Color.FromArgb(255,255,127,0);
							var _Color2 = Color.FromArgb(0,_Color1);
							//var _Color2 = Color.Black;
							
							GL.Color4(_Color1); GL.Vertex2(0, 0);
							GL.Color4(_Color1); GL.Vertex2(_Size, 0);
							GL.Color4(_Color2); GL.Vertex2(_Size, _Size);
							GL.Color4(_Color2); GL.Vertex2(0, _Size);
						}
						GL.End();

						///SetFrameMatrix(iCanvas);

						///GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
						GL.BlendFunc((BlendingFactorSrc)iCanvas.BlendingModes[iCanvas.SrcBlend], (BlendingFactorDest)iCanvas.BlendingModes[iCanvas.DstBlend]);
					
						GL.Scale(_Size,_Size, 1);
						GL.Begin(PrimitiveType.LineLoop);
						{
							GL.Color4(0.0,1.0,1.0,1.0);

							GL.Vertex2(0,    0);
							GL.Vertex2(0.5,  0);
							GL.Vertex2(0.5,  0.5);
							GL.Vertex2(0.75, 0.25);
							GL.Vertex2(0.10, 0.75);
							GL.Vertex2(0,    1.0);
							GL.Vertex2(1.0,  0.8);
						}
						GL.End();

						//GL.BindTexture(TextureTarget.Texture2D, iCanvas.TestTexId);
						//GL.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 0, iCanvas.Height, iCanvas.TestTexSize , iCanvas.TestTexSize , 0);

						//GL.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 0, iCanvas.TestTexSize, iCanvas.TestTexSize, iCanvas.TestTexSize, 0);
						///GL.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 0, iCanvas.Height - iCanvas.TestTexSize, iCanvas.TestTexSize, iCanvas.TestTexSize, 0);
						GL.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 0, iCanvas.Height - iCanvas.TestTexSize, iCanvas.TestTexSize, iCanvas.TestTexSize, 0);
						//GL.CopyPixels();
						///GL.GetTexImage(TextureTarget,,,,
					}
					///GL.Clear(ClearBufferMask.ColorBufferBit);


				}
				public static void EndTest   (GLCanvasControl iCanvas) 
				{
					var _10Seconds = (double)(DateTime.Now.Ticks % 1e8 / 1e8);
					//var _Size = iCanvas.TestTexSize * 2;
					var _Offs = new Vector3(200 + -((float)Math.Sin(_10Seconds * Math.PI * 2.0) * 0),200 + ((float)Math.Cos(_10Seconds * Math.PI * 2.0) * 0),0);

					GL.BindTexture(TextureTarget.Texture2D, iCanvas.TestTexId);
					///GL.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 0, iCanvas.Height - iCanvas.TestTexSize, iCanvas.TestTexSize, iCanvas.TestTexSize, 0);

					
					//GL.ClearColor(Color.Red);
					///GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);



					GL.Viewport(0,0,iCanvas.Width, iCanvas.Height);
					GL.BindTexture(TextureTarget.Texture2D, iCanvas.TestTexId);

					SetCanvasMatrix(iCanvas);
					//SetFrameMatrix(iCanvas);

					//GL.MatrixMode(MatrixMode.Projection);
					//GL.LoadIdentity();
					//GL.MatrixMode(MatrixMode.Modelview);
					//GL.LoadIdentity();
					//GL.Ortho(0.0, iCanvas.Width, iCanvas.Height, 0.0, 0.0, 4.0);


					//GL.Scale(1f / iCanvas.Width, -1f / iCanvas.Height, 1);
					//GL.Translate(0,-iCanvas.Height,0);
					//GL.Translate(_Offs);


					//if(iCanvas.Palette.IsLightTheme)
					//{
					////    GL.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.Zero);
						
					//}
					//else
					//{
					//    ///One,SrcColor
					////    GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
					//}
					GL.Color4(Color.FromArgb(255, Color.White));

					
					//GL.BlendFunc((BlendingFactorSrc)iCanvas.BlendingModes[iCanvas.SrcBlend], (BlendingFactorDest)iCanvas.BlendingModes[iCanvas.DstBlend]);
					///GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

					///http://stackoverflow.com/questions/1317393/image-blending-problem-when-rendering-to-texture
					//GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

					
					//GL.Scale(
					//GL.Enable(EnableCap.Texture2D);
					//GL.Begin(PrimitiveType.Quads);
					//{
						//var _Size = iCanvas.TestTexSize;
						//var cSize = _Size / 2;

						//GL.Scale();

						GL.Translate(10,200,0);

						for(var cSizeI = 0; cSizeI < 5; cSizeI++)
						{
							var cSize = iCanvas.TestTexSize * Math.Pow(cSizeI, 2);

							GL.Translate(cSize / 10, 0, 0);

							GL.Enable(EnableCap.Texture2D);
							GL.Color4(Color.White);
							GL.Begin(PrimitiveType.Quads);
							{
								for(var cPass = 0; cPass < 1; cPass++)
								{
									GL.TexCoord2(0,0);  GL.Vertex2(0,0);
									GL.TexCoord2(1,0);  GL.Vertex2(cSize,0);
									GL.TexCoord2(1,-1); GL.Vertex2(cSize,cSize);
									GL.TexCoord2(0,-1); GL.Vertex2(0,cSize);
								}
								//GL.TexCoord2(0, 0); GL.Vertex2(30 + (cSize - _Size),         0);
								//GL.TexCoord2(1, 0); GL.Vertex2(0 + (cSize, 0);
								//GL.TexCoord2(1,-1); GL.Vertex2(0 + cSize, 0 + cSize);
								//GL.TexCoord2(0,-1); GL.Vertex2(0,         0 + cSize);

								
							}
							GL.End();

							
							GL.Disable(EnableCap.Texture2D);

							GL.Color4(iCanvas.Canvas.Palette.GlareColor);
							GL.Begin(PrimitiveType.LineLoop);
							{
								GL.Vertex2(0,     0);
								GL.Vertex2(cSize, 0);
								GL.Vertex2(cSize, cSize);
								GL.Vertex2(0,     cSize);
							}
							GL.End();


							cSize *= 2;
						}

						
						//GL.TexCoord2(0,0);  GL.Vertex2(0,         0);
						//GL.TexCoord2(1,0);  GL.Vertex2(0 + _Size, 0);
						//GL.TexCoord2(1,-1); GL.Vertex2(0 + _Size, 0 + _Size);
						//GL.TexCoord2(0,-1); GL.Vertex2(0,         0 + _Size);
					//}
				
					
					//GL.Translate(200,0,0);
					//GL.Scale(2,2,1);
					//GL.Begin(PrimitiveType.Quads);
					//{
					//    //var _X = 260;
					//    //var _Y = 100;

					//    GL.TexCoord2(0,0);  GL.Vertex2(0, 0);
					//    GL.TexCoord2(1,0);  GL.Vertex2(1, 0);
					//    GL.TexCoord2(1,-1); GL.Vertex2(1, 1);
					//    GL.TexCoord2(0,-1); GL.Vertex2(0, 1);
					//}
					//GL.End();

					///GL.Disable(EnableCap.Texture2D);

					//GL.Color4(iCanvas.Palette.GlareColor);
					//GL.Begin(PrimitiveType.LineLoop);
					//{
					//    GL.Vertex2(0,         0);
					//    GL.Vertex2(0 + _Size, 0);
					//    GL.Vertex2(0 + _Size, 0 + _Size);
					//    GL.Vertex2(0,         0 + _Size);
					//}
					//GL.End();

					///GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
				
				}

				public static void Begin               (GLCanvasControl iCanvas) 
				{
					PrepareGL(iCanvas);

					//PrepareGL(iCanvas);
					///SetFrameMatrix(iCanvas);
					SetCanvasMatrix(iCanvas);

					///BeginTest(iCanvas);
					
					
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
					GL.ClearColor(Color.FromArgb(0,iCanvas.Canvas.Frame.Palette.ShadeColor));
					GL.Clear(ClearBufferMask.ColorBufferBit);
				}
				public static void End                 (GLCanvasControl iCanvas) 
				{
					///EndTest(iCanvas);

					iCanvas.SwapBuffers();
				}

				
				public static void Render              (GLCanvasControl iCanvas) 
				{
					
					SetCanvasMatrix(iCanvas);

					//GL.BlendFunc((BlendingFactorSrc)iCanvas.BlendingModes[iCanvas.SrcBlend], (BlendingFactorDest)iCanvas.BlendingModes[iCanvas.DstBlend]);


					//GL.BlendFunc((BlendingFactorSrc)iCanvas.BlendingModes[iCanvas.SrcBlend], (BlendingFactorDest)iCanvas.BlendingModes[iCanvas.DstBlend]);
					//GL.BlendFunc(BlendingFactorSrc.OneMinusSrcColor, BlendingFactorDest.SrcAlphaSaturate);
					///Canvas.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127,Color.White), new RectangleF(0,0,1,1));

					GL.Enable(EnableCap.Blend);
					
										
					//return;
					var _Frame = iCanvas.Canvas.Frame;
					
					//_Frame.Prepare();
					
					_Frame.DefaultRender();
					///RenderFrame(iCanvas.Canvas.Frame);

					//iCanvas.Text.Value = DateTime.Now.Ticks.ToString();
					//iCanvas.Text.UpdateVertexArray();
					//iCanvas.Text.Draw();


					ProcessEffects(iCanvas);
				}
				public static void RenderFrame         (Frame iFrame)
				{
					if(!iFrame.IsVisible) return;

					//GL.PushMatrix();
					//GL.Translate(iFrame.Bounds.X,iFrame.Bounds.Y,0.0);
					{
						PrepareFrame(iFrame); ///iFrame.Prepare();
						///Rendering.RenderFrame (iFrame);

						iFrame.OnBeforeRender();
						

						if(true)
						{
							if(iFrame.Layers.Length != 0)
							{
								for(var cLi = 0; cLi < iFrame.Layers.Length; cLi++)
								{
									var cLayer = iFrame.Layers[cLi] as GLGraphicsLayer; if(cLayer == null) continue;

									if(cLayer.CurrentPhase == GraphicsLayer.Phase.Drawing)
									{
										cLayer.Context.BindPalette(iFrame.Palette);
										cLayer.Context.Clear();

										var _Cter = cLayer.Context.BeginContainer();
										{
											switch(cLi)
											{
												case 0 : iFrame.DrawBackground(cLayer.Context); break;
												case 1 : iFrame.DrawForeground(cLayer.Context); break;
												//case 2 : iFrame.DrawHoverground(cLayer.Context); break;

												default : throw new Exception("WTFE");
											}
											cLayer.Context.EndContainer(_Cter);
										}
										
										cLayer.SetProcessingPhase(GraphicsLayer.Phase.Synchronization);
									}
									
									if(cLayer.CurrentPhase == GraphicsLayer.Phase.Synchronization) cLayer.UpdateTextureData();
									if(cLayer.CurrentPhase == GraphicsLayer.Phase.Ready)           RenderLayer(cLayer);
								}
								//foreach(GLGraphicsLayer cLayer in iFrame.Layers)
								//{
								//    ///cLayer.Invalidate();
								//    //this.UpdateGraphics();
									
								//}
							}
							else
							{
								iFrame.CustomRender();
							}
							
							//else if(iFrame.IsCustomRender)
							//{

							//}
						}
						iFrame.OnAfterRender();
							

						if(iFrame == iFrame.Canvas.ActiveFrame && iFrame.Dock != DockStyle.Fill)
						{
							Routines.Rendering.DrawFocus(iFrame);
						}
						//if(iFrame == iFrame.Canvas.ActiveFrame) Routines.Rendering.DrawFocus(iFrame);

						for(var cFi = 0; cFi < iFrame.Children.Count; cFi++)
						//for(var cFi = iFrame.Children.Count - 1; cFi >= 0; cFi--)
						{
							RenderFrame(iFrame.Children[cFi]);
						}

						//foreach(var cFrame in iFrame.Children)
						//{
						//    RenderFrame(cFrame);
						//}
					}
					//GL.PopMatrix();
				}
				public static void PrepareFrame        (Frame iFrame)
				{
					GL.Disable(EnableCap.DepthTest);

					if(iFrame.Parent != null) GL.Viewport(iFrame.Bounds.X, iFrame.Parent.Height      - iFrame.Height - iFrame.Bounds.Y, iFrame.Width, iFrame.Height);
					else                      GL.Viewport(0,             iFrame.Canvas.Size.Height - iFrame.Height - iFrame.Bounds.Y, iFrame.Width, iFrame.Height);
					

					//GL.MatrixMode(MatrixMode.Projection);
					//GL.LoadIdentity();


					//OpenTK.Matrix4 _OrthoMat = OpenTK.Matrix4.CreateOrthographicOffCenter(0,1, 1,0, 0.0f,4.0f);
				   
					GL.MatrixMode(MatrixMode.Projection);
					GL.LoadIdentity();

					GL.Ortho(0.0, iFrame.Width,iFrame.Height, 0.0, 0.0, 4.0);
					
					GL.MatrixMode(MatrixMode.Modelview);
					GL.LoadIdentity();
				}
				public static void RenderLayer(GLGraphicsLayer iLayer)
				{
					if(iLayer.TexID == -1) throw new Exception();

					var   _TexSize = iLayer.TexSize;
					var   _Zoom = iLayer.OwnerFrame.Zoom;
					int   _ImgW = iLayer.Image.Width, _ImgH = iLayer.Image.Height;
					int   _QuadW = _ImgW * _Zoom, _QuadH = _ImgH * _Zoom;
					float _TexW = (float)_ImgW / _TexSize, _TexH = (float)_ImgH / _TexSize;

					GL.Enable(EnableCap.Texture2D);
					GL.BindTexture(TextureTarget.Texture2D, iLayer.TexID);
					GL.Begin(PrimitiveType.Quads);
					{
						///GL.Color4(1.0,1.0,1.0, iLayer.Opacity);
						GL.Color4(1.0,1.0,1.0,1.0);

						GL.TexCoord2(    0,     0); GL.Vertex2(0.0,     0.0);
						GL.TexCoord2(_TexW,     0); GL.Vertex2(_QuadW,   0.0);
						GL.TexCoord2(_TexW, _TexH); GL.Vertex2(_QuadW, _QuadH);
						GL.TexCoord2(    0, _TexH); GL.Vertex2(0.0,   _QuadH);
					}
					GL.End();
					GL.Disable(EnableCap.Texture2D);

					//GL.Begin(PrimitiveType.LineLoop);
					//{
					//    GL.Color4(0.5,0.5,0.5,1.0);

					//    GL.Vertex2(0.0,     0.0);
					//    GL.Vertex2(_AbsW,   0.0);
					//    GL.Vertex2(_AbsW, _AbsH);
					//    GL.Vertex2(0.0,   _AbsH);
					//}
					//GL.End();
					

				}


				public static void DrawFocus           (Frame iFrame)
				{
					float _Phase = (float)Math.Sin(DateTime.Now.Millisecond / 1000d * Math.PI * 4d) * 0.5f + 0.5f;
					float _AW = iFrame.Width, _AH = iFrame.Height;
					float _CornAS = 10f, _CW = _CornAS, _CH = _CornAS;
					float _CornAO = (float)(_CornAS * _Phase * 0.2f) + 10f;
					//float _L = _CornAO / _AW, _T = _CornAO / _AH, _R = 1f - _L, _B = 1f - _T;
					float _L = _CornAO, _T = _CornAO, _R = _AW - _L, _B = _AH - _T;
					
					GL.LineWidth(1f);
					GL.Begin(PrimitiveType.Lines);
					{
						GL.Color4(Color.FromArgb((int)MathEx.Clamp(255 * (1f - _Phase), 64, 255),iFrame.Palette.ForeColor));

						GL.Vertex2(_L,_T); GL.Vertex2(_L,       _T + _CH);
						GL.Vertex2(_L,_T); GL.Vertex2(_L + _CW, _T      );

						GL.Vertex2(_R,_T); GL.Vertex2(_R,       _T + _CH);
						GL.Vertex2(_R,_T); GL.Vertex2(_R - _CW, _T      );

						GL.Vertex2(_L,_B); GL.Vertex2(_L,       _B - _CH);
						GL.Vertex2(_L,_B); GL.Vertex2(_L + _CW, _B      );

						GL.Vertex2(_R,_B); GL.Vertex2(_R,       _B - _CH);
						GL.Vertex2(_R,_B); GL.Vertex2(_R - _CW, _B      );
					}
					GL.End();
				}
				
//                public static void ProcessEffects          (Canvas iCanvas)
//                {
//                    ///return;


//                    GL.Viewport(0,0,iCanvas.Width,iCanvas.Height);
//                    GL.MatrixMode(MatrixMode.Projection);
//                    GL.LoadIdentity();
//                    GL.MatrixMode(MatrixMode.Modelview);
//                    GL.LoadIdentity();
//                    GL.Ortho(0.0, 1,1, 0.0, 0.0, 4.0);
					

//                    GL.BlendFunc((BlendingFactorSrc)iCanvas.BlendingModes[iCanvas.SrcBlend], (BlendingFactorDest)iCanvas.BlendingModes[iCanvas.DstBlend]);
/////					Canvas.DrawRectangle(PrimitiveType.Quads, Color.FromArgb((byte) (Canvas.GammaCorrection * 255), Color.White), new RectangleF(0,0,1,1));

//                    /**
//                        1~~~, One,DstColor
//                              One,OneMinusConstantColor
//                              OneMinusConstantColor, DstColor,
						
					
//                    */
//                    byte _Byte = (byte) (Canvas.GammaCorrection * 255);
//                    Canvas.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(255,_Byte,_Byte,_Byte), new RectangleF(0,0,1,1));
//                    //Canvas.DrawRectangle(PrimitiveType.Quads, Color.White, new RectangleF(0,0,1,1));


//                    //Canvas.GammaCorrection = (Math.Sin(DateTime.Now.Ticks / 1e7) * 0.8 / 2 + 0.5);
//                    //Canvas.GammaCorrection = iCanvas;

//                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
					
//                }
				public static void ProcessEffects          (GLCanvasControl iCanvas)
				{
					///return;


					GL.Viewport(0,0,iCanvas.Width,iCanvas.Height);


					//GL.MatrixMode(MatrixMode.Projection);
					//GL.LoadIdentity();
					//GL.MatrixMode(MatrixMode.Modelview);
					//GL.LoadIdentity();
					//GL.Ortho(0.0, 1,1, 0.0, 0.0, 4.0);
					SetFrameMatrix(iCanvas);
					

					GL.BlendFunc((BlendingFactorSrc)iCanvas.BlendingModes[iCanvas.SrcBlend], (BlendingFactorDest)iCanvas.BlendingModes[iCanvas.DstBlend]);
					GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127,Color.White), new RectangleF(0,0,1,1));

					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
					GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, Color.FromArgb((byte) (iCanvas.Canvas.GammaCorrection * 255), Color.White), new RectangleF(0,0,1,1));
				}
				

				public static void DrawBackground      (GLCanvasControl iCanvas) 
				{
					if(false)
					{
						
						GL.Begin(PrimitiveType.Quads);
						{
						    //GL.Color4(255,255,255,255);
						    GL.Color4(1.0,1.0,1.0,1.0);
						    GL.Vertex2(0, 0); GL.Vertex2(1, 0);
						    GL.Vertex2(1, 1); GL.Vertex2(0, 1);
						}
						GL.End();
						//GL.Enable(EnableCap.Blend);
					}
					else
					{
					    GL.Clear(ClearBufferMask.ColorBufferBit);
						//GL.Enable(EnableCap.Blend);

						//GL.Begin(PrimitiveType.Quads);
						//{
						//    GL.Color4(iCanvas.Frame.Palette.BackGradTopColor);    GL.Vertex2(0, 0); GL.Vertex2(1, 0);
						//    GL.Color4(iCanvas.Frame.Palette.BackGradBottomColor); GL.Vertex2(1, 1); GL.Vertex2(0, 1);
						//}
						//GL.End();
					}
				}

				public static void DrawCell            (Frame iFrame, SymbolCell iCell, bool iDoHighlight, float iLineWidth, float iPointWidth)
				{
					//var _B = iCell.Bounds;


					
					GL.LineWidth(iLineWidth);

					//if(iDoHighlight && iFrame.DoShowGrid)
					//{
					//    GL.Color4(Color.FromArgb(64, Canvas.Palette.Colors[7]));
					//    ///GL.LineWidth(iDoHighlight ? 3 : 1);
					//    GL.Begin(PrimitiveType.LineLoop);
					//    {
					//        GL.Vertex2(0,0);
					//        GL.Vertex2(1,0);
					//        GL.Vertex2(1,1);
					//        GL.Vertex2(0,1);
					//    }
					//    GL.End();


					//    ///var _Pointer = new Vector2d(iFrame.Pointer.X - iCell.Position.X, - iFrame.Pointer.Y - iCell.Position.Y);/// Vector3.Transform((Vector3)iFrame.Pointer, iCell.Matrix).Xy;

					//    //GL.Color4(iFrame.Palette.GlareColor);
					//    //GL.PointSize(10);
					//    //GL.Begin(PrimitiveType.Points);
					//    //{
					//    //    GL.Vertex2(_Pointer);
					//    //}
					//    //GL.End();
					//}

					

					if(iCell.Lines.Count != 0)
					{
						///GL.Color4(iFrame.Palette.Colors[2]);
						if(iDoHighlight) GL.LineWidth(iLineWidth * 2);
						
						GL.Begin(PrimitiveType.Lines);
						{
							foreach(var cLine in iCell.Lines)
							{
								///if(!iFrame.DoShowSampleText) GL.Color4(iFrame.Palette.Colors[cLine.IsSelected ? 3 : 9]);

								GL.Vertex2(cLine.Point1);
								GL.Vertex2(cLine.Point2);
							}
						}
						GL.End();

						if(iPointWidth >= 1f)
						{
							GL.PointSize(iDoHighlight ? iPointWidth * 2: iPointWidth);
							///GL.Color4(iFrame.Palette.Colors[10]);
							GL.Begin(PrimitiveType.Points);
							{
								foreach(var cLine in iCell.Lines)
								{
									GL.Vertex2(cLine.Point1);
									GL.Vertex2(cLine.Point2);
								}
							}
							GL.End();
						}
					}
				}
			}
		}
	}
}