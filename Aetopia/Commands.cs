using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
//using AE.DescriptionLanguage;
using SOM = AE.Visualization.SchemeObjectModel;

namespace AE
{
	public class CommandInfoAttribute : Attribute
	{
		public string Name        = "?NAME?";
		public string Description = "?DESCRIPTION?";
		public string Details     = "?DETAILS?";

		public CommandInfoAttribute(string iDescription) : this(null, iDescription){}
		public CommandInfoAttribute(string iName, string iDescription) : this(iName, iDescription, null){}
		public CommandInfoAttribute(string iName, string iDescription, string iDetails)
		{
			this.Name        = iName;
			this.Description = iDescription;
			this.Details     = iDetails;
		}

		//public CommandInfoAttribute(string iName) : this(iName, null){}
		//public CommandInfoAttribute(string iName, string iDescription) : this(iName, iDescription, null){}
		//public CommandInfoAttribute(string iName, string iDescription, string iDetails)
		//{
		//    this.Name        = iName;
		//    this.Description = iDescription;
		//    this.Details     = iDetails;
		//}
	}
	public class CommandCollection
	{
		public Dictionary<string, CommandDelegate> Table = new Dictionary<string,CommandDelegate>();

		public void UpdateTable()
		{
			///enumerate and register all methods with CommandInfoAttribute
			throw new NotImplementedException();
		}
	}
	

	public delegate string CommandDelegate(string iArgs);
	public static partial class Commands
	{
		public static Dictionary<string, CommandDelegate> Table = new Dictionary<string,CommandDelegate>();
		public static bool IsScriptMode = false;

		public static void Add(object iObject)
		{
			//var _CmdMM = iObject.GetType().GetMethods();
			//{
			//    foreach(var cCmdM in _CmdMM)
			//    {
			//        if(!cCmdM.Name.StartsWith("_")) continue;

			//        CommandInfoAttribute cCmdInfo = null;
			//        {
			//            foreach(CommandInfoAttribute cAttr in cCmdM.GetCustomAttributes(true))
			//            {
			//                cCmdInfo = cAttr;
			//                //if(cAttr is CommandInfoAttribute)
			//                //{
								
			//                //}
			//            }
			//            if(cCmdInfo == null) cCmdInfo = new CommandInfoAttribute("???");
			//            if(cCmdInfo.Name == null)
			//            {
			//                cCmdInfo.Name = cCmdM.Name.Substring(1).ToLower();
			//            }
			//        }



			//        var cCmdD = (CommandDelegate)Delegate.CreateDelegate(typeof(CommandDelegate), cCmdM);

			//        Commands.Register(cCmdInfo, cCmdD);
			//    }
			//}
		}
		public static void Delete(object iObject)
		{
			throw new NotImplementedException();
		}
		//public static void Add(CommandCollection iCommands)
		//{
		//    foreach(var cPair in iCommands.Table)
		//    {
		//        Table.Add(cPair.Key, cPair.Value);
		//    }
		//    //iCommands.UpdateTable();
		//}

		public static void Register(CommandInfoAttribute iInfo, CommandDelegate iHandler)
		{
			Table[iInfo.Name] = iHandler;
		}

