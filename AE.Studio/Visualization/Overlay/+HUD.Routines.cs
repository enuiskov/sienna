using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
using OpenTK;
//using OpenTK.Audio;
//using OpenTK.Audio.OpenAL;
//using OpenTK.Graphics.OpenGL;
//using AE.Editor;
//using System.Windows.Forms;
//using System.Runtime.InteropServices;

using AE.Simulation;

namespace AE.Visualization
{
	public partial class VehicleHUDFrame : OverlayFrame
	{
		public struct Routines
		{
			public struct Defaults
			{
				public static StringFormat CenteredStringFormat = new StringFormat{Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};

				public static Font HeadingFont = new Font("Quartz", 12);
				public static Font InfoFont    = new Font(FontFamily.GenericMonospace, 8);

				public static Brush DefaultBrush;
				public static Pen   DefaultPen;

				public static void Update()
				{
					DefaultBrush = new SolidBrush(G.Screen.Palette.Adapt(new CHSAColor(0.6f,2)));
					DefaultPen   = new Pen(DefaultBrush, 1);
				}
			}
			

			public static void DrawVectors(VehicleHUDFrame iFrame, GraphicsContext iGrx)
			{
				//var _InfoFont = new Font(FontFamily.GenericMonospace, 8);
				//var _InfoBrush = iGrx.Palette.Glare;

				var _Names = new string[]{"Frame","Position","Linear","Angular"};
				var _Values = new Vector3d[]
				{
					Vector3d.Zero,
					G.Vehicle.Position,
					G.Vehicle.Velocity.Linear,
					G.Vehicle.Velocity.Angular,
				};


				iGrx.Save();
				iGrx.Translate(10, iGrx.Image.Height - 60);
				for(var cNi = 0; cNi < _Names.Length; cNi ++)
				{
					iGrx.DrawString(_Names[cNi], Defaults.InfoFont, Defaults.DefaultBrush, 0,0);

					var cVec = _Values[cNi];

					iGrx.DrawString(ToString2(cVec.X,6), Defaults.InfoFont, Defaults.DefaultBrush, 0,10);
					iGrx.DrawString(ToString2(cVec.Y,6), Defaults.InfoFont, Defaults.DefaultBrush, 0,20);
					iGrx.DrawString(ToString2(cVec.Z,6), Defaults.InfoFont, Defaults.DefaultBrush, 0,30);
					iGrx.DrawString(ToString2(cVec.Length,6), Defaults.InfoFont, Defaults.DefaultBrush, 0,40);



					iGrx.Translate(70,0);
				}
				iGrx.Restore();

					//var _Ctr = iGrx.BeginContainer();
				//{
					
					


					

				//}
				//iGrx.EndContainer(_Ctr);
			}
			public static void DrawAttitude(VehicleHUDFrame iFrame, GraphicsContext iGrx)
			{
				//var _InfoFont   = new Font(FontFamily.GenericMonospace, 8);
				//var _InfoBrush  = iGrx.Palette.Glare;
				//var _Pen        = new Pen(Defaults.DefaultBrush, 1);
				//var _InfoStrFmt = new StringFormat{Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};

				var _EulerAngles = SimMath.EulerFromQuaternion(G.Vehicle.Rotation, "ZXY");
				var _HdgFix = 0;
				var _Heading = (float)((360 + (_HdgFix) - (_EulerAngles.Z * MathEx.RTD)) % 360);
				var _Pitch   = (float)(_EulerAngles.X * MathEx.RTD);
				var _Bank    = (float)(_EulerAngles.Y * MathEx.RTD);

				var _PitchStep  = iGrx.Image.Height * 0.022f;
				var _ScaleWidth = 50;
				var _LabelOffs  = _ScaleWidth + 20;


				var _SawPath = new GraphicsPath();
				{
					_SawPath.StartFigure(); for(var cX = -10; cX >= -_ScaleWidth; cX -= 10) _SawPath.AddLine(cX, -2, cX - 5, +2);
					_SawPath.StartFigure(); for(var cX = +10; cX <= +_ScaleWidth; cX += 10) _SawPath.AddLine(cX, -2, cX + 5, +2);
				}

				iGrx.Save();
				iGrx.Translate(iGrx.Image.Width / 2,iGrx.Image.Height / 2);
				{
					///~~ center 'bird';
					iGrx.DrawLines
					(
						Defaults.DefaultPen, new int[,]
						{
							{-6, 0,-3, 0},
							{-3, 0, 0,+3},
							{ 0,+3,+3, 0},
							{+3, 0,+6, 0},
						}
					);

					iGrx.Save();
					iGrx.Rotate(-_Bank);
					iGrx.Translate(0, (_Pitch / 90) * (_PitchStep * 90));
					{
						///~~ zenith & nadir;
						iGrx.Save();
						{
							iGrx.Translate(0,-90 * _PitchStep);
							iGrx.DrawLine(Defaults.DefaultPen, -5, 0,+5, 0);
							iGrx.DrawLine(Defaults.DefaultPen,  0,-5, 0,+5);

							iGrx.Translate(0, +180 * _PitchStep);
							iGrx.DrawLine(Defaults.DefaultPen, -5, 0,+5, 0);
							iGrx.DrawLine(Defaults.DefaultPen,  0,-5, 0,+5);
						}
						iGrx.Restore();


						///~~ horizon line;
						iGrx.DrawLine(Defaults.DefaultPen, -10,0, -100, 0);
						iGrx.DrawLine(Defaults.DefaultPen, +10,0, +100, 0);

						///~~ pitch grid;
						iGrx.Save();
						{

							iGrx.Translate(0,-90 * _PitchStep);

							for(var cA = +80; cA >= -80; cA -= 10)
							{
								iGrx.Translate(0,_PitchStep * 10);

								if(cA == 0) continue;
								if(Math.Abs(cA - _Pitch) > 15) continue;

								if(cA < 0)
								{
									iGrx.DrawPath(Defaults.DefaultPen, _SawPath);
									//iCtx.MoveTo(-7, cY + 1); for(var cX = -10; cX >= -_ScaleWidth; cX -= 6) iCtx.LineTo(cX, cY - 1), iCtx.LineTo(cX - 3, cY + 1);
									//iCtx.MoveTo(+7, cY + 1); for(var cX = +10; cX <= +_ScaleWidth; cX += 6) iCtx.LineTo(cX, cY - 1), iCtx.LineTo(cX + 3, cY + 1);
								}
								else 
								{
									iGrx.DrawLine(Defaults.DefaultPen, -7,0, -_ScaleWidth, 0);
									iGrx.DrawLine(Defaults.DefaultPen, +7,0, +_ScaleWidth, 0);
								}

								///if(_IsPrimaryIndicator)
								{
									iGrx.Save();
									{
										iGrx.Translate(-_LabelOffs,0);
										iGrx.Rotate((float)(+_Bank * MathEx.DTR));
										
										iGrx.DrawString(cA.ToString(), Defaults.InfoFont, Defaults.DefaultBrush, 0,0, Defaults.CenteredStringFormat);
										iGrx.Translate(+_LabelOffs * 2,0);
										iGrx.DrawString(cA.ToString(), Defaults.InfoFont, Defaults.DefaultBrush, 0,0, Defaults.CenteredStringFormat);

										
									}
									iGrx.Restore();
								}
							}
						}
						iGrx.Restore();
						
							//}
						//}



						///~~ flight path vector;
						var _Vel = G.Vehicle.Velocity.Linear; if(_Vel.Length > 10)
						{
							var _HrzVel  = Math.Sqrt((_Vel.X * _Vel.X) + (_Vel.Y * _Vel.Y));
							var _VrtVel  = _Vel.Z;

							var _HrzA = MathEx.DeltaAngle((_Heading - _HdgFix) * MathEx.DTR, Math.Atan2(_Vel.X, _Vel.Y));
							var _VrtA  = Math.Atan2(_VrtVel, _HrzVel);

							iGrx.Translate((float)(_HrzA * MathEx.RTD) * _PitchStep, -(float)(_VrtA * MathEx.RTD) * _PitchStep);
							iGrx.DrawLines
							(
								Defaults.DefaultPen, new int[,]
								{
									{-6, 0,-3, 0},
									{-3, 0, 0,+3},
									{ 0,+3,+3, 0},
									{+3, 0,+6, 0},
								}
								//new Point[]
								//{
								//   new Point(-6,  0),new Point(-3,  0),
								//   new Point(-3,  0),new Point( 0, +3),
								//   new Point( 0, +3),new Point(+3,  0),
								//   new Point(+3,  0),new Point(+6,  0),
								//}
							);

							

							//iCtx.Save();
							//iCtx.Translate((_HrzA * MathEx.RTD) * _PitchStep, -(_VrtA * MathEx.RTD) * _PitchStep);
							//iCtx.BeginPath();
							//{
							//   iCtx.MoveTo( -5, +2);
							//   iCtx.LineTo( -2, +2);
							//   iCtx.LineTo(  0,  0);
							//   iCtx.LineTo(  0, -2);

							//   iCtx.MoveTo(  0,  0);
							//   iCtx.LineTo( +2, +2);
							//   iCtx.LineTo( +5, +2);
							//}
							//iCtx.LineWidth = 1;
							//iCtx.Stroke();
							//iCtx.Restore();
						}
					}
					iGrx.Restore();
				}
				iGrx.Restore();
			}
			public static void DrawHeading(VehicleHUDFrame iFrame, GraphicsContext iGrx)
			{
				//var _InfoFont   = new Font(FontFamily.GenericMonospace, 12);
				//var _InfoBrush  = iGrx.Palette.Glare;
				//var _Pen        = new Pen(_InfoBrush, 1);

				var _EulerAngles = SimMath.EulerFromQuaternion(G.Vehicle.Rotation, "ZXY");
				var _HdgFix = 0;
				var _Heading = (int)(((360 + _HdgFix - (_EulerAngles.Z * MathEx.RTD)) % 360));

				iGrx.Save();
				iGrx.Translate(iGrx.Image.Width * 0.5f,iGrx.Image.Height * 0.1f);
				
				iGrx.DrawString(_Heading.ToString("D3"), Defaults.HeadingFont, Defaults.DefaultBrush, 0,0, Defaults.CenteredStringFormat);

				iGrx.Restore();
			}
			public static void DrawAltitude(VehicleHUDFrame iFrame, GraphicsContext iGrx)
			{
				var _Alt = (int)(G.Vehicle.Position.Z);

				iGrx.Save();
				iGrx.Translate(iGrx.Image.Width * 0.8f,iGrx.Image.Height * 0.4f);
				
				iGrx.DrawString(_Alt.ToString("D5"), Defaults.HeadingFont, Defaults.DefaultBrush, 0,0, Defaults.CenteredStringFormat);

				iGrx.Restore();
			}
			public static void DrawSpeed(VehicleHUDFrame iFrame, GraphicsContext iGrx)
			{
				////var _InfoFont   = new Font(FontFamily.GenericMonospace, 12);
				//var _InfoBrush  = iGrx.Palette.Glare;
				//var _Pen        = new Pen(_InfoBrush, 1);

				//var _Alt = (int)(G.Vehicle.Position.Z);

				//iGrx.Save();
				//iGrx.Translate(iGrx.Image.Width * 0.8f,iGrx.Image.Height * 0.4f);
				
				//iGrx.DrawString(_Alt.ToString("D5"), Defaults.HeadingFont, _InfoBrush, 0,0, Defaults.CenteredStringFormat);

				//iGrx.Restore();
			}
		}
		
	}
}
