using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AE.Visualization
{
	public class WaveEditorFrame : AudioTestFrame
	{
		public float[]      SineValues;
		public Point[]      SinePoints;

		public  float[]     DataValues;
		private Point[]     DataPoints;
		private Rectangle[] DataRectangles;

		public  bool        IsDataUpdated        = true; ///~~ for AudioTest only?;
		public  float       Pitch    = 1.0f;
		public  float       Throttle = 0.0f;
		public  float       TargetThrottle = 1.0f;

		private bool        NeedsGraphSync       = true;
		private Point       CurrCursorPosition   = new Point(-1,-1);
		private int         LastCursorValueIndex = -1;
		private int         CurrCursorValueIndex = -1;
		private float       VertNormRange        = 0.49f;


		private int         PointSize            {get{return (int)(this.Width / this.DataValues.Length * 0.5f);}}
		//private float       VertNormRange = 0.49f;
		

		
		public WaveEditorFrame()
		{
			this.SineValues = GenerateWaves(1,2000);

			this.DataValues = new float[this.SineValues.Length];
			{
				Array.Copy(this.SineValues, this.DataValues, this.SineValues.Length);
				//this.DataValues[ 0] = +0.5f;
				//this.DataValues[10] = +0.2f;
				//this.DataValues[20] = +0.1f;
				//this.DataValues[30] = -0.5f;
				//this.DataValues[40] = -0.9f;
				//this.DataValues[50] = -1.0f;
				//this.DataValues[60] = -0.3f;
				//this.DataValues[70] = +0.5f;
				//this.DataValues[80] = +0.9f;
				//this.DataValues[90] = +1.0f;
				//this.DataValues[95] = +0.0f;

			}
			//this.OnResize(null);
		}
		public float[] GenerateWaves(float iWaveCount, int iLength)
		{
			var oSine = new float[iLength];
			{
				for(var cVi = 0; cVi < iLength; cVi ++)
				{
					var cValue = (float)Math.Cos(((double)cVi / iLength * MathEx.D360 - MathEx.D180) * iWaveCount);
					oSine[cVi] = (float)MathEx.ClampNP(cValue * 1e0);

					
				}
			}
			return oSine;
		}
		public void SyncGraph(bool iDoSineToo)
		{
			var _PointCount  = this.DataValues.Length;
			var _PointSize   = this.PointSize;
			//var _
			//this.Sine

			this.DataPoints     = new Point    [_PointCount];
			this.DataRectangles = new Rectangle[_PointCount]; if(_PointSize < 2) this.DataRectangles = null;
			{
				for(var cVi = 0; cVi < _PointCount; cVi ++)
				{
					var cV = this.DataValues[cVi];
					var cPoint = this.GetPointAt(cVi, false);

					//var cX = (int)(((float)cVi / _PointCount) * this.Width);
					//var cY = (int)(((- cV) * 0.49f + 0.5f)    * this.Height);
					//var cPoint = new Point(cX,cY);

					var cRect  = new Rectangle(cPoint.X - (_PointSize / 2), cPoint.Y - (_PointSize / 2), _PointSize, _PointSize);
					

					this.DataPoints[cVi] = cPoint;
					if(this.DataRectangles != null) this.DataRectangles[cVi] = cRect;
					//_
				}
			}
			this.NeedsGraphSync = false;


			///if(iDoSineToo)
			{
				this.SinePoints = new Point[_PointCount];
				{
					for(var cVi = 0; cVi < _PointCount; cVi++)
					{
						var cSineValue = this.SineValues[cVi];
						var cSinePoint = this.GetPointAt(cVi, true);

						this.SinePoints[cVi] = cSinePoint;
					}
				}
			}
			this.Invalidate();
		}
		//public override void OnBeforeRender()
		//{
		//    this.Invalidate(1);
		//}

		public Point GetPointAt(int iIndex, bool iDoGetSinePoint)
		{
			return new Point
			(
				///(int)(((float)iIndex / this.DataValues.Length) * this.Width),
				5 + (int)(((float)iIndex / this.DataValues.Length) * (this.Width - 10)),

				(int)(((- (iDoGetSinePoint ? this.SineValues : this.DataValues)[iIndex]) * this.VertNormRange + 0.5f) * this.Height)
			);
		}
		public int GetIndexAtX(int iX)
		{
			///return MathEx.Clamp((int)Math.Round((float)iX / this.Width * this.DataValues.Length), 0, this.DataValues.Length - 1);
			return MathEx.Clamp((int)Math.Round((float)(iX - 5) / (this.Width - 10) * this.DataValues.Length), 0, this.DataValues.Length - 1);
		}
		public float GetValueAtY(int iY)
		{
			//return (1f - ((float)iY / this.Height)) / 0.49f - 1f;
			///return (1.02f - ((float)iY / this.Height / 0.49f));

			//return (1.02f - ((float)iY / this.Height / 0.49f));
			return ((((float)this.Height - iY) / this.Height) - 0.5f) / this.VertNormRange;/// / 0.49f;
		}
		//public float GetValueAtX(int iX)
		//{
		//    return Math.Round((float)iX / this.Data.Length * this.Height);
		//}
		


		public void DrawCursor(GraphicsContext iGrx)
		{
			if(this.CurrCursorValueIndex == -1) return;

			var _Pen = new Pen(this.Palette.Glare);

			//var _
			//var _CursorPoint      = 
			var _CursorValuePoint = this.GetPointAt(this.CurrCursorValueIndex, false);
			

			iGrx.DrawLine(_Pen, new Point(this.CurrCursorPosition.X, 5), new Point(this.CurrCursorPosition.X, this.Height - 10));

			iGrx.DrawLine(_Pen, new Point(5, this.CurrCursorPosition.Y), new Point(this.Width - 10, this.CurrCursorPosition.Y));
			///iGrx.DrawLine(_Pen, new Point(5,       _CursorValuePoint.Y), new Point(this.Width - 10,       _CursorValuePoint.Y));

			iGrx.DrawLine(_Pen, new Point(_CursorValuePoint.X - 10, _CursorValuePoint.Y - 10), new Point(_CursorValuePoint.X + 10, _CursorValuePoint.Y + 10));
			iGrx.DrawLine(_Pen, new Point(_CursorValuePoint.X - 10, _CursorValuePoint.Y + 10), new Point(_CursorValuePoint.X + 10, _CursorValuePoint.Y - 10));
			//iGrx.DrawLine(_Pen, new Point(_CursorPoint.X, 5), new Point(_CursorPoint.X, this.Height - 10));

			var _CrsValPointSize = this.PointSize * 2;
			iGrx.FillRectangle(this.Palette.Fore, new Rectangle(_CursorValuePoint.X - (this.PointSize), _CursorValuePoint.Y - (this.PointSize), _CrsValPointSize, _CrsValPointSize));
		}
		public override void DrawForeground(GraphicsContext iGrx)
		{
			if(this.NeedsGraphSync) this.SyncGraph(false);

			var _SineBrush = new SolidBrush(Color.FromArgb(64,this.Palette.GlareColor));
			var _DataBrush = this.Palette.Fore;

			var _SinePen = new Pen(_SineBrush, 1f);
			var _DataPen = new Pen(_DataBrush, 1f);



			iGrx.DrawLines(_SinePen, this.SinePoints);
			iGrx.DrawLines(_DataPen, this.DataPoints);

			if(this.DataRectangles != null) iGrx.FillRectangles(_DataBrush, this.DataRectangles);


			
			this.DrawCursor(iGrx);


			var _Font = new Font("Quartz", 12f, FontStyle.Regular);
			iGrx.DrawString("Pitch    : " + this.Pitch,     _Font, this.Palette.Glare, 20,20);
			iGrx.DrawString("Throttle : " + this.Throttle, _Font, this.Palette.Glare, 20,40);

			///iGrx.DrawRectangle(new Pen(this.Palette.Fore, 1), new Rectangle(Point.Empty, this.Bounds.Size - new Size(1,1)));


		}
		public override void OnBeforeRender()
		{
			base.OnBeforeRender();

			
			this.Pitch = MathEx.Clamp(((this.Pitch + ((this.TargetThrottle - this.Pitch) * 0.0001f)) + this.Throttle), 0.0f, 5.0f);
			this.Throttle *= 0.99f;
			//this.Pitch *= 0.999f;
			this.Invalidate();
		}

		protected override void OnLoad(GenericEventArgs iEvent)
		{
			this.OnResize(null);
			//base.OnLoad(iEvent);
		}
		protected override void OnResize(GenericEventArgs iEvent)
		{
			this.SyncGraph(true);
		}
		protected override void OnMouseDown(MouseEventArgs iEvent)
		{
			this.LastCursorValueIndex = -1;

			this.OnMouseMove(iEvent);
		}
		protected override void OnMouseUp(MouseEventArgs iEvent)
		{
			this.LastCursorValueIndex = -1;
		}
		protected override void OnMouseMove(MouseEventArgs iEvent)
		{
			var _CurrCursorValueIndex = this.GetIndexAtX(iEvent.X);// MathEx.Clamp((int)Math.Round((float)iEvent.X / this.Width * this.Data.Length), 0, this.Data.Length - 1);
			var _CurrCursorValue      = this.GetValueAtY(iEvent.Y);// (1f - ((float)iEvent.Y / this.Height)) * 2f - 1f;

			this.CurrCursorPosition   = iEvent.Location;
			this.CurrCursorValueIndex = _CurrCursorValueIndex;



			if((iEvent.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				var _LastCursorValueIndex = this.LastCursorValueIndex != -1 ? this.LastCursorValueIndex : _CurrCursorValueIndex; 
				var _LastCursorValue      = this.DataValues[_LastCursorValueIndex];

				var _MinValueIndex = Math.Min(_CurrCursorValueIndex, _LastCursorValueIndex);
				var _MaxValueIndex = Math.Max(_CurrCursorValueIndex, _LastCursorValueIndex);
				var _IsCurrGreater = _CurrCursorValueIndex > _LastCursorValueIndex;
				//var _CurrCursorValue = this.Data[
				//var _Range    = (float)(_MaxValue - _MinValue);
				//var _

				for(int cVi = _MinValueIndex; cVi <= _MaxValueIndex; cVi ++)
				{
					var cWeight = ((float)(cVi - _MinValueIndex) / (_MaxValueIndex - _MinValueIndex + 1)); 
					var cValue  = MathEx.Mix(_CurrCursorValue,_LastCursorValue, _IsCurrGreater ?  1f - cWeight : cWeight);

					this.DataValues[cVi] = cValue;
				}
				this.NeedsGraphSync = true;
				this.LastCursorValueIndex = _CurrCursorValueIndex;

				this.IsDataUpdated = true;
			}
			this.Invalidate();
		}
		protected override void OnKeyDown(KeyEventArgs iEvent)
		{
			///base.OnKeyDown(iEvent);

			
			float[] _SineValues = null; switch(iEvent.KeyCode)
			{
				//case Keys.D1 : _SineValues = this.GenerateWaves(1, this.DataValues.Length); break;
				//case Keys.D2 : _SineValues = this.GenerateWaves(2, this.DataValues.Length); break;
				//case Keys.D3 : _SineValues = this.GenerateWaves(3, this.DataValues.Length); break;
				//case Keys.D4 : _SineValues = this.GenerateWaves(4, this.DataValues.Length); break;
				//case Keys.D5 : _SineValues = this.GenerateWaves(5, this.DataValues.Length); break;
				//case Keys.D6 : _SineValues = this.GenerateWaves(6, this.DataValues.Length); break;
				//case Keys.D7 : _SineValues = this.GenerateWaves(7, this.DataValues.Length); break;
				//case Keys.D8 : _SineValues = this.GenerateWaves(8, this.DataValues.Length); break;
				//case Keys.D9 : _SineValues = this.GenerateWaves(9, this.DataValues.Length); break;
				//case Keys.D0 : _SineValues = this.GenerateWaves(10, this.DataValues.Length); break;

				case Keys.D1 : _SineValues = this.GenerateWaves(1, this.DataValues.Length); break;
				case Keys.D2 : _SineValues = this.GenerateWaves(2, this.DataValues.Length); break;
				case Keys.D3 : _SineValues = this.GenerateWaves(4, this.DataValues.Length); break;
				case Keys.D4 : _SineValues = this.GenerateWaves(8, this.DataValues.Length); break;
				case Keys.D5 : _SineValues = this.GenerateWaves(16, this.DataValues.Length); break;
				case Keys.D6 : _SineValues = this.GenerateWaves(32, this.DataValues.Length); break;
				case Keys.D7 : _SineValues = this.GenerateWaves(64, this.DataValues.Length); break;
				case Keys.D8 : _SineValues = this.GenerateWaves(128, this.DataValues.Length); break;
				case Keys.D9 : _SineValues = this.GenerateWaves(256, this.DataValues.Length); break;
				case Keys.D0 : _SineValues = this.GenerateWaves(512, this.DataValues.Length); break;


				//case Keys.D1 : _SineValues = this.GenerateWaves((float)AudioTestFrame.HalfToneFactor * 1, this.DataValues.Length); break;
				//case Keys.D2 : _SineValues = this.GenerateWaves((float)AudioTestFrame.HalfToneFactor * 2, this.DataValues.Length); break;
				//case Keys.D3 : _SineValues = this.GenerateWaves((float)AudioTestFrame.HalfToneFactor * 3, this.DataValues.Length); break;
				//case Keys.D4 : _SineValues = this.GenerateWaves((float)AudioTestFrame.HalfToneFactor * 4, this.DataValues.Length); break;
				//case Keys.D5 : _SineValues = this.GenerateWaves((float)AudioTestFrame.HalfToneFactor * 5, this.DataValues.Length); break;
				//case Keys.D6 : _SineValues = this.GenerateWaves((float)AudioTestFrame.HalfToneFactor * 6, this.DataValues.Length); break;
				//case Keys.D7 : _SineValues = this.GenerateWaves((float)AudioTestFrame.HalfToneFactor * 7, this.DataValues.Length); break;
				//case Keys.D8 : _SineValues = this.GenerateWaves((float)AudioTestFrame.HalfToneFactor * 8, this.DataValues.Length); break;
				//case Keys.D9 : _SineValues = this.GenerateWaves((float)AudioTestFrame.HalfToneFactor * 9, this.DataValues.Length); break;
				//case Keys.D0 : _SineValues = this.GenerateWaves((float)AudioTestFrame.HalfToneFactor * 10, this.DataValues.Length); break;


				//Math.Pow(AudioTestFrame.HalfToneFactor,this.BaseOffset + iNoteI)

				case Keys.Up   : this.Throttle += 0.001f; break;
				case Keys.Down : this.Throttle -= 0.001f; break;

				case Keys.F1   : this.TargetThrottle  = 0.2f; break;
				case Keys.F3   : this.TargetThrottle += 0.1f; break;
				case Keys.F2   : this.TargetThrottle -= 0.1f; break;
				case Keys.F4   : this.TargetThrottle  = 4.2f; break;


				default : return;
			}
			if(_SineValues != null)
			{
				for(var cVi = 0; cVi < this.DataPoints.Length; cVi++)
				{
					var cDataV = this.DataValues[cVi];/// * 0.1f;
					var cSineV = _SineValues[cVi];

					this.DataValues[cVi] = MathEx.Mix(cDataV, iEvent.Shift ? -cSineV : +cSineV, 0.02f);
				}
				this.IsDataUpdated = true;
				this.NeedsGraphSync = true;
			}
			this.Invalidate();
		}
	}
}
