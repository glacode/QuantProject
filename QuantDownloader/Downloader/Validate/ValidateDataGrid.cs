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
		public ValidateDataGrid()
		{
			//
			// TODO: Add constructor logic here
			//
		}
    #region "Validate"
    private void validate_setTableStyle()
    {
      DataGridTableStyle dataGridTableStyle = new DataGridTableStyle();
      dataGridTableStyle.ColumnHeadersVisible = true;
      dataGridTableStyle.MappingName = "quotes";
      DataGridTextBoxColumn dataGridColumnStyle = new DataGridTextBoxColumn();
      dataGridColumnStyle.MappingName = "quTicker";
      dataGridColumnStyle.HeaderText = "Ticker";
      dataGridTableStyle.GridColumnStyles.Add( dataGridColumnStyle );
      this.TableStyles.Add( dataGridTableStyle );
    }
    public ValidateDataTable Validate( string tickerIsLike , string suspiciousRatio )
    {
      this.validateDataTable = new ValidateDataTable();
      this.DataSource = validateDataTable;
      validate_setTableStyle();
      validateDataTable.AddRows( tickerIsLike , Convert.ToDouble( suspiciousRatio ) );
      return this.validateDataTable;
    }
    #endregion
	}
}
