using System;
using System.Data;
using QuantProject.Business.Financial.Accounting.Reporting;

namespace QuantProject.Business.Financial.Accounting.Reporting.Tables
{
	/// <summary>
	/// Summary description for RoundTrades.
	/// </summary>
	[Serializable]
  public class RoundTrades : ReportTable
	{
    private ReportTable transactionTable;

    public RoundTrades( string reportName , ReportTable transactionTable ) :
      base( reportName + " - Round Trades" )
		{
      this.transactionTable = transactionTable;
      this.DataTable = this.getRoundTrades();
    }
    #region "getRoundTrades"
    private void getRoundTradeTable_setColumns( DataTable roundTradeDataTable )
    {
      roundTradeDataTable.Columns.Add( "Trade" , Type.GetType( "System.String" ) );
      roundTradeDataTable.Columns.Add( "EntryDate"  , Type.GetType( "System.DateTime" ) );
      roundTradeDataTable.Columns.Add( "EntryPrice" , Type.GetType( "System.Double" ) );
      roundTradeDataTable.Columns.Add( "ExitDate"  , Type.GetType( "System.DateTime" ) );
      roundTradeDataTable.Columns.Add( "ExitPrice"  , Type.GetType( "System.Double" ) );
      roundTradeDataTable.Columns.Add( "%chg"  , Type.GetType( "System.Double" ) );
      roundTradeDataTable.Columns.Add( "Profit"  , Type.GetType( "System.Double" ) );
      roundTradeDataTable.Columns.Add( "%Profit"  , Type.GetType( "System.Double" ) );
      roundTradeDataTable.Columns.Add( "#bars"  , Type.GetType( "System.Int32" ) );
      roundTradeDataTable.Columns.Add( "ProfitPerBar"  , Type.GetType( "System.Double" ) );
      //roundTradeDataTable.Columns.Add( "AccountValue"  , Type.GetType( "System.Double" ) );
      //roundTradeDataTable.Columns.Add( "PnL"  , Type.GetType( "System.Double" ) );
    }
    private void getRoundTradeTable_setRow( DataRow dataRow , DataTable roundTradeDataTable )
    {
      DataRow roundTrade;
      switch ( (string)dataRow[ "TransactionType" ] )
      {
        case "BuyLong":
          roundTrade = roundTradeDataTable.NewRow();
          roundTrade[ "Trade" ] = "Long";
          roundTrade[ "EntryDate" ] = dataRow[ "DateTime" ];
          roundTrade[ "EntryPrice" ] = dataRow[ "Price" ];
          roundTradeDataTable.Rows.Add( roundTrade );
          break;
        case "Sell":
          roundTrade = roundTradeDataTable.Rows[ roundTradeDataTable.Rows.Count - 1 ];
          roundTrade[ "ExitDate" ] = dataRow[ "DateTime" ];
          roundTrade[ "ExitPrice" ] = dataRow[ "Price" ];
          roundTrade[ "%chg" ] =
            ((double)roundTrade[ "ExitPrice" ] - (double)roundTrade[ "EntryPrice" ])/
            ((double)roundTrade[ "EntryPrice" ])*100;
          roundTrade[ "%Profit" ] = roundTrade[ "%chg" ];
          roundTrade[ "#bars" ] =
            ((TimeSpan)((DateTime)roundTrade[ "ExitDate" ] - (DateTime)roundTrade[ "EntryDate" ])).Days;
          roundTrade[ "ProfitPerBar" ] = (double)roundTrade[ "%chg" ] / (int)roundTrade[ "#bars" ];
          break;
        case "SellShort":
          roundTrade = roundTradeDataTable.NewRow();
          roundTrade[ "Trade" ] = "Short";
          roundTrade[ "EntryDate" ] = dataRow[ "DateTime" ];
          roundTrade[ "EntryPrice" ] = dataRow[ "Price" ];
          roundTradeDataTable.Rows.Add( roundTrade );
          break;
        case "Cover":
          roundTrade = roundTradeDataTable.Rows[ roundTradeDataTable.Rows.Count - 1 ];
          roundTrade[ "ExitDate" ] = dataRow[ "DateTime" ];
          roundTrade[ "ExitPrice" ] = dataRow[ "Price" ];
          roundTrade[ "%chg" ] =
            ((double)roundTrade[ "ExitPrice" ] - (double)roundTrade[ "EntryPrice" ])/
            ((double)roundTrade[ "EntryPrice" ])*100;
          roundTrade[ "%Profit" ] = - ((double)roundTrade[ "%chg" ]);
          roundTrade[ "#bars" ] =
            ((TimeSpan)((DateTime)roundTrade[ "ExitDate" ] - (DateTime)roundTrade[ "EntryDate" ])).Days;
          roundTrade[ "ProfitPerBar" ] = (double)roundTrade[ "%chg" ] / (int)roundTrade[ "#bars" ];
          break;
      }

    }
    private void getRoundTradeTable_setRows( DataTable roundTradeDataTable )
    {
      foreach (DataRow dataRow in this.transactionTable.DataTable.Rows )
        getRoundTradeTable_setRow( dataRow , roundTradeDataTable );
    }
    private DataTable getRoundTrades()
    {
      DataTable roundTradeDataTable = new DataTable();
      getRoundTradeTable_setColumns( roundTradeDataTable );
      getRoundTradeTable_setRows( roundTradeDataTable );
      return roundTradeDataTable;
    }
    #endregion

	}
}
