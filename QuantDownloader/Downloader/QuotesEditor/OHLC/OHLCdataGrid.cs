using System;
using System.Data;
using System.Windows.Forms;
using QuantProject.Applications.Downloader.Validate;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// It contains the quotes with Open High Low Close inconsistency
	/// </summary>
	public class OHLCdataGrid : QuotesDataGrid
	{
    private DataTable openHighLowCloseDataTable;
		public OHLCdataGrid()
		{
		}
    public void DataBind( ValidateDataTable validateDataTable )
    {
      this.openHighLowCloseDataTable = validateDataTable.Clone();
      string select = "1=1";
      DataRow[] dataRows = validateDataTable.Select(
        select );
      DataView dataView = new DataView( validateDataTable );
      dataView.RowFilter = "ValidationWarning=" +
        Convert.ToInt16( ValidationWarning.OpenHighLowCloseLogicalInconsistency );
      this.DataSource = dataView;
      //      foreach ( DataRow dataRow in dataRows )
      //        this.openHighLowCloseDataTable.Rows.Add( dataRow );
    }
	}
}
