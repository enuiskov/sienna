using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using AE;
using AE.Visualization;
using AE.Visualization.SchemeObjectModel;

public static class Workarounds
{
	public static Random RNG         = new Random(0);

	public static Screen             Screen;

	public static BufferConsoleFrame DebugConsoleFrame;
	public static BufferConsoleFrame MainConsoleFrame;
	public static Frame              RecentNonConsoleFrame;

	//public static ZoomableFrame<  AnyZoomableFrame; 
	public static NGonSchemeFrame    NGonSchemeFrame;



	public static NGonNode        Scheme      {get{return NGonSchemeFrame.Scheme;}}

	public struct Performance
	{
		public static int TotalNodeProjectionUpdateAttempts = 0;
		public static int TotalNodeProjectionUpdates        = 0;
		public static int TotalNodeProjectionResets         = 0;

		
		//public static int TotalAttemptsToTransformMatrix = 0;
		public static int TotalMatrixTransforms          = 0;

		public static int TotalAttemptsToRenderNode = 0;
		public static int TotalRenderedNodes  = 0;
		public static int TotalRenderedPorts  = 0;
		public static int TotalRenderedLines  = 0;
		public static int TotalRenderedJoints = 0;

		public static void ResetProjector()
		{
			TotalNodeProjectionUpdateAttempts = 0;
			TotalNodeProjectionUpdates        = 0;
			TotalNodeProjectionResets         = 0;
		}
		public static void ResetRenderer()
		{
			//TotalAttemptsToTransformMatrix = 0;
			TotalMatrixTransforms          = 0;

			TotalAttemptsToRenderNode = 0;

			TotalRenderedNodes  = 0;
			TotalRenderedPorts  = 0;
			TotalRenderedLines  = 0;
			TotalRenderedJoints = 0;
			
		}
		public static void PostProjectorStats()
		{
			GCon.Message
			(
				"<8>\r\n== ProjectorStats ==" + "\r\n" + 

				" TotalNodeProjectionUpdateAttempts : " + TotalNodeProjectionUpdateAttempts + "\r\n" + 
				" TotalNodeProjectionUpdates        : " + TotalNodeProjectionUpdates +  " (" + Math.Round((float)TotalNodeProjectionUpdates / TotalNodeProjectionUpdateAttempts * 100) + "%)" + "\r\n" + 
				" TotalNodeProjectionResets         : " + TotalNodeProjectionResets + "\r\n" + 
				""
			);
		}
		public static void PostRendererStats()
		{
			GCon.Message
			(
				"<8>\r\n== RendererStats ==" + "\r\n" +

				//" TotalAttemptsToTransformMatrix : " + TotalAttemptsToTransformMatrix);
				" TotalMatrixTransforms : "          + TotalMatrixTransforms + "\r\n" +// + " (" + Math.Round((float)TotalMatrixTransforms / TotalAttemptsToTransformMatrix * 100) + "%)");

				" TotalAttemptsToRenderNode : " + TotalAttemptsToRenderNode + "\r\n" +
				" TotalRenderedNodes        : " + TotalRenderedNodes + " (" + Math.Round((float)TotalRenderedNodes / TotalAttemptsToRenderNode * 100) + "%)" + "\r\n" +
				" TotalRenderedPorts        : " + TotalRenderedPorts + "\r\n" +
				" TotalRenderedLines        : " + TotalRenderedLines + "\r\n" +
				" TotalRenderedJoints       : " + TotalRenderedJoints + "\r\n" +
				""
			);
		}
	}
}

