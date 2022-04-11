using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CellList = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;

namespace AE.Visualization
{
	public partial class TextBufferFrame
	{
		public enum CellGradientType
		{
			None
		}

		public class TextBufferDrawingInfo : DrawingInfo
		{
			public TextBufferRow[] Rows;

		}

		public struct CellStyle
		{
			//!!! Value = 0..1, where
			//    0.0 = ShadeColor
			//    0.5 = Color (max value)
			//    1.0 = GlareColor

			//public static CellStyle Default = new CellStyle(2,0);
			///public static CellStyle Default {get{return new CellStyle(CHSAColor.Glare, CHSAColor.Transparent);}}
			public static CellStyle Default    = new CellStyle(CHSAColor.Glare, CHSAColor.Transparent);
			public static CellStyle Whitespace = new CellStyle(CHSAColor.Glare.WithAlpha(0.25f), CHSAColor.Transparent);

			
			//public int ForeColorIndex{get{return this.ForeColorIndex_;}set{this.ForeColorIndex_ = value; this.UpdateBytes();}} private int ForeColorIndex_;
			//public int BackColorIndex{get{return this.BackColorIndex_;}set{this.BackColorIndex_ = value; this.UpdateBytes();}} private int BackColorIndex_;

			public Color4    ForeColorBytes;
			public Color4    BackColorBytes;

			public CHSAColor ForeColor;
			public CHSAColor BackColor;

			public FontStyle FontStyle;

			//public float Opacity;

			//public CellGradientType Gradient;
			//public bool IsBlinking;

			public CellStyle(int iForeColor, int iBackColor)
			{
				this = new CellStyle(ColorPalette.Default.Colors[iForeColor], ColorPalette.Default.Colors[iBackColor]);
			}
			public CellStyle(CHSAColor iForeColor, CHSAColor iBackColor) : this(iForeColor, iBackColor, FontStyle.Regular){}
			public CellStyle(CHSAColor iForeColor, CHSAColor iBackColor, FontStyle iFontStyle)
			{
				this.ForeColor = iForeColor;
				this.BackColor = iBackColor;
				this.FontStyle = iFontStyle;

				this.ForeColorBytes = new Color4();
				this.BackColorBytes = new Color4();



				this.UpdateBytes(true);
			}

			//public CellStyle(int iForeColor, int iBackColor) : this(iForeColor, iBackColor, 1f)
			//{
			//}
			//public CellStyle(int iForeColorIndex, int iBackColorIndex, float iOpacity)
			//{
			//    //GL.Color4(
			//    this.ForeColorIndex_ = iForeColorIndex;
			//    this.BackColorIndex_ = iBackColorIndex;
			//    this.ForeColorBytes  = new Color4();
			//    this.BackColorBytes  = new Color4();

			//    this.ForeColorBytes  = new Color4();
			//    this.BackColorBytes  = new Color4();
				
			//    this.Opacity    = iOpacity;



			//    this.UpdateBytes();
			//    //this.Gradient   = CellGradientType.None;
			//    //this.IsBlinking = false;
			//}
			//public CellStyle(int iForeColorIndex, int iBackColorIndex, float iOpacity)
			//{
			//    //GL.Color4(
			//    this.ForeColorIndex_ = iForeColorIndex;
			//    this.BackColorIndex_ = iBackColorIndex;
			//    this.ForeColorBytes  = new Color4();
			//    this.BackColorBytes  = new Color4();

			//    this.ForeColorBytes  = new Color4();
			//    this.BackColorBytes  = new Color4();
				
			//    this.Opacity    = iOpacity;



			//    this.UpdateBytes();
			//    //this.Gradient   = CellGradientType.None;
			//    //this.IsBlinking = false;
			//}
			
			public void UpdateBytes(bool iDoAdapt)
			{
				this.ForeColorBytes = iDoAdapt ? ColorPalette.Default.Adapt(this.ForeColor,true,true) : this.ForeColor;
				this.BackColorBytes = iDoAdapt ? ColorPalette.Default.Adapt(this.BackColor,true,true) : this.BackColor;
			}
		}
		
		public struct TextBufferOffset
		{
			public int X;
			public int Y;
			public bool IsEmpty {get {return this.X == -1 && this.Y == -1;}}
			
			public TextBufferOffset(int iX, int iY)
			{
				this.X = iX;
				this.Y = iY;
			}

