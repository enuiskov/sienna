using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace AE.Visualization
{
	public partial class TextBufferFrame
	{
		///private T2fC4ubV3fVertex[] GlyphVertices;
		private int    GlyphSplitLine  = 0;

		protected bool NeedsVertexSync = true;

		public bool    IsGradientMode  = false;
		public bool    IsTextureMode   = true;

		//public 
		///protected Color4[] Colors;
		public int[]   DefaultColors = new int[]{0,1,2,3,4,5,6,7,8,9,10,11,12};



		public void UpdateSheetImage(bool iDoForceFullUpdate)
		{
			var _Grx = new GraphicsContext(this.SheetImage, this.Palette);

			//_Grx.Clear();

			var _ForeBrush = new SolidBrush(Color.Transparent);
			var _BackBrush = new SolidBrush(Color.Transparent);
			//var _Format = new StringFormat{a};
			var _RegularFont   = this.Settings.Font;
			var _BoldFont      = new Font(_RegularFont, FontStyle.Bold);
			var _ItalicFont    = new Font(_RegularFont, FontStyle.Italic);
			var _StrikeFont    = new Font(_RegularFont, FontStyle.Strikeout);
			var _UnderlineFont = new Font(_RegularFont, FontStyle.Underline);


			_Grx.Device.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

			if(this.Palette.IsLightTheme)
			{
				_Grx.Device.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				//_Grx.Device.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
			}
			else
			{
				_Grx.Device.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
			}


			for(var cRi = 0; cRi < this.Rows.Length; cRi++)
			{
				var cRow = this.Rows[cRi]; if(cRow.IsValidated && !iDoForceFullUpdate) continue;

				_Grx.Device.SetClip(new Rectangle(0,cRi * this.Settings.LineHeight,this.Width, this.Settings.LineHeight), System.Drawing.Drawing2D.CombineMode.Intersect);
				_Grx.Clear();
				_Grx.ResetClip();

				for(var cCi = 0; cCi < cRow.Cells.Length; cCi++)
				{
					var cCell = cRow.Cells[cCi]; if(cCell.Value == 0) continue;
					var cPos  = new Point(cCi * this.Settings.CharWidth, cRi * this.Settings.LineHeight);
					var cForeColor = cCell.Style.ForeColor;
					var cBackColor = cCell.Style.BackColor;

					var cNeedsForeDrawing = cForeColor.A != 0;
					var cNeedsBackDrawing = cBackColor.A != 0;
					
					//cBrush.Color
					///if(cCi > cRi) continue;
					

					if(cNeedsBackDrawing)
					{
						_BackBrush.Color = this.Palette.Adapt(cBackColor,true,true);
						_Grx.FillRectangle(_BackBrush, new RectangleF(cPos.X,cPos.Y,this.Settings.CharWidth * 1.5f,this.Settings.LineHeight));
					}

					if(cNeedsForeDrawing)
					{

						//var _Color = ((Color)cForeColor);
						//if(!this.Palette.IsLightTheme && cForeColor.H == 7)
						//{
						
						//}
						_ForeBrush.Color = this.Palette.Adapt(cForeColor,true,true);

						
						var cFont = _RegularFont; switch(cCell.Style.FontStyle)
						{
							case FontStyle.Bold      : cFont = _BoldFont;      break;
							case FontStyle.Italic    : cFont = _ItalicFont;    break;
							case FontStyle.Strikeout : cFont = _StrikeFont;    break;
							case FontStyle.Underline : cFont = _UnderlineFont; break;
						}
						_Grx.Device.DrawString(cCell.Value.ToString(), cFont, _ForeBrush, cPos);
					}
				}

				cRow.IsValidated = true;
			}

			this.NeedsVertexSync = false;
		}
		public override void DrawForeground(GraphicsContext iGrx)
		{
			if(this.SheetImage == null) return;
			if(this.NeedsVertexSync) this.UpdateSheetImage(false);


			iGrx.Device.SetClip(new Rectangle(0, 0, this.Width, this.BufferSize.Height * this.Settings.LineHeight));

			var _SplitY = this.GlyphSplitLine * this.Settings.LineHeight;
			var _Pos1 = new Point(0,-_SplitY);
			var _Pos2 = new Point(0, (this.BufferSize.Height * this.Settings.LineHeight) - _SplitY);

			//iGrx.Device.DrawImage(this.SheetImage, new Rectangle(_Pos1, this.Bounds.Size));
			//iGrx.Device.DrawImage(this.SheetImage, new Rectangle(_Pos2, this.Bounds.Size));

			var _Rect1 = new Rectangle(_Pos1, this.SheetImage.Size);
			var _Rect2 = new Rectangle(_Pos2, this.SheetImage.Size);

			///iGrx.Device.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
			///iGrx.Device.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
			
			iGrx.Device.DrawImage(this.SheetImage, new Rectangle(_Pos1, this.SheetImage.Size));
			//var _ImgAttr = new System.Drawing.Imaging.ImageAttributes();
			//{
			//   _ImgAttr.
			//}
			//iGrx.Device.DrawImage(this.SheetImage, new Rectangle(_Pos1, this.SheetImage.Size), new Rectangle(Point.Empty, this.SheetImage.Size), GraphicsUnit.Pixel, );

			
			///iGrx.Device.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(255,Color.Green)),4f), _Rect1);

			if(_SplitY != 0)
			{
				iGrx.Device.DrawImage(this.SheetImage,_Rect2);
				///iGrx.Device.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(255,Color.Orange)),2f), _Rect2);
			}
			iGrx.ResetClip();
		}

		//private void CreateFontAtlas()
		//{
		//    if(this.FontAtlas != null)    this.FontAtlas.Dispose();
		//    if(this.FontAtlasTexID != -1) GL.DeleteTexture(this.FontAtlasTexID);


		//    var _CharCount = Math.Pow(this.FontAtlasGrid,2);
		//    var _CellSize  = this.FontAtlasSize / this.FontAtlasGrid;
		//    //var _Font        = new Font("Lucida Console", this.FontSize, FontStyle.Regular);
		//    var _Font      = new Font(FontFamily.GenericMonospace, this.FontSize, FontStyle.Regular);
		//    ///var _Format    = new StringFormat{a};
		//    var _Brush     = new SolidBrush(Color.White);

		//    this.FontAtlas = new Bitmap(this.FontAtlasSize,this.FontAtlasSize);
			
		//    var _Grx = Graphics.FromImage(this.FontAtlas);
		//    {
		//        //_Grx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
		//        //_Grx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
		//        //_Grx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

		//        //_Grx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
		//        ///_Grx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
		//        //_Grx.TextContrast = 0;
		//        ///_Grx.Clear(Color.FromArgb(32,255,255,255));

		//        for(var cCharI = 0; cCharI < _CharCount; cCharI++)
		//        {
		//            var cChar  = (char)cCharI;
		//            var cCellX = (cCharI % this.FontAtlasGrid) * _CellSize;
		//            var cCellY = (cCharI / this.FontAtlasGrid) * _CellSize;

		//            _Grx.SetClip(new Rectangle(cCellX,cCellY, _CellSize, _CellSize));
		//            _Grx.DrawString(cChar.ToString(), _Font, _Brush, cCellX,cCellY);
		//        }
		//    }

		//    this.FontAtlasTexID = GL.GenTexture();
		//    {
		//        GL.BindTexture  (TextureTarget.Texture2D, this.FontAtlasTexID);
		//        GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, this.FontAtlasSize, this.FontAtlasSize, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
		//        GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
		//        GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
				
		//        var _Data = this.FontAtlas.LockBits
		//        (
		//            new Rectangle(0, 0, this.FontAtlasSize, this.FontAtlasSize),
		//            System.Drawing.Imaging.ImageLockMode.ReadOnly,
		//            System.Drawing.Imaging.PixelFormat.Format32bppArgb
		//        );
		//        GL.TexSubImage2D
		//        (
		//            TextureTarget.Texture2D, 0,
		//            0,0,this.FontAtlasSize, this.FontAtlasSize,
		//            PixelFormat.Bgra,
		//            PixelType.UnsignedByte,
		//            _Data.Scan0
		//        );
		//        this.FontAtlas.UnlockBits(_Data);
		//    }
		//    ///this.FontAtlas.Save("FontAtlas.png");
		//}
		
		protected virtual void UpdateShapes(bool iForceUpdate)
		{
			
		}
		protected virtual void UpdateGlyphs(bool iForceUpdate)
		{
			//var _AtlasGrid  = this.FontAtlas.Grid;
			//var _AtlasCell  = this.FontAtlas.Size / this.FontAtlas.Grid;

			//var _TxCellSize = 1f / _AtlasGrid; //per cell
			
			//var _QuadW    = (float)_AtlasCell;
			//var _QuadH    = _QuadW;
			//var _QStepX   = this.FontAtlas.CharWidth;
			//var _QStepY   = this.FontAtlas.LineHeight;
			//var _ColCount = this.BufferSize.Width;
			//var _RowCount = this.BufferSize.Height;
			
			////var _ColCount   = this.Width / (int)_QStepX;
			
			//var _DoSyncAllRows = this.IsGradientMode || iForceUpdate || this.GlyphVertices == null;
			//var _VertexArray   = _DoSyncAllRows ? new T2fC4ubV3fVertex[_RowCount * _ColCount * 4] : this.GlyphVertices;
			//{
			//    ///var _SplitRow = this.GlyphSplitLine;
			//    //var _Color = Color.White;
			//    //for(var cRow = 0; cRow < _RowCount; cRow ++)
			//    for(var cBufRowI = 0; cBufRowI < _RowCount; cBufRowI ++)
			//    ///for(int cRow = this.VertexSplitLine; cRow != this.VertexSplitLine - 1; cRow = (cRow % this.Lines.Count) + 1)
			//    {
			//        //var cLine = this.Lines[cLineI]; if(!(cLine.IsNew || _DoSyncAllLines)) continue;
					
			//        var cBufferRow  = this.Rows[cBufRowI]; if(!_DoSyncAllRows && cBufferRow.IsValidated) continue;/// if(!(cBufferRow.IsNew || _DoSyncAllRows)) continue;
			//        var cVertexRowI = cBufferRow.VertexRowIndex;

			//        //if(this.IsGra
			//        //var cGradValue  = (float)cBufRowI / _RowCount;
			//        //var cCell
			//        //var cStr   = cRow.ToString() + (cRow < 10 ? " " : "") + " | " + cLine.String;

			//        //var cColor = Color4.FromColor(cLine.Color);
					

			//        for(var cCol = 0; cCol < _ColCount; cCol++)
			//        {
			//            var cBufferCell = cBufferRow.Cells[cCol];
			//            var cStyle      = cBufferCell.Style;
			//            var cChar       = cBufferCell.Value;
			//            var cBackColor  = cStyle.BackColorBytes;
			//            var cForeColor  = cStyle.ForeColorBytes;
			//            {
			//                //if(cStyle.Opacity < 1f)
			//                //{
			//                //    //cForeColor = Color.FromArgb((int)(cStyle.Opacity * cForeColor.A), cForeColor);
			//                //    //cBackColor = Color.FromArgb((int)(cStyle.Opacity * cBackColor.A), cBackColor);

			//                //    cForeColor.A = (byte)(cForeColor.A * cStyle.Opacity);
			//                //    cBackColor.A = (byte)(cBackColor.A * cStyle.Opacity);
			//                //}

							

			//                if(this.IsGradientMode)
			//                {

			//                    cBackColor.A = (byte)(cBackColor.A * cBufferRow.Opacity);
			//                    cForeColor.A = (byte)(cForeColor.A * cBufferRow.Opacity);
			//                }
			//            }
						

			//            //var cChar  = cCol < cStr.Length ? cStr[cCol] : ' ';
			//            //var cChar  = cStr[cCol] : ' ';
						
						
			//            var cAtlasCellI = (int)cChar;//43;//
			//            var cVertexI    = (cVertexRowI * (_ColCount * 4)) + (cCol * 4);

			//            if(cAtlasCellI != 32)
			//            {
			//                var cTexBB = new RectangleF
			//                (
			//                    (cAtlasCellI % _AtlasGrid) * _TxCellSize,
			//                    (cAtlasCellI / _AtlasGrid) * _TxCellSize, 

			//                    _TxCellSize,_TxCellSize
			//                );
			//                var cX  = cCol        * _QStepX;
			//                var cY  = cVertexRowI * _QStepY;
							
			//                //if(this.IsGradientMode)
			//                //{
			//                    _VertexArray[cVertexI + 0] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Left,  cTexBB.Top),    cForeColor, new Vector3(cX,          cY,          0));
			//                    _VertexArray[cVertexI + 1] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Right, cTexBB.Top),    cForeColor, new Vector3(cX + _QuadW, cY,          0));
			//                    _VertexArray[cVertexI + 2] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Right, cTexBB.Bottom), cForeColor, new Vector3(cX + _QuadW, cY + _QuadH, 0));
			//                    _VertexArray[cVertexI + 3] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Left,  cTexBB.Bottom), cForeColor, new Vector3(cX,          cY + _QuadH, 0));
			//                //}
			//            }
			//            else
			//            {
			//                _VertexArray[cVertexI + 0] = T2fC4ubV3fVertex.Offscreen;
			//                _VertexArray[cVertexI + 1] = T2fC4ubV3fVertex.Offscreen;
			//                _VertexArray[cVertexI + 2] = T2fC4ubV3fVertex.Offscreen;
			//                _VertexArray[cVertexI + 3] = T2fC4ubV3fVertex.Offscreen;
			//            }
			//        }

			//        cBufferRow.IsValidated = true;
			//        ///cLine.IsNew = false;
			//    }
			//}
			
			//this.GlyphVertices = _VertexArray;
			//this.NeedsVertexSync = false;
		}
		
		///public override void Render()
		public /**override*/ void RenderOLD()
		{
			if(this.Opacity != 0)
			{
				///GLCanvasControl.Routines.Drawing.DrawGradient (PrimitiveType.Quads, new RectangleF(0,0, this.Width, this.Height), Color.FromArgb((int)(this.Opacity * 255), this.Palette.BackGradTopColor), Color.FromArgb((int)(this.Opacity * 255), this.Palette.BackGradBottomColor), true);
				///GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.LineLoop, this.Palette.ForeColor, 0, 0, this.Width, this.Height);
			}

			if(this.BufferSize.Height == 0) return;


			if(this.Canvas.Control is GdiCanvasControl)
			{
				
			}
			else
			{
				base.DefaultRender();
			}

			if(this.IsTextureMode) this.RenderGlyphs();
			else                   this.RenderShapes();
			
			//GL.Blend
			//this.DrawCursor();
			//this.DrawFontAtlas();
		}
		public void RenderShapes()
		{
			//GL.Enable(EnableCap.LineSmooth);
			////GL.LineWidth(

			//GL.PushMatrix();
			//GL.Scale(this.FontAtlas.CharWidth, this.FontAtlas.LineHeight, 1);
			//GL.Scale(0.8, 0.8, 1);
					
			//for(var cRi = 0; cRi < this.Rows.Length; cRi++)
			//{
			//    var cRow = this.Rows[cRi];

			//    GL.PushMatrix();
			//    for(var cCi = 0; cCi < cRow.Cells.Length; cCi++)
			//    {
			//        var cBufferCell = cRow.Cells[cCi];
			//        var cCellIndex  = (int)cBufferCell.Value;

			//        var cFontCell = this.FontData.Cells[cCellIndex];


			//        GL.Color4(cBufferCell.Style.ForeColor);
			//        GLCanvasControl.Routines.Rendering.DrawCell(this, cFontCell, false, 1f, 0);

			//        GL.Translate(1.0, 0, 0);
			//    }
			//    GL.PopMatrix();

			//    GL.Translate(0, 1.0, 0);
			//}
			//GL.PopMatrix();
			////this.Rows[0].
			////this.FontAtlas.CharWidth;
			////this.FontAtlas.LineHeight;
		}
		public void RenderGlyphs()
		{
			////if(DateTime.Now.Millisecond >= 980)
			//if(this.NeedsVertexSync)
			//{
			//    if(this.IsGradientMode)
			//    {
			//        var _RowCount = this.Rows.Length;
			//        var _SplitI   = this.GlyphSplitLine;
			//        var _AttenF   = 10.0; 

			//        for(var cRi = 0; cRi < _RowCount; cRi++)
			//        {
			//            var cIsBefSplit    = cRi < _SplitI;
			//            var cPosRelToSplit = (double)(cRi - _SplitI % _RowCount) / _RowCount;
			//            var cLinAlpha      = cIsBefSplit ? 1f + cPosRelToSplit : cPosRelToSplit;
			//            var cExpAlpha      = Math.Max(0, Math.Log(cLinAlpha * _AttenF, _AttenF));
						
			//            this.Rows[cRi].Opacity = (float)cExpAlpha;
			//        }
			//    }
			//    this.UpdateGlyphs(false);
			//}

			//unsafe
			//{
			//    fixed (T2fC4ubV3fVertex* _VertArrPtr = this.GlyphVertices)
			//    {
			//        GL.InterleavedArrays(OpenTK.Graphics.OpenGL.InterleavedArrayFormat.T2fC4ubV3f,0,(IntPtr)_VertArrPtr); 

			//        GL.BindTexture(TextureTarget.Texture2D, this.FontAtlas.TexID);

			//        var _LineVertexCount = this.GlyphVertices.Length / this.BufferSize.Height;

			//        //var _RowCount        = this.BufferSize.Height;
			//        //var _LineVertexCount = this.VertexArray.Length / _RowCount;
				
					
					
			//        GL.Enable(EnableCap.Texture2D);


			//        //GL.DrawArrays(PrimitiveType.Quads, 0, this.VertexArray.Length);

			//        //GL.Disable(EnableCap.Texture2D);
			//        //GL.DrawArrays(PrimitiveType.LineLoop, 0, this.VertexArray.Length);
			//        //GL.Enable(EnableCap.Texture2D);

			//        GL.PushMatrix();
			//        {
			//            //var _OffsY1 = -((this.VertexSplitLine + 1) * this.LineHeight);
			//            //var _OffsY2 = +this.Height - (this.Height % this.LineHeight);
			//            //var _SplitI = (this.VertexSplitLine + 1) * _LineVertexCount;

			//            var _OffsY1 = -(this.GlyphSplitLine * this.FontAtlas.LineHeight);
			//            var _OffsY2 = +this.Height - (this.Height % this.FontAtlas.LineHeight);
			//            var _SplitI = this.GlyphSplitLine * _LineVertexCount;

			//            GL.Translate(0f, +_OffsY1, 0f);

			//            ///Screen.DrawGradient(PrimitiveType.Quads, new RectangleF(0,0,this.Width,this.Height), Color.Transparent, Color.FromArgb(127,100,255,0), false);
			//            GL.DrawArrays(PrimitiveType.Quads, _SplitI, this.GlyphVertices.Length - _SplitI);

			//            //GL.Disable(EnableCap.Texture2D);
			//            //GL.DrawArrays(PrimitiveType.LineLoop, _SplitI, this.VertexArray.Length - _SplitI);
			//            //GL.Enable(EnableCap.Texture2D);

			//            GL.Translate(0f, +_OffsY2, 0f);
			//            ///Screen.DrawGradient(PrimitiveType.Quads, new RectangleF(0,0,this.Width,this.Height), Color.Transparent, Color.FromArgb(127,255,100,0), false);
			//            GL.DrawArrays(PrimitiveType.Quads, 0, _SplitI);

			//            //GL.Disable(EnableCap.Texture2D);
			//            //GL.DrawArrays(PrimitiveType.LineLoop, 0, _SplitI);
			//            //GL.Enable(EnableCap.Texture2D);

			//        }
			//        GL.PopMatrix();
			//        GL.Disable(EnableCap.Texture2D);

					
			//        GL.Finish();
			//    }
			//}

		}
		//private void DrawCursor()
		//{
		//    var _CrsW = this.CharWidth;
		//    var _CrsH = this.LineHeight;
		//    var _CrsX = this.Curs
			
		//    //var _CrsW = ;
		//    //var _Rect = new RectangleF(0,0,100,100);
		//    Screen.DrawRectangle(PrimitiveType.Quads, Color.Yellow, new RectangleF(0,0,100,100));
		//}
		//private void RenderLine(int iRowI, int iVertexCount)
		//{
		//    //var cLine     = this.Lines[cRowI];
		//    var _VertOffs = iRowI * iVertexCount;

		//    ///GL.Color4(iLine.Color);
		//    //GL.Enable(EnableCap.Texture2D);
		//    GL.DrawArrays(PrimitiveType.Quads, _VertOffs, iVertexCount);
		//    //GL.Disable(EnableCap.Texture2D);
		//    //GL.DrawArrays(PrimitiveType.LineLoop, _VertOffs, iVertexCount);

		//    ///iLine.IsNew = false;
		//}
		
		public void DrawFontAtlas()
		{
			////GL.Enable(EnableCap.Texture2D);
			////Screen.DrawRectangle(PrimitiveType.Quads, Color.White, 0,0,this.FontAtlas.Width,this.FontAtlas.Height);
			//var _W = this.FontAtlas.Image.Width;//.. / this.Width;
			//var _H = this.FontAtlas.Image.Height;// / this.Height;

			//GL.Enable(EnableCap.Texture2D);
			//GL.Begin(PrimitiveType.Quads);
			//{
			//    GL.Color4(1.0,1.0,0.0,1.0);
				
			//    //GL.TexCoord2(    0,     0); GL.Vertex2(0.0,     0.0);
			//    //GL.TexCoord2(_RelW,     0); GL.Vertex2(_AbsW,   0.0);
			//    //GL.TexCoord2(_RelW, _RelH); GL.Vertex2(_AbsW, _AbsH);
			//    //GL.TexCoord2(    0, _RelH); GL.Vertex2(0.0,   _AbsH);

			//    GL.TexCoord2(0,0); GL.Vertex2( 0,  0);
			//    GL.TexCoord2(1,0); GL.Vertex2(_W,  0);
			//    GL.TexCoord2(1,1); GL.Vertex2(_W, _H);
			//    GL.TexCoord2(0,1); GL.Vertex2( 0, _H);
			//}
			//GL.End();
			//GL.Disable(EnableCap.Texture2D);
		}
	}
}
