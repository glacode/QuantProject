/*
QuantProject - Quantitative Finance Library

QuotesDataGrid.cs
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
using System.Windows.Forms;
using System.Drawing;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Base class for data grids containing quotes
	/// </summary>
	public class QuotesDataGrid : DataGrid
	{
    DataGridTableStyle dataGridTableStyle;
		public QuotesDataGrid()
		{
			this.setTableStyles();
		}
    #region setTableStyles
    private void setTableStyles_setColumnStyle( string mappingName , string headerText )
    {
      this.dataGridTableStyle.ColumnHeadersVisible = true;
      DataGridTextBoxColumn dataGridColumnStyle = new DataGridTextBoxColumn();
      dataGridColumnStyle.MappingName = mappingName;
      dataGridColumnStyle.HeaderText = headerText;
      Graphics g = this.CreateGraphics();
      dataGridColumnStyle.Width = (int)g.MeasureString( headerText , this.Font ).Width + 5;
      this.dataGridTableStyle.GridColumnStyles.Add( dataGridColumnStyle );
    }
    private void setTableStyles()
    {
      this.dataGridTableStyle = new DataGridTableStyle();
      this.dataGridTableStyle.MappingName = "quotes";
      this.setTableStyles_setColumnStyle( "quTicker" , "Ticker" );
      this.setTableStyles_setColumnStyle( "quDate" , "Date" );
      this.setTableStyles_setColumnStyle( "quOpen" , "Open" );
      this.setTableStyles_setColumnStyle( "quHigh" , "High" );
      this.setTableStyles_setColumnStyle( "quLow" , "Low" );
      this.setTableStyles_setColumnStyle( "quClose" , "Close" );
      this.setTableStyles_setColumnStyle( "quAdjustedClose" , "Adj. Close" );
      this.setTableStyles_setColumnStyle( "ValidationWarning" , "Warning" );
      this.setTableStyles_setColumnStyle( "Yuppy" , "Ew" );
      this.TableStyles.Add( dataGridTableStyle );
    }
    #endregion
	}
}
