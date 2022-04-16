using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
//using System.Windows.Forms;
using AE.Visualization;

/**
G.MCon.Message();

G.D.Message
*/

namespace AE
{
	public static class G
	{
		public static ApplicationBase Application;
		public static Canvas          Screen;
		
		public static Tools.Console   Console;
		public static Tools.Console   Debug;

		public static Simulation.SimEngine     Simulator;
		public static Simulation.DynamicObject Vehicle;

		
		
		

		static G()
		{
			Console = new Tools.Console();
			Debug   = new Tools.Console();

			G.Simulator = new AE.Simulation.SimEngine();
			G.Vehicle   = G.Simulator.Objects[0] as Simulation.DynamicObject;
			//Console
		}

		static void Main(string[] iArgs)
		{
			G.Application = new ApplicationBase();
			G.Screen      = null;
		}
	}
	public class ApplicationBase
	{
		public Dictionary<string, CommandDelegate> Commands = new Dictionary<string,CommandDelegate>();

		public string SampleText;
		public string[] SampleLines;

		public ApplicationBase()
		{
			var _SampleFilePath = "0.src";

			if(System.IO.File.Exists(_SampleFilePath))
			{
				this.SampleText = System.IO.File.ReadAllText(_SampleFilePath);
				this.SampleLines = this.SampleText.Split(new string[]{"\r\n"}, StringSplitOptions.None);
			}

			this.AddCommands(this);
		}

		public virtual void OnLoad()
		{
			///throw new NotImplementedException();

			G.Console.Message("*ApplicationBase.OnLoad");


			///foreach(var cPair in this
		}
		public virtual void OnUnload()
		{
			///throw new NotImplementedException();
		}
		public virtual void Run()
		{
			throw new NotImplementedException();
		}
		public virtual void Exit()
		{
			System.Windows.Forms.Application.Exit();
			///G.Screen.ParentForm.Close();
			//throw new NotImplementedException();
		}

		//public static 
		public void AddCommands(object iObject)
		{
			var _CmdMM = iObject.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
			{
				foreach(var cCmdM in _CmdMM)
				{
					if(!cCmdM.Name.StartsWith("_")) continue;

					CommandInfoAttribute cCmdInfo = null;
					{
						foreach(CommandInfoAttribute cAttr in cCmdM.GetCustomAttributes(true))
						{
							cCmdInfo = cAttr;
							//if(cAttr is CommandInfoAttribute)
							//{
								
							//}
						}
						if(cCmdInfo == null) cCmdInfo = new CommandInfoAttribute("???");
						if(cCmdInfo.Name == null)
						{
							cCmdInfo.Name = cCmdM.Name.Substring(1).ToLower();
						}
					}



					var cCmdD = (CommandDelegate)Delegate.CreateDelegate(typeof(CommandDelegate), iObject, cCmdM.Name);

					this.Commands.Add(cCmdInfo.Name, cCmdD);
					///Commands.Register(cCmdInfo, cCmdD);
				}

			}
		}
		public void DeleteCommands(object iObject)
		{
			throw new NotImplementedException();
		}

		[CommandInfo("exit application")]
		protected virtual string _Exit(string iArgs)
		{
			G.Application.Exit();
			///Application.MainForm.Close();

			return null;
		}
		[CommandInfo("clear console buffer")]
		protected virtual string _Clear(string iArgs)
		{
			G.Console.Frame.History.Clear();

			return null;
		}

		[CommandInfo("get current frames-per-second value")]
		protected virtual string _Fps(string iArgs)
		{
			return Math.Round(G.Screen.AverageFrameRate).ToString();
		}

		[CommandInfo("convert numeral system")]
		protected virtual string _Num(string iArgs)
		{
			var _Args = iArgs.Split(' ');

			var _NumIn  = MathEx.ArbitraryToDecimalSystem(_Args[0], Int32.Parse(_Args[1]));
			var _NumOut = MathEx.DecimalToArbitrarySystem(_NumIn, Int32.Parse(_Args[2]));

			return _Args[1] + " -> " + _Args[2] + " = " + _NumOut;
		}
	}
	public struct Tools
	{
		public class Console
		{
			public BufferConsoleFrame Frame;

			public AudioPlayer Audio;// = new AudioPlayer();

			//public static void Echo(string iStr)
			//{
			//    if(Frame == null) return;
				