			public override string ToString()
			{
				return this.X + ":" + this.Y;
			}
			//public static bool operator ==(TextBufferOffset ixOffset, TextBufferOffset iyOffset)
			//{
			//    return ixOffset.X == iyOffset.X && ixOffset.Y == iyOffset.Y;
			//}
			//public static bool operator !=(TextBufferOffset ixOffset, TextBufferOffset iyOffset)
			//{
			//    return !(ixOffset == iyOffset);
			//}
			public static explicit operator Point(TextBufferOffset iOffset)
			{
				return new Point(iOffset.X, iOffset.Y);
			}
			public static explicit operator Size(TextBufferOffset iOffset)
			{
				return new Size(iOffset.X, iOffset.Y);
			}

			public static TextBufferOffset Zero = new TextBufferOffset();

			public static TextBufferOffset Add(TextBufferOffset ixOffset, TextBufferOffset iyOffset)
			{
				return new TextBufferOffset(ixOffset.X + iyOffset.X, ixOffset.Y + iyOffset.Y);
			}
			public static TextBufferOffset Subtract(TextBufferOffset ixOffset, TextBufferOffset iyOffset)
			{
				return new TextBufferOffset(ixOffset.X - iyOffset.X, ixOffset.Y - iyOffset.Y);
			}

			public static bool operator == (TextBufferOffset iOffs1, TextBufferOffset iOffs2)
			{
				return iOffs1.X == iOffs2.X && iOffs1.Y == iOffs2.Y;
			}
			public static bool operator != (TextBufferOffset iOffs1, TextBufferOffset iOffs2)
			{
				return !(iOffs1 == iOffs2);
			}
			public static bool operator < (TextBufferOffset iOffs1, TextBufferOffset iOffs2)
			{
				return
				(
					(iOffs1.Y < iOffs2.Y)

					? true
					: 
					(
						(iOffs1.Y == iOffs2.Y)

						? iOffs1.X < iOffs2.X
						: false
					)
				);
			}
			public static bool operator > (TextBufferOffset iOffs1, TextBufferOffset iOffs2)
			{
				return iOffs1 != iOffs2 && !(iOffs1 < iOffs2);
			}
		}
		public struct TextBufferSize
		{
			public int Width;
			public int Height;

			public TextBufferSize(int iWidth, int iHeight)
			{
				this.Width  = iWidth;
				this.Height = iHeight;
			}
		}
		public struct TextBufferRegion
		{
			public int X;
			public int Y;
			public int Width;
			public int Height;

			public TextBufferRegion(int iX, int iY, int iWidth, int iHeight)
			{
				this.X      = iX;
				this.Y      = iY;
				this.Width  = iWidth;
				this.Height = iHeight;
			}
		}
		//public struct CellCoords
		//{
		//    public int Column;
		//    public int Row;
			
		//    public CellCoords(int iColumn, int iRow)
		//    {
		//        this.Column = iColumn;
		//        this.Row    = iRow;
		//    }

		//    public static explicit operator Point(CellCoords iInfo)
		//    {
		//        return new Point(iInfo.Column, iInfo.Row);
		//    }
		//    public static explicit operator Size(CellCoords iInfo)
		//    {
		//        return new Size(iInfo.Column, iInfo.Row);
		//    }

		//    public static CellCoords Zero = new CellCoords();

		//    public static CellCoords Add(CellCoords ixCoords, CellCoords iyCoords)
		//    {
		//        return new CellCoords(ixCoords.Column + iyCoords.Column, ixCoords.Row + iyCoords.Row);
		//    }
		//    public static CellCoords Subtract(CellCoords ixCoords, CellCoords iyCoords)
		//    {
		//        return new CellCoords(ixCoords.Column - iyCoords.Column, ixCoords.Row - iyCoords.Row);
		//    }
		//}
		public struct TextBufferCell
		{
			public static TextBufferCell Invalid = new TextBufferCell{IsValid = false};

			public char      Value;
			//public char      ParseValue;
			public CellStyle Style;
			public bool      IsValid;
			//public byte      ForeColor;
			//public byte      BackColor;
			

			public TextBufferCell(char iValue) : this(iValue, CellStyle.Default) {}
			public TextBufferCell(char iValue, CellStyle iStyle)
			{
				this.Value     = iValue;
				this.Style     = iStyle;
				this.IsValid   = true;
			}

			public override string ToString()
			{
				return this.Value.ToString();
			}

