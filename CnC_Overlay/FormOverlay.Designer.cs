namespace CnC_Hack
{
	partial class FormOverlay
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
			this.SuspendLayout();
			// 
			// FormOverlay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1374, 914);
			this.Name = "FormOverlay";
			this.Text = "FormOverlay";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormOverlay_FormClosing);
			this.Load += new System.EventHandler(this.FormOverlay_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormOverlay_Paint);
			this.ResumeLayout(false);

		}

		#endregion
	}
}