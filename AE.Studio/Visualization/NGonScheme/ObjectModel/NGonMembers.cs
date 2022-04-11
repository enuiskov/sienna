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
	public interface IPropagationPathMember
	{
		//bool IsHighlighting        {get;set;}
		//void PropagateHighlighting {get;set;}

		void PropagateMessage      (DataNode iMessage);
	}

	public class NGonJointGraph
	{
		//public NGonNode           Parent;
		public List<NGonJoint>    Joints; // Node -> Port -> Joint -> Graph -> Joint -> Port -> Node
		//public List<NGonLine>     Lines;
		public List<NGonEdgePort> Ports;

		public NGonJointGraph(NGonJoint iJoint)//(NGonNode iParent)
		{
			//this.Parent = iParent;
			this.Joints = new List<NGonJoint>(){iJoint};
			//this.Lines  = new List<NGonLine>();
			this.Ports  = new List<NGonEdgePort>();
		}

		public void ProcessMessage(string iMessage, NGonEdgePort iSender)
		{
			G.Console.Message("*Message sent");

			foreach(var cPort in this.Ports)
			{
				if(cPort != iSender)
				{
					cPort.ReceiveMessage(iMessage);
				}
			}
		}
		public void MergeWith(NGonJointGraph iOtherGraph)
		{
			var _GraphJoints = new List<NGonJoint>();
			{
				_GraphJoints.AddRange(this.Joints);


				foreach(var cJoint in iOtherGraph.Joints)
				{
					_GraphJoints.Add(cJoint);
				}

				//for(var cJi = 0; cJi < iOtherGraph.Joints.Count; cJi++)
				//{
				//    var cOtherJoint = iOtherGraph.Joints[cJi];

				//    iOtherGraph.Joints.RemoveAt(cJi);

				//    cJoint.Graph = this;
				//}
			}
			for(var cJi = 0; cJi < _GraphJoints.Count; cJi++)
			{
				NGonJoint cxJoint = _GraphJoints[cJi], cyJoint;

				foreach(var cLine in cxJoint.Lines)
				{
					cyJoint = cLine.Joint1; if(cyJoint != cxJoint && !_GraphJoints.Contains(cyJoint)) _GraphJoints.Add(cyJoint);
					cyJoint = cLine.Joint2; if(cyJoint != cxJoint && !_GraphJoints.Contains(cyJoint)) _GraphJoints.Add(cyJoint);
				}
			}
			//while(true)
			//{
				
			//}
			foreach(var cJoint in _GraphJoints)
			{
				 cJoint.Graph = this;
			}

			if(_GraphJoints.Count > 2)
			{
			
			}
			this.Joints = _GraphJoints;
			//{

			
			//for(var cJi = 0; cJi < iOtherGraph.Joints.Count; cJi++)
			//{
			//    var cOtherJoint = iOtherGraph.Joints[cJi];

			//    iOtherGraph.Joints.RemoveAt(cJi);

			//    cJoint.Graph = this;
			//}

			//foreach(var cJoint in iOtherGraph.Joints)
			//{
			//    cJoint.Graph = this;
			
			//    foreach(var cLine in cJoint.Lines)
			//    {
			//        if(cLine.Joint1 != cJoint) this.Joints.Add(cLine
			//    }

			//    if(!this.Joints.Contains(cJoint))
			//    {

			//        this.Joints.Add(cJoint);
			//    }
			//    else
			//    {
				
			//    }
			//}
			//this.Joints.
		}
	}
	
	public enum NGonEdgePortType
	{
		Undefined,

		Input,
		Output
	}
	public class NGonEdgePort : IVisualObject, ISelectable
	{
		public NGonNode Owner;
		public double   Bearing;
		public double   Elevation;
		public bool     IsPointerOver;

		public NGonEdgePortType Type;
		public bool             IsHighlighting;
		public double           Value;

		public Vector3d     InnerEdgePoint;
		public Vector3d     OuterEdgePoint;

		public Vector3d     InnerPinPoint;
		public Vector3d     OuterPinPoint;
		
		public NGonJoint    InnerPin;
		public NGonJoint    OuterPin;

		public NGonEdgePort InnerPort;
		public NGonEdgePort OuterPort;
		
		

		//public Vector3d InnerPin;
		//public Vector3d OuterPin;
		//public Vector3d ЩгеукPinЗщышешщт;

		
		//public NGonJoint ParentPin;


		//public Vector3d OuterPinEx;


		public PinType InPinType;
		public PinType OuPinType;

		public static double InnerPinLength = 0.05;
		public static double OuterPinLength = 0.05;

		public NGonEdgePort()
		{
			//this.InnerJoint = new NGonJoint{Port = this};
			//this.OuterJoint = new NGonJoint{Port = this};
		}

		public void Update(bool iDoCreateJoints, bool iDoLinkPorts)
		{
			var _Owner = this.Owner; if(_Owner == null) return;
			var _Angle = this.Bearing;
			var _ProjR = NGonNode.Routines.Projections.GetSideDistance(_Owner, _Angle);

			this.InnerEdgePoint = new Vector3d(Math.Cos(_Angle), Math.Sin(_Angle), 0.0) * _ProjR;
			this.OuterEdgePoint = Vector3d.Transform(this.InnerEdgePoint, this.Owner.Matrix);
			
			var _SideAngle = Math.Round(_Angle / _Owner.SideAngle) * _Owner.SideAngle;
			var _PinOffs   = new Vector3d(Math.Cos(_SideAngle), Math.Sin(_SideAngle), 0.0);
			
			this.InnerPinPoint  = this.InnerEdgePoint - (_PinOffs * NGonEdgePort.InnerPinLength);
			this.OuterPinPoint  = this.InnerEdgePoint + (_PinOffs * NGonEdgePort.OuterPinLength);

			if(iDoCreateJoints)
			{
				if(this.InnerPin == null)
				{
					this.InnerPin = new NGonJoint{Port = this};
					this.Owner.Joints.Add(this.InnerPin);
					//this.Owner.Joints.Insert(0,this.InnerJoint);
				}
				if(this.OuterPin == null && this.Owner.Parent != null)
				{
					this.OuterPin = new NGonJoint{Port = this};
					this.Owner.Parent.Joints.Add(this.OuterPin);
					//this.Owner.Parent.Joints.Insert(0,this.OuterJoint);
				}
			}
			if(this.InnerPin != null) this.InnerPin.Position = this.InnerPinPoint;
			if(this.OuterPin != null) this.OuterPin.Position = Vector3d.Transform(this.OuterPinPoint, this.Owner.Matrix);
		}

		public void ReceiveMessage(string iMessage)
		{
			G.Console.Message("*Message received");
		}
		public void Draw()
		{
			//return;

			this.Owner.Frame.Profiler.TotalRenderedPorts++;
			//var _Parent = this.Parent;// if(_Parent == null) ?>>
			//var _Angle = this.Bearing;
			//var _ProjR = _Parent.GetSideDistance(_Angle);

			//var _EdgePoint = new Vector2d(Math.Cos(_Angle), Math.Sin(_Angle)) * _ProjR;
			
			//double _SideAngle = Math.Round(_Angle / _Parent.SideAngle) * _Parent.SideAngle;
			//{
			//    //var _CorrAngle     = _Angle + (_Angle < 0 ? Math.PI * 2.0 : 0);		
			//    //var _SegmAbsPos    = _CorrAngle / (_Parent.HalfSideAngle * 2.0);
			//    //var _SegmRelPos    = Math.Abs(_SegmAbsPos - Math.Round(_SegmAbsPos));
			//    //var _SegmPosNorm   = _SegmRelPos * 2.0;
			
				

			//    //_SideIndex) *;
			//}
			
			////var _SideAngle = Math.Round(_Angle / (2.0 * Math.PI / _Parent.Sides) + (_Parent.Sides == 3 ?  : 0));
			////var _Si
			////var _I
			//var _PinOffs = new Vector2d(Math.Cos(_SideAngle), Math.Sin(_SideAngle)) * 0.05;
			//var _InnerPin  = _EdgePoint + _PinOffs;
			//var _OuterPin  = _EdgePoint - _PinOffs;
			
			//var _PinPerpV = new Vector3d((this.InnerPin - this.EdgePoint).Xy.PerpendicularRight);

			GL.Color4(this.Owner.Palette.Colors[this.Color]);

			GL.LineWidth(this.IsHighlighting || this.IsSelected ? 8f : this.IsPointerOver ? 4f : 1f);
			GL.Begin(PrimitiveType.Lines);
			{
				if(this.InnerPort == null){GL.Vertex3(this.InnerEdgePoint); GL.Vertex3(this.InnerPinPoint);}
				if(this.OuterPort == null){GL.Vertex3(this.InnerEdgePoint); GL.Vertex3(this.OuterPinPoint);}
				

				//GL.Vertex3(this.InnerPin - _PinPerpV);
				//GL.Vertex3(this.InnerPin + _PinPerpV);

				//GL.Vertex3(this.InnerPin - _PinPerpV);
				//GL.Vertex3(this.InnerPin + _PinPerpV);
				

				//GL.Vertex3(this.InnerPin );
				//GL.Vertex3(this.OuterPin);
			}
			GL.End();

			
			GL.PointSize(this.IsSelected ? 8f : this.IsPointerOver ? 8f : 5f );/// / (float)this.Parent.Viewpoint.Position.Z);
			GL.Begin(PrimitiveType.Points);
			{
				GL.Vertex3(this.InnerEdgePoint);

				if(this.IsPointerOver || this.IsSelected)
				{
					GL.Vertex3(this.InnerPinPoint);
					GL.Vertex3(this.OuterPinPoint);
				}
			}
			GL.End();



			if(this.Type != NGonEdgePortType.Undefined)
			{
				var _IsInputType = this.Type == NGonEdgePortType.Input;

				GL.Color4(this.Owner.Palette.Colors[_IsInputType ? 4 : 9]);
				//GL.Color4(System.Drawing.Color.FromArgb(127, this.Parent.Palette.Colors[2]));
				
				GL.Begin(PrimitiveType.Triangles);
				{
					var _CenP  = this.InnerEdgePoint;///Vector3d.Lerp(this.EdgePoint, this.InnerPoint, 0.5);

					var _BegP  = this.InnerEdgePoint - (_IsInputType ? this.OuterPinPoint : this.InnerPinPoint);
					var _BegA  = (Math.PI / 2) - Math.Atan2(_BegP.Y, _BegP.X);
					var _Scale = this.Owner.Viewpoint.Position.Z * 0.015 * (this.Owner.IsPointerOver ? 2.0 : this.IsSelected ? 1.5 : 1.0);
					
					for(var cRelA = 0.0; cRelA < Math.PI * 2; cRelA += Math.PI / 1.5)
					{
						var cAbsA = _BegA + cRelA;
						
						var cTriaPoint = new Vector3d(Math.Sin(cAbsA) * _Scale, Math.Cos(cAbsA) * _Scale, 0.0);

						GL.Vertex3(_CenP + cTriaPoint);
					}
				}
				GL.End();

			}
		}
		
		public enum PinType
		{
			NoPin,
			Link,

			TwoWay1,
			TwoWay2,
			TwoWay3,

			In1,
			In2,
			In3,

			Out1,
			Out2,
			Out3,
		}
		public class Collection : List<NGonEdgePort>
		{
			public NGonNode Owner;

			public Collection(NGonNode iOwner)
			{
				this.Owner  = iOwner;
			}

			public new void Add(NGonEdgePort iItem)
			{
				iItem.Owner = this.Owner;

				iItem.Update(false,false);

				if(this.Find(_Item => _Item.Bearing == iItem.Bearing) == null)
				///if(this.Find(_Item => Math.Abs(_Item.Bearing - iItem.Bearing) < 0.1) == null)
				{
					base.Add(iItem);
				}
				///else throw new WTFE();
			}
		}

		#region Члены IVisualObject
		public int  Color {get;set;}

		public void Transform()
		{
			//throw new NotImplementedException();

		}

		public void UpdatePalette(bool iIsLightTheme)
		{
			throw new NotImplementedException();
		}

		public void UpdatePointer(Vector3d iPointer)
		{
			throw new NotImplementedException();
		}

		public void UpdateViewpoint(Viewpoint2D iViewpoint)
		{
			throw new NotImplementedException();
		}

		public void UpdateProjections(bool iDoReset)
		{
			throw new NotImplementedException();
		}

		#endregion



		#region Члены ISelectable

		public bool IsSelected{get;set;}

		#endregion
	}
	public class NGonJoint : IVisualObject, ISelectable
	{
		public NGonNode Parent;
		public Vector3d Position;
		public bool     IsHighlighting;
		public double   Value;

		//public NGonLine     Line;
		public NGonJointGraph Graph;
		public List<NGonLine> Lines;
		public NGonEdgePort   Port;

		public bool     IsPointerOver;


		//public bool     IsSelected;

		public NGonJoint()
		{
			this.Graph = new NGonJointGraph(this);
			this.Lines = new List<NGonLine>();
		}
		public void Draw()
		{
			//return;

			this.Parent.Frame.Profiler.TotalRenderedJoints++;
			//GL.PointSize(this.IsPointerOver ? 10f : (this.IsSelected ? 5f : 5f));
			GL.PointSize(this.IsPointerOver ? 5f : (this.IsSelected ? 5f : 5f));

			//if(this.IsSelected)
			//{
				
			//}
			//GL.Color4(this.IsSelected ? this.Parent.Palette.Colors[4] : this.Parent.Palette.Colors[2]);
			///GL.Color4(this.Parent.Palette.Colors[this.IsSelected ? 4 : 2]);
			if(this.Parent != null) GL.Color4(this.Parent.Palette.Colors[this.IsSelected ? 3 : 10]);

			if(this.IsHighlighting)
			{
				GL.PointSize(10f);
				GL.Color4(this.Parent.Palette.Colors[6]);
			}
			
			GL.Begin(PrimitiveType.Points);
			{
			    GL.Vertex3(this.Position);
			}
			GL.End();


			if(this.Parent != null && this.IsPointerOver)
			{
				var _CrossSize = this.Parent.Viewpoint.Position.Z * 0.03;
				var _CrossInc = this.Parent.Viewpoint.Inclination + (this.IsSelected ? Math.PI / 4 : 0);

				var _HrzV = Vector3d.Transform(Vector3d.UnitX, Quaterniond.FromAxisAngle(Vector3d.UnitZ, _CrossInc)) * _CrossSize;
				var _VrtV = Vector3d.Cross(_HrzV, Vector3d.UnitZ);

				GL.Begin(PrimitiveType.Lines);
				{
					GL.Vertex3(this.Position - _HrzV);
					GL.Vertex3(this.Position + _HrzV);

					GL.Vertex3(this.Position - _VrtV);
					GL.Vertex3(this.Position + _VrtV);
				}
				GL.End();
			}
			//if(this.IsSelected)
			//{
			//    GL.Begin(PrimitiveType.Lines);
			//    {

			//        GL.Vertex3(this.Position);
			//    }
			//    GL.End();
			//}
		}


		#region Члены IVisualObject
		public int  Color {get;set;}

		public void Transform()
		{
			//throw new NotImplementedException();
		}

		public void UpdatePalette(bool iIsLightTheme)
		{
			throw new NotImplementedException();
		}

		public void UpdatePointer(Vector3d iPointer)
		{
			throw new NotImplementedException();
		}

		public void UpdateViewpoint(Viewpoint2D iViewpoint)
		{
			throw new NotImplementedException();
		}

		public void UpdateProjections(bool iDoReset)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Члены ISelectable

		public bool IsSelected {get;set;}

		#endregion

		public void MergeWith(NGonJoint iOtherJoint)
		{
			///this.Parent.Joints.Remove(iOtherJoint);
			
			for(var cLi = 0; cLi < this.Lines.Count; cLi++)
			{
				if(this.Lines[cLi].Joint1 == iOtherJoint || this.Lines[cLi].Joint2 == iOtherJoint)
				{
					this.Lines.RemoveAt(cLi);
				}
			}

			foreach(var cLine in iOtherJoint.Lines)
			{
				if(!this.Lines.Contains(cLine))
				{
					this.Lines.Add(cLine);

					if     (iOtherJoint == cLine.Joint1 && iOtherJoint != cLine.Joint2) cLine.Joint1 = this;
					else if(iOtherJoint == cLine.Joint2 && iOtherJoint != cLine.Joint1) cLine.Joint2 = this;
					else throw new WTFE();
				}
				else throw new WTFE();
			}

			if(this.Port == null && iOtherJoint.Port != null)
			{
				this.Port = iOtherJoint.Port;

				if     (this.Port.InnerPin == iOtherJoint) this.Port.InnerPin = this;
				else if(this.Port.OuterPin == iOtherJoint) this.Port.OuterPin = this;
				else throw new WTFE();
			}
			else if(this.Port != null && iOtherJoint.Port == null)
			{

				///this.Port = iOtherJoint.Port;

				//if     (this.Port.InnerJoint == iOtherJoint) this.Port.InnerJoint = this;
				//else if(this.Port.OuterJoint == iOtherJoint) this.Port.OuterJoint = this;
				//else throw new WTFE();
			}
		}
		//public void Absorb(NGonJoint iOtherJoint)
		//{
		//    //if(this.Port == null && iOtherJoint.Port != null)
		//    //{
		//    //    ///~~ ;
		//    //    iOtherJoint.Absorb(this);
		//    //    return;
		//    //}
			


		//    //this.Graph.MergeWith(iOtherJoint.Graph);

		//    //iOtherJoint
		//    for(var cLi = 0; cLi < this.Lines.Count; cLi++)
		//    {
		//        if(this.Lines[cLi].Joint1 == iOtherJoint || this.Lines[cLi].Joint2 == iOtherJoint)
		//        {
		//            this.Lines.RemoveAt(cLi);
		//        }
		//    }

		//    //foreach(var cLine in this.Lines)
		//    //{
		//    //    if(cLine.Joint1 == iOtherJoint || cLine.Joint1 == iOtherJoint) continue;
		//    //}

		//    foreach(var cLine in iOtherJoint.Lines)
		//    {
		//        if(!this.Lines.Contains(cLine))
		//        {
		//            this.Lines.Add(cLine);

		//            if     (iOtherJoint == cLine.Joint1 && iOtherJoint != cLine.Joint2) cLine.Joint1 = this;
		//            else if(iOtherJoint == cLine.Joint2 && iOtherJoint != cLine.Joint1) cLine.Joint2 = this;
		//            else throw new WTFE();
		//        }
		//        ///else throw new WTFE();
		//    }

		//    //DCon.Message(this.Parent.Joints.Remove(iOtherJoint));
			
		//    //this.Joints.RemoveAt(cbJi);
			
		//    ///if(iOtherJoint.Lines.Contains

		//    //var _IsThisWithPort  = this.Port        != null;
		//    //var _IsOtherWithPort = iOtherJoint.Port != null;

		//    //var _Is_IsThisWithPort ^ _IsOtherWithPort;

		//    //var _Is = _IsThisWithPort ^ _IsOtherWithPort;

		//    ///var _ThisHasPort        = this.Port        != null;
		//    ///var _OtherHasPort       = iOtherJoint.Port != null; if(_ThisHasPort && _OtherHasPort) throw new WTFE();
		//    ///var _NeedsJointsCopy    = _ThisHasPort ^ _OtherHasPort;


		//    //var _IsCopyThisToOther  = _NeedsJointsCopy && _ThisHasPort;
		//    //var _IsCopyOtherToThis  = _NeedsJointsCopy && _OtherHasPort;
		//    ///var _JointCopyDirection = _NeedsJointsCopy ? (_ThisHasPort ? +1 : -1) : 0;
			
			
			
		//    //if(this.Port != null)
		//    //{
		//    //    //if(iOtherJoint
		//    //    //if     (iOtherJoint == this.Port.InnerJoint) _DstJoint.Port.InnerJoint = _SrcJoint;
		//    //    //else if(_SrcJoint == _SrcJoint.Port.OuterJoint) _DstJoint.Port.OuterJoint = _SrcJoint;
		//    //    //else    throw new WTFE();

		//    //}
		//    //if(_NeedsJointsCopy)
		//    //{
		//    //    NGonJoint _SrcJoint, _DstJoint;
		//    //    {
		//    //        if     (_ThisHasPort)  {_SrcJoint = this; _DstJoint = iOtherJoint;}
		//    //        else if(_OtherHasPort) {_DstJoint = this; _SrcJoint = iOtherJoint;}
		//    //        else    throw new WTFE();
		//    //    }

		//    //    _DstJoint.Port = _SrcJoint.Port;

		//    //    if     (_SrcJoint == _SrcJoint.Port.InnerJoint) _DstJoint.Port.InnerJoint = _SrcJoint;
		//    //    else if(_SrcJoint == _SrcJoint.Port.OuterJoint) _DstJoint.Port.OuterJoint = _SrcJoint;
		//    //    else    throw new WTFE();
			
		//    //    ///this.Port = iOtherJoint.Port;
				
		//    //}
			
		//    //if(this.Port == null)
		//    //{
		//    //    if(iOtherJoint.Port != null)
		//    //    {
					

		//    //        //var _InJ = this.Port.InnerJoint;
		//    //        //var _OuJ = this.Port.OuterJoint;

		//    //        if     (iOtherJoint == iOtherJoint.Port.InnerJoint) this.Port.InnerJoint = this;
		//    //        else if(iOtherJoint == iOtherJoint.Port.OuterJoint) this.Port.OuterJoint = this;

		//    //        else throw new WTFE();

		//    //        this.Port = iOtherJoint.Port;
		//    //        //this.Port.
		//    //    }
		//    //}
		//    //else if(iOtherJoint.Port == null)
		//    //{
		//    //    if(this.Port != null)
		//    //    {
		//    //        if     (iOtherJoint == iOtherJoint.Port.InnerJoint) this.Port.InnerJoint = this;
		//    //        else if(iOtherJoint == iOtherJoint.Port.OuterJoint) this.Port.OuterJoint = this;

		//    //        else throw new WTFE();

		//    //        this.Port = iOtherJoint.Port;
		//    //    }
		//    //}
		//    //else throw new WTFE();
		//    //else if
			
		//    //if(this.Port != null && this.Port != iOtherJoint.Port) throw new WTFE();

		//            //else

		//    //if(iOtherJoint.Port != null)
		//    //{
		//    //    if(this.Port == null)
		//    //    {
		//    //        //this.Port = iOtherJoint.Port;
		//    //        //this.Port.Update(true);

		//    //        //if     (this.Port.InnerJoint == iOtherJoint){this.Port.InnerJoint = this;}
		//    //        //else if(this.Port.OuterJoint == iOtherJoint){this.Port.OuterJoint = this;}

		//    //        //else throw new WTFE();


		//    //        ////caJoint.Port.OuterJoint = caJoint;
			        
		//    //    }
		//    //    ///else throw new WTFE();
		//    //}



		//    ///if(iOtherJoint.Graph != this.Graph)
		//    //{
		//    //    this.Graph.MergeWith(iOtherJoint.Graph);
		//    //    ///iOtherJoint.Graph.Joints.Remove(iOtherJoint);
		//    //    //iOtherJoint.Graph.Ports.Joints.Remove(iOtherJoint);
		//    //}
		//}

		
		public class Collection : List<NGonJoint>
		{
			public NGonNode Owner;

			public Collection(NGonNode iOwner)
			{
				this.Owner  = iOwner;
			}

			//public new void Insert(int iIndex, NGonJoint iItem)
			//{
			//    iItem.Parent = this.Owner;
			//    base.Insert(iIndex, iItem);
			//}
			public new void Add(NGonJoint iItem)
			{
				iItem.Parent = this.Owner;
				base.Add(iItem);
			}
			public new void Remove(NGonJoint iItem)
			{
				iItem.Parent = null;
				//iItem.
				base.Remove(iItem);
			}

		}
		//public class Cluster : List<NGonJoint>
		//{
		//    public Vector3d Center;
		//    public double   Radius;

		//    public static void Create()
		//    {
		//        var oClusters = new List<NGonJoint.Cluster>();
		//        {
		//            //oClusters.F
		//            ///foreach(var cJoint in _Parent.Joints)
		//            {
						
		//            }
		//        }
		//    }
			
		//}
	}
	public class NGonLine : IVisualObject, ISelectable
	{
		public NGonNode  Parent;
		public NGonJoint Joint1;
		public NGonJoint Joint2;
		public bool      IsPointerOver;
		public bool      IsHighlighting;
		public double    Value;

		public void Draw()
		{
			//return;

			this.Parent.Frame.Profiler.TotalRenderedLines++;
			var _Palette = (this.Parent ?? this.Parent.Frame.Scheme).Palette;

			///GL.LineStipple(1,0xff00);
			///GL.Enable(EnableCap.LineStipple);

			GL.LineWidth(this.IsHighlighting ? 5f : 1f);
			GL.Color4(_Palette.Colors[this.Color]);
			GL.Begin(PrimitiveType.Lines);
			{
				GL.Vertex3(this.Joint1.Position);
				GL.Vertex3(this.Joint2.Position);
			}
			GL.End();

			///GL.Disable(EnableCap.LineStipple);
			//GL.LineStipple(1,0x0101);
		}

		public class Collection : List<NGonLine>
		{
			public NGonNode Owner;

			public Collection(NGonNode iOwner)
			{
				this.Owner  = iOwner;
			}

			public new void Add(NGonLine iItem)
			{
				iItem.Parent = this.Owner;
				base.Add(iItem);
			}
		}

		#region Члены IVisualObject
		public int  Color {get;set;}

		public void Transform()
		{
			//throw new NotImplementedException();
		}

		public void UpdatePalette(bool iIsLightTheme)
		{
			throw new NotImplementedException();
		}

		public void UpdatePointer(Vector3d iPointer)
		{
			throw new NotImplementedException();
		}

		public void UpdateViewpoint(Viewpoint2D iViewpoint)
		{
			throw new NotImplementedException();
		}

		public void UpdateProjections(bool iDoReset)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Члены ISelectable

		public bool IsSelected{get;set;}

		#endregion
	}
	//public class 

	
	//public class GenericLink : NGonChildNode
	//{
	//    public Vector3d Point1;
	//    public Vector3d Point2;

	//    //public NGonNode Node1;
	//    //public NGonNode Node2;
	//    //public int      Edge1 = -1;
	//    //public int      Edge2 = -1;
	//    //public Vector3d MidPoints;

	//    //public Port Port1;
	//    //public Port Port2;

	//    //public SchemeShape
	//    //public               NGonEdgeLink        () : this(3){}
	//    ////public               NGonNode            (int iSides, int iColor)
	//    //public               NGonEdgeLink        (int iColor)
	//    //{
	//    //    this.Color = iColor;
	//    //}
		
	//    public override DataNode WriteNode(DataNode iNode)
	//    {
	//        //SchemeGeometry
	//        //throw new NotImplementedException("don't save this please");

	//        var oNode = iNode ?? new DataNode("NGonEdgeLink");
	//        {
	//            oNode["@color"] = this.Color;
	//            //oNode["@p1"]    = this.Point1.X + ' ' + this.Point1.Y + ' ' + this.Point1.Z;
	//            //oNode["@p2"]    = this.Point2.X + ' ' + this.Point2.Y + ' ' + this.Point2.Z;
	//        }
	//        return oNode;
	//    }
	//    public override void ReadNode(DataNode iNode)
	//    {
	//        this.Color = iNode["@color"];
			
	//        //var _P1Str = (iNode["@p1"] as string).Split(' ');
	//        //var _P2Str = (iNode["@p2"] as string).Split(' ');





	//        //var _Lines = iStr.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
	//        //base.ReadNode(iNode);

	//        //this.UpdateSides(iNode["@sides"]);
	//        //{
	//        //    string _EdgeTypesStr = iNode["@edgeTypes"];

	//        //    for(var cEi = 0; cEi < _EdgeTypesStr.Length; cEi++)
	//        //    {
	//        //        this.EdgeTypes[cEi] = (EdgeType) Int32.Parse(_EdgeTypesStr[cEi].ToString());
	//        //    }
	//        //}




	//        //this.Children.Clear();

	//        //foreach(var cChildN in iNode.Children)
	//        //{
	//        //    var cChildO = new NGonNode();
	//        //    {
	//        //        cChildO.ReadNode(cChildN);
	//        //    }
	//        //    this.Children.Add(cChildO);
	//        //}
	//    }

	//    //public NGonEdgeLink        Clone(int iNewColor, float iFillOpacity, float iEdgeWidth)
	//    //{
	//    //    var oNode = new NGonNode(this.Sides, iNewColor);
	//    //    {
	//    //        oNode.FillOpacity = iFillOpacity;
	//    //        oNode.EdgeWidth   = iEdgeWidth;

	//    //        oNode.Position = this.Position;
	//    //        oNode.Rotation = this.Rotation;
	//    //        oNode.Scale    = this.Scale;
	//    //    }
	//    //    return oNode;
	//    //}
	//}
	//public class NGonEdgeLink : NGonChildNode
	//{
	//    //public NGonNode Node1;
	//    //public NGonNode Node2;
	//    //public int      Edge1 = -1;
	//    //public int      Edge2 = -1;
	//    //public Vector3d MidPoints;

	//    public Port Port1;
	//    public Port Port2;

	//    //public SchemeShape
	//    public               NGonEdgeLink        () : this(3){}
	//    //public               NGonNode            (int iSides, int iColor)
	//    public               NGonEdgeLink        (int iColor)
	//    {
	//        this.Color = iColor;
	//    }
		
	//    public override DataNode WriteNode(DataNode iNode)
	//    {
	//        //SchemeGeometry
	//        //throw new NotImplementedException("don't save this please");

	//        var oNode = iNode ?? new DataNode("NGonEdgeLink");
	//        {
	//            oNode["@color"] = this.Color;
	//            //oNode["@p1"]    = this.Point1.X + ' ' + this.Point1.Y + ' ' + this.Point1.Z;
	//            //oNode["@p2"]    = this.Point2.X + ' ' + this.Point2.Y + ' ' + this.Point2.Z;
	//        }
	//        return oNode;
	//    }
	//    public override void ReadNode(DataNode iNode)
	//    {
	//        this.Color = iNode["@color"];
			
	//        //var _P1Str = (iNode["@p1"] as string).Split(' ');
	//        //var _P2Str = (iNode["@p2"] as string).Split(' ');





	//        //var _Lines = iStr.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
	//        //base.ReadNode(iNode);

	//        //this.UpdateSides(iNode["@sides"]);
	//        //{
	//        //    string _EdgeTypesStr = iNode["@edgeTypes"];

	//        //    for(var cEi = 0; cEi < _EdgeTypesStr.Length; cEi++)
	//        //    {
	//        //        this.EdgeTypes[cEi] = (EdgeType) Int32.Parse(_EdgeTypesStr[cEi].ToString());
	//        //    }
	//        //}




	//        //this.Children.Clear();

	//        //foreach(var cChildN in iNode.Children)
	//        //{
	//        //    var cChildO = new NGonNode();
	//        //    {
	//        //        cChildO.ReadNode(cChildN);
	//        //    }
	//        //    this.Children.Add(cChildO);
	//        //}
	//    }

	//    //public NGonEdgeLink        Clone(int iNewColor, float iFillOpacity, float iEdgeWidth)
	//    //{
	//    //    var oNode = new NGonNode(this.Sides, iNewColor);
	//    //    {
	//    //        oNode.FillOpacity = iFillOpacity;
	//    //        oNode.EdgeWidth   = iEdgeWidth;

	//    //        oNode.Position = this.Position;
	//    //        oNode.Rotation = this.Rotation;
	//    //        oNode.Scale    = this.Scale;
	//    //    }
	//    //    return oNode;
	//    //}
	//    public class Port
	//    {
	//        public Vector3d           Position;
	//        public NGonNode           Owner;
	//        public List<NGonEdgeLink> Links;

	//        public Port(Vector3d iPosition) : this(iPosition, null){}
	//        public Port(Vector3d iPosition, NGonNode iOwner)
	//        {
	//            this.Position = iPosition;
	//            this.Owner    = iOwner;
	//            this.Links    = new List<NGonEdgeLink>();
	//        }

	//        public class Collection : List<Port>
	//        {
	//            public void Reset()
	//            {
	//                this.Clear();
	//            }
	//        }
	//    }
	//}
	
	//public class NeighbourEnvironment : IEnumerable<NGonNode>
	//{
	//    public EdgeNeighbourGroup[] Groups;

	//    //private NGonNode[] _All;
	//    public List<NGonNode> All
	//    {
	//        get
	//        {
	//            var oNodes = new List<NGonNode>();
	//            {
	//                foreach(var cGroup in this.Groups)
	//                {
	//                    if(cGroup != null) foreach(var cNode in cGroup)
	//                    {
	//                        oNodes.Add(cNode);
	//                    }
	//                }
	//            }
				
	//            return oNodes;
	//        }
	//    }

	//    public EdgeNeighbourGroup this[int iSide]
	//    {
	//        get
	//        {
	//            return this.Groups[iSide];
	//        }
	//    }
	//    public NeighbourEnvironment(int iSides)
	//    {
	//        this.Groups = new EdgeNeighbourGroup[iSides];
	//    }

	//    #region Члены IEnumerable<NGonNode>

	//    public IEnumerator<NGonNode> GetEnumerator()
	//    {
	//        return this.All.GetEnumerator();
	//    }

	//    #endregion

	//    #region Члены IEnumerable

	//    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	//    {
	//        return this.All.GetEnumerator();
	//        //throw new NotImplementedException();
	//    }

	//    #endregion
	//}
	//public class EdgeNeighbourGroup : IEnumerable<NGonNode>
	//{
	//    public List<NGonNode> Nodes;

	//    public EdgeNeighbourGroup()
	//    {
	//        this.Nodes = new List<NGonNode>();
	//    }

	//    #region Члены IEnumerable<NGonNode>

	//    public IEnumerator<NGonNode> GetEnumerator()
	//    {
	//        return this.Nodes.GetEnumerator();
	//    }

	//    #endregion

	//    #region Члены IEnumerable

	//    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	//    {
	//        return this.Nodes.GetEnumerator();
	//    }

	//    #endregion
	//}
	//public class Thread
	//{
	//    public Thread()
	//    {

	//    }
	//    public void Step()
	//    {
			
	//    }
	//}


	
	//public class TransmissionPort
	//{
	//    public NGonNode RemoteNode;
	//    public int      RemotePort;

	//}
	//public class Transmission
	//{
	//    public NGonNode SrcNode;
	//    public int      SrcSide;
	//    public NGonNode DstNode;
	//    public int      DstSide;

	//    public Vector3  Value = Vector3.One;

	//    /**
	//    NextStep

	//    foreach cNode in ProcessingNodes

	//        if(cNode.IsReadyForTransmission)

	//        foreach cNode.Outputs
	//            cOutput.Pass()
			
		
	//    */
	//}
}
