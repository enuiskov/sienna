using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Data;
using System.Diagnostics;
//using System.Text;
using System.Windows.Forms;
using OpenTK;
//using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public class Texture2D
	{
		public string        FilePath;
		public Bitmap        Image;

		public TextureTarget Type;
		public int           Size = -1;
		public int           Id   = -1;

		public Texture2D(string iFilePath)
		{
			this.FilePath = iFilePath;
			this.Image    = (Bitmap)Bitmap.FromFile(iFilePath);
			this.Type  = TextureTarget.Texture2D;
			this.Size  = -1;
			this.Id    = -1;
		}
		public void Bind()
		{
			if(this.Id == -1)
			{
				this.CreateTexture();
				this.UpdateTextureData();
			}

			GL.BindTexture(this.Type, this.Id);
			//GL.BindTexture(TextureTarget.Texture2D, _MyTexture.Id);
		}

		public static Texture2D FromFile(string iFilePath)
		{
			return new Texture2D(iFilePath);
		}

		public void CreateTexture()
		{
			this.Id   = GL.GenTexture();
			this.Size = (int)Math.Pow(2, Math.Ceiling(Math.Log(Math.Max(this.Image.Width, this.Image.Height),2)));

			{
				GL.BindTexture  (TextureTarget.Texture2D, this.Id);
				GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, this.Size, this.Size, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
				//GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _TexSize, _TexSize, 0, PixelFormat.Bgra, PixelType.Bitmap, IntPtr.Zero);
				//GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Mipmap);

				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, (int)All.True);
			}

			//Console.WriteLine("-CreTex : " + _TexSize);

			//this.UpdateTextureProperties(_TexID, _TexSize, -1);
		}
		public  void DeleteTexture()
		{
			//Console.WriteLine("-DelTex");
			
			GL.DeleteTexture(this.Id);

			//this.UpdateTextureProperties(-1,-1,-1);
		}
		
		//public  void UpdateTextureProperties(int iTexID, int iTexSize, int iAtlasRegion)
		//{
		//    this.TexID       = iTexID;
		//    this.TexSize     = iTexSize;
		//    this.AtlasRegion = iAtlasRegion;
		//}
		public  void UpdateTextureData()
		{
		    var _Data = this.Image.LockBits
		    (
				//new Rectangle(0, 0, this.Image.Width, 3),
				new Rectangle(0, 0, this.Image.Width, this.Image.Height),
		        System.Drawing.Imaging.ImageLockMode.ReadOnly,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb
				//System.Drawing.Imaging.PixelFormat.Format32bppPArgb
		    );
		    GL.BindTexture(this.Type, this.Id);
			
		    GL.TexSubImage2D
		    (
		        TextureTarget.Texture2D, 0,
				//0,(int)((float)DateTime.Now.Millisecond*0.3), this.Image.Width, 3,
				0,0,this.Image.Width, this.Image.Height,
		        PixelFormat.Bgra,
				PixelType.UnsignedByte,
		        _Data.Scan0
		    );
		    this.Image.UnlockBits(_Data);
		}
	}
	public class GlyphAtlas : IDisposable
	{
		public int    Size; ///~~ power of 2;
		public int    Grid; ///~~ power of 2;
		public Bitmap Image;
		public int    TexID;

		public bool   IsTexSmoothEnabled;
		public bool   IsMipmapEnabled;


		public GlyphAtlas() : this(1024, 64){}
		public GlyphAtlas(int iSize, int iGrid)
		{
			this.Size = iSize;
			this.Grid = iGrid;
			this.Image = new Bitmap(iSize,iSize);
			this.TexID = -1;

			//this.CreateImage();
			//this.CreateTexture();
		}

		public virtual void CreateImage()
		{
			if(this.Image != null)    this.Image.Dispose();
			
			this.Image = new Bitmap(this.Size,this.Size);
			
			var _Grx = Graphics.FromImage(this.Image);
			{
				//for(var cCharI = 0; cCharI < _CharCount; cCharI++)
				//{
				//    var cChar  = (char)cCharI;
				//    var cCellX = (cCharI % this.FontAtlasGrid) * _CellSize;
				//    var cCellY = (cCharI / this.FontAtlasGrid) * _CellSize;

				//    _Grx.SetClip(new Rectangle(cCellX,cCellY, _CellSize, _CellSize));
				//    _Grx.DrawString(cChar.ToString(), _Font, _Brush, cCellX,cCellY);
				//}
			}

			
		}
		public virtual void CreateTexture()
		{
			if(this.TexID != -1) GL.DeleteTexture(this.TexID);

			this.TexID = GL.GenTexture();
			{
				GL.BindTexture  (TextureTarget.Texture2D, this.TexID);
				GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, this.Size, this.Size, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)(this.IsTexSmoothEnabled ? All.Linear : All.Nearest));
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)(this.IsTexSmoothEnabled ? All.Linear : All.Nearest));
				

				if(this.IsMipmapEnabled)
				{
					GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, (int)All.True);
				}
				//else
				//{
					

				//    if(this.IsTexSmoothEnabled)
				//    {
						
				//    }
				//    else
				//    {
						
				//    }
				//}
		        //glTexParameteri(GL_TEXTURE_2D, GL_GENERATE_MIPMAP, GL_TRUE );






				var _Data = this.Image.LockBits
				(
					new Rectangle(0, 0, this.Size, this.Size),
					System.Drawing.Imaging.ImageLockMode.ReadOnly,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb
				);
				GL.TexSubImage2D
				(
					TextureTarget.Texture2D, 0,
					0,0,this.Size, this.Size,
					PixelFormat.Bgra,
					PixelType.UnsignedByte,
					_Data.Scan0
				);
				this.Image.UnlockBits(_Data);
			}
		}
		public void Dispose()
		{
			if(this.TexID != -1) GL.DeleteTexture(this.TexID);
			if(this.Image != null) this.Image.Dispose();

		}
	}
	public class FontGlyphAtlas : GlyphAtlas
	{
		public string            FontName;
		public float             FontSize;
		public int               CharOffset;
		public int               TextContrast = 0;
		public TextRenderingHint TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
		public int               BackBlurRadius  = 1;
		public float             BackBlurOpacity = 1f;
		//public float  FontSize;
		public float             CharWidth; //{get{return this.FontSize * 0.7f;}}
		public float             LineHeight;//{get{return this.FontSize + 2f;}}

		public FontGlyphAtlas() : this(10.5f){}
		public FontGlyphAtlas(float iFontSize) : this(null,iFontSize, 1024,64)
		{
			//this.FontSize   = iFontSize;
			//this.CharWidth  = this.FontSize * 0.8f;
			//this.LineHeight = this.FontSize + 5f;
		}
		public FontGlyphAtlas(string iFontName, float iFontSize, int iAtlasSize, int iAtlasGrid) : base(iAtlasSize,iAtlasGrid)
		{
			this.FontName   = iFontName ?? "Lucida Console";
			this.FontSize   = iFontSize;
			this.CharOffset = 0;
			this.CharWidth  = this.FontSize * 0.75f;
			this.LineHeight = this.FontSize * 1.43f;
		}

		public override void CreateImage()
		{
			var _CharCount = Math.Pow(this.Grid,2);
			var _CellSize  = this.Size / this.Grid;
			var _Font      = new Font(this.FontName, this.FontSize, FontStyle.Regular);
			var _Format    = new StringFormat{Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};

			var _IsBlurAlreadyProcessed = false;
			

			//var _Brush     = new SolidBrush(Color.White);
			//var _BackBrush = new SolidBrush(Color.FromArgb(128,Color.White));

			//this.FontAtlas = new Bitmap(this.Size,this.Size);
			
			//var _Opacity = 255f;
			var _Grx = Graphics.FromImage(this.Image);
			TextDrawing:
			{
				var _Brush = new SolidBrush(Color.FromArgb((int)((!_IsBlurAlreadyProcessed ? this.BackBlurOpacity : 1) * 255), Color.White));

				//if(!_IsBlurAlreadyProcessed)
				//{
				//    _Brush = new SolidBrush(Color.FromArgb((int)(this.BackBlurOpacity * 255), Color.White));
				//}
				//_Grx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				//_Grx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				//_Grx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
				//_Grx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

				_Grx.TextRenderingHint = this.TextRenderingHint;
				_Grx.TextContrast = this.TextContrast;

				///_Grx.Clear(Color.FromArgb(16,255,255,255));
				_Grx.Clear(Color.FromArgb(0,255,255,255));

				//_Grx.Clear(Color.FromArgb(0,255,255,255));
				//_Grx.Clear(Color.FromArgb(0,255,0,0));

				for(var cCharI = 0; cCharI < _CharCount; cCharI++)
				{
					var cChar  = (char)(this.CharOffset + cCharI);
					var cCellX = (cCharI % this.Grid) * _CellSize;
					var cCellY = (cCharI / this.Grid) * _CellSize;

					//var cBackBrush = new System.Drawing.Drawing2D.LinearGradientBrush
					//(
					//    new RectangleF(cCellX, cCellY, 10, this.LineHeight),
						
					//    Color.FromArgb(64,Color.White),
					//    Color.FromArgb(0,Color.White),

					//    -90
					//);

					_Grx.SetClip(new Rectangle(cCellX,cCellY, _CellSize, _CellSize));
					//_Grx.FillRectangle
					//(
					//    cBackBrush,
					//    cCellX + 2f, cCellY, _CellSize * 0.5f, _CellSize * 0.9f
					//);
					_Grx.DrawString
					(
						cChar.ToString(), _Font, _Brush,
						cCellX,// + (_CellSize / 2),
						cCellY//, + (_CellSize / 2),
						//_Format
					);
				}
				_Grx.Flush();
				
				if(this.BackBlurRadius != 0 && !_IsBlurAlreadyProcessed)
				{
					//var _SrcImage = this.Image;
					//var _BluredImage = new Bitmap(this.Image, new Size((int)(this.Image.Width * (1f / this.BackBlurRadius)), (int)(this.Image.Height * (1f / this.BackBlurRadius))));
					//_BluredImage.Save("Blured.png");
					//_Grx.DrawImage(_BluredImage, new Rectangle(Point.Empty,this.Image.Size));
					
					//_BluredImage.Dispose();
					///this.ProcessBlur(this.BackBlurRadius);
					///_IsBlurAlreadyProcessed = true;

					
					///goto TextDrawing;
				}
			}

			///this.Image.Save("FontAtlas.png");
			//this.ProcessBlur(this.Image);
		}

		/**
			// source channel, target channel, width, height, radius
			function gaussBlur_1 (scl, tcl, w, h, r) {
				var rs = Math.ceil(r * 2.57);     // significant radius
				for(var i=0; i<h; i++)
					for(var j=0; j<w; j++) {
						var val = 0, wsum = 0;
						for(var iy = i-rs; iy<i+rs+1; iy++)
							for(var ix = j-rs; ix<j+rs+1; ix++) {
								var x = Math.min(w-1, Math.max(0, ix));
								var y = Math.min(h-1, Math.max(0, iy));
								var dsq = (ix-j)*(ix-j)+(iy-i)*(iy-i);
								var wght = Math.exp( -dsq / (2*r*r) ) / (Math.PI*2*r*r);
								val += scl[y*w+x] * wght;  wsum += wght;
							}
						tcl[i*w+j] = Math.round(val/wsum);            
					}
			}
		
		*/
		public void ProcessBlur(int iRadius)
		{
			var _SrcImage = (Bitmap)this.Image.Clone();//new Rectangle(Point.Empty, this.Image.Size), this.Image.PixelFormat);
			var _TgtImage = this.Image;

			var _W = this.Image.Width;
			var _H = this.Image.Height;

			var _GridArea = (iRadius + 1) * (iRadius + 1);
			
			for(var caY = 0; caY < _H; caY++)
			{
				for(var caX = 0; caX < _W; caX++)
				{
					var cSum = _SrcImage.GetPixel(caX,caY).A;

					//for(var cRound = 1; cRound <= iRadius; cRound++)
					//{
						for(var cHo = -iRadius; cHo < +iRadius; cHo++)
						{
							var cbY = MathEx.Clamp(caY + cHo, 0, _H - 1);

							for(var cWo = -iRadius; cWo < +iRadius; cWo++)
							{
								if(cHo == 0 && cWo == 0) continue;

								var cbX = MathEx.Clamp(caX + cWo, 0, _W - 1);

								var cSrcColor = _SrcImage.GetPixel(cbX,cbY);

								cSum += cSrcColor.A;
							}
						}
						//var 
						//CrossAppDomainDelegate
					//}
					var cAvgValue = (float)cSum / _GridArea;

					var cAvgColor = Color.FromArgb((int)cAvgValue, Color.White);
					//{
					//    cAvgColor = Color.FromArgb(128, cAvgColor);
					//}
					_TgtImage.SetPixel(caX, caY, cAvgColor);
				}
			}

			
			//var _OldImage = this.Image;
			//this.Image = new Bitmap(_OldImage, this.Image.Size * 0.1);
			//_OldImage.Dispose();
		}
		//public void ProcessBlur(int iRadius)
		//{
		//    var _SrcImage = (Bitmap)this.Image.Clone();
		//    var _TgtImage = this.Image;

		//    var _W = this.Image.Width;
		//    var _H = this.Image.Height;
			
		//    var _SignRadius = Math.Ceiling(iRadius * 2.57); 


		//    var rs = Math.Ceiling(r * 2.57);     // significant radius
		//    for(var i=0; i<h; i++) for(var j=0; j<w; j++)
		//    {
		//        var val = 0, wsum = 0;
		//        for(var iy = i-rs; iy<i+rs+1; iy++) for(var ix = j-rs; ix<j+rs+1; ix++)
		//        {
		//            var x = Math.Min(w-1, Math.Max(0, ix));
		//            var y = Math.Min(h-1, Math.Max(0, iy));
		//            var dsq = (ix-j)*(ix-j)+(iy-i)*(iy-i);
		//            var wght = Math.Exp( -dsq / (2*r*r) ) / (Math.PI*2*r*r);

		//            ///val += scl[y*w+x] * wght;
		//            val += _SrcImage.GetPixel(x,y).A * wght;
					
		//            wsum += wght;
		//        }
		//        ///tcl[i*w+j] = Math.Round(val/wsum);
		//        tcl[i*w+j] = Math.Round(val/wsum);
		//    }
		
		//    //var _OldImage = this.Image;
		//    //this.Image = new Bitmap(_OldImage, this.Image.Size * 0.1);
		//    //_OldImage.Dispose();
		//}
		//public void ProcessBlur(int iRadius)
		//{
		//    var _SrcImage = this.Image.Clone();
		//    var _TgtImage = this.Image;

		//    var _W = this.Image.Width;
		//    var _H = this.Image.Height;
			
		//    var _SignRadius = Math.Ceiling(iRadius * 2.57); 

		//    for(var cY = 0; cY < _H; cY++)
		//    {
		//        for(var cX = 0; cX < _W; cX++)
		//        {
		//            var val = 0, wsum = 0;

		//            for(var iy = i - rs; iy < i + rs + 1; iy ++)
		//            {
		//                for(var ix = j - rs; ix < j + rs + 1; ix ++)
		//                {
		//                    var x = Math.Min(_W - 1, Math.max(0, ix));
		//                    var y = Math.Min(_H - 1, Math.max(0, iy));
		//                    var dsq = (ix - j) * (ix - j) + (iy - i) * (iy - i);
		//                    var wght = Math.Exp(-dsq / (2 * r * r) ) / (Math.PI * 2 * r * r);
		//                    val += scl[y * w + x] * wght;
							
		//                    wsum += wght;
		//                }
		//            }
		//            _TgtImage[i*w+j] = Math.Round(val/wsum);        
		//        }
		//    }
		//    //var _OldImage = this.Image;
		//    //this.Image = new Bitmap(_OldImage, this.Image.Size * 0.1);
		//    //_OldImage.Dispose();
		//}
		//public override void CreateImage()
		//{
		//    var _CharCount = Math.Pow(this.Grid,2);
		//    var _CellSize  = this.Size / this.Grid;
		//    var _Font      = new Font(this.FontName, this.FontSize, FontStyle.Regular);
		//    var _Format    = new StringFormat{Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
		//    var _Brush     = new SolidBrush(Color.White);
		//    var _BackBrush = new SolidBrush(Color.FromArgb(128,Color.White));

		//    //this.FontAtlas = new Bitmap(this.Size,this.Size);
			
		//    var _Grx = Graphics.FromImage(this.Image);
		//    {
		//        //_Grx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
		//        //_Grx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
		//        //_Grx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

		//        //_Grx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

		//        _Grx.TextRenderingHint = this.TextRenderingHint;
		//        _Grx.TextContrast = this.TextContrast;


		//        ///_Grx.Clear(Color.FromArgb(16,255,255,255));
		//        //_Grx.Clear(Color.FromArgb(0,255,255,255));
		//        //_Grx.Clear(Color.FromArgb(0,255,0,0));

		//        for(var cCharI = 0; cCharI < _CharCount; cCharI++)
		//        {
		//            var cChar  = (char)cCharI;
		//            var cCellX = (cCharI % this.Grid) * _CellSize;
		//            var cCellY = (cCharI / this.Grid) * _CellSize;

		//            var cBackBrush = new System.Drawing.Drawing2D.LinearGradientBrush
		//            (
		//                new RectangleF(cCellX, cCellY, 10, this.LineHeight),
						
		//                Color.FromArgb(64,Color.White),
		//                Color.FromArgb(0,Color.White),

		//                -90
		//            );

		//            _Grx.SetClip(new Rectangle(cCellX,cCellY, _CellSize, _CellSize));
		//            _Grx.FillRectangle
		//            (
		//                cBackBrush,
		//                cCellX + 2f, cCellY, _CellSize * 0.5f, _CellSize * 0.9f
		//            );
		//            _Grx.DrawString
		//            (
		//                cChar.ToString(), _Font, _Brush,
		//                cCellX,// + (_CellSize / 2),
		//                cCellY//, + (_CellSize / 2),
		//                //_Format
		//            );
		//        }
		//    }
		//}
	}
	public class VertexText
	{
		//public Size BufferSize = new Size(10,10);

		public int HorizontalAlign = 0;
		public int VerticalAlign   = 0;

		private string Value_ = "Hello";
		public  string Value {get{return this.Value_;} set{this.Value_ = value; this.NeedsVertexSync = true;}}
		public Vector3d Position = Vector3d.Zero;
		public int      Color = 2;

		//public  bool IsValueUpdated = true;
		public  bool NeedsVertexSync = true;
		

		public T2fC4ubV3fVertex[] VertexArray;

		public VertexText()
		{
			///this.Position = new Vector3d(0.0,0,0);
			//this.BufferSize = 
		}

		//public void UpdateText()
		//{
			

		//    this.IsValueUpdated = false;
		//}
		public void Draw()
		{
			//Screen.Routines.Drawing.DrawText(this.Text);
			//Screen.Routines.Drawing.DrawText(this.Text);
			GLCanvasControl.Routines.Drawing.DrawVertexArray(this.VertexArray, GLCanvasControl.HighResFontAtlas.TexID);
		}
		public virtual void UpdateVertexArray()
		{
			var _NewVertexCount = this.Value.Length * 4;
			var _Lines          = this.Value_.Split(new string[]{"\r\n"}, StringSplitOptions.None);
			
			var _MaxLineLength  = 0; foreach(var cLine in _Lines) if(cLine.Length > _MaxLineLength) _MaxLineLength = cLine.Length;
			

			var _Color     = ColorPalette.Default.Colors[this.Color];
			var _FontAtlas = GLCanvasControl.HighResFontAtlas;


			//var _AtlasGrid  = _FontAtlas.Grid;
			var _AtlasCell  = _FontAtlas.Size / _FontAtlas.Grid;

			var _TxCellSize = 1f / _FontAtlas.Grid; //per cell
			var _OffsX    = (float)this.Position.X;
			var _OffsY    = (float)this.Position.Y;
			var _Scale    = 0.005f;
			var _QuadW    = (float)_AtlasCell * _Scale;
			var _QuadH    = _QuadW;
			var _QStepX   = _FontAtlas.CharWidth * _Scale;
			var _QStepY   = _FontAtlas.LineHeight * _Scale;
			//var _ColCount = this.BufferSize.Width;
			//var _RowCount = _Rows.Length;
			//var _Color =
			
			if(this.VertexArray == null || this.VertexArray.Length != _NewVertexCount)
			{
				this.VertexArray = new T2fC4ubV3fVertex[_NewVertexCount];
			}


			var cCharI = 0; for(var cLi = 0; cLi < _Lines.Length; cLi ++)
		    {
				var cLine    = _Lines[cLi];
				var cIndent  = cLine.Length / _MaxLineLength;
				
				for(var cCi = 0; cCi < cLine.Length; cCi++)
				{
					//var 
					//var cBufferCell = cBufferRow.Cells[cCol];
					//var cStyle      = cBufferCell.Style;
					var cChar       = cLine[cCi];
					var cColor      = _Color;
					//var cBackColor  = cStyle.BackColorBytes;
					//var cForeColor  = cStyle.ForeColorBytes;
					/**
						TextAlign =  0;

						        Name
						      Node Type
						Description goes here
					
					*/
					
					var cAtlasCellI = (int)cChar;//43;//
					var cVertexI    = cCharI * 4;

					if(cAtlasCellI != 32)
					{
						var cTexBB = new RectangleF
						(
							(cAtlasCellI % _FontAtlas.Grid) * _TxCellSize,
							(cAtlasCellI / _FontAtlas.Grid) * _TxCellSize, 

							_TxCellSize,_TxCellSize
						);
						var cX  = +(_OffsX + (cCi * _QStepX));
						var cY  = -(_OffsY + (cLi * _QStepY));
						
						this.VertexArray[cVertexI + 0] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Left,  cTexBB.Top),    cColor, new Vector3(cX,          cY + _QuadH,          0));
						this.VertexArray[cVertexI + 1] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Right, cTexBB.Top),    cColor, new Vector3(cX + _QuadW, cY + _QuadH,          0));
						this.VertexArray[cVertexI + 2] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Right, cTexBB.Bottom), cColor, new Vector3(cX + _QuadW, cY,          0));
						this.VertexArray[cVertexI + 3] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Left,  cTexBB.Bottom), cColor, new Vector3(cX,          cY,          0));

						//this.VertexArray[cVertexI + 0] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Left,  cTexBB.Top),    cColor, new Vector3(cX,          cY,          0));
						//this.VertexArray[cVertexI + 1] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Right, cTexBB.Top),    cColor, new Vector3(cX + _QuadW, cY,          0));
						//this.VertexArray[cVertexI + 2] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Right, cTexBB.Bottom), cColor, new Vector3(cX + _QuadW, cY + _QuadH, 0));
						//this.VertexArray[cVertexI + 3] = new T2fC4ubV3fVertex(new Vector2(cTexBB.Left,  cTexBB.Bottom), cColor, new Vector3(cX,          cY + _QuadH, 0));
					}
					else
					{
						this.VertexArray[cVertexI + 0] = T2fC4ubV3fVertex.Offscreen;
						this.VertexArray[cVertexI + 1] = T2fC4ubV3fVertex.Offscreen;
						this.VertexArray[cVertexI + 2] = T2fC4ubV3fVertex.Offscreen;
						this.VertexArray[cVertexI + 3] = T2fC4ubV3fVertex.Offscreen;
					}
					cCharI ++;
		        }
				//cBufferRow.IsValidated = true;
		    }
			this.NeedsVertexSync = false;
		}
	}

	//public struct Color4 //4bytes
	//{
	//    public byte R;
	//    public byte G;
	//    public byte B;
	//    public byte A;

	//    public Color4(byte iA, byte iR, byte iG, byte iB)
	//    {
	//        this.A = iA;
	//        this.R = iR;
	//        this.G = iG;
	//        this.B = iB;
	//    }
	//    //public static Color4 FromColor(Color iColor)
	//    //{
			
	//    //}
	//    //public static Color4 FromColor(HSCAColor iColor)
	//    //{

	//    //    return new Color4(iColor.A,iColor.R,iColor.G,iColor.B);
	//    //}

	//    public static implicit operator Color4(Color iColor)
	//    {
	//        return new Color4(iColor.A,iColor.R,iColor.G,iColor.B);
	//    }
	//    public static implicit operator Color(Color4 iColor)
	//    {
	//        return Color.FromArgb(iColor.A,iColor.R,iColor.G,iColor.B);
	//    }
	//    public static implicit operator Color4(CHSAColor iColor)
	//    {
	//        return (Color)iColor;
	//    }
	//    public static implicit operator CHSAColor(Color4 iColor)
	//    {
	//        return (Color)iColor;
	//    }

	//    //public static Color4 Transparent = new Color4();
	//}
	public struct T2fC4ubV3fVertex //8b + 4b + 12b = 24b per vertex = 96b per quad = (96 * 120)b per row, (11500 * 50)b per screen = 576000b
	{
		public static T2fC4ubV3fVertex Empty = new T2fC4ubV3fVertex();
		public static T2fC4ubV3fVertex Offscreen = new T2fC4ubV3fVertex(Vector2.Zero, new Color4(), new Vector3(Single.MinValue,Single.MinValue,0f));
		//public static T2fC4ubV3fVertex Offscreen = new T2fC4ubV3fVertex(Vector2.Zero, new Color4(), new Vector3(0,0,0));

		public Vector2 TexCoords;
		public Color4  Color;
		public Vector3 Position;

		public T2fC4ubV3fVertex(Vector2 iTexCoords, Color4 iColor, Vector3 iPosition)
		{
			this.TexCoords = iTexCoords;
			this.Color     = iColor;
			this.Position  = iPosition;
		}
	}
}