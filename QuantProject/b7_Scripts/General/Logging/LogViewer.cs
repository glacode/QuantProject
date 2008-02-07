/*
QuantProject - Quantitative Finance Library

LogViewer.cs
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

using QuantProject.Business.Strategies.Logging;

namespace QuantProject.Scripts.General.Logging
{
	/// <summary>
	/// This Form displays a BackTestLog and executes a specific script for
	/// every single LogItem
	/// </summary>
	public class LogViewer : System.Windows.Forms.Form
	{
		private BackTestLog backTestLog;
		private System.Windows.Forms.DataGrid dataGridLogItems;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LogViewer( BackTestLog backTestLog )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.backTestLog = backTestLog;
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
				new ArrayList( this.backTestLog );
			this.dataGridLogItems.DataSource =
				dataGridItems;
			this.dataGridLogItems.Refresh();
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
			this.dataGridLogItems = new System.Windows.Forms.DataGrid();
			((System.ComponentModel.ISupportInitialize)(this.dataGridLogItems)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridLogItems
			// 
			this.dataGridLogItems.DataMember = "";
			this.dataGridLogItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridLogItems.Location = new System.Drawing.Point(16, 16);
			this.dataGridLogItems.Name = "dataGridLogItems";
			this.dataGridLogItems.Size = new System.Drawing.Size(568, 248);
			this.dataGridLogItems.TabIndex = 0;
			this.dataGridLogItems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridLogItems_MouseUp);
			// 
			// LogViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(592, 278);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.dataGridLogItems});
			this.Name = "LogViewer";
			this.Text = "LogViewer";
			((System.ComponentModel.ISupportInitialize)(this.dataGridLogItems)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		#region dataGridLogItems_MouseUp
		private int getRowNumber(  object sender , MouseEventArgs eventArgs )
		{
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( eventArgs.X , eventArgs.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			//			DataTable dataTable = (DataTable)dataGrid.DataSource;
			return hitTestInfo.Row;
		}
		private LogItem	getLogItem(	DataGrid dataGrid , int rowNumber )
		{
			//			DataTable dataTable = (DataTable)dataGrid.DataSource;
			LogItem logItem = this.backTestLog[ rowNumber ];
			return logItem;
		}
		private LogItem	getLogItem(
			object sender, System.Windows.Forms.MouseEventArgs e )
		{
			int rowNumber = this.getRowNumber( sender , e );
			LogItem logItem = this.getLogItem( (DataGrid)sender , rowNumber );
			return logItem;
		}
		private void rightClickEventHandler(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//			wFLagWeightedPositions wFLagChosenPositions =
			//				this.rightClickEventHandler_getWFLagChosenPositions( sender , e );
			LogItem logItem = this.getLogItem( sender , e );
			logItem.Run();
		}
		private void dataGridLogItems_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Right )
				this.rightClickEventHandler( sender , e );
		}
		#endregion dataGridLogItems_MouseUp
	}
}
