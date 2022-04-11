using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace AE
{
	class Start
	{
		public static void Main()
		{
			//AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);


			var _DirPath = Path.GetFullPath(@"..\..\..\AE.Studio\bin\Debug");

			System.Environment.CurrentDirectory = _DirPath;
			Directory.SetCurrentDirectory(_DirPath);

			var _AppDomainSetup = new AppDomainSetup();
			{
			    _AppDomainSetup.ApplicationName = "";
			    _AppDomainSetup.DynamicBase = System.Environment.CurrentDirectory;
			}
			var _MyDomain = AppDomain.CreateDomain("MyDomain");//, null, _AppDomainSetup);
			{
				_MyDomain.AssemblyResolve += new ResolveEventHandler(_MyDomain_AssemblyResolve);
				//_MyDomain.Exe
			}
			
			

			//_MyDomain.SetDynamicBase(
				///.DynamicDirectory = Directory.GetCurrentDirectory();
			var _Asm = _MyDomain.Load("AE.Studio.exe");
			//var _Asm = Assembly.LoadFrom(Path.GetFullPath("AE.Studio.exe"));

			///var _Asm = Assembly.LoadFile(System.IO.Path.GetFullPath(@"AE.Studio.exe"));
			//_Asm.LoadModule("",

			_Asm.EntryPoint.Invoke(null, new object[1]);
		}

		static Assembly _MyDomain_AssemblyResolve(object iSender, ResolveEventArgs iEvent)
		{
			
			var _Bytes = File.ReadAllBytes(iEvent.Name);
			
			var oAsm = AppDomain.CurrentDomain.Load(_Bytes);

			var _MM = oAsm.GetModules();
			//iEvent.Name;
			
			return oAsm;//(iSender as AppDomain).LoadA;
			//throw new NotImplementedException();
		}

		//static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		//{
		//    throw new NotImplementedException();
		//}
	}
}
