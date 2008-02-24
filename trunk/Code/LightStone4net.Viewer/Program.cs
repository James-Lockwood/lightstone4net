using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LightStone4net.Core;
using LightStone4net.WinUI;

namespace LightStone4net.Viewer
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			LightStoneUI.Initialize();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}