using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace SimpleInstaller
{
	public partial class updater : Form
	{


		private UpdaterLogic m_updater = new UpdaterLogic();

		public updater()
		{
			InitializeComponent();
			StartPosition = FormStartPosition.CenterScreen;
			this.Text = $"{Program.m_info.GameTitle} - {Program.m_info.Version} " + strings.titleUpdate;
		}

		private void updater_Load(object sender, EventArgs e)
		{


			this.Icon = Program.GetAppWin32Icon();

			CultureUI();

			var banner = Path.Combine(Directory.GetCurrentDirectory(), "update.png");
			if (File.Exists(banner)) pictureBoxBanner.Load(banner);

			if (Program.m_REG_KEY != null)
			{
				textBox1.Text = Program.m_REG_KEY?.GetValue("InstallPath") as string;
				button1.Enabled = true;
			}
			else
			{
			//	var str = strings.MsgUpdateNotinstalled;
			//	str = str.Replace("%gametitle%", Program.m_info.GameTitle);
			//	str = str.Replace("\\n", Environment.NewLine);
			//	MessageBox.Show(str, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void CultureUI()
		{
			var culture = System.Globalization.CultureInfo.CurrentUICulture;
			var name = System.Globalization.CultureInfo.CurrentCulture.Name;

			if (name != "ja-JP") name = "en-US";

			Thread.CurrentThread.CurrentUICulture = new CultureInfo(name);

			button1.Text = strings.updateBtnText;
			label2.Text = strings.updatePath;

		}

		private async void button1_Click(object sender, EventArgs e)
		{
			await DoUpdate();
		}


		private void EnableUI(bool isEnable)
		{
			button1.Enabled = isEnable;
		}

		//-----------------------------------------------------------
		//
		//-----------------------------------------------------------
		private async Task DoUpdate()
		{
			EnableUI(false);

			try
			{
				var result = await Task.Run(() => m_updater.Update());

				MessageBox.Show(this, result ? strings.MsgUpdateComp : strings.MsgUpdateCancel, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			finally
			{
				EnableUI(true);
			}
		}

		private void updater_Shown(object sender, EventArgs e)
		{
			if (Program.m_REG_KEY == null)
			{
				var str = strings.MsgUpdateNotinstalled;
				str = str.Replace("%gametitle%", Program.m_info.GameTitle);
				str = str.Replace("\\n", Environment.NewLine);
				MessageBox.Show(str, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}

}
