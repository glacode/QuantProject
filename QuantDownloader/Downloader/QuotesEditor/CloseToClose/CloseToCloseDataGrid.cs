/*
QuantProject - Quantitative Finance Library

CloseToCloseDataGrid.cs
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
using QuantProject.DataAccess.Tables;
using QuantProject.Applications.Downloader.Validate;
using QuantProject.Business.Validation;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Contains the quotes with suspicious close to close ratio
	/// </summary>
	public class CloseToCloseDataGrid : ValidationDataGrid
	{
    private DataView dataView;

    private bool styleIsDefined = false;

		public CloseToCloseDataGrid()
    {
    }
    #region setStyles
    private void setStyles_set_DataGridBoolColumn_FalseValueChanged(object sender, EventArgs e)
    {
      MessageBox.Show( "Prova" );
    }
    private void setStyles_set_DataGridBoolColumn()
    {
      DataGridBoolColumn dataGridBoolColumn = new DataGridBoolColumn();
      dataGridBoolColumn.MappingName = "CloseToCloseHasBeenVisuallyValidated";
      dataGridBoolColumn.HeaderText = "Ok?";
      dataGridBoolColumn.Width = 30;
      dataGridBoolColumn.AllowNull = false;
      // quiiii!!!!!
//      dataGridBoolColumn. +=
//        new System.EventHandler( this.setStyles_set_DataGridBoolColumn_FalseValueChanged );
      this.TableStyles[ "quotes" ].GridColumnStyles.Add( dataGridBoolColumn );
      this.styleIsDefined = true;
    }
    public void setStyles()
    {
//      this.TableStyles.Clear();
      int i=0;
      while (i<TableStyles[ "quotes" ].GridColumnStyles.Count)
      {
        DataGridColumnStyle dataGridColumnStyle = this.TableStyles[ "quotes" ].GridColumnStyles[i];
        if ( ( dataGridColumnStyle.HeaderText != "Date" ) &&
          ( dataGridColumnStyle.HeaderText != "Adj. Close" ) )
          this.TableStyles[ "quotes" ].GridColumnStyles.Remove( dataGridColumnStyle );
        else
          i++;
      }
      setStyles_set_DataGridBoolColumn();
    }
    #endregion
    public override void DataBind()
    {
      ValidateDataTable validateDataTable = ((QuotesEditor)this.FindForm()).ValidateDataTable;
      this.dataView = new DataView( validateDataTable );
      this.dataView.RowFilter = "ValidationWarning=" +
        Convert.ToInt16( ValidationWarning.SuspiciousCloseToCloseRatio );
      this.DataSource = dataView;
      this.dataView.AllowNew = false;
      this.setStyles();
    }
		private void confirmVisualValidation()
		{
			VisuallyValidatedTickers.ValidateCloseToClose( ((QuotesEditor)this.FindForm()).Ticker );
		}
    protected override void OnClick( EventArgs e )
    {
			Console.WriteLine( "CloseToCloseDataGrid.OnClick()" );
      if (this.TableStyles[0].GridColumnStyles[this.CurrentCell.ColumnNumber] is
        DataGridBoolColumn)
      {
        this[this.CurrentCell] = !System.Convert.ToBoolean(this[this.CurrentCell]);
        this.Select( true , true );
      }
      DataView checkedDataView = new DataView( ((QuotesEditor)this.FindForm()).ValidateDataTable );
      checkedDataView.RowFilter = "(" + this.dataView.RowFilter +
        " AND (CloseToCloseHasBeenVisuallyValidated=true))";
      int checkedItems = 0;
      for (int rowIndex=0 ; rowIndex<this.dataView.Count ; rowIndex++)
        if ( (bool)this[ rowIndex , 2] )
          checkedItems++;
			if ( checkedItems == this.dataView.Count )
				// all suspicious close to close ratios have been visually validated
				if ( MessageBox.Show( this , "You have visually validated all the suspicious " +
					"quotes, with respect to the close to close ratio. Do you confirm your " +
					"visual validation to be permanentely stored into the database?" ,
					"Visual Validation Confirmation" ,
					MessageBoxButtons.YesNo ,
					MessageBoxIcon.Question ,
					MessageBoxDefaultButton.Button1 ) == DialogResult.Yes )
					// the user asked to write the visual validation to the database
					this.confirmVisualValidation();
    }
    protected override void OnPaint( PaintEventArgs e )
    {
			Console.WriteLine( "CloseToCloseDataGrid.OnPaint()" );
      if ( !this.styleIsDefined )
        // no style has been defined yet
        this.DataBind();
      base.OnPaint( e );
    }
  }
}
