using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
//using OpenTK;
//using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;

using RowList  = System.Collections.Generic.List<AE.Visualization.BufferConsoleFrame.ConsoleRow>;
using CellList = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;

namespace AE.Visualization
{
	public partial class BufferConsoleFrame : TextBufferFrame
	{
		public class ConsoleRow
		{
			public CellList Cells;

			public ConsoleRow() : this(new CellList()){}
			public ConsoleRow(CellList iCells)
			{
				this.Cells          = iCells;
			}

			public override string ToString()
			{
				char[] _Chars = new char[this.Cells.Count];
				{
					for(var cCell = 0; cCell < _Chars.Length; cCell++)
					{
						_Chars[cCell] = this.Cells[cCell].Value;
					}
				}
				return new String(_Chars);
			}
		}

		public class InputData
		{
			public RowList       Rows             = new RowList(){new ConsoleRow()};
			public ConsoleRow    CurrentRow       {get{return this.Rows[this.Carriage.Y];}set{this.Rows[this.Carriage.Y] = value;}}
			public bool          IsUpdated        = true;
			public bool          IsEmpty          = true;

			public TextBufferOffset BufferOffset     = new TextBufferOffset();
			public TextBufferOffset Carriage         = new TextBufferOffset();
			public TextBufferOffset SelectionOrigin  = new TextBufferOffset(-1,-1);
			
			
			public InputData()
			{
				//this.Rows
			}
			
			public void CheckIsEmpty()
			{
				this.IsEmpty = this.Rows.Count == 1 && this.Rows[0].Cells.Count == 0;
			}
			//public string CurrentLine {}
			public void InsertCharacter (char iChar)
			{
				//if(this.CarriagePosition.X 
				//if(this.Lines.Count 
				///this.CurrentRowValue = this.CurrentRowValue.Insert(this.Carriage.Column, iChar.ToString());
				this.CurrentRow.Cells.Insert(this.Carriage.X, new TextBufferCell(iChar, CellStyle.Default));
				this.MoveCarriage(+1,0);
				this.IsUpdated = true;

				this.CheckIsEmpty();
			}
			public void DeleteCharacter ()
			{
/**
aaaaaaaaaaaaaaaa
bbbbbbbbbbbbbbbb
cccccccccccccccc
*/
				//return;
				if(this.Carriage.X >= this.CurrentRow.Cells.Count)
				{
				    if(this.Carriage.Y < this.Rows.Count - 1)
				    {
						//this.Lines.RemoveAt(this.CarriagePosition.Y);
						//this.CarriagePosition.Y
						//this.CarriagePosition.Y++;
				    }
				    else return;
				}

				///this.CurrentRowValue = this.CurrentRowValue.Remove(this.Carriage.Column, 1);
				this.CurrentRow.Cells.RemoveAt(this.Carriage.X);
				this.MoveCarriage(0,0);

				this.IsUpdated = true;
				this.CheckIsEmpty();
				//this.NeedsVertexSync = true;
			}
			public void MoveCarriage    (int iColOffs, int iRowOffs)
			{
				this.Carriage.X = Math.Min(Math.Max(this.Carriage.X + iColOffs, 0), this.CurrentRow.Cells.Count);
				//this.Carriage.Column = Math.Max(this.Carriage.Column + iColOffs, 0);

				///YY??
			}
			//public void NewLine()
			//{
			
			//}
			public string[] GetStrings()
			{
				var oStrA = new string[this.Rows.Count];
				{
					for(var cRow = 0; cRow < oStrA.Length; cRow++)
					{
						oStrA[cRow] = this.Rows[cRow].ToString();
					}
				}
				return oStrA;
			}
			public string GetSingleString()
			{
				return String.Join("\r\n", this.GetStrings());
			}
		}
	}
}
