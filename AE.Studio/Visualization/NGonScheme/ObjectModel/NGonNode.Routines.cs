using System;
using System.Collections.Generic;
using System.Text;
using SD = System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using AE.Visualization;


namespace AE.Visualization.SchemeObjectModel
{
	public partial class NGonNode : SchemeNode//, IProcessable
	{
		public new struct Routines
		{
			public static NGonNode DeepClone(NGonNode iSrcNode)
			{
				var _Node = new DataNode();

				iSrcNode.WriteNode(_Node);
				var oDstNode = new NGonNode();
				oDstNode.ReadNode(_Node);


				//var oDstNode = new NGonNode
				//{
				//    Sides = iSrcNode.Sides,
				//    Color = iSrcNode.Color,

				//    //Matrix = iSrcNode.Matrix,
				//    //Position = iSrcNode.Position,
				//    //Rotation = iSrcNode.Rotation,
				//    //Scale = iSrcNode.Scale,

				//    EdgeTypes = (EdgeType[])iSrcNode.EdgeTypes.Clone(),
					
				//    Name = iSrcNode.Name,
					
				//    /*
				//        Children = iSrcNode.Children.Clone(),
				//        Ports = iSrcNode.Ports.Clone(),
				//        Joints = iSrcNode.Joints.Clone(),
				//        Lines = iSrcNode.Lines.Clone(),
				//    */
				//};
				//oDstNode.UpdateSides(oDstNode.Sides);
				
				//oDstNode.UpdateGlobalMatrix(Matrix4d.Identity);
				//oDstNode.UpdateContactPoints(false);
				//oDstNode.UpdateProjections();
				//oDstNode.
				return oDstNode;
			}


			public class Projections
			{
				public static void    UpdateProjections   (NGonNode iNode, bool iDoReset)
				{
					iNode.Frame.Profiler.TotalNodeProjectionUpdateAttempts++;
					

					if(iDoReset)
					{
						//this.Profiler.TotalNodeProjectionResets++;

						//iNode.Viewpoint.Reset();
						//foreach(var cChildNode in iNode.Children) cChildNode.UpdateProjections(true);
					
						//return;
					}

					iNode.Frame.Profiler.TotalNodeProjectionUpdates++;
					
					var _IsChild = iNode.Parent != null;

					if(_IsChild)
					{
						//var _tParentViewZ = iNode.Parent.Viewpoint.Position.Z;

						//var _PareV = iNode.Parent.Viewpoint.Position;
						//var _ModPareV = new Vector3d(_PareV.X,_PareV.Y,0.0);
						//var _PareZ = _PareV.Z;
							
						iNode.Viewpoint.Position     = Vector3d.Transform(iNode.Parent.Viewpoint.Position, iNode.MatrixInverted);
						//iNode.Viewpoint.Position.Z   /= iNode.Scale;// / iNode.Parent.Viewpoint.Position.Z;
						iNode.Viewpoint.Inclination  = iNode.Parent.Viewpoint.Inclination - iNode.Rotation;

						iNode.Pointer               = Vector3d.Transform(iNode.Parent.Pointer, iNode.MatrixInverted);
					}

					iNode.IsPointerOver = CheckPointerIsOver(iNode);
					{
						if(iNode.IsPointerOver)
						{
							iNode.Frame.HoverNode = iNode;

							if(_IsChild)
							{
								iNode.Parent.IsPointerOver = false;
							}
						}
					}

					//if(iNode.Viewpoint.Position.Z < 1)
					foreach(var cChildNode in iNode.Children)
					{
						///cChildNode.UpdateProjections(iDoReset || iNode.Viewpoint.Position.Z > iNode.OuterRadius * 20);
						cChildNode.UpdateProjections(iDoReset || iNode.Pointer.Xy.Length > iNode.OuterRadius);
					}


					iNode.HoverSegment = -1; if(iNode.IsPointerOver)
					{
						var _PointerAngle =  Math.Atan2(iNode.Pointer.Y, iNode.Pointer.X);
						var _CorrAngle    = _PointerAngle + (_PointerAngle < 0 ? Math.PI * 2.0 : 0);
					
						var _SegmAbsPos    = _CorrAngle / (iNode.HalfSideAngle * 2.0);
						
						var _SegmIndex     = Math.Round(_SegmAbsPos) % iNode.Sides;

						iNode.HoverSegment = (int)_SegmIndex;
					}

					
					//if(false)
					{

						foreach(var cJoint in iNode.Joints) cJoint.IsPointerOver = false;
						var _HvrJoints = GetNearestJoints(iNode, iNode.Pointer, iNode.Viewpoint.Position.Z * 0.01);
						
						if(_HvrJoints != null)
						{
							foreach(var cJoint in _HvrJoints)
							{
								if(cJoint.Port == null) cJoint.IsPointerOver = true;
							}
						}
					}
					//if(false)
					{

						foreach(var cPort in iNode.Ports) cPort.IsPointerOver = false;

						var _HvrPorts = GetNearestPorts(iNode, iNode.Pointer, iNode.Viewpoint.Position.Z * 0.02);
						
						if(_HvrPorts != null)
						{
							foreach(var cPort in _HvrPorts)
							{
								cPort.IsPointerOver = true;
							}
						}
					}
				}

				public static double  GetSideDistance     (NGonNode iNode, Vector3d iVec)
				{
					//return this.GetSideDistance(Math.Atan2(-iVec.X, iVec.Y));
					return GetSideDistance(iNode, Math.Atan2(iVec.Y, iVec.X));
				}
				public static double  GetSideDistance     (NGonNode iNode, double iAngle)
				{
					//iAngle = iAngle + (Math.PI / 2.0);

					var _CorrAngle     = iAngle + (iAngle < 0 ? Math.PI * 2.0 : 0);/// + (Math.PI / 2.0);
					
					var _SegmAbsPos    = _CorrAngle / (iNode.HalfSideAngle * 2.0);
					var _SegmRelPos    = Math.Abs(_SegmAbsPos - Math.Round(_SegmAbsPos));
					var _SegmPosNorm   = _SegmRelPos * 2.0;
					
					var _ProjSegmAngle = iNode.HalfSideAngle * _SegmPosNorm;
					var _ProjDist      = iNode.InnerRadius / Math.Cos(_ProjSegmAngle);
					
					return _ProjDist;
				}
				//public static double  GetSideDistance     (NGonNode iNode, double iAngle)
				//{
				//    var _CorrAngle     = iAngle + (iAngle < 0 ? Math.PI * 2.0 : 0);
					
				//    var _SegmAbsPos    = _CorrAngle / (iNode.HalfSideAngle * 2.0);
				//    var _SegmRelPos    = Math.Abs(_SegmAbsPos - Math.Round(_SegmAbsPos));
				//    var _SegmPosNorm   = _SegmRelPos * 2.0;
					
				//    var _ProjSegmAngle = iNode.HalfSideAngle * _SegmPosNorm;
				//    var _ProjDist      = iNode.InnerRadius / Math.Cos(_ProjSegmAngle);
					
				//    return _ProjDist;
				//}
				
				public static List<NGonJoint>    GetNearestJoints   (NGonNode iNode, Vector3d iPosition, double iMaxDistance)
				{
					if(iNode.Joints == null || iNode.Joints.Count == 0) return null;


					var oJoints = new List<NGonJoint>();
					{
						double _MaxDist = iMaxDistance, _MinDist = _MaxDist;

						foreach(var cJoint in iNode.Joints)
						{
							var cErrV = iPosition - cJoint.Position;
							var cDist = cErrV.Length;

							if(cDist < _MaxDist)/// && cDist < _MinDist)
							{
								//_MinErrV = cCenErrV;
								oJoints.Add(cJoint);
								_MinDist = cDist;
							}
						}
					}
					return oJoints;
				}
				
				public static NGonLine           GetNearestLine     (NGonNode iNode, Vector3d iPosition, double iDistance)
				{
					throw new NotImplementedException();
				}
				public static List<NGonEdgePort> GetNearestPorts     (NGonNode iNode, Vector3d iPosition, double iMaxDistance)
				{
					if(iNode.Ports == null || iNode.Ports.Count == 0) return null;

					var oPorts = new List<NGonEdgePort>();
					{
						double _MaxDist = iMaxDistance, _MinDist = _MaxDist;

						foreach(var cPort in iNode.Ports)
						{
							var cLineDist     = ModelViewer.Routines.Calculations.GetPointToLineDistance(iPosition, cPort.InnerPinPoint, cPort.InnerEdgePoint, true);

							if(cLineDist < _MaxDist)
							{
								oPorts.Add(cPort);
								_MinDist = cLineDist;
							}

							//var cBaseDist     = (iPosition - cPort.InnerEdgePoint).Length;
							//var cPinDist      = (iPosition - cPort.InnerPinPoint).Length;
							//var cLineDist     = ModelViewer.Routines.Calculations.GetPointToLineDistance(iPosition, cPort.InnerPinPoint, cPort.InnerEdgePoint, false);
							//var cMinPointDist = Math.Max(cBaseDist, cPinDist);

							//if(cLineDist < _MaxDist && cMinPointDist < NGonEdgePort.InnerPinLength)
							//{
							//    oPorts.Add(cPort);
							//    _MinDist = cLineDist;
							//}
						}
					}
					return oPorts;
				}
				public static bool               CheckPointerIsOver (NGonNode iNode)
				{
					var _CentDist = iNode.Pointer.Length;

					return _CentDist < 10 && _CentDist < GetSideDistance(iNode, iNode.Pointer);
				}
			}
			public class Rendering
			{
				public static void Draw               (NGonNode iNode)
				{
					var _IsPerspMode = iNode.Frame.IsPerspectiveMode;

					iNode.Frame.Profiler.TotalAttemptsToRenderNode++;

					var _ViewP       = iNode.Viewpoint.Position;
					var _ViewDist    = _ViewP.Xy.Length;
					var _FieldOfView = _ViewP.Z * Viewpoint2D.FieldOfView;
					
					//if(_ViewZoom != 0)
					//{
						
						if(_FieldOfView > 100 * 2.0) return;
						///if(_ViewDist > (_FieldOfView + iNode.OuterRadius) * 2.0) return;
						if(_ViewDist > (_FieldOfView + iNode.OuterRadius)) return;

					//var _FillOpacity   = MathEx.Clamp((1.0 * (iNode.Viewpoint.Position.Z * 5.0) / iNode.OuterRadius) - 0.0, 0.0,0.8);
					var _FillOpacity = (iNode.Palette.Colors[iNode.Color].A / 255f) * MathEx.Clamp((_FieldOfView * 0.1 / iNode.OuterRadius) - 0.0, 0.0,0.8);

					var _LineWidth = iNode.EdgeWidth * (iNode.IsSelected ? 5f : iNode.IsPointerOver ? 3f : 1f);

					var _FillColor = iNode.Palette.Colors[iNode.Color];
					{
						if(_FieldOfView == 0)
						{
							//fitting object
							_FillOpacity = 0.0;
						}
					}
					
					if(!_IsPerspMode)
					{
						if(_ViewP.Z > 0.1 || _ViewP.Z == 0)
							///for(var cI = 0; cI < 10; cI++)
								Routines.Rendering.DrawShape(iNode, (float)_FillOpacity, _LineWidth, _FillColor, iNode.Frame.IsGradientFillMode, _IsPerspMode);
						
						

						iNode.Frame.Profiler.TotalRenderedNodes++;
					}

					if(_ViewP.Z > 0.01 && _ViewP.Z < 10 && (iNode.IsSelected || iNode.Visuals.IsVisible))
					{
						GL.PushMatrix();
						GL.Rotate(iNode.Viewpoint.Inclination * MathEx.RTD, Vector3d.UnitZ);
						iNode.Visuals.Draw();
						GL.PopMatrix();
					}
					///NGonSchemeFrame.Routines.Rendering.DrawFlowArrows(this);
					//NGonSchemeFrame.Routines.Rendering.DrawHoverSegment(this);
					///if(this.IsPointerOver) SchemeNode.Routines.DrawPointsOnShape(this, this.Shape, Math.Atan2(this.Pointer.Y, this.Pointer.X));

					//NGonSchemeFrame.Routines.Rendering.DrawNeighbours(this);
					///if(this.IsPointerOver) Routines.Rendering.DrawEdgePortGrid(this);

					foreach(var cHelper in iNode.Helpers)
					{
						cHelper.Draw();
					}

					//if(iNode.IsPointerOver)
					//{

					//    if(Workarounds.NGonSchemeFrame.ProtractorHelpers != null)
					//    {
					//        foreach(var cHelper in Workarounds.NGonSchemeFrame.ProtractorHelpers) if(cHelper.Parent == iNode)  cHelper.Draw();
					//    }
					//    if(Workarounds.NGonSchemeFrame.RulerHelpers != null)
					//    {
					//        foreach(var cHelper in Workarounds.NGonSchemeFrame.RulerHelpers)      if(cHelper.Parent == iNode) cHelper.Draw();
					//    }
					//    if(Workarounds.NGonSchemeFrame.ArcHelpers != null)
					//    {
					//        foreach(var cHelper in Workarounds.NGonSchemeFrame.ArcHelpers)        if(cHelper.Parent == iNode) cHelper.Draw();
					//    }
						
						
					//    ///DrawPointer(iNode, _FieldOfView * 0.02);
					//}
					if(iNode.IsSelected)
					{
						///SchemeNode.Routines.DrawPointer(iNode);
						//DrawPointer(iNode, _FieldOfView * 0.02);
						//Routines.Rendering.DrawPointsOnShape(iNode, 
					}
					//Routines.Rendering..DrawEdgePortGrid(this);
					//thos
					

					if(_FieldOfView < 30.0)
					{
						Routines.Rendering.DrawPorts(iNode);
						Routines.Rendering.DrawLines(iNode);

						if(iNode.IsPointerOver)/// && Workarounds.NGonSchemeFrame.IsCreationMode)
						{
							Routines.Rendering.DrawJoints(iNode);
						}
					}
					


					//NGonSchemeFrame.Routines.Rendering.DrawContactPoints(this);

					//this.Chid
					
					foreach(var cNode in iNode.Children)
					{
						//if(cNode.Viewpoint.
						//if(iDoRese
						if(cNode.IsPointerOver)
						{
							GL.PushMatrix();
							GL.MultMatrix(ref iNode.MatrixInverted);
							iNode.Frame.Profiler.TotalMatrixTransforms++;

							NGonSchemeFrame.Routines.Rendering.DrawContactPoints(iNode);
							GL.PopMatrix();
						}
						GL.PushMatrix();
						{
							cNode.Transform ();
							cNode.Draw      ();
						}
						GL.PopMatrix();

						
						///NGonSchemeFrame.Routines.Rendering.DrawContactPoints(cNode as NGonNode);
						///NGonSchemeFrame.Routines.Rendering.DrawEdgeLinkPoints(cNode as NGonNode);
					}

					if(_IsPerspMode)
					{
					    Routines.Rendering.DrawShape(iNode, (float)_FillOpacity, _LineWidth, _FillColor, iNode.Frame.IsGradientFillMode, _IsPerspMode);
					    iNode.Frame.Profiler.TotalRenderedNodes++;
					}


					//SchemeNode.Routines.DrawPointer(iNode);
					///SchemeNode.Routines.DrawViewpoint(iNode);


					///var _ParentMatrix = this.Parent != null ? this.Parent.MatrixInv : Matrix4d.Identity;
					
					
					
					
					


					{
						if(iNode.IsPointerOver)
						{
					
							///DrawPointsOnShape(iNode, iNode.Shape, Math.Atan2(iNode.Pointer.Y,iNode.Pointer.X));
							
							
							///~~ drawing fitting object with parent matrix while NOT in creation mode (dragging child node);
							var _Frame = iNode.Frame;
							var _IsCreationMode = _Frame.IsCreationMode;


							if(!_IsCreationMode)
							{
							    GL.PushMatrix();
							    GL.MultMatrix(ref iNode.MatrixInverted);
							    iNode.Frame.Profiler.TotalMatrixTransforms++;
							}

							

							if(_IsCreationMode)
							{
								NGonSchemeFrame.Routines.Rendering.DrawNewObjects(_Frame, iNode);
							}
							//if(this.Parent != null) NGonSchemeFrame.Routines.Rendering.DrawContactPoints(this.Parent);
							//NGonSchemeFrame.Routines.Rendering.DrawExtraObjects (_Frame);
							NGonSchemeFrame.Routines.Rendering.DrawExtraObject  (_Frame, _Frame.FittingObject);
							
							if(!_IsCreationMode)
							{
							    GL.PopMatrix();
							}
						}
					}
					
					
				}

				public static void DrawShape          (NGonNode iNode, float iOpacity, float iLineWidth, SD.Color iColor, bool iIsGradMode, bool iIsPerspMode)
				{
					//var _PremulAlphaF = iColor.A / 255f;
					//iColor = SD.Color.FromArgb(iColor.A, (int)(iColor.R * _PremulAlphaF),(int)(iColor.G * _PremulAlphaF),(int)(iColor.B * _PremulAlphaF));

					//var _Color    = iNode.Palette.Colors[iColorIndex];
					var _Vertices = iNode.Shape.Vertices;

					//var _IsActive    = iObj == iObj.Scheme.Viewport.Get;
					//var _IsMouseOver = iObj.IsPointerOver;


					//if(!_IsActive && !_IsMouseOver)  return;

					///if(!iIsPerspMode)
					{
						GL.Begin(PrimitiveType.Polygon);
						{
							if(iIsGradMode)
							{
								//double _GradAngle;
								//{
								//    if(Workarounds.NGonSchemeFrame.IsPerspectiveMode)
								//    {
										
								//    }
									
								//}
								//var _GlobalView = Workarounds.NGonSchemeFrame.Viewpoint;

								
								
								///var _GradAngle = iNode.Viewpoint.Inclination + (iNode.Frame.IsPerspectiveMode ?  iNode.Frame.Viewpoint.CurrentState.PerspEyeAzimuth + (Math.PI / 2) : 0);
								var _GradAngle =
								(
									iNode.Frame.IsPerspectiveMode
									///iNode.Frame.Viewpoint.CurrentState.PerspEyeAzimuth
									///? (iNode.Viewpoint.Inclination + (Math.PI / 2.0)) + (iNode.Frame.Viewpoint.CurrentState.PerspEyeAzimuth)
									///? (iNode.Viewpoint.Inclination + (Math.PI / 2.0)) + (iNode.Frame.Viewpoint.CurrentState.PerspEyeAzimuth)
									? (iNode.Viewpoint.Inclination + (Math.PI / 2.0)) + (iNode.Frame.Viewpoint.CurrentState.PerspEyeAzimuth - iNode.Frame.Viewpoint.CurrentState.Inclination)
									: iNode.Viewpoint.Inclination
								);
								//this.Rotation - iViewpoint.Inclination

								//iOpacity = 1;
								//GL.Color4(SDColor.Transparent);

								//ColorPalette.PremultiplyAlpha(iColor, iOpacity);

								///GL.Color4(ColorPalette.PremultiplyAlpha(iColor, iOpacity * 0.5));

								GL.Color4(SD.Color.FromArgb((int)(iOpacity * 127), iColor));
								GL.Vertex3(0,0,0);
								
								for(var cI = -1; cI < _Vertices.Length; cI++)
								{
									var cVi = cI == -1 ? _Vertices.Length - 1 : cI;
									var cVertex = _Vertices[cVi];
									var cAngle = Math.Atan2(cVertex.Y,cVertex.X) - _GradAngle;//((4.0 * Math.PI) + cAbsA);// - iNode.Viewpoint.Inclination;
									var cOpacity = -Math.Sin(cAngle) / 2.0 + 0.5;

									//var _PremulAlphaF = (iOpacity * cOpacity);
									//var cColor = SD.Color.FromArgb((int)(_PremulAlphaF * 255), (int)(iColor.R * _PremulAlphaF),(int)(iColor.G * _PremulAlphaF),(int)(iColor.B * _PremulAlphaF));

									

									//var cColor = 

 									GL.Color4(SD.Color.FromArgb((int)(iOpacity * cOpacity * 255), iColor));
									///GL.Color4(ColorPalette.PremultiplyAlpha(iColor, iOpacity * cOpacity));
									GL.Vertex3(cVertex);
								}
							}
							else
							{
								//Vector3
								
								//Color4
								
								//if(false)
								//{
								//    var _BackColor_Bytes = iNode.Frame.Palette.ShadeColor;
								//    var _BackColor  = new Vector3(_BackColor_Bytes.R / 255f,_BackColor_Bytes.G / 255f, _BackColor_Bytes.B / 255f);;
								//    var _ShapeColor = new Vector3(iColor.R / 255f,iColor.G / 255f, iColor.B / 255f);
								//    GL.Color3(Vector3.Lerp(_BackColor, _ShapeColor, iOpacity));
								//}
								//else GL.Color4(SD.Color.FromArgb((int)(iOpacity * 127), iColor));
								GL.Color4(SD.Color.FromArgb((int)(iOpacity * 127), iColor));
								
								foreach(var cVertex in _Vertices) GL.Vertex3(cVertex);
							}
						}
						GL.End();
					}

					if(iLineWidth != 0)
					{
						GL.LineWidth(iLineWidth);
						GL.Color4(iColor);
						GL.Begin(PrimitiveType.LineLoop);
						{
							foreach(var cVertex in _Vertices)
							{
								//GL.Vertex3(cVertex + new Vector3d(0,0, 0.2));
								GL.Vertex3(cVertex);
							}
						}
						GL.End();

						var _VolumeMode = iNode.Frame.VolumeRenderingMode;
						var _NodeHeight = iOpacity * 2.0 - 0.1;

						if(_VolumeMode != 0)
						if(iIsPerspMode && _NodeHeight > 0)
						{
							var _TopOffs = new Vector3d(0,0, _VolumeMode == 1 ? -0.5 : (_VolumeMode == 2 ? +0.3 : _NodeHeight));
							
							
							
							if(_VolumeMode != 1)
							{
								GL.Begin(PrimitiveType.Lines);
								{
									foreach(var cVertex in _Vertices)
									{
										GL.Vertex3(cVertex);
										GL.Vertex3(cVertex + _TopOffs);
									}
								}
								GL.End();
								GL.Begin(PrimitiveType.LineLoop);
								{
									foreach(var cVertex in _Vertices)
									{
										//GL.Vertex3(cVertex);
										GL.Vertex3(cVertex + _TopOffs);
									}
								}
								GL.End();
							}

							//var _ShapeColor = 
							//var _BotWallColor = SD.Color.FromArgb((byte)(255 * Math.Min(iOpacity * 3.0, 1.0)), iColor);

							//if(false)
							{
								var _BotWallColor = SD.Color.FromArgb((byte)(255 * Math.Min(iOpacity * (_VolumeMode == 1 ? 1.0 : 3.0), 1.0)), iColor);//SD.Color.FromArgb((byte)(255 * Math.Min(_NodeHeight * 2.0, 1.0)), SD.Color.Black);
								var _TopWallColor = SD.Color.FromArgb((byte)(255 * Math.Min(iOpacity * (_VolumeMode == 1 ? 0 : 1.0), 0.5)), iColor);// SD.Color.Transparent;//SD.Color.FromArgb((byte)(255 * Math.Min(_NodeHeight * 1.0, 1.0)), iColor);
								var _CeilingColor = _TopWallColor;//SD.Color.FromArgb((byte)(255 * Math.Min(_NodeHeight * 1.0, 1.0)), iColor);

								GL.Begin(PrimitiveType.QuadStrip);
								{
									//GL.Color4(SD.Color.FromArgb((byte)(255 * Math.Min(iOpacity * 4.0, 1.0)), iNode.Palette.Colors[iNode.Color]));
									//GL.Color4(iNode.Palette.Colors[iNode.Color]);

									
									foreach(var cVertex in _Vertices)
									{
										GL.Color4(_BotWallColor); GL.Vertex3(cVertex);
										GL.Color4(_TopWallColor); GL.Vertex3(cVertex + _TopOffs);
									}
									GL.Color4(_BotWallColor); GL.Vertex3(_Vertices[0]);
									GL.Color4(_TopWallColor); GL.Vertex3(_Vertices[0] + _TopOffs);
								}
								GL.End();
								GL.Begin(PrimitiveType.Polygon);
								{
									GL.Color4(_CeilingColor); 

									foreach(var cVertex in _Vertices) GL.Vertex3(cVertex + _TopOffs);
								}
								GL.End();
							}
						}
					}

					if(iNode.IsPointerOver)
					{
						//GL.PointSize(10f);
						//GL.Begin(PrimitiveType.Points);
						//{
						//    for(var cVi = 0; cVi < iShape.Vertices.Length; cVi++)
						//    {
						//        GL.Color4(SDColor.FromArgb((int)(255 * (1 - ((float)cVi / iShape.Vertices.Length))), iObj.Palette.Colors[5]));

						//        GL.Vertex3(iShape.Vertices[cVi]);
						//    }
							
							
						//}
						//GL.End();
					}

					

					///if(iNode.IsHighlighting && DateTime.Now.Millisecond / 250 % 2 == 0)
					if(iNode.IsHighlighting)
					{
						//GL.LineWidth(2);
						GL.LineWidth(DateTime.Now.Millisecond / 250 % 2 == 0 ? 2 : 1);
						GL.Color4(iColor);

						GL.Begin(PrimitiveType.Lines);
						{
							foreach(var cVertex in _Vertices)
							{
								//GL.Vertex3(cVertex + new Vector3d(0,0, 0.2));
								GL.Vertex3(Vector3d.Zero);
								//GL.Vertex3(cVertex * 0.2);
								GL.Vertex3(cVertex * 0.5);
							}
						}
						GL.End();
					}


					//GL.PointSize(5f);
					//GL.Begin(PrimitiveType.Points);
					//{
					//    GL.Color4(Screen.Colors[iObj.Color]);
					//    foreach(var cVertex in iShape.Vertices) GL.Vertex2(cVertex);
					//}
					//GL.End();
				}
				public static void DrawJoints         (NGonNode iNode)
				{
					GL.Color4(iNode.Palette.GlareColor);

					foreach(var cJoint in iNode.Joints)
					{
						cJoint.Draw();
					}
				}
				
				public static void DrawLines          (NGonNode iNode)
				{
					foreach(var cLine in iNode.Lines)
					{
						cLine.Draw();
					}
				}
				
				public static void DrawPorts          (NGonNode iNode)
				{
					if(iNode.Ports.Count == 0) return;

					var _InR = iNode.InnerRadius;
					var _OuR = iNode.OuterRadius;
					

					foreach(var cPort in iNode.Ports)
					{
						cPort.Draw();
					}
				}
				
				
				public static void DrawEdgePortGrid   (NGonNode iNode)
				{
					var _EdgeIndex = iNode.HoverSegment;
					var _Pointer   = iNode.Pointer;

					var _P1Angle = ( _EdgeIndex      * iNode.SideAngle) - iNode.HalfSideAngle;
					var _P2Angle = ((_EdgeIndex + 1) * iNode.SideAngle) - iNode.HalfSideAngle;


					var _EdgeP1 = new Vector2d(Math.Cos(_P1Angle),Math.Sin(_P1Angle)) * iNode.OuterRadius;
					var _EdgeP2 = new Vector2d(Math.Cos(_P2Angle),Math.Sin(_P2Angle)) * iNode.OuterRadius;

					var _ZoomFactor = MathEx.Clamp(iNode.Viewpoint.Position.Z * 10, 1, 256);
					///GCon.Message("Zoom=" + _ZoomF + ", PosZ=" + iNode.Viewpoint.Position.Z);
					var _InterpPoints = new Vector2d[(int)Math.Pow(2, Math.Round(Math.Log(_ZoomFactor, 2)))];
					{
						for(var cPi = 1; cPi < _InterpPoints.Length; cPi++)
						{
							var cFrac = (double)cPi / _InterpPoints.Length;
							var cPoint = Vector2d.Lerp(_EdgeP1, _EdgeP2, cFrac);

							_InterpPoints[cPi] = cPoint;
						}
					}

					
					GL.LineWidth(1);
					GL.PointSize(10);
					GL.Begin(PrimitiveType.Points);
					{
						GL.Color3(1.0,1.0,0.0);
						
						foreach(var cPoint in _InterpPoints)
						{
							GL.Vertex2(cPoint);
						}
					}
					GL.End();
				}


				public static void DrawPointsOnShape  (NGonNode iNode, SchemeShape iShape, double iAngle)
				{
					var _InR = iNode.InnerRadius;
					var _OuR = iNode.OuterRadius;
					var _ProjR = Projections.GetSideDistance(iNode, iAngle);
					//_NGon.GetSideDistance(_NGon.MousePosition);
					//var _ProjS = Math.Abs(Math.Cos(( iAngle) * _NGon.Sides / 2));

					//var _IsEvenSideNGon = _NGon.Sides % 2 == 0;
					//var _HalfSideAngle  = _NGon.HalfSideAngle;
					//var _AbsAngle = iAngle + Math.PI;
					
					//var _SegmAbsPos = (_AbsAngle / (_HalfSideAngle * 2.0));
					//var _SegmRelPos = Math.Abs(_SegmAbsPos - Math.Round(_SegmAbsPos));
					//var _SegmPosNorm = 2.0 * (_NGon.Sides % 2 == 0 ? _SegmRelPos : 0.5 - _SegmRelPos);

					//var _ProjSegmAngle = _HalfSideAngle * _SegmPosNorm;

					////var _ProjR = _OuR - ((_OuR - _InR) * Math.Cos(_ProjSegmAngle));//(Math.Cos(_ProjSegmAngle));
					//var _ProjR = _InR / Math.Cos(_ProjSegmAngle);//(Math.Cos(_ProjSegmAngle));
					//Console.WriteLine(_ProjSegmAngle / Math.PI * 180);

					var _X = Math.Cos(iAngle);
					var _Y = Math.Sin(iAngle);


					//var _MinX = _X * _MinR;
					//var _MinY = _Y * _MinR;

					//var _MaxX = Math.Sin(iAngle) * _MaxRadius;
					//var _MaxY = Math.Cos(iAngle) * _MaxRadius;


					
					//GL.PointSize(10f);
					GL.Color4(SD.Color.FromArgb(127, SD.Color.Magenta));

					GL.Begin(PrimitiveType.Lines);
					{
						GL.Vertex2(0,0);
						GL.Vertex2(_X * _InR, _Y * _InR);

						GL.Vertex2(0,0);
						GL.Vertex2(_X * _OuR, _Y * _OuR);
					}
					GL.End();

					

					GL.PointSize(10f);
					GL.Begin(PrimitiveType.Points);
					{
						
						GL.Color4(SD.Color.Orange);
						GL.Vertex2(0,0);

						GL.Color4(SD.Color.Cyan);
						GL.Vertex2(_X * _InR,_Y * _InR);

						GL.Color4(SD.Color.Magenta);
						GL.Vertex2(_X * _OuR,_Y * _OuR);

						
					}
					GL.End();


					GL.PointSize(20f);
					GL.Begin(PrimitiveType.Points);
					{
						GL.Vertex2(_X * _ProjR,_Y * _ProjR);
					}
					GL.End();
				}
				public static void DrawPointer        (NGonNode iNode, double iRadius)
				{
					GL.LineWidth(1);
					GL.Begin(PrimitiveType.LineLoop);
					{
						for(var cA = 0.0; cA < Math.PI * 2; cA += Math.PI / 24)
						{
							GL.Vertex3(iNode.Pointer + new Vector3d(Math.Sin(cA) * iRadius, Math.Cos(cA) * iRadius, 0.0));
						}
					}
					GL.End();
				}
			}
			public class Processing
			{
				//public static void UpdatePorts(NGonNode iNode, bool iIsRecursive)
				//{
				//    iNode.UpdatePorts         ();

				//    if(iIsRecursive) foreach(var cChild in iNode.Children) UpdatePorts(cChild, true);
				//}
				
				public struct Linking
				{
					public static void ResetAllLinks(NGonNode iNode, bool iIsRecursive)
				{
					if(iIsRecursive)
					{
						foreach(var cChild in iNode.Children) ResetAllLinks(cChild, true);
					}

					foreach(var cPort in iNode.Ports)
					{
						cPort.InnerPort = null;
						cPort.OuterPort = null;

						cPort.InnerPin = null;
						cPort.OuterPin = null;
					}
					foreach(var cJoint in iNode.Joints)
					{
						cJoint.Graph = new NGonJointGraph(cJoint);
						cJoint.Port  = null;
					}
				}
					public static void ConnectAll(NGonNode iNode, bool iIsRecursive)
					{
						if(iNode.Parent == null) ResetAllLinks(iNode, true);

						
						if(iIsRecursive)
						{
							///~~ children must be processed before parent because ports create OUTER joints;
							
							///foreach(var cChild in iNode.Children) CheckPorts(cChild);
							foreach(var cChild in iNode.Children) ConnectAll(cChild, true);
						}
						///CheckPorts(iNode);

						
						UpdatePorts         (iNode);
						//UpdatePorts         (iNode, true,true);
						MergeJoints         (iNode);
						
						MergeJointGraphs    (iNode);
						LinkPorts           (iNode);
						//iNode.LinkPortsWithGraphs ();

						
					}

					public static double JoiningThreshold = 0.00001;
					public static void CheckPorts(NGonNode iNode)
					{
						for(var caPi = 0; caPi < iNode.Ports.Count; caPi++)
						{
							var caPort = iNode.Ports[caPi];

							for(var cbPi = 0; cbPi < iNode.Ports.Count; cbPi++)
							{
								var cbPort = iNode.Ports[cbPi];

								if(caPort == cbPort) continue;

								if(Math.Abs(caPort.Bearing - cbPort.Bearing) < JoiningThreshold)
								{
									throw new WTFE();
								}
							}
						}
					}

					public static void UpdatePorts(NGonNode iNode)
					{
						iNode.UpdatePorts(true,true);

						foreach(var caPort in iNode.Ports)
						{
							if(iNode.Parent != null)
							{
								
								foreach(var cSiblingNode in iNode.Parent.Children)
								{
									///if(cSiblingNode == iNode) continue;

									foreach(var cbPort in cSiblingNode.Ports)
									{
										if(caPort == cbPort) continue;

										if((caPort.OuterEdgePoint - cbPort.OuterEdgePoint).Length < JoiningThreshold)
										{
											if(caPort.OuterPort == null && cbPort.OuterPort == null)
											{
												caPort.OuterPort = cbPort;
												cbPort.OuterPort = caPort;
											}
											else if(caPort.OuterPort == cbPort && cbPort.OuterPort == caPort)
											{
												continue;
											}
											///else throw new WTFE();
										}
									}
								}


								foreach(var cbPort in iNode.Parent.Ports)
								{
									if((caPort.OuterEdgePoint - cbPort.InnerEdgePoint).Length < JoiningThreshold)
									{
										if(caPort.OuterPort == null && cbPort.InnerPort == null)
										{
											caPort.OuterPort = cbPort;
											cbPort.InnerPort = caPort;
										}
										else if(caPort.OuterPort == cbPort && cbPort.InnerPort == caPort)
										{
											continue;
										}
										else throw new WTFE();
									}
								}
							}
							
							///cPort.EdgePoint
						}
					}
					public static void MergeJoints(NGonNode iNode)
					{
						for(var caJi = 0; caJi < iNode.Joints.Count; caJi++)
						{
							var caJoint = iNode.Joints[caJi];

							for(var cbJi = caJi + 1; cbJi < iNode.Joints.Count; cbJi++)
							{
								var cbJoint = iNode.Joints[cbJi];
								
								if(cbJoint.Port == null && cbJoint.Lines.Count == 0)
								{
									cbJoint.Graph.Joints.Remove(cbJoint);
									cbJoint.Parent.Joints.Remove(cbJoint);

									G.Debug.Message("!Free joint detected/removed");

									cbJi--;
								}
								else if((caJoint.Position - cbJoint.Position).Length < 0.001)
								{
									caJoint.MergeWith(cbJoint);

									cbJoint.Graph.Joints.Remove(cbJoint);
									iNode.Joints.RemoveAt(cbJi);

									cbJi--;
								}
							}
						}
					}
					
					public static void MergeJointGraphs(NGonNode iNode)
					{
						foreach(var cJoint in iNode.Joints)
						{
							var cGraph = cJoint.Graph;

							foreach(var cLine in cJoint.Lines)
							{
								//var cLine.Joint1
								if(cLine.Joint1 != cJoint)
								{
									if(cLine.Joint1.Graph != cGraph)
									{
										cGraph.MergeWith(cLine.Joint1.Graph);
									}
								}
								if(cLine.Joint2 != cJoint)
								{
									if(cLine.Joint2.Graph != cGraph)
									{
										cGraph.MergeWith(cLine.Joint2.Graph);
									}
								}
							}
						//    if(cJoint.Graph == null)
						//    {
						//        cJoint.Graph = new NGonJointGraph();
						//    }
						}


					}
					public static void LinkPorts(NGonNode iNode)
					{
						foreach(var cPort in iNode.Ports)
						{
							///cPort.EdgePoint
						}
					}
				}
				//public struct IO
				//{
				//    public static void ResetIO(NGonSchemeFrame iFrame)
				//    {
						
				//    }
				//    public static void AttachIO(NGonSchemeFrame iFrame)
				//    {

				//        //iFrame.Scheme.Children.For
						
				//        AttachIO(iFrame.Scheme, true);
				//    }
				//    public static void AttachIO(NGonNode iNode, bool iDoRecursive)
				//    {
				//        if(iNode.Type != "")
				//        {
				//            if(iNode.Type.StartsWith("I["))
				//            {
				//                ///register input node
				//            }
				//            else if(iNode.Type.StartsWith("O["))
				//            {
				//                ///register output node
				//            }
				//            else
				//            {
				//                ///register standard logic/math node
				//            }
				//        }
						
				//        if(iDoRecursive) foreach(var cChild in iNode.Children)
				//        {
				//            AttachIO(cChild, iDoRecursive);
				//        }
				//    }
				//}
				


				//this.MergeJoints();
				//this.MergeJointGraphs();
				//this.LinkPortsWithGraphs();
			}
		}
	}
}
