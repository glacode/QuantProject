using System;
using System.Windows.Forms;
using System.Data;
using QuantProject.Business.Validation;

namespace QuantProject.Applications.Downloader.Validate
{
	/// <summary>
	/// Summary description for ValidateDataGrid.
	/// </summary>
  public class ValidateDataGrid : QuotesDataGrid
  {
    private ValidateDataTable validateDataTable;

    public ValidateDataGrid()
    {
    }
    #region "Validate"
    public ValidateDataTable Validate( string tickerIsLike , string suspiciousRatio )
    {
      this.validateDataTable = new ValidateDataTable();
      this.DataSource = validateDataTable;
      validateDataTable.AddRows( tickerIsLike );
      return this.validateDataTable;
    }
    public ValidateDataTable Validate(DataTable dataTable, string suspiciousRatio )
    {
      return this.Validate((string)dataTable.Rows[0][0], suspiciousRatio);
      //this.validateDataTable = new ValidateDataTable(dataTable);
      //this.DataSource = validateDataTable;
//      if ( this.TableStyles.Count == 0 )
//        // styles have not been defined yet
//        validate_setTableStyle();
      //validateDataTable.AddRows( "pippo" );
      //return this.validateDataTable;
    }
	
    #endregion
  }
}