			//    //GLO.Line("Position",_PosV);
			//    Frame.WriteLine(iStr);
			//}
			public void Message(string iStr)
			{
				if(this.Frame == null || this.Frame.BufferSize.IsEmpty) return;

				this.Frame.IsLoggingMode = true;//.Logg

				byte _ForeColor = 2, _BackColor = 0;
				{
					if(iStr.StartsWith("!")){_ForeColor  = 3; iStr = iStr.Substring(1);}
					if(iStr.StartsWith("*")){_ForeColor  = 4; iStr = iStr.Substring(1);}

					
					
					//if(iStr.Contains("function")) _DefaultColor = this.Palette.GetAdaptedColor(Color.Red);
					//else if(iStr.Contains("var")) _DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange);
				}
				this.Frame.SetForeColor(_ForeColor);
				this.Frame.SetBackColor(_BackColor);

				this.Frame.WriteLine(iStr, true);
			}
			public void Message(object iObj)
			{
				this.Message(iObj.ToString());
			}
			public void Beep(double iFrequency, int iDuration)
			{
				if(this.Audio == null) return;
				
				this.Audio.PlayBeep(iFrequency, iDuration);
				//GLO.Line("Position",_PosV);
				//Frame.WriteLine(iStr);
			}
			public void StartSound(double iFrequency, float[] iWaveform)
			{
				if(this.Audio == null) return;
				
				this.Audio.PlaySound(iFrequency, iWaveform);
				//GLO.Line("Position",_PosV);
				//Frame.WriteLine(iStr);
			}
			public void Clear()
			{
				this.Frame.History.Clear();
				this.Frame.CursorPosition.X = 0;
				this.Frame.CursorPosition.Y = 0;

				this.Frame.NeedsCompleteUpdate = true;
				//this.Frame.IsLoggingMode = false;


				this.Frame.Reset(); ///??
			}
		}

	}
	
	//public static class GCon
	//{
	//    public static Visualization.BufferConsoleFrame Frame;//.IConsole

	//    public static AudioPlayer Audio;// = new AudioPlayer();

	//    //public static void Echo(string iStr)
	//    //{
	//    //    if(Frame == null) return;
			
	//    //    //GLO.Line("Position",_PosV);
	//    //    Frame.WriteLine(iStr);
	//    //}
	//    public static void Message(string iStr)
	//    {
	//        if(Frame == null) return;
	//        if(Frame.BufferSize.IsEmpty) return;


	//        Frame.IsLoggingMode = true;//.Logg

	//        byte _ForeColor = 2, _BackColor = 0;
	//        {
	//            if(iStr.StartsWith("!")){_ForeColor  = 3; iStr = iStr.Substring(1);}
	//            if(iStr.StartsWith("*")){_ForeColor  = 6; iStr = iStr.Substring(1);}

				
				
	//            //if(iStr.Contains("function")) _DefaultColor = this.Palette.GetAdaptedColor(Color.Red);
	//            //else if(iStr.Contains("var")) _DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange);
	//        }
	//        Frame.SetForeColor(_ForeColor);
	//        Frame.SetBackColor(_BackColor);

	//        Frame.WriteLine(iStr, true);
	//    }
	//    public static void Message(object iObj)
	//    {
	//        Message(iObj.ToString());
	//    }
	//    public static void Beep(double iFrequency, double iDuration)
	//    {
	//        if(Audio == null) return;
			
	//        Audio.PlayBeep(iFrequency, iDuration);
	//        //GLO.Line("Position",_PosV);
	//        //Frame.WriteLine(iStr);
	//    }
	//    public static void StartSound(double iFrequency)
	//    {
	//        if(Audio == null) return;
			
	//        Audio.PlaySound(iFrequency);
	//        //GLO.Line("Position",_PosV);
	//        //Frame.WriteLine(iStr);
	//    }
	//    public static void Clear()
	//    {
	//        Frame.Reset();
	//    }
	//}
	//public static class DCon
	//{
	//    public static Visualization.BufferConsoleFrame Frame;//.IConsole

	//    public static void Message(string iStr)
	//    {
	//        if(Frame == null) return;
	//        if(Frame.BufferSize.IsEmpty) return;

	//        Frame.IsLoggingMode = false;//.Logg

	//        byte _ForeColor = 2, _BackColor = 0;
	//        {
	//            if(iStr.StartsWith("!")){_ForeColor = 3; iStr = iStr.Substring(1);}
	//            if(iStr.StartsWith("*")){_ForeColor = 6; iStr = iStr.Substring(1);}

				
				
	//            //if(iStr.Contains("function")) _DefaultColor = this.Palette.GetAdaptedColor(Color.Red);
	//            //else if(iStr.Contains("var")) _DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange);
	//        }
	//        Frame.SetForeColor(_ForeColor);
	//        Frame.SetBackColor(_BackColor);

