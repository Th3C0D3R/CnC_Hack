using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CnC_Hack
{
	static class Program
	{

		private static Mutex mutex = null;

		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			const string appName = "overlay";
			bool createdNew;
			mutex = new Mutex(true, appName, out createdNew);
			if (!createdNew)
				return;

			if (args.Length != 2)
				return;
			if (args[0].GetType() != typeof(string))
				return;
			if (!(int.TryParse(args[1],out int baseaddr)))
				return;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FormOverlay(args));
		}
	}
}
