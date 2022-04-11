using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace AE.Visualization
{
	public partial class BufferConsoleFrame : TextBufferFrame//, IConsole
	{
		//public struct Handlers
		//{
		//    public static void BatchRegister()
		//    {
		//        var _CmdMM = typeof(Handlers).GetMethods(BindingFlags.Public | BindingFlags.Static);
		//        {
		//            foreach(var cCmdM in _CmdMM)
		//            {
		//                if(cCmdM.Name == "BatchRegister") continue;

		//                //cCmdF.Inv
		//                //Delegate.CreateDelegate(
		//                var cCmdD = (CommandDelegate)Delegate.CreateDelegate(typeof(CommandDelegate), cCmdM);

		//                Commands.Register(cCmdM.Name.ToLower(), cCmdD);
		//            }
		//        }


		//        //Commands.Register("clear", new CommandDelegate(Clear));
		//    }
			
		//    public static string Exit(string iArgs)
		//    {
		//        G.Application.Exit();
		//        ///Application.MainForm.Close();

		//        return null;
		//    }
		//    public static string Clear(string iArgs)
		//    {
		//        G.Console.Frame.History.Clear();

		//        return null;
		//    }
		//    public static string Fps(string iArgs)
		//    {
		//        return Math.Round(G.Screen.AverageFrameRate).ToString();
		//    }
		//    public static string Help(string iArgs)
		//    {
		//        var oStr = " Supported commands\r\n";
		//        {
		//            //oStr += " Type 'cmdname /?' for more detailed information:\r\n";
		//            oStr += " ------------------------------------------------\r\n";
		//            oStr += " | clear - clear text buffer\r\n";
		//            oStr += " | fps   - request frame rate\r\n";
		//            oStr += " | exit  - close console frame\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "\r\n";
		//            //oStr += "--";
		//            oStr += " ------------------------------------------------\r\n";
		//        }
		//        return oStr;
		//    }
		//    public static string NN(string iArgs)
		//    {
		//        ///Workarounds.NGonSchemeFrame.SelectedNodes.ForEach(cNode => cNode.Name = iArgs);

		//        return null;//"Changed node name(s) = '" + iArgs + "'";
		//    }
		//    public static string NT(string iArgs)
		//    {
		//        ///Workarounds.NGonSchemeFrame.SelectedNodes.ForEach(cNode => cNode.Type = iArgs);

		//        return null;
		//    }
		//    public static string ND(string iArgs)
		//    {
		//        ///Workarounds.NGonSchemeFrame.SelectedNodes.ForEach(cNode => cNode.Description = iArgs);

		//        return null;
		//    }
		//}
	}
}
