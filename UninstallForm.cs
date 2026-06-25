using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;



namespace SimpleInstaller
{
	public partial class UninstallForm : Form
	{

		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		public UninstallForm()
		{
			InitializeComponent();

			StartPosition	= FormStartPosition.CenterScreen;
			this.Text		= $"{Program.m_info.GameTitle} - {Program.m_info.Version} " + strings.titleUninstall;
		}


		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		private void Btn_Click(object? sender, EventArgs e)
		{
			if (MessageBox.Show(this, strings.MsgUninstallConfirm, "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) return;

			bool flowControl = DoUninstall();
			if (!flowControl)
			{
				return;
			}
		}

		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		private bool DoUninstall()
		{
			try
			{
				string? path		= Program.m_REG_KEY?.GetValue("InstallPath") as string;
				var manifestPath	= Path.Combine(path, "uninstall.ins");

				if (!File.Exists(manifestPath))
				{
					MessageBox.Show(this, strings.msgUnfoundUninstallInfo, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}

				InstallManifest manifest = new InstallManifest();
				manifest.LoadManifest(manifestPath);

				if (manifest != null)
				{
					foreach (var f in manifest.FileList)
					{
						try
						{
							if (File.Exists(f))
							{
								FileInfo file = new FileInfo(f);
								if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
								{
									file.Attributes = FileAttributes.Normal;
								}
								File.Delete(f);
							}
						}
						catch { }
					}

					try
					{
						if (!string.IsNullOrEmpty(manifest.SaveFolder))
						{
							var savePath = manifest.SaveFolder;

							var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
							if (Directory.Exists(savePath) && Path.GetFullPath(savePath).StartsWith(Path.GetFullPath(userProfile), StringComparison.OrdinalIgnoreCase))
							{
								try { Directory.Delete(savePath, true); } catch { }
							}
						}
					}
					catch { }
				}

				//バナーとインストール情報を消す
				try { File.Delete(manifestPath); } catch { }
				try { File.Delete(Path.Combine(path, "uninstall.png")); } catch { }

				//インストールフォルダを削除する
				//todo:アンインストーラーをtmpフォルダにコピーしてバトンタッチする形式に変更してから
				//try { if(Directory.GetFiles(path).Length == 0 ) Directory.Delete(path, true); } catch { }

				//スタートメニューを削除する
				DeleteStartMenuShortcut();

				//レジストリを削除する
				try { Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(Program.REG_KEY_BASE); } catch { }

				//windowsのアンインストール情報レジストリを削除する
				try { Microsoft.Win32.Registry.LocalMachine.DeleteSubKeyTree($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{Program.m_info.GameTitle}"); } catch { }

				MessageBox.Show(this, strings.MsgUninstallComp, "", MessageBoxButtons.OK, MessageBoxIcon.Information);

				//自分自分を削除して終了するバッチファイルを作成して実行する
				SelfDeleteAndExit();
				//Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "error:" + ex.Message);
			}

			return true;
		}


		//-----------------------------------------------------------
		//自分を削除するバッチファイルを作成して実行する
		//-----------------------------------------------------------
		public static void SelfDeleteAndExit()
		{
			string currentExe = Environment.ProcessPath; // 自身のEXEパス
			string currentDir = AppDomain.CurrentDomain.BaseDirectory; // 自身の配置フォルダ

			// cmd.exe 引数の組み立て choiceで3秒待機した後、EXEを削除、さらにフォルダの削除を試みて終了
			string arguments = $"/c choice /t 3 /d y /n > nul & del /f /q \"{currentExe}\" & rd \"{currentDir}\"";
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName			= "cmd.exe",
				Arguments			= arguments,
				CreateNoWindow		= true,		// 画面を非表示にする
				UseShellExecute		= false,
				WindowStyle			= ProcessWindowStyle.Hidden
			};

			Process.Start(startInfo);

			// 自身のプロセスを即座に終了させ、ファイルのロックを解除する
			Environment.Exit(0);
		}

		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		private void UninstallForm_Load(object sender, EventArgs e)
		{

			this.Icon = Program.GetAppWin32Icon();

			string? path = Program.m_REG_KEY?.GetValue("InstallPath") as string;

			textBox1.Text = path;
			var banner = Path.Combine(Directory.GetCurrentDirectory(), "uninstall.png");
			if (File.Exists(banner)) pictureBoxBanner.Load(banner);

			if (Program.m_info.SaveFolder == "" || Directory.Exists(Program.m_info.SaveFolder) == false)
			{
				checkBox1.Enabled = false;
				checkBox1.Checked = false;
			}

			CultureUI();
		}


		private void CultureUI()
		{
			var culture = System.Globalization.CultureInfo.CurrentUICulture;
			var name = System.Globalization.CultureInfo.CurrentCulture.Name;

			if (name != "ja-JP") name = "en-US";

			Thread.CurrentThread.CurrentUICulture = new CultureInfo(name);

			button1.Text	= strings.uninstallBtn;
			label2.Text		= strings.uninstallPath;
			checkBox1.Text	= strings.deleteSave;

		}


		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		public static void DeleteStartMenuShortcut()
		{
			// 1. スタートメニューの「プログラム」フォルダのパスを取得
			// ※全ユーザー対象の場合は SpecialFolder.CommonPrograms を使用
			string startMenuProgramsProvider = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

			// 2. あなたのアプリ専用のフォルダ名（インストーラー作成時と同じ名前）
			string appFolderName = Program.m_info.GameTitle;
			string targetFolderPath = Path.Combine(startMenuProgramsProvider, appFolderName);

			try
			{
				// フォルダが存在するか確認
				if (Directory.Exists(targetFolderPath))
				{
					// 3. フォルダ内のすべてのファイル（.lnkなど）を削除
					string[] files = Directory.GetFiles(targetFolderPath);
					foreach (string file in files)
					{
						File.Delete(file);
					}

					// 5. 空になった親フォルダを削除
					Directory.Delete(targetFolderPath);

					//Console.WriteLine("スタートメニューのショートカットとフォルダを削除しました。");
				}
			}

			catch (Exception ex)
			{
				// 権限エラーや、他プロセスがファイルを開いている場合のハンドリング
				Console.WriteLine($"削除中にエラーが発生しました: {ex.Message}");
			}
		}
	}
}
