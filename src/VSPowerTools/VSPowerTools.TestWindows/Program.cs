using System;
using System.Threading;
using System.Windows;
using AMLib.Wpf.Controls;
using VSPowerTools.ToolWindows.LanguageMassEditor;
using LanguageMassEditor = VSPowerTools.ToolWindows.LanguageMassEditor.UserControls.LanguageMassEditor;

namespace VSPowerTools.TestWindows
{
	class Program
	{
		private static bool _stop = false;

		static void Main(string[] args)
		{
			Thread windowThread = new Thread(new ThreadStart(() =>
			{
				Application app = new Application();
				app.ShutdownMode = ShutdownMode.OnMainWindowClose;
//				var window = new Window();
				var window = new ThemeWindow();
				var control = new LanguageMassEditor();
				window.Title = "what the fuck?";
				window.Content = control;
				window.Closed += WindowClosed;
				control.SolutionPath = @"D:\Dateien\Software\Eigene Programme\C#\ResourceFileSolution\ResourceFileSolution.sln";
				//                control.SolutionPath = @"D:\SVN\Checkout\Entertainment Assistant 4\EntertainmentAssistant 4.0.sln";
				//                control.SolutionPath = @"D:\Dateien\Software\Eigene Programme\C#\ResharperPluginDummy\ResharperPluginDummy\ResharperPluginDummy.sln";

				
				app.Run(window);
			}));

			windowThread.TrySetApartmentState(ApartmentState.STA);
			windowThread.Start();

			
			while (!_stop)
			{
				Thread.Sleep(10000);
				Console.Out.WriteLine("still running. {0}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
			}
		}

		static void WindowClosed(object sender, EventArgs e)
		{
			_stop = true;
		}
	}
}
