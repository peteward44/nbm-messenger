using System;
using WinForms = System.Windows.Forms;

// 1/6/03

namespace NBM
{
	/// <summary>
	/// Simply holds the Main() method for the application.
	/// </summary>
	public class EntryPoint
	{
		private EntryPoint()
		{
		}


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main() 
		{
			IStorage mainStorage = null;

			try
			{
				// create userdata directory if it does not exist
				System.IO.Directory.CreateDirectory(Config.Constants.UserDataPath);

				mainStorage = new XmlStorage(Config.Constants.UserDataPath + System.IO.Path.DirectorySeparatorChar + "settings.xml");

				// load all our global settings
				NBM.Plugin.Settings.GlobalSettings.Instance(mainStorage);
				NBM.Plugin.Settings.GlobalSettings.Instance().Load();

				// run main form
				WinForms.Application.Run(new MainForm(mainStorage));
			}
			catch (Exception e)
			{
				BugReportForm form = new BugReportForm(e);
				form.ShowDialog();
			}
			catch
			{
				BugReportForm form = new BugReportForm();
				form.ShowDialog();
			}
			finally
			{
				if (mainStorage != null)
				{
					NBM.Plugin.Settings.GlobalSettings.Instance().Save();
					mainStorage.Close();
				}
			}

			return 0;
		}
	}
}
