namespace SimpleInstaller
{
	partial class MainForm
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

		private void InitializeComponent()
		{
			pictureBoxBanner = new PictureBox();
			lblTitle = new Label();
			lblStatus = new Label();
			txtInstallPath = new TextBox();
			btnBrowse = new Button();
			chkDesktopShortcut = new CheckBox();
			chkRunAfter = new CheckBox();
			btnInstall = new Button();
			progressBar = new ProgressBar();
			btnBoot = new Button();
			((System.ComponentModel.ISupportInitialize)pictureBoxBanner).BeginInit();
			SuspendLayout();
			// 
			// pictureBoxBanner
			// 
			pictureBoxBanner.Location = new Point(12, 12);
			pictureBoxBanner.Name = "pictureBoxBanner";
			pictureBoxBanner.Size = new Size(560, 120);
			pictureBoxBanner.SizeMode = PictureBoxSizeMode.CenterImage;
			pictureBoxBanner.TabIndex = 0;
			pictureBoxBanner.TabStop = false;
			// 
			// lblTitle
			// 
			lblTitle.AutoSize = true;
			lblTitle.Location = new Point(12, 144);
			lblTitle.Name = "lblTitle";
			lblTitle.Size = new Size(164, 15);
			lblTitle.TabIndex = 1;
			lblTitle.Text = "インストール先を指定してください。";
			lblTitle.Click += ResXManager;
			// 
			// lblStatus
			// 
			lblStatus.AutoSize = true;
			lblStatus.Location = new Point(15, 290);
			lblStatus.Name = "lblStatus";
			lblStatus.Size = new Size(43, 15);
			lblStatus.TabIndex = 8;
			lblStatus.Text = "準備中";
			// 
			// txtInstallPath
			// 
			txtInstallPath.Location = new Point(15, 165);
			txtInstallPath.Name = "txtInstallPath";
			txtInstallPath.Size = new Size(460, 23);
			txtInstallPath.TabIndex = 2;
			// 
			// btnBrowse
			// 
			btnBrowse.Location = new Point(482, 165);
			btnBrowse.Name = "btnBrowse";
			btnBrowse.Size = new Size(90, 23);
			btnBrowse.TabIndex = 3;
			btnBrowse.Text = "参照...";
			btnBrowse.UseVisualStyleBackColor = true;
			btnBrowse.Click += btnBrowse_Click;
			// 
			// chkDesktopShortcut
			// 
			chkDesktopShortcut.AutoSize = true;
			chkDesktopShortcut.Location = new Point(15, 200);
			chkDesktopShortcut.Name = "chkDesktopShortcut";
			chkDesktopShortcut.Size = new Size(197, 19);
			chkDesktopShortcut.TabIndex = 4;
			chkDesktopShortcut.Text = "デスクトップにショートカットを作成する";
			chkDesktopShortcut.UseVisualStyleBackColor = true;
			// 
			// chkRunAfter
			// 
			chkRunAfter.AutoSize = true;
			chkRunAfter.Location = new Point(15, 225);
			chkRunAfter.Name = "chkRunAfter";
			chkRunAfter.Size = new Size(178, 19);
			chkRunAfter.TabIndex = 5;
			chkRunAfter.Text = "インストール後にアプリを起動する";
			chkRunAfter.UseVisualStyleBackColor = true;
			// 
			// btnInstall
			// 
			btnInstall.Location = new Point(482, 200);
			btnInstall.Name = "btnInstall";
			btnInstall.Size = new Size(90, 50);
			btnInstall.TabIndex = 6;
			btnInstall.Text = "インストール";
			btnInstall.UseVisualStyleBackColor = true;
			btnInstall.Click += btnInstall_Click;
			// 
			// progressBar
			// 
			progressBar.Location = new Point(15, 260);
			progressBar.Name = "progressBar";
			progressBar.Size = new Size(557, 23);
			progressBar.TabIndex = 7;
			// 
			// btnBoot
			// 
			btnBoot.Enabled = false;
			btnBoot.Location = new Point(386, 200);
			btnBoot.Name = "btnBoot";
			btnBoot.Size = new Size(90, 50);
			btnBoot.TabIndex = 9;
			btnBoot.Text = "起動";
			btnBoot.UseVisualStyleBackColor = true;
			btnBoot.Click += btnBoot_Click;
			// 
			// MainForm
			// 
			ClientSize = new Size(584, 321);
			Controls.Add(btnBoot);
			Controls.Add(lblStatus);
			Controls.Add(progressBar);
			Controls.Add(btnInstall);
			Controls.Add(chkRunAfter);
			Controls.Add(chkDesktopShortcut);
			Controls.Add(btnBrowse);
			Controls.Add(txtInstallPath);
			Controls.Add(lblTitle);
			Controls.Add(pictureBoxBanner);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			Name = "MainForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Simple Installer";
			Load += MainForm_Load;
			((System.ComponentModel.ISupportInitialize)pictureBoxBanner).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		private System.Windows.Forms.PictureBox pictureBoxBanner;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.TextBox txtInstallPath;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.CheckBox chkDesktopShortcut;
		private System.Windows.Forms.CheckBox chkRunAfter;
		private System.Windows.Forms.Button btnInstall;
		private System.Windows.Forms.ProgressBar progressBar;
		private Button btnBoot;
	}
}