			public static TextBufferCell Transparent = new TextBufferCell(' ', new CellStyle());
			public static TextBufferCell[] ParseString(string iString, ref CellStyle iStyle, bool iDoProcessMarkup)
			{
				//var _Style = iStyle;
				var oCells = new List<TextBufferCell>(iString.Length);
				{
					string cTagValue = null;
					
					for(var cCi = 0; cCi < iString.Length; cCi++)
					{
						var cChar = iString[cCi];

						if(iDoProcessMarkup)
						{
							if(cChar == '<') cTagValue = "";

							else if(cTagValue != null)
							{
								if(cChar == '>')
								{
									///if(cTagValue != "2") {}
									iStyle.ForeColor = ColorPalette.Default.Adapt(ColorPalette.DefaultColors[Int32.Parse(cTagValue)], true, true);
									//iStyle.ForeColor = ColorPalette.Default.Colors[Int32.Parse(cTagValue)];
									iStyle.UpdateBytes(false);

									cTagValue = null;
									continue;

									
								}
								else cTagValue += cChar;
							}
							//_Style.ForeColor;
						}

						if(iDoProcessMarkup && cTagValue != null) continue;

						else oCells.Add(new TextBufferCell(iString[cCi], iStyle));
					}
				}
				return oCells.ToArray();
			}
			public static List<TextBufferCell> ParseString(string iString, CellStyle iStyle)
			{
				var oCells = new List<TextBufferCell>(iString.Length);
				{
					for(var cCi = 0; cCi < iString.Length; cCi++)
					{
						oCells.Add(new TextBufferCell(iString[cCi], iStyle));
					}
				}
				return oCells;
			}
			//public static TextBufferCell[] ParseString(string iString, CellStyle iStyle, bool iDoProcessMarkup)
			//{
			//    //var _SubstrMM = Regex.Matches(iString, @"<.*>[^<]*");
			//    //foreach(var cPart in _SubstrMM)
			//    //{
			//    //    var c = cPart;

			//    //}
				

			//    //CellStyle _Styles = null;
			//    //{
			//    //    if(iDoProcessMarkup)
			//    //    {
			//    //         new CellStyle[iString.Length];
			//    //    }
			//    //    for(var 
			//    //}
			//    //var _Substrings = new List
			//    var _Style = iStyle;
			//    var oCells = new TextBufferCell[iString.Length];
			//    {
			//        for(var cCi = 0; cCi < oCells.Length; cCi++)
			//        {
			//            if(iDoProcessMarkup)
			//            {
			//                if(
			//                _Style.ForeColor;
			//            }
			//            //var cStyle = iStyle; 
			//            oCells[cCi] = new TextBufferCell
			//            {
			//                Value = iString[cCi],
			//                Style = _Style,
			//            };
			//        }
			//    }
			//    return oCells;
			//}
			//public static void UpdateStyle(TextBufferCell[] iCells, CellStyle iStyle)
			//{
			//    for(var cCi = 0; cCi < iCells.Length; cCi++)
			//    {
			//        iCells[cCi].Style = iStyle;
			//    }
			//}
		}
		public class TextBufferRow
		{
			public TextBufferCell[] Cells;

			public int      VertexRowIndex = -1;
			public float    Opacity        = 1f;
			public bool     IsValidated    = false;

			//public TextBufferRow()
			//{
			//    this.Cells = new TextBufferCell[];
			//}
			//public TextBufferRow(int iCellCount) : this(iCellCount, -1)
			//{}
			//public TextBufferRow(int iCellCount, int iVertexRowIndex) : this(new CellList(iCellCount), iVertexRowIndex)
			//{}
			public TextBufferRow(int iCellCount, float iOpacity, int iVertexRowIndex)
			{
				this.Cells          = new TextBufferCell[iCellCount];
				this.Opacity        = iOpacity;
				this.VertexRowIndex = iVertexRowIndex;
			}
			
			public override string ToString()
			{
				return "VRi = " + this.VertexRowIndex;
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
		//struct T2fC4ubV3fVertex //8b + 4b + 12b = 24b per vertex = 96b per quad = (96 * 120)b per row, (11500 * 50)b per screen = 576000b
		//{
		//    public static T2fC4ubV3fVertex Empty = new T2fC4ubV3fVertex();
		//    public static T2fC4ubV3fVertex Offscreen = new T2fC4ubV3fVertex(Vector2.Zero, new Color4(), new Vector3(Single.MinValue,Single.MinValue,0f));
		//    //public static T2fC4ubV3fVertex Offscreen = new T2fC4ubV3fVertex(Vector2.Zero, new Color4(), new Vector3(0,0,0));

		//    public Vector2 TexCoords;
		//    public Color4  Color;
		//    public Vector3 Position;

		//    public T2fC4ubV3fVertex(Vector2 iTexCoords, Color4 iColor, Vector3 iPosition)
		//    {
		//        this.TexCoords = iTexCoords;
		//        this.Color     = iColor;
		//        this.Position  = iPosition;
		//    }
		//}
	}
}
