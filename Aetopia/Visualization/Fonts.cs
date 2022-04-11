using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
//using System.Data;
////using System.Text;
//using System.Windows.Forms;
//using System.IO;
using OpenTK;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

//using System.Drawing;
//using OpenTK.Input;


namespace AE.Visualization
{
	public enum FontEditorMode
	{
		AddLines,
		RemoveLines,
	}
	public struct SymbolLine
	{
		public Vector2d Point1;
		public Vector2d Point2;
		public bool     IsSelected;

		public double Length {get{return (this.Point1 - this.Point2).Length;}}
		public bool IsEmpty  {get{return this.Point1 == Vector2d.Zero && this.Point2 == Vector2d.Zero;}}
		public bool IsFilled {get{return this.IsEmpty && this.Point2 != Vector2d.Zero;}}

		
		public SymbolLine(double iPoint1X, double iPoint1Y, double iPoint2X, double iPoint2Y)
		{
			this.Point1 = new Vector2d(iPoint1X, iPoint1Y);
			this.Point2 = new Vector2d(iPoint2X, iPoint2Y);

			this.IsSelected = false;
		}


		public static SymbolLine Empty = new SymbolLine();

		//public static bool operator ==(Line ixLine, Line iyLine)
		//{
		//    return ixLine.Point1 == iyLine.Point1 && ixLine.Point2 == iyLine.Point2;
		//}
	}
	public class SymbolCell
	{
		public int     AlphabetIndex;
		///public Vector2 Position;
		//public int OuterGrid;
		//public RectangleF Bounds;
		public Matrix4 Matrix;

		public List<SymbolLine> Lines;

		public SymbolCell(int iAlphabetIndex)
		{
			this.AlphabetIndex    = iAlphabetIndex;
			//this.Position = new Vector2(iIndex % iGrid, iIndex / iGrid);
			//this.Matrix   = Matrix4.CreateTranslation(this.Position.X - 0.0001f, this.Position.Y - 0.0001f, 0) * Matrix4.CreateScale(1,-1,1);

			this.Lines = new List<SymbolLine>();
		}
		
		//public SymbolCell(int iIndex, int iGrid)
		//{
		//    this.Index    = iIndex;
		//    this.Position = new Vector2(iIndex % iGrid, iIndex / iGrid);
		//    this.Matrix   = Matrix4.CreateTranslation(this.Position.X - 0.0001f, this.Position.Y - 0.0001f, 0) * Matrix4.CreateScale(1,-1,1);

		//    this.Lines = new List<SymbolLine>();
		//}

		public void UpdateMatrix(int iGridPosition, int iGridSize)
		{
			var _CellPosition = new Vector2(iGridPosition % iGridSize, iGridPosition / iGridSize);
				
			//this.Position = new Vector2(iGridPosition % iGridSize, iGridPosition / iGridSize);
			this.Matrix   = Matrix4.CreateTranslation(_CellPosition.X - 0.0001f, _CellPosition.Y - 0.0001f, 0) * Matrix4.CreateScale(1,-1,1);
		}
	}
	//public class 
	
	public class FontInfo
	{
		public static string FontsDirectory = "Fonts";

		public string       Name;
		public SymbolCell[] Cells;


		public FontInfo() : this("Default"){}
		public FontInfo(string iFontName)
		{
			this.LoadFile(iFontName);
		}

		//public void ResetC
		public void ResetCells()
		{
			this.Cells = new SymbolCell[UInt16.MaxValue];
			{
				for(var cCi = 0; cCi < this.Cells.Length; cCi++)
				{
					var cCell = new SymbolCell(cCi);

					this.Cells[cCi] = cCell;
				}
			}
		}
		public void LoadFile(string iFontName)
		{
			var _FilePath = System.IO.Path.Combine(FontsDirectory,  iFontName + ".font");
			if(!System.IO.File.Exists(_FilePath))
			{
				G.Console.Message("!Font '" + iFontName + "' not found");
				return; 
			}


			this.Name = iFontName;
			
			
			
			this.ResetCells();
			//this.CurrentLine = new SymbolLine();
			//this.
            

			var _FontNode = DataNode.Load(_FilePath);
			{
				foreach(var cCellNode in _FontNode)
				{
					int cCharIndex    = cCellNode["@index"];
				
					var cCell = new SymbolCell(cCharIndex);
					{
						var cLinesNode = cCellNode["Lines"];
						

						if(cLinesNode != null) foreach(var cLineStr in cLinesNode.Value.Split(' '))
						{
							if(cLineStr.Trim() == "") continue;

							var cLineStrPP = cLineStr.Split(',');
							var cLine      = new SymbolLine
							(
								Double.Parse(cLineStrPP[0]),
								Double.Parse(cLineStrPP[1]),
								Double.Parse(cLineStrPP[2]),
								Double.Parse(cLineStrPP[3])
							);
							cCell.Lines.Add(cLine);
						}


					}

					if(this.Cells[cCharIndex] == null || this.Cells[cCharIndex].Lines.Count == 0)
					{
						this.Cells[cCharIndex] = cCell;
					}
					else throw new WTFE();
				}
			}
		}
		public void SaveFile(string iFontName)
		{
			var _FilePath = System.IO.Path.Combine(FontsDirectory,  iFontName);


			var _FontNode = new DataNode("Font");
			{
				foreach(var cCell in this.Cells)
				{
					if(cCell.Lines.Count == 0) continue;

					var cCellNode = new DataNode("Cell");
					{
						cCellNode["@index"] = cCell.AlphabetIndex;
						//cCellNode["@bounds"] =
						//(
						//    cCell.Bounds.X      + "," +
						//    cCell.Bounds.Y      + "," +
						//    cCell.Bounds.Width  + "," +
						//    cCell.Bounds.Height
						//);

						var cLinesStr = ""; foreach(var cLine in cCell.Lines)
						{
							cLinesStr +=
							(
								cLine.Point1.X + "," +
								cLine.Point1.Y + "," +
								cLine.Point2.X + "," +
								cLine.Point2.Y + " "
							);
						}
						cCellNode.Create("Lines", cLinesStr);
					}
					_FontNode.Include(cCellNode);
				}
			}
			DataNode.Save(_FontNode, System.IO.Path.Combine(FontsDirectory, this.Name + ".font"));

			G.Console.Message("Saved font '" + this.Name + "'");

			SaveFileBin(iFontName);
		}
		public void SaveFileBin(string iFontName)
		{
			var _FilePath = System.IO.Path.Combine(FontsDirectory,  iFontName + ".bin");

			var _Stream = System.IO.File.OpenWrite(_FilePath);
			var _Writer = new System.IO.BinaryWriter(_Stream);
			{
				foreach(var cCell in this.Cells)
				{
					if(cCell.Lines.Count == 0) continue;

					_Writer.Write((ushort) cCell.AlphabetIndex);
					_Writer.Write((byte) cCell.Lines.Count);

					foreach(var cLine in cCell.Lines)
					{
						_Writer.Write((byte)Math.Round(cLine.Point1.X * 200));
						_Writer.Write((byte)Math.Round(cLine.Point1.Y * 200));
						_Writer.Write((byte)Math.Round(cLine.Point2.X * 200));
						_Writer.Write((byte)Math.Round(cLine.Point2.Y * 200));
					}
				}
				_Writer.Flush();
			}
			_Stream.Flush();
			_Stream.Close();

			G.Console.Message("Saved font '" + this.Name + "' (BINARY)");
		}
	}
}