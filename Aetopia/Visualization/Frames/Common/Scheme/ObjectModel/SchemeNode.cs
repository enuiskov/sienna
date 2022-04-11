using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using AE.Visualization;
using SDColor = System.Drawing.Color;

namespace AE.Visualization.SchemeObjectModel
{
	//public interface IDataStream
	//{
	//    void ReadData(DataNode iNode);
	//    void WriteData(DataNode iNode);

	//    void ReadData(System.IO.BinaryReader iStream);
	//    void WriteData(System.IO.BinaryWriter iStream);
	//}
	
	//public class TestObject : SchemeObject
	//{
	//    public new double   Scale {get {return this.Matrix.ExtractScale().X;}}
	//    public new Matrix4d Matrix;
	//}
	//public class SchemeNode : SchemeObject{}
	//public class SchemeObject
	//{
		
	//}
	//public class SchemePoint : SchemeObject, ISelectable
	//{
	//    public virtual Vector3d Position {get;set;}


	//    #region Члены ISelectable

	//    public virtual bool IsSelected {get;set;}
		
	//    #endregion
	//}
	public class SchemeNode : IVisualObject, IEventSystemMember, ISelectable
	{
		public string            Name;
		public int Color         {get;set;}
		//public SchemeObjectType  Type;
		//public Node             Parent;
		//public Scheme           Scheme;

		public Vector3d Position {get{return this.Matrix.ExtractTranslation();}                                                             set{this.UpdateMatrix(value,         this.Rotation, this.Scale);}}
		public double   Rotation {get{var _AxisA = this.Matrix.ExtractRotation(true).ToAxisAngle(); return _AxisA.W * Math.Sign(_AxisA.Z);} set{this.UpdateMatrix(this.Position, value,         this.Scale);}}
		public double   Scale    {get{return this.Matrix.ExtractScale().X;}                                                                 set{this.UpdateMatrix(this.Position, this.Rotation, value);     }}

		public Matrix4d          Matrix;
		public Matrix4d          MatrixInverted;
		//public Matrix4d          MatrixGlobal;
		//public Matrix4d         MatrixInv;


		public void             UpdateMatrix(Vector3d iPosition, double iRotation, double iScale)
		{
			//this.Matrix = Matrix4d.Identity * Matrix4d.Scale(iScale,iScale,1.0) * Matrix4d.RotateZ(iRotation) * Matrix4d.CreateTranslation(iPosition);
			this.Matrix = Matrix4d.Identity * Matrix4d.Scale(iScale,iScale,iScale) * Matrix4d.RotateZ(iRotation) * Matrix4d.CreateTranslation(iPosition);
			this.MatrixInverted = this.Matrix.Inverted();
		}
		public void             UpdateGlobalMatrix(Matrix4d iParentMatrix)
		{
			//this.MatrixGlobal = this.
		}

		public virtual void UpdateTransform()
		{

		}
		//public Vector3d         Position;
		//public double           Rotation;
		//public double           Scale;

		//public Matrix4d Matrix
		//{
		//    get
		//    {
		//        var oMat = Matrix4d.Identity;
		//        {
		//            oMat *= Matrix4d.Scale(Math.Pow(2,this.Scale));
		//            oMat *= Matrix4d.RotateZ(this.Rotation);
		//            oMat *= Matrix4d.CreateTranslation(this.Position);
		//        }
		//        return oMat;
		//    }
		//}



		//public bool CanMagnify = true;

		//public Matrix4d         Matrix;
		//public Matrix4d         GlobalMatrix;
		public Viewpoint2D      Viewpoint;// {get;set;}
		public Vector3d         Pointer;// {get;set;}
		public bool             IsPointerOver;
		//public bool             IsActiveObject;
		//public bool             IsTransformed;
		public bool    IsSelected {get;set;}

		public ColorPalette     Palette;
		public SchemeGeometry   Geometry;
		public Image            Image;
		//public Color            Color;


		public delegate void SchemeEventHandler  (Visualization.GenericEventArgs iEvent);
		public delegate void MouseEventHandler   (Visualization.MouseEventArgs   iEvent);
		public delegate void KeyEventHandler     (Visualization.KeyEventArgs     iEvent);

		public event MouseEventHandler MouseClick;
		public event MouseEventHandler MouseDoubleClick;
		public event MouseEventHandler MouseDown;
		public event MouseEventHandler MouseUp;
		public event MouseEventHandler MouseMove;

