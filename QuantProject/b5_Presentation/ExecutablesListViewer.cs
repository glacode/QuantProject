/*
QuantProject - Quantitative Finance Library

ExecutablesListViewer.cs
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using QuantProject.Business.Scripting;

namespace QuantProject.Presentation
{
	/// <summary>
	/// Shows a list of Executable: right clicking an item fires
	/// the Run() of that executable
	/// </summary>
	public class ExecutablesListViewer : System.Windows.Forms.Form
	{
		private IList executables;
		private System.Windows.Forms.DataGrid executablesDataGrid;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// hows a list of Executable: right clicking an item fires
		/// the Run() of that executable
		/// </summary>
		/// <param name="executables">each element has to be an
		/// IExecutable</param>
		public ExecutablesListViewer( IList executables )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.executables = executables;
			this.setDataGridTableStyle();
			this.updateGrid();
		}

		private void setDataGridTableStyle()
		{
			DataGridTableStyle dataGridTableStyle = new DataGridTableStyle( true );
			//			DataGridColumnStyle dataGridColumnStyle = new DataGridColumnStyle();
		}
		private void updateGrid()
		{
			ArrayList dataGridItems =
				new ArrayList( this.executables );
			this.executablesDataGrid.DataSource =
				dataGridItems;
			this.executablesDataGrid.Refresh();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.executablesDataGrid = new System.Windows.Forms.DataGrid();
			((System.ComponentModel.ISupportInitialize)(this.executablesDataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// executablesDataGrid
			// 
			this.executablesDataGrid.DataMember = "";
			this.executablesDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.executablesDataGrid.Location = new System.Drawing.Point(8, 8);
			this.executablesDataGrid.Name = "executablesDataGrid";
			this.executablesDataGrid.Size = new System.Drawing.Size(808, 464);
			this.executablesDataGrid.TabIndex = 0;
			this.executablesDataGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.executablesDataGrid_MouseUp);
			// 
			// ExecutablesListViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(824, 478);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.executablesDataGrid});
			this.Name = "ExecutablesListViewer";
			this.Text = "ExecutablesListViewer";
			((System.ComponentModel.ISupportInitialize)(this.executablesDataGrid)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		#region executablesDataGrid_MouseUp
		private int getRowNumber(  object sender , MouseEventArgs eventArgs )
		{
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( eventArgs.X , eventArgs.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			//			DataTable dataTable = (DataTable)dataGrid.DataSource;
			return hitTestInfo.Row;
		}
		private void getDataGridItem_checkParameters( int rowNumber )
		{
			if ( !(this.executables[ rowNumber ] is QuantProject.Business.Scripting.IExecutable) )
				throw new Exception( "The ICollection given to the constructor " +
					"contains a non IExecutable item (rowNumber=" + rowNumber +
					"). This class requires an ICollection of IExecutable(s)!" );
		}
		private IExecutable	getDataGridItem(	DataGrid dataGrid , int rowNumber )
		{
			this.getDataGridItem_checkParameters( rowNumber );
			//			DataTable dataTable = (DataTable)dataGrid.DataSource;
			IExecutable executable = (IExecutable)this.executables[ rowNumber ];
			return executable;
		}
		private IExecutable	getDataGridItem(
			object sender, System.Windows.Forms.MouseEventArgs e )
		{
			int rowNumber = this.getRowNumber( sender , e );
			IExecutable executable = this.getDataGridItem( (DataGrid)sender , rowNumber );
			return executable;
		}
		private void rightClickEventHandler(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//			wFLagWeightedPositions wFLagChosenPositions =
			//				this.rightClickEventHandler_getWFLagChosenPositions( sender , e );
			IExecutable executable = this.getDataGridItem( sender , e );
			executable.Run();
		}
		private void executablesDataGrid_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Right )
				this.rightClickEventHandler( sender , e );
		}
		#endregion executablesDataGrid_MouseUp

	}
}
