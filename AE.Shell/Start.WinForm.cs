using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace AE
{
	class Start
	{
		[STAThread]
		static void Main()
		{
			var _MyForm = new Form2();

			System.Windows.Forms.Application.Run(_MyForm);
		}
	}
}
