using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CnC_Hack
{
	public partial class FormOverlay : Form
	{
		IntPtr handle;
		string processName;
		int entryBaseAddress;
		RECT rect;
		public struct RECT
		{
			public int left, top, right, bottom;
		}

		FormLog frmLog = new FormLog();

		Graphics g;
		Pen myPen = new Pen(Color.Red);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		[DllImport("user32.dll", SetLastError = true)]
		static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll", SetLastError = true)]
		static extern int GetWindowRect(IntPtr hWnd, out RECT lpRect);

		public FormOverlay(string[] args)
		{
			InitializeComponent();
			processName = args[0];
			entryBaseAddress = int.Parse(args[1]);
		}

		private void FormOverlay_Load(object sender, EventArgs e)
		{
			frmLog.Show();
			frmLog.Write("INIT OVERLAY");
			this.BackColor = Color.Aqua;
			this.TransparencyKey = Color.Aqua;
			this.TopMost = true;
			frmLog.Write("TopMost = true");
			this.Text = "TEST!!";
			this.DoubleBuffered = true;
			frmLog.Write("DoubleBuffered = true");
			//this.FormBorderStyle = FormBorderStyle.None;

			int initialStyle = GetWindowLong(this.Handle, -20);
			SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);

			GetWindowRect(handle, out rect);
			frmLog.Write("rect = " + rect.ToString());

			this.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);
			this.Top = rect.top;
			this.Left = rect.left;
			frmLog.Write("Top = " + rect.top);
			frmLog.Write("Left = " + rect.left);

		}
		private void FormOverlay_Paint(object sender, PaintEventArgs e)
		{
			GetWindowRect(handle, out rect);
			frmLog.Write("rect = " + rect.ToString());

			this.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);
			this.Top = rect.top;
			this.Left = rect.left;
			frmLog.Write("Top = " + rect.top);
			frmLog.Write("Left = " + rect.left);

		}
		public void SetWindow(IntPtr wndHandle)
		{
			handle = wndHandle;
		}

		private void FormOverlay_FormClosing(object sender, FormClosingEventArgs e)
		{
			frmLog.Close();
		}
	}
}