		public SchemeNode()
		{
			this.Name         = "";
			//this.Type         = SchemeObjectType.Unknown;
			//this.Parent       = null;
			this.Palette      = new ColorPalette();
			this.Color        = 2;//Color.FromArgb(128,128,128);
			///this.Matrix       = Matrix4d.Scale(new Vector3d(1.0,1.0,1.0));
			this.Matrix       = Matrix4d.Identity;

			this.Viewpoint = new Viewpoint2D(null);


			//this.ContactPoints = new ContactPoint.Collection();

			//this.MouseDown = new SchemeEventHandler(
			//this.GlobalMatrix = new Matrix4d();
			
			//this.Geometry = new SchemeGeometry();
			//this.

		}
		
		//public virtual void UpdateProjections   ()
		//{
		//    var _IsChild = this.Parent != null;

		//    if(_IsChild) this.Pointer = Vector3d.Transform(this.Parent.Pointer, this.Matrix.Inverted());

		//    this.IsPointerOver = Math.Abs(this.Pointer.X) <= 1.0 && Math.Abs(this.Pointer.Y) <= 1.0;
			
		//    if(_IsChild && this.IsPointerOver) this.Parent.IsPointerOver = false;
		//}
		public virtual void UpdatePointer(Vector3d iPointer)
		{
			//this.Pointer = new Vector3d(iPointer.X, iPointer.Y, 0.0);
			
			this.Pointer = Vector3d.Transform(iPointer, this.MatrixInverted);
		}
		public virtual void UpdateViewpoint(Viewpoint2D iViewpoint)
		{
			this.Viewpoint.Frame = iViewpoint.Frame;
			//this.Viewpoint.Position    = Vector3d.Transform(iViewpoint.Position, this.MatrixInverted);
			//this.Viewpoint.Position.Z  *= iViewpoint.Position.Z * this.Scale;

			this.Viewpoint.Position   = Vector3d.Transform(iViewpoint.Position, this.MatrixInverted);
			//this.Viewpoint.Position.Z /= this.Scale;

			this.Viewpoint.Inclination = this.Rotation - iViewpoint.Inclination;
		}
		//public virtual void UpdateViewpoint(Viewpoint2D iViewpoint)
		//{
		//    this.Viewpoint.Position   = Vector3d.Transform(iViewpoint.Position, this.MatrixInverted);
		//    this.Viewpoint.Position.Z *= this.Scale;

		//    this.Viewpoint.Inclination = this.Rotation - iViewpoint.Inclination;
		//}
		
		public virtual void UpdateProjections   (bool iDoReset)
		{
			///var _IsChild = this.Parent != null;

			///if(_IsChild) this.Pointer = Vector3d.Transform(this.Parent.Pointer, this.MatrixInv);

			this.IsPointerOver = CheckPointerIsOver();
			
			///if(this.Parent != null && this.IsPointerOver) this.Parent.IsPointerOver = false;
		}
		//public virtual void UpdatePointer
		protected virtual bool CheckPointerIsOver()
		{
			return Math.Abs(this.Pointer.X) <= 1.0 && Math.Abs(this.Pointer.Y) <= 1.0;
		}

		//public virtual void UpdateProjections ()
		//{
		//    this.Pointer = Vector3d.Transform(this.Parent.Pointer, this.Matrix.Inverted());
			
		//    this.IsPointerOver = Math.Abs(this.Pointer.X) <= 1.0 && Math.Abs(this.Pointer.Y) <= 1.0;
			
		//    if(this.IsPointerOver && this.Parent != null)
		//    {
		//        this.Parent.IsPointerOver = false;
		//    }
		//}
		
		//public virtual void UpdateContactPoints ()
		//{
		//    //var _ContactPoints = new ContactPoint.Collection();
		//    //{
		//    //    foreach(var cNode in this.Child)
		//    //    {
		//    //        var cNGon = cNode as NGonNode;
		//    //        if(cNGon == null) continue;
		//    //        if((this.Scheme.Frame as NGonSchemeFrame).IsDragging && cNGon.IsActiveObject) continue;
					
		//    //        foreach(var cVertex in cNGon.Shape.Vertices)
		//    //        {
		//    //            var cContactP = new ContactPoint(Vector3d.Transform(cVertex, cNGon.Matrix));
		//    //            cContactP.Nodes.Add(cNGon);
						
