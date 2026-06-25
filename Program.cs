using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleInstaller
{

	public enum InstallMode
	{
		Install,
		Uninstall,
		Update,
	}


	static class Program
	{
		public static InstallManifest				m_info		= new InstallManifest();
		public static InstallMode					m_BootMode	= InstallMode.Install;
		public static string						REG_KEY_BASE = "";
		public static Microsoft.Win32.RegistryKey?	m_REG_KEY = null;


		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);


#if DEBUG
#else
			Directory.SetCurrentDirectory( Path.GetDirectoryName( Application.ExecutablePath ));
#endif


			var exeDir = Directory.GetCurrentDirectory();

			var installFilePath			= Path.Combine( exeDir, "install.ins" );
			var uninstallFilePath		= Path.Combine( exeDir, "uninstall.ins" );
			var updateFilePath			= Path.Combine( exeDir, "update.ins" );

			if ( File.Exists(uninstallFilePath))		m_BootMode = InstallMode.Uninstall;
			if ( File.Exists(installFilePath))			m_BootMode = InstallMode.Install;
			if ( File.Exists(updateFilePath))			m_BootMode = InstallMode.Update;

			if ( m_BootMode == InstallMode.Uninstall)	m_info.LoadManifest( uninstallFilePath );
			if ( m_BootMode == InstallMode.Install)		m_info.LoadManifest( installFilePath );
			if ( m_BootMode == InstallMode.Update )		m_info.LoadManifest( updateFilePath );

			REG_KEY_BASE	= $"SOFTWARE\\{m_info.MakerName}\\{m_info.GameTitle}";
			m_REG_KEY		= Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Program.REG_KEY_BASE);

			if (m_BootMode == InstallMode.Uninstall)
			{
				Application.Run(new UninstallForm());
			}
			else if (m_BootMode == InstallMode.Install)
			{
				Application.Run(new MainForm());
			}
			else if (m_BootMode == InstallMode.Update)
			{
				Application.Run(new updater());
			}
		}


		public static string ReplaceSpecialFolders(string path)
		{
			path = path.Replace( "%localappdata%",				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
			path = path.Replace( "%appdata%",					Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			path = path.Replace( "%programfiles%",				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			path = path.Replace( "%programfiles(x86)%",			Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
			path = path.Replace( "%userprofile%",				Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
			path = path.Replace( "%desktop%", 					Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
			path = path.Replace( "%mydocuments%",				Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

			//アプリ固有のパス置換
			path = path.Replace( "%makername%",					m_info.MakerName);
			path = path.Replace( "%gametitle%",					m_info.GameTitle);
			path = path.Replace( "%version%",					m_info.Version);

			return path;
		}

		public static void OffFileInfoReadOnly(string path)
		{
			try
			{
				if (File.Exists(path))
				{
					FileInfo file = new FileInfo(path);
					if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
					{
						file.Attributes = FileAttributes.Normal;
					}
				}
			}
			catch { }
		}




		public static Icon? GetAppWin32Icon()
		{
			// 単一ファイル（SingleFile）発行に対応するため、
			// アセンブリからではなく、現在起動しているプロセスのモジュールハンドルを直接取得します
			IntPtr hModule = GetModuleHandleW(null);

			// Win32リソースのメインアイコン（ID: 32512 = IDI_APPLICATION）からハンドルを抽出
			IntPtr hIcon = LoadIconW(hModule, (IntPtr)32512);

			// アイコンハンドルからIconオブジェクトを生成して返す
			return hIcon != IntPtr.Zero ? Icon.FromHandle(hIcon) : null;
		}

		// Windows API の宣言を更新
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern IntPtr GetModuleHandleW(string? lpModuleName);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr LoadIconW(IntPtr hInstance, IntPtr lpIconName);
	}
}
