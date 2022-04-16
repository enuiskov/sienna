using System;
using System.Collections.Generic;
using System.Text;
//using System.IO;
using System.Drawing;
//using System.Windows.Forms;
using AE.Visualization;

namespace AE
{
	public partial class Studio : ApplicationBase
	{
		private static string   DefaultConfigurationPath = @"Defaults.xml";
		public  static DataNode DefaultConfiguration;
		public  static string[] Args;
		//public void InitWorkarounds(Canvas iCanvas)
		//{
		//    //var _FontEditor = new FontEditorFrame();
		//    //Canvas.Font = _FontEditor.Font.Cells;
		//}
		public void InitWorkspace(Canvas iCanvas)
		{
			var _ClockPatt = "-clock";
			var _NoisePatt = "-noise";

			var _Args = Studio.Args;
			var _AppMode = "";
			{
				if(_Args.Length >= 1)
				{
					_AppMode = _Args[0];
				}

				if(_AppMode == "") _AppMode = "NGonSchemeFrame";

				if(_AppMode == _ClockPatt || _AppMode == _NoisePatt)
				{
					_AppMode = "";
				}
			}

			var _IsClock = (_Args.Length >= 1 && _Args[0] == _ClockPatt) || (_Args.Length >= 2 && _Args[1].Contains(_ClockPatt)) || (_Args.Length >= 3 && _Args[2].Contains(_ClockPatt));
			var _IsNoise = (_Args.Length >= 1 && _Args[0] == _NoisePatt) || (_Args.Length >= 2 && _Args[1].Contains(_NoisePatt)) || (_Args.Length >= 3 && _Args[2].Contains(_NoisePatt));


			//this.InitWorkarounds(iCanvas);
			iCanvas.Frame = new RootFrame{Name = "RootFrame", Palette = new GdiColorPalette(), Canvas = iCanvas, Dock = DockStyle.Fill};
			{
				switch(_AppMode)
				{
					case "NGonSchemeFrame"         : iCanvas.Frame.Children.Add(new NGonSchemeFrame          {Name = "NGonScheme",      Bounds = new Rectangle(0,0,600,500),  Palette = new GdiColorPalette(),  Dock = DockStyle.Fill}); break;
					case "TerrainEditorFrame"      : iCanvas.Frame.Children.Add(new TerrainEditorFrame       {Name = "TerrainEditor",   Bounds = new Rectangle(0,0,600,500),  Palette = new GdiColorPalette(),  Dock = DockStyle.Fill}); break;
					case "FontEditorFrame"         : iCanvas.Frame.Children.Add(new FontEditorFrame          {Name = "FontEditor",      Bounds = new Rectangle(0,0,600,500),  Palette = new GdiColorPalette(),  Dock = DockStyle.Fill, IsPerspectiveMode = true}); break;
					case "TextEditorFrame"         : iCanvas.Frame.Children.Add(new TextEditorFrame          {Name = "TextEditor",      Bounds = new Rectangle(0,0,640,480), Palette = new GdiColorPalette(0.0,0,0,0.9), Dock = DockStyle.Left, IsPointerIgnored = false}); break;
					case "TextBufferFrame"         : iCanvas.Frame.Children.Add(new TextBufferFrame          {Name = "TextBuffer",      Bounds = new Rectangle(0,0,640,480), Palette = new GdiColorPalette(0.5,2.5,0.5), Dock = DockStyle.Left, IsPointerIgnored = false}); break;
					case "CodeEditorFrame"         : iCanvas.Frame.Children.Add(new CodeEditorFrame          {Name = "TextEditor",      Bounds = new Rectangle(10,10,640,480), Palette = new GdiColorPalette(0.5,2.5,0.5), Dock = DockStyle.None, IsPointerIgnored = false}); break;
					case "TestCrossFrame"          : iCanvas.Frame.Children.Add(new TestCrossFrame           {Name = "TestCross",       Bounds = new Rectangle(0,0,640,480), Palette = new GdiColorPalette(0.5,2.5,0.5), Dock = DockStyle.Left, IsPointerIgnored = false}); break;
					//case "" : iCanvas.Frame.Children.Add(new CodeEditorGdiGLFrame   {Name = "CodeEditorGDI", Bounds = new Rectangle(0,0,512,400), Dock = DockStyle.Fill, Palette = new GdiColorPalette(0.025f), IsPointerIgnored = false}); break;
					case "ModelViewer"             : iCanvas.Frame.Children.Add(new ModelViewer              {Name = "ModelViewer",     Bounds = new Rectangle(0,0,600,500),  Palette = new GdiColorPalette(),  Dock = DockStyle.Fill});break;
					case "VBOTest"                 : iCanvas.Frame.Children.Add(new VBOTest                  {Name = "VBOTest",         Bounds = new Rectangle(0,0,600,500),  Palette = new GdiColorPalette(),  Dock = DockStyle.Fill}); break;
					case "TriangleTesselationTest" : iCanvas.Frame.Children.Add(new TriangleTesselationTest  {Name = "IcosahedronTest", Bounds = new Rectangle(0,0,600,500),  Palette = new GdiColorPalette(),  Dock = DockStyle.Fill}); break;
					case "IcosahedronTest"         : iCanvas.Frame.Children.Add(new IcosahedronTest          {Name = "IcosahedronTest", Bounds = new Rectangle(0,0,600,500),  Palette = new GdiColorPalette(),  Dock = DockStyle.Fill}); break;
					
					//case "" : iCanvas.Frame.Children.Add(new CodeEditorFrame          {Name = "CodeEditor",            Margin = new Padding(20,20,300,200), Palette = new GdiColorPalette(0,0,0,0.98), Dock = DockStyle.None, IsPointerIgnored = false}); break;
					case "Frame3D"                 : iCanvas.Frame.Children.Add(new Frame3D                  {Name = "3D",              Bounds = new Rectangle(0,0,100,100), Palette = new GdiColorPalette(0.2f), Margin = new Padding(0,0,-1,-1)}); break;
					//case "" : iCanvas.Frame.Children.Add(new FastLineConsoleFrame     {Name = "F3",              Bounds = new Rectangle(0,0,500,300), Palette = new GdiColorPalette(0.5f), Dock = DockStyle.Fill}); break;
					//case "TextBufferFrame"         : iCanvas.Frame.Children.Add(new TextBufferFrame          {Name = "F3",              Bounds = new Rectangle(0,0,500,300), Palette = new GdiColorPalette(0.5f), Dock = DockStyle.Fill}); break;
					case "BufferConsoleFrame1"     : iCanvas.Frame.Children.Add(new BufferConsoleFrame       {Name = "F3",              Bounds = new Rectangle(0,0,500,300), Palette = new GdiColorPalette(0.5f), Dock = DockStyle.Fill}); break;
					case "BufferConsoleFrame2"     : iCanvas.Frame.Children.Add(new BufferConsoleFrame       {Name = "BufferConsole",   Bounds = new Rectangle(0,0,300,50), Palette = new GdiColorPalette(0.5f), Dock = DockStyle.Top}); break;



					case "NGonSchemeToolboxFrame"  : iCanvas.Frame.Children.Add(new NGonSchemeToolboxFrame   {Name = "Watch",            Bounds = new Rectangle(0,0,50,300),    Palette = new GdiColorPalette(0.2f), Margin = new Padding(0,0,-1,-1), IsVisible = false}); break;
					
					
					case "ImageGeneratorFrame"     : iCanvas.Frame.Children.Add(new ImageGeneratorFrame      {Name = "ImageGenerator",   Bounds = new Rectangle(10,10,512,512), Palette = new GdiColorPalette(0.2f)}); break;
					case "KeyMatrixFrame"          : iCanvas.Frame.Children.Add(new KeyMatrixFrame           {Name = "KeyMatrix",        Bounds = new Rectangle(10,10,750,400), Palette = new GdiColorPalette(0.3f,1.0),   Dock = DockStyle.Fill}); break;
					case "Frame"                   : iCanvas.Frame.Children.Add(new Frame                    {Name = "???",              Bounds = new Rectangle(0,0,100,100), Palette = new GdiColorPalette(0.1f),   Margin = new Padding(-1,-1,0,-1)}); break;
					//case "" : iCanvas.Frame.Children.Add(new GDIConsoleFrame          {Name = "F3",               Bounds = new Rectangle(0,0,500,300), Palette = new GdiColorPalette(0.5f), Dock = DockStyle.Fill}); break;
					//case "" : iCanvas.Frame.Children.Add(new SubTexTestFrame          {Name = "F3",               Bounds = new Rectangle(0,0,500,300), Palette = new GdiColorPalette(0.5f), Dock = DockStyle.Fill}); break;
					//case "" : iCanvas.Frame.Children.Add(new ConsoleFrame             {Name = "F3",               Bounds = new Rectangle(0,0,400,300), Palette = new GdiColorPalette(0.3f), Margin = new Padding(-1,-1,0,0)}); break;
					case "TestFrame"               : iCanvas.Frame.Children.Add(new TestFrame                {Name = "Test",             Bounds = new Rectangle(0,0,400,300), Palette = new GdiColorPalette(0.7f, 0,1, 1), Margin = new Padding(-1,-1,0,0)}); break;
					case "AudioTestFrame"          : iCanvas.Frame.Children.Add(new AudioTestFrame           {Name = "AudioTest",        Bounds = new Rectangle(0,0,150,80), Palette = new GdiColorPalette(0.0f,0.9f,1f,0.9f), Margin = new Padding(-1,-1,0,0)}); break;
					case "AudioPitchTestFrame"     : iCanvas.Frame.Children.Add(new AudioPitchTestFrame      {Name = "AudioPitchTest",   Bounds = new Rectangle(0,0,150,80), Palette = new GdiColorPalette(0.0f,0.9f,1f,0.9f), Margin = new Padding(-1,-1,0,0)}); break;
					case "WaveEditorFrame"         : iCanvas.Frame.Children.Add(new WaveEditorFrame          {Name = "WaveEditor",       Bounds = new Rectangle(0,0,640,480), Palette = new GdiColorPalette(0.0f,6.0f,1f,0.9f), Margin = new Padding(0,0,-1,-1)}); break;
					case "EventTestFrame"          : iCanvas.Frame.Children.Add(new EventTestFrame           {Name = "Test",             Bounds = new Rectangle(0,0,400,200), Palette = new GdiColorPalette(0.0f), Margin = new Padding(-1,-1,0,0)}); break;

					case "VehicleHUDFrame"         :
					{
						iCanvas.Frame.Children.Add(new IcosahedronTest          {Name = "IcosahedronTest", Bounds = new Rectangle(0,0,600,500),  Palette = new GdiColorPalette(),  Dock = DockStyle.Fill});
						iCanvas.Frame.Children.Add(new VehicleHUDFrame          {Name = "HUD",              Zoom = 2,                              Palette = new GdiColorPalette(0.5f,5f,1f,0.9f), Margin = new Padding(0,0,0,0)});

						break;
					}

					default :
					{
						break;
					}
				}

				if(_IsClock) iCanvas.Frame.Children.Add(new ClockFrame    {Name = "Clock",            Bounds = new Rectangle(0,0,150,80),    Palette = new GdiColorPalette(0.5f,5f,1f,0.9f), Margin = new Padding(-1,-1,0,-1)});
				if(_IsNoise) iCanvas.Frame.Children.Add(new WNoiseFrame   {Name = "WNoiseTest",       Bounds = new Rectangle(0,0,150,40), Palette = new GdiColorPalette(0.1f,1f,1f,0.9f), Margin = new Padding(-1,81,0,-1)});
				
				
				if(!String.IsNullOrEmpty(_AppMode))
				{
					var _DebugConsole = new BufferConsoleFrame   {Name = "DebugConsole",          Bounds = new Rectangle(0,0,300,200), Palette = new GdiColorPalette(0.5f,1,1,1),  Margin = new Padding(-1,-1,0,0), Opacity = 0.8f, IsPointerIgnored = true};
					{
						 G.Debug.Frame = _DebugConsole;

						 //Workarounds.DebugConsoleFrame = _DebugConsole;
						
					}
					var _MainConsole = new BufferConsoleFrame   {Name = "MainConsole",         Bounds = new Rectangle(0,0,950,200), Palette = new GdiColorPalette(0.5f), Opacity = 0, Dock = DockStyle.Bottom, IsGradientMode = true, IsPointerIgnored = true};
					{
						 G.Console.Frame = _MainConsole;
						 //Workarounds.MainConsoleFrame = _MainConsole;
						
					}
					///iCanvas.Frame.Children.Add(_DebugConsole);
					iCanvas.Frame.Children.Add(_MainConsole);
					
					//iCanvas.Frame.Children.Add(new FPSMeterFrame  {Name = "???",        Bounds = new Rectangle(0,205,64,30), Palette = new GdiColorPalette(), Margin = new Padding(-1,0,0,-1)});
					
					iCanvas.Frame.Children[0].Focus();
					//iCanvas.Frame.Children[1].Focus();
					//iCanvas.Frame.Focus();

					///_MainConsole.Focus();

					iCanvas.MainFrame = iCanvas.Frame.Children[0];
					iCanvas.RecentNonMainFrame = _MainConsole;
				}
			}
			
			

			///AE.DescriptionLanguage.Scripting.Tests.Logic1();
		}