		//    //            _ContactPoints.Add(cContactP);
		//    //        }
		//    //    }
		//    //}
		//    ////if(this.Parent != null)
		//    ////{
		//    ////    //var _PareScheme = this.Parent as NGonScheme;

		//    ////    //_PareScheme.ContactPoints.AddRange(_ContactPoints);
		//    ////    //else
		//    ////    //{

		//    ////    //}
		//    ////}
				
		//    ////if(
		//    //(this.Parent as IHasContactPoints).AddContactPoints(_ContactPoints);


		//    //foreach(var cNode in this.Children)
		//    //{
		//    //    cNode.UpdateContactPoints();
		//    //}
		//}
		public virtual void Transform           ()
		{
			//GL.Translate(this.Position);///.MultMatrix(ref this.Matrix);
			//GL.Rotate(this.Rotation / Math.PI * 180, Vector3d.UnitZ);///.MultMatrix(ref this.Matrix);
			//GL.Scale(new Vector3d(this.Scale, this.Scale, 1.0));

			//var _Mat = Matrix4d.Identity;
			//{
			//    _Mat *= Matrix4d.Scale(this.Scale);
			//    _Mat *= Matrix4d.RotateZ(this.Rotation);
			//    _Mat *= Matrix4d.CreateTranslation(this.Position);
			//}

			

			var _Mat = this.Matrix;
			GL.MultMatrix(ref _Mat);
			///Workarounds.Performance.TotalMatrixTransforms++;
		}
		public virtual void Draw                ()
		{
			///this.UpdateProjections();
			
			Routines.DrawFrame(this);
			Routines.DrawShapes(this);
			//Routines.DrawMouse(this);

			//if(this.Geometry != null)
			//{
			//    Routines.DrawShapes(this);
			//    //foreach(var cShape in this.Geometry.Shapes)
			//    //{
					
			//    //}
			//}
			
			//foreach(var cObject in this.C
		}
		
		public virtual void UpdatePalette       (bool iIsLightTheme)
		{
			//throw new NotImplementedException();
			this.Palette.Update(iIsLightTheme);
		}
		public virtual void ProcessEvent        (Visualization.GenericEventArgs iEvent)
		{
			switch(iEvent.Type)
			{
				case EventType.MouseClick:       if(this.MouseClick       != null) this.MouseClick       ((MouseEventArgs)iEvent); break;
				case EventType.MouseDoubleClick: if(this.MouseDoubleClick != null) this.MouseDoubleClick ((MouseEventArgs)iEvent); break;
				case EventType.MouseDown:        if(this.MouseDown        != null) this.MouseDown        ((MouseEventArgs)iEvent); break;
				case EventType.MouseUp:          if(this.MouseUp          != null) this.MouseUp          ((MouseEventArgs)iEvent); break;
				case EventType.MouseMove:        if(this.MouseMove        != null) this.MouseMove        ((MouseEventArgs)iEvent); break;

				default : throw new NotImplementedException();
			}
			//_Event.
			///Console.WriteLine(this.GetType().Name + ".ProcessEvent: " + iEvent.Type);
			//switch(_Event.Type)
			//{
			//    case EventType.Click:
			//    case EventType.MouseDown: this.MouseDown(null,new EventArgs());
			//    case EventType.MouseUp:
			//    {
					
					
			//        break;
			//    }
			//}
		}
		//public virtual void WriteData (){}
		//public virtual void ReadData  (){}


		public virtual DataNode WriteNode (DataNode iNode)
		{
			var oNode = iNode ?? new DataNode("SchemeObject");
			{
				if(!String.IsNullOrEmpty(this.Name)) oNode["@name"] = this.Name;

				oNode["@position"] = this.Position.X + "," + this.Position.Y + "," + this.Position.Z;
				oNode["@rotation"] = this.Rotation;
				oNode["@scale"]    = this.Scale;
				oNode["@color"]    = this.Color;
			}
			return oNode;
		}
		public virtual void     ReadNode  (DataNode iNode)
		{
			this.Name     = iNode["@name"] ?? "";

			var _PosStrPP = ((string)iNode["@position"] ?? "0,0,0").Split(' ',',');
			this.Position = new Vector3d(Double.Parse(_PosStrPP[0]),Double.Parse(_PosStrPP[1]),Double.Parse(_PosStrPP[2]));

			
			this.Rotation = iNode["@rotation"] ?? 0.0;
			this.Scale    = iNode["@scale"]    ?? 1.0;
			this.Color    = iNode["@color"]    ?? 4; this.Color %= ColorPalette.DefaultColors.Length;
		
			
			//oNode["@rotation"]; = this.Rotation;
			//oNode["@scale"];    = this.Scale;
			//oNode["@color"];    = this.Color;

			//throw new NotImplementedException();
		}
		//public virtual string WriteString         ()
		//{
		//    throw new NotImplementedException();
		//}
		//public virtual void   ReadString          (string iStr)
		//{
		//    throw new NotImplementedException();
		//}
		
