using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sienna.BlackTorus;
using SDColor = System.Drawing.Color;

namespace Sienna.SchemeObjectModel
{
	public class PixelMap : Node
	{
		//public static Color[] Colors;

		public Size            Size;
		public PixelMapCell[,] Cells;
		//public Bitmap          Bitmap;
		public Point           HoverCell;
		public string          ImagePath;
		public BitmapData      ImageData;

		public Image           SrcImage;

		//public byte            CurrentColor = 1;

		//public Pix
		static PixelMap()
		{
			//Colors = new Color[]
			//{
			//    Color.White,Color.Black,
			//    Color.Red,Color.Orange,Color.Yellow,Color.Green,Color.Cyan,Color.Blue,Color.Violet
			//};
		}
		public PixelMap() : this(0, 0)
		{

		}
		public PixelMap(int iWidth, int iHeight) : this(iWidth, iHeight, "Default")
		{

		}
		public PixelMap(int iWidth, int iHeight, string iImagePath)
		{
			this.ImagePath = @"L:\ZDisk.Data\Temp\Images\PixelMaps\" + iImagePath + ".bmp";
			this.SrcImage = Bitmap.FromFile(this.ImagePath);
			{
				var _FreeImage  = new Bitmap(this.SrcImage);//= this.Image.Clone();
				
				//this.Image
				this.SrcImage.Dispose();

				this.SrcImage = (Image)_FreeImage;
				//this.Image = (Image)new Bitmap(this.SourceImage);
			}
			this.FilterMap();

			//this.Image = Bitmap.FromFile(this.ImagePath);
			//{
			//    var _FreeImage  = new Bitmap(this.Image);//= this.Image.Clone();
				
			//    //this.Image
			//    this.Image.Dispose();

			//    this.SourceImage = (Image)_FreeImage;
			//    this.Image = (Image)new Bitmap(this.SourceImage);
			//}


			if(iWidth == 0 || iHeight == 0)
			{
				iWidth = this.Image.Width;
				iHeight = this.Image.Height;
			}

			this.Size  = new Size(iWidth, iHeight);
			this.Cells = new PixelMapCell[iWidth, iHeight];
			{
				Random _RNG = new Random();

				for(var cX = 0; cX < iWidth; cX++)
				{
					for(var cY = 0; cY < iHeight; cY++)
					{
						var cCell = new PixelMapCell();
						cCell.Color = 0;//(byte)_RNG.Next(9);

						this.Cells[cX,cY] = cCell;////.Color = Color.Green;
					}
				}
			}
			
			this.Color = 2;//SDColor.Red;

			this.MouseDown += new MouseEventHandler(PixelMap_MouseDown);
			this.MouseMove += new MouseEventHandler(PixelMap_MouseMove);

			//this.ProcessMap();
		}

		

		void PixelMap_MouseDown(BlackTorus.MouseEventArgs iEvent)
		{
			this.PixelMap_MouseMove(iEvent);
			//this.Cells[this.HoverCell.X, this.HoverCell.Y].Color = 1;//Color.Red;
			//Console.WriteLine(iEvent.Button);
			//throw new NotImplementedException();
		}
		void PixelMap_MouseMove(BlackTorus.MouseEventArgs iEvent)
		{
			//if(iEvent.Button
			if(iEvent.Button == MouseButtons.Left)
			{
				this.UpdateMap();
			}
		}
		//void PixelMap_MouseDown(MouseEventArgs iEvent)
		//{
		//    this.Cells[this.HoverCell.X, this.HoverCell.Y].Color = 1;//Color.Red;
		//    //Console.WriteLine(iEvent.Button);
		//    //throw new NotImplementedException();
		//}


		public override void UpdateProjections()
		{
			base.UpdateProjections();

			//var _Size = 1.0;
			var _OffCenX = this.Size.Width  / 2;
			var _OffCenY = this.Size.Height / 2;

			var _MouX = (int)(Math.Floor(this.Pointer.X * _OffCenX)) + _OffCenX;
			var _MouY = (int)(Math.Floor(this.Pointer.Y * _OffCenY)) + _OffCenY;
			//var _MouX = (int)(Math.Floor(this.MousePosition.X * _Size) / _Size);
			//var _MouY = (int)(Math.Floor(this.MousePosition.Y * _Size) / _Size);
			
			//this.HoverCell = new Point(_X,_Y);
			this.HoverCell = new Point(_MouX,_MouY);
		}
		public override void Draw()
		{

			//base.Draw();

			if(this.Image != null)
			{
				//SchemeViewport.Routines.Rendering.DrawImage(this.Scheme.Viewport, (Bitmap)this.Image, Color.White, -1.0,-1.0,0,2.0);
				SchemeFrame.Routines.Rendering.DrawImage(this.Scheme.Frame, (Bitmap)this.Image, SDColor.White, -1.0,-1.0,0,2.0);
			}


			//var _Step = 1.0;
			var _Size = 1.0;//Math.Min(Math.Max(this.Scheme.Viewport.Viewpoint.Z / 100.0, 0.9), 1.0);// 0.9 / 2.0;
			//var _Cells = this.Cells;
			var _Width   = this.Size.Width;
			var _Height  = this.Size.Height;
			var _XOffset = _Width  / 2.0;
			var _YOffset = _Height / 2.0;
			var _XStep   = 1.0 / _Width;
			var _YStep   = 1.0 / _Height;
			var _MaxSize = Math.Max(_Width, _Height);


			GL.PushMatrix();
			GL.Translate(-1.0,-1.0,0.0);
			GL.Scale(2.0 / _MaxSize, 2.0 / _MaxSize, 1.0);

			GL.PushMatrix();
			GL.Translate((1.0 - _Size) / 2.0, (1.0 - _Size) / 2.0,0.0);
			

			this.DrawCursor();
			GL.PopMatrix();
		}
		
