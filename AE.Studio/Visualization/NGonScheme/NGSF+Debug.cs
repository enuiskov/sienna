using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace AE.Visualization
{
	public partial class NGonSchemeFrame : SchemeFrame<AE.Visualization.SchemeObjectModel.NGonNode>
	{
		public Profiler_ Profiler;

		public class Profiler_
		{
			public int TotalNodeProjectionUpdateAttempts = 0;
			public int TotalNodeProjectionUpdates        = 0;
			public int TotalNodeProjectionResets         = 0;

			
			//public int TotalAttemptsToTransformMatrix = 0;
			public int TotalMatrixTransforms          = 0;

			public int TotalAttemptsToRenderNode = 0;
			public int TotalRenderedNodes  = 0;
			public int TotalRenderedPorts  = 0;
			public int TotalRenderedLines  = 0;
			public int TotalRenderedJoints = 0;

			public void ResetProjector()
			{
				this.TotalNodeProjectionUpdateAttempts = 0;
				this.TotalNodeProjectionUpdates        = 0;
				this.TotalNodeProjectionResets         = 0;
			}
			public void ResetRenderer()
			{
				//this.TotalAttemptsToTransformMatrix = 0;
				this.TotalMatrixTransforms          = 0;

				this.TotalAttemptsToRenderNode = 0;

				this.TotalRenderedNodes  = 0;
				this.TotalRenderedPorts  = 0;
				this.TotalRenderedLines  = 0;
				this.TotalRenderedJoints = 0;
			}
			public void PostProjectorStats()
			{
				G.Console.Message
				(
					"<8>\r\n== ProjectorStats ==" + "\r\n" + 

					" TotalNodeProjectionUpdateAttempts : " + this.TotalNodeProjectionUpdateAttempts + "\r\n" + 
					" TotalNodeProjectionUpdates        : " + this.TotalNodeProjectionUpdates +  " (" + Math.Round((float)this.TotalNodeProjectionUpdates / this.TotalNodeProjectionUpdateAttempts * 100) + "%)" + "\r\n" + 
					" TotalNodeProjectionResets         : " + this.TotalNodeProjectionResets + "\r\n" + 
					""
				);
			}
			public void PostRendererStats()
			{
				G.Console.Message
				(
					"<8>\r\n== RendererStats ==" + "\r\n" +

					//" TotalAttemptsToTransformMatrix : " + this.TotalAttemptsToTransformMatrix);
					" TotalMatrixTransforms : "          + this.TotalMatrixTransforms + "\r\n" +// + " (" + Math.Round((float)this.TotalMatrixTransforms / this.TotalAttemptsToTransformMatrix * 100) + "%)");

					" TotalAttemptsToRenderNode : " + this.TotalAttemptsToRenderNode + "\r\n" +
					" TotalRenderedNodes        : " + this.TotalRenderedNodes + " (" + Math.Round((float)this.TotalRenderedNodes / this.TotalAttemptsToRenderNode * 100) + "%)" + "\r\n" +
					" TotalRenderedPorts        : " + this.TotalRenderedPorts + "\r\n" +
					" TotalRenderedLines        : " + this.TotalRenderedLines + "\r\n" +
					" TotalRenderedJoints       : " + this.TotalRenderedJoints + "\r\n" +
					""
				);
			}
		}
	}
}