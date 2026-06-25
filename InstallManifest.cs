using System;
using System.Collections.Generic;

namespace SimpleInstaller
{
	public class InstallManifest
	{
		public string MakerName				{ get; set; } = "";
		public string GameTitle				{ get; set; } = "";
		public string Version				{ get; set; } = "";
		public string MainExeName			{ get; set; } = "";
		public string InstallPath			{ get; set; } = "";
		public string SaveFolder			{ get; set; } = "";
		public List<string> FileList		{ get; set; } = new List<string>();
		public List<string> DiscNameList	{ get; set; } = new List<string>();

		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		public void LoadManifest( string path )
		{
			var buf		= File.ReadAllText( path );
			var lines	= buf.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

			MakerName		= lines[0];
			GameTitle		= lines[1];
			Version			= lines[2];
			MainExeName		= lines[3];
			SaveFolder		= lines[4];
			InstallPath		= lines[5];

			FileList		= new List<string>();

			string	discName = "";

			for( int i = 6; i < lines.Length; i++)
			{
				if (!string.IsNullOrEmpty(lines[i]))
				{
					if(lines[i].IndexOf("[") == -1 )
					{
						FileList.Add(lines[i]);
						DiscNameList.Add( discName );
					}
					else
					{
						discName = lines[i];
					}
				}
			}
		}

		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		public void SaveManifest( string path )
		{
			string buf = "";

			buf += MakerName	+ Environment.NewLine;
			buf += GameTitle	+ Environment.NewLine;
			buf += Version		+ Environment.NewLine;
			buf += MainExeName	+ Environment.NewLine;
			buf += SaveFolder	+ Environment.NewLine;
			buf += InstallPath	+ Environment.NewLine;

			foreach (var f in FileList)
			{
				buf += f + Environment.NewLine;
			}

			File.WriteAllText(path, buf);
		}


		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		public void SetList(List<string> files)
		{
			FileList = files;
		}
	}
}



