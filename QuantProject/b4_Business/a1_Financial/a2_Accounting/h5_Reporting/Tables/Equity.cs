using System;
using System.Data;
using QuantProject.Business.Financial.Accounting.Reporting;

namespace QuantProject.Business.Financial.Accounting.Reporting.Tables
{
	/// <summary>
	/// Summary description for Equity.
	/// </summary>
	public class Equity : ReportTable
	{
		public static string Date = "Date";
		public static string PnL = "PnL";
		public static string AccountValue = "AccountValue";
		public static string PercentageChange = "%chg";

    public Equity( string reportName , DataTable detailedDataTable ) :
      base( reportName + " - Equity" )
		{
      this.DataTable = this.getEquity( detailedDataTable );
    }
    #region "getEquity"
    private void getEquityTable_setColumns( DataTable equityDataTable )
    {
      equityDataTable.Columns.Add( "Date"  , Type.GetType( "System.DateTime" ) );
      equityDataTable.Columns.Add( "PnL" , Type.GetType( "System.Double" ) );
      equityDataTable.Columns.Add( "AccountValue"  , Type.GetType( "System.Double" ) );
      equityDataTable.Columns.Add( "%chg"  , Type.GetType( "System.Double" ) );
    }
    private void getEquityTable_setRows( DataTable equityDataTable , DataTable detailedDataTable )
    {
      foreach (DataRow detailedRow in detailedDataTable.Rows )
        if ( detailedRow[ "TransactionType" ] == System.DBNull.Value )
          // current detailed row reports an equity row
        {
          DataRow dataRow = equityDataTable.NewRow();
          dataRow[ "Date" ] = detailedRow[ "DateTime" ];
          dataRow[ "PnL" ] = detailedRow[ "PnL" ];
          dataRow[ "AccountValue" ] = detailedRow[ "AccountValue" ];
          if ( equityDataTable.Rows.Count > 0 )
            dataRow[ "%chg" ] = ((double)dataRow[ "AccountValue" ] -
              (double)equityDataTable.Rows[ equityDataTable.Rows.Count - 1 ][ "AccountValue" ])/
              (double)equityDataTable.Rows[ equityDataTable.Rows.Count - 1 ][ "AccountValue" ]*
              100;
          equityDataTable.Rows.Add( dataRow );
        }
    }
    private DataTable getEquity( DataTable detailedDataTable )
    {
      DataTable equityDataTable = new DataTable();
      getEquityTable_setColumns( equityDataTable );
      getEquityTable_setRows( equityDataTable , detailedDataTable );
      return equityDataTable;
    }
    #endregion
	}
}
