using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browser
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// FormBrowserHostから起動された時は引数に通信用URLが渡される
			if (args.Length == 0)
			{
				MessageBox.Show("Please start the application using ElectronicObserver.exe",
					"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			Application.Run(new FormBrowser(args[0]));
		}

		private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			if (args.Name.StartsWith("CefSharp"))
			{
				string asmname = args.Name.Split(",".ToCharArray(), 2)[0] + ".dll";
				string arch = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Environment.Is64BitProcess ? "x64" : "x86", asmname);

				if (!System.IO.File.Exists(arch))
					return null;

				try
				{
					return System.Reflection.Assembly.LoadFile(arch);
				}
				catch (IOException ex) when (ex is FileNotFoundException || ex is FileLoadException)
				{
					if (MessageBox.Show(
$@"The browser component could not be loaded.
Microsoft Visual C++ 2015 Redistributable is required.
Open the download page?
(Please install vc_redist.{(Environment.Is64BitProcess ? "x64" : "x86")}.exe)",
						"CefSharp Load Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
						== DialogResult.Yes)
					{
						System.Diagnostics.Process.Start(@"https://www.microsoft.com/en-US/download/details.aspx?id=53587");
					}

					// なんにせよ今回は起動できないのであきらめる
					throw;
				}
				catch (NotSupportedException)
				{
					// 概ね ZoneID を外し忘れているのが原因

					if (MessageBox.Show(
@"Browser startup failed.
This may be caused by the fact that the operation required for installation has not been performed.
Do you want to open the installation guide?",
							"Browser Load Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Error)
						== DialogResult.Yes)
						System.Diagnostics.Process.Start(@"https://github.com/andanteyk/ElectronicObserver/wiki/Install");

					// なんにせよ今回は起動できないのであきらめる
					throw;
				}
			}
			return null;
		}
	}
}