	//        Frame.WriteLine(iStr, true);
	//    }
	//    public static void Message(object iObj)
	//    {
	//        Message(iObj.ToString());
	//    }
	
	//    public static void Clear()
	//    {
	//        if(Frame == null) return;
	//        Frame.Reset();
	//    }

	//    public static void Line(string iStr)
	//    {
	//        Message(iStr);
	//    }
	//    public static void Table(params object[] iCells)
	//    {
	//        throw new NotImplementedException();
	//        //Message(iCell);
	//    }
	//}
	
	
	//static partial class Application
	//{
	//    public static DataNode               Configuration;
	//    public static MainForm               MainForm;
	//    //public static GraphicsConsole Console = new GraphicsConsole(30);
	//    //public static GraphicsConsole Console = new GraphicsConsole(30);
		
	//    public static string          SampleFilePath = @"0.src";
	//    public static string[]        SampleText     = new string[]{"","",""};
	//    //public static string[]        SampleText = System.IO.File.ReadAllLines(@"License.txt");
	//    public static int             SampleLine     = 0;

	//    static Application()
	//    {
	//        if(System.IO.File.Exists(SampleFilePath)) SampleText = System.IO.File.ReadAllLines(SampleFilePath);;
	//    }
		

	//    //static Image    Img;
	//    //static Graphics Grx;

	//    static void Main()
	//    {
	//        Application.Routines.LoadDefaultConfiguration();
	//        Application.Routines.LoadConfiguration();

	//        MainForm = new MainForm();

	//        //GCon.Frame = MainForm.Viewport.Frame.C
	//        //Console  = new Console  (30);
	//        //MainForm.Show();
	//        //Application.TextSample = System.IO.File.ReadAllLines(@"");

	//        Application.Routines.ReadConfiguration();
			
				
	//        GCon.Audio = new AudioPlayer();



	//        //Img = new Bitmap(1,1);
	//        //Grx = Graphics.FromImage(Img);
	//        //Grx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;//.AntiAliasGridFit;
	//        //_Grx.Dispose();

	//        //System.Windows.Forms.Application.Run(new Form1());
	//        System.Windows.Forms.Application.Run(MainForm);

	//        Application.Routines.WriteConfiguration();
	//        Application.Routines.SaveConfiguration();

	//        //System.Console.WriteLine("Quit!");
			
	//    }
	//    internal static void OnLoad()
	//    {
		
	//        ///RunProcTest();
	//        //_.
	//    }
	//    static void RunProcTest()
	//    {
	//        for(var cI = 0; cI < 30; cI++)
	//        {
	//            var cStr = "";// + cI;

	//            for(var cChar = 0; cChar < cI; cChar++)
	//            {
	//                cStr += "$";
	//            }

	//            //new String(
	//            GCon.Message((cI % 2 == 0 ? "*" : "") + cI + " | " + cStr);
	//        }
	//        //return;
			
	//        var _Proc = new System.Diagnostics.Process();
	//        {
	//            var _StartI = _Proc.StartInfo;
	//            {
	//                _StartI.UseShellExecute        = false;
	//                _StartI.RedirectStandardOutput = true;
	//                _StartI.FileName = "cmd";
	//                //_StartI.FileName = "chkdsk.exe";
	//                _StartI.Arguments = "/c dir /s";
	//                _StartI.WorkingDirectory = @"L:\";//Environment.GetFolderPath(Environment.SpecialFolder.System);

					
	//            }

	//            _Proc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(_Proc_OutputDataReceived);
	//            //_Proc.StandardOutput.
	//            //_Proc.StartInfo.Verb = "Print";
	//            //_Proc.StartInfo.CreateNoWindow = true;
				

	//            //_Proc.
	//        }
	//        _Proc.Start();
	//        _Proc.BeginOutputReadLine();
	//    }
	//    static void _Proc_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs iEvent)
	//    {
	//        var _SrcStr = iEvent.Data; if(String.IsNullOrEmpty(_SrcStr)) return;
	//        var _SrcBytes = Encoding.Default.GetBytes(_SrcStr);

	//        //var _Encoding.GetEncoding(866)

	//        var _ConvStr = Encoding.GetEncoding(866).GetString(_SrcBytes);

	//        //var _Bytes = Encoding.ASCII.GetBytes(_Str);
	//        //var _StrUtf = Encoding.UTF8.GetString(_Bytes);

	//        GCon.Message(_ConvStr);
	//        ///Console.WriteLine(_ConvStr);
	//        //throw new NotImplementedException();
	//    }

	//    //static void LoadTextSample()
	//    //{
	//    //    Application.TextSample = System.IO.File.ReadAllLines(@"");
	//    //}
	//}
}