		public struct Routines
		{
			public static void DrawFrame          (SchemeNode iNode)
			{
				var _IsActive    = iNode.IsSelected;
				var _IsMouseOver = iNode.IsPointerOver;


				//if(!_IsActive && !_IsMouseOver)  return;

				GL.LineWidth(_IsActive ? 3 : (_IsMouseOver ? 1 : 0));


				//if() 
				//else GL.LineWidth(1);
				
				GL.Begin(PrimitiveType.Lines);
				{
					GL.Color4(iNode.Palette.Colors[iNode.Color]);

					GL.Vertex2(-0.8, -0.8);
					GL.Vertex2(+0.8, +0.8);
					GL.Vertex2(+0.8, -0.8);
					GL.Vertex2(-0.8, +0.8);
				}
				GL.End();

				GL.Begin(PrimitiveType.LineLoop);
				{
					GL.Color4(iNode.Palette.Colors[iNode.Color]);

					GL.Vertex2(-1.0, -1.0);
					GL.Vertex2(+1.0, -1.0);
					GL.Vertex2(+1.0, +1.0);
					GL.Vertex2(-1.0, +1.0);
				}
				GL.End();
			}
			public static void DrawPointer        (SchemeNode iNode)
			{
				var _P = iNode.Pointer;

				//GL.LineWidth(1);
				//GL.Begin(PrimitiveType.LineLoop);
				//{
				//    for(var cA = 0.0; cA < Math.PI * 2; cA += Math.PI / 24)
				//    {
				//        GL.Vertex2(_P.X + Math.Sin(cA) * 0.1, _P.Y + Math.Cos(cA) * 0.1);
				//    }
				//}
				//GL.End();

				GL.LineWidth(1);
				GL.Begin(PrimitiveType.Lines);
				{
				    GL.Color4(iNode.Palette.Colors[iNode.Color]);

					
				    GL.Vertex2(0,0);
				    GL.Vertex2(_P.X,_P.Y);

				    GL.Vertex2(_P.X - 0.1, _P.Y);
				    GL.Vertex2(_P.X + 0.1, _P.Y);

				    GL.Vertex2(_P.X, _P.Y - 0.1);
				    GL.Vertex2(_P.X, _P.Y + 0.1);
					
				}
				GL.End();
			}
			public static void DrawViewpoint      (SchemeNode iNode)
			{
				/**
					Z=_ViewP.Z;
					W

				
				*/
				var _V = iNode.Viewpoint.Position;
				var _Size = (_V.Z) + ((new Random().NextDouble() - 0.5) * 0.002);
				//var _Size = (_V.Z * 0.5) + ((new Random().NextDouble() - 0.5) * 0.002);
				//iNode.
				//var _V = iNode.Vi;
				GL.LineWidth(2);
				GL.Begin(PrimitiveType.Lines);
				{
					GL.Color4(iNode.Palette.Colors[iNode.Color]);

					GL.Vertex2(0,0);
					GL.Vertex2(_V.X,_V.Y);
				}
				GL.End();

				GL.PushMatrix();
				GL.Translate(_V.X,_V.Y,0.0);
				GL.Rotate(iNode.Viewpoint.Inclination * MathEx.RTD, 0,0,1);
				{
					
					GL.Begin(PrimitiveType.LineLoop);
					{
						GL.Vertex2(-_Size,-_Size);
						GL.Vertex2(+_Size,-_Size);
						GL.Vertex2(+_Size,+_Size);
						GL.Vertex2(-_Size,+_Size);

						//GL.Vertex2(_V.X - _Size,_V.Y - _Size);
						//GL.Vertex2(_V.X + _Size,_V.Y - _Size);
						//GL.Vertex2(_V.X + _Size,_V.Y + _Size);
						//GL.Vertex2(_V.X - _Size,_V.Y + _Size);

					}
					GL.End();
				}
				GL.PopMatrix();
			}
			public static void DrawShapes         (SchemeNode iNode)
			{
				if(iNode.Geometry == null) return;

				GL.LineWidth(iNode.IsSelected ? 5 : iNode.IsPointerOver ? 3 : 1);

				foreach(var cShape in iNode.Geometry.Shapes)
				{
					DrawShape(iNode, cShape, 0.5f, 3f, iNode.Palette.Colors[iNode.Color]);
				}
			}
			public static void DrawShape          (SchemeNode iNode, SchemeShape iShape, float iOpacity, float iLineWidth, Color iColor)
			{
				//var _IsActive    = iNode == iNode.Scheme.Viewport.Get;
				//var _IsMouseOver = iNode.IsPointerOver;


				//if(!_IsActive && !_IsMouseOver)  return;






				GL.Begin(PrimitiveType.Polygon);
				{
					//iOpacity = 1;
					//GL.Color4(SDColor.Transparent);
					GL.Color4(SDColor.FromArgb((int)(iOpacity * 127), iColor));
					GL.Vertex3(0,0,0);
					GL.Color4(SDColor.FromArgb((int)(iOpacity * 127), iColor));

					var _VV = iShape.Vertices;
					for(var cI = -1; cI < _VV.Length; cI++)
					{
						var cVi = cI == -1 ? _VV.Length - 1 : cI;
					    //if(cVi == 0) 
					    //{
					    //    //GL.Color4(SDColor.FromArgb((int)(iOpacity * ((float)(cVi + 1) / _VV.Length) * 255), iColor));
					    //}
					    //else
					    //{
					    //    GL.Color4(SDColor.FromArgb((int)(iOpacity * ((float)(cVi + 1) / _VV.Length) * 255), iColor));
					    //}
					    //GL.Color4(SDColor.FromArgb((int)(iOpacity * ((float)(cVi + 1) / _VV.Length) * 255), iColor));
						var cVertex = _VV[cVi];
						var cAngle = Math.Atan2(cVertex.Y,cVertex.X);
						var cOpacity = Math.Abs(cAngle / Math.PI);


 						GL.Color4(SDColor.FromArgb((int)(iOpacity * cOpacity * 255), iColor));
						GL.Vertex3(cVertex);
					}
					//GL.Vertex3(iShape.Vertices[0]);


					//foreach(var cVertex in iShape.Vertices)
					//{
					//    var cAngle = Math.Atan2(cVertex.Y,cVertex.X);
					//    var cOpacity = Math.Abs(cAngle / Math.PI);

					//    GL.Color4(SDColor.FromArgb((int)(iOpacity * cOpacity * 255), iColor));
					//    GL.Vertex3(cVertex);
					//}
					//GL.Vertex3(iShape.Vertices[iShape.Vertices.Length - 1]);
					
				}
				GL.End();


				if(iLineWidth != 0)
				{
					GL.LineWidth(iLineWidth);
					GL.Begin(PrimitiveType.LineLoop);
					{
						//GL.Color4(SDColor.FromArgb(64, iNode.Palette.Colors[iNode.Color]));
						GL.Color4(iNode.Palette.Colors[iNode.Color]);
						foreach(var cVertex in iShape.Vertices) GL.Vertex3(cVertex);
					}
					GL.End();
				}

				if(iNode.IsPointerOver)
				{
					//GL.PointSize(10f);
					//GL.Begin(PrimitiveType.Points);
					//{
					//    for(var cVi = 0; cVi < iShape.Vertices.Length; cVi++)
					//    {
					//        GL.Color4(SDColor.FromArgb((int)(255 * (1 - ((float)cVi / iShape.Vertices.Length))), iNode.Palette.Colors[5]));

					//        GL.Vertex3(iShape.Vertices[cVi]);
					//    }
						
						
					//}
					//GL.End();
				}

				if(iNode.IsPointerOver)
				{
					//DrawPointsOnShape(iNode, iShape, Math.Atan2(iNode.Pointer.Y,iNode.Pointer.X));
				}

				//GL.PointSize(5f);
				//GL.Begin(PrimitiveType.Points);
				//{
				//    GL.Color4(Screen.Colors[iNode.Color]);
				//    foreach(var cVertex in iShape.Vertices) GL.Vertex2(cVertex);
				//}
				//GL.End();
			}
		}
	}

	
	//public class SchemeObjectCollection : List<SchemeObject>
	//{
	//    public Node Owner;
	//    //public SchemeObject Scheme;

