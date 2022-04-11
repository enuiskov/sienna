using System;
using System.Collections.Generic;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public class Spot
	{
		public Vector3d Vector;
		public double   Radius;
		public double   HeightBias;
	}

	public struct Face
	{
		public Vector3d V1;
		public Vector3d V2;
		public Vector3d V3;
		public byte    Distance;

		public Face(Vector3d iV1,Vector3d iV2,Vector3d iV3)
		{
			this.V1 = iV1;
			this.V2 = iV2;
			this.V3 = iV3;
			this.Distance = 0;
		}
		public void Normalize()
		{
			this.V1.Normalize();
			this.V2.Normalize();
			this.V3.Normalize();
		}

		public bool IsNeighbourFor(Face iOtherFace, bool iDoIgnoreItself)
		{
			var oRes =
			(
				this.V1 == iOtherFace.V1 || this.V1 == iOtherFace.V2 || this.V1 == iOtherFace.V3 || 
				this.V2 == iOtherFace.V1 || this.V2 == iOtherFace.V2 || this.V2 == iOtherFace.V3 || 
				this.V3 == iOtherFace.V1 || this.V3 == iOtherFace.V2 || this.V3 == iOtherFace.V3
			);
			if(oRes == true && iDoIgnoreItself)
			{
				return !
				(
					(this.V1 == iOtherFace.V1 || this.V1 == iOtherFace.V2 || this.V1 == iOtherFace.V3) &&
					(this.V2 == iOtherFace.V1 || this.V2 == iOtherFace.V2 || this.V2 == iOtherFace.V3) && 
					(this.V3 == iOtherFace.V1 || this.V3 == iOtherFace.V2 || this.V3 == iOtherFace.V3)
				);
			}
			else return oRes;
		}
		public static Face[] Tesselate(Face iFace, int iLevels)
		{
			var oNewFaces = new List<Face>();
			{
				oNewFaces.Add(iFace);

				for(var cLi = 0; cLi < iLevels; cLi ++)
				{
					var cNewFaceList = new List<Face>();
					{
						foreach(var cFace in oNewFaces)
						{
							cNewFaceList.AddRange(Face.Tesselate(cFace));
						}
					}
					oNewFaces = cNewFaceList;
				}
			}
			return oNewFaces.ToArray();
		}
		public static Face[] Tesselate(Face iFace)
		{
			var oNewFaces = new Face[4];
			{
				var cV11 = iFace.V1;
				var cV22 = iFace.V2;
				var cV33 = iFace.V3;

				var cV12 = Vector3d.Lerp(cV11,cV22, 0.5f);///.Normalized();
				var cV23 = Vector3d.Lerp(cV22,cV33, 0.5f);///.Normalized();
				var cV31 = Vector3d.Lerp(cV33,cV11, 0.5f);///.Normalized();

				oNewFaces[0] = new Face(cV12,cV23,cV31);
				oNewFaces[1] = new Face(cV11,cV12,cV31);
				oNewFaces[2] = new Face(cV22,cV23,cV12);
				oNewFaces[3] = new Face(cV33,cV31,cV23);
			}
			return oNewFaces;
		}
		public static List<Face> GetNeighbourFaces(Face iCenterFace, IEnumerable<Face> iFaceArray)
		{
			var oFaces = new List<Face>(12); foreach(var cFace in iFaceArray)
			{
				if(cFace.IsNeighbourFor(iCenterFace, true))
				{
					oFaces.Add(cFace);
				}
			}
			return oFaces;
		}
		public static double IsIntersecting(Vector3d iRay, Face iFace)
		{
			return IsIntersecting(Vector3d.Zero, iRay, iFace.V1, iFace.V2, iFace.V3);
		}
		public static double IsIntersecting(Vector3d iOrig, Vector3d iDir, Vector3d iV0, Vector3d iV1, Vector3d iV2)
		{
			var _E1 = iV1 - iV0;
			var _E2 = iV2 - iV0;

			// Вычисление вектора нормали к плоскости
			var _PVec = Vector3d.Cross(iDir, _E2);
			var _Det = Vector3d.Dot(_E1, _PVec);

			// Луч параллелен плоскости
			if(_Det < 1e-8 && _Det > -1e-8)
			{
				return 0;
			}

			var _Inv_det = 1f / _Det;
			var  _TVec = iOrig - iV0;
			var _U = Vector3d.Dot(_TVec, _PVec) * _Inv_det;
			if (_U < 0 || _U > 1) {
			  return 0;
			}

			var _QVec = Vector3d.Cross(_TVec, _E1);
			var _V = Vector3d.Dot(iDir, _QVec) * _Inv_det;
			if (_V < 0 || _U + _V > 1) {
			  return 0;
			}
			return Vector3d.Dot(_E2, _QVec) * _Inv_det;
		}
	}

	public class GeoSphere
	{
		public Vector3d[] Vertices;
		public List<Face> Faces;

		public GeoSphere(int iDetailLevel)
		{
			this.Update(iDetailLevel);
		}
		public void Update(int iDetailLevel)
		{
			this.Vertices = this.GetBaseVertices();
			this.Faces = this.GetBaseTriangles();


			this.Tesselate(iDetailLevel);

			this.Normalize();
		}

		public void Normalize()
		{
			for(var cFi = 0; cFi < this.Faces.Count; cFi ++)
			{
				var cFace = this.Faces[cFi];

				
				this.Faces[cFi] = new Face
				(
					cFace.V1.Normalized(),
					cFace.V2.Normalized(),
					cFace.V3.Normalized()
				);
			}
		}
		public Vector3d[] GetBaseVertices()
		{
			var _HLeg       = (float)Math.Cos(Math.Atan(0.5));
			var _VLeg       = _HLeg / 2;
			var _RadialStep = (float)(Math.PI / 2.5);
			
			var oVertices = new Vector3d[12];
			{
				oVertices[0]  = new Vector3d(0,0,+1);
				oVertices[11] = new Vector3d(0,0,-1);

				var cIdx = 1;
				{
					for(var cA = 0f; cA < (Math.PI * 2); cA += _RadialStep)
					{
						var cX = (float)Math.Cos(cA);
						var cY = (float)Math.Sin(cA);

						oVertices[cIdx ++]  = new Vector3d(+cX, +cY, +_VLeg).Normalized();
						oVertices[cIdx + 4] = new Vector3d(-cX, -cY, -_VLeg).Normalized();
					}
					//for(var cA = 0d; cA < (Math.PI * 2); cA += _RadialStep)
					//{
					//   var cX = -Math.Cos(cA);// + Math.PI);
					//   var cY = Math.Sin(cA);// + Math.PI);

					//   oVertices[cIdx ++]  = new Vector3((float)cX, (float)cY, -(float)_VLeg);
					//}
				}
			}
			return oVertices;
		}
		//public Face[] GetBaseTriangles()
		//{
		//   var _VV = this.GetBaseVertices();

			
		//   var oFF = new Face[1]; var cFi = 0;
		//   {
		//      ///oFF[cFi ++] = new Face(_VV[0],_VV[1],_VV[2]);
		//      //oFF[cFi ++] = new Face(_VV[0],_VV[2],_VV[3]);
		//      //oFF[cFi ++] = new Face(_VV[0],_VV[3],_VV[4]);
		//      //oFF[cFi ++] = new Face(_VV[0],_VV[4],_VV[5]);
		//      oFF[cFi ++] = new Face(_VV[0],_VV[5],_VV[1]);

		//      //oFF[cFi ++] = new Face(_VV[1],_VV[ 8],_VV[ 9]);
		//      //oFF[cFi ++] = new Face(_VV[2],_VV[ 9],_VV[10]);
		//      //oFF[cFi ++] = new Face(_VV[3],_VV[10],_VV[ 6]);
		//      //oFF[cFi ++] = new Face(_VV[4],_VV[ 6],_VV[ 7]);
		//      //oFF[cFi ++] = new Face(_VV[5],_VV[ 7],_VV[ 8]);

		//      //oFF[cFi ++] = new Face(_VV[ 6],_VV[4],_VV[3]);
		//      //oFF[cFi ++] = new Face(_VV[ 7],_VV[5],_VV[4]);
		//      //oFF[cFi ++] = new Face(_VV[ 8],_VV[1],_VV[5]);
		//      //oFF[cFi ++] = new Face(_VV[ 9],_VV[2],_VV[1]);
		//      //oFF[cFi ++] = new Face(_VV[10],_VV[3],_VV[2]);

		//      //oFF[cFi ++] = new Face(_VV[11],_VV[ 7],_VV[ 6]);
		//      //oFF[cFi ++] = new Face(_VV[11],_VV[ 8],_VV[ 7]);
		//      //oFF[cFi ++] = new Face(_VV[11],_VV[ 9],_VV[ 8]);
		//      //oFF[cFi ++] = new Face(_VV[11],_VV[10],_VV[ 9]);
		//      //oFF[cFi ++] = new Face(_VV[11],_VV[ 6],_VV[10]);
		//   }
		//   return oFF;
		//}
		public List<Face> GetBaseTriangles()
		{
		   var _VV = this.GetBaseVertices();

			
		   var oFF = new Face[20]; var cFi = 0;
		   {
		      oFF[cFi ++] = new Face(_VV[ 0],_VV[ 1],_VV[ 2]);
		      oFF[cFi ++] = new Face(_VV[ 0],_VV[ 2],_VV[ 3]);
		      oFF[cFi ++] = new Face(_VV[ 0],_VV[ 3],_VV[ 4]);
		      oFF[cFi ++] = new Face(_VV[ 0],_VV[ 4],_VV[ 5]);
		      oFF[cFi ++] = new Face(_VV[ 0],_VV[ 5],_VV[ 1]);

		      oFF[cFi ++] = new Face(_VV[ 1],_VV[ 8],_VV[ 9]);
		      oFF[cFi ++] = new Face(_VV[ 2],_VV[ 9],_VV[10]);
		      oFF[cFi ++] = new Face(_VV[ 3],_VV[10],_VV[ 6]);
		      oFF[cFi ++] = new Face(_VV[ 4],_VV[ 6],_VV[ 7]);
		      oFF[cFi ++] = new Face(_VV[ 5],_VV[ 7],_VV[ 8]);

		      oFF[cFi ++] = new Face(_VV[ 6],_VV[ 4],_VV[ 3]);
		      oFF[cFi ++] = new Face(_VV[ 7],_VV[ 5],_VV[ 4]);
		      oFF[cFi ++] = new Face(_VV[ 8],_VV[ 1],_VV[ 5]);
		      oFF[cFi ++] = new Face(_VV[ 9],_VV[ 2],_VV[ 1]);
		      oFF[cFi ++] = new Face(_VV[10],_VV[ 3],_VV[ 2]);

		      oFF[cFi ++] = new Face(_VV[11],_VV[ 7],_VV[ 6]);
		      oFF[cFi ++] = new Face(_VV[11],_VV[ 8],_VV[ 7]);
		      oFF[cFi ++] = new Face(_VV[11],_VV[ 9],_VV[ 8]);
		      oFF[cFi ++] = new Face(_VV[11],_VV[10],_VV[ 9]);
		      oFF[cFi ++] = new Face(_VV[11],_VV[ 6],_VV[10]);
		   }
		   return new List<Face>(oFF);
		}


		public void Tesselate(int iLevels)
		{
			for(var cLi = 0; cLi < iLevels; cLi ++)
			{
				this.Tesselate();
				///this.Normalize();
			}
		}
		//public void Tesselate()
		//{
		//   var _NewFaces = new Face[this.Faces.Length + 3];
		//   {
		//      var cLastFaceI = this.Faces.Length - 1;
		//      var cLastFace = this.Faces[cLastFaceI];

		//      var cV11 = cLastFace.V1;
		//      var cV22 = cLastFace.V2;
		//      var cV33 = cLastFace.V3;

		//      var cV12 = Vector3.Lerp(cV11,cV22, 0.5f);///.Normalized();
		//      var cV23 = Vector3.Lerp(cV22,cV33, 0.5f);///.Normalized();
		//      var cV31 = Vector3.Lerp(cV33,cV11, 0.5f);///.Normalized();

				

		//      _NewFaces[cLastFaceI ++] = new Face(cV11,cV12,cV31);
		//      _NewFaces[cLastFaceI ++] = new Face(cV22,cV23,cV12);
		//      _NewFaces[cLastFaceI ++] = new Face(cV33,cV31,cV23);
		//      _NewFaces[cLastFaceI ++] = new Face(cV12,cV23,cV31);

		//      //for(int cOldFi = 0, cNewFi = 0; cOldFi < this.Faces.Length; cOldFi ++)
		//      //{
		//      //   var cOldFace = this.Faces[cOldFi];

		//      //   var cV11 = cOldFace.V1;
		//      //   var cV22 = cOldFace.V2;
		//      //   var cV33 = cOldFace.V3;

		//      //   var cV12 = Vector3.Lerp(cV11,cV22, 0.5f);///.Normalized();
		//      //   var cV23 = Vector3.Lerp(cV22,cV33, 0.5f);///.Normalized();
		//      //   var cV31 = Vector3.Lerp(cV33,cV11, 0.5f);///.Normalized();

					
		//      //   _NewFaces[cNewFi ++] = new Face(cV11,cV12,cV31);
		//      //   _NewFaces[cNewFi ++] = new Face(cV22,cV23,cV12);
		//      //   _NewFaces[cNewFi ++] = new Face(cV33,cV31,cV23);
		//      //   _NewFaces[cNewFi ++] = new Face(cV12,cV23,cV31);
		//      //}
		//   }

		//   Array.Copy(this.Faces, 0, _NewFaces, 0, this.Faces.Length - 1);
		//   ///this.Faces.CopyTo(_NewFaces, 0);
		//   this.Faces = _NewFaces;
		//}
		public void Tesselate()
		{
		   var _NewFaces = new Face[this.Faces.Count * 4];
		   {
		      for(int cOldFi = 0, cNewFi = 0; cOldFi < this.Faces.Count; cOldFi ++)
		      {
		         var cOldFace = this.Faces[cOldFi];

		         var cV11 = cOldFace.V1;
		         var cV22 = cOldFace.V2;
		         var cV33 = cOldFace.V3;

		         var cV12 = Vector3d.Lerp(cV11,cV22, 0.5f);///.Normalized();
		         var cV23 = Vector3d.Lerp(cV22,cV33, 0.5f);///.Normalized();
		         var cV31 = Vector3d.Lerp(cV33,cV11, 0.5f);///.Normalized();

		         _NewFaces[cNewFi ++] = new Face(cV12,cV23,cV31);
		         _NewFaces[cNewFi ++] = new Face(cV11,cV12,cV31);
		         _NewFaces[cNewFi ++] = new Face(cV22,cV23,cV12);
		         _NewFaces[cNewFi ++] = new Face(cV33,cV31,cV23);
					
		      }
		   }
		   ///this.Faces = _NewFaces;
			this.Faces.Clear();
			this.Faces.AddRange(_NewFaces);
		}
		
		public void Build()
		{
		
		}
		public void Draw()
		{
			
		}

		/**
			Eng wiki: https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm

			#include <glm/glm.hpp>
			using namespace::glm;

			// orig и dir задают начало и направление луча. v0, v1, v2 - вершины треугольника.
			// Функция возвращает расстояние от начала луча до точки пересечения или 0.
			float
			triangle_intersection(const vec3& orig,
										 const vec3& dir,
										 const vec3& v0,
										 const vec3& v1,
										 const vec3& v2) {
				 vec3 e1 = v1 - v0;
				 vec3 e2 = v2 - v0;
				 // Вычисление вектора нормали к плоскости
				 vec3 pvec = cross(dir, e2);
				 float det = dot(e1, pvec);

				 // Луч параллелен плоскости
				 if (det < 1e-8 && det > -1e-8) {
					  return 0;
				 }

				 float inv_det = 1 / det;
				 vec3 tvec = orig - v0;
				 float u = dot(tvec, pvec) * inv_det;
				 if (u < 0 || u > 1) {
					  return 0;
				 }

				 vec3 qvec = cross(tvec, e1);
				 float v = dot(dir, qvec) * inv_det;
				 if (v < 0 || u + v > 1) {
					  return 0;
				 }
				 return dot(e2, qvec) * inv_det;
			}
		*/
		
	}
}