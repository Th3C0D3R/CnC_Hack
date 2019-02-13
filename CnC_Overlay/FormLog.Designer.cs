namespace CnC_Hack
{
	partial class FormLog
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
			this.lbLog = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// lbLog
			// 
			this.lbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lbLog.FormattingEnabled = true;
			this.lbLog.ItemHeight = 20;
			this.lbLog.Location = new System.Drawing.Point(12, 12);
			this.lbLog.Name = "lbLog";
			this.lbLog.Size = new System.Drawing.Size(539, 724);
			this.lbLog.TabIndex = 0;
			// 
			// FormLog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(563, 746);
			this.Controls.Add(this.lbLog);
			this.Name = "FormLog";
			this.Text = "FormLog";
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.ListBox lbLog;
	}
}