using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{

	public partial class IcosahedronTest : ModelViewer
	{
		public Spot[] Spots;
		public int Seed = 2;///18;
		public int SpotLevel = 0;///15;///0;

		public int TesselLevel = 0;

		public GeoSphere Sphere;
		public int[] DisplayLists;

		public void GenSpots(int iLevel)
		{
			///var iLevel = 10;
			var iElevation = 10;

			var _Rng = new Random(this.Seed);

			///var _MyVec = new Vector3d(0.545f,-0.652f,0.525f).Normalized();
			var _MyVec = new Vector3d(0,0f,1f).Normalized();
			var _Spots = new List<Spot>();
			{
				for(var cLevel = 2; cLevel < iLevel; cLevel++)
				{
					var cStepCount = Math.Pow(2, cLevel-1) * 1;///1
					var cFalloff   = (1.0 / (int)(Math.Pow(1.4, cLevel))) * 2.0;
					///var cFalloff   = (1.0 / cLevel) * 1.0;


					///var cElevRange = 1.0 / (int)(Math.Pow(1.1, cLevel))*0.001;///cFalloff * 0.01;
					var cElevRange = 2.0 / (int)(Math.Pow(1.2, cLevel))*0.001;///cFalloff * 0.01;
					///var cElevRange = cFalloff * 0.01;
					

					

					for(var cStep = 0; cStep < cStepCount; cStep ++)
					{
						var cVec        = new Vector3d(_Rng.NextDouble() - 0.5,_Rng.NextDouble() - 0.5,_Rng.NextDouble() - 0.5).Normalized();
						var cRadius     = cFalloff;
						var cHeightBias = cElevRange * (_Rng.NextDouble() - 0.5) * iElevation;


						//if(cVec.X < 0 || cVec.Y > 0 || cVec.Z < 0) continue;
						if(Vector3d.Dot(cVec, _MyVec) < 0.99f) continue;

						var cSpot = new Spot
						{
							Vector     = cVec,
							Radius     = (float)cRadius,
							HeightBias = (float)cHeightBias
						};

						_Spots.Add(cSpot);

						//var cElevD = cElevRange * (_Rng.NextDouble() - 0.5) * iElevation;
						//var cCenRi = _Rng.Next(iHeight);
						//var cCenCi = _Rng.Next(iWidth);


						//for(var cRo = -cFalloff; cRo < cFalloff; cRo++)
						//for(var cCo = -cFalloff; cCo < cFalloff; cCo++)
						//{
						//   var cRi = cCenRi + cRo; if(cRi < 0 || cRi > iHeight - 1) continue;
						//   var cCi = cCenCi + cCo; if(cCi < 0 || cCi > iWidth  - 1) continue;

						//   var cRowDistF = (float)Math.Abs(cRo) / cFalloff;
						//   var cColDistF = (float)Math.Abs(cCo) / cFalloff;

						//   var cFoDistF = Math.Min(1.0, Math.Sqrt((cRowDistF * cRowDistF) + (cColDistF * cColDistF)));
						//   ///var cFoElevD = cElevD * (Math.Cos(cFoDistF * Math.PI) / 2 + 0.5);
						//   var cFoElevD = cElevD * (1 - cFoDistF);/// * (Math.Cos(cFoDistF * Math.PI) / 2 + 0.5);

						//   _HeightMap[cRi,cCi] += (float)cFoElevD;
						//}
					}
				}
			}
			this.Spots = _Spots.ToArray();
		}

		public IcosahedronTest()
		{
			var _View = this.Views.Perspective;
			{
				_View.Target = Vector3d.UnitZ;
				///_View.EyeOffset = new Vector3d(0,-0.2,0.2);

				_View.EyeZenith = -0.1;
				_View.EyeDistance = 0.03;
			}


			this.GenSpots(this.SpotLevel);

			this.Sphere = new GeoSphere(this.TesselLevel);
			

			this.PointerVector = Vector3d.UnitZ;
			this.UpdateHighlighting();
		}
		
		public Vector3d PointerVector = new Vector3d(0,0,1);
		public List<Face> HighlightingFaces = new List<Face>();

		public void UpdateHighlighting()
		{
			this.HighlightingFaces = new List<Face>(13);///Vector3d.Zero,Vector3d.Zero,Vector3d.Zero);

			for(var cFi = 0; cFi < this.Sphere.Faces.Count; cFi ++)
			{
				var cFace = this.Sphere.Faces[cFi];

				///cFace.Normalize();
				//Routines.I
				var cIntersection = Face.IsIntersecting(this.PointerVector, cFace);

				if(cIntersection > 0)
				{
					this.HighlightingFaces.Add(cFace);
					break;
				}
			}

			var _CenterFace = this.HighlightingFaces[0];

			this.ExpandSelection(this.HighlightingFaces, 5);
			//{
			//   var _NearNeighbourFaces = Face.GetNeighbourFaces(_CenterFace, this.Sphere.Faces);
			//   this.HighlightingFaces.AddRange(_NearNeighbourFaces);

			//   foreach(var cNearFace in _NearNeighbourFaces)
			//   {
			//      var cFarNeighbourFaceList = Face.GetNeighbourFaces(cNearFace, this.Sphere.Faces);

			//      foreach(var cFarFace in cFarNeighbourFaceList)
			//      {
			//         if(!this.HighlightingFaces.Contains(cFarFace))
			//         {
			//            this.HighlightingFaces.Add(cFarFace);
			//         }
			//      }
			//   }
			//}

			if(false)
			{
				//var _NewHLFaces = new List<Face>();
				//{
				//   for(var cOldFi = 0; cOldFi < 13; cOldFi ++)
				//   {
				//      _NewHLFaces.AddRange(Face.Tesselate(this.HighlightingFaces[cOldFi], 1));
				//   }
				//   this.HighlightingFaces.RemoveRange(0,13);
					
				//   this.HighlightingFaces.InsertRange(0, _NewHLFaces);
				//}
			}
			if(true)
			{
				//var _NewHLFaces = new List<Face>();
				//{
				//   for(var cOldFi = 0; cOldFi < 13; cOldFi ++)
				//   {
				//      _NewHLFaces.AddRange(Face.Tesselate(this.HighlightingFaces[cOldFi], 1));
				//   }
				//   this.HighlightingFaces.RemoveRange(0,13);
					
				//   this.HighlightingFaces.InsertRange(0, _NewHLFaces);
				//}
			}

			//if(false)
			{
				var _NewHLFaces = new List<Face>();
				{
					for(var cOldFi = 0; cOldFi < this.HighlightingFaces.Count; cOldFi ++)
					{
						_NewHLFaces.AddRange(Face.Tesselate(this.HighlightingFaces[cOldFi], 2));///4
					}
					this.HighlightingFaces = _NewHLFaces;
				}
			}
			

			for(var cFi = 0; cFi < this.HighlightingFaces.Count; cFi ++)
			{
				var cFace = this.HighlightingFaces[cFi];
				cFace.Normalize();
				this.HighlightingFaces[cFi] = cFace;
			}
		}
		public void ExpandSelection(List<Face> ioSelFaces, int iSteps)
		{
			for(var cSi = 0; cSi < iSteps; cSi ++)
			{
				var _SelFaces = new List<Face>(ioSelFaces);
				{
					foreach(var cSelFace in ioSelFaces)
					{
						var cNearNeighbourFaces = Face.GetNeighbourFaces(cSelFace, this.Sphere.Faces);

						foreach(var cNearFace in cNearNeighbourFaces)
						{
							if(!_SelFaces.Contains(cNearFace))
							{
								_SelFaces.Add(cNearFace);
							}
						}
					}
				
				}
				ioSelFaces.Clear();
				ioSelFaces.AddRange(_SelFaces);
			}
		}
		//public void ExpandSelection(List<Face> ioSelFaces)
		//{
		//   var _SelFaces = new List<Face>(ioSelFaces);
		//   {
		//      foreach(var cSelFace in ioSelFaces)
		//      {
		//         var _NearNeighbourFaces = Face.GetNeighbourFaces(cSelFace, this.Sphere.Faces);
		//         _SelFaces.AddRange(_NearNeighbourFaces);

		//         foreach(var cNearFace in _NearNeighbourFaces)
		//         {
		//            var cFarNeighbourFaceList = Face.GetNeighbourFaces(cNearFace, this.Sphere.Faces);

		//            foreach(var cFarFace in cFarNeighbourFaceList)
		//            {
		//               if(!_SelFaces.Contains(cFarFace))
		//               {
		//                  _SelFaces.Add(cFarFace);
		//               }
		//            }
		//         }
		//      }
		//   }
		//   ioSelFaces.Clear();
		//   ioSelFaces.AddRange(_SelFaces);
		//}

		public override void CustomRender()
		{
			this.ApplyControl(G.Vehicle as Simulation.DynamicObject);
			G.Simulator.Update();



			

			ModelViewer.Routines.Rendering.SetProjectionMatrix(this);


			if(G.Simulator.IsActive)
			{
				G.Simulator.SetProjectionMatrix(this);

			}
		
			//ModelViewer.Routines.Rendering.Draw(this);

			
			
			//GL.BindTexture(TextureTarget.Texture2D, Routines.TexID);
			//GL.Color4(1.0f,1.0f,1.0f,1.0f);

			//this.EnvOn();
			//ModelViewer.Routines.Rendering.EnableEnvironment(false,false,false);
			if(G.Simulator.IsActive)
			{
				this.DrawGridXY();
				this.DrawVehicle();
				return;
			}


			this.DrawIcosa_Immed();

			

			GL.Color4(0f,1f,0f,1f);
			GL.LineWidth(3f);

			if(false) foreach(var cFace in this.HighlightingFaces)
			{
				GL.Begin(PrimitiveType.LineLoop);
				{
					GL.Vertex3(cFace.V1);
					GL.Vertex3(cFace.V2);
					GL.Vertex3(cFace.V3);
				}
				GL.End();

			}
			
			///Routines.Rendering.EnableEnvironment(false,false,false);
			///this.DrawRandoms_Immed();
			///Routines.Rendering.DisableEnvironment();
			///ModelViewer.Routines.Rendering.DisableEnvironment();
			

			///ModelViewer.Routines.Rendering.DrawPivotPoint(this);
			//ModelViewer.Routines.Rendering.DrawPivotPoint(this);
			
			//this.DrawCube_VBO();
			
			///this.EnvOff();
			
			
			//GL.Disable(EnableCap.Lighting);
			//GL.BufferData
			///Test();



			//if(!G.Simulator.IsActive)
			//{
				var _Ptr = this.Views.Perspective.Pointer;
				this.PointerVector = _Ptr.Target.Normalized();



				var _PerspViewTgt = this.PointerVector;///this.Views.Perspective.Target.Normalized();
				{
					GL.Color4(1f,1f,0f,1f);

					//GL.LineWidth(1);
					//GL.Begin(PrimitiveType.Lines);
					//{
					//   GL.Vertex3(0,0,0);
					//   GL.Vertex3(_PerspViewTgt);
					//}
					//GL.End();

					GL.PointSize(10);
					GL.Begin(PrimitiveType.Points);
					{
						GL.Vertex3(_PerspViewTgt);
					}
					GL.End();
				}
			//}
		}
		public double GetVectorLength(Vector3d iVec)
		{
			var oLen = 1.0; foreach(var cSpot in this.Spots)
			{
				var cDist = Vector3d.Subtract(iVec,cSpot.Vector).Length; if(cDist >= cSpot.Radius) continue;


				oLen += cSpot.HeightBias * ((cSpot.Radius - cDist) / cSpot.Radius);
			}
			///iVec;
			return oLen;
		}
		public void DrawRandoms_Immed()
		{
			var _Rng = new Random(0);



			///var _BaseVec = new Vector3d(_Rng.NextDouble(),_Rng.NextDouble(),_Rng.NextDouble()).Normalized();
			var _BaseVec = new Vector3d(0,-1,0).Normalized();
			

			GL.Color4(this.Palette.Adapt(CHSAColor.Glare));
			///GL.Color4(0.0,0.0,0.0,0.5);

			var _BlueColor  = new Vector4(0f, 0f, 1f, 1f);
			var _GreenColor = new Vector4(0f, 1f, 0f, 1f);
			var _RedColor   = new Vector4(1f, 0f, 0f, 1f);
			

			GL.PointSize(10f);
			GL.Begin(PrimitiveType.Points);
			{
				
				var _EyeP = this.Views.CurrentPerspective.Eye;
				var _EyeD = this.Views.CurrentPerspective.EyeDistance * 0.5;

				for(var cPi = 0; cPi < 10000; cPi ++)
				{
					var cVec = new Vector3d
					(
						(float)(_Rng.NextDouble() - 0.5),
						(float)(_Rng.NextDouble() - 0.5),
						(float)(_Rng.NextDouble() - 0.5)
					);
					
					
					///GL.Color4(0.5,0.5,0.5,_Rng.NextDouble());
					//GL.Color4
					//(
					//   Math.Abs(cVec.X / 2 + 0.5),
					//   Math.Abs(cVec.Y / 2 + 0.5),
					//   Math.Abs(cVec.Z / 2 + 0.5),
					//   1
					//);///_Rng.NextDouble());


					cVec.Normalize();

					var cDotP = Vector3d.Dot(cVec,_EyeP);
					{
						if(cDotP < _EyeD) continue;
					}


					var cVecLen = this.GetVectorLength(cVec);
					var cHeight = (float)cVecLen - 1;

					cVec = Vector3d.Multiply(cVec, cVecLen);


					var cColor = _BlueColor;
					{
						cColor = Vector4.Lerp(cColor, _GreenColor, MathEx.ClampZP((cHeight + 0.1f)  * 10f));
						cColor = Vector4.Lerp(cColor, _RedColor,   MathEx.ClampZP((cHeight + 0.0f)  * 10f));
					}




					//(_BaseVec.Sub(cVec)
					///GL.Color4(MathEx.ClampZP(1.0 - (Vector3d.Subtract(_BaseVec,cVec).Length * 2)),0,0,1d);

					//var cColor = 
					
					//GL.Color4(0,0,0,MathEx.ClampZP(1.0 - (Vector3d.Subtract(_BaseVec,cVec).Length * 2)));
					///GL.Color4(0,0,0,MathEx.ClampZP(1.0 - (cHeight * 1)));

					GL.Color4(cColor);
					GL.Vertex3(cVec);
				}
			}
			GL.End();
		}

		///public Vector4 _BlueColor  = new Vector4(0f, 0f, 1f, 1f);
		///public Vector4 _GreenColor = new Vector4(0f, 1f, 0f, 1f);
		//public Vector4 _RedColor   = new Vector4(1f, 0f, 0f, 1f);

		public Vector4 _SeabedColor = new Vector4(0.1f, 0.2f, 0.3f, 1f);
		public Vector4 _WaterColor  = new Vector4(  0f, 0.5f,   1f, 1f);
		public Vector4 _SandColor   = new Vector4(1.0f, 1.0f, 0.6f, 1f);
		public Vector4 _GrassColor  = new Vector4(0.2f, 0.6f,   0f, 1f);
		public Vector4 _RockColor   = new Vector4(0.5f, 0.4f, 0.3f, 1f);
		public Vector4 _SnowColor   = new Vector4(  1f,   1f,   1f, 1f);
		
		
		public void AddVec(Vector3d iVec, bool iDoAffectColor)
		{
			var cVecLen = this.GetVectorLength(iVec);
			var cHeight = (float)cVecLen - 1;

			var cColor = _SeabedColor;
			{
				///var _Scalar = 1000;
				var _WaterF = MathEx.ClampZP((cHeight + 0.005f) * 100);
				var _SandF  = MathEx.ClampZP((cHeight - 0.000f) * 1000);
				var _GrassF = MathEx.ClampZP((cHeight - 0.001f) * 1000);
				var _RockF  = MathEx.ClampZP((cHeight - 0.002f) * 100);
				var _SnowF  = MathEx.ClampZP((cHeight - 0.010f) * 10000);

				cColor = Vector4.Lerp(cColor, _WaterColor, _WaterF);
				cColor = Vector4.Lerp(cColor, _SandColor,   _SandF);
				cColor = Vector4.Lerp(cColor, _GrassColor,   _GrassF);
				cColor = Vector4.Lerp(cColor, _RockColor,   _RockF);
				cColor = Vector4.Lerp(cColor, _SnowColor,   _SnowF);
			}

			cHeight = Math.Max(cHeight, 0);
			iVec = Vector3d.Multiply(iVec, (cVecLen + (cHeight * 3)));

			if(iDoAffectColor) GL.Color4(cColor);
			GL.Vertex3(iVec);
		}
		///public void AddVec(Vector3d iVec)
		//{
		//   var cVecLen = this.GetVectorLength(iVec);
		//   var cHeight = cVecLen - 1;

		//   var cColor = _BlueColor;
		//   {
		//      ///var _Scalar = 1000;
		//      var _GrnF = MathEx.ClampZP((cHeight - 0.00f) * 1000);
		//      var _RedF = MathEx.ClampZP((cHeight - 0.01f) * 1000);

		//      cColor = Vector4.Lerp(cColor, _GreenColor, _GrnF);
		//      cColor = Vector4.Lerp(cColor, _RedColor,   _RedF);
		//   }

		//   cHeight = Math.Max(cHeight, 0);
		//   iVec = Vector3d.Multiply(iVec, cVecLen + (cHeight * 10));

		//   GL.Color4(cColor);
		//   GL.Vertex3(iVec);
		//}
		public void GenerateIcosaList()
		{
			this.DisplayLists = new int[1];
			var _ListID = this.DisplayLists[0] = GL.GenLists(1);

			GL.NewList(_ListID, ListMode.Compile);
			{
				GL.Enable(EnableCap.DepthTest);
				///GL.Enable(EnableCap.CullFace);
				///GL.CullFace(CullFaceMode.Back);
				GL.FrontFace(FrontFaceDirection.Ccw);
				


				GL.PushMatrix();
				///GL.LoadIdentity();
				//GL.Translate(0,0,-6e6);
				//GL.Scale(6e6,6e6,6e6);

				///GL.Translate(0,0,-1);
				GL.Scale(1,1,1);

				//if(false)
				{
					GL.Begin(PrimitiveType.Triangles);
					{
						///foreach(var cFace in this.Sphere.Faces)
						foreach(var cFace in this.HighlightingFaces)
						{
							//GL.Vertex3(cFace.V1);
							//GL.Vertex3(cFace.V2);
							//GL.Vertex3(cFace.V3);

							this.AddVec(cFace.V1,true);
							this.AddVec(cFace.V2,true);
							this.AddVec(cFace.V3,true);
						}
					}
					GL.End();
				}
				//if(false)
				//{
				//   GL.Begin(PrimitiveType.Triangles);
				//   {
				//      foreach(var cFace in this.Sphere.Faces)
				//      {
				//         //GL.Vertex3(cFace.V1);
				//         //GL.Vertex3(cFace.V2);
				//         //GL.Vertex3(cFace.V3);

				//         this.AddVec(cFace.V1);
				//         this.AddVec(cFace.V2);
				//         this.AddVec(cFace.V3);
				//      }
				//   }
				//   GL.End();
				//}

				
				//if(false)
				{
					GL.LineWidth(1f);
					///GL.Color4(this.Palette.Adapt(CHSAColor.Glare));
					GL.Color4(0,0,0,1f);
					

					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
					GL.Enable(EnableCap.PolygonOffsetLine);
					GL.PolygonOffset(-1,-1);

					GL.Begin(PrimitiveType.Triangles);
					{
						///foreach(var cFace in this.Sphere.Faces)
						foreach(var cFace in this.HighlightingFaces)
						{
							//GL.Vertex3(cFace.V1);
							//GL.Vertex3(cFace.V2);
							//GL.Vertex3(cFace.V3);
							this.AddVec(cFace.V1,false);
							this.AddVec(cFace.V2,false);
							this.AddVec(cFace.V3,false);
						}
					}
					GL.End();

					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
					GL.Disable(EnableCap.PolygonOffsetLine);
				}
				
				if(false)
				{
					GL.PointSize(5f);
					GL.Color4(this.Palette.Adapt(CHSAColor.Glare));
					
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
					GL.Enable(EnableCap.PolygonOffsetPoint);
					GL.PolygonOffset(0,0);

					GL.Begin(PrimitiveType.Triangles);
					{
						foreach(var cFace in this.Sphere.Faces)
						{
							GL.Vertex3(cFace.V1);
							GL.Vertex3(cFace.V2);
							GL.Vertex3(cFace.V3);

							this.AddVec(cFace.V1,true);
							this.AddVec(cFace.V2,true);
							this.AddVec(cFace.V3,true);
						}
					}
					GL.End();

					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
					GL.Disable(EnableCap.PolygonOffsetPoint);
				}


				//GL.Color4(this.Palette.Adapt(CHSAColor.Glare));

				//GL.Begin(PrimitiveType.Lines);
				//{ 
				//   foreach(var cFace in this.Sphere.Faces)
				//   {
				//      GL.Vertex3(cFace.V1); GL.Vertex3(cFace.V3);
				//      GL.Vertex3(cFace.V2); GL.Vertex3(cFace.V1);
				//      GL.Vertex3(cFace.V3); GL.Vertex3(cFace.V2);
				//   }
				//}
				//GL.End();

				//GL.LineWidth(3f);
				//GL.Begin(PrimitiveType.LineLoop);
				//{
				//   foreach(var cVertex in this.Sphere.Vertices)
				//   {
				//      GL.Vertex3(cVertex);
				//   }
				//}
				//GL.End();

				//GL.PointSize(10f);
				//GL.Begin(PrimitiveType.Points);
				//{
				//   foreach(var cVertex in this.Sphere.Vertices)
				//   {
				//      GL.Vertex3(cVertex);
				//   }
				//}
				//GL.End();

				GL.Disable(EnableCap.CullFace);
				GL.Disable(EnableCap.DepthTest);

				GL.PopMatrix();
			}
			GL.EndList();
		}
		public void ResetLists()
		{
			if(this.DisplayLists != null && this.DisplayLists[0] != -1)
			{
				GL.DeleteLists(this.DisplayLists[0], 1);
				this.DisplayLists[0] = -1;
			}
		}
		public void DrawIcosa_Immed()
		{
			///GL.Clear(ClearBufferMask.DepthBufferBit);
			GL.Clear(ClearBufferMask.DepthBufferBit);
			

			GL.Color4(this.Palette.Adapt(CHSAColor.Grey));
			

			if(this.DisplayLists == null || this.DisplayLists[0] == -1)
			{
				this.GenerateIcosaList();
			}
			GL.CallList(this.DisplayLists[0]);

			
		}
		//public void DrawGridXY()
		//{
		//   var _Size = 1000;
		//   var _Step = 100;
		//   GL.Color4(0f,0.5f,0f,1f);
		//   GL.PointSize(3f);
		//   GL.Begin(PrimitiveType.Points);
		//   {
		//      for(var cX = -_Size; cX < _Size; cX += _Step)
		//      {
		//         for(var cY = -_Size; cY < _Size; cY += _Step)
		//         {
		//            GL.Vertex3(cX,cY,0);
		//         }
		//      }

				
				
		//   }
		//   GL.End();


		//   ModelViewer.Routines.Rendering.DrawPivotPoint(this);
		//}
		public void DrawGridXY()
		{
		   var _Size = 1000;
		   var _Step = 10;
		   GL.Color4(0f,0.2f,0f,1f);
		   GL.LineWidth(1);
		   GL.Begin(PrimitiveType.Lines);
		   {
		      for(var cX = -_Size; cX < _Size; cX += _Step)
		      {
		         GL.Vertex3(cX,-_Size,0);
		         GL.Vertex3(cX,+_Size,0);
		      }

		      for(var cY = -_Size; cY < _Size; cY += _Step)
		      {
		         GL.Vertex3(-_Size,cY,0);
		         GL.Vertex3(+_Size,cY,0);
		      }
				
		   }
		   GL.End();


			GL.Color4(0f,1f,0f,1f);
		   GL.LineWidth(2);
		   GL.Begin(PrimitiveType.LineLoop);
		   {
		      GL.Vertex3(+1000,+10,0);
				GL.Vertex3(+1000,-10,0);
				GL.Vertex3(-1000,-10,0);
				GL.Vertex3(-1000,+10,0);
		   }
		   GL.End();

		   ModelViewer.Routines.Rendering.DrawPivotPoint(this);
		}
		public void DrawVehicle()
		{
			var _ActObj = G.Simulator.Objects[0] as Simulation.DynamicObject;

			var _ObjMat = Matrix4d.CreateFromQuaternion(_ActObj.Rotation) * Matrix4d.CreateTranslation(_ActObj.Position);
			///var _ObjMat = Matrix4d.Identity * Matrix4d.Scale(1) * Matrix4d.CreateFromQuaternion(_ActObj.Rotation) * Matrix4d.CreateTranslation(_ActObj.Position);
			//var _ObjMat = Matrix4d.Identity * Matrix4d.Scale(1) * Matrix4d.CreateFromQuaternion(_ActObj.Rotation);

			GL.PushMatrix();
			///GL.LoadMatrix(ref _ObjMat);
			///GL.Translate(_ActObj.Position);
			GL.MultMatrix(ref _ObjMat);

			GL.LineWidth(3f);
			GL.Begin(PrimitiveType.Lines);
			{
				GL.Color4(1f,1.0f,1f,1f);

				///~~ front and rear;
				GL.Vertex3(-1,+2,-0.5); GL.Vertex3(+1,+2,-0.5);
				GL.Vertex3(-1,+2,+0.0); GL.Vertex3(+1,+2,+0.0);
				GL.Vertex3(-1,-2,-0.5); GL.Vertex3(+1,-2,-0.5);
				GL.Vertex3(-1,-2,+0.5); GL.Vertex3(+1,-2,+0.5);

				///~~ left and right;
				GL.Vertex3(-1,+2,-0.5); GL.Vertex3(-1,-2,-0.5);
				GL.Vertex3(-1,+2,+0.0); GL.Vertex3(-1,-2,+0.5);
				GL.Vertex3(+1,+2,-0.5); GL.Vertex3(+1,-2,-0.5);
				GL.Vertex3(+1,+2,+0.0); GL.Vertex3(+1,-2,+0.5);

				///~~ verticals;
				GL.Vertex3(-1,+2,-0.5); GL.Vertex3(-1,+2,+0.0);
				GL.Vertex3(+1,+2,-0.5); GL.Vertex3(+1,+2,+0.0);

				GL.Vertex3(-1,-2,-0.5); GL.Vertex3(-1,-2,+0.5);
				GL.Vertex3(+1,-2,-0.5); GL.Vertex3(+1,-2,+0.5);

				///~~ cabin;
				{
					///~~ vert;
					GL.Vertex3(-1.0,+1.0,0.1); GL.Vertex3(-0.8,+0.2,+1.0);
					GL.Vertex3(+1.0,+1.0,0.1); GL.Vertex3(+0.8,+0.2,+1.0);
					GL.Vertex3(-1.0,-2.0,0.5); GL.Vertex3(-0.8,-1.0,+1.0);
					GL.Vertex3(+1.0,-2.0,0.5); GL.Vertex3(+0.8,-1.0,+1.0);

					///~~ hrz sides;
					GL.Vertex3(-0.8,+0.2,+1.0); GL.Vertex3(-0.8,-1.0,+1.0);
					GL.Vertex3(+0.8,+0.2,+1.0); GL.Vertex3(+0.8,-1.0,+1.0);
					///~~ hrz front and rear;
					GL.Vertex3(-0.8,+0.2,+1.0); GL.Vertex3(+0.8,+0.2,+1.0);
					GL.Vertex3(-0.8,-1.0,+1.0); GL.Vertex3(+0.8,-1.0,+1.0);
				}

				//GL.Vertex3(-1,+2,+0.5); GL.Vertex3(+1,+2,+0.5);
				//GL.Vertex3(-1,-2,-0.5); GL.Vertex3(+1,-2,-0.5);
				//GL.Vertex3(-1,-2,+0.5); GL.Vertex3(+1,-2,+0.5);

//            GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);
//GL.Vertex3(,,); GL.Vertex3(,,);

			}
			GL.End();

			foreach(var cForce in _ActObj.Forces)
			{
				if(cForce is Simulation.Force.Wheel)
				{
					this.DrawWheel(cForce as Simulation.Force.Wheel);
				}
			}
			GL.PopMatrix();


			///~~ projecting wheels to the world's XY plane;
			foreach(var cForce in _ActObj.Forces)
			{
				if(cForce is Simulation.Force.Wheel && cForce.Factor > 0)
				{
					//if()

					this.DrawWheelProjection(cForce as Simulation.Force.Wheel);
				}
			}
		}
		public void DrawWheel(Simulation.Force.Wheel iWheel)
		{
			///iWheel.Rotation *= Quaterniond.FromAxisAngle(Vector3d.UnitX, -0.1);
			var _Mat = Matrix4d.Identity * Matrix4d.RotateX(iWheel.Angle) * Matrix4d.CreateFromQuaternion(iWheel.Rotation) * Matrix4d.CreateTranslation(iWheel.Position - new Vector3d(0,0,-0.4));///-0.4 + iWheel.Compression));
			///var _Mat = Matrix4d.Identity * Matrix4d.CreateTranslation(iWheel.Position);/// * Matrix4d.CreateFromQuaternion(iWheel.Rotation);

			GL.PushMatrix();
			GL.MultMatrix(ref _Mat);


			GL.Begin(PrimitiveType.LineLoop);
			{
				for(var cA = 0.0; cA < MathEx.D360; cA += Math.PI / 4)
				{
					//var cAngle = cA + (iWheel.Angle);/// % MathEx.D360);
					GL.Vertex3(0, Math.Sin(cA) * 0.4, Math.Cos(cA) * 0.4);
				}
			}
			GL.End();

			GL.Begin(PrimitiveType.Lines);
			{
				GL.Vertex3(0,0,-0.0);
				GL.Vertex3(0,0,+0.4);
			}
			GL.End();
			GL.PopMatrix();
		}
		public void DrawWheelProjection(Simulation.Force.Wheel iWheel)
		{
			GL.PushMatrix();

			var _Veh = iWheel.Owner;
			var _ProjV = _Veh.Position + Vector3d.Transform(iWheel.Position, _Veh.Rotation);
			GL.Translate(_ProjV.X,_ProjV.Y,0);

			GL.LineWidth(1);
			GL.Begin(PrimitiveType.Lines);
			{
				GL.Vertex3(0,0,0);
				GL.Vertex3(0,0,_ProjV.Z);
			}
			GL.End();
			
			GL.LineWidth(2);
			GL.Begin(PrimitiveType.Lines);
			{
				GL.Vertex3(0,+0.2,0);
				GL.Vertex3(0,-0.2,0);

				GL.Vertex3(+0.2,0,0);
				GL.Vertex3(-0.2,0,0);
			}
			GL.End();
			GL.PopMatrix();
		}
		//public void DrawIcosa_Immed()
		//{
		//   GL.Color4(this.Palette.Adapt(CHSAColor.Glare));

		//   GL.PointSize(10f);
		//   GL.Begin(PrimitiveType.LineLoop);
		//   {
		//      foreach(var cVertex in this.Sphere.Vertices)
		//      {
		//         GL.Vertex3(cVertex);
		//      }
		//   }
		//   GL.End();
		//}
		//public void DrawIcosa_Immed()
		//{
		//   var _HLeg       = Math.Cos(Math.Atan(0.5));
		//   var _VLeg       = _HLeg / 2;
		//   var _RadialStep = Math.PI / 2.5;
			
		//   GL.Color4(this.Palette.Adapt(CHSAColor.Glare));

		//   GL.PointSize(10f);
		//   GL.Begin(PrimitiveType.Points);
		//   {
		//      GL.Vertex3(0,0,1f);


		//      for(var cA = 0d; cA < (Math.PI * 2); cA += _RadialStep)
		//      {
		//         //var cA * 
		//         var cX = Math.Cos(cA);
		//         var cY = Math.Sin(cA);

		//         GL.Vertex3(+cX, +cY, +_VLeg);
		//         GL.Vertex3(-cX, -cY, -_VLeg);

		//      }
		//      //GL.Vertex3(0, _HLeg, _VLeg);
		//      //GL.Vertex3(0,-_HLeg,-_VLeg);





		//      GL.Vertex3(0,0,-1f);
		//   }
		//   GL.End();
		//}
		protected override void OnKeyDown(KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);

			if(iEvent.KeyCode == Keys.F12)
			{
				G.Simulator.IsActive =! G.Simulator.IsActive;

				return;
			}

			if(!G.Simulator.IsActive)
			{
				
				switch(iEvent.KeyCode)
				{
					case Keys.PageUp:   this.Seed --; this.GenSpots(this.SpotLevel); this.ResetLists(); break;
					case Keys.PageDown: this.Seed ++; this.GenSpots(this.SpotLevel); this.ResetLists(); break;

					case Keys.OemOpenBrackets : this.SpotLevel = Math.Max(this.SpotLevel - 1, 2); this.GenSpots(this.SpotLevel); this.ResetLists(); break;
					case Keys.OemCloseBrackets : this.SpotLevel = Math.Max(this.SpotLevel + 1, 2); this.GenSpots(this.SpotLevel); this.ResetLists(); break;

					case Keys.Space : 
					{
						//var _PerspView = this.Views.CurrentPerspective;
						//_PerspView.Target = _PerspView.EyeOffset.Normalized();
						//_PerspView.EyeOffset = _PerspView.Target.Normalized();
						
						this.UpdateHighlighting();
						///this.Sphere.Update(this.TesselLevel);
						this.ResetLists();
						break;
					}
					case Keys.Oemplus : 
					{
						this.TesselLevel = Math.Max(this.TesselLevel + 1, 0);
						this.Sphere.Update(this.TesselLevel);
						this.ResetLists();
						break;
					}
					case Keys.OemMinus : 
					{
						this.TesselLevel = Math.Max(this.TesselLevel - 1, 0);
						this.Sphere.Update(this.TesselLevel);
						this.ResetLists();
						break;
					}
					case Keys.F3 : 
					{
						System.Windows.Forms.MessageBox.Show(this.PointerVector.ToString());
						break;
					}
					
					case Keys.Enter : 
					{
						break;
					}
					



				}
			}
			else
			{
				switch(iEvent.KeyCode)
				{
					case Keys.NumPad0 : G.Simulator.ViewMode = AE.Simulation.SimViewMode.Num0; break;
					case Keys.NumPad1 : G.Simulator.ViewMode = AE.Simulation.SimViewMode.Num1; break;
					case Keys.NumPad2 : G.Simulator.ViewMode = AE.Simulation.SimViewMode.Num2; break;
					case Keys.NumPad3 : G.Simulator.ViewMode = AE.Simulation.SimViewMode.Num3; break;
					case Keys.NumPad4 : G.Simulator.ViewMode = AE.Simulation.SimViewMode.Num4; break;
					case Keys.NumPad5 : G.Simulator.ViewMode = AE.Simulation.SimViewMode.Num5; break;
					case Keys.NumPad6 : G.Simulator.ViewMode = AE.Simulation.SimViewMode.Num6; break;
					case Keys.NumPad7 : G.Simulator.ViewMode = AE.Simulation.SimViewMode.Num7; break;
					case Keys.NumPad8 : G.Simulator.ViewMode = AE.Simulation.SimViewMode.Num8; break;
					case Keys.NumPad9 : G.Simulator.ViewMode = AE.Simulation.SimViewMode.Num9; break;

					case Keys.H       :
					{
						var _HUD = (this.Parent.Children[1] as VehicleHUDFrame);
						_HUD.DoDisplayHorizon =! _HUD.DoDisplayHorizon;
						
						break;
					}
					
					//case Keys.Insert:   G.Vehicle.Rotation = Quaterniond.Multiply(G.Vehicle.Rotation, Quaterniond.FromAxisAngle(Vector3d.UnitZ, +0.01));   break;
					//case Keys.PageUp:   G.Vehicle.Rotation = Quaterniond.Multiply(G.Vehicle.Rotation, Quaterniond.FromAxisAngle(Vector3d.UnitZ, -0.01));   break;

					//case Keys.Home:     G.Vehicle.Rotation = Quaterniond.Multiply(G.Vehicle.Rotation, Quaterniond.FromAxisAngle(Vector3d.UnitX, -0.01));   break;
					//case Keys.End:      G.Vehicle.Rotation = Quaterniond.Multiply(G.Vehicle.Rotation, Quaterniond.FromAxisAngle(Vector3d.UnitX, +0.01));   break;

					//case Keys.Delete:   G.Vehicle.Rotation = Quaterniond.Multiply(G.Vehicle.Rotation, Quaterniond.FromAxisAngle(Vector3d.UnitY, -0.05));   break;
					//case Keys.PageDown: G.Vehicle.Rotation = Quaterniond.Multiply(G.Vehicle.Rotation, Quaterniond.FromAxisAngle(Vector3d.UnitY, +0.05));   break;

					//case Keys.Up:       G.Vehicle.Position = Vector3d.Add(G.Vehicle.Position, Vector3d.Transform(Vector3d.UnitY * +0.001, G.Vehicle.Rotation));   break;
					//case Keys.Down:     G.Vehicle.Position = Vector3d.Add(G.Vehicle.Position, Vector3d.Transform(Vector3d.UnitY * -0.001, G.Vehicle.Rotation));   break;

					case Keys.PageDown:  break;

				}
			}
		}
		protected override void OnMouseDown(MouseEventArgs iEvent)
		{
			base.OnMouseDown(iEvent);

			if(iEvent.Button == MouseButtons.Left)
			{
				//var _Ptr = this.Views.Perspective.Pointer;
				//this.PointerVector = (Vector3d)_Ptr.Target.Normalized();
			}

			
		}
		
		protected override void OnMouseMove(MouseEventArgs iEvent)
		{
		   base.OnMouseMove(iEvent);


			if(G.Screen.Dragmeter.MiddleButton.IsDragging)
			{
				var _DeltaX = G.Screen.Dragmeter.MiddleButton.OffsetInt.X * -0.01;
				var _DeltaY = G.Screen.Dragmeter.MiddleButton.OffsetInt.Y * -0.01;

				//this.EyeAzimuth += iAzimuthDelta;
				//this.EyeZenith   = MathEx.Clamp(this.EyeZenith + _DeltaX, -Math.PI / 2 * 0.999, +Math.PI / 2 * 0.999);
					

				G.Simulator.ExtViewpointInfo.X = (G.Simulator.ExtViewpointInfo.X + _DeltaX);
				G.Simulator.ExtViewpointInfo.Y = MathEx.Clamp(G.Simulator.ExtViewpointInfo.Y + _DeltaY, -Math.PI / 2 * 0.999, +Math.PI / 2 * 0.999);
			}


			if(this.Views.Perspective.Target.Length == 0)
			{
				this.Views.Perspective.Target = this.Views.Perspective.EyeOffset;
			}
		   this.Views.Perspective.Target.Normalize();



			
			///this.UpdateHighlighting();
		}
		protected override void OnLoad(GenericEventArgs iEvent)
		{
			base.OnLoad(iEvent);

			
		}

		public void ApplyControl(Simulation.DynamicObject iVehicle)
		{
			///iVehicle.Velocity.Acceleration.Angular = Vector3d.Zero;
			///return;

			//if(this.State.Keys.Delete   == 1) iVehicle.Velocity.Acceleration.Angular += new Vector3d(0,-0.0001,0);
			//if(this.State.Keys.PageDown == 1) iVehicle.Velocity.Acceleration.Angular += new Vector3d(0,+0.0001,0);

			//if(this.State.Keys.Home     == 1) iVehicle.Velocity.Acceleration.Angular += new Vector3d(-0.0001,0,0);
			//if(this.State.Keys.End      == 1) iVehicle.Velocity.Acceleration.Angular += new Vector3d(+0.0001,0,0);

			//if(this.State.Keys.Insert   == 1) iVehicle.Velocity.Acceleration.Angular += new Vector3d(0,0,+0.0001);
			//if(this.State.Keys.PageUp   == 1) iVehicle.Velocity.Acceleration.Angular += new Vector3d(0,0,-0.0001);

			//if(this.State.Keys.Up       == 1) iVehicle.Velocity.Acceleration.Linear += Vector3d.Transform(Vector3d.UnitY * +0.000001, iVehicle.Rotation);
			//if(this.State.Keys.Down     == 1) iVehicle.Velocity.Acceleration.Linear += Vector3d.Transform(Vector3d.UnitY * -0.000001, iVehicle.Rotation);

			//if(this.State.Keys.Left     == 1) iVehicle.Velocity.Acceleration.Linear += Vector3d.Transform(Vector3d.UnitX * -0.000001, iVehicle.Rotation);
			//if(this.State.Keys.Right    == 1) iVehicle.Velocity.Acceleration.Linear += Vector3d.Transform(Vector3d.UnitX * +0.000001, iVehicle.Rotation);
			var _Keys = this.State.Keys;
			var _Forces = iVehicle.Forces;


			var _LocPitch = _Forces["Local_Pitch"];
			var _LocBank = _Forces["Local_Bank"];
			var _LocYaw = _Forces["Local_Yaw"];

			var _LocLift = _Forces["Local_Lift"];
			var _WheelFL = _Forces["WheelFL"];
			var _WheelFR = _Forces["WheelFR"];
			
			var _CruEng = _Forces["CruiseEngine"];
			


			_LocPitch.Factor = 0;
			_LocBank.Factor = 0;
			_LocYaw.Factor = 0;
			_CruEng.Factor = 0;
			

			///iVehicle.Forces[4].Factor = MathEx.Clamp(iVehicle.Forces[4].Factor, 0.0, 1); ///~~ lift;
			_LocLift.Factor = 0;///MathEx.Clamp(_LocLift.Factor, 0.0, 1); ///~~ lift;
			//iVehicle.Forces[8].Factor = 0; ///~~ test globalforce;


			_WheelFL.Rotation = Quaterniond.Identity;
			_WheelFR.Rotation = Quaterniond.Identity;
			//iVehicle.Forces[5].Rotation = Quaterniond.Identity;
			//iVehicle.Forces[6].Rotation = Quaterniond.Identity;



		
			//var _Forces = iVehicle.Forces;

			if(_Keys.Home     == 1) _LocPitch.Factor = -1;
			if(_Keys.End      == 1) _LocPitch.Factor = +1;

			if(_Keys.Delete   == 1) _LocBank.Factor = -1;
			if(_Keys.PageDown == 1) _LocBank.Factor = +1;

			if(_Keys.Insert   == 1) _LocYaw.Factor = +1;
			if(_Keys.PageUp   == 1) _LocYaw.Factor = -1;



			if(_Keys.Up       == 1) _CruEng.Factor = +1;
			if(_Keys.Down     == 1) _CruEng.Factor = -1;
			if(_Keys.Left     == 1) _WheelFL.Rotation =_WheelFR.Rotation = Quaterniond.FromAxisAngle(Vector3d.UnitZ, +0.5);
			if(_Keys.Right    == 1) _WheelFL.Rotation = _WheelFR.Rotation = Quaterniond.FromAxisAngle(Vector3d.UnitZ, -0.5);

			if(_Keys.A        == 1) _LocLift.Factor = +1;
			if(_Keys.Z        == 1) _LocLift.Factor = -1;

			//if(_Keys.F        == 1) _Forces[8].Factor = 1;

			if(_Keys.Space == 1)
			{
				G.Vehicle.Velocity.Linear = G.Vehicle.Velocity.Angular = Vector3d.Zero;
			}


			if(_Keys.D1 == 1)
			{
				G.Vehicle.Position = new Vector3d(-10,0,1);
				//G.Vehicle.Rotation = Quaterniond.Identity;
				G.Vehicle.Rotation = Quaterniond.FromAxisAngle(Vector3d.UnitZ, MathEx.D180);
				G.Vehicle.Velocity.Linear = Vector3d.Zero;
				G.Vehicle.Velocity.Angular = Vector3d.Zero;
			}
			if(_Keys.D2 == 1)
			{
				G.Vehicle.Position = new Vector3d(-10,0,1);
				//G.Vehicle.Rotation = Quaterniond.Identity;
				G.Vehicle.Rotation = Quaterniond.FromAxisAngle(Vector3d.UnitZ, MathEx.D90);
				G.Vehicle.Velocity.Linear = Vector3d.Zero;
				G.Vehicle.Velocity.Angular = Vector3d.Zero;
			}

			
		}
	}
}