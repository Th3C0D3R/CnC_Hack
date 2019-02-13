using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CnC_Hack
{
	public partial class FormLog : Form
	{
		public FormLog()
		{
			InitializeComponent();
		}

		public void Write(string text)
		{
			lbLog.Items.Add(text);
		}
	}
}
