using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using AE.Visualization;
using AE.Simulation;
using SOM = AE.Visualization.SchemeObjectModel;

namespace AE
{
	///public class NGonSchemeFrame : Frame{}

	public partial class Studio : ApplicationBase
	{
		public DataNode        Configuration;
		public MainForm        MainForm;
		public NGonSchemeFrame NGonSchemeFrame;
		public ZoomableFrame   AnyZoomableFrame;
		
		
		public string    SampleFilePath = @"0.src";
		//public string[]  SampleText     = new string[]{"","",""};
		public int       SampleLine     = 0;


		public Studio()
		{
			///if(System.IO.File.Exists(this.SampleFilePath)) this.SampleText = System.IO.File.ReadAllLines(SampleFilePath);;
			if(System.IO.File.Exists(this.SampleFilePath)) this.SampleText = System.IO.File.ReadAllText(SampleFilePath);;

		}

		public override void OnLoad()
		{
			base.OnLoad();

			GLCanvasControl.LowResFontAtlas.CreateImage();
			GLCanvasControl.LowResFontAtlas.CreateTexture();
			
			///GLCanvasControl.HighResFontAtlas.CreateImage();
			///GLCanvasControl.HighResFontAtlas.CreateTexture();

			//NGonSchemeFrame.Handlers.Register();
			AE.Commands.Add(this);
			//Canvas.LowResFontAtlas.Image.Save("LowResFontAtlas.png");
			//Canvas.HighResFontAtlas.Image.Save("HighResFontAtlas.png");
		}
		public override void OnUnload()
		{
			base.OnUnload();

			AE.Commands.Delete(this);
		}
		//static Image    Img;
		//static Graphics Grx;
		//[STAThread
		public override void Run()
		{
			this.LoadDefaultConfiguration();
			this.LoadConfiguration();

			this.MainForm = new MainForm();
			{
				this.InitWorkspace(this.MainForm.Screen.Canvas);
				this.NGonSchemeFrame = this.MainForm.Screen.Canvas.Frame.GetAllChildFrames().Find(cChild => (cChild is NGonSchemeFrame)) as NGonSchemeFrame;
				this.AnyZoomableFrame = this.MainForm.Screen.Canvas.Frame.GetAllChildFrames().Find(cChild => (cChild is ZoomableFrame)) as ZoomableFrame;

				G.Screen = this.MainForm.Screen.Canvas;
				//G.Screen.MainFrame = this.MainForm.Screen.Frame.GetAllChildFrames().Find(cChild => (cChild.Name == "MainConsole")) as BufferConsoleFrame;
			}

			//GCon.Frame = MainForm.Viewport.Frame.C
			//Console  = new Console  (30);
			//MainForm.Show();
			//Application.TextSample = System.IO.File.ReadAllLines(@"");

			this.ReadConfiguration();
			
				
			G.Console.Audio = new AudioPlayer();



			//Img = new Bitmap(1,1);
			//Grx = Graphics.FromImage(Img);
			//Grx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;//.AntiAliasGridFit;
			//_Grx.Dispose();

			//System.Windows.Forms.Application.Run(new Form1());
			G.Screen.OnLoad(null);

			System.Windows.Forms.Application.Run(this.MainForm);

			this.WriteConfiguration();
			this.SaveConfiguration();

			//System.Console.WriteLine("Quit!");

		}
		[STAThread]
		static void Main(string[] iArgs)
		{
			RunTest();


			Studio.Args = iArgs;

			G.Application = new Studio();
			G.Application.Run();
		}
	}
	//public partial class Studio : ApplicationBase
	//{
	//    public static DataNode               Configuration;
	//    public static MainForm               MainForm;
	//    //public static GraphicsConsole Console = new GraphicsConsole(30);
	//    //public static GraphicsConsole Console = new GraphicsConsole(30);
		
	//    public static string          SampleFilePath = @"0.src";
	//    public static string[]        SampleText     = new string[]{"","",""};
	//    //public static string[]        SampleText = System.IO.File.ReadAllLines(@"License.txt");
	//    public static int             SampleLine     = 0;

	//    static Studio()
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
	//        {
	//            Routines.InitWorkspace(MainForm.Screen);
	//        }

	//        //GCon.Frame = MainForm.Viewport.Frame.C
	//        //Console  = new Console  (30);
	//        //MainForm.Show();
	//        //Application.TextSample = System.IO.File.ReadAllLines(@"");

	//        Application.Routines.ReadConfiguration();
			
				
	//        G.Console.Audio = new AudioPlayer();



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
	//            G.Console.Message((cI % 2 == 0 ? "*" : "") + cI + " | " + cStr);
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

	//        G.Console.Message(_ConvStr);
	//        ///Console.WriteLine(_ConvStr);
	//        //throw new NotImplementedException();
	//    }

	//    //static void LoadTextSample()
	//    //{
	//    //    Application.TextSample = System.IO.File.ReadAllLines(@"");
	//    //}
	//}
}
