using System;
using System.Collections.Generic;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public partial class ModelViewer : GLFrame
	{
		public partial struct Routines
		{
			public class Rendering
			{
				public static Vector3d[] CubeVertices = new Vector3d[]
				{
					new Vector3d(0,0,+1.5),

					new Vector3d(-1,-1,-1),
					new Vector3d(+1,-1,-1),
					new Vector3d(+1,+1,-1),
					new Vector3d(-1,+1,-1),

					new Vector3d(-1,-1,+1),
					new Vector3d(+1,-1,+1),
					new Vector3d(+1,+1,+1),
					new Vector3d(-1,+1,+1),
				};
				public static void SetProjectionMatrix (ModelViewer iFrame) 
				{
					var _CurrView = iFrame.Views.Current;
					_CurrView.Update();

					GL.MatrixMode(MatrixMode.Projection); GL.LoadMatrix(ref _CurrView.ProjMatrix);
					GL.MatrixMode(MatrixMode.Modelview);  GL.LoadMatrix(ref _CurrView.ViewMatrix);



					//if(_View.Type == ViewType.Perspective)
					//{
					
					//}
					//else
					//{
					//    GL.LoadMatrix(ref _ProjMat);
					//}

					//var _Viewpoint = iFrame.Viewpoint.CurrentState;
					//var _ProjMat = iFrame.IsPerspectiveMode ? _Viewpoint.PerspProjMatrix : _Viewpoint.OrthoProjMatrix;
					
					//GL.LoadMatrix(ref _ProjMat);
					
					//if(iFrame.IsPerspectiveMode)
					//{
					//    var _LookAtMat = _Viewpoint.PerspLookAtMatrix;
					//    GL.MultMatrix(ref _LookAtMat);
					//    ///GL.MultMatrix(ref _LookAtMat);
					//}

					//GL.MatrixMode(MatrixMode.Modelview);
					//GL.LoadIdentity();

				}
				
				public static void Draw(ModelViewer iFrame)
				{
					///EnableCap.Lin.Lin

					
					///G.Debug.Clear();
					///G.Debug.Message("Pointer: " + iFrame.Views.Current.Pointer.Target);

					DrawModel(iFrame);

					
					SetProjectionMatrix(iFrame); ///~~ once again, due to the projections experiments;
					DrawPivotPoint(iFrame);
					DrawPointer(iFrame);
				}
				//public static void Se

				public static void DrawGeometry(ModelViewer iFrame)
				{
					var _VertexData      = Geometry.Teapot.VertexData;
					var _NormalData      = Geometry.Teapot.NormalData;
					var _VertexIndexData = Geometry.Teapot.VertexIndexData;
					var _NormalIndexData = Geometry.Teapot.NormalIndexData;

					var _TriangleCount = _VertexIndexData.GetLength(0);

					var _PerspView = iFrame.Views.Current as PerspectiveView;
					var _OrthoView = iFrame.Views.Current as OrthographicView;

					var _TransMat = Matrix4d.CreateTranslation(0,-20,0) * Matrix4d.CreateRotationX(90 * MathEx.DTR) * Matrix4d.Scale(0.01);
					var _TransMatInverted = _TransMat.Inverted();
					///var _EyePos = Vector3d.Transform(_PerspView.Eye, _TransMatInverted);
					///var _PoiPos = Vector3d.Transform(_PerspView.Pointer.Target, _TransMatInverted);
						
						///_PerspView.Eye - _PerspView.Pointer.Target;

					GL.Disable(EnableCap.LineSmooth);

					GL.Enable(EnableCap.RescaleNormal);
					
					GL.PushMatrix();
					GL.MultMatrix(ref _TransMat);
					//GL.Rotate(90, Vector3d.UnitX);
					//GL.Scale(0.01,0.01,0.01);
					//GL.Translate(0,-20,0);
					{
						GL.Enable(EnableCap.CullFace);

						//GL.Disable( EnableCap.LineSmooth);
						
						
						GL.Color4(Color.FromArgb(255, 0,100,0));
						GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
						///GL.PolygonOffset(0,0);

						
						//GL.Begin(PrimitiveType.Quads);
						//{
						//   ///GL.Color4(1,1,1,1f);
						//   //GL.TexCoord2(0f,0f); GL.Vertex3(+1,+1,0);
						//   //GL.TexCoord2(0f,1f); GL.Vertex3(+1,-1,0);
						//   //GL.TexCoord2(1f,1f); GL.Vertex3(-1,-1,0);
						//   //GL.TexCoord2(1f,0f); GL.Vertex3(-1,+1,0);

						//   GL.Normal3( 0.5f, 0.5f, 0.5f); GL.Vertex3(+10,+10,0);
						//   GL.Normal3( 0.5f, 0.5f, 0.5f); GL.Vertex3(+10,-10,0);
						//   GL.Normal3(-0.5f,-0.5f,-0.5f); GL.Vertex3(-10,-10,20);
						//   GL.Normal3(-0.5f,-0.5f,-0.5f); GL.Vertex3(-10,+10,20);
						//}
						//GL.End();


						for(var cPass = 0; cPass <= 1; cPass++)
						{
							GL.Begin(PrimitiveType.Triangles);
							{
								for(var cTi = 0; cTi < _TriangleCount; cTi++)
								{
									GL.Normal3(_NormalData[_NormalIndexData[cTi,0]]); GL.Vertex3(_VertexData[_VertexIndexData[cTi,0]]);
									GL.Normal3(_NormalData[_NormalIndexData[cTi,1]]); GL.Vertex3(_VertexData[_VertexIndexData[cTi,1]]);
									GL.Normal3(_NormalData[_NormalIndexData[cTi,2]]); GL.Vertex3(_VertexData[_VertexIndexData[cTi,2]]);
								}
							}
							GL.End();

							GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
							GL.Enable(EnableCap.PolygonOffsetLine);
							GL.PolygonOffset(-1,-1);
							GL.Disable(EnableCap.Lighting);
							GL.Color4(Color.FromArgb(255, iFrame.Palette.GlareColor));
						}
						GL.Disable(EnableCap.PolygonOffsetLine);


						//GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
						//GL.Color4(iFrame.Palette.Colors[7]);
						//GL.Begin(PrimitiveType.Lines);
						//{
						//    for(var cTi = 0; cTi < _TriangleCount; cTi++)
						//    {
						//        GL.Normal3(_NormalData[_NormalIndexData[cTi,0]]); GL.Vertex3(_VertexData[_VertexIndexData[cTi,0]]);
						//        GL.Normal3(_NormalData[_NormalIndexData[cTi,1]]); GL.Vertex3(_VertexData[_VertexIndexData[cTi,1]]);

						//        GL.Normal3(_NormalData[_NormalIndexData[cTi,1]]); GL.Vertex3(_VertexData[_VertexIndexData[cTi,1]]);
						//        GL.Normal3(_NormalData[_NormalIndexData[cTi,2]]); GL.Vertex3(_VertexData[_VertexIndexData[cTi,2]]);

						//        GL.Normal3(_NormalData[_NormalIndexData[cTi,2]]); GL.Vertex3(_VertexData[_VertexIndexData[cTi,2]]);
						//        GL.Normal3(_NormalData[_NormalIndexData[cTi,0]]); GL.Vertex3(_VertexData[_VertexIndexData[cTi,0]]);
						//    }
						//}
						//GL.End();

						
						
						

						GL.Disable(EnableCap.CullFace);
					}
					
					GL.PopMatrix();
					GL.Disable(EnableCap.RescaleNormal);

					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);




					///if(false)
					{
						var _Pointer = new Vector2d((double)iFrame.State.Mouse.AX / iFrame.Width * 2 - 1.0, 1.0 - ((double)iFrame.State.Mouse.AY / iFrame.Height * 2));

						Matrix4d _ProjMat, _ViewMat;
						GL.GetDouble(GetPName.ProjectionMatrix, out _ProjMat);
						GL.GetDouble(GetPName.ModelviewMatrix,  out _ViewMat);

						var _MvpMat = _TransMat * _ViewMat * _ProjMat;
						
						GL.MatrixMode(MatrixMode.Projection);
						GL.LoadIdentity();
						GL.MatrixMode(MatrixMode.Modelview);
						GL.LoadIdentity();

						GL.Begin(PrimitiveType.Triangles);
						{
							for(var cTi = 0; cTi < _TriangleCount; cTi++)
							{
								var cIsHover = true;
								var _PrevDiffAngle = Double.MinValue;

								for(var cVi = 0; cVi <= 3; cVi++)
								{
									var cScrPoint = Project(_VertexData[_VertexIndexData[cTi,cVi % 3]], _MvpMat);
									var cDiff     = cScrPoint.Xy - _Pointer;

									var cDiffAngle = Math.Atan2(cDiff.Y, cDiff.X);

									if(MathEx.DeltaAngle(_PrevDiffAngle, cDiffAngle) < 0)
									{
										cIsHover = false;
									}
									_PrevDiffAngle = cDiffAngle;
								}


								
								if(cIsHover) for(var cVi = 0; cVi < 3; cVi++)
								{
									var cScrPoint = Project(_VertexData[_VertexIndexData[cTi,cVi]], _MvpMat);
									
									GL.Color4(iFrame.Palette.Colors[cIsHover ? 5 : 4]);
									GL.Vertex2(cScrPoint.Xy); 
								}
							}
						}
						GL.End();

						GL.PointSize(10);
						GL.Begin(PrimitiveType.Points);
						{
							foreach(var cVertex in Geometry.Teapot.VertexData)
							{
								var cScrPoint   = Project(cVertex, _MvpMat);
								var cScrPointXY = cScrPoint.Xy;

								var cPointerDist = (cScrPointXY - _Pointer).Length;
								var cIsHover = cPointerDist < 0.02; ///if(!cIsHover) continue;

								///GL.Color4(iFrame.Palette.Colors[cIsHover ? 3 : 10]);
								GL.Color4(Color.FromArgb((int)((1.0 - MathEx.ClampZP(Math.Pow(0.2+cScrPoint.Z, 2))) * 255), iFrame.Palette.Colors[cIsHover ? 3 : 10]));
								GL.Vertex2(cScrPointXY); 
							}
						}
						GL.End();
						
					}
				}
				
				public static Vector3d Project(Vector3d iPoint, Matrix4d iMatrix)
				{
					var _PointTrans = Vector4d.Transform(new Vector4d(iPoint,1), iMatrix);

					if(_PointTrans.W <= 0) return Vector3d.Zero;

					var oScrPoint  = _PointTrans.Xyz / _PointTrans.W;

					return oScrPoint;
				}
				public static void DrawModel              (ModelViewer iFrame)
				{
					if(iFrame.GeometryListID == -1)
					{
						iFrame.GeometryListID = GL.GenLists(1);

						GL.NewList(iFrame.GeometryListID, ListMode.Compile);
						DrawGeometry(iFrame);
						GL.EndList();
					}

					//GL.Render


					var mat_specular = new OpenTK.Graphics.Color4(1.0f, 1.0f, 1.0f, 1.0f);
					//var mat_diffuse  = new OpenTK.Graphics.Color4(0.5f, 1.0f, 0.0f, 1.0f);
					var mat_shininess = 10.0f;
					///var light_position = new Vector4(25.0f, 0.0f, 5.0f, 1.0f);

					var _Ticks = (double)(DateTime.Now.Ticks / 1e6) * 0.001;
					///var light_position = new Vector4(50 + (float)(Math.Sin(_Ticks) * 100f), -50 + (float)(Math.Cos(_Ticks) * 25f), 100.0f, 1.0f);
					var light_position = new Vector4((float)(Math.Sin(_Ticks) * 100f), (float)(Math.Cos(_Ticks) * 100f), 50.0f, 1.0f);


					//GL.ClearColor(0,0,0,0);

					
					///GL.ShadeModel(ShadingModel.Flat);
					GL.ShadeModel(ShadingModel.Smooth);

					//GL.Material(MaterialFace.Front, MaterialParameter.Specular, new Vector4(1));
					GL.Material(MaterialFace.Front, MaterialParameter.Specular, mat_specular);
					/////GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, mat_diffuse);
					GL.Material(MaterialFace.Front, MaterialParameter.Shininess, mat_shininess);

					
					GL.Light(LightName.Light0, LightParameter.Position, light_position);
					///GL.Light(LightName.Light0, LightParameter.QuadraticAttenuation, 0.001f);
					///GL.Light(LightName.Light0, LightParameter.Ambient, new Vector4(0,0,0,0));
					//GL.Light(LightName.Light0, LightParameter..Ambient, new Vector4(1,0,0,1));

					//GL.Light(LightName.Light0, LightParameter.LinearAttenuation, 0.1f);
					
					GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);

					//GL.ClearDepth(0);
					GL.Clear(ClearBufferMask.DepthBufferBit);

					

					//if(iFrame.IsTexturingEnabled)
					//{
					//    ///iFrame.Grass.Bind();
					//    //GL.BindTexture(iFrame.TexIDs[0]);
					//    GL.Enable(EnableCap.Texture2D);
					//}


					if(iFrame.IsLightingEnabled)
					{
						GL.Enable(EnableCap.Lighting);

						
						GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Diffuse);
						GL.Enable(EnableCap.ColorMaterial);
					}

					GL.Enable(EnableCap.Light0);
					GL.Enable(EnableCap.DepthTest);
					
					///GL.CallList(iFrame.GeometryListID);
					DrawGeometry(iFrame);


					
					




					GL.Disable(EnableCap.Light0);
					GL.Disable(EnableCap.Lighting);
					GL.Disable(EnableCap.DepthTest);

					//GL.Disable(EnableCap.Fog);

				}
				public static void DrawPivotPoint         (ModelViewer iFrame) 
				{
					GL.LineWidth(3);
					GL.Begin(PrimitiveType.Lines);
					{
						GL.Color4(iFrame.Palette.Colors[3]);
						GL.Vertex3(0,0,0); GL.Vertex3(1,0,0);

						GL.Color4(iFrame.Palette.Colors[7]);
						GL.Vertex3(0,0,0); GL.Vertex3(0,1,0);

						GL.Color4(iFrame.Palette.Colors[10]);
						GL.Vertex3(0,0,0); GL.Vertex3(0,0,1);
					}
					GL.End();
				}
				public static void DrawPointer         (ModelViewer iFrame) 
				{
					var _P    = iFrame.Views.Current.Pointer.Target;
					var _Size = 0.1;
					
					GL.PointSize(5f);
					GL.LineWidth(1f);
					GL.Color4(iFrame.Palette.Colors[4]);

					GL.Begin(PrimitiveType.Lines);
					{
						GL.Vertex3(_P.X - _Size,_P.Y,_P.Z);
						GL.Vertex3(_P.X + _Size,_P.Y,_P.Z);

						GL.Vertex3(_P.X,_P.Y - _Size,_P.Z);
						GL.Vertex3(_P.X,_P.Y + _Size,_P.Z);
						
						GL.Vertex3(_P.X,_P.Y,_P.Z - _Size);
						GL.Vertex3(_P.X,_P.Y,_P.Z + _Size);


						//GL.Vertex2(-10.0, _Y);
						//GL.Vertex2(+10.0, _Y);

						//GL.Vertex2(_X, -10.0);
						//GL.Vertex2(_X, +10.0);

						//GL.Vertex2(_X, -10.0);
						//GL.Vertex2(_X, +10.0);
					}
					GL.End();
				}

				public static void EnableEnvironment(bool iDo1, bool iDo2, bool iDoFog)
				{

					if(iDo1)
					{
						var _Ticks = (double)(DateTime.Now.Ticks / 1e6) * 0.1;
						var light_position = new Vector4(5 + (float)(Math.Sin(_Ticks) * 2.5f), 5 + (float)(Math.Cos(_Ticks) * 2.5f), 1.0f, 1.0f);
						//var light_position = new Vector4(0f,0f,(float)Math.Sin(_Ticks) * 100f, 1.0f);

						var mat_specular = new OpenTK.Graphics.Color4(0.0f, 1.0f, 1.0f, 1.0f);
						var mat_diffuse  = new OpenTK.Graphics.Color4(0.5f, 1.0f, 0.0f, 1.0f);
						var mat_shininess = 100.0f;

						GL.ShadeModel(ShadingModel.Smooth);

						GL.Material(MaterialFace.Front, MaterialParameter.Specular, mat_specular);
						GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, mat_diffuse);
						GL.Material(MaterialFace.Front, MaterialParameter.Shininess, mat_shininess);

						
						GL.Light(LightName.Light0, LightParameter.Position, light_position);
						
						GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
						GL.Clear(ClearBufferMask.DepthBufferBit);

						GL.Enable(EnableCap.Lighting);
						GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Diffuse);
						GL.Enable(EnableCap.ColorMaterial);

						
					GL.Enable(EnableCap.Light0);
					GL.Enable(EnableCap.DepthTest);
					}
					if(iDoFog)
					{
						/**
							glEnable(GL_FOG);                       // Включает туман (GL_FOG)
							glFogi(GL_FOG_MODE, fogMode[fogfilter]);// Выбираем тип тумана
							glFogfv(GL_FOG_COLOR, fogColor);        // Устанавливаем цвет тумана
							glFogf(GL_FOG_DENSITY, 0.35f);          // Насколько густым будет туман
							glHint(GL_FOG_HINT, GL_DONT_CARE);      // Вспомогательная установка тумана
							glFogf(GL_FOG_START, 1.0f);             // Глубина, с которой начинается туман
							glFogf(GL_FOG_END, 5.0f);               // Глубина, где туман заканчивается.
						*/

						//var _FogColor = new Vector4(1,1,1,1);
						var _FogColor = new float[]{1,1,1,1};

						///GL.ClearColor(_FogColor[0],_FogColor[1],_FogColor[2],_FogColor[3]);
						GL.Clear(ClearBufferMask.ColorBufferBit);

						GL.Enable(EnableCap.Fog);
						GL.Fog(FogParameter.FogMode, (int)FogMode.Exp);
						GL.Fog(FogParameter.FogColor, _FogColor);
						GL.Fog(FogParameter.FogDensity, 2.0f);///0.2f); 
						GL.Hint(HintTarget.FogHint, HintMode.Fastest);
						GL.Fog(FogParameter.FogStart, 0.0f);
						GL.Fog(FogParameter.FogEnd, 10f);
					}
					
				}
				public static void DisableEnvironment()
				{
					GL.Disable(EnableCap.Light0);
					GL.Disable(EnableCap.Lighting);
					GL.Disable(EnableCap.DepthTest);

					GL.Disable(EnableCap.Fog);
				}
			}
			public class Calculations
			{
				public static double GetPointToLineDistance(Vector2d iPoint, Vector2d iLineP1, Vector2d iLineP2, bool iIsSection)
				{
					var _PL1 = iPoint - iLineP1;
					var _PL2 = iPoint - iLineP2;
					var _LL  = iLineP1 - iLineP2;

					var _PLCrsP = Math.Abs((_PL1.X * _PL2.Y) - (_PL1.Y * _PL2.X));

					var oDist = _PLCrsP / _LL.Length;

					if(iIsSection)
					{
						///~~ TODO: rewrite this dirty shit;
						var _PL1Len = _PL1.Length;
						var _PL2Len = _PL2.Length;

						var _MinPointDist = Math.Min(_PL1Len, _PL2Len);
						var _MaxPointDist = Math.Max(_PL1Len, _PL2Len);

						if(_MaxPointDist > _LL.Length)
						{
							oDist = _MinPointDist;
						}
					}

					return oDist;
				}
				public static double GetPointToLineDistance(Vector3d iPoint, Vector3d iLineP1, Vector3d iLineP2, bool iIsSection)
				{
					var _PL1 = iPoint - iLineP1;
					var _PL2 = iPoint - iLineP2;
					var _LL  = iLineP1 - iLineP2;

					var _PLCrsP = Vector3d.Cross(_PL1,_PL2);

					var oDist = _PLCrsP.Length / _LL.Length;


					if(iIsSection)
					{
						///~~ TODO: rewrite this dirty shit;
						var _PL1Len = _PL1.Length;
						var _PL2Len = _PL2.Length;

						var _MinPointDist = Math.Min(_PL1Len, _PL2Len);
						var _MaxPointDist = Math.Max(_PL1Len, _PL2Len);

						if(_MaxPointDist > _LL.Length)
						{
							oDist = _MinPointDist;
						}
					}


					return oDist;
				}
				public static double GetLineToLineDistance(Vector3d ixP1, Vector3d ixP2, Vector3d iyP1, Vector3d iyP2)
				{
					///http://mathworld.wolfram.com/Line-LineDistance.html

					var _xPL = ixP1 - ixP2;
					var _yPL = iyP1 - iyP2;
					var _zPL = ixP1 - iyP1;

					var _PLCrsP = Vector3d.Cross(_xPL, _yPL);

					var oDist = Math.Abs(Vector3d.Dot(_zPL, _PLCrsP)) / Math.Abs(_PLCrsP.Length);


					//var _PL1 = iPoint - iLineP1;
					//var _PL2 = iPoint - iLineP2;
					//var _LL  = iLineP1 - iLineP2;

					//var _PLCrsP = Vector3d.Cross(_PL1,_PL2);/// Math.Abs((_PL1.X * _PL2.Y) - (_PL1.Y * _PL2.X));

					//var oDist = _PLCrsP.Length / _LL.Length;

					return oDist;
				}
			}
		}
	}
}