using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AE.Visualization
{
	public enum DockStyle
	{
		None   = 0,
		Top    = 1,
		Bottom = 2,
		Left   = 3,
		Right  = 4,
		Fill   = 5,
	}
	public struct Padding
	{
		public static Padding None = new Padding(-1);

		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public Padding(int iAll) : this(iAll,iAll,iAll,iAll){}
		public Padding(int iLeft, int iTop, int iRight, int iBottom)
		{
			this.Left   = iLeft;
			this.Top    = iTop;
			this.Right  = iRight;
			this.Bottom = iBottom;
		}

		public override bool Equals(object iObj)
		{
			return this == (Padding)iObj;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public static bool operator ==(Padding ixP, Padding iyP)
		{
			return ixP.Left == iyP.Left && ixP.Top == iyP.Top && ixP.Right == iyP.Right && ixP.Bottom == iyP.Bottom;
		}
		public static bool operator !=(Padding ixP, Padding iyP)
		{
			return !(ixP == iyP);
		}
	}

	//public struct Vector2
	//{
	//    public static Vector2 Zero;
	//    public float X;
	//    public float Y;
	//}
	public class MouseDragmeter
	{
		public class MouseButtonDragInfo
		{
			public MouseButtons Button;
			public bool         IsDragging;
			public object       OriginValue;

			public PointF  Origin;
			public PointF  OriginInt;
			public PointF  Offset;
			public PointF  OffsetInt;
			public PointF  TotalDistance;
		}

		public MouseButtonDragInfo LeftButton;
		public MouseButtonDragInfo MiddleButton;
		public MouseButtonDragInfo RightButton;


		public MouseButtonDragInfo this[MouseButtons iButton]
		{
			get
			{
				switch(iButton)
				{
					case MouseButtons.Left   : return this.LeftButton;
					case MouseButtons.Middle : return this.MiddleButton;
					case MouseButtons.Right  : return this.RightButton;

					default: return null;
				}
			}
		}

		public MouseDragmeter()
		{
			this.LeftButton   = new MouseButtonDragInfo();
			this.MiddleButton = new MouseButtonDragInfo();
			this.RightButton  = new MouseButtonDragInfo();
		}
		public void Reset(MouseButtons iMouseButton, int iX, int iY)
		{
			var _DragInfo = this[iMouseButton]; if(_DragInfo == null) return;
			{
				_DragInfo.Origin        = new PointF(iX,iY);
				_DragInfo.OriginInt     = new PointF(iX,iY);

				_DragInfo.Offset        = PointF.Empty;
				_DragInfo.OffsetInt     = PointF.Empty;

				_DragInfo.TotalDistance = PointF.Empty;
				_DragInfo.IsDragging    = false;
			}

			//switch(iMouseButton)
			//{
			//    case WF.MouseButtons.Left   : this.LMouseDragOrigin = new Vector2(iX, iY); this.LMouseDragOffset = Vector2.Zero; this.LMouseDragDistance = Vector2.Zero; break;
			//    case WF.MouseButtons.Middle : this.MMouseDragOrigin = new Vector2(iX, iY); this.MMouseDragOffset = Vector2.Zero; this.MMouseDragDistance = Vector2.Zero; break;
			//    case WF.MouseButtons.Right  : this.RMouseDragOrigin = new Vector2(iX, iY); this.RMouseDragOffset = Vector2.Zero; this.RMouseDragDistance = Vector2.Zero; break;

			//    default : throw new WTFE();
			//}
		}
		public void Update(MouseButtons iMouseButton, int iX, int iY)
		{
			var _DragInfo = this[iMouseButton]; if(_DragInfo == null) return;
			{
				var _OriginInt_Old = _DragInfo.OriginInt;
				var _OriginInt_New =  new PointF(iX,iY);

				var _OffsetInt_Old = _DragInfo.OffsetInt;
				var _OffsetInt_New = PointF.Subtract(_OriginInt_New, new SizeF(_OriginInt_Old));

				//_DragInfo.Origin;
				_DragInfo.OriginInt  = _OriginInt_New;
	
				_DragInfo.Offset     = PointF.Subtract(_DragInfo.OriginInt, new SizeF(_DragInfo.Origin));
				_DragInfo.OffsetInt  = _OffsetInt_New;

				_DragInfo.TotalDistance  = PointF.Add(_DragInfo.TotalDistance, new SizeF(Math.Abs(_OffsetInt_New.X), Math.Abs(_OffsetInt_New.Y)));
				_DragInfo.IsDragging     = Math.Max(Math.Abs(_DragInfo.TotalDistance.X),Math.Abs(_DragInfo.TotalDistance.Y)) > 5;
			}


			//switch(iMouseButton)
			//{
			//    case MouseButtons.Left   : this.LMouseDragDistance += new Vector2(Math.Abs(iX - this.LMouseDragOrigin.X), Math.Abs(iY - this.LMouseDragOrigin.Y)); this.LMouseDragOrigin = new Vector2(iX, iY); break;
			//    case MouseButtons.Middle : this.MMouseDragDistance += new Vector2(Math.Abs(iX - this.MMouseDragOrigin.X), Math.Abs(iY - this.MMouseDragOrigin.Y)); this.MMouseDragOrigin = new Vector2(iX, iY); break;
			//    case MouseButtons.Right  :
			//                               this.RMouseDragDistance += new Vector2(Math.Abs(iX - this.RMouseDragOrigin.X), Math.Abs(iY - this.RMouseDragOrigin.Y));
			//                               this.RMouseDragOrigin = new Vector2(iX, iY);
			//                               ///GCon.Message("L:" + this.RMouseDragDistance.Length);
			//                               break;


			//    //default : throw new WTFE();
			//}
		}
		

	}
	
}
