using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using Sienna.Editor;
using System.Windows.Forms;
//using LineList = System.Collections.Generic.List<Sienna.BlackTorus.OGLConsoleFrame.ConsoleLine>;

namespace Sienna.BlackTorus
{
	public partial class BufferConsoleFrame : TextBufferFrame, IConsole
	{
		private void CreateFontAtlas()
		{
			var _CharsInAtlas   = Math.Pow(this.FontAtlasGrid,2);
			var _AtlasCellSize  = this.FontAtlasSize / this.FontAtlasGrid;
			//var _Font        = new Font("Lucida Console", this.FontSize, FontStyle.Regular);
			var _Font        = new Font("Courier", this.FontSize, FontStyle.Regular);
			var _Brush       = new SolidBrush(Color.White);

			this.FontAtlas = new Bitmap(this.FontAtlasSize,this.FontAtlasSize);
			
			var _Grx = Graphics.FromImage(this.FontAtlas);
			{
				//_Grx.Clear(Color.FromArgb(32,255,255,255));

				for(var cCharI = 0; cCharI < _CharsInAtlas; cCharI++)
				{
					var cChar  = (char)cCharI;
					var cCellX = (cCharI % this.FontAtlasGrid) * _AtlasCellSize;
					var cCellY =  cCharI / this.FontAtlasGrid  * _AtlasCellSize;

					_Grx.DrawString(cChar.ToString(), _Font, _Brush, cCellX,cCellY);
				}
			}

			this.FontAtlasTexID = GL.GenTexture();
			{
				GL.BindTexture  (TextureTarget.Texture2D, this.FontAtlasTexID);
				GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, this.FontAtlasSize, this.FontAtlasSize, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);

				var _Data = this.FontAtlas.LockBits
				(
					new Rectangle(0, 0, this.FontAtlasSize, this.FontAtlasSize),
					System.Drawing.Imaging.ImageLockMode.ReadOnly,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb
				);
				GL.TexSubImage2D
				(
					TextureTarget.Texture2D, 0,
					0,0,this.FontAtlasSize, this.FontAtlasSize,
					PixelFormat.Bgra,
					PixelType.UnsignedByte,
					_Data.Scan0
				);
				this.FontAtlas.UnlockBits(_Data);
			}
		}
		private void UpdateVertexArray()
		{
			var _AtlasGrid  = this.FontAtlasGrid;
			var _AtlasCell  = this.FontAtlasSize / this.FontAtlasGrid;

			var _TxCellSize = 1f / _AtlasGrid; //per cell
			
			var _QuadW      = (float)_AtlasCell;
			var _QuadH      = _QuadW;
			var _QStepX     = this.FontSize * 0.70f;
			var _QStepY     = this.LineHeight;
			var _RowCount   = this.MaxLineCount;
			var _ColCount   = this.Width / (int)_QStepX;
			
			var _DoSyncAllLines = this.VertexArray == null;
			var _VV = _DoSyncAllLines ? new T2fC4ubV3fVertex[_RowCount * _ColCount * 4] : this.VertexArray;
			{
				var _SplitRow = this.VertexSplitLine;

				
				/**
					oooooo|
					ooooo|o
					oooo|oo

					ooo|ooo

					oo|oooo
					o|ooooo
					|oooooo
				
				*/
				//for(var cRow = 0; cRow < _RowCount; cRow ++)
				for(var cLineI = 0; cLineI < _RowCount; cLineI ++)
				///for(int cRow = this.VertexSplitLine; cRow != this.VertexSplitLine - 1; cRow = (cRow % this.Lines.Count) + 1)
			    {
					var cLine = this.Lines[cLineI]; if(!(cLine.IsNew || _DoSyncAllLines)) continue;

					var cRow   = cLine.VertexRow;

					var cStr   = cRow.ToString() + (cRow < 10 ? " " : "") + " | " + cLine.String;
					var cColor = Color4.FromColor(cLine.Color);
					//var cRow = cLineI > this.VertexSplitLine ? cLineI - this.VertexSplitLine : cLineI;
					//(cLineI < this.VertexSplitLine ? cLineI + this.VertexSplitLine :  cLineI - this.VertexSplitLine) % _RowCount;
					//var cVertRow = this.VertexSplitLine > 0 ? cRow % this.VertexSplitLine : 0;// < this.VertexSplitLine ? this.VertexSplitLine - cRow : cRow;
					//if(cRow < this.VertexSplitLine)
					///if(cRow < _RowCount - 1 && DateTime.Now.Millisecond < 980) continue;

					//var cLine = cRow < this.Lines.Count ? this.Lines[cRow] : new Line("", Color.Transparent);
					
					
					
					
					for(var cCol = 0; cCol < _ColCount; cCol++)
					{
						var cChar  = cCol < cStr.Length ? cStr[cCol] : ' ';
						var cCellI = (int)cChar;//43;//
						var cVertI = (cRow * (_ColCount * 4)) + (cCol * 4);

						if(cCellI != 32)
						{
							var cTexBB = new RectangleF
							(
								(cCellI % _AtlasGrid) * _TxCellSize,
								(cCellI / _AtlasGrid) * _TxCellSize, 

								_TxCellSize,_TxCellSize
							);
							var cX  = cCol * _QStepX;
							var cY  = cRow * _QStepY;
							
							_VV[cVertI + 0] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Left,  cTexBB.Top),    cColor, new Vector3(cX,          cY,          0));
							_VV[cVertI + 1] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Right, cTexBB.Top),    cColor, new Vector3(cX + _QuadW, cY,          0));
							_VV[cVertI + 2] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Right, cTexBB.Bottom), cColor, new Vector3(cX + _QuadW, cY + _QuadH, 0));
							_VV[cVertI + 3] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Left,  cTexBB.Bottom), cColor, new Vector3(cX,          cY + _QuadH, 0));

							//_VV[cVertI + 0] = new T2fV3fVertex(cX,          cY,           cTexBB.Left,  cTexBB.Top);
							//_VV[cVertI + 1] = new T2fV3fVertex(cX + _QuadW, cY,           cTexBB.Right, cTexBB.Top);
							//_VV[cVertI + 2] = new T2fV3fVertex(cX + _QuadW, cY + _QuadH,  cTexBB.Right, cTexBB.Bottom);
							//_VV[cVertI + 3] = new T2fV3fVertex(cX,          cY + _QuadH,  cTexBB.Left,  cTexBB.Bottom);
						}
						else
						{
							_VV[cVertI + 0] = T2fC4ubV3fVertex.Offscreen;
							_VV[cVertI + 1] = T2fC4ubV3fVertex.Offscreen;
							_VV[cVertI + 2] = T2fC4ubV3fVertex.Offscreen;
							_VV[cVertI + 3] = T2fC4ubV3fVertex.Offscreen;
						}
			        }

					cLine.IsNew = false;
			    }
			}
			
			this.VertexArray = _VV;
			this.NeedsVertexSync = false;
		}
		public override void Render()
		{
			if(this.MaxLineCount == 0) return;
			//if(DateTime.Now.Millisecond >= 980)
			if(this.NeedsVertexSync)
			{
				this.UpdateVertexArray();
			}

			unsafe
			{
				fixed (T2fC4ubV3fVertex* _VertArrPtr = this.VertexArray)
				{
					GL.InterleavedArrays(OpenTK.Graphics.OpenGL.InterleavedArrayFormat.T2fC4ubV3f,0,(IntPtr)_VertArrPtr); 

					GL.BindTexture(TextureTarget.Texture2D, this.FontAtlasTexID);
					

					var _RowCount        = this.MaxLineCount;
					var _LineVertexCount = this.VertexArray.Length / _RowCount;
				
					
					
					GL.Enable(EnableCap.Texture2D);


					//GL.DrawArrays(PrimitiveType.Quads, 0, this.VertexArray.Length);

					//GL.Disable(EnableCap.Texture2D);
					//GL.DrawArrays(PrimitiveType.LineLoop, 0, this.VertexArray.Length);
					//GL.Enable(EnableCap.Texture2D);

					GL.PushMatrix();
					{
						var _OffsY1 = -((this.VertexSplitLine + 1) * this.LineHeight);
						var _OffsY2 = +this.Height - (this.Height % this.LineHeight);

						var _SplitI = (this.VertexSplitLine + 1) * _LineVertexCount;

						GL.Translate(0f, +_OffsY1, 0f);

						Screen.DrawGradient(PrimitiveType.Quads, new RectangleF(0,0,this.Width,this.Height), Color.Transparent, Color.FromArgb(127,200,255,0), false);
						GL.DrawArrays(PrimitiveType.Quads, _SplitI, this.VertexArray.Length - _SplitI);


						GL.Translate(0f, +_OffsY2, 0f);
						Screen.DrawGradient(PrimitiveType.Quads, new RectangleF(0,0,this.Width,this.Height), Color.Transparent, Color.FromArgb(127,255,200,0), false);
						GL.DrawArrays(PrimitiveType.Quads, 0, _SplitI);




						//GL.Translate(0f, -_OffsY2, 0f);

						
					//    for(var cRowI = 0; cRowI < _RowCount; cRowI ++)
					//    {
					////    //    //if     (cRowI == 0)                        {Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Green), new RectangleF(0,0,this.Width,this.Height));}
					////    //    //else if(cRowI == this.VertexSplitLine + 1) {Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Red), new RectangleF(0,0,this.Width,this.Height));}



					////    //    //if     (cRowI == 0)                       {GL.Translate(0f, +_OffsY2, 0f); GL.Color4(Color.Green);}
					////    //    //else if(cRowI == this.VertexSplitLine + 1) {GL.Translate(0f, -_OffsY2, 0f); GL.Color4(Color.Red);}


							
					////    //    //if     (cRowI == 0)                       {GL.Translate(0f, +_OffsY2, 0f); GL.Color4(Color.Green); Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Green), new RectangleF(0,0,this.Width,this.Height));}
					////    //    //else if(cRowI == this.VertexSplitLine + 1) {GL.Translate(0f, -_OffsY2, 0f); GL.Color4(Color.Red);  Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Red), new RectangleF(0,0,this.Width,this.Height));}

					//        if     (cRowI == 0)                        GL.Translate(0f, +_OffsY2, 0f);
					//        else if(cRowI == this.VertexSplitLine + 1) GL.Translate(0f, -_OffsY2, 0f);

					////        this.RenderLine(cRowI, _LineVertexCount);
					////    }
					}
					GL.PopMatrix();
					GL.Disable(EnableCap.Texture2D);
					GL.Finish();
				}
			}
		}
		//public override void Render()
		//{
		//    if(this.MaxLineCount == 0) return;
		//    //if(DateTime.Now.Millisecond >= 980)
		//    if(this.NeedsVertexSync)
		//    {
		//        this.UpdateVertexArray();
		//    }

		//    unsafe
		//    {
		//        fixed (T2fC4ubV3fVertex* _VertArrPtr = this.VertexArray)
		//        {
		//            GL.InterleavedArrays(OpenTK.Graphics.OpenGL.InterleavedArrayFormat.T2fC4ubV3f,0,(IntPtr)_VertArrPtr); 

		//            GL.BindTexture(TextureTarget.Texture2D, this.FontAtlasTexID);
					

		//            var _RowCount        = this.MaxLineCount;
		//            var _LineVertexCount = this.VertexArray.Length / _RowCount;
				
					
					
		//            GL.Enable(EnableCap.Texture2D);
		//            GL.DrawArrays(PrimitiveType.Quads, 0, this.VertexArray.Length);

		//            //GL.Disable(EnableCap.Texture2D);
		//            //GL.DrawArrays(PrimitiveType.LineLoop, 0, this.VertexArray.Length);
		//            //GL.Enable(EnableCap.Texture2D);

		//            //GL.PushMatrix();
		//            //{
		//            //    var _OffsY1 = -((this.VertexSplitLine + 1) * this.LineHeight);
		//            //    var _OffsY2 = +this.Height - (this.Height % this.LineHeight);

		//            //    //GL.Translate(0f, _OffsY1, 0f);

		//            //    for(var cRowI = 0; cRowI < _RowCount; cRowI ++)
		//            //    {
		//            //    //    //if     (cRowI == 0)                        {Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Green), new RectangleF(0,0,this.Width,this.Height));}
		//            //    //    //else if(cRowI == this.VertexSplitLine + 1) {Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Red), new RectangleF(0,0,this.Width,this.Height));}



		//            //    //    //if     (cRowI == 0)                       {GL.Translate(0f, +_OffsY2, 0f); GL.Color4(Color.Green);}
		//            //    //    //else if(cRowI == this.VertexSplitLine + 1) {GL.Translate(0f, -_OffsY2, 0f); GL.Color4(Color.Red);}


							
		//            //    //    //if     (cRowI == 0)                       {GL.Translate(0f, +_OffsY2, 0f); GL.Color4(Color.Green); Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Green), new RectangleF(0,0,this.Width,this.Height));}
		//            //    //    //else if(cRowI == this.VertexSplitLine + 1) {GL.Translate(0f, -_OffsY2, 0f); GL.Color4(Color.Red);  Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Red), new RectangleF(0,0,this.Width,this.Height));}

		//            //        //if     (cRowI == 0)                        GL.Translate(0f, +_OffsY2, 0f);
		//            //        //else if(cRowI == this.VertexSplitLine + 1) GL.Translate(0f, -_OffsY2, 0f);

		//            //        this.RenderLine(cRowI, _LineVertexCount);
		//            //    }
		//            //}
		//            //GL.PopMatrix();
		//            GL.Disable(EnableCap.Texture2D);
		//            GL.Finish();
		//        }
		//    }
		//}
		private void RenderLine(int iRowI, int iVertexCount)
		{
		    //var cLine     = this.Lines[cRowI];
		    var _VertOffs = iRowI * iVertexCount;

		    ///GL.Color4(iLine.Color);
			//GL.Enable(EnableCap.Texture2D);
		    GL.DrawArrays(PrimitiveType.Quads, _VertOffs, iVertexCount);
			//GL.Disable(EnableCap.Texture2D);
		    //GL.DrawArrays(PrimitiveType.LineLoop, _VertOffs, iVertexCount);

		    ///iLine.IsNew = false;
		}
		//public override void Render()
		//{
		//    if(this.MaxLineCount == 0) return;
		//    //if(DateTime.Now.Millisecond >= 980)
		//    if(this.NeedsVertexSync)
		//    {
		//        this.UpdateVertexArray();
		//    }

		//    unsafe
		//    {
		//        fixed (T2fC4ubV3fVertex* _VertArrPtr = this.VertexArray)
		//        {
		//            GL.InterleavedArrays(OpenTK.Graphics.OpenGL.InterleavedArrayFormat.T2fC4ubV3f,0,(IntPtr)_VertArrPtr); 

		//            GL.BindTexture(TextureTarget.Texture2D, this.FontAtlasTexID);
		//            GL.Enable(EnableCap.Texture2D);

		//            var _RowCount = this.MaxLineCount;
		//            var _LineVertexCount = this.VertexArray.Length / _RowCount;
					
		//            GL.PushMatrix();
		//            {
		//                var _OffsY1 = -((this.VertexSplitLine + 1) * this.LineHeight);
		//                var _OffsY2 = +this.Height - (this.Height % this.LineHeight);

		//                GL.Translate(0f, _OffsY1, 0f);

		//                for(var cRowI = 0; cRowI < _RowCount; cRowI ++)
		//                {
		//                    //if     (cRowI == 0)                        {Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Green), new RectangleF(0,0,this.Width,this.Height));}
		//                    //else if(cRowI == this.VertexSplitLine + 1) {Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Red), new RectangleF(0,0,this.Width,this.Height));}



		//                    //if     (cRowI == 0)                       {GL.Translate(0f, +_OffsY2, 0f); GL.Color4(Color.Green);}
		//                    //else if(cRowI == this.VertexSplitLine + 1) {GL.Translate(0f, -_OffsY2, 0f); GL.Color4(Color.Red);}


							
		//                    //if     (cRowI == 0)                       {GL.Translate(0f, +_OffsY2, 0f); GL.Color4(Color.Green); Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Green), new RectangleF(0,0,this.Width,this.Height));}
		//                    //else if(cRowI == this.VertexSplitLine + 1) {GL.Translate(0f, -_OffsY2, 0f); GL.Color4(Color.Red);  Screen.DrawRectangle(PrimitiveType.Quads, Color.FromArgb(127, Color.Red), new RectangleF(0,0,this.Width,this.Height));}

		//                    if     (cRowI == 0)                        GL.Translate(0f, +_OffsY2, 0f);
		//                    else if(cRowI == this.VertexSplitLine + 1) GL.Translate(0f, -_OffsY2, 0f);

		//                    this.RenderLine(cRowI, _LineVertexCount);
		//                }
		//            }
		//            GL.PopMatrix();
		//            GL.Disable(EnableCap.Texture2D);
		//            GL.Finish();
		//        }
		//    }
		//}
		//private void RenderLine(int iRowI, int iVertexCount)
		//{
		//    //var cLine     = this.Lines[cRowI];
		//    var _VertOffs = iRowI * iVertexCount;

		//    ///GL.Color4(iLine.Color);
		//    GL.Enable(EnableCap.Texture2D);
		//    GL.DrawArrays(PrimitiveType.Quads, _VertOffs, iVertexCount);
		//    GL.Disable(EnableCap.Texture2D);
		//    //GL.DrawArrays(PrimitiveType.LineLoop, _VertOffs, iVertexCount);

		//    ///iLine.IsNew = false;
		//}
		public void DrawFontAtlas()
		{
			var _W = (float)this.FontAtlas.Width / this.Width;
			var _H = (float)this.FontAtlas.Height / this.Height;


			GL.Begin(PrimitiveType.Quads);
			{
				GL.Color4(1.0,0.0,0.0,1.0);
				
				//GL.TexCoord2(    0,     0); GL.Vertex2(0.0,     0.0);
				//GL.TexCoord2(_RelW,     0); GL.Vertex2(_AbsW,   0.0);
				//GL.TexCoord2(_RelW, _RelH); GL.Vertex2(_AbsW, _AbsH);
				//GL.TexCoord2(    0, _RelH); GL.Vertex2(0.0,   _AbsH);

				GL.TexCoord2(0,0); GL.Vertex2( 0,  0);
				GL.TexCoord2(1,0); GL.Vertex2(_W,  0);
				GL.TexCoord2(1,1); GL.Vertex2(_W, _H);
				GL.TexCoord2(0,1); GL.Vertex2( 0, _H);
			}
			GL.End();
		}
	}
}