		public static string ProcessInput(string iInput)
		{
			iInput = iInput.Trim();

			if(iInput.Length == 0) return null;

			if     (iInput == "scr"){IsScriptMode = true;  return "script mode";}
			else if(iInput == "cmd"){IsScriptMode = false; return "command mode";}

			if(IsScriptMode)
			{
				return ProcessScript(iInput);
			}
			else
			{
				return ProcessCommand(iInput);
			}
		}
		public static string ProcessCommand(string iInput)
		{
			string _CmdName = iInput, _CmdArgs = "";
			{
				var _WsI = iInput.IndexOf(" "); if(_WsI != -1)
				{
					_CmdName = iInput.Substring(0, _WsI);
					_CmdArgs = iInput.Substring(_WsI + 1);
				}
			}
			if(G.Application.Commands.ContainsKey(_CmdName))
			{
				var _Output = G.Application.Commands[_CmdName](_CmdArgs);
				///if(_Res != null)
				return _Output;
			}
			else if(_CmdName == "?")
			{
				var _CmdList = new Dictionary<string,string>(G.Application.Commands.Count);
				var _MaxCmdNameLen = 0;
				{
					foreach(var cCmd in G.Application.Commands)
					{
						var cCmdInfo = (cCmd.Value.Method.GetCustomAttributes(false)[0] as CommandInfoAttribute);

						//var cCmdDesc = .Description;
						//cCmd.Value.Method.Name
						//_CmdList += "  " + cCmd.Key + " - " + cCmdDesc +  "\r\n";
						var cCmdName = cCmdInfo.Name ?? cCmd.Key;
						_CmdList[cCmdName] = cCmdInfo.Description;

						if(cCmdName.Length > _MaxCmdNameLen) _MaxCmdNameLen = cCmdName.Length;
					}
				}
				var _Spaces = "                                               ";
				var _CmdListStr = ""; foreach(var cCmd in _CmdList)
				{
					_CmdListStr += "  " + cCmd.Key + (_Spaces.Substring(0,_MaxCmdNameLen - cCmd.Key.Length)) + " - " + cCmd.Value +  "\r\n";
				}

				//var _CmdList = ""; foreach(var cCmd in G.Application.Commands)
				//{
				//    var cCmdDesc = (cCmd.Value.Method.GetCustomAttributes(false)[0] as CommandInfoAttribute).Description;
				//    //cCmd.Value.Method.Name
				//    _CmdList += "  " + cCmd.Key + " - " + cCmdDesc +  "\r\n";
				//}
				return "<8>Supported commands\r\n" + _CmdListStr;
			}
			else return "<3>'" + _CmdName + "' - command not supported";
		
		}
		public static string ProcessScript(string iInput)
		{
			//var _Lexer   = new DescriptionLanguage.Lexer();
			//var _Parser  = new DescriptionLanguage.ASTParser();

			//var _Tokens  = _Lexer.ParseBuffer(iInput);
			//var _StxNode = _Parser.ParseTokens(_Tokens);
			
			//return _StxNode.ToString();
	
			return "STUB";
		}

		//public class CommandHandler
		//{
		//    public string Name;
		//    public string ProcessCommand(string iArgs)
		//    {
		//        return "--";
		//    }
		//}
		//class ClearConsoleCommand
		//{

		//}


		public struct Handlers
		{
			//public static void BatchRegister()
			//{
			//    var _CmdMM = typeof(Handlers).GetMethods(BindingFlags.Public | BindingFlags.Static);
			//    {
			//        foreach(var cCmdM in _CmdMM)
			//        {
			//            if(cCmdM.Name == "BatchRegister") continue;

			//            //cCmdF.Inv
			//            //Delegate.CreateDelegate(
			//            var cCmdD = (CommandDelegate)Delegate.CreateDelegate(typeof(CommandDelegate), cCmdM);

			//            Commands.Register(cCmdM.Name.ToLower(), cCmdD);
			//        }
			//    }


			//    //Commands.Register("clear", new CommandDelegate(Clear));
			//}
			//[CommandInfo("exit application")]
			//private string _Exit(string iArgs)
			//{
			//    G.Application.Exit();
			//    ///Application.MainForm.Close();

			//    return null;
			//}
			//[CommandInfo("clear console buffer")]
			//private string _Clear(string iArgs)
			//{
			//    G.Console.Frame.History.Clear();

			//    return null;
			//}

			//[CommandInfo("get current frames-per-second value")]
			//private string _Fps(string iArgs)
			//{
			//    return Math.Round(G.Screen.AverageFrameRate).ToString();
			//}

			//[CommandInfo("get command list")]
			//public static string Help(string iArgs)
			//{
			//    var oStr = " Supported commands\r\n";
			//    {
			//        //oStr += " Type 'cmdname /?' for more detailed information:\r\n";
			//        oStr += " ------------------------------------------------\r\n";
			//        oStr += " | clear - clear text buffer\r\n";
			//        oStr += " | fps   - request frame rate\r\n";
			//        oStr += " | exit  - close console frame\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "\r\n";
			//        //oStr += "--";
			//        oStr += " ------------------------------------------------\r\n";
			//    }
			//    return oStr;
			//}
			
		}
	}
}
