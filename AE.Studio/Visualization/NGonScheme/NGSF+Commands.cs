using System;
using System.Collections.Generic;
using System.ComponentModel;

//using System.Drawing;
//using System.Data;
////using System.Text;
//using System.Windows.Forms;
//using OpenTK;
//using OpenTK.Graphics;
//using OpenTK.Graphics.OpenGL;

namespace AE.Visualization
{
	public partial class NGonSchemeFrame : SchemeFrame<AE.Visualization.SchemeObjectModel.NGonNode>
	{
		[CommandInfo("set node name")]
		protected virtual string _NN(string iArgs)
		{
			(G.Application as Studio).NGonSchemeFrame.SelectedNodes.ForEach(cNode => cNode.Name = iArgs);

			return null;//"Changed node name(s) = '" + iArgs + "'";
		}
		[CommandInfo("set node type")]
		protected virtual string _NT(string iArgs)
		{
			(G.Application as Studio).NGonSchemeFrame.SelectedNodes.ForEach(cNode => cNode.Type = iArgs);

			return null;
		}
		[CommandInfo("set node description")]
		protected virtual string _ND(string iArgs)
		{
			(G.Application as Studio).NGonSchemeFrame.SelectedNodes.ForEach(cNode => cNode.Description = iArgs);

			return null;
		}

		//public class Commands
		//{
		//    [CommandInfo(null,"set node name")]
		//    public static string NN(string iArgs)
		//    {
		//        (G.Application as Studio).NGonSchemeFrame.SelectedNodes.ForEach(cNode => cNode.Name = iArgs);

		//        return null;//"Changed node name(s) = '" + iArgs + "'";
		//    }
		//    [CommandInfo(null,"set node type")]
		//    public static string NT(string iArgs)
		//    {
		//        (G.Application as Studio).NGonSchemeFrame.SelectedNodes.ForEach(cNode => cNode.Type = iArgs);

		//        return null;
		//    }
		//    [CommandInfo(null,"set node description")]
		//    public static string ND(string iArgs)
		//    {
		//        (G.Application as Studio).NGonSchemeFrame.SelectedNodes.ForEach(cNode => cNode.Description = iArgs);

		//        return null;
		//    }
		//}
	}
}