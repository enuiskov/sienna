using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Text;
using System.Windows.Forms;
using OpenTK;
//using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using AE.Visualization.SchemeObjectModel;


namespace AE.Visualization
{
	public partial class NGonSchemeFrame : SchemeFrame<NGonNode>
	{
		public new partial struct Routines
		{
			public static void ChangeColor(NGonNode iNode, int iColor)
			{
				iNode.Color = iColor;
			}

			public class Rendering
			{
				//public static void SetPerspectiveMatrix  (NGonSchemeFrame iFrame) 
				//{
				//    var _Viewpoint = iFrame.Viewpoint.CurrentState;
				//    var _ViewP     = _Viewpoint.Position;
				//    var _ViewI     = _Viewpoint.Inclination;
				//    var _ScaleF    = 1.0 / _ViewP.Z;// * 0.5;
				//    var _AspectR   = iFrame.AspectRatio;
					

				//    GL.MatrixMode(MatrixMode.Projection);
				//    {
				//        Matrix4 _PerspMat = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver3, (float)iFrame.Width / iFrame.Height, 0.0000001f, 6400);
				//        //Matrix4 _PerspMat = Matrix4.CreatePerspectiveFieldOfView(0.1f, (float)iFrame.Width / iFrame.Height, 0.001f, 6400);
				//        //Matrix4.Crea
				//        GL.LoadMatrix(ref _PerspMat);
				//    }
				//    GL.MatrixMode(MatrixMode.Modelview);
				//    {
				//        //var _Dist = 1.0 / _ViewP.Z;
				//        var _TgtPos = new Vector3d(_ViewP.X, _ViewP.Y, 0.0);
				//        var _EyePos = _TgtPos + iFrame.Viewpoint.CurrentState.PerspEyeOffset;
				//        //var _EyePos = new Vector3d(_ViewP.X;

				//        Matrix4d _LookAtMat = Matrix4d.LookAt(_EyePos, _TgtPos, Vector3d.UnitZ);
				//        GL.LoadMatrix(ref _LookAtMat);
				//    }
				//}

				public static void Draw              (NGonSchemeFrame iFrame)
				{
					GL.BindTexture(TextureTarget.Texture2D, GLCanvasControl.HighResFontAtlas.TexID);

					ZoomableFrame.Routines.Rendering.SetProjectionMatrix(iFrame);
					//if(iFrame.IsPerspectiveMode)
					//{
					//    SetPerspectiveMatrix (iFrame);
					//}
					//else
					//{
					//    ZoomableFrame.Routines.Rendering.SetOrthographicMatrix (iFrame);
					//}
					
					//ZoomableFrame.Routines.Rendering.DrawRootObject      (iFrame);

					//GL.Enable(EnableCap.LineSmooth);
					

					//GL.Enable(EnableCap.Fog);
					////GL.Fog(
					//GL.Fog(FogParameter.FogDensity, 1);
					//GL.Fog(FogParameter.FogStart, 10);
					//GL.Fog(FogParameter.FogEnd, 1000);
					//GL.LineWidth(1);

					ZoomableFrame.Routines.Rendering.DrawUnitSpace(iFrame);
					///ZoomableFrame.Routines.Rendering.DrawPropeller(iFrame);
					





					///if(iFrame.IsPerspectiveMode)
					///{
					///    GL.Clear(ClearBufferMask.DepthBufferBit);
					///    GL.Enable(EnableCap.DepthTest);
					///}
					GL.PushMatrix();
					{
						iFrame.Scheme.Transform();
						iFrame.Scheme.Draw();
					}
					GL.PopMatrix();
					///if(iFrame.IsPerspectiveMode) GL.Disable(EnableCap.DepthTest);



		
					
					
					///DrawPointer(iFrame);
					///DrawViewpoint(iFrame);


					//ZoomableFrame.Routines.Rendering.DrawViewpoint(iFrame);
					//ZoomableFrame.Routines.Rendering.DrawPointer(iFrame);


					
					//{
					//    //DrawContactPoints(iFrame.Scheme as NGonNode);
					//    DrawExtraObjects (iFrame, iFrame.NewObjects);
					//    DrawExtraObject  (iFrame, iFrame.FittingObject);
					//}
					
				}
				//public static void DrawScheme          (SchemeFrame iFrame)
				//{
				//    GL.Enable(EnableCap.LineSmooth);
				//    GL.LineWidth(1);
				//    iFrame.Scheme.Draw();
				//}
				
				//public static void DrawExtraObjects  (NGonSchemeFrame iFrame)
				//{
				//    foreach(var cNode  in iFrame.NewNodes)  DrawExtraObject(iFrame, cNode);
				//    foreach(var cPort  in iFrame.NewPorts)  DrawExtraObject(iFrame, cPort);
				//    foreach(var cLine  in iFrame.NewLines)  DrawExtraObject(iFrame, cLine);
				//    foreach(var cJoint in iFrame.NewJoints) DrawExtraObject(iFrame, cJoint);
				//}

				//public static void DrawExtraObjects  (NGonSchemeFrame iFrame, List<NGonNode> iObjects)
				//{
				//    foreach(var cObj in iObjects)
				//    {
				//        DrawExtraObject(iFrame, cObj);
				//    }
				//}
				public static void DrawPointer       (NGonSchemeFrame iFrame)
				{
					var _P = iFrame.Pointer;

					GL.LineWidth(1);
					GL.Begin(PrimitiveType.Lines);
					{
						GL.Color4(iFrame.Palette.GlareColor);

						GL.Vertex2(_P.X - 0.1, _P.Y);
						GL.Vertex2(_P.X + 0.1, _P.Y);

						GL.Vertex2(_P.X, _P.Y - 0.1);
						GL.Vertex2(_P.X, _P.Y + 0.1);

						GL.Vertex2(_P.Xy);
						GL.Vertex3(_P);
						
					}
					GL.End();

					GL.PointSize(10f);
					GL.Begin(PrimitiveType.Points);
					{
						GL.Color3(0.0,1.0,0.0);
						GL.Vertex3(_P);
					}
					GL.End();
				}
				public static void DrawViewpoint     (NGonSchemeFrame iFrame)
				{
					//var _ViewP = iFrame.Scheme.Viewpoint.Position;
					var _ViewP = iFrame.Viewpoint.CurrentState.Position;
					var _Radius = Viewpoint2D.FieldOfView;

					GL.PushMatrix();

					//GL.MultMatrix(ref iFrame.Scheme.Matrix);
					
					GL.Translate(_ViewP.X, _ViewP.Y, 0.0);
					GL.Scale(_ViewP.Z,_ViewP.Z,1.0);
					GL.Rotate(-iFrame.Viewpoint.CurrentState.Inclination * MathEx.RTD, Vector3d.UnitZ);
					{
						GL.LineWidth(1);
						GL.Color4(iFrame.Palette.GlareColor);
						GL.Begin(PrimitiveType.LineLoop);
						{
							for(var cA = 0.0; cA < Math.PI * 2.0; cA += Math.PI / 24)
							{
								GL.Vertex2(Math.Sin(cA) * _Radius, Math.Cos(cA) * _Radius);
							}
						}
						GL.End();
					}
					GL.PopMatrix();
				}
				public static void DrawExtraObject   (NGonSchemeFrame iFrame, IVisualObject iObj)
				{
					if(iObj == null) return;

					GL.PushMatrix();
					{
						iObj.Transform();
						iObj.Draw();

						//DrawContactPoints(iObj as NGonNode);
					}
					GL.PopMatrix();
				}
				public static void DrawContactPoints (NGonNode iNode)
				{
					//if(iNode.ContactPoints == null) return;

					GL.PointSize(5f);
					GL.Begin(PrimitiveType.Points);
					{
					    GL.Color4(iNode.Palette.Colors[3]);
					    //GL.Color4(Screen.Colors[iObj.Color]);

					    foreach(var cVertex in iNode.OuterContactPoints)
					    {
					        GL.Vertex3(cVertex.Position);
					    }
					}
					GL.End();
				}
				
				//public static void DrawFlowArrow     (NGonNode iObj)
				//{
				//    if(iObj == null) return;

				//    var _ApoL   = iObj.InnerRadius;
				//    var _ArrL   = 0.2;

				//    var _ArrHW1 = 0.1;
				//    var _ArrHW2 = 0.5;


				//    for(var cSide = 0; cSide < iObj.Sides; cSide++)
				//    {
				//        //if(!iObj.Targets[cSide]) continue;
				//        //if(cSide != 0) continue;
				//        if(iObj.Edges[cSide] == EdgeType.Blind) continue;
							
				//        GL.PushMatrix();
				//        GL.Rotate(+((float)cSide / iObj.Sides) * 360f, Vector3.UnitZ);
				//        //ПДюiObj.Transform();
				//        {
				//            GL.Color4(iObj.Palette.Colors[iObj.Color]);

				//            GL.Begin(PrimitiveType.Quads);
				//            {
				//                GL.Vertex2(_ApoL + _ArrL, -_ArrHW1);
				//                GL.Vertex2(_ApoL + _ArrL, +_ArrHW1);
				//                GL.Vertex2(_ApoL,         +_ArrHW2);
				//                GL.Vertex2(_ApoL,         -_ArrHW2);

				//                //GL.Vertex2(-_ArrHW1, _ApoL + _ArrL);
				//                //GL.Vertex2(+_ArrHW1, _ApoL + _ArrL);
				//                //GL.Vertex2(+_ArrHW2, _ApoL);
				//                //GL.Vertex2(-_ArrHW2, _ApoL);
				//            }
				//            GL.End();
				//        }
				//        GL.PopMatrix();
				//    }
				//}
				public static void DrawFlowArrows    (NGonNode iNode)
				{
					//if(iNode == null) return;

					//for(var cSide = 0; cSide < iNode.Sides; cSide++)
					//{
					//    var cEdgeType = iNode.EdgeTypes[cSide];

					//    if(cEdgeType == EdgeType.Blind) continue;



					//    if     (cEdgeType == EdgeType.ForwardPropagation)  DrawFlowArrow(iNode, cSide, iNode.Palette.Colors[4], true);
					//    else if(cEdgeType == EdgeType.BackwardPropagation) DrawFlowArrow(iNode, cSide, iNode.Palette.Colors[9], true);
					//}
				}
				public static void DrawFlowArrow     (NGonNode iNode, int iSide, Color iColor, bool iDoDrawCenterLine)
				{
					var _ApoL   = iNode.InnerRadius;
					var _ArrL   = 0.2;//_ApoL * 0.4;
					var _ArrHW1 = 0.1;
					var _ArrHW2 = 0.5;

					//Matrix4d.Identity.



					GL.PushMatrix();
					GL.Rotate(+((float)iSide / iNode.Sides) * 360f, Vector3.UnitZ);
					//ПДюiObj.Transform();
					{
						GL.Color4(iColor);

						//GL.Begin(PrimitiveType.Quads);
						//{
						//    GL.Vertex2(_ApoL + _ArrL, -_ArrHW1);
						//    GL.Vertex2(_ApoL + _ArrL, +_ArrHW1);
						//    GL.Vertex2(_ApoL,         +_ArrHW2);
						//    GL.Vertex2(_ApoL,         -_ArrHW2);

						//    //GL.Vertex2(-_ArrHW1, _ApoL + _ArrL);
						//    //GL.Vertex2(+_ArrHW1, _ApoL + _ArrL);
						//    //GL.Vertex2(+_ArrHW2, _ApoL);
						//    //GL.Vertex2(-_ArrHW2, _ApoL);
						//}
						//GL.End();

						GL.Begin(PrimitiveType.Lines);
						{
							GL.Vertex2(_ApoL * 0.2, 0);
							GL.Vertex2(_ApoL, 0);
						}
						GL.End();
					}
					GL.PopMatrix();
				}
				//public static void DrawFlowArrow     (NGonNode iObj, int iSide, Color iColor, bool iDoDrawCenterLine)
				//{
				//    var _ApoL   = iObj.InnerRadius;
				//    var _ArrL   = 0.2;//_ApoL * 0.4;
				//    var _ArrHW1 = 0.1;
				//    var _ArrHW2 = 0.5;





				//    GL.PushMatrix();
				//    GL.Rotate(+((float)iSide / iObj.Sides) * 360f, Vector3.UnitZ);
				//    //ПДюiObj.Transform();
				//    {
				//        GL.Color4(iColor);

				//        //GL.Begin(PrimitiveType.Quads);
				//        //{
				//        //    GL.Vertex2(_ApoL + _ArrL, -_ArrHW1);
				//        //    GL.Vertex2(_ApoL + _ArrL, +_ArrHW1);
				//        //    GL.Vertex2(_ApoL,         +_ArrHW2);
				//        //    GL.Vertex2(_ApoL,         -_ArrHW2);

				//        //    //GL.Vertex2(-_ArrHW1, _ApoL + _ArrL);
				//        //    //GL.Vertex2(+_ArrHW1, _ApoL + _ArrL);
				//        //    //GL.Vertex2(+_ArrHW2, _ApoL);
				//        //    //GL.Vertex2(-_ArrHW2, _ApoL);
				//        //}
				//        //GL.End();

				//        GL.Begin(PrimitiveType.Lines);
				//        {
				//            GL.Vertex2(_ApoL * 0.2, 0);
				//            GL.Vertex2(_ApoL, 0);
				//        }
				//        GL.End();
				//    }
				//    GL.PopMatrix();
				//}
				public static void DrawHoverSegment  (NGonNode iNode)
				{
					if(iNode == null)            return;
					if(iNode.HoverSegment == -1) return;
					if(!iNode.IsPointerOver)     return;

					DrawFlowArrow(iNode, iNode.HoverSegment, Color.FromArgb(128, iNode.Palette.Colors[iNode.Color]), true);

					//var _ApoL   = iNode.InnerRadius;
					//var _ArrL   = 1.0;//_ApoL * 0.4;
					//var _ArrHW1 = _ApoL * 0.1;
					//var _ArrHW2 = 0.5;



					//GL.PushMatrix();
					//GL.Rotate(+((float)iNode.HoverSegment / iNode.Sides) * 360f, Vector3.UnitZ);
					//{
					//    GL.Color4(Color.FromArgb(128, iNode.Palette.Colors[iNode.Color]));

					//    GL.Begin(PrimitiveType.Triangles);
					//    {s
					//        GL.Vertex2(0, 0);
					//        GL.Vertex2(_ApoL, +_ArrHW2);
					//        GL.Vertex2(_ApoL, -_ArrHW2);
					//    }
					//    GL.End();
					//}
					//GL.PopMatrix();






					////for(var cSide = 0; cSide < iNode.Sides; cSide++)
					////{
					////    //if(!iNode.Targets[cSide]) continue;
					////    if(iNode.Edges[cSide] == EdgeType.Blind) continue;
							
					////    GL.PushMatrix();
					////    GL.Rotate(-((float)cSide / iNode.Sides) * 360f, Vector3.UnitZ);
					////    //ПДюiNode.Transform();
					////    {
					////        GL.Color4(iNode.Palette.Colors[iNode.Color]);

					////        GL.Begin(PrimitiveType.Quads);
					////        {
					////            GL.Vertex2(-_ArrHW1, _ApoL + _ArrL);
					////            GL.Vertex2(+_ArrHW1, _ApoL + _ArrL);
					////            GL.Vertex2(+_ArrHW2, _ApoL);
					////            GL.Vertex2(-_ArrHW2, _ApoL);
					////        }
					////        GL.End();
					////    }
					////    GL.PopMatrix();
					////}
				}
				//public static void DrawNeighbours    (NGonNode iObj)
				//{
				//    if(iObj == null) return;

				//    var _ApoL   = iObj.InnerRadius;
				//    var _ArrL   = _ApoL * 0.4;
				//    var _ArrHW1 = _ApoL * 0.1;
				//    var _ArrHW2 = 0.5;

				//    GL.PushMatrix();
				//    ///iObj.Transform();
				//    {
				//        GL.Color4(iObj.Palette.Colors[iObj.Color]);

				//        GL.Begin(PrimitiveType.Quads);
				//        {
				//            GL.Vertex2(-_ArrHW1, _ApoL + _ArrL);
				//            GL.Vertex2(+_ArrHW1, _ApoL + _ArrL);
				//            GL.Vertex2(+_ArrHW2, _ApoL);
				//            GL.Vertex2(-_ArrHW2, _ApoL);
				//        }
				//        GL.End();
				//    }
				//    GL.PopMatrix();
				//}

				public static void DrawEdgeLinkPoints(NGonNode iNode)
				{
					var _Radius = iNode.InnerRadius;

					GL.PointSize(5f);
					GL.Begin(PrimitiveType.Points);
					{
						GL.Color3(iNode.Palette.Colors[7]);

						for(var cEi = 0; cEi < iNode.Sides; cEi++)
						{
							var cAngle = (cEi * iNode.SideAngle);/// - (Math.PI / 2.0);

							var cPoint = Vector3d.Transform(new Vector3d(Math.Cos(cAngle) * _Radius, Math.Sin(cAngle) * _Radius, 0.0), iNode.Matrix);

							GL.Vertex3(cPoint);
							//GL.
						}
					}
					GL.End();
					//Math.Cos(
				}
			
				public static void DrawNewObjects(NGonSchemeFrame iFrame, NGonNode iHostNode)
				{
					switch(iFrame.CurrentMode)
					{
						case EditorMode.NodeCreate :
						{
							//NGonSchemeFrame.Routines.Rendering.DrawExtraObjects (_Frame);
							
							NGonSchemeFrame.Routines.Rendering.DrawExtraObject  (iFrame, iFrame.FittingObject);
							foreach(var cNode in iFrame.NewNodes)  DrawExtraObject(iFrame, cNode);

							break;
						}
						case EditorMode.LineCreate :
						{
							GL.Color4(Color.Red);
							foreach(var cLine in iFrame.NewLines)
							{
								if(cLine.Parent == iFrame.HoverNode) DrawExtraObject(iFrame, cLine);
							}
							foreach(var cJoint in iFrame.NewJoints)
							{
								if(cJoint.Parent == iFrame.HoverNode) DrawExtraObject(iFrame, cJoint);
							}

							break;
						}
						case EditorMode.PortCreate :
						{
							GL.Color4(Color.Red);
							foreach(var cPort in iFrame.NewPorts)
							{
								cPort.Owner = iHostNode;
								DrawExtraObject(iFrame, cPort);
							}

							break;
						}

						default : break;
					}
					//iFrame.New
				}
				
			}

			public class Tests
			{
				public static void Atlas(NGonSchemeFrame iViewport)
				{
					//var _SVP   = iViewport; if(_SVP == null) return;
					//var _RNG   = new Random();
					//var _Sizes = new List<Size>();
					//{
					//    //for(int _Count = (int)((DateTime.Now.Second / 60.0) * 200), cI = 0; cI < _Count; cI++)
					//    for(int _Count = 100, cI = 0; cI < _Count; cI++)
					//    {
					//        //var cSize = new Size(_RNG.Next(1,64), _RNG.Next(1,64));
					//        var cPowOf2 = (int)Math.Pow(2, _RNG.Next(0, 4));
					//        var cSize = new Size(cPowOf2,cPowOf2);//_RNG.Next(1,64), _RNG.Next(1,64));


					//        _Sizes.Add(cSize);
					//    }

					//    //_Sizes.Sort(new Comparison<Size>(this.Atlas.CompareAreas));
					//}
					//iViewport.SchemeAtlas.AllocateRegions(_Sizes.ToArray(), 256);

					//var _Grx = Graphics.FromImage((_SVP.Scheme.Children[0] as SchemeObjectModel.PixelMap).Image);
					//{
					//    _Grx.Clear(Color.Transparent);
					//    foreach(var cRegion in _SVP.SchemeAtlas.Regions)
					//    {
					//        _Grx.FillRectangle(new SolidBrush(Color.FromArgb(128, (int)(_RNG.NextDouble() * 255), (int)(_RNG.NextDouble() * 255), (int)(_RNG.NextDouble() * 255))), cRegion.Bounds);
					//    }
					//}
					//_Grx.DrawString("Processed: " + _SVP.SchemeAtlas.Regions.Length + "/" + _Sizes.Count + "(" + Math.Floor((float)_SVP.SchemeAtlas.Regions.Length / _Sizes.Count * 100) +  "%)", new Font(FontFamily.GenericSansSerif, 30), Brushes.Red, 10,10);

					////_Grx.
				}
			}
			//public class Calculations
			//{
				
			//}
			public class Selections
			{
				public static NGonNode           GetHoverNode     (NGonNode iParentNode)
				{
					if(iParentNode.IsPointerOver) return iParentNode;

					foreach(var cChildN in iParentNode.Children)
					{
						var cHoverN = GetHoverNode(cChildN as NGonNode);

						if(cHoverN != null)
						{
							return cHoverN;
						}
						else continue;
					}
				
					return null;
				}
				public static List<NGonEdgePort> GetHoverPorts    (NGonNode iParentNode)
				{
					///NGonNode.Routines.Projections.GetNearestJoints(iParentNode, iParentNode.Pointer
					var oPorts = new List<NGonEdgePort>();
					{
						foreach(var cPort in iParentNode.Ports)
						{
							if(cPort.IsPointerOver) oPorts.Add(cPort);
						}
					}
					return oPorts;
				}
				public static List<NGonLine>     GetHoverLines    (NGonNode iParentNode)
				{
					///NGonNode.Routines.Projections.GetNearestJoints(iParentNode, iParentNode.Pointer
					var oLines = new List<NGonLine>();
					{
						foreach(var cLine in iParentNode.Lines)
						{
							if(cLine.IsPointerOver) oLines.Add(cLine);
						}
					}
					return oLines;
				}
				public static List<NGonJoint>    GetHoverJoints   (NGonNode iParentNode)
				{
					///NGonNode.Routines.Projections.GetNearestJoints(iParentNode, iParentNode.Pointer
					var oJoints = new List<NGonJoint>();
					{
						foreach(var cJoint in iParentNode.Joints)
						{
							if(cJoint.IsPointerOver) oJoints.Add(cJoint);
						}
					}
					return oJoints;
				}
				
				
				public static List<NGonNode>     SelectObjectGroup (NGonNode iParentNode)
				{
					//switch()

					return SelectNodeGroup(iParentNode);
				}
				public static List<NGonNode>     SelectNodeGroup   (NGonNode iParentNode)
				{
					var _HvrNode = GetHoverNode(iParentNode);
					if(_HvrNode == null) return new List<NGonNode>();
					
					//List<NGonNode> 
					return GetAllNeighbours(_HvrNode);// : new List<NGonNode>();
				}
/**

OOOOO
OOOOO
OOOOO
OOOOO

*/
				public static List<NGonNode>     GetAllNeighbours  (NGonNode iBaseNode)
				{
					if(iBaseNode == null) throw new WTFE();

					var oAllNeighbours = new List<NGonNode>();
					{
						oAllNeighbours.Add(iBaseNode);

						List<NGonNode> _NewFoundNodes;
						{
							do
							{
								_NewFoundNodes = new List<NGonNode>();

								foreach(var cFoundNode in oAllNeighbours)
								{
									foreach(var cPoint in cFoundNode.OuterContactPoints)
									{
										if(cPoint.Nodes == null) throw new WTFE();

										foreach(NGonNode cNode in cPoint.Nodes)
										{
											if(!oAllNeighbours.Contains(cNode) && !_NewFoundNodes.Contains(cNode))
											{
												_NewFoundNodes.Add(cNode);
											}
										}
									}
								}
								oAllNeighbours.AddRange(_NewFoundNodes);
							}
							while(_NewFoundNodes.Count != 0);
						}
						//foreach(var cSiblingNode in iBaseNode.Neighbours)
						//{
						//    if(cSiblingNode == null) continue;
						//    if(oAllNeighbours.Contains(cSiblingNode)) continue;

						//    oAllNeighbours.Add(cSiblingNode);

							
						//    //var cNeighNodes = GetAllNeighbours(cSiblingNode,oAllNeighbours);
						//    //{
							
						//    //}

						//    //oAllNeighbours.AddRange(cNeighNodes);
						//}
					}
					return oAllNeighbours;
				}
				//public static List<NGonNode> GetAllNeighbours(NGonNode iBaseNode, List<NGonNode> iExceptList)
				//{
				//    List<NGonNode> oAllNeighbours = iExceptList != null ? iExceptList : new List<NGonNode>();
				//    {
				//        if(!oAllNeighbours.Contains(iBaseNode)) oAllNeighbours.Add(iBaseNode);

				//        if(iBaseNode != null) foreach(var cSiblingNode in iBaseNode.Neighbours)
				//        {
				//            if(cSiblingNode == null) continue;
				//            if(oAllNeighbours.Contains(cSiblingNode)) continue;

				//            var cNeighNodes = GetAllNeighbours(cSiblingNode,oAllNeighbours);
				//            {
							
				//            }

				//            oAllNeighbours.AddRange(cNeighNodes);
				//        }
				//    }
				//    return oAllNeighbours;
				//}
			}
			public class Editing
			{
				public static void BeginObjects (NGonSchemeFrame iFrame)
				{
					switch(iFrame.CurrentMode)
					{
						case EditorMode.NodeCreate:  BeginNodes(iFrame); break;
						case EditorMode.PortCreate:  BeginPorts(iFrame); break;
						case EditorMode.JointCreate: BeginJoints(iFrame); break;
						case EditorMode.LineCreate:  BeginLines(iFrame); break;
						
						
						default : throw new WTFE();
					}

					//iFrame.SelectedNodes.Clear();
					//iFrame.SelectedPorts.Clear();
					//iFrame.SelectedLines.Clear();
					//iFrame.SelectedJoints.Clear();

					//iFrame.Helpers.Clear();
				}
				public static void AddObjects   (NGonSchemeFrame iFrame)
				{
					switch(iFrame.CurrentMode)
					{
						case EditorMode.NodeCreate:  AddNodes(iFrame); break;
						case EditorMode.PortCreate:  AddPorts(iFrame); break;
						case EditorMode.JointCreate: AddJoints(iFrame); break;
						case EditorMode.LineCreate:  AddLines(iFrame); break;
						
						
						default : throw new WTFE();
					}
				}
				public static void EndObjects   (NGonSchemeFrame iFrame)
				{
					switch(iFrame.CurrentMode)
					{
						case EditorMode.NodeCreate:  EndNodes(iFrame);  break;
						case EditorMode.PortCreate:  EndPorts(iFrame);  break;
						case EditorMode.JointCreate: EndJoints(iFrame); break;
						case EditorMode.LineCreate:  EndLines(iFrame);  break;
						
						///default : throw new WTFE();
					}
					iFrame.OperandNode = null;
				}

				public static void BeginNodes   (NGonSchemeFrame iFrame)
				{
					AddNodes(iFrame);
					//var _HvrNode = NGonSchemeFrame.Routines.Selections.GetHoverNode(iFrame.Scheme);

					//if(iFrame.NewNodes.Count != 0)
					//{
						
					//    foreach(var cNode in iFrame.NewNodes)
					//    {
					//        cNode.Position = iFrame.FittingObject.Position;

					//        _HvrNode.Children.Add(cNode);

					//        iFrame.CreationHistory.Add(cNode);
							

					//        while(iFrame.CreationHistory.Count > 100)
					//        {
					//            iFrame.CreationHistory.RemoveAt(0);
					//        }
					//    }
					//    //this.NewObject.Position = this.FittingObject.Position;
					//    //this.RootObject.Children.Add(this.NewObject);

					//    iFrame.UpdateContactPoints();
					//    iFrame.UpdateNodeLinks();

					//    iFrame.NewNodes.Clear();

					//}
					
					//iFrame.Selection.Clear();


					//NGonNode _NewNode;
					//{
					//    if(iFrame.TemplateNode != null)
					//    {
					//        _NewNode = NGonNode.Routines.DeepClone(iFrame.TemplateNode);
					//    }
					//    else
					//    {
					//        _NewNode = new NGonNode(NGonsTextureAtlas.AvailableNGonTypes[iFrame.CurrentNGonType], iFrame.CurrentColor);
					//        {
					//            _NewNode.Parent   = _HvrObj;
					//            _NewNode.Scale    = iFrame.CurrentScale;
					//            _NewNode.Rotation = iFrame.CurrentAngle;
					//            _NewNode.Position = iFrame.Pointer;//new Vector3d(this.MousePosition.X, this.MousePosition.Y, 0.0);
					//        }
					//    }
					//}
					
					

					//iFrame.NewNodes.Add(_NewNode);

					//iFrame.UpdateFittingNodeect();
				}
				public static void BeginPorts   (NGonSchemeFrame iFrame)
				{
					iFrame.NewPorts.Add(new NGonEdgePort{Bearing = 0, Elevation = 0, Color = iFrame.CurrentColor});
				}
				public static void BeginJoints  (NGonSchemeFrame iFrame)
				{
					
				}
				public static void BeginLines   (NGonSchemeFrame iFrame)
				{
					iFrame.ResetNewObjects();

					//var ;
					//var _HvrNode = iFrame.HoverNode;
					
					iFrame.NewJoints.Add(new NGonJoint{Parent = iFrame.HoverNode, Position = iFrame.HoverNode != null ? iFrame.HoverNode.Pointer : Vector3d.Zero});
				}

				public static void EndNodes     (NGonSchemeFrame iFrame)
				{
					
				}
				public static void EndPorts     (NGonSchemeFrame iFrame)
				{
					foreach(var cPort in iFrame.NewPorts)
					{
						///cPort.Owner.Joints.Remove(cPort.OuterJoint);
						//cPort.Owner.Joints.Remove(cPort.InnerJoint);
						//iFrame.HoverNode.Joints.Remove(
					}
					
					iFrame.HoverNode.Helpers.RemoveAll(NGonRulerHelper.Match);
				}
				public static void EndJoints    (NGonSchemeFrame iFrame)
				{
					
				}
				public static void EndLines     (NGonSchemeFrame iFrame)
				{
					///var _HvrNode = iFrame.HoverNode; if(_HvrNode == null) return;// NGonSchemeFrame.Routines.Selections.GetHoverNode(iFrame.Scheme);
					
					//iFrame.HoverNode.Helpers.RemoveAll(NGonProtractorHelper.Match);
					
					if(iFrame.OperandNode != null && iFrame.NewLines.Count == 1)
					{
						var _ExJoint = iFrame.NewJoints[0];
						iFrame.OperandNode.Joints.Add(_ExJoint);

						//AddLines(iFrame);
						///iFrame.New

						//iFra
						//iFrame.NewJoints.RemoveAt(0);
						//iFrame.NewLines.Clear();
						//AddLines(iFrame);
						///~~is other joint

						
						//iFrame.OperandNode.Joints.Add(iFrame.NewJoints[0]);///.Lines.Remove(iFrame.NewLines[0]);
						//var _IsLineRemoved = iFrame.OperandNode.Lines.Remove(iFrame.NewLines[0]);
						//{
						//    if(_IsLineRemoved)
						//    {
							
						//    }
						//}

					}
					//if()
					//{
					//    //iFrame.OperandNode
					//    ///AddLines(iFrame);
					//}

					(iFrame.OperandNode ?? iFrame.HoverNode).Helpers.RemoveAll(NGonProtractorHelper.Match);
				//    if   (iFrame.OperandNode != null) iFrame.OperandNode.Helpers.RemoveAll(NGonProtractorHelper.Match);
				//    else                              iFrame.HoverNode.Helpers.RemoveAll(NGonProtractorHelper.Match);
				}

				public static void AddNodes     (NGonSchemeFrame iFrame)
				{
					if(iFrame.HoverNode == null) return;

					if(iFrame.NewNodes.Count != 0)
					{
						
						foreach(var cNode in iFrame.NewNodes)
						{
							cNode.Position = iFrame.FittingObject.Position;

							iFrame.HoverNode.Children.Add(cNode);

							iFrame.CreationHistory.Add(cNode);
							

							while(iFrame.CreationHistory.Count > 100)
							{
								iFrame.CreationHistory.RemoveAt(0);
							}
						}
						//this.NewObject.Position = this.FittingObject.Position;
						//this.RootObject.Children.Add(this.NewObject);

						iFrame.UpdateContactPoints();
						iFrame.UpdateNodeLinks();

						iFrame.NewNodes.Clear();

					}
					
					iFrame.Selection.Clear();

					NGonNode _NewNode;
					{
						if(iFrame.IsPasteMode && iFrame.TemplateNode != null)
						{
							_NewNode = NGonNode.Routines.DeepClone(iFrame.TemplateNode);
						}
						else
						{
							_NewNode = new NGonNode(NGonsTextureAtlas.AvailableNGonTypes[iFrame.CurrentNGonType], iFrame.CurrentColor);
							{
								_NewNode.Parent   = iFrame.HoverNode;
								_NewNode.Scale    = iFrame.CurrentScale;
								_NewNode.Rotation = iFrame.CurrentAngle;
								_NewNode.Position = iFrame.Pointer;//new Vector3d(this.MousePosition.X, this.MousePosition.Y, 0.0);
								_NewNode.UpdatePalette(iFrame.Palette.IsLightTheme);
							}
						}
					}

					iFrame.NewNodes.Add(_NewNode);

					iFrame.UpdateFittingObject();
				}
				public static void AddPorts     (NGonSchemeFrame iFrame)
				{
					if(iFrame.HoverNode == null) return;

					var _Rulers = iFrame.HoverNode.Helpers.FindAll(NGonRulerHelper.Match);

					if(_Rulers.Count != 0 && (_Rulers[0] as NGonRulerHelper).InterPoints.Length == 0) return;

					var _Port = iFrame.NewPorts[0];

					iFrame.HoverNode.Ports.Add(_Port);
					//_HvrNode.Joints.Add(_Port.InnerJoint);
					//if(_HvrNode.Parent != null) _HvrNode.Parent.Joints.Add(_Port.OuterJoint);

					iFrame.NewPorts.Clear();

					BeginPorts(iFrame);
					///iFrame.NewPorts.Add(new NGonEdgePort());
				}
				public static void AddJoints    (NGonSchemeFrame iFrame)
				{
					
				}

				//private static bool IsFirst
				public static void AddLines     (NGonSchemeFrame iFrame)
				{
					if(iFrame.HoverNode == null && iFrame.OperandNode == null) return;
					
					if(iFrame.NewLines.Count > 1) throw new WTFE();

					//var _IsFirstJoint = iFrame.NewJoints.Count == 1 && 0 == iFrame.NewLines.Count;
					//var _IsOtherJoint = iFrame.NewJoints.Count == 2 && 1 == iFrame.NewLines.Count;

					if(iFrame.NewJoints.Count == 1 && 0 == iFrame.NewLines.Count) iFrame.OperandNode = iFrame.HoverNode;
					
					var _Parent = iFrame.OperandNode; NGonLine _Line; NGonJoint _Joint1, _Joint2;
					{
						if(iFrame.NewJoints.Count == 2 && 1 == iFrame.NewLines.Count)
						{
							_Parent.Joints.Add(_Joint1 = iFrame.NewJoints[0]);
							_Parent.Lines .Add(_Line   = iFrame.NewLines [0]);
							
							iFrame.NewJoints.RemoveAt(0);
							iFrame.NewLines .Clear();
						}
						if(iFrame.NewJoints.Count == 1 && 0 == iFrame.NewLines.Count)
						{
							_Joint1 = iFrame.NewJoints[0];
							_Joint2 = new NGonJoint{Position = _Parent.Pointer};

							_Line =  new NGonLine{Parent = _Parent, Joint1 = _Joint1, Joint2 = _Joint2, Color = iFrame.CurrentColor};

							_Joint1.Lines.Add(_Line);
							_Joint2.Lines.Add(_Line);

							iFrame.NewJoints.Add(_Joint2);
							iFrame.NewLines .Add(_Line);
						}
						else throw new WTFE();
					}
					
					//else throw new WTFE();
					

					//AddNewJoint : NGonJoint _Joint1, _Joint2; NGonLine _Line; NGonNode _Parent;
					//{
						
					//    if(_IsFirstJoint)
					//    {
					//        _Parent = iFrame.OperandNode = iFrame.HoverNode;
					//        _Joint1 = iFrame.NewJoints[0];
					//        _Joint2 = new NGonJoint{Position = _Parent.Pointer};
					//        //_HvrNode.Joints.Add(_Joint1);

					//        //iFrame.NewJoints.Remove(_Joint1);
					//        iFrame.NewJoints.Add(_Joint2);

					//        _Line =  new NGonLine{Parent = _Parent, Joint1 = _Joint1, Joint2 = _Joint2, Color = iFrame.CurrentColor};

					//        _Joint1.Lines.Add(_Line);
					//        _Joint2.Lines.Add(_Line);

					//        iFrame.NewLines.Add(_Line);

					//        ///iFrame.NewJoints.Clear();

					//        //_HvrNode.Joints.Add(_Joint1);
					//        //iFrame.NewJoints.Add(_Joint1);
					//    }
					//    else if(_IsOtherJoint)
					//    {
					//        _Parent = iFrame.OperandNode;

					//        ///if(iFrame.HoverNode != iFrame.OperandNode) return;


					//        _Parent.Lines.Add(iFrame.NewLines[0]);
					//        _Parent.Joints.Add(iFrame.NewJoints[0]);

							

					//        //_HvrNode.Joints.Add(iFrame.NewJoints[1]);

					//        iFrame.NewLines.Clear();
					//        iFrame.NewJoints.RemoveAt(0);

					//        goto AddNewJoint;
					//    }
					//    else throw new WTFE();
					//}
				}
				//public static void AddLines     (NGonSchemeFrame iFrame)
				//{
				//    if(iFrame.HoverNode == null && iFrame.OperandNode == null) return;
					
				//    if(iFrame.NewLines.Count > 1) throw new WTFE();

				//    AddNewJoint : NGonJoint _Joint1, _Joint2; NGonLine _Line; NGonNode _Parent;
				//    {
				//        var _IsFirstJoint = iFrame.NewJoints.Count == 1 && 0 == iFrame.NewLines.Count;
				//        var _IsOtherJoint = iFrame.NewJoints.Count == 2 && 1 == iFrame.NewLines.Count;

				//        if(_IsFirstJoint)
				//        {
				//            _Parent = iFrame.OperandNode = iFrame.HoverNode;
				//            _Joint1 = iFrame.NewJoints[0];
				//            _Joint2 = new NGonJoint{Position = _Parent.Pointer};
				//            //_HvrNode.Joints.Add(_Joint1);

				//            //iFrame.NewJoints.Remove(_Joint1);
				//            iFrame.NewJoints.Add(_Joint2);

				//            _Line =  new NGonLine{Parent = _Parent, Joint1 = _Joint1, Joint2 = _Joint2, Color = iFrame.CurrentColor};

				//            _Joint1.Lines.Add(_Line);
				//            _Joint2.Lines.Add(_Line);

				//            iFrame.NewLines.Add(_Line);

				//            ///iFrame.NewJoints.Clear();

				//            //_HvrNode.Joints.Add(_Joint1);
				//            //iFrame.NewJoints.Add(_Joint1);
				//        }
				//        else if(_IsOtherJoint)
				//        {
				//            _Parent = iFrame.OperandNode;

				//            ///if(iFrame.HoverNode != iFrame.OperandNode) return;


				//            _Parent.Lines.Add(iFrame.NewLines[0]);
				//            _Parent.Joints.Add(iFrame.NewJoints[0]);

							

				//            //_HvrNode.Joints.Add(iFrame.NewJoints[1]);

				//            iFrame.NewLines.Clear();
				//            iFrame.NewJoints.RemoveAt(0);

				//            goto AddNewJoint;
				//        }
				//        else throw new WTFE();
				//    }
				//}
				//public static void AddLines     (NGonSchemeFrame iFrame)
				//{
				//    if(iFrame.HoverNode == null && iFrame.OperandNode == null) return;
					
				//    if(iFrame.NewLines.Count > 1) throw new WTFE();

				//    AddNewJoint : NGonJoint _Joint1, _Joint2; NGonLine _Line; NGonNode _Parent;
				//    {
				//        var _IsFirstJoint = iFrame.NewJoints.Count == 1 && 0 == iFrame.NewLines.Count;
				//        var _IsOtherJoint = iFrame.NewJoints.Count == 2 && 1 == iFrame.NewLines.Count;

				//        if(_IsFirstJoint)
				//        {
				//            _Parent = iFrame.OperandNode = iFrame.HoverNode;
				//            _Joint1 = iFrame.NewJoints[0];
				//            _Joint2 = new NGonJoint{Position = _Parent.Pointer};
				//            //_HvrNode.Joints.Add(_Joint1);

				//            //iFrame.NewJoints.Remove(_Joint1);
				//            iFrame.NewJoints.Add(_Joint2);

				//            _Line =  new NGonLine{Parent = _Parent, Joint1 = _Joint1, Joint2 = _Joint2, Color = iFrame.CurrentColor};

				//            _Joint1.Lines.Add(_Line);
				//            _Joint2.Lines.Add(_Line);

				//            iFrame.NewLines.Add(_Line);

				//            ///iFrame.NewJoints.Clear();

				//            //_HvrNode.Joints.Add(_Joint1);
				//            //iFrame.NewJoints.Add(_Joint1);
				//        }
				//        else if(_IsOtherJoint)
				//        {
				//            _Parent = iFrame.OperandNode;

				//            ///if(iFrame.HoverNode != iFrame.OperandNode) return;


				//            iFrame.HoverNode.Lines.Add(iFrame.NewLines[0]);
				//            iFrame.HoverNode.Joints.Add(iFrame.NewJoints[0]);

							

				//            //_HvrNode.Joints.Add(iFrame.NewJoints[1]);

				//            iFrame.NewLines.Clear();
				//            iFrame.NewJoints.RemoveAt(0);

				//            goto AddNewJoint;
				//        }
				//        else throw new WTFE();
				//    }
				//}
			}
			public class Processing
			{
				public struct IO
				{
					public static void BatchReset(NGonSchemeFrame iFrame)
					{
						iFrame.Inputs.Clear();
						iFrame.Outputs.Clear();
						iFrame.Scripts.Clear();
					}
					public static void BatchRegister(NGonSchemeFrame iFrame)
					{
						RegisterNode(iFrame, iFrame.Scheme, true);
					}
					public static void RegisterNode(NGonSchemeFrame iFrame, NGonNode iNode, bool iDoRecursive)
					{
						///if(iNode.Type != "")
						if(!String.IsNullOrEmpty(iNode.Type))
						{
							if(iNode.Type.StartsWith("I["))
							{
								iFrame.Inputs.Add(iNode);
							}
							else if(iNode.Type.StartsWith("O["))
							{
								iFrame.Outputs.Add(iNode);
							}
							else
							{
								iFrame.Scripts.Add(iNode);
							}
						}
						
						if(iDoRecursive) foreach(var cChild in iNode.Children)
						{
							RegisterNode(iFrame, cChild, iDoRecursive);
						}
					}
				}
				

				//public static void ConnectAll   (NGonSchemeFrame iFrame)
				//{
				//    //Select
				//    //iFrame.Scheme.

				//    //iFrame.Scheme
				//    //this.MergeJoints();
				//    //this.MergeJointGraphs();
				//    //this.LinkPortsWithGraphs();
				//}
				
			}
		}
	}
}