	//    public SchemeObjectCollection(Node iOwner)
	//    {
	//        this.Owner  = iOwner;
	//        //this.Scheme = iOwner.Scheme;
	//    }
	//    public new void Add(SchemeObject iObj)
	//    {
	//        this.LinkChild (iObj);
	//        base.Add       (iObj);
	//    }
	//    public void LinkChild(SchemeObject iObj)
	//    {
	//        iObj.Parent = this.Owner;
	//        //iObj.Scheme = this.Owner.Scheme;

	//        if(iObj is Node)
	//        {
	//            var _Children = ((Node)iObj).Children;

	//            foreach(var cChildO in _Children)
	//            {
	//                _Children.LinkChild(cChildO);
	//            }
	//        }
			
	//    }
	//    //public void LinkScheme(Node iObj, bool iIsRecursive)
	//    //{
			
	//    //}
	//}
	//public enum SchemeObjectType
	//{
	//    Unknown,

	//    Node,
	//    Link,
	//    Port,
	//    CustomNode,
	//}

	public class SchemeShape// : IDataNode
	{
		public Vector3d[] Vertices;

		//public void ReadData(DataNode iNode){}
		public SchemeShape()
		{
		
		}
		public SchemeShape(PointF[] iVertices)
		{
			this.Vertices = new Vector3d[iVertices.Length];

			for(var cVi = 0; cVi < iVertices.Length; cVi++)
			{
				var cPoint = iVertices[cVi];
				this.Vertices[cVi] = new Vector3d(cPoint.X, cPoint.Y, 0.0);
			}
		}
	}
	public class SchemeShapeCollection : List<SchemeShape>
	{

	}
	public class SchemeGeometry
	{
		public SchemeShapeCollection Shapes;

