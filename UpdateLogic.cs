






















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

	public class UpdaterLogic
	{
		public event Action<string>?	LogMessage;
		public event Action<int>?		ProgressChanged;

		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		public bool Update()
		{
			Log( strings.LogUpdateStart );

			string? path	= Program.m_REG_KEY?.GetValue("InstallPath") as string;

			var manifest = new InstallManifest
			{
				MakerName	= Program.m_info.MakerName,
				GameTitle	= Program.m_info.GameTitle,
				Version		= Program.m_info.Version,
				MainExeName	= Program.m_info.MainExeName,
				SaveFolder	= Program.m_info.SaveFolder,
				FileList	= new List<string>(),
				InstallPath = path,
			};

			//ファイルリストはインストール先のものベースにアップデート分を追加する形で。
			var targetManifestPath	= Path.Combine(manifest.InstallPath, "uninstall.ins");
			var targetManifest		= new InstallManifest();
			targetManifest.LoadManifest(targetManifestPath);
			
			foreach ( var f in targetManifest.FileList ) manifest.FileList.Add(f);

			try
			{
				var payloadRoot		= Path.Combine(Directory.GetCurrentDirectory(), "data");
				var sourceRoot		= Directory.GetCurrentDirectory();

				var files			= (Program.m_info.FileList.Count() == 0 ? GatherFilesForInstall(payloadRoot, sourceRoot) : GatherFilesForInstall(Program.m_info.FileList));
				int total			= Math.Max(1, files.Count);
				int doneCount		= 0;

				for( int i = 0; i < files.Count; i++ )
				{
					var f				= files[i];
					string relative		= f.RelativePath;
					string target		= Path.Combine(manifest.InstallPath, relative);
					string sourceFile	= f.SourcePath;

					if (File.Exists(sourceFile))
					{
						CopyFile(sourceFile, target);

						Program.OffFileInfoReadOnly(target);

						if(manifest.FileList.IndexOf(target) == -1) manifest.FileList.Add(target);

						doneCount++;
						ProgressChanged?.Invoke((int)((doneCount / (double)total) * 100));
						Log($"{relative} " + strings.LogUpdateCopy + " ({doneCount}/{total})");

					}
					else
					{
						//Log($"警告: {relative} が見つかりませんでした。");
					}
				}

				var manifestPath	= Path.Combine(manifest.InstallPath, "uninstall.ins");
				manifest.SaveManifest(manifestPath);

				Log( strings.LogUpdateComplete );

				return true;

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
		// ファイルをバッファでコピー（最適化版）
		//-----------------------------------------------------------
		private void CopyFile(string sourceFile, string targetFile)
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

					while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
					{
						targetStream.Write(buffer, 0, bytesRead);
					}
				}
			}
			catch (Exception ex)
			{
				// エラーハンドリング
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
