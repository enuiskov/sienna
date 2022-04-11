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
	public partial class FontEditorFrame : ZoomableFrame
	{
		public new struct Routines
		{
			public static void ClearLineSelection(FontEditorFrame iFrame)
			{
				var _SelectedCell = iFrame.Font.Cells[iFrame.SelectedCell];

				for(var cLi = 0; cLi < _SelectedCell.Lines.Count; cLi++)
				{
					var cLine = _SelectedCell.Lines[cLi];
					cLine.IsSelected = false;
					_SelectedCell.Lines[cLi] = cLine;
				}
			}
			public static void SelectLine(FontEditorFrame iFrame)
			{
				/// && iFrame.SelectedCell != iFrame.CurrentCell.AlphabetIndex)
				
				///var _Pointer          = new Vector2d(iFrame.Pointer.X,-iFrame.Pointer.Y);
				var _CellOffset   = iFrame.CurrentCell.AlphabetIndex - iFrame.CharsetOffset;
				var _CellPosition = new Vector2(_CellOffset % iFrame.OuterGrid, _CellOffset / iFrame.OuterGrid);
				var _Pointer      = new Vector2d(iFrame.Pointer.X - _CellPosition.X, - iFrame.Pointer.Y - _CellPosition.Y);

				var _ClosestLineIndex = -1;
				var _ClosestDist      = Double.MaxValue;
				

				//foreach(var cLine in iFrame.CurrentCell.Lines)
				for(var cLi = 0; cLi < iFrame.CurrentCell.Lines.Count; cLi++)
				{
					var cLine = iFrame.CurrentCell.Lines[cLi];
					//{
					//    cLine.IsSelected = false;
					//    iFrame.CurrentCell.Lines[cLi] = cLine;
					//}

					var cLineDist = ModelViewer.Routines.Calculations.GetPointToLineDistance(_Pointer, cLine.Point1, cLine.Point2, true);

					if(cLineDist < _ClosestDist)
					{
						_ClosestDist      = cLineDist;
						_ClosestLineIndex = cLi;
					}

					
				}


				
				if(_ClosestLineIndex != -1 && _ClosestDist < 0.05)
				{
					var _ClosestLine = iFrame.CurrentCell.Lines[_ClosestLineIndex];
					_ClosestLine.IsSelected = true;
					iFrame.CurrentCell.Lines[_ClosestLineIndex] = _ClosestLine;


					
					iFrame.SelectedCell  = iFrame.CurrentCell.AlphabetIndex;
					iFrame.SelectedLines = new int[]{_ClosestLineIndex};
				}


			}
			public static void DeleteLines(FontEditorFrame iFrame)
			{
				if(iFrame.SelectedCell == -1) return;

				var _SelectedCell = iFrame.Font.Cells[iFrame.SelectedCell];

				for(var cLi = 0; cLi < _SelectedCell.Lines.Count; cLi++)
				{
					if(_SelectedCell.Lines[cLi].IsSelected)
					{
						_SelectedCell.Lines.RemoveAt(cLi--);
					}
				}

				//for(var cLi = 0; cLi < _SelectedCell.Lines.Count; cLi++)
				//{
				//    var cLine = _SelectedCell.Lines[cLi];
				//    cLine.IsSelected = false;
				//    _SelectedCell.Lines[cLi] = cLine;
				//}
			}

			public class Generation
			{
			}
			public class Rendering
			{
				public static void Draw               (FontEditorFrame iFrame)
				{
					if(iFrame.DoUseAntialiasing) GL.Enable  (EnableCap.LineSmooth);
					else                         GL.Disable (EnableCap.LineSmooth);



					if(iFrame.DoShowSampleText)
					{
						Routines.Rendering.DrawSampleText(iFrame);
					}
					else
					{
						ZoomableFrame.Routines.Rendering.SetProjectionMatrix(iFrame);

						ZoomableFrame.Routines.Rendering.DrawUnitSpace(iFrame);
						ZoomableFrame.Routines.Rendering.DrawPropeller(iFrame);

						
						GL.Scale(1,-1,1);

						///Routines.Rendering.DrawSampleText(iFrame);
						Routines.Rendering.DrawEditor(iFrame);
					}
				}
				public static void DrawSampleText     (FontEditorFrame iFrame)
				{
					GL.PushMatrix();
					GL.Scale(iFrame.SampleFontSize, iFrame.SampleFontSize * (iFrame.DoHRescaleSample ? 1.5 : 1.0), 1);
					//GL.Translate(0, 1, 0);

					GL.Color4(iFrame.Palette.GlareColor);

					if(iFrame.SampleTextDisplayList == -1)
					{
						///iFrame.SampleTextDisplayList = GL.GenLists(1);
						///GL.NewList(iFrame.SampleTextDisplayList, ListMode.CompileAndExecute);

						for(var cLi = 0; cLi < iFrame.SampleLines.Length; cLi++)
						{
							var cLine = iFrame.SampleLines[cLi];

							GL.PushMatrix();
							for(var cCi = 0; cCi < cLine.Length; cCi++)
							{
								var cChar      = cLine[cCi];
								var cCellIndex = (int)cChar;

								var cCell = iFrame.Font.Cells[cCellIndex];

								GLCanvasControl.Routines.Rendering.DrawCell(iFrame, cCell, false, iFrame.SampleLineWidth, 0);

								GL.Translate(0.8, 0, 0);
							}
							GL.PopMatrix();

							GL.Translate(0, 1.0, 0);
						}
						///GL.EndList();
					}
					else
					{
						GL.CallList(iFrame.SampleTextDisplayList);
					}
					GL.PopMatrix();
				}
				public static void DrawEditor          (FontEditorFrame iFrame)
				{
					DrawSourceAlphabet(iFrame);
					DrawGrid(iFrame);
					DrawCells(iFrame);

					DrawPointer(iFrame);
				}
				public static void DrawSourceAlphabet  (FontEditorFrame iFrame)
				{
					if(!iFrame.DoShowSourceAlphabet) return;

					//var _OuterGrid = iFrame.OuterGrid;

					GL.Color4(Color.FromArgb(64, iFrame.Palette.GlareColor));

					GL.BindTexture(TextureTarget.Texture2D, iFrame.CharAtlas.TexID);
					GL.Enable(EnableCap.Texture2D);
					GL.Begin(PrimitiveType.Quads);
					{
						GL.TexCoord2(0,0); GL.Vertex2(0, 0);
						GL.TexCoord2(1,0); GL.Vertex2(iFrame.OuterGrid, 0);
						GL.TexCoord2(1,1); GL.Vertex2(iFrame.OuterGrid, iFrame.OuterGrid);
						GL.TexCoord2(0,1); GL.Vertex2(0, iFrame.OuterGrid);
					}
					GL.End();
					GL.Disable(EnableCap.Texture2D);
				}
				public static void DrawGrid            (FontEditorFrame iFrame)
				{
					if(!iFrame.DoShowGrid) return;

					var _OuterGrid = iFrame.OuterGrid;
					var _InnerGrid = iFrame.InnerGrid;
					//var _OuterGridColor = Color.FromArgb(64, iFrame.Palette.Colors[7]);
					//var _InnerGridColor = iFrame.Palette.Colors[7];

					GL.Color4(iFrame.Palette.Colors[7]);

					GL.LineWidth(3);
					GL.Begin(PrimitiveType.LineLoop);
					{
						GL.Vertex2(         0,          0);
						GL.Vertex2(_OuterGrid,          0);
						GL.Vertex2(_OuterGrid, _OuterGrid);
						GL.Vertex2(         0, _OuterGrid);
					}
					GL.End();



					//GL.LineWidth(1);
					//GL.Begin(PrimitiveType.Lines);
					//{
					//    for(var cI = 0; cI <= iFrame.OuterGrid; cI ++)
					//    {
					//        GL.Vertex2(cI,0);
					//        GL.Vertex2(cI,_OuterGrid);

					//        GL.Vertex2(0,cI);
					//        GL.Vertex2(_OuterGrid,cI);
					//    }
					//}
					//GL.End();






					var _CharGridStep = 1f / _InnerGrid;
					//int _PtrCellX     = (int)Math.Floor(iFrame.Pointer.X), _PtrCellY = (int)Math.Floor(iFrame.Pointer.Y);
					var _PtrCell = new Vector2d(Math.Floor(iFrame.AbsPointer.X),Math.Floor(iFrame.AbsPointer.Y));
					{
						GL.Color4(Color.FromArgb(64, iFrame.Palette.Colors[9]));
						GL.LineWidth(1);
						GL.Begin(PrimitiveType.Lines);
						{
							
							for(float cI = _CharGridStep; cI < 1; cI += _CharGridStep)
							{
								GL.Vertex2(_PtrCell.X + cI,           0);
								GL.Vertex2(_PtrCell.X + cI, _OuterGrid);

								GL.Vertex2(0,          _PtrCell.Y + cI);
								GL.Vertex2(_OuterGrid, _PtrCell.Y + cI);
							}
						}
						GL.End();



						GL.PointSize(2f);
						GL.Color4(iFrame.Palette.Colors[9]);
						GL.Begin(PrimitiveType.Points);
						{
							for(float cXo = _CharGridStep; cXo < 1; cXo += _CharGridStep)
							for(float cYo = _CharGridStep; cYo < 1; cYo += _CharGridStep)
							{
								GL.Vertex2(_PtrCell.X + cXo, _PtrCell.Y + cYo);
							}
							//for(float cYo = _CharGrid; cYo <= 1 - _CharGrid; cYo += _CharGrid)
							//{
								
							//}
							
						}
						GL.End();
					}
				}
				public static void DrawCells           (FontEditorFrame iFrame)
				{
					var _CurrentCell = iFrame.CurrentCell;
					var _PointWidth  = 5.0f / (float)iFrame.Viewpoint.CurrentState.Position.Z;

					//GL.Color4(iFrame.Palette.Colors[2]);
					//GL.LineWidth(1);
					//GL.Begin(PrimitiveType.Lines);
					{
						GL.PushMatrix();

						//foreach(var cCell in iFrame.Cells)
						for(int _CellCount = iFrame.OuterGrid * iFrame.OuterGrid, cCi = 0; cCi < _CellCount; cCi++)
						
						{
							var cCell = iFrame.Font.Cells[iFrame.CharsetOffset + cCi];
							//GL.PushMatrix();
							//GL.Translate(cCell.Bounds.X, cCell.Bounds.Y, 0);
							GL.LoadMatrix(ref cCell.Matrix);
							GL.Color4(iFrame.Palette.Colors[9]);

							GLCanvasControl.Routines.Rendering.DrawCell(iFrame, cCell, cCell == _CurrentCell, cCell == _CurrentCell ? 3 : 1, _PointWidth);

							if(cCell == iFrame.CurrentCell)
							{
								DrawCurrentLine(iFrame);
							}


							//GL.PopMatrix();
						}
						GL.PopMatrix();
					}
					//GL.End();
				}
				//public static void DrawCell            (FontEditorFrame iFrame, SymbolCell iCell, bool iDoHighlight, float iLineWidth, float iPointWidth)
				//{
				//    //var _B = iCell.Bounds;

					
				//    GL.LineWidth(iLineWidth);

				//    if(iDoHighlight && iFrame.DoShowGrid)
				//    {
				//        GL.Color4(Color.FromArgb(64, iFrame.Palette.Colors[7]));
				//        ///GL.LineWidth(iDoHighlight ? 3 : 1);
				//        GL.Begin(PrimitiveType.LineLoop);
				//        {
				//            GL.Vertex2(0,0);
				//            GL.Vertex2(1,0);
				//            GL.Vertex2(1,1);
				//            GL.Vertex2(0,1);
				//        }
				//        GL.End();


				//        ///var _Pointer = new Vector2d(iFrame.Pointer.X - iCell.Position.X, - iFrame.Pointer.Y - iCell.Position.Y);/// Vector3.Transform((Vector3)iFrame.Pointer, iCell.Matrix).Xy;

				//        //GL.Color4(iFrame.Palette.GlareColor);
				//        //GL.PointSize(10);
				//        //GL.Begin(PrimitiveType.Points);
				//        //{
				//        //    GL.Vertex2(_Pointer);
				//        //}
				//        //GL.End();
				//    }



				//    if(iCell.Lines.Count != 0)
				//    {
				//        GL.Color4(iFrame.Palette.Colors[2]);
				//        if(iDoHighlight) GL.LineWidth(iLineWidth * 2);
						
				//        GL.Begin(PrimitiveType.Lines);
				//        {
				//            foreach(var cLine in iCell.Lines)
				//            {
				//                ///if(!iFrame.DoShowSampleText) GL.Color4(iFrame.Palette.Colors[cLine.IsSelected ? 3 : 9]);

				//                GL.Vertex2(cLine.Point1);
				//                GL.Vertex2(cLine.Point2);
				//            }
				//        }
				//        GL.End();

				//        if(iPointWidth >= 1f)
				//        {
				//            GL.PointSize(iDoHighlight ? iPointWidth * 2: iPointWidth);
				//            ///GL.Color4(iFrame.Palette.Colors[10]);
				//            GL.Begin(PrimitiveType.Points);
				//            {
				//                foreach(var cLine in iCell.Lines)
				//                {
				//                    GL.Vertex2(cLine.Point1);
				//                    GL.Vertex2(cLine.Point2);
				//                }
				//            }
				//            GL.End();
				//        }
				//    }
				//}
				public static void DrawPointer         (FontEditorFrame iFrame)
				{
					var _P = iFrame.AbsPointer;
					if(_P == Vector2d.Zero) return;

					GL.Color4(Color.FromArgb(255, iFrame.Palette.Colors[iFrame.IsEraseMode ? 3 : 6]));

					GL.LineWidth(1f);
					GL.Begin(PrimitiveType.Lines);
					{
						GL.Vertex2(_P.X, 0);
						GL.Vertex2(_P.X, iFrame.OuterGrid);

						GL.Vertex2(0,_P.Y);
						GL.Vertex2(iFrame.OuterGrid,_P.Y);
					}
					GL.End();
				

					GL.PointSize(10f);
					//GL.Color4(iFrame.Palette.Colors[4]);
					GL.Begin(PrimitiveType.Points);
					{
						GL.Vertex2(_P.X, _P.Y);
					}
					GL.End();
					
				
				}
				public static void DrawCurrentLine     (FontEditorFrame iFrame)
				{
					if(iFrame.CurrentLine.IsEmpty) return;
					//var _IsFilled = 
					
					GL.LineWidth(3f);
					GL.Color4(iFrame.Palette.Colors[5]);
					GL.Begin(PrimitiveType.Lines);
					{
						GL.Vertex2(iFrame.CurrentLine.Point1);
						GL.Vertex2(iFrame.CurrentLine.IsFilled ? iFrame.CurrentLine.Point2 : iFrame.RelPointer);
					}
					GL.End();

					GL.PointSize(5f);
					GL.Color4(iFrame.Palette.Colors[5]);
					GL.Begin(PrimitiveType.Points);
					{
						//GL.Vertex2(_P.X, _P.Y);
						if(iFrame.CurrentLine.Point1 != Vector2d.Zero) GL.Vertex2(iFrame.CurrentLine.Point1);
						if(iFrame.CurrentLine.Point2 != Vector2d.Zero) GL.Vertex2(iFrame.CurrentLine.Point2);
					}
					GL.End();
				}
			}
		}
	}
}