		private         void DrawCursor()
		{
			var _X = (double)this.HoverCell.X;// - 0.05;// / this.Size.Width;
			var _Y = (double)this.HoverCell.Y;// - 0.05;// / this.Size.Height;
			var _W = 1.0;
			var _H = 1.0;
			//var _W

			if(_X < 0 || _X >= this.Size.Width || _Y < 0 || _Y >= this.Size.Height)
			{
				return;
			}

			//GL.LineWidth((float)_W * 10);
			GL.LineWidth(1);
			GL.Begin(PrimitiveType.Lines);
			{
				GL.Color4(SDColor.Black);

				GL.Vertex2(_X + (_W * .5), 0);
				GL.Vertex2(_X + (_W * .5), _Y - (_H * 1));
				GL.Vertex2(_X + (_W * .5), _Y + (_H * 2));
				GL.Vertex2(_X + (_W * .5), this.Size.Height);

				GL.Vertex2(0,             _Y + (_H * .5));
				GL.Vertex2(_X - (_W * 1), _Y + (_H * .5));
				GL.Vertex2(_X + (_W * 2),   _Y + (_H * .5));
				GL.Vertex2(this.Size.Width, _Y + (_H * .5));
			}
			GL.End();

			GL.LineWidth(2f);
			GL.Begin(PrimitiveType.LineLoop);
			{
				GL.Color4(SDColor.Black);

				GL.Vertex2(_X,      _Y     );
				GL.Vertex2(_X,      _Y + _H);
				GL.Vertex2(_X + _W, _Y + _H);
				GL.Vertex2(_X + _W, _Y     );
			}
			GL.End();

			GL.Begin(PrimitiveType.Quads);
			{
				GL.Color4(this.Palette.Colors[this.Scheme.Frame.Screen.DrawingColor]);

				GL.Vertex2(_X + (_W * .25), _Y + (_H * .25));
				GL.Vertex2(_X + (_W * .25), _Y + (_H * .75));
				GL.Vertex2(_X + (_W * .75), _Y + (_H * .75));
				GL.Vertex2(_X + (_W * .75), _Y + (_H * .25));
				
			}
			GL.End();

			
		}
		
		private void UpdateMap()
		{
			


			var _Cell = this.HoverCell;
			//var _W = this.Cells.GetLength(0);
			//var _H = this.Cells.GetLength(1);

			if(_Cell.X < 0 || _Cell.X >= this.Size.Width)  return;
			if(_Cell.Y < 0 || _Cell.Y >= this.Size.Height) return;

			//this.Cells[_Cell.X, _Cell.Y].Color = (byte)MathEx.Clamp(this.Scheme.Viewport.DrawingColor, 0, SchemeObject.Colors.Length - 1);//Color.Red;
			
			//var _Grx = Graphics.FromImage(this.Image);
			//(Image)(this.Image).
			var _Bmp = this.SrcImage as Bitmap;
			_Bmp.SetPixel(_Cell.X, _Cell.Y, this.Scheme.Frame.Palette.Colors[this.Scheme.Frame.Screen.DrawingColor]);//(byte)MathEx.Clamp(this.Scheme.Viewport.DrawingColor, 0, SchemeObject.Colors.Length - 1));//Color.Red;

			this.ResetMap();
		}
		public void FilterMap()
		{
			

			var _Bmp = new Bitmap(this.SrcImage);
			{
				if(this.Scheme != null)
				{
					var _PenC = this.Scheme.Frame.Palette.Colors[this.Scheme.Frame.Screen.DrawingColor];

					for(var cY = 0; cY < _Bmp.Height; cY++)
					{
						for(var cX = 0; cX < _Bmp.Width; cX++)
						{
							var cColor = _Bmp.GetPixel(cX, cY);

							if(cColor.R != _PenC.R || cColor.G != _PenC.G || cColor.B != _PenC.B)
							{
								_Bmp.SetPixel(cX, cY, SDColor.Transparent);
							}
						}
					}
				}
			}
			this.Image = _Bmp;

			

			
			//this.Ima
			
			//.Create
		}
		public void ResetMap()
		{
			//this.Image = new Bitmap(this.SrcImage);
			this.Image = this.SrcImage;
		}

		public static PixelMap GenerateDefaultMap()
		{
			var oMap = new PixelMap();
			{
				//oMap[
				//oMap.Matrix *= Matrix4d.RotateZ(0.2);
				//oMap.Matrix *= Matrix4d.CreateTranslation(2.0,1.0,0.0);
				//var _InnerMap = new PixelMap();
				//{
				//    //_InnerMap.Matrix = Matrix4d.
				//}
				//oMap.Children.Add(_InnerMap);
			}
			return oMap;
		}
	}
	public enum PixelMapCellType
	{
		None,
		Object,
		Port,
		Link,
	}
	public class PixelMapCell
	{
		//public PixelMapCellType Type;
		public byte Color = 0;
	}


	public struct Routines
	{
		public static void DrawHoverCell(PixelMap iMap)
		{
			//iMap.Size

			GL.Begin(PrimitiveType.LineLoop);
			{
				
			}
			GL.End();
		}
	}
}