		public SchemeGeometry()
		{
			this.Shapes = new SchemeShapeCollection();
		}
	}

	

	//public class GroupNode : SchemeNode
	//{

	//}
	//public class Node : SchemeObject
	//{
	//    public SchemeObjectCollection Children;

	//    public Node()
	//    {
	//        this.Children = new SchemeObjectCollection(this);
	//    }

	//    public override void UpdateProjections()
	//    {
	//        //return;
	//        base.UpdateProjections();

	//        foreach(var cChildN in this.Children)
	//        {
	//            cChildN.UpdateProjections();
	//        }
	//    }
	//    public override void UpdatePalette()
	//    {
	//        base.UpdatePalette();

	//        foreach(var cChildNode in this.Children)
	//        {
	//            cChildNode.UpdatePalette();
	//        }
	//    }
	//    //public override void UpdateContactPoints()
	//    //{
	//    //    ////base.UpdateContactPoints();

	//    //    //var _ContactPoints = new ContactPoint.Collection();
	//    //    //{
	//    //    //    foreach(var cNode in this.Children)
	//    //    //    {

	//    //    //    }
	//    //    //}
	//    //}
	//    public override void Draw()
	//    {
	//        base.Draw();

	//        foreach(var cNode in this.Children)
	//        {
	//            GL.PushMatrix();
	//            {
	//                cNode.Transform ();
	//                cNode.Draw      ();
	//            }
	//            GL.PopMatrix();
	//        }
	//        //this.Dra
	//    }

	//    public override void ProcessEvent(Visualization.GenericEventArgs iEvent)
	//    {
	//        base.ProcessEvent(iEvent);

	//        foreach(var cChildN in this.Children)
	//        {
	//            if(cChildN.IsPointerOver)
	//            {
	//                cChildN.ProcessEvent(iEvent);
	//            }
	//        }
	//    }
	
	//    //public DataNode WriteToNode()
	//    //{
	//    //    var oNode = new DataNode();

	//    //}
	//}
	//public class Link : SchemeNode{}
	//public class Port : SchemeObject{}
	
	//public class Scheme : Node
	//{
	//    //pub
	//    //public string Path;
	//    public Visualization.SchemeFrame Frame;
	//    //public VertexManager

