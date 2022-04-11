using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using AE.Visualization;

namespace AE.Visualization.SchemeObjectModel
{
	//public enum NGonHelperEventType
	//{
	//    Visibility,
	//    ZoomChange,
	//    PointerIn,
	//    PointerOut,
	//}
	public class NGonHelper
	{
		public NGonNode Parent;

		public virtual void Draw()
		{
		}
		public virtual void Update()
		{
		}

		public static bool Match(NGonHelper iHelper)
		{
			return true;
		}

		public class Collection : List<NGonHelper>
		{
			public NGonNode Owner;

			public Collection(NGonNode iOwner)
			{
				this.Owner = iOwner;
			}
			

			public new void Add(NGonHelper iItem)
			{
				iItem.Parent = this.Owner;
				base.Add(iItem);
			}

			public void RemoveAll(Type iHelperType)
			{
				for(var cHi = 0; cHi < this.Count; cHi++)
				{
					if(this[cHi].GetType() == iHelperType) this.RemoveAt(--cHi);
				}
			}
			//public NGonHelper this[]
			//{
			//    get
			//    {
					
			//    }
			//    set
			//    {
					
			//    }
			//}
		}
	}

	public class NGonProtractorHelper : NGonHelper
	{
		public Vector3d Center;
		
		public override void Draw()
		{
			GL.LineWidth(1);
			GL.Begin(PrimitiveType.Lines);
			{
				
			    GL.Color4(Color.FromArgb(64, this.Parent.Frame.Palette.GlareColor));

				for(var cA = 0d; cA < Math.PI; cA += Math.PI / 12)
				{
					var cOffs = new Vector3d(Math.Sin(cA), Math.Cos(cA), 0.0);

					GL.Vertex3(this.Center - cOffs);
					GL.Vertex3(this.Center + cOffs);
				}
			}
			GL.End();

			GL.PointSize(10);
			GL.Begin(PrimitiveType.Points);
			{
			    GL.Color4(this.Parent.Frame.Palette.GlareColor);

			    GL.Vertex3(this.Center);
			}
			GL.End();
		}

		public static new bool Match(NGonHelper iHelper)
		{
			return iHelper is NGonProtractorHelper;
		}
	}
	public class NGonRulerHelper : NGonHelper
	{
		public Vector3d   Point1;
		public Vector3d   Point2;
		public Vector3d[] InterPoints;

		public void UpdateInterpoints()
		{
			var _ZoomFactor = MathEx.Clamp(10.0 / this.Parent.Viewpoint.Position.Z, 1, 256);
			///GCon.Message("Zoom=" + _ZoomF + ", PosZ=" + iNode.Viewpoint.Position.Z);
			var _PointCount = (int)Math.Max(0, Math.Pow(2, Math.Round(Math.Log(_ZoomFactor, 2))) - 1);
			
			this.InterPoints = new Vector3d[_PointCount];
			{
				for(var cPi = 0; cPi < _PointCount; cPi++)
				{
					var cFrac = (cPi + 1.0) / (_PointCount + 1.0);
					var cPoint = Vector3d.Lerp(this.Point1, this.Point2, cFrac);

					this.InterPoints[cPi] = cPoint;
				}
			}
		}
		
		public void SetPosition(Vector3d iPoint1, Vector3d iPoint2)
		{
			this.Point1 = iPoint1;
			this.Point2 = iPoint2;

			this.UpdateInterpoints();
		}
		public override void Draw()
		{
			GL.LineWidth(1);
			//GL.Begin(PrimitiveType.Lines);
			//{
			//    GL.Color3(1.0,1.0,0.0);

			//    GL.Vertex3(this.Point1);
			//    GL.Vertex3(this.Point2);
			//}
			//GL.End();

			//GL.PointSize(10);
			//GL.Begin(PrimitiveType.Points);
			//{
			//    GL.Color3(1.0,1.0,0.0);

			//    GL.Vertex3(this.Point1);
			//    GL.Vertex3(this.Point2);
			//}
			//GL.End();



			//GL.PointSize(5);


			GL.Begin(PrimitiveType.Lines);
			{
				GL.Color3(this.Parent.Palette.GlareColor);
				
				var _PerpV = Vector3d.Cross((this.Point1 - this.Point2) * 0.05, Vector3d.UnitZ);


				//var _ZoomF = 1.0 / this.Parent.Viewpoint.Position.Z;

				var _GridLen = this.InterPoints.Length + 1;
				for(var cPi = 0; cPi < this.InterPoints.Length; cPi++)
				{
					/// 128 = 1, (64,192 = 0.5), (32,96,160,225 = 0.25) etc
					//var cLineLen = 0 + (cPi % 256 % 128 % 64 % 32);

					var cNum = cPi + 1;
					var cLineLen = 8.0f;
					{
						for(var cGridStep = 1; cGridStep < 256; cGridStep *= 2)
						{
							if(_GridLen > cGridStep)
							{
								cLineLen *= (cNum % (_GridLen / cGridStep) == 0 ? 1f : 0.5f);
							}
						}
					}
					var cPoint1  = this.InterPoints[cPi];
					var cPoint2  = Vector3d.Add(cPoint1, _PerpV * (cLineLen * 0.5));

					GL.Vertex3(cPoint1);
					GL.Vertex3(cPoint2);
				}
				//foreach(var cPoint in this.InterPoints)
				//{
				//    var cPoint1 = cPoint;
				//    var cPoint2 = Vector3d.Add(cPoint, _PerpV);

				//    GL.Vertex3(cPoint1);
				//    GL.Vertex3(cPoint2);
				//}
			}
			GL.End();

			//GL.PointSize(5f);
			//GL.Begin(PrimitiveType.Points);
			//{
			//    GL.Color3(1.0,0.0,0.0);
				
			//    foreach(var cPoint in this.InterPoints)
			//    {
			//        GL.Vertex3(cPoint);
			//    }
			//}
			//GL.End();
		}

