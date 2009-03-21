using System;
using System.Data;
using QuantProject.Business.Financial.Accounting.Reporting;

namespace QuantProject.Business.Financial.Accounting.Reporting.Tables
{
	/// <summary>
	/// Summary description for Transactions.
	/// </summary>
	[Serializable]
  public class Transactions : ReportTable
	{
  	public static string FieldNameForDateTime
  	{
  		get{ return "DateTime"; }
  	}
  	public static string FieldNameForTicker
  	{
  		get{ return "InstrumentKey"; }
  	}
  	public static string FieldNameForTransactionType
  	{
  		get{ return "TransactionType"; }
  	}
		public Transactions( string reportName , DataTable detailedDataTable ) :
      base( reportName + " - Transactions" )
		{
      this.DataTable = this.getTransactionTable_actually( detailedDataTable );
		}
    private void getTransactionTable_setColumns( DataTable detailedDataTable ,
      DataTable transactionsDataTable )
    {
      transactionsDataTable.Columns.Add(
  			Transactions.FieldNameForDateTime , Type.GetType( "System.DateTime" ) );
//      transactionsDataTable.Columns.Add( "BarComponent" , Type.GetType( "System.String" ) );
		transactionsDataTable.Columns.Add( "TransactionType"  , Type.GetType( "System.String" ) );
      transactionsDataTable.Columns.Add(
	Transactions.FieldNameForTicker  , Type.GetType( "System.String" ) );
//      transactionsDataTable.Columns.Add( "InstrumentKey"  , Type.GetType( "System.String" ) );
      transactionsDataTable.Columns.Add( "Quantity"  , Type.GetType( "System.Int32" ) );
      transactionsDataTable.Columns.Add( "Price"  , Type.GetType( "System.Double" ) );
			transactionsDataTable.Columns.Add( "TransactionAmount"  , Type.GetType( "System.Double" ) );
			transactionsDataTable.Columns.Add( "Commission"  , Type.GetType( "System.Double" ) );
			transactionsDataTable.Columns.Add( "AccountCash"  , Type.GetType( "System.Double" ) );
      transactionsDataTable.Columns.Add( "PortfolioValue"  , Type.GetType( "System.Double" ) );
      transactionsDataTable.Columns.Add( "AccountValue"  , Type.GetType( "System.Double" ) );
      transactionsDataTable.Columns.Add( "PnL"  , Type.GetType( "System.Double" ) );
    }
    private void getTransactionTable_setRows(  DataTable detailedDataTable ,
      DataTable transactionsDataTable )
    {
      foreach ( DataRow detailedRow in detailedDataTable.Rows )
        if ( detailedRow[ "TransactionType" ] != System.DBNull.Value )
          // current detailed row reports a transaction
        {
          DataRow dataRow = transactionsDataTable.NewRow();
          dataRow[ "DateTime" ] = detailedRow[ "DateTime" ];
//          dataRow[ "BarComponent" ] = detailedRow[ "BarComponent" ];
          dataRow[ "TransactionType" ] = detailedRow[ "TransactionType" ];
          dataRow[ "InstrumentKey" ] = detailedRow[ "InstrumentKey" ];
          dataRow[ "Quantity" ] = detailedRow[ "Quantity" ];
          dataRow[ "Price" ] = detailedRow[ "Price" ];
					dataRow[ "TransactionAmount" ] = detailedRow[ "TransactionAmount" ];
					dataRow[ "Commission" ] = detailedRow[ "Commission" ];
					dataRow[ "AccountCash" ] = detailedRow[ "AccountCash" ];
          dataRow[ "PortfolioValue" ] = detailedRow[ "PortfolioValue" ];
          dataRow[ "AccountValue" ] = detailedRow[ "AccountValue" ];
          dataRow[ "PnL" ] = detailedRow[ "PnL" ];
          transactionsDataTable.Rows.Add( dataRow );
        }
    }
    private DataTable getTransactionTable_actually( DataTable detailedDataTable )
    {
      DataTable transactionsDataTable = new DataTable();
      getTransactionTable_setColumns( detailedDataTable , transactionsDataTable );
      getTransactionTable_setRows( detailedDataTable , transactionsDataTable );
      return transactionsDataTable;
    }
//    private ReportTable getTransactionTable( string reportName , DataTable detailedDataTable )
//    {
//      DataTable transactionsDataTable = getTransactionTable_actually( detailedDataTable );
//      return new ReportTable( reportName + " - Transactions" ,
//        transactionsDataTable );
//      //ExcelManager.Add( reportTable ); daCanc
//    }

	}
}
