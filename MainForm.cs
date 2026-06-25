using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace SimpleInstaller
{
	public partial class MainForm : Form
	{
		private bool						m_isNowInstall	= false;
		private InstallerLogic				installer		= new InstallerLogic();
		private CancellationTokenSource?	cancellationTokenSource;

		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		public MainForm()
		{
			Program.m_BootMode = InstallMode.Install;

			InitializeComponent();

			this.Text = $"{Program.m_info.GameTitle} - {Program.m_info.Version} " + strings.titleInstall;

			//特殊フォルダパス置換
			string installPath = Program.ReplaceSpecialFolders(Program.m_info.InstallPath);

			try
			{
				var banner = Path.Combine(Directory.GetCurrentDirectory(), "install.png");
				if (File.Exists(banner)) pictureBoxBanner.Load(banner);
			}
			catch { }

			txtInstallPath.Text			= installPath;
			chkDesktopShortcut.Checked	= true;                              // デフォルトでショートカットを作成するようにする
			chkRunAfter.Checked			= true;                             // デフォルトでインストール後に実行するようにする
		}


		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		private async void btnInstall_Click(object sender, EventArgs e)
		{
			if (m_isNowInstall == false)
			{
				var driveLetter = Path.GetPathRoot(txtInstallPath.Text);
				var driveInfor	= new DriveInfo(driveLetter);

				await DoInstall();
			}
			else
			{
				DoCancel();
			}
		}

		private async Task DoInstall()
		{
			EnableUI(false);

			m_isNowInstall = true;

			progressBar.Value = 0;

			var options = new InstallerOptions
			{
				InstallPath				= txtInstallPath.Text,
				CreateDesktopShortcut	= chkDesktopShortcut.Checked,
				RunAfterInstall			= chkRunAfter.Checked
			};

			installer.LogMessage		+= (s) => BeginInvoke(() => lblStatus.Text = s);
			installer.ProgressChanged	+= (p) => BeginInvoke(() => progressBar.Value = Math.Min(100, Math.Max(0, p)));

			// キャンセルトークンの作成
			cancellationTokenSource = new CancellationTokenSource();

			try
			{
				var result				= await Task.Run(() => installer.Install(options, cancellationTokenSource.Token));
				var uninstallExePath	= Path.Combine(options.InstallPath, "uninstall.exe");

				RegisterUninstaller(Program.m_info.GameTitle, uninstallExePath, Program.m_info.GameTitle, Program.m_info.Version, Program.m_info.MakerName, options.InstallPath);

				MessageBox.Show(this, result ? strings.MsgInstallComp : strings.MsgInstallCancel, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
				progressBar.Value = 0;

				if (this.chkRunAfter.Checked)
				{
					var exe = Path.Combine(options.InstallPath, Program.m_info.MainExeName);
					if (exe != null)
					{
						try { Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true }); }
						catch { }
					}
				}
			}
			catch (OperationCanceledException)
			{
				MessageBox.Show(this, strings.MsgInstallCancel, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
				progressBar.Value = 0;
			}
			finally
			{
				cancellationTokenSource?.Dispose();
				cancellationTokenSource	= null;
				m_isNowInstall			= false;
				EnableUI(true);
			}
		}

		private void EnableUI(bool isEnable)
		{
			//	btnInstall.Enabled	= isEnable;
			txtInstallPath.Enabled	= isEnable;
			btnBrowse.Enabled		= isEnable;
			btnInstall.Text			= isEnable ? strings.installBtnText : strings.Cancel;
		}

		//-----------------------------------------------------------
		// キャンセルボタンのイベントハンドラー
		//-----------------------------------------------------------
		private void DoCancel()
		{
			if (cancellationTokenSource != null && !cancellationTokenSource.Token.IsCancellationRequested)
			{
				cancellationTokenSource.Cancel();
				lblStatus.Text = strings.Cancel;
			}
		}

		//-----------------------------------------------------------
		//インストール先変更
		//-----------------------------------------------------------
		private void btnBrowse_Click(object sender, EventArgs e)
		{
			using var dlg = new FolderBrowserDialog();
			dlg.SelectedPath = txtInstallPath.Text;
			if (dlg.ShowDialog(this) == DialogResult.OK) txtInstallPath.Text = dlg.SelectedPath;

			string baseDir = Program.ReplaceSpecialFolders(Program.m_info.MakerName + "\\" + Program.m_info.GameTitle);
			if (!string.IsNullOrEmpty(baseDir) && !txtInstallPath.Text.EndsWith(baseDir, StringComparison.OrdinalIgnoreCase))
			{
				txtInstallPath.Text = Path.Combine(txtInstallPath.Text, baseDir);
			}
		}


		//-----------------------------------------------------------
		//windwosのアンインストーラーに登録する
		//-----------------------------------------------------------
		public void RegisterUninstaller(string keyName, string exePath, string displayName, string version, string publisher, string installFolder)
		{
			var baseKey = Registry.LocalMachine;
			using var key = baseKey.CreateSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{keyName}");

			if (key == null) return;

			key.SetValue("DisplayName",		displayName);
			key.SetValue("DisplayVersion",	version);
			key.SetValue("Publisher",		publisher);
			key.SetValue("InstallLocation", installFolder);
			key.SetValue("UninstallString", exePath);
			key.SetValue("DisplayIcon",		exePath);
			//	key.SetValue( "EstimatedSize", sizeKb, RegistryValueKind.DWord); 		//一覧に表示する容量(kb換算)とのこと
		}

		private void ResXManager(object sender, EventArgs e)
		{

		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			this.Icon = Program.GetAppWin32Icon();

			CultureUI();
		}

		private void CultureUI()
		{
			var culture = System.Globalization.CultureInfo.CurrentUICulture;
			var name	= System.Globalization.CultureInfo.CurrentCulture.Name;

			if (name != "ja-JP") name = "en-US";

			Thread.CurrentThread.CurrentUICulture = new CultureInfo(name);

			btnInstall.Text			= strings.installBtnText;
			btnBrowse.Text			= strings.dirBtn;
			chkDesktopShortcut.Text	= strings.createDesktopShortcut;
			chkRunAfter.Text		= strings.startup;
			lblTitle.Text			= strings.installPath;
			lblStatus.Text			= strings.updatePath;

		}

	}

}