	//    public Scheme() : base()
	//    {
	//        this.Scheme = this;
	//        //this.
	//    }

	//    public new struct Routines
	//    {
	//        //public static Scheme GenerateDefaultScheme()
	//        //{
	//        //    var oScheme = new Scheme();
	//        //    {
	//        //        var _Child1 = new Node();
	//        //        {
	//        //            _Child1.Color = 2;//SDColor.Red;

	//        //            //_Child1.Geometry = new SchemeGeometry

	//        //            _Child1.Matrix = 
	//        //            (
	//        //                Matrix4d.Scale(new Vector3d(0.5,0.5,1.0))   *
	//        //                Matrix4d.Rotate(new Vector3d(0,0,1.0), 3.1415) *
	//        //                Matrix4d.CreateTranslation(0.5,0.5,0)
	//        //            );
	//        //            _Child1.Geometry = new SchemeGeometry();
	//        //            var _Shape = new SchemeShape();
	//        //            {
	//        //                _Shape.Vertices = new Vector2d[]
	//        //                {
	//        //                    new Vector2d(-3.0,-2.0),
	//        //                    new Vector2d(+1.0,-2.0),
	//        //                    new Vector2d(+3.0,-1.0),
	//        //                    new Vector2d(+4.0,+2.0),
	//        //                    new Vector2d(-1.0,+3.0),
	//        //                    new Vector2d(-5.0,+1.0),
	//        //                };
	//        //            }
	//        //            _Child1.Geometry.Shapes.Add(_Shape);
	//        //        }
	//        //        var _Child2 = new Node();
	//        //        {
	//        //            _Child2.Color = 5;//SDColor.FromArgb(0,180,255);

	//        //            _Child2.Matrix = 
	//        //            (
	//        //                Matrix4d.Scale(new Vector3d(0.5,0.5,1.0))   *
	//        //                Matrix4d.Rotate(new Vector3d(0,0,1.0), 2.0) *
	//        //                Matrix4d.CreateTranslation(-0.5,-0.5,0)
	//        //            );

	//        //            var _SubChild1 = new Node();
	//        //            {
	//        //                _SubChild1.Color = 4;//SDColor.FromArgb(160,200,0);

	//        //                _SubChild1.Matrix = 
	//        //                (
	//        //                    Matrix4d.Scale(new Vector3d(0.5,0.5,1.0))      *
	//        //                    Matrix4d.Rotate(new Vector3d(0,0,1.0), 3.1415) *
	//        //                    Matrix4d.CreateTranslation(0.5,0.5,0)
	//        //                );
	//        //            }
						

	//        //            var _SubChild11 = new Node();
	//        //            {
	//        //                _SubChild11.Color = 3;//SDColor.FromArgb(255,100,0);

	//        //                _SubChild11.Matrix = 
	//        //                (
	//        //                    Matrix4d.Scale(new Vector3d(0.5,0.5,1.0))   *
	//        //                    Matrix4d.Rotate(new Vector3d(0,0,1.0), 2.0) *
	//        //                    Matrix4d.CreateTranslation(+0.25,+0.25,0)
	//        //                );
	//        //            }
	//        //            _SubChild1.Children.Add(_SubChild11);

						

	//        //            var _SubChild2 = new Node();
	//        //            {
	//        //                _SubChild2.Color = 4;//SDColor.FromArgb(255,100,0);

	//        //                _SubChild2.Matrix = 
	//        //                (
	//        //                    Matrix4d.Scale(new Vector3d(0.5,0.5,1.0))   *
	//        //                    Matrix4d.Rotate(new Vector3d(0,0,1.0), 2.0) *
	//        //                    Matrix4d.CreateTranslation(+0.25,+0.25,0)
	//        //                );
	//        //            }

	//        //            _Child2.Children.Add(_SubChild2);
	//        //            _Child2.Children.Add(_SubChild1);
						
	//        //        }
					
	//        //        //_Child2

					
	//        //        oScheme.Children.Add(_Child2);
	//        //        oScheme.Children.Add(_Child1);
	//        //    }
	//        //    return oScheme;
	//        //}
	//        //public static Scheme GenerateRandomScheme()
	//        //{
	//        //    var oScheme = new Scheme();
	//        //    {
	//        //        var _RNG = new Random(0);

