namespace SimpleInstaller
{
	partial class updater
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			label2 = new Label();
			pictureBoxBanner = new PictureBox();
			button1 = new Button();
			textBox1 = new TextBox();
			((System.ComponentModel.ISupportInitialize)pictureBoxBanner).BeginInit();
			SuspendLayout();
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(15, 100);
			label2.Name = "label2";
			label2.Size = new Size(104, 15);
			label2.TabIndex = 8;
			label2.Text = "アップデート先フォルダ";
			// 
			// pictureBoxBanner
			// 
			pictureBoxBanner.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			pictureBoxBanner.Location = new Point(15, 12);
			pictureBoxBanner.Margin = new Padding(0);
			pictureBoxBanner.Name = "pictureBoxBanner";
			pictureBoxBanner.Size = new Size(420, 80);
			pictureBoxBanner.SizeMode = PictureBoxSizeMode.CenterImage;
			pictureBoxBanner.TabIndex = 6;
			pictureBoxBanner.TabStop = false;
			// 
			// button1
			// 
			button1.Anchor = AnchorStyles.None;
			button1.Enabled = false;
			button1.Location = new Point(305, 149);
			button1.Name = "button1";
			button1.Size = new Size(130, 32);
			button1.TabIndex = 5;
			button1.Text = "アップデート";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// textBox1
			// 
			textBox1.BackColor = SystemColors.Window;
			textBox1.Location = new Point(15, 120);
			textBox1.Name = "textBox1";
			textBox1.ReadOnly = true;
			textBox1.Size = new Size(420, 23);
			textBox1.TabIndex = 9;
			// 
			// updater
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(454, 211);
			Controls.Add(textBox1);
			Controls.Add(label2);
			Controls.Add(pictureBoxBanner);
			Controls.Add(button1);
			MaximizeBox = false;
			MaximumSize = new Size(470, 250);
			MinimumSize = new Size(470, 250);
			Name = "updater";
			Text = "updater";
			Load += updater_Load;
			Shown += updater_Shown;
			((System.ComponentModel.ISupportInitialize)pictureBoxBanner).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Label label2;
		private PictureBox pictureBoxBanner;
		private Button button1;
		private TextBox textBox1;
	}
}