		public void LoadDefaultConfiguration()
		{
			if(System.IO.File.Exists(DefaultConfigurationPath))
			{
				DefaultConfiguration = DataNode.Load(DefaultConfigurationPath);
			}
			else
			{
				DefaultConfiguration = new DataNode("Defaults");
				{
					var _Viewpoint = DefaultConfiguration.Create("Viewpoint");
					{
						_Viewpoint["@position"] = "0,0,3";
					}
				}
			}
		}
		public void LoadConfiguration()
		{
			Configuration = System.IO.File.Exists(DefaultConfigurationPath) ? DataNode.Load(DefaultConfigurationPath) : DefaultConfiguration;

			//InitConfiguration();
		}
		public void SaveConfiguration()
		{
			DataNode.Save(Configuration, DefaultConfigurationPath);
		}

		public void ReadConfiguration()
		{
			if(this.AnyZoomableFrame == null) return;
			//if(Workarounds.SchemeFrame == null) return;

			string[] _OrthoPosStr = ((string)Configuration["Viewpoint/@position"] ?? "0,0,3").Split(',');
			string[] _PerspPosStr = ((string)Configuration["Viewpoint/@perspective"] ?? "-1.5707,-0.5").Split(',');
			bool     _IsPerspMode = (Configuration["Viewpoint/@isPerspectiveMode"] ?? false);
			//var      _VPPos    = new OpenTK.Vector3d();

			var _ViewP = this.AnyZoomableFrame.Viewpoint;
			{
				_ViewP.Update
				(
					new OpenTK.Vector3d
					(
						Double.Parse(_OrthoPosStr[0]),
						Double.Parse(_OrthoPosStr[1]),
						Double.Parse(_OrthoPosStr[2])
					),
					0.0
				);

				
				_ViewP.CurrentState.PerspEyeAzimuth = Double.Parse(_PerspPosStr[0]);
				_ViewP.CurrentState.PerspEyeZenith  = Double.Parse(_PerspPosStr[1]);
					
				_ViewP.CurrentState.UpdatePerspEyePoint();

				this.AnyZoomableFrame.IsPerspectiveMode = _IsPerspMode;
			}
		}
		public void WriteConfiguration() 
		{
			//return;///!!!
			
			if(this.AnyZoomableFrame == null) return;

			var _ViewP = this.AnyZoomableFrame.Viewpoint.CurrentState;
			//var _ViewP = _View;
			var _OrthoPosStr = _ViewP.Position.X.ToString() + "," + _ViewP.Position.Y.ToString() + "," + _ViewP.Position.Z.ToString();
			var _PerspPosStr = _ViewP.PerspEyeAzimuth.ToString() + "," + _ViewP.PerspEyeZenith.ToString();

			Configuration["Viewpoint/@position"]   = _OrthoPosStr;
			Configuration["Viewpoint/@perspective"] = _PerspPosStr;
			Configuration["Viewpoint/@isPerspectiveMode"] = this.AnyZoomableFrame.IsPerspectiveMode;
		}
	}
}
