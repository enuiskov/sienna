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
	public partial class TerrainEditorFrame : ModelViewer
	{
		public struct C4fN3fV3fVertex //8b + 4b + 12b = 24b per vertex = 96b per quad = (96 * 120)b per row, (11500 * 50)b per screen = 576000b
		{
			public static C4fN3fV3fVertex Empty = new C4fN3fV3fVertex();
			
			public Vector4 Color;
			public Vector3 Normal;
			public Vector3 Position;

			public C4fN3fV3fVertex(Vector4 iColor, Vector3 iNormal, Vector3 iPosition)
			{
				this.Color     = iColor;
				this.Normal    = iNormal;
				this.Position  = iPosition;
			}
		}
		public struct T2fC4fN3fV3fVertex //8b + 4b + 12b = 24b per vertex = 96b per quad = (96 * 120)b per row, (11500 * 50)b per screen = 576000b
		{
			public static T2fC4ubV3fVertex Empty = new T2fC4ubV3fVertex();
			
			public Vector2 TexCoords;
			public Vector4 Color;
			public Vector3 Normal;
			public Vector3 Position;

			public T2fC4fN3fV3fVertex(Vector2 iTexCoords, Vector4 iColor, Vector3 iNormal, Vector3 iPosition)
			{
				this.TexCoords = iTexCoords;
				this.Color     = iColor;
				this.Normal    = iNormal;
				this.Position  = iPosition;
			}
		}
	
		public new struct Routines
		{
			public class Generation
			{
				
				public static C4fN3fV3fVertex[,] GenerateVertexGrid(int iWidth, int iHeight, float iElevation, int iRandomSeed, int iLevel) 
				{
					var _Rng      = new Random(iRandomSeed);///(11);//8
					//var _CellSize = 1.0f;
					var _Offs     = Vector2.Zero;/// new Vector2(-(iWidth * _CellSize / 2.0f), -(iLength * _CellSize / 2.0f));
					

					var _HeightMap = new float[iHeight, iWidth];
					{
						for(var cRi = 0; cRi < iHeight; cRi++)
						for(var cCi = 0; cCi < iWidth; cCi++)
						{
							///_HeightMap[cRi,cCi] = 6000000;
						}

						for(var cLevel = 2; cLevel < iLevel; cLevel++)
						{
							var cStepCount = Math.Pow(2, cLevel) * 1;
							var cFalloff   = Math.Max(iWidth,iHeight) / (int)(Math.Pow(1.5, cLevel));
							var cElevRange = cFalloff * 0.5;

							for(var cStep = 0; cStep < cStepCount; cStep++)
							{
								var cElevD = cElevRange * (_Rng.NextDouble() - 0.5) * iElevation;
								var cCenRi = _Rng.Next(iHeight);
								var cCenCi = _Rng.Next(iWidth);


								for(var cRo = -cFalloff; cRo < cFalloff; cRo++)
								for(var cCo = -cFalloff; cCo < cFalloff; cCo++)
								{
									var cRi = cCenRi + cRo; if(cRi < 0 || cRi > iHeight - 1) continue;
									var cCi = cCenCi + cCo; if(cCi < 0 || cCi > iWidth  - 1) continue;

									var cRowDistF = (float)Math.Abs(cRo) / cFalloff;
									var cColDistF = (float)Math.Abs(cCo) / cFalloff;

									var cFoDistF = Math.Min(1.0, Math.Sqrt((cRowDistF * cRowDistF) + (cColDistF * cColDistF)));
									///var cFoElevD = cElevD * (Math.Cos(cFoDistF * Math.PI) / 2 + 0.5);
									var cFoElevD = cElevD * (1 - cFoDistF);/// * (Math.Cos(cFoDistF * Math.PI) / 2 + 0.5);

									_HeightMap[cRi,cCi] += (float)cFoElevD;
								}
							}
						}

						if(true)
						{
							var _H = _HeightMap.GetLength(0);
							var _W = _HeightMap.GetLength(1);

							for(var cRi = 0; cRi < _H; cRi ++ )
							{
								for(var cCi = 0; cCi < _W; cCi ++ )
								{
									_HeightMap[cRi,cCi] = Math.Max(_HeightMap[cRi,cCi], -3);
								}
							}
						}
					}

					var _SeabedColor = new Vector4(0.1f, 0.2f, 0.3f, 1f);
					var _WaterColor  = new Vector4(  0f, 0.5f,   1f, 1f);
					var _SandColor   = new Vector4(1.0f, 1.0f, 0.6f, 1f);
					var _GrassColor  = new Vector4(0.2f, 0.6f,   0f, 1f);
					var _RockColor   = new Vector4(0.5f, 0.4f, 0.3f, 1f);
					var _SnowColor   = new Vector4(  1f,   1f,   1f, 1f);
					

					var oVertices = new C4fN3fV3fVertex[iHeight, iWidth];
					{
						for(var cRi = 0; cRi < iHeight; cRi++)
						{
							for(var cCi = 0; cCi < iWidth; cCi++)
							{
								//var cH = (float)_Rng.NextDouble();
								var cX = _Offs.X + cCi;
								var cY = _Offs.Y + cRi;

								//var cDist = Math.Sqrt((cX * cX) + (cY * cY));

								var cZ = _HeightMap[cRi,cCi];// _Rng.NextDouble();


								//var cZ = Math.Sin(cDist * 1.0) * Math.Pow(cDist, 3) * 0.000001;
								//var cZ = Math.Sin(cDist * 1.0) * 1f;
									///cZ = MathEx.Clamp(cZ, 0, 10);

								///var cColor = new Vector4(0,1,1,1);
								var cColor = _SeabedColor;
								{
									//cColor = Vector4.Lerp(cColor, _SandColor,   MathEx.ClampZP((cZ + 1)  * 1f));
									///cColor = Vector4.Lerp(cColor, _SandColor,   MathEx.ClampZP((cZ + 3)  * 0.5f));
									cColor = Vector4.Lerp(cColor, _SandColor,   MathEx.ClampZP((cZ + 3)  * 1f));

									cColor = Vector4.Lerp(cColor, _GrassColor, MathEx.ClampZP((cZ - 1)  * 0.5f));
									cColor = Vector4.Lerp(cColor, _RockColor,  MathEx.ClampZP((cZ - 10) * 0.2f));
									cColor = Vector4.Lerp(cColor, _SnowColor,  MathEx.ClampZP((cZ - 20) * 0.3f));
								}
								var cVertex = new C4fN3fV3fVertex(cColor, Vector3.UnitZ, new Vector3(cX, -cY, (float)cZ));

								oVertices[cRi,cCi] = cVertex;
							}
						}

						var _NeiWalkOrder = new int[,]{{-1,-1}, {-1, 0}, { 0,+1}, {+1,+1}, {+1, 0}, { 0,-1}, {-1,-1}};
						for(var cRi = 0; cRi < iHeight; cRi ++)
						{
							for(var cCi = 0; cCi < iWidth; cCi ++)
							{
								//if(cRi == 5 && cCi == 4)
								//{
									
								//}
								//else continue;

								var cVertex    = oVertices[cRi,cCi];
								var cNormalSum = Vector3.Zero;
								{
									for(var cNeighTi = 0; cNeighTi < 6; cNeighTi ++)
									{
										var caNeiVi  = cNeighTi;
										var caNeiVRi = cRi + _NeiWalkOrder[caNeiVi,0];
										var caNeiVCi = cCi + _NeiWalkOrder[caNeiVi,1];
										if(caNeiVRi < 0 || caNeiVRi > iHeight - 1) continue;
										if(caNeiVCi < 0 || caNeiVCi > iWidth  - 1) continue;

										var cbNeiVi  = cNeighTi + 1;
										var cbNeiVRi = cRi + _NeiWalkOrder[cbNeiVi,0];
										var cbNeiVCi = cCi + _NeiWalkOrder[cbNeiVi,1];
										if(cbNeiVRi < 0 || cbNeiVRi > iHeight - 1) continue;
										if(cbNeiVCi < 0 || cbNeiVCi > iWidth  - 1) continue;


										var caNeiV = oVertices[caNeiVRi, caNeiVCi];
										var cbNeiV = oVertices[cbNeiVRi, cbNeiVCi];

										//var cVertex = oVertices[c

										var caDiff = cVertex.Position - caNeiV.Position;
										var cbDiff = cVertex.Position - cbNeiV.Position;

										var cNormal = Vector3.Cross(cbDiff, caDiff).Normalized();
										cNormalSum += cNormal;


										//caNeiV.Color = new Vector4(1,0,0,1);
										//oVertices[caNeiVRi, caNeiVCi] = caNeiV;
									}
								}
								cVertex.Normal = cNormalSum.Normalized();

								oVertices[cRi,cCi] = cVertex;
							}
						}
					}
					return oVertices;
				}
				
				public static void Terrain           (TerrainEditorFrame iFrame, bool iDoRandom) 
				{
					//var _Triangles = GenerateTriangleGrid(10,10,10);
					///var _Vertices   = GenerateVertexGrid(300,300,1f, iDoRandom ? new Random().Next(10000) : iFrame.Seed, iFrame.DetailLevel);
                    var _Vertices = GenerateVertexGrid(300, 300, 1f, iDoRandom ? new Random().Next(10000) : iFrame.Seed, iFrame.DetailLevel);

					var _GridWidth  = _Vertices.GetLength(1);
					var _GridHeight = _Vertices.GetLength(0);
					
					if(iFrame.DisplayLists != null)
					{
						foreach(var cListId in iFrame.DisplayLists)
						{
							GL.DeleteLists(cListId, 1);
						}
					}



					iFrame.DisplayLists = new int[1];
					var _ListID = iFrame.DisplayLists[0] = GL.GenLists(1);

					//var _Grid     = new Vector3d(100,100,10);
					//GL.Enable(EnableCap.Lighting);
					
					
					GL.NewList(_ListID, ListMode.Compile);

					//GL.EndList();
					//return;

					GL.Enable(EnableCap.CullFace);

					if(false)
					{
						GL.PointSize(1);
						GL.Begin(PrimitiveType.Points);
						{
							foreach(var cV in _Vertices)
							{
								GL.Normal3(cV.Normal);
								///GL.Color4(cV.Color);
								GL.Color4(1f,1f,1f,1f);
								GL.Vertex3(cV.Position);
							}
						}
						GL.End();
					}
					
					if(true)
					{
						//GL.Enable(EnableCap.Texture2D);
						GL.Begin(PrimitiveType.Triangles);
						{
							var _VertWalkOrder = new int[,]
							{
								{-1,0}, {0,-1}, { 0, 0},
								{0,-1}, {-1,0}, {-1,-1}

								//{-1,0}, {0,-1}, {-1,-1}
							};

							//var _TexScale = 1f;

							for(var cRi = 1; cRi < _GridHeight; cRi ++)
							for(var cCi = 1; cCi < _GridWidth;  cCi ++)
							{
								for(var cI = 0; cI < 6; cI ++)
								{
									///var cTexCoordX = _VertWalkOrder[cI,1];
									///var cTexCoordY = _VertWalkOrder[cI,0];

									//var cTexCoordX = ((float)cCi / _GridWidth)  + (_VertWalkOrder[cI,1] * 0.01);
									//var cTexCoordY = ((float)cRi / _GridHeight) + (_VertWalkOrder[cI,0] * 0.01);

									///  0.5
									
									//var cTexCoordX = ((float)cCi / _GridWidth * _TexScale)  + (_VertWalkOrder[cI,1] / _TexScale);
									//var cTexCoordY = ((float)cRi / _GridHeight * _TexScale) + (_VertWalkOrder[cI,0] / _TexScale);
									var cTexCoordX = (_VertWalkOrder[cI,1] / iFrame.TexScale) + ((float)cCi / iFrame.TexScale);
									var cTexCoordY = (_VertWalkOrder[cI,0] / iFrame.TexScale) + ((float)cRi / iFrame.TexScale);

									//cRi + 
									var cVertex = _Vertices[cRi + _VertWalkOrder[cI,0], cCi + _VertWalkOrder[cI,1]];

									GL.TexCoord2 (cTexCoordX, cTexCoordY);
									GL.Normal3   (cVertex.Normal);
									GL.Color4    (cVertex.Color);
									GL.Vertex3   (cVertex.Position);
								}
							}
						}
						GL.End();
						GL.Disable(EnableCap.Texture2D);
					}

					if(false)
					{
						GL.Disable(EnableCap.Lighting);
						GL.Translate(0,0,0.02);
						GL.Color4(0,0.5,0,1);
						GL.Begin(PrimitiveType.Lines);
						{
							for(int _RRc = _GridHeight, cRi = 1; cRi < _RRc; cRi ++)
							{
								for(int _CCc = _GridWidth, cCi = 1; cCi < _CCc; cCi ++)
								{
									GL.Vertex3(_Vertices[cCi,cRi].Position); GL.Vertex3(_Vertices[cCi - 1, cRi].Position);
									GL.Vertex3(_Vertices[cCi,cRi].Position); GL.Vertex3(_Vertices[cCi,     cRi - 1].Position);
									GL.Vertex3(_Vertices[cCi,cRi].Position); GL.Vertex3(_Vertices[cCi - 1, cRi - 1].Position);
								}
							}
						}
						GL.End();
					}
					//GL.Enable(EnableCap.DepthTest);

					if(false)
					{
						GL.Color4(1.0,1.0,0.0,1.0);
						GL.Begin(PrimitiveType.Lines);
						{
							foreach(var cV in _Vertices)
							{
								GL.Vertex3(cV.Position);
								GL.Vertex3(cV.Position + (cV.Normal * 0.5f));
							}
						}
						GL.End();
					}
					GL.Disable(EnableCap.CullFace);

					GL.EndList();
				}
			}
			public class Rendering
			{
				public static void Draw                (TerrainEditorFrame iFrame)
				{
					
					ModelViewer.Routines.Rendering.SetProjectionMatrix(iFrame);

					///GL.Translate(0,0,6000000d);
					///ModelViewer.Routines.Rendering.DrawUnitSpace(iFrame);
					///ModelViewer.Routines.Rendering.DrawPropeller(iFrame);

					var mat_specular = new OpenTK.Graphics.Color4(1.0f, 1.0f, 1.0f, 1.0f);
					//var mat_diffuse  = new OpenTK.Graphics.Color4(0.5f, 1.0f, 0.0f, 1.0f);
					var mat_shininess = 100.0f;
					///var light_position = new Vector4(25.0f, 0.0f, 5.0f, 1.0f);

					var _Ticks = (double)(DateTime.Now.Ticks / 1e6) * 0.01;
					var light_position = new Vector4(50 + (float)(Math.Sin(_Ticks) * 100f), -50 + (float)(Math.Cos(_Ticks) * 100f), 100.0f, 1.0f);


					//GL.ClearColor(0,0,0,0);

					
					//GL.ShadeModel(ShadingModel.Flat);
					GL.ShadeModel(ShadingModel.Smooth);

					///GL.Material(MaterialFace.Front, MaterialParameter.Specular, new Vector4(0.5f,0.5f,0.5f,1f));
					//GL.Material(MaterialFace.Front, MaterialParameter.Specular, mat_specular);
					/////GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, mat_diffuse);
					///GL.Material(MaterialFace.Front, MaterialParameter.Shininess, mat_shininess);

					
					GL.Light(LightName.Light0, LightParameter.Position, light_position);
					//GL.Light(LightName.Light0, LightParameter.QuadraticAttenuation, 0.001f);
					//GL.Light(LightName.Light0, LightParameter.Ambient, new Vector4(0,0,0,0));
					//GL.Light(LightName.Light0, LightParameter..Ambient, new Vector4(1,0,0,1));

					//GL.Light(LightName.Light0, LightParameter.LinearAttenuation, 0.1f);
					
					GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
					GL.Clear(ClearBufferMask.DepthBufferBit);

					

					if(iFrame.IsTexturingEnabled)
					{
						iFrame.Grass.Bind();
						//GL.BindTexture(iFrame.TexIDs[0]);
						GL.Enable(EnableCap.Texture2D);
					}


					if(iFrame.IsLightingEnabled)
					{
						GL.Enable(EnableCap.Lighting);
						GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Diffuse);
						GL.Enable(EnableCap.ColorMaterial);
					}

					
					if(iFrame.IsFogEnabled)
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
						var _FogColor = new float[]{0.5f,0.5f,0.5f,1};

						GL.ClearColor(_FogColor[0],_FogColor[1],_FogColor[2],_FogColor[3]);
						GL.Clear(ClearBufferMask.ColorBufferBit);

						GL.Enable(EnableCap.Fog);
						GL.Fog(FogParameter.FogMode, (int)FogMode.Exp);
						GL.Fog(FogParameter.FogColor, _FogColor);
						GL.Fog(FogParameter.FogDensity, 0.02f);
						GL.Hint(HintTarget.FogHint, HintMode.Fastest);
						GL.Fog(FogParameter.FogStart, 0.0f);
						GL.Fog(FogParameter.FogEnd, 10f);
					}



					GL.Enable(EnableCap.Light0);
					GL.Enable(EnableCap.DepthTest);
					
					
					GL.CallList(iFrame.DisplayLists[0]);


					if(false)
					{
						//GL.Disable(EnableCap.ColorMaterial);
						//GL.Disable(EnableCap.Lighting);
						GL.Disable(EnableCap.Texture2D);

						//GL.Disable(EnableCap.Light0);
						//GL.Disable(EnableCap.Lighting);
						//GL.Disable(EnableCap.DepthTest);
						///GL.Material(MaterialFace.Front, MaterialParameter.Specular, new Vector4(1));
						///GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, mat_diffuse);
						///GL.Material(MaterialFace.Front, MaterialParameter.Shininess, 30f);


						///GL.Color4(0,0.4f,0.8f,0.8f);
						GL.Color4(0,0.2f,0.4f,0.8f);

						GL.Begin(PrimitiveType.Quads);
						{
							GL.Vertex3(   0,    0,0);
							GL.Vertex3(+1000,    0,0);
							GL.Vertex3(+1000, -1000,0);
							GL.Vertex3(   0, -1000,0);

							//GL.Vertex3(-100,-100,0);
							//GL.Vertex3(+100,-100,0);
							//GL.Vertex3(+100,+100,0);
							//GL.Vertex3(-100,+100,0);
						}
						GL.End();
					}

					GL.Disable(EnableCap.Light0);
					GL.Disable(EnableCap.Lighting);
					GL.Disable(EnableCap.DepthTest);

					GL.Disable(EnableCap.Fog);
				}
			}
		}
	}
}