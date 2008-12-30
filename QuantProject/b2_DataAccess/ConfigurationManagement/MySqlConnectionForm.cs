/*
QuantProject - Quantitative Finance Library

ConfigManager.cs
Copyright (C) 2008
Glauco Siliprandi

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Reads data from the user and returns the MySQL connection string for those data
	/// </summary>
	public partial class MySqlConnectionForm : Form
	{
		private string mySqlConnectionString;
		
		public string MySqlConnectionString {
			get { return this.mySqlConnectionString; }
		}
		
		public MySqlConnectionForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		#region ButtonOkClick
		void ButtonOkClick(object sender, EventArgs e)
		{
			this.mySqlConnectionString =
				"Database=" + this.textBoxDataBase.Text +
				";Data Source=" + this.textBoxServerHost.Text +
				";User Id=" + this.textBoxUsername.Text +
				";Password=" + this.textBoxPassword.Text;
			this.Close();
		}
		#endregion ButtonOkClick
	}
}
