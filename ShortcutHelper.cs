using System;
using System.IO;
using static System.Windows.Forms.Design.AxImporter;

namespace SimpleInstaller
{
	public static class ShortcutHelper
	{
		public static void CreateDesktopShortcut(string name, string targetPath)
		{
			try
			{
				var desk					= Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				var lnk						= Path.Combine(desk, name + ".lnk");
				Type t						= Type.GetTypeFromProgID("WScript.Shell");
				dynamic shell				= Activator.CreateInstance(t);
				var shortcut				= shell.CreateShortcut(lnk);
				shortcut.TargetPath			= targetPath;
				shortcut.WorkingDirectory	= Path.GetDirectoryName(targetPath);
				shortcut.Save();
			}
			catch { }
		}


		public static void CreateStartMenuShortcut(string DirName, string name,string targetPath)
		{
			try
			{
				var desk		= Environment.GetFolderPath(Environment.SpecialFolder.Programs);
				var baseDirPath	= Path.Combine(desk, DirName);
				if (!System.IO.Directory.Exists(baseDirPath)) Directory.CreateDirectory(baseDirPath);

				var lnk						= Path.Combine(baseDirPath, name + ".lnk");
				Type t						= Type.GetTypeFromProgID("WScript.Shell");
				dynamic shell				= Activator.CreateInstance(t);
				var shortcut				= shell.CreateShortcut(lnk);
				shortcut.TargetPath			= targetPath;
				shortcut.WorkingDirectory	= Path.GetDirectoryName(targetPath);
				shortcut.Save();
			}
			catch { }
		}
	}
}
