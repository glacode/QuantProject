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
    private void setTableStyles_setColumnStyle( string mappingName , string headerText ,
      HorizontalAlignment horizontalAlignment , string format )
    {
      DataGridTextBoxColumn dataGridColumnStyle = new DataGridTextBoxColumn();
      dataGridColumnStyle.MappingName = mappingName;
      dataGridColumnStyle.HeaderText = headerText;
      dataGridColumnStyle.Alignment = horizontalAlignment;
      dataGridColumnStyle.Format = format;
//      dataGridColumnStyle.Format = "#,#.00";
      Graphics g = this.CreateGraphics();
      dataGridColumnStyle.Width = (int)g.MeasureString( headerText , this.Font ).Width + 5;
      this.dataGridTableStyle.GridColumnStyles.Add( dataGridColumnStyle );
    }
    private void setTableStyles_setColumnStyle( string mappingName , string headerText ,
      HorizontalAlignment horizontalAlignment )
    {
      this.setTableStyles_setColumnStyle( mappingName , headerText , horizontalAlignment ,
        "#,#.00" );
    }
    private void setTableStyles_setColumnStyle( string mappingName , string headerText )
    {
      this.setTableStyles_setColumnStyle( mappingName , headerText , HorizontalAlignment.Left );
    }
    private void setTableStyles()
    {
      this.dataGridTableStyle = new DataGridTableStyle();
      this.dataGridTableStyle.MappingName = "quotes";
      this.RowHeaderWidth = 15;
      this.setTableStyles_setColumnStyle( "quTicker" , "Ticker" , HorizontalAlignment.Left , "" );
      this.setTableStyles_setColumnStyle( "quDate" , "Date" , HorizontalAlignment.Left , "d" );
      this.setTableStyles_setColumnStyle( "quOpen" , "Open" , HorizontalAlignment.Right , "#,#.00" );
      this.setTableStyles_setColumnStyle( "quHigh" , "High" , HorizontalAlignment.Right , "#,#.00" );
      this.setTableStyles_setColumnStyle( "quLow" , "Low" , HorizontalAlignment.Right , "#,#.00" );
      this.setTableStyles_setColumnStyle( "quClose" , "Close" , HorizontalAlignment.Right ,
        "#,#.00" );
      this.setTableStyles_setColumnStyle( "quAdjustedClose" , "Adj. Close" ,
        HorizontalAlignment.Right , "#,#.00" );
      this.setTableStyles_setColumnStyle( "ValidationWarning" , "Warning" ,
        HorizontalAlignment.Left , "");
      this.setTableStyles_setColumnStyle( "Yuppy" , "Ew" );
      this.TableStyles.Add( dataGridTableStyle );
    }
    #endregion
	}
}
