/*
QuantProject - Quantitative Finance Library

WFLagDebugChosenPositionsCollection.cs
Copyright (C) 2003
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

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Form to debug a chosen positions' collection
	/// </summary>
	public class WFLagDebugChosenPositionsCollection : System.Windows.Forms.Form
	{
		private System.Windows.Forms.DataGrid dataGridChosenPositionsWithAttributes;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ArrayList chosenPositionsDebugInfoList;
		private int inSampleDays;
		string benchmark;

		public WFLagDebugChosenPositionsCollection( int inSampleDays , string benchmark ,
			ArrayList chosenPositionsDebugInfoList )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.inSampleDays = inSampleDays;
			this.benchmark = benchmark;
			this.chosenPositionsDebugInfoList = chosenPositionsDebugInfoList;
			this.setDataGridTableStyle();
			this.updateGrid();
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
			this.dataGridChosenPositionsWithAttributes = new System.Windows.Forms.DataGrid();
			((System.ComponentModel.ISupportInitialize)(this.dataGridChosenPositionsWithAttributes)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridChosenPositionsWithAttributes
			// 
			this.dataGridChosenPositionsWithAttributes.DataMember = "";
			this.dataGridChosenPositionsWithAttributes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridChosenPositionsWithAttributes.Location = new System.Drawing.Point(24, 40);
			this.dataGridChosenPositionsWithAttributes.Name = "dataGridChosenPositionsWithAttributes";
			this.dataGridChosenPositionsWithAttributes.Size = new System.Drawing.Size(576, 240);
			this.dataGridChosenPositionsWithAttributes.TabIndex = 0;
			this.dataGridChosenPositionsWithAttributes.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridChosenPositionsWithAttributes_MouseUp);
			// 
			// WFLagDebugChosenPositionsCollection
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(640, 301);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.dataGridChosenPositionsWithAttributes});
			this.Name = "WFLagDebugChosenPositionsCollection";
			this.Text = "WFLagDebugChosenPositionsCollection";
			((System.ComponentModel.ISupportInitialize)(this.dataGridChosenPositionsWithAttributes)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


		private void setDataGridTableStyle()
		{
			DataGridTableStyle dataGridTableStyle = new DataGridTableStyle( true );
//			DataGridColumnStyle dataGridColumnStyle = new DataGridColumnStyle();
		}
		private void updateGrid()
		{
			this.dataGridChosenPositionsWithAttributes.DataSource =
				this.chosenPositionsDebugInfoList;
			this.dataGridChosenPositionsWithAttributes.Refresh();
		}

		#region DataGridMouseClickEventHandler
		private int rightClickEventHandler_getRowNumber(  object sender ,
			MouseEventArgs eventArgs )
		{
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( eventArgs.X , eventArgs.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
//			DataTable dataTable = (DataTable)dataGrid.DataSource;
			return hitTestInfo.Row;
		}
		private WFLagChosenPositionsDebugInfo
			rightClickEventHandler_getWFLagChosenPositionsDebugInfo(
			DataGrid dataGrid , int rowNumber )
		{
//			DataTable dataTable = (DataTable)dataGrid.DataSource;
			WFLagChosenPositionsDebugInfo wFLagChosenPositionsDebugInfo =
				(WFLagChosenPositionsDebugInfo)(this.chosenPositionsDebugInfoList[ rowNumber ]);
			return wFLagChosenPositionsDebugInfo;
		}

		private WFLagChosenPositionsDebugInfo
			rightClickEventHandler_getWFLagChosenPositionsDebugInfo(
			object sender, System.Windows.Forms.MouseEventArgs e )
		{
			int rowNumber = rightClickEventHandler_getRowNumber(
				sender , e );
			WFLagChosenPositionsDebugInfo wFLagChosenPositionsDebugInfo =
				this.rightClickEventHandler_getWFLagChosenPositionsDebugInfo(
					(DataGrid)sender , rowNumber );
			return wFLagChosenPositionsDebugInfo;
		}
		private void rightClickEventHandler(object sender, System.Windows.Forms.MouseEventArgs e)
		{
//			wFLagWeightedPositions wFLagChosenPositions =
//				this.rightClickEventHandler_getWFLagChosenPositions( sender , e );
			WFLagChosenPositionsDebugInfo wFLagChosenPositionsDebugInfo =
				rightClickEventHandler_getWFLagChosenPositionsDebugInfo(
					sender , e );
			WFLagWeightedPositions wFLagWeightedPositions =
				wFLagChosenPositionsDebugInfo.GetChosenPositions();
			WFLagDebugGenome wFLagDebugGenome = new WFLagDebugGenome(
				wFLagWeightedPositions ,
				wFLagChosenPositionsDebugInfo.LastOptimization ,
				this.inSampleDays , this.benchmark );
			wFLagDebugGenome.Show();
		}
		private void dataGridChosenPositionsWithAttributes_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Right )
				this.rightClickEventHandler( sender , e );
		}
		#endregion
	}
}
