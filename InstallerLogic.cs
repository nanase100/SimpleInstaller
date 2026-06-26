using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SimpleInstaller
{
	public class InstallerOptions
	{
		public string InstallPath			{ get; set; } = "";					//インストール
		public bool CreateDesktopShortcut	{ get; set; }						//デスクトップにショートカットを作成するかどうか
		public bool RunAfterInstall			{ get; set; }						//インストール後、自動でゲームを起動するかどうか
	}

	public class InstallerLogic
	{
		public event Action<string>?	LogMessage;
		public event Action<int>?		ProgressChanged;

		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		public bool Install(InstallerOptions options, CancellationToken cancellationToken = default)
		{
			Log( strings.LogInsStart );

			var manifest = new InstallManifest
			{
				MakerName	= Program.m_info.MakerName,
				GameTitle	= Program.m_info.GameTitle,
				Version		= Program.m_info.Version,
				MainExeName	= Program.m_info.MainExeName,
				InstallPath	= options.InstallPath,
				SaveFolder	= Program.m_info.SaveFolder,

				FileList	= new List<string>(),
			};

			try
			{
				Directory.CreateDirectory(options.InstallPath);

				var payloadRoot		= Path.Combine(Directory.GetCurrentDirectory(), "data");
				var sourceRoot		= Directory.GetCurrentDirectory();

				var files		= (Program.m_info.FileList.Count() == 0 ? GatherFilesForInstall(payloadRoot, sourceRoot) : GatherFilesForInstall(Program.m_info.FileList));
				int total		= Math.Max(1, files.Count);
				int doneCount	= 0;


				List<bool> alreadyInsFlgList = new List<bool>();
				for( int j = 0; j < files.Count; j++) alreadyInsFlgList.Add(false);

				//foreach (var f in files)
				//for (int i = 0; i < files.Count; i++)
				int i = 0;
				while(true)
				{
					if (alreadyInsFlgList[i])
					{
						i++;
						continue;
					}

					var f = files[i];

					// キャンセルチェック
					cancellationToken.ThrowIfCancellationRequested();

					string relative	= f.RelativePath;
					string target	= Path.Combine(options.InstallPath, relative);

					Directory.CreateDirectory(Path.GetDirectoryName(target) ?? options.InstallPath);

					string sourceFile = f.SourcePath;
					if (File.Exists(sourceFile))
					{				

						CopyFileWithCancellation(sourceFile, target, cancellationToken);

						Program.OffFileInfoReadOnly(target);

						manifest.FileList.Add(target);

						doneCount++;
						ProgressChanged?.Invoke((int)((doneCount / (double)total) * 100));
						Log($"{relative} " + strings.LogInsCopy + $" ({doneCount}/{total})");

						alreadyInsFlgList[i] = true;
					}
					else
					{
						//Log($"警告: {relative} が見つかりませんでした。");
					}

					//インストール完了によるループ脱出判定
					var breakFlg = true;
					for( int j = 0; j < alreadyInsFlgList.Count; j++)
					{
						if (!alreadyInsFlgList[j])
						{
							breakFlg = false;
							break;
						}
					}
					
					if(breakFlg ) break;


					//ファイルリストの末尾までいってまだインストール対象が残っていれば、ループの先頭に戻る前に、ディスク交換の確認をする
					if ( i == files.Count -1)
					{
						
						//まだインストールできていない先頭のファイル番号を調べる
						int itemNo = 0;
						while(true)	
						{ 
							if(alreadyInsFlgList[itemNo] == false ) break;
							itemNo++;
							if (itemNo >= alreadyInsFlgList.Count) throw new FileNotFoundException( strings.fileListIncorrect ) ;
						}

						sourceFile = files[itemNo].SourcePath;
						string discName = Program.m_info.DiscNameList[itemNo];
						while (true)
						{
							var result = MessageBox.Show( strings.msgDiscChangeFront + $" {discName} " + strings.msgDiscChangeRear, "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
							if (result == DialogResult.OK)
							{
								if (File.Exists(sourceFile)) break;
							}
							else if (result == DialogResult.Cancel)
							{
								throw new FileNotFoundException( strings.MsgInstallCancel );
							}
						}
					}
					//if (i >= files.Count) break;


					i++;
					if (i >= files.Count)
					{
						i = 0;
					}
				}

				// キャンセルチェック
				//cancellationToken.ThrowIfCancellationRequested();

				if (options.CreateDesktopShortcut)
				{
					//var exe			= manifest.InstalledFiles.FirstOrDefault(p => p.EndsWith(".exe", StringComparison.OrdinalIgnoreCase));
					var exeName			= manifest.MainExeName;
					var shortcutName	= Path.GetFileNameWithoutExtension(exeName);
					var shortcutpath	= Path.Combine(options.InstallPath, exeName);
					if (exeName != null) ShortcutHelper.CreateDesktopShortcut(shortcutName, shortcutpath);
				}

				//スタートメニューにショートカットを作成する
				{
					var shortcutpath	= Path.Combine(options.InstallPath, manifest.MainExeName);
					var name			= Program.m_info.GameTitle;
					ShortcutHelper.CreateStartMenuShortcut(name, manifest.GameTitle, shortcutpath);

					var shortcutName	= "uninstall.exe";
					shortcutpath		= Path.Combine(options.InstallPath, shortcutName);
					ShortcutHelper.CreateStartMenuShortcut(name, manifest.GameTitle + strings.startMenuUninstall, shortcutpath);
				}

				// キャンセルチェック
				cancellationToken.ThrowIfCancellationRequested();

				var manifestPath	= Path.Combine(options.InstallPath, "uninstall.ins");
				

				try
				{
					using var key = Registry.CurrentUser.CreateSubKey(Program.REG_KEY_BASE);
					key.SetValue("InstallPath", options.InstallPath);
					key.SetValue("Version", manifest.Version);
					key.SetValue("CreateDesktopShortcut", options.CreateDesktopShortcut ? 1 : 0, Microsoft.Win32.RegistryValueKind.DWord);
					}
					catch { }

					//インストーラーをインストール先にコピーしておく（アンインストーラーとして利用するため）

					string uninstallerPath = Path.Combine(options.InstallPath, "uninstall.exe");
					CopyFileWithCancellation(Application.ExecutablePath, uninstallerPath, cancellationToken);
					//manifest.FileList.Add(uninstallerPath);
					Program.OffFileInfoReadOnly(uninstallerPath);

					string uninstPicPath = Path.Combine(options.InstallPath, "uninstall.png");
					CopyFileWithCancellation("uninstall.png", uninstPicPath, cancellationToken);
					manifest.FileList.Add(uninstPicPath);
					Program.OffFileInfoReadOnly(uninstPicPath);

					manifest.SaveManifest(manifestPath);

				Log( strings.LogInsComplete );

					return true;
				}
				catch (OperationCanceledException)
			{

				//コピー済みのファイルを削除

				manifest.FileList.ForEach(f =>
				{
					try
					{
						if (File.Exists(f)) File.Delete(f);
					}
					catch { }
				});

				Log( strings.LogInsCancel );
				return false;
			}
			catch (Exception ex)
			{
				Log("error: " + ex.Message);
				return false;
			}
		}


		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		void Log(string s) => LogMessage?.Invoke(s);



		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		List<(string RelativePath, string SourcePath)> GatherFilesForInstall(string payloadRoot, string fallbackRoot)
		{
			var list = new List<(string, string)>();
			if (Directory.Exists(payloadRoot))
			{
				foreach (var f in Directory.GetFiles(payloadRoot, "*", SearchOption.AllDirectories))
				{
					var rel = Path.GetRelativePath(payloadRoot, f);
					list.Add((rel, f));
				}
			}
			else
			{
				foreach (var f in Directory.GetFiles(fallbackRoot, "*", SearchOption.TopDirectoryOnly))
				{
					var name = Path.GetFileName(f);
					if (string.Equals(name, Path.GetFileName(Application.ExecutablePath), StringComparison.OrdinalIgnoreCase)) continue;
					list.Add((name, f));
				}
			}
			return list;
		}

		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		List<(string RelativePath, string SourcePath)> GatherFilesForInstall(List<string> fileList)
		{
			var list = new List<(string, string)>();
			var payloadRoot = Path.Combine(Directory.GetCurrentDirectory(), "data");

			foreach (var f in fileList)
			{
				var path = Path.Combine(payloadRoot, f);
				list.Add((f, path));
			}

			return list;
		}

		//-----------------------------------------------------------
		// ファイルをバッファでコピー（キャンセルトークン対応）
		//-----------------------------------------------------------
		/// <summary>
		/// テスト用: CopyFileWithCancellation を public 経由で呼び出し可能にします
		/// </summary>
		public void PublicCopyFileWithCancellation(string sourceFile, string targetFile, CancellationToken cancellationToken)
		{
			CopyFileWithCancellation(sourceFile, targetFile, cancellationToken);
		}

		private void CopyFileWithCancellation(string sourceFile, string targetFile, CancellationToken cancellationToken)
		{
			// ファイルサイズに応じたバッファサイズの自動選択
			var fileInfo = new FileInfo(sourceFile);
			int bufferSize = DetermineOptimalBufferSize(fileInfo.Length);

			try
			{
				using (var sourceStream = new FileStream(
					sourceFile, 
					FileMode.Open, 
					FileAccess.Read, 
					FileShare.Read, 
					bufferSize,
					FileOptions.SequentialScan))
				using (var targetStream = new FileStream(
					targetFile, 
					FileMode.Create, 
					FileAccess.Write, 
					FileShare.None, 
					bufferSize,
					FileOptions.WriteThrough))
				{
					byte[] buffer = new byte[bufferSize];
					int bytesRead;
					long cancellationCheckInterval = Math.Max(10 * 1024 * 1024, bufferSize * 10); // 10MB または バッファサイズの10倍
					long bytesProcessed = 0;

					while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
					{
						targetStream.Write(buffer, 0, bytesRead);
						bytesProcessed += bytesRead;

						// 定期的にキャンセルをチェック（オーバーヘッド削減）
						if (bytesProcessed >= cancellationCheckInterval)
						{
							cancellationToken.ThrowIfCancellationRequested();
							bytesProcessed = 0;
						}
					}

					// 最終的なキャンセルチェック
					cancellationToken.ThrowIfCancellationRequested();
				}
			}
			catch (OperationCanceledException)
			{
				// キャンセル時は不完全なファイルを削除
				try
				{
					if (File.Exists(targetFile))
					{
						File.Delete(targetFile);
					}
				}
				catch { }

				throw;
			}
		}

		/// <summary>
		/// ファイルサイズに応じた最適なバッファサイズを決定します
		/// </summary>
		private int DetermineOptimalBufferSize(long fileSize)
		{
			// 小ファイル（<10MB）: 256KB
			if (fileSize < 10 * 1024 * 1024)
				return 256 * 1024;

			// 中ファイル（10MB～100MB）: 1MB（デフォルト）
			if (fileSize < 100 * 1024 * 1024)
				return 1024 * 1024;

			// 大ファイル（>100MB）: 4MB
			return 4 * 1024 * 1024;
		}
	}
}
