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
using System.Data;
using System.Windows.Forms;
using QuantProject.Applications.Downloader.Validate;
using QuantProject.Business.Validation;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// DataGrid used for (record by record) visual validation
	/// </summary>
	public abstract class VisualValidationDataGrid : ValidationDataGrid
	{
		private DataView dataView;

		private bool styleIsDefined = false;

		protected string confirmMessage;
		protected ValidationWarning validationWarning;

		public VisualValidationDataGrid()
		{
			//
			// TODO: Add constructor logic here
			//
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
				Convert.ToInt16( this.validationWarning );
			this.DataSource = dataView;
			this.dataView.AllowNew = false;
			this.setStyles();
		}
		protected abstract void confirmVisualValidation( string ticker , DateTime quoteDate );
//		{
//			VisuallyValidatedTickers.ValidateCloseToClose( ((QuotesEditor)this.FindForm()).Ticker );
//		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
//			Console.WriteLine( "VisualValidationDataGrid.OnMouseUp() " +
//				this[ this.CurrentRowIndex , 0 ].ToString() );
//			((VisualValidationTabPage)this.Parent).VisualValidationChart.Invalidate( true );
			base.OnMouseUp( e );
		}
		protected override void OnClick( EventArgs e )
		{
			Console.WriteLine( "VisualValidationDataGrid.OnClick()" );
			base.OnClick( e );
			((VisualValidationTabPage)this.Parent).VisualValidationChart.SuspiciousDateTime =
				(DateTime)this[ this.CurrentRowIndex , 0 ];
//			((VisualValidationTabPage)this.Parent).Invalidate( true );
			if (this.TableStyles[0].GridColumnStyles[this.CurrentCell.ColumnNumber] is
				DataGridBoolColumn)
			{
				this[this.CurrentCell] = !System.Convert.ToBoolean(this[this.CurrentCell]);
				this.Select( true , true );
				if ( MessageBox.Show( this , "You have visually validated all the suspicious " +
					"quotes, with respect to the " + this.confirmMessage +
					". Do you confirm your " +
					"visual validation to be permanentely stored into the database?" ,
					"Visual Validation Confirmation" ,
					MessageBoxButtons.YesNo ,
					MessageBoxIcon.Question ,
					MessageBoxDefaultButton.Button1 ) == DialogResult.Yes )
					// the user asked to write the visual validation to the database
				{
					this.confirmVisualValidation( ((QuotesEditor)this.FindForm()).Ticker ,
						(DateTime)this[ this.CurrentCell.RowNumber , 0 ] );
					((QuotesEditor)this.FindForm()).Renew();
				}
			}
//			DataView checkedDataView = new DataView( ((QuotesEditor)this.FindForm()).ValidateDataTable );
//			checkedDataView.RowFilter = "(" + this.dataView.RowFilter +
//				" AND (CloseToCloseHasBeenVisuallyValidated=true))";
//			int checkedItems = 0;
//			for (int rowIndex=0 ; rowIndex<this.dataView.Count ; rowIndex++)
//				if ( (bool)this[ rowIndex , 2] )
//					checkedItems++;
//			if ( checkedItems == this.dataView.Count )
//				// all suspicious data rows have been visually validated
//				if ( MessageBox.Show( this , "You have visually validated all the suspicious " +
//					"quotes, with respect to the " + this.confirmMessage +
//					". Do you confirm your " +
//					"visual validation to be permanentely stored into the database?" ,
//					"Visual Validation Confirmation" ,
//					MessageBoxButtons.YesNo ,
//					MessageBoxIcon.Question ,
//					MessageBoxDefaultButton.Button1 ) == DialogResult.Yes )
//					// the user asked to write the visual validation to the database
//					this.confirmVisualValidation();
		}
		protected override void OnPaint( PaintEventArgs e )
		{
			if ( !this.styleIsDefined )
				// no style has been defined yet
				this.DataBind();
			base.OnPaint( e );
		}
	}
}
