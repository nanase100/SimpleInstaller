using System;
namespace SimpleInstaller
{
	partial class UninstallForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Initialize form components.
		/// Minimal implementation so InitializeComponent exists for the partial class.
		/// </summary>
		private void InitializeComponent()
		{
			button1 = new Button();
			pictureBoxBanner = new PictureBox();
			checkBox1 = new CheckBox();
			label2 = new Label();
			textBox1 = new TextBox();
			((System.ComponentModel.ISupportInitialize)pictureBoxBanner).BeginInit();
			SuspendLayout();
			// 
			// button1
			// 
			button1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			button1.Location = new Point(305, 149);
			button1.Name = "button1";
			button1.Size = new Size(130, 40);
			button1.TabIndex = 0;
			button1.Text = "アンインストール";
			button1.UseVisualStyleBackColor = true;
			button1.Click += Btn_Click;
			// 
			// pictureBoxBanner
			// 
			pictureBoxBanner.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			pictureBoxBanner.Location = new Point(15, 10);
			pictureBoxBanner.Name = "pictureBoxBanner";
			pictureBoxBanner.Size = new Size(420, 80);
			pictureBoxBanner.SizeMode = PictureBoxSizeMode.CenterImage;
			pictureBoxBanner.TabIndex = 1;
			pictureBoxBanner.TabStop = false;
			// 
			// checkBox1
			// 
			checkBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			checkBox1.AutoSize = true;
			checkBox1.Location = new Point(15, 160);
			checkBox1.Name = "checkBox1";
			checkBox1.Size = new Size(130, 19);
			checkBox1.TabIndex = 3;
			checkBox1.Text = "セーブデータも削除する";
			checkBox1.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(15, 100);
			label2.Name = "label2";
			label2.Size = new Size(125, 15);
			label2.TabIndex = 4;
			label2.Text = "アンインストール先フォルダ";
			// 
			// textBox1
			// 
			textBox1.BackColor = SystemColors.Window;
			textBox1.Location = new Point(15, 120);
			textBox1.Name = "textBox1";
			textBox1.ReadOnly = true;
			textBox1.Size = new Size(420, 23);
			textBox1.TabIndex = 10;
			// 
			// UninstallForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(454, 211);
			Controls.Add(textBox1);
			Controls.Add(label2);
			Controls.Add(checkBox1);
			Controls.Add(pictureBoxBanner);
			Controls.Add(button1);
			MaximizeBox = false;
			MinimumSize = new Size(470, 250);
			Name = "UninstallForm";
			Load += UninstallForm_Load;
			((System.ComponentModel.ISupportInitialize)pictureBoxBanner).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private Button button1;
		private PictureBox pictureBoxBanner;
		private CheckBox checkBox1;
		private Label label2;
		private TextBox textBox1;
	}
}