	//        //        //Node cNode = null, pNode = GenerateNode(4, _RNG), nNode = null;
	//        //        for(var cNi = 0; cNi < 10; cNi ++)
	//        //        {
	//        //            var cNode = GenerateNode(_RNG.Next(3, 8), 2, _RNG);
	//        //            oScheme.Children.Add(cNode);
	//        //        }
	//        //    }
	//        //    return oScheme;
	//        //}
	//        public static Scheme GenerateNGonsScheme(Scheme iScheme)
	//        {
	//            var oScheme = iScheme ?? new Scheme();
	//            {
	//                var _RNG = new Random();

	//                //Node cNode = null, pNode = GenerateNode(4, _RNG), nNode = null;
	//                var _AvNGons = new int[]{3,4,6,8,12};

	//                for(int cNi = 0; cNi <= 100; cNi ++)
	//                {
	//                    var cEdgeCount = _AvNGons[cNi % _AvNGons.Length];
					
	//                    //var cNode = new GdiNGonNode(_RNG.Next(3, 10), 4);
	//                    var cNode = new NGonNode(cEdgeCount, 4);
	//                    {
	//                        cNode.Position = new Vector3d((_RNG.NextDouble() - 0.5) * 300,(_RNG.NextDouble() - 0.5) * 300, 0.0);
	//                        ///cNode.Matrix *= Matrix4d.CreateTranslation((_RNG.NextDouble() - 0.5) * 300,(_RNG.NextDouble() - 0.5) * 300,0.0);
							
	//                        cNode.Color = _RNG.Next(2,Screen.DefaultColors.Length);
	//                    }

	//                    //var cNode = GenerateNode(_RNG.Next(3, 8), 2, _RNG);
	//                    oScheme.Children.Add(cNode);
	//                }
	//            }
	//            return oScheme;
	//        }
	//        //public static Node   GenerateNode(int iEdgeCount, int iSubLevels, Random iRNG)
	//        //{
	//        //    //throw new NotImplementedException();
	//        //    var oNode = new NGonNode();
	//        //    {
	//        //        oNode.Color = 1 + iRNG.Next(Screen.Colors.Length - 2);

					
	//        //        oNode.Matrix = 
	//        //        (
	//        //            Matrix4d.Scale(new Vector3d(0.1,0.1,1.0))   *
	//        //            //Matrix4d.Rotate(new Vector3d(0,0,1.0), 0) *
	//        //            Matrix4d.CreateTranslation(1.0 - (iRNG.NextDouble() * 2.0), 1.0 - (iRNG.NextDouble() * 2.0),0)
	//        //        );
	//        //        oNode.Geometry = new SchemeGeometry();
	//        //        {
	//        //            var _Shape = new SchemeShape();
	//        //            {
	//        //                _Shape.Vertices = NGonNode.GenerateShape(iRNG.Next(3, 8));//iEdgeCount);
	//        //            }
	//        //            oNode.Geometry.Shapes.Add(_Shape);
	//        //        }

	//        //        if(iSubLevels != 0)
	//        //        {
	//        //            for(var cNi = 0; cNi < 10; cNi ++)
	//        //            {
	//        //                var cChildN = GenerateNode(iEdgeCount, iSubLevels - 1, iRNG);
	//        //                oNode.Children.Add(cChildN);
	//        //            }
	//        //        }
	//        //        //for(var cL = iSubLevels; cL >= 0; cL --)
	//        //        //{
	//        //        //    var cChildN = GenerateNode(iEdgeCount, cL - 1, iRNG);
	//        //        //    oNode.Children.Add(cChildN);
	//        //        //}
	//        //    }
	//        //    return oNode;
	//        //}
		
	//    }

	//    public override void UpdateProjections()
	//    {
			
	//        //base.UpdateProjections();

	//        this.Frame.UpdatePointer();

	//        this.Pointer = this.Frame.Pointer;//new Vector2d(this.Viewport.MousePosition.X, this.Viewport.Mouse.SY);




	//        //return;
	//        foreach(var cChildN in this.Children)
	//        {
	//            cChildN.UpdateProjections();
	//        }
	//    }
	//}
	
	public class ContactPoint
	{
		public Vector3d           Position;
		public List<SchemeNode>   Nodes;

		public ContactPoint(Vector3d iPosition)
		{
			this.Position = iPosition;
			this.Nodes    = new List<SchemeNode>();
		}

		public class Collection : List<ContactPoint>
		{
			
		}
	}
}
