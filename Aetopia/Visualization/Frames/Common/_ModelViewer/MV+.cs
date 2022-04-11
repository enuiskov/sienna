using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public partial class ModelViewer : Frame
	{
		public ViewManager Views;
	}

	public class ViewManager
	{
		public OrthographicView Top;
		public OrthographicView Bottom;
		public OrthographicView Left;
		public OrthographicView Right;

		public PerspectiveView  Perspective;
		public OrthographicView Orthographic;

		public OrthographicView CurrentOrthographic;
		public PerspectiveView  CurrentPerspective;

		public void SetCustom(OrthographicView iView)
		{
			throw new NotImplementedException();
		}
		public void SetCustom(PerspectiveView iView)
		{
			throw new NotImplementedException();
		}
	}
	public class OrthographicView
	{
		public Vector2d Position;
		public double   Zoom;
		public double   Inclination;

		public Matrix4d Matrix;
	}
	public class PerspectiveView
	{
		public Vector3d Eye;
		public Vector3d Target;
		public Vector3d Top;
		public double   FieldOfView;

		public Matrix4d Matrix;
	}
}