/*
QuantProject - Quantitative Finance Library

DbTypeChooser.cs
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
	/// Use this form to let the user chose the database type to be used
	/// </summary>
	public partial class DbTypeChooser : Form
	{
		private DbType dbType;
		
		public DbType DbType {
			get { return this.dbType; }
		}
		
		
		public DbTypeChooser()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		#region ButtonSelectClick
		private bool hasTheDatabaseTypeBeenSelected()
		{
			bool hasBeenSelected =
				( this.radioButtonAccess.Checked || this.radioButtonMySql.Checked );
			return hasBeenSelected;
		}
		private void assign_dbType()
		{
			if ( this.radioButtonAccess.Checked )
				this.dbType = DbType.Access;
			if ( this.radioButtonMySql.Checked )
				this.dbType = DbType.MySql;
		}
		void ButtonSelectClick(object sender, EventArgs e)
		{
			if ( this.hasTheDatabaseTypeBeenSelected() )
				// at least a radio button has been selected
				this.assign_dbType();
			else
				// no radio button has been selected
				MessageBox.Show( "Select the database type, please" );
			this.Close();
		}
		#endregion ButtonSelectClick
		
		void DbTypeChooserLoad(object sender, EventArgs e)
		{
			this.radioButtonAccess.Checked = false;
		}
	}
}
