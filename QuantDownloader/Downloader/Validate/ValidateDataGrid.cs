using System;
using System.Windows.Forms;

namespace QuantProject.Applications.Downloader.Validate
{
	/// <summary>
	/// Summary description for ValidateDataGrid.
	/// </summary>
	public class ValidateDataGrid : DataGrid
	{
    private ValidateDataTable validateDataTable;

    private DataGridTableStyle dataGridTableStyle;

		public ValidateDataGrid()
		{
		}
    #region "Validate"
    private void validate_setTableStyle_setColumnStyle( string mappingName , string headerText )
    {
      this.dataGridTableStyle.ColumnHeadersVisible = true;
      DataGridTextBoxColumn dataGridColumnStyle = new DataGridTextBoxColumn();
      dataGridColumnStyle.MappingName = mappingName;
      dataGridColumnStyle.HeaderText = headerText;
      this.dataGridTableStyle.GridColumnStyles.Add( dataGridColumnStyle );
    }
    private void validate_setTableStyle()
    {
//      DataGridTableStyle dataGridTableStyle = new DataGridTableStyle();
//      dataGridTableStyle.ColumnHeadersVisible = true;
//      dataGridTableStyle.MappingName = "quotes";
//      DataGridTextBoxColumn dataGridColumnStyle = new DataGridTextBoxColumn();
//      dataGridColumnStyle.MappingName = "quTicker";
//      dataGridColumnStyle.HeaderText = "Ticker";
//      dataGridTableStyle.GridColumnStyles.Add( dataGridColumnStyle );
//      this.TableStyles.Add( dataGridTableStyle );
      this.dataGridTableStyle = new DataGridTableStyle();
      this.dataGridTableStyle.MappingName = "quotes";
      validate_setTableStyle_setColumnStyle( "quTicker" , "Ticker" );
      validate_setTableStyle_setColumnStyle( "quDate" , "Date" );
      validate_setTableStyle_setColumnStyle( "quOpen" , "Open" );
      validate_setTableStyle_setColumnStyle( "quHigh" , "High" );
      validate_setTableStyle_setColumnStyle( "quLow" , "Low" );
      validate_setTableStyle_setColumnStyle( "quClose" , "Close" );
      validate_setTableStyle_setColumnStyle( "quAdjustedClose" , "Adj. Close" );
      this.TableStyles.Add( dataGridTableStyle );
    }
    public ValidateDataTable Validate( string tickerIsLike , string suspiciousRatio )
    {
      this.validateDataTable = new ValidateDataTable();
      this.DataSource = validateDataTable;
      if ( this.TableStyles.Count == 0 )
        // styles have not been defined yet
        validate_setTableStyle();
      validateDataTable.AddRows( tickerIsLike , Convert.ToDouble( suspiciousRatio ) );
      return this.validateDataTable;
    }
    #endregion
	}
}
