/*
QuantProject - Quantitative Finance Library

ReportTabControl.cs
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
using System.Data;
using System.Windows.Forms;

using QuantProject.Business.Financial.Accounting.Reporting;

namespace QuantProject.Presentation.Reporting.WindowsForm
{
	/// <summary>
	/// DataGrid to be displayed within a report TabPage
	/// </summary>
	[Serializable]
	public class ReportGrid : DataGrid
	{
		private ReportTable reportTable;
		private DataGridTableStyle dataGridStyle;
		
		public ReportGrid( ReportTable reportTable )
		{
			this.reportTable = reportTable;
			this.DataSource = reportTable.DataTable;
			this.setFormat();
		}
		
		#region setFormat
		
		private void setDataGridTableStyle()
		{
			this.dataGridStyle = new DataGridTableStyle();
			dataGridStyle.MappingName = this.reportTable.DataTable.TableName;
			this.TableStyles.Add( dataGridStyle );
		}
		
		#region addDataGridColumnStyles
		
		#region setFormatForDateTimeColumn
		
		#region setFormatForDateTimeColumn_actually
//		private DataGridTextBoxColumn getDataGridTextBoxColumn(
//			int columnIndex )
//		{
//			DataGridTableStyle dataGridTableStyle =
//				this.TableStyles[ 0 ];
//			DataGridTextBoxColumn dataGridTextBoxColumn =
//				( DataGridTextBoxColumn )dataGridTableStyle.GridColumnStyles[
//					columnIndex ];
////			dataGridTableStyle.GridColumnStyles.a
//			return dataGridTextBoxColumn;
//		}
		private DataGridTextBoxColumn addDataGridTextBoxColumn( int columnIndex )
		{
			DataGridTextBoxColumn dataGridTextBoxColumn =
				new DataGridTextBoxColumn();
			dataGridTextBoxColumn.MappingName =
				this.reportTable.DataTable.Columns[ columnIndex ].ColumnName;
			dataGridTextBoxColumn.HeaderText =
				this.reportTable.DataTable.Columns[ columnIndex ].ColumnName;
			this.TableStyles[ 0 ].GridColumnStyles.Add( dataGridTextBoxColumn );
			return dataGridTextBoxColumn;
		}
		#endregion setFormatForDateTimeColumn_actually
		
		private void addDataGridColumnStyle( int columnIndex )
		{
			DataGridTextBoxColumn dataGridTextBoxColumn =
				this.addDataGridTextBoxColumn( columnIndex );
			if ( this.reportTable.DataTable.Columns[ columnIndex ].DataType ==
			    System.Type.GetType("System.DateTime") )
				dataGridTextBoxColumn.Format = "u";
		}
		#endregion setFormatForDateTimeColumn
		
		private void addDataGridColumnStyles()
		{
			for( int columnIndex = 0 ;
			    columnIndex < this.reportTable.DataTable.Columns.Count ;
			    columnIndex++ )
				this.addDataGridColumnStyle( columnIndex );
		}
		#endregion addDataGridColumnStyles
		
		private void setFormat()
		{
			this.setDataGridTableStyle();
			this.addDataGridColumnStyles();
		}
		#endregion setFormat
	}
}
