using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AE.Simulation
{
	//public struct Euler
	//{
	//   public double X;
	//   public double Y;
	//   public double Z;
	//}
	public static class SimMath
	{
		public static Quaterniond QuaternionFromRotationVector(Vector3d iRotV)
		{
			var _AxisV = iRotV.Normalized(); if(Double.IsNaN(_AxisV.X * _AxisV.Y * _AxisV.Z)) _AxisV = Vector3d.One;
			var _Angle = iRotV.Length;
			
			return Quaterniond.FromAxisAngle(_AxisV, _Angle);
		}
		public static Vector3d EulerFromQuaternion(Quaterniond iQuat, string iOrder)
		{
			///~~ iQuat is assumed to be normalized;
			///~~ http://www.mathworks.com/matlabcentral/fileexchange/20696-function-to-convert-between-dcm-euler-angles-iQuatuatuaternions-and-euler-vectors/content/SpinCalc.m ;

			var _SqX = iQuat.X * iQuat.X;
			var _SqY = iQuat.Y * iQuat.Y;
			var _SqZ = iQuat.Z * iQuat.Z;
			var _SqW = iQuat.W * iQuat.W;


			var oEulerA = new Vector3d(); switch(iOrder)
			{
				case "XYZ":

					oEulerA.X = Math.Atan2(2 * (iQuat.X * iQuat.W - iQuat.Y * iQuat.Z), (_SqW - _SqX - _SqY + _SqZ));
					oEulerA.Y = Math.Asin(MathEx.ClampNP(2 * (iQuat.X * iQuat.Z + iQuat.Y * iQuat.W)));
					oEulerA.Z = Math.Atan2(2 * (iQuat.Z * iQuat.W - iQuat.X * iQuat.Y), (_SqW + _SqX - _SqY - _SqZ));
					break;

				case "YXZ":

					oEulerA.X = Math.Asin(MathEx.ClampNP(2 * (iQuat.X * iQuat.W - iQuat.Y * iQuat.Z)));
					oEulerA.Y = Math.Atan2(2 * (iQuat.X * iQuat.Z + iQuat.Y * iQuat.W), (_SqW - _SqX - _SqY + _SqZ));
					oEulerA.Z = Math.Atan2(2 * (iQuat.X * iQuat.Y + iQuat.Z * iQuat.W), (_SqW - _SqX + _SqY - _SqZ));
					break;

				case "ZXY":

					oEulerA.X = Math.Asin(MathEx.ClampNP(2 * (iQuat.X * iQuat.W + iQuat.Y * iQuat.Z)));
					oEulerA.Y = Math.Atan2(2 * (iQuat.Y * iQuat.W - iQuat.Z * iQuat.X), (_SqW - _SqX - _SqY + _SqZ));
					oEulerA.Z = Math.Atan2(2 * (iQuat.Z * iQuat.W - iQuat.X * iQuat.Y), (_SqW - _SqX + _SqY - _SqZ));
					break;

				case "ZYX":

					oEulerA.X = Math.Atan2(2 * (iQuat.X * iQuat.W + iQuat.Z * iQuat.Y), (_SqW - _SqX - _SqY + _SqZ));
					oEulerA.Y = Math.Asin(MathEx.ClampNP(2 * (iQuat.Y * iQuat.W - iQuat.X * iQuat.Z)));
					oEulerA.Z = Math.Atan2(2 * (iQuat.X * iQuat.Y + iQuat.Z * iQuat.W), (_SqW + _SqX - _SqY - _SqZ));
					break;

				case "YZX":

					oEulerA.X = Math.Atan2(2 * (iQuat.X * iQuat.W - iQuat.Z * iQuat.Y), (_SqW - _SqX + _SqY - _SqZ));
					oEulerA.Y = Math.Atan2(2 * (iQuat.Y * iQuat.W - iQuat.X * iQuat.Z), (_SqW + _SqX - _SqY - _SqZ));
					oEulerA.Z = Math.Asin(MathEx.ClampNP(2 * (iQuat.X * iQuat.Y + iQuat.Z * iQuat.W)));
					break;

				case "XZY":

					oEulerA.X = Math.Atan2(2 * (iQuat.X * iQuat.W + iQuat.Y * iQuat.Z), (_SqW - _SqX + _SqY - _SqZ));
					oEulerA.Y = Math.Atan2(2 * (iQuat.X * iQuat.Z + iQuat.Y * iQuat.W), (_SqW + _SqX - _SqY - _SqZ));
					oEulerA.Z = Math.Asin(MathEx.ClampNP(2 * (iQuat.Z * iQuat.W - iQuat.X * iQuat.Y)));
					break;

				default : throw new Exception("Invalid order: '" + iOrder + "'"); 
			}
			return oEulerA;
		}

	}
	public interface ISpatialDisplacement
	{
		Vector3d    Position {get;set;}
		Quaterniond Rotation {get;set;}
	}
	public interface ISpatialRateOfChange
	{
		Vector3d    Linear   {get;set;}
		Vector3d    Angular  {get;set;}
	}
	
	public enum SimViewMode
	{
		Num0,
		Num1,
		Num2,
		Num3,
		Num4,
		Num5,
		Num6,
		Num7,
		Num8,
		Num9,
	}
	public class SimEngine
	{
		public DateTime LastUpdate;
		public double LastDelta;
		public double Time = 0;
		public double TimeScale = 1;
		public bool   IsActive = false;

		//public Matrix4d ProjMatrix;/// = Matrix4d.CreatePerspectiveFieldOfView(45 * MathEx.DTR, this.Owner.AspectRatio, _NearZ, _FarZ);
		public SimViewMode ViewMode;
		public Matrix4d ViewMatrix;/// = Matrix4d.LookAt(this.Eye, this.Target, Vector3d.UnitZ);
		public Vector3d ExtViewpointInfo = new Vector3d(3.7,0,10);

		public SimObject.Collection Objects;
		

		public SimEngine()
		{
			this.Objects = new SimObject.Collection();
			{
				var _Vehicle = new DynamicObject();
				{
					
					_Vehicle.Position = new Vector3d(0,0,1);
					_Vehicle.Velocity = new Velocity{Linear = new Vector3d(0,0,0), Angular = new Vector3d(0,0,0)};

					//_Vehicle.Position = new Vector3d(2000000,0,40000);
					//_Vehicle.Velocity = new Velocity{Linear = new Vector3d(-20000,0,0), Angular = new Vector3d(0,0,0)};
				}
				this.Objects.Add(_Vehicle);
			}

			this.ViewMode = SimViewMode.Num4 ;

			
			
			this.Update();
			///this.UpdateView();
		}

		public virtual void Update()
		{
			if(!this.IsActive) return;

			var _CurrTime = DateTime.Now;
			var _TimeSpan = _CurrTime - this.LastUpdate;
			{
				if(_TimeSpan.TotalSeconds > 1)
				{
					this.LastUpdate = _CurrTime;
					return;
				}
			}

			//if(this.LastUpdate == undefined) this.LastUpdate = new Date().ValueOf() - 1;
			var _DeltaT = _TimeSpan.TotalSeconds * this.TimeScale;

			this.Update(_DeltaT);
		}
		public virtual void Update(double iDeltaT)
		{
			this.Time       += iDeltaT;
			this.LastDelta   = iDeltaT;
			this.LastUpdate  = DateTime.Now;


			if(iDeltaT  == 0) iDeltaT = 1e-12;

			foreach(var cObj in this.Objects)
			{
				cObj.Update(iDeltaT);
			}
		}
		public void UpdateView()///bool iDoProjToo)
		{
			//if(iDoProjToo)
			//{
			//   this.ProjMatrix = Matrix4d.CreatePerspectiveFieldOfView(45 * MathEx.DTR, this., 1e-3, 1e+3);
			//}
			var _ActiveObject = this.Objects[0];

			var _PseudoView = this.ExtViewpointInfo;
			var _EyeAzimuth = _PseudoView.X;
			var _EyeZenith  = _PseudoView.Y;
			var _EyeDistance = _PseudoView.Z;



			Vector3d _EyePos = Vector3d.Zero, _EyeTgt = _ActiveObject.Position, _EyeTop = new Vector3d(0,0,0.5f);
			double _FoV = 45;

			

			switch(this.ViewMode)
			{
				case SimViewMode.Num1 :
				{
					_EyePos = _ActiveObject.Position;
					_EyeTgt = Vector3d.Add(_EyePos, Vector3d.Transform(Vector3d.UnitY, _ActiveObject.Rotation));
					_EyeTop = Vector3d.Transform(Vector3d.UnitZ, _ActiveObject.Rotation);

					break;
				}
				case SimViewMode.Num4 :
				{
					//this.ExtViewpointInfo.X

					//this.EyeAzimuth += iAzimuthDelta;
					//this.EyeZenith   = MathEx.Clamp(this.EyeZenith + iZenithDelta, -Math.PI / 2 * 0.999, +Math.PI / 2 * 0.999);
					
					
					var _RotationQ = Quaterniond.FromAxisAngle(Vector3d.UnitZ, _EyeAzimuth)
									   * Quaterniond.FromAxisAngle(Vector3d.UnitY, _EyeZenith);

					_EyePos = _EyeTgt + Vector3d.Transform(Vector3d.UnitX, _RotationQ) * _EyeDistance;


					//var _AbsHrzAngle = (this.ExtViewpointInfo.X * MathEx.D360) - SimMath.EulerFromQuaternion(_ActiveObject.Rotation, "ZXY").Z;
					//var _VertAngle   = this.ExtViewpointInfo.Y * 1;

					///_EyePos = _ActiveObject.Position + (new Vector3d(Math.Sin(_AbsA) * this.ExtViewpointInfo.Z, Math.Cos(_AbsA) * this.ExtViewpointInfo.Z, this.ExtViewpointInfo.Y * 100));
					///_EyePos = _ActiveObject.Position + (new Vector3d(Math.Sin(_AbsHrzAngle) * this.ExtViewpointInfo.Z, Math.Cos(_AbsHrzAngle) * this.ExtViewpointInfo.Z, _VertAngle));


					//_TgtP.Add(new Vector3(0,0,2), self);
					_FoV = 2000 / this.ExtViewpointInfo.Z;// 70;
				
					break;
				}
			}

			
			
			this.ViewMatrix = Matrix4d.LookAt(_EyePos, _EyeTgt, _EyeTop);
			///this.ViewMatrix
		}

		public virtual void SetProjectionMatrix(AE.Visualization.Frame iFrame)
		{
			this.UpdateView();

			///GL.MatrixMode(MatrixMode.Projection); GL.LoadMatrix(ref this.ProjMatrix);
			GL.MatrixMode(MatrixMode.Modelview);  GL.LoadMatrix(ref this.ViewMatrix);
		}
	}

	public class SimObject : ISpatialDisplacement
	{
		public string               Id;
		public SimObject            Parent;
		public SimObject.Collection Children;

		public Vector3d    Position {get;set;}
		public Quaterniond Rotation {get;set;}
		//public Vector3d    Scale; ///??
		public SimObject()
		{
			this.Id = "SimObject";

			this.Position = new Vector3d(0,0,1);
			this.Rotation = Quaterniond.Identity;
		}
		public virtual void Update(double iDeltaT)
		{
			//this.
		}
		public Vector3d LocalToGlobal(Vector3d iVec)
		{
			return Vector3d.Transform(iVec, this.Rotation) + this.Position;
		}
		public Vector3d GlobalToLocal(Vector3d iVec)
		{
			return Vector3d.Transform(iVec + (this.Position * -1), this.Rotation.Inverted());
		}


			//LocalToGlobal : function(iVec)
			// {
			//   //return iVec.Rotate(this.Rotation);
			//   return iVec.Rotate(this.Rotation).Add(this.Position);
			// },
			//GlobalToLocal : function(iVec)
			// {
			//   //return iVec.Rotate(this.Rotation.Inverse());
			//   return iVec.Add(this.Position.Inverse()).Rotate(this.Rotation.Inverse());
			// },

		public class Collection : List<SimObject>
		{
			
		}
	}
	public class StaticObject  : SimObject
	{
		
	}
	public class DynamicObject : SimObject
	{
		public Mass             Mass;
		public Velocity         Velocity;
		public Force.Collection Forces;

		public DynamicObject()
		{
			this.Mass     = new Mass(1,new Vector3d(1,1,1) * 5, Vector3d.Zero);
			this.Velocity = new Velocity();
			this.Forces   = new Force.Collection(this);
			{
				this.Forces.Add(new Force.LocalTorque {Id = "Local_Pitch", Value = new Vector3d(10,0,0)});
				this.Forces.Add(new Force.LocalTorque {Id = "Local_Bank",  Value = new Vector3d(0,10,0)});
				this.Forces.Add(new Force.LocalTorque {Id = "Local_Yaw",   Value = new Vector3d(0,0,10)});

				this.Forces.Add(new Force.Wheel       {Id = "WheelFL", Factor = 1, Position = new Vector3d(-1,+1.3,-0.9)});
				this.Forces.Add(new Force.Wheel       {Id = "WheelFR", Factor = 1, Position = new Vector3d(+1,+1.3,-0.9)});
				this.Forces.Add(new Force.Wheel       {Id = "WheelRL", Factor = 1, Position = new Vector3d(-1,-1.3,-0.9)});
				this.Forces.Add(new Force.Wheel       {Id = "WheelRR", Factor = 1, Position = new Vector3d(+1,-1.3,-0.9)});


				this.Forces.Add(new Force.LocalForce  {Id = "Local_Lift",  Value = new Vector3d(0,0, this.Mass.Value * 9.8 * 2), Factor = 0.0});
				this.Forces.Add(new Force.LocalForce  {Id = "CruiseEngine",  Value = new Vector3d(0,10,0), Factor = 0.0});

				///this.Forces.Add(new Force.LocalForce  {Id = "TestForce",  Value = new Vector3d(0,0, 10), Factor = 0.0, Position = new Vector3d(0,10,0)});
				///this.Forces.Add(new Force.GlobalForce  {Id = "TestForce",  Value = new Vector3d(0,0, 1), Factor = 0.0, Position = new Vector3d(0,10,0)});
				

				this.Forces.Add(new Force.Gravity     {Id = "Gravity", Factor = 1});
				this.Forces.Add(new Force.AirDrag     {Id = "AirDrag", Factor = 1});

				
			}
			///this.Rotation = Quaterniond.FromAxisAngle(Vector3d.UnitZ, MathEx.D90);
		}
		public virtual void OnBeforeUpdate(double iDeltaT)
		{
			
		}
		public virtual void OnAfterUpdate(double iDeltaT)
		{
			
		}
		public override void Update(double iDeltaT)
		{
			this.OnBeforeUpdate(iDeltaT);

			this.ApplyForces(iDeltaT);
			this.ApplyMotion(iDeltaT);

			this.OnAfterUpdate(iDeltaT);
		}
		public void Stop(bool iDoLinear, bool iDoAngular)
		{
			if(iDoLinear) this.Velocity.Linear = Vector3d.Zero;
			if(iDoAngular) this.Velocity.Angular = Vector3d.Zero;
		}

		public virtual void ApplyForces(double iDeltaT)
		{
			///return;


			var _Mass     =  this.Mass;
			//var _InvMass  = _Mass.Inverted;
			//var _InvInerV = _InvMass.Distribution * _InvMass.Value;

			var _GloP     =  this.Position;
			var _GloR     =  this.Rotation;
			var _InvGloP  = _GloP * -1;
			var _InvGloR  = _GloR.Inverted();

			var _AccR   = new Acceleration();
			var _TotMom = new Momentum();


			if(iDeltaT > 0.0) foreach(var cForce in this.Forces)
			{
				

				var cForceIsWeak  = Math.Abs(cForce.Factor) < 1e-9; if(cForceIsWeak) continue;
				var cNeedsIntegra = !cForce.IsCustomIntegration;

				var cMom = cForce.ProduceMomentum(iDeltaT); if(cMom == null) continue;

				if(cForce.Id == "TestForce" && cForce.Factor > 0)
				{
					
				}


				var cMomLocP = Vector3d.Subtract((cMom.IsAbsolute ? this.GlobalToLocal(cMom.Position) : cForce.Position), _Mass.Center);
				
				

				var cMomLocR = cMom.IsAbsolute ? Quaterniond.Multiply(_InvGloR, cMom.Rotation) : cForce.Rotation ;
				///not sure about this ^^^^^^^^^^^^^^^

				
				///~~ Primary linear and secondary linear-to-angular effects;
				Vector3d cPriLinV = cMom.Linear, cSecAngV = Vector3d.Zero;
				double	cPriLinL = cPriLinV.Length, cSecAngL = 0; if(cPriLinL != 0)
				{
					cPriLinV = Vector3d.Transform(cPriLinV, cMomLocR);
					cPriLinL = cPriLinV.Length;///~~???

					if(cMomLocP.Length != 0)
					{
						cSecAngV = Vector3d.Cross(cMomLocP, cPriLinV);
						cSecAngL = cSecAngV.Length;
					}
				}

				///~~ Primary angular and secondary angular-to-linear(?) effects;
				Vector3d cPriAngV = cMom.Angular, cSecLinV = Vector3d.Zero;
				double cPriAngL = cPriAngV.Length, cSecLinL = 0; if(cPriAngL != 0)
				{
					cPriAngV = Vector3d.Transform(cPriAngV, cMomLocR);
					cPriAngL = cPriAngV.Length;
				}

				
				if(cSecLinL != 0) cPriLinV = Vector3d.Add(cPriLinV, cSecLinV); 
				if(cSecAngL != 0) cPriAngV = Vector3d.Add(cPriAngV, cSecAngV);///~~ not sure about this, need to learn some math :);
				
				cMom.Linear  = Vector3d.Transform(cPriLinV, _GloR);
				cMom.Angular = cPriAngV;

				if(cNeedsIntegra)      cMom.MultiplyScalar(iDeltaT,true);
				if(cForce.Factor != 1) cMom.MultiplyScalar(cForce.Factor,true);
		

				if(Double.IsNaN(cMom.Linear.Length) || Double.IsNaN(cMom.Angular.Length))
				{
					throw new Exception("WTFE");
				}
				_TotMom.Add(cMom, true);
			}

			_AccR = _TotMom.Accelerate(this);


			if(iDeltaT == 0) iDeltaT = 1e-12;
			
			this.Velocity.Acceleration.Jerk .Set(_AccR.Subtract(this.Velocity.Acceleration.MultiplyScalar(iDeltaT, false), false));
			this.Velocity.Acceleration      .Set(_AccR.MultiplyScalar(1 / iDeltaT, false));
			this.Velocity                   .Set(this.Velocity.Add(_AccR,false));

			///this.Velocity.Acceleration.Jerk .Set(_AccR.Subtract(this.Velocity.Acceleration.MultiplyScalar(iDeltaT)));
			///this.Velocity.Acceleration      .Set(_AccR.MultiplyScalar(1 / iDeltaT));
			///this.Velocity                   .Set(this.Velocity.Add(_AccR));
		}
		public virtual void ApplyMotion(double iDeltaT)
		{
			var _Transform = this.PredictTransform(iDeltaT, null);

			this.Position = _Transform.Position;
			this.Rotation = _Transform.Rotation;

			///////////////////////////

			//this.Velocity.Linear  += this.Velocity.Acceleration.Linear  * (1.0 / iDeltaT);
			//this.Velocity.Angular += this.Velocity.Acceleration.Angular * (1.0 / iDeltaT);
			
			//this.Velocity.Acceleration.Linear  = Vector3d.Zero;
			//this.Velocity.Acceleration.Angular = Vector3d.Zero;


			/////G.Vehicle.Position = Vector3d.Add(G.Vehicle.Position, Vector3d.Transform(Vector3d.UnitY * +0.001, G.Vehicle.Rotation));
			//this.Position = Vector3d.Add(this.Position, this.Velocity.Linear * iDeltaT);
			//this.Rotation = Quaterniond.Multiply(this.Rotation, Quaterniond.FromAxisAngle(this.Velocity.Angular, this.Velocity.Angular.Length * iDeltaT));
		}
		public virtual SpatialDisplacement PredictTransform(double iDeltaT, ISpatialDisplacement iNestO)
		{
			var _OwnerP = this.Position + (this.Velocity.Linear * iDeltaT);
			var _OwnerR = SimMath.QuaternionFromRotationVector(this.Velocity.Angular * iDeltaT) * this.Rotation;//, self);
			
			
			var oData = new SpatialDisplacement
			{
				//Position = Vector3d   .Add      (_OwnerP, Vector3d.Transform(iNestO.Position, _OwnerR)),
				//Rotation = Quaterniond.Multiply (_OwnerR, iNestO.Rotation)

				Position = iNestO != null ? _OwnerP + Vector3d.Transform(iNestO.Position, _OwnerR) : _OwnerP,
				Rotation = iNestO != null ? _OwnerR * iNestO.Rotation : _OwnerR
			};
			
			return oData;
		}
	}
	public class KinematicObject  : SimObject
	{
		
	}
	public class Mass
	{
		public double       Value;
		public Vector3d     Center       = Vector3d.Zero;
		public Vector3d     Distribution = Vector3d.One;
		public InvertedMass Inverted;
		
		public Mass(double iValue, Vector3d iExtents, Vector3d iCenter)
		{
			this.Value = iValue;
			this.Center = iCenter;
			this.Distribution = GetMassDistribution(iExtents);
			this.Inverted = new Mass.InvertedMass
			{
				Value        = 1 / this.Value,
				Distribution = new Vector3d(1 / this.Distribution.X, 1 / this.Distribution.Y, 1 / this.Distribution.Z)
			};
		}

		public static Vector3d GetMassDistribution(Vector3d iExtents)
		{
			///~~ I.x = m/12 * (y*y + z*z);

			return new Vector3d
			(
				1.0 / 12 * ((iExtents.Y * iExtents.Y) + (iExtents.Z * iExtents.Z)),
				1.0 / 12 * ((iExtents.X * iExtents.X) + (iExtents.Z * iExtents.Z)),
				1.0 / 12 * ((iExtents.Y * iExtents.Y) + (iExtents.X * iExtents.X))
			);
		}

		public class InvertedMass
		{
			public double   Value = 1;
			public Vector3d Distribution;
		}
	}
	public class Force : ISpatialDisplacement
	{
		public string        Id;
		public double        Factor;
		public bool          IsCustomIntegration = false; ///~~ custom (or no) integration intension (e.g: rigid body impact)

		public Vector3d      Position {get;set;}
		public Quaterniond   Rotation {get;set;}

		public DynamicObject Owner;

		public Force()
		{
			this.Rotation = Quaterniond.Identity;
		}

		public virtual Momentum ProduceMomentum(double iDeltaT)
		{
			throw new NotImplementedException();
		}

		public class Collection : List<Force>
		{
			public DynamicObject Owner;

			public Collection(DynamicObject iOwner)
			{
				this.Owner = iOwner;
			}

			public Force this[string iId]{get{foreach(var cForce in this) if(cForce.Id == iId) return cForce; return null;}}

			public new void Add(Force iForce)
			{
				iForce.Owner = this.Owner;

				base.Add(iForce);
			}

			
		}
		public class LocalTorque : Force
		{
			public Vector3d Value;

			

			public override Momentum ProduceMomentum(double iDeltaT)
			{
				return Momentum.Relative(Vector3d.Zero, this.Value);
			}
		}
		public class LocalForce : Force
		{
			public Vector3d Value;

			public override Momentum ProduceMomentum(double iDeltaT)
			{
				return Momentum.Relative(this.Value, Vector3d.Zero);
			}
		}
		public class GlobalForce : Force
		{
			public Vector3d Value;

			public override Momentum ProduceMomentum(double iDeltaT)
			{
				return Momentum.Absolute(this.Value, Vector3d.Zero, new Vector3d(0,0,0));
			}
		}
		public class AirDrag : Force
		{
			public override Momentum ProduceMomentum(double iDeltaT)
			{
				//return Momentum.Absolute
				//(
				//   this.Owner.Velocity.Linear * -10000 * MathEx.ClampZP(MathEx.Scale01(50000 - this.Owner.Position.Z, 10000, 50000)),
				//   this.Owner.Velocity.Angular * -100,
				//   this.Owner.Position
				//);
				return Momentum.Absolute
				(
					///this.Owner.Velocity.Linear * -1,/// * MathEx.ClampZP(MathEx.Scale01(50000 - this.Owner.Position.Z, 10000, 50000)),
					this.Owner.Velocity.Linear * -0.1 * MathEx.ClampZP(MathEx.Scale01(10 - this.Owner.Position.Z, 0, 10)),
					this.Owner.Velocity.Angular * -1,
					this.Owner.Position
				);
			}
		}
		public class Gravity : Force
		{
			public override Momentum ProduceMomentum(double iDeltaT)
			{
				return Momentum.Absolute(new Vector3d(0,0,-9.8 * this.Owner.Mass.Value),Vector3d.Zero, this.Owner.Position);/// * Math.Scale01(this.Owner.Velocity.Linear.Length, 10000, 0)
				///return Momentum.Absolute( new Vector3d(0,0,-0.005), Vector3d.Zero);/// * Math.Scale01(this.Owner.Velocity.Linear.Length, 10000, 0)
			}
		}
		public class Wheel : Force
		{
			public double Stiffness = 1;///0.5;///0.5;
			public double Damping = 5;
			public double Friction = 1;
			public double TurnMass = 1;
			public double Angle = 0;
			public double Acceleration = 0.1;
			public double Brake = 0;
			public double Compression {get{return MathEx.Clamp(0.3 - this.Owner.PredictTransform(0, this).Position.Z, 0, 1);}}
			public double Rpm = 0;

			public Wheel()
			{
				this.IsCustomIntegration = true;
			}

			public override Momentum ProduceMomentum(double iDeltaT)
			{
				var _MassV  = this.Owner.Mass.Value;
				var _GloT1 = this.Owner.PredictTransform(0,       this);
				var _GloT2 = this.Owner.PredictTransform(iDeltaT, this);
				var _CompF = this.Compression;

				var _TerrOffs = 0;///(Math.Cos(_GloT1.Position.X * 0.1) * 1);
				_GloT1.Position += new Vector3d(0,0,_TerrOffs);
				_GloT2.Position += new Vector3d(0,0,_TerrOffs);


				Vector3d _VrtF = Vector3d.Zero;
				{
					
					//_Z1 += _TerrOffs;
					//_Z2 += _TerrOffs;


					var _Z1 = _GloT1.Position.Z;
					var _Z2 = _GloT2.Position.Z; if(_Z2 > 0) return Momentum.Zero;

					

					var _ZVel       = _Z2 - _Z1;
					var _Resistance = this.Stiffness * Math.Max(0,-_Z1);// * iDeltaT;// * iDeltaT;//Magic(1 + Clamp(1 - _ToGndDist, 0, 1), 30) * iDeltaT;
					var _Damping    = this.Damping   * _ZVel;

					_VrtF           = new Vector3d(0,0, (_Resistance - _Damping) * _MassV * 50);
				}
				Vector3d _HrzF = Vector3d.Zero;
				{
					var _LinVel   = _GloT2.Position - _GloT1.Position;
					var _Whl2GndV = Vector3d.Transform(_LinVel, _GloT1.Rotation.Inverted());
					//var _Whl2GndV = ;

					//var _SideF  = new Vector3(0,0,0);
					var _SideF  = Vector3d.Transform(new Vector3d(-MathEx.ClampNP(_Whl2GndV.X * this.Friction * MathEx.Magic(_CompF,5) * 500), 0, 0), _GloT1.Rotation) * _MassV*3;
					//var _SideF  = Vector3d.Transform(new Vector3d(-MathEx.ClampZP(_Whl2GndV.X * this.Friction * Math.Pow(_CompF,1d / 5) * 500), 0, 0), _GloT1.Rotation) * _MassV;
					///var _BrakeF = Vector3.Clamp(this.Owner.Velocity.Linear).MultiplyScalar(-_MassV * this.Brake);
					///var _BrakeF = .Clamp(this.Owner.Velocity.Linear).MultiplyScalar(-_MassV * this.Brake);
					var _BrakeF = Vector3d.Zero;
					
					_HrzF = _SideF + _BrakeF;

					this.Angle += _Whl2GndV.Y * -2.5;
				}

				

				return Momentum.Absolute((_VrtF + _HrzF) * iDeltaT, Vector3d.Zero, _GloT1.Position);

				///return Momentum.Absolute(new Vector3d(0,0,this.Compression * _Mass * 1), Vector3d.Zero, _GloT2.Position);
				//return Momentum.Absolute(new Vector3d(0,0,this.Compression * 1000000), Vector3d.Zero);///, Vector3d.Zero, Quaterniond.Identity);
			}
		}
	}

	public class SpatialDisplacement : ISpatialDisplacement
	{
		public Vector3d    Position {get;set;}
		public Quaterniond Rotation {get;set;}

		//public SpatialDisplacement()
		//{
		//   //this.Rotation = Quaterniond.Identity;
		//}
	}
	public class SpatialRateOfChange : ISpatialRateOfChange
	{
		public Vector3d Linear   {get;set;}
		public Vector3d Angular  {get;set;}

		public SpatialRateOfChange Add(SpatialRateOfChange iRate, bool iDoSelf)
		{
			var oRate = iDoSelf ? this : new SpatialRateOfChange();
			{
				oRate.Linear  = Vector3d.Add(this.Linear, iRate.Linear);
				oRate.Angular = Vector3d.Add(this.Angular, iRate.Angular);
			}
			return oRate;
		}
		public SpatialRateOfChange Subtract(SpatialRateOfChange iRate, bool iDoSelf)
		{
			var oRate = iDoSelf ? this : new SpatialRateOfChange();
			{
				oRate.Linear  = Vector3d.Subtract(this.Linear, iRate.Linear);
				oRate.Angular = Vector3d.Subtract(this.Angular, iRate.Angular);
			}
			return oRate;
		}
		public SpatialRateOfChange MultiplyScalar(double iScalar, bool iDoSelf)
		{
			var oRate = iDoSelf ? this : new SpatialRateOfChange();
			{
				oRate.Linear  = Vector3d.Multiply(this.Linear, iScalar);
				oRate.Angular = Vector3d.Multiply(this.Angular, iScalar);
			}
			return oRate;
		}

		public void Set(SpatialRateOfChange iRate)
		{
			this.Linear = iRate.Linear;
			this.Angular = iRate.Angular;
		}
		///public void Set(Vector3d iLinear, Vector3d iAngular)
		//{
		//   this.Linear = iLinear;
		//   this.Angular = iAngular;
		//}
	}
	//public class SpatialRateOfChange : ISpatialRateOfChange
	//{
	//   public Vector3d Linear   {get;set;}
	//   public Vector3d Angular  {get;set;}

	//   public SpatialRateOfChange Add(SpatialRateOfChange iRate)
	//   {
	//      this.Linear  = Vector3d.Add(this.Linear, iRate.Linear);
	//      this.Angular = Vector3d.Add(this.Angular, iRate.Angular);

	//      return this;
	//   }
	//   public SpatialRateOfChange Subtract(SpatialRateOfChange iRate)
	//   {
	//      this.Linear  = Vector3d.Subtract(this.Linear, iRate.Linear);
	//      this.Angular = Vector3d.Subtract(this.Angular, iRate.Angular);

	//      return this;
	//   }
	//   public SpatialRateOfChange MultiplyScalar(double iScalar)
	//   {
	//      this.Linear  = Vector3d.Multiply(this.Linear, iScalar);
	//      this.Angular = Vector3d.Multiply(this.Angular, iScalar);

	//      return this;
	//   }

	//   public void Set(SpatialRateOfChange iRate)
	//   {
	//      this.Linear = iRate.Linear;
	//      this.Angular = iRate.Angular;
	//   }
	//   ///public void Set(Vector3d iLinear, Vector3d iAngular)
	//   //{
	//   //   this.Linear = iLinear;
	//   //   this.Angular = iAngular;
	//   //}
	//}

	public class Momentum : SpatialRateOfChange, ISpatialDisplacement
	{
		public Vector3d      Position {get;set;}
		public Quaterniond   Rotation {get;set;}

		//public Vector3d      Linear   {get;set;}
		//public Vector3d      Angular  {get;set;}

		public bool IsAbsolute;

		public static Momentum Zero {get{return new Momentum();}}
		//public bool IsRelative;

		
		public Acceleration Accelerate(DynamicObject iObject)
		{
			var oAcc = new Acceleration();
			{
				oAcc.Linear  = Vector3d.Multiply(this.Linear, iObject.Mass.Inverted.Value);
				oAcc.Angular = Vector3d.Transform(Vector3d.Multiply(this.Angular, iObject.Mass.Inverted.Distribution), iObject.Rotation);
			}
			return oAcc;
		}

		//public static Momentum Absolute(Vector3d iLinear, Vector3d iAngular)
		//{
		//   return Absolute(iLinear, iAngular, Vector3d.Zero);
		//}
		public static Momentum Absolute(Vector3d iLinear, Vector3d iAngular, Vector3d iWorldPos)
		{
			return Absolute(iLinear, iAngular, iWorldPos, Quaterniond.Identity);
		}
		public static Momentum Absolute(Vector3d iLinear, Vector3d iAngular, Vector3d iWorldPos,Quaterniond iWorldRot)
		{
			var oMom = new Momentum
			{
				IsAbsolute = true,

				Position   = iWorldPos,
				Rotation   = iWorldRot,
				Linear     = iLinear,
				Angular    = iAngular,
			};
			return oMom;
		}
		public static Momentum Relative(Vector3d iLinear, Vector3d iAngular)
		{
			return Relative(iLinear, iAngular, Vector3d.Zero, Quaterniond.Identity);
		}
		public static Momentum Relative(Vector3d iLinear, Vector3d iAngular, Vector3d iPosition, Quaterniond iRotation)
		{
			var oMom = new Momentum
			{
				IsAbsolute = false,

				Position   = iPosition,
				Rotation   = iRotation,
				Linear     = iLinear,
				Angular    = iAngular,
			};
			return oMom;
		}
	}

	

	public class Velocity : SpatialRateOfChange
	{
		public Acceleration Acceleration;

		public Velocity()
		{
			this.Acceleration = new Acceleration();
		}
	}
	public class Acceleration : SpatialRateOfChange
	{
		public Jerk Jerk;

		public Acceleration()
		{
			this.Jerk = new Jerk();
		}
	}
	public class Jerk : SpatialRateOfChange
	{

	}
}
