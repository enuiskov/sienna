using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public struct Triangle
	{
		public Vector3 V1;
		public Vector3 V2;
		public Vector3 V3;

		public static Random RNG = new Random(0);

		public Triangle(Vector3 iV1,Vector3 iV2,Vector3 iV3)
		{
			this.V1 = iV1;
			this.V2 = iV2;
			this.V3 = iV3;
		}
		public Triangle[] Tesselate()
		{
			var _V11 = this.V1;
			var _V22 = this.V2;
			var _V33 = this.V3;

			var _V12 = Vector3.Lerp(this.V1,this.V2,0.5f);
			var _V23 = Vector3.Lerp(this.V2,this.V3,0.5f);
			var _V31 = Vector3.Lerp(this.V3,this.V1,0.5f);


			_V12 = RandomizeHeight(_V12);
			_V23 = RandomizeHeight(_V23);
			_V31 = RandomizeHeight(_V31);

			return new Triangle[]
			{
				new Triangle(_V12,_V23,_V31),

				new Triangle(_V11,_V12,_V31),
				new Triangle(_V22,_V23,_V12),
				new Triangle(_V33,_V31,_V23),
			};
		}
		public static Vector3 RandomizeHeight(Vector3 iVec)
		{
			///iVec.X + iVec.Y;
			var oVec = iVec;
			///oVec.Z = (float)RNG.NextDouble()*0.001f;

			return oVec;
		}
	}
	public partial class TriangleTesselationTest : ZoomableFrame
	{
		public Triangle RootTriangle;
		public List<Triangle> AllTriangles;

		public TriangleTesselationTest()
		{
			//this.Views.Perspective.EyeDistance = 3f;
			this.RootTriangle = this.GenerateRootTriangle();
			this.AllTriangles = new List<Triangle>();

			///this.AllTriangles.Add(this.RootTriangle);

			var cBaseTri = this.RootTriangle;
			this.AllTriangles.Add(cBaseTri);

			for(var cRi = 0; cRi < 22; cRi ++)
			{
				var cNestedTT = cBaseTri.Tesselate();

				///this.AllTriangles.AddRange(cNestedTT);
				
				cBaseTri = cNestedTT[3];
				this.AllTriangles.Add(cBaseTri);

				for(var cAddTi = 0; cAddTi < 3; cAddTi ++)
				{
					this.AllTriangles.AddRange(cNestedTT[cAddTi].Tesselate());
				}
			}
		}

		public override void CustomRender()
		{
			ZoomableFrame.Routines.Rendering.SetProjectionMatrix(this);

			ZoomableFrame.Routines.Rendering.DrawUnitSpace(this);
			///ZoomableFrame.Routines.Rendering.DrawPropeller(this);

			
			///this.DrawIcosa_Immed();
			GL.Color4(this.Palette.Adapt(new CHSAColor(0.5f,7,1,0.5f)));
			
			this.DrawTriangle(this.RootTriangle);



			
			var _TT = this.AllTriangles;

			
			var _NestCount = 5;
			foreach(var cT in _TT)
			{

				GL.Color4(this.Palette.GlareColor);

				this.DrawTriangle(cT);
			}
		}
		public void DrawNestedTriangle(Triangle iParent, int[] iPath)
		{
			iParent.Tesselate();
		}
		public void DrawTriangle(Triangle iVertices)
		{
			///GL.Color4(this.Palette.Adapt(this.Palette.ForeColor));
			
			GL.Begin(PrimitiveType.LineLoop);
			GL.Vertex3(iVertices.V1);
			GL.Vertex3(iVertices.V2);
			GL.Vertex3(iVertices.V3);
			//for(var cVi = 0; cVi < iVertices.Length; cVi ++)
			//{
			//   GL.Vertex3(iVertices[cVi]);
			//}
			GL.End();

			if(false)
			{

				GL.Color4(this.Palette.Colors[10]);
				GL.PointSize(5f);
				GL.Begin(PrimitiveType.Points);
				GL.Vertex3(iVertices.V1);
				GL.Vertex3(iVertices.V2);
				GL.Vertex3(iVertices.V3);
				//for(var cVi = 0; cVi < iVertices.Length; cVi ++)
				//{
				//   GL.Vertex3(iVertices[cVi]);
				//}
				GL.End();
			}
		}
		
		public Triangle GenerateRootTriangle()
		{
			var oVertices = new Vector3[3];
			{
				for(var cVi = 0; cVi < oVertices.Length; cVi ++)
				{
					var cAngle = ((double)cVi / 3) * MathEx.D360;
					oVertices[cVi] = new Vector3((float)Math.Sin(cAngle),(float)Math.Cos(cAngle),0f);
				}
			}
			return new Triangle{V1 = oVertices[0], V2 = oVertices[1], V3 = oVertices[2]};
		}
		public Vector3[] GenerateNestedTriangle(Vector3[] iParentTriangle)
		{
			throw new NotImplementedException();
		}
	}
}