		public static new bool Match(NGonHelper iHelper)
		{
			return iHelper is NGonRulerHelper;
		}
	}
	public class NGonArcHelper : NGonHelper
	{
		public Vector3d Point1;
		public Vector3d Point2;
		public Vector3d[] EdgePoints;

		public void UpdateEdgePoints()
		{
			//var _ZoomFactor = MathEx.Clamp(this.Parent.Viewpoint.Position.Z * 10, 1, 256);
			/////GCon.Message("Zoom=" + _ZoomF + ", PosZ=" + iNode.Viewpoint.Position.Z);
			//var _PointCount = (int)Math.Max(0, Math.Pow(2, Math.Round(Math.Log(_ZoomFactor, 2))) - 1);
			
			var _EdgeCount = 24;
			var _ArcPointCount = _EdgeCount - 1;

			this.EdgePoints = new Vector3d[_ArcPointCount * 2];
			{
				var _BaseEdge  = this.Point2 - this.Point1;
				var _BaseAngle = Math.Atan2(_BaseEdge.Y, _BaseEdge.X);
				var _Radius    = (_BaseEdge.Length) * (0.5 / Math.Sin(Math.PI / 24));

				var _MidPoint = Vector3d.Lerp(this.Point1,this.Point2, 0.5);

				var _TanOffs = Vector3d.Cross(Vector3d.Normalize(_BaseEdge), Vector3d.UnitZ) * _Radius;
				var _CenterP1 = _MidPoint + _TanOffs;
				var _CenterP2 = _MidPoint - _TanOffs;
				var _AngleOffset = (Math.PI / 24.0 * 3.0);

				for(var cPi = 0; cPi < _ArcPointCount; cPi++)
				{
					var cAngle  = ((float)cPi / _EdgeCount * (2.0 * Math.PI)) + _AngleOffset;
					var cAngle1 = -_BaseAngle + cAngle;// + _AngleOffset;
					var cAngle2 = -_BaseAngle - cAngle;// - _AngleOffset;

			        var cPoint1 = _CenterP1 + new Vector3d(Math.Sin(cAngle1) * _Radius, Math.Cos(+cAngle1) * _Radius, 0.0);// Vector3d.Lerp(this.Point1, this.Point2, cFrac);
					var cPoint2 = _CenterP2 - new Vector3d(Math.Sin(cAngle2) * _Radius, Math.Cos(+cAngle2) * _Radius, 0.0);// Vector3d.Lerp(this.Point1, this.Point2, cFrac);

			        this.EdgePoints[                 cPi] = cPoint1;
					this.EdgePoints[_ArcPointCount + cPi] = cPoint2;
				}
				//for(var cPi = 0; cPi < _PointCount; cPi++)
				//{
				//    var cFrac = (cPi + 1.0) / (_PointCount + 1.0);
				//    var cPoint = Vector3d.Lerp(this.Point1, this.Point2, cFrac);

				//    this.EdgePoints[cPi] = cPoint;
				//}
			}
		}
		public void SetPosition(Vector3d iPoint1, Vector3d iPoint2)
		{
			this.Point1 = iPoint1;
			this.Point2 = iPoint2;

			this.UpdateEdgePoints();
		}
		
		public override void Draw()
		{
			GL.LineWidth(1);
			//GL.Begin(PrimitiveType.Lines);
			//{
			//    GL.Color3(1.0,1.0,0.0);

			//    GL.Vertex3(this.Point1);
			//    GL.Vertex3(this.Point2);
			//}
			//GL.End();

			GL.PointSize(10);
			GL.Begin(PrimitiveType.Points);
			{
			    GL.Color3(1.0,1.0,0.0);

			    GL.Vertex3(this.Point1);
			    GL.Vertex3(this.Point2);
			}
			GL.End();



			//GL.PointSize(5);


			
			GL.PointSize(5f);
			GL.Begin(PrimitiveType.Points);
			{
				GL.Color3(1.0,0.0,0.0);
				
				foreach(var cPoint in this.EdgePoints)
				{
					GL.Vertex3(cPoint);
				}
			}
			GL.End();
		}

		public static new bool Match(NGonHelper iHelper)
		{
			return iHelper is NGonArcHelper;
		}
	}
}
