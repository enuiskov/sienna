using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using AE.Visualization;
using SOM = AE.Visualization.SchemeObjectModel;
//using AE.Data.DescriptionLanguage;

namespace AE
{
	public partial class Studio : ApplicationBase
	{
		static void RunTest()
		{
			return;
			//AEDLNodeConverter();
			//ProcTest();



			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}


		static void AEDLNodeConverter()
		{
			//var _Lexer  = new Lexer();
			//var _Parser = new ASTParser();

			/////var _Tokens = _Lexer.ParseBuffer("\"Config\" (\"attrName1\" \"AttrValue1\"; \"attrName2\" \"AttrValue2\"; \"NodeValue1\"; \"NodeValue2\"; \"NodeValue3\"; \"Settings\" (\"attr1\" \"val1\"); );");
			//var _Tokens = _Lexer.ParseBuffer("\"Config\"(\"attrName1\"\"AttrValue1\";\"attrName2\"\"AttrValue2\";\"Settings\"(\"attr1\"\"val1\"););");
			//var _AST    = _Parser.ParseTokens(_Tokens);

			//var _XmlNode = Converter.ToDataNode(_AST.Children[0], null);
		}
		static void AEDLToDataNode()
		{

		}
		static void DataNodeToAEDL()
		{
			
		}
		static void ProcTest()
		{
			for(var cI = 0; cI < 30; cI++)
			{
				var cStr = "";// + cI;

				for(var cChar = 0; cChar < cI; cChar++)
				{
				    cStr += "$";
				}

				//new String(
				G.Console.Message((cI % 2 == 0 ? "*" : "") + cI + " | " + cStr);
			}
			//return;
			
			var _Proc = new System.Diagnostics.Process();
			{
				var _StartI = _Proc.StartInfo;
				{
					_StartI.UseShellExecute        = false;
					_StartI.RedirectStandardOutput = true;
					_StartI.FileName = "cmd";
					//_StartI.FileName = "chkdsk.exe";
					_StartI.Arguments = "/c dir /s";
					_StartI.WorkingDirectory = @"L:\";//Environment.GetFolderPath(Environment.SpecialFolder.System);

					
				}

				_Proc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(_Proc_OutputDataReceived);
				//_Proc.StandardOutput.
				//_Proc.StartInfo.Verb = "Print";
				//_Proc.StartInfo.CreateNoWindow = true;
				

				//_Proc.
			}
			_Proc.Start();
			_Proc.BeginOutputReadLine();
		}
		static void _Proc_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs iEvent)
		{
			var _SrcStr = iEvent.Data; if(String.IsNullOrEmpty(_SrcStr)) return;
			var _SrcBytes = Encoding.Default.GetBytes(_SrcStr);

			//var _Encoding.GetEncoding(866)

			var _ConvStr = Encoding.GetEncoding(866).GetString(_SrcBytes);

			//var _Bytes = Encoding.ASCII.GetBytes(_Str);
			//var _StrUtf = Encoding.UTF8.GetString(_Bytes);

			G.Console.Message(_ConvStr);
			///Console.WriteLine(_ConvStr);
			//throw new NotImplementedException();
		}

		//static void LoadTextSample()
		//{
		//    Application.TextSample = System.IO.File.ReadAllLines(@"");
		//}

	}
}
