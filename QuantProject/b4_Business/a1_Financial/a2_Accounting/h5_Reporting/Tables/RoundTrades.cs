using System;
using System.Data;

using QuantProject.ADT;
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
    private string fieldNameForTicker;
		private string fieldNameForDateTime;
		private string fieldNameForTransactionType;
		
		private void setFieldNames()
		{
			this.fieldNameForTicker = 
				QuantProject.Business.Financial.Accounting.Reporting.Tables.Transactions.FieldNameForTicker;
			this.fieldNameForDateTime = 
				QuantProject.Business.Financial.Accounting.Reporting.Tables.Transactions.FieldNameForDateTime;
			this.fieldNameForTransactionType = 
				QuantProject.Business.Financial.Accounting.Reporting.Tables.Transactions.FieldNameForTransactionType;
		}
    public RoundTrades( string reportName , ReportTable transactionTable ) :
      base( reportName + " - Round Trades" )
		{
      this.transactionTable = transactionTable;
      this.setFieldNames();
      this.DataTable = this.getRoundTrades();
    }
    #region "getRoundTrades"
    private void getRoundTradeTable_setColumns( DataTable roundTradeDataTable )
    {
      roundTradeDataTable.Columns.Add( fieldNameForTicker , Type.GetType( "System.String" ) );
      roundTradeDataTable.Columns.Add( "Trade" , Type.GetType( "System.String" ) );
      roundTradeDataTable.Columns.Add( "Quantity"  , Type.GetType( "System.Int32" ) );
      roundTradeDataTable.Columns.Add( "EntryDate"  , Type.GetType( "System.DateTime" ) );
      roundTradeDataTable.Columns.Add( "EntryPrice" , Type.GetType( "System.Double" ) );
      roundTradeDataTable.Columns.Add( "ExitDate"  , Type.GetType( "System.DateTime" ) );
      roundTradeDataTable.Columns.Add( "ExitPrice"  , Type.GetType( "System.Double" ) );
      roundTradeDataTable.Columns.Add( "%chg"  , Type.GetType( "System.Double" ) );
      roundTradeDataTable.Columns.Add( "Profit"  , Type.GetType( "System.Double" ) );
      roundTradeDataTable.Columns.Add( "%Profit"  , Type.GetType( "System.Double" ) );
      roundTradeDataTable.Columns.Add( "#minutes"  , Type.GetType( "System.Int32" ) );
      roundTradeDataTable.Columns.Add( "ProfitPerMinute"  , Type.GetType( "System.Double" ) );
      //roundTradeDataTable.Columns.Add( "AccountValue"  , Type.GetType( "System.Double" ) );
      //roundTradeDataTable.Columns.Add( "PnL"  , Type.GetType( "System.Double" ) );
    }
    private void getRoundTradeTable_addRow( int rowIndex, DataRow dataRow , DataTable roundTradeDataTable )
    {
			DateTime startDateTimeOfTrade = (DateTime)dataRow[fieldNameForDateTime];
			DateTime endDateTimeOfTrade = new DateTime(1900,1,1);
			string currentTicker = (string)dataRow[fieldNameForTicker];
			string currentTradeType = (string)dataRow[fieldNameForTransactionType];
			double signForRoundTradeProfit = 1.0;
    	if(currentTradeType == "SellShort")
    		signForRoundTradeProfit = -1.0;
			double entryPrice = (double)dataRow["Price"];
			double exitPrice = 0.0;
			int instrumentQuantity = (int)dataRow["Quantity"];
			for (int j = rowIndex + 1; j < this.transactionTable.DataTable.Rows.Count ; j++)
			{
				if( currentTicker == (string)this.transactionTable.DataTable.Rows[ j ][fieldNameForTicker] &&
				    ((currentTradeType == "BuyLong" && 
				     (string)this.transactionTable.DataTable.Rows[ j ][fieldNameForTransactionType] == "Sell")
				     ||
				     (currentTradeType == "SellShort" && 
				     (string)this.transactionTable.DataTable.Rows[ j ][fieldNameForTransactionType] == "Cover")
				    ) )
				{
					endDateTimeOfTrade = 
						(DateTime)this.transactionTable.DataTable.Rows[ j ][fieldNameForDateTime];
					exitPrice = (double)this.transactionTable.DataTable.Rows[ j ]["Price"];
					j = this.transactionTable.DataTable.Rows.Count;
				}
			}
			TimeSpan timeSpanBetweenEnterAndExit = 
				endDateTimeOfTrade.Subtract(startDateTimeOfTrade);
      DataRow roundTrade = roundTradeDataTable.NewRow();
      roundTrade[ fieldNameForTicker ] = currentTicker;
      if ( currentTradeType == "BuyLong" )
      	roundTrade[ "Trade" ] = "Long";
      else
      	roundTrade[ "Trade" ] = "Short";
      roundTrade[ "Quantity" ] = instrumentQuantity;
      roundTrade[ "EntryDate" ] = startDateTimeOfTrade;
      roundTrade[ "ExitDate" ] = endDateTimeOfTrade;
      roundTrade[ "EntryPrice" ] = entryPrice;
      roundTrade[ "ExitPrice" ] = exitPrice;
      roundTrade[ "%chg" ] = (exitPrice - entryPrice)/entryPrice * 100.0;
      roundTrade[ "%Profit" ] = signForRoundTradeProfit * (double)roundTrade[ "%chg" ];
      roundTrade[ "Profit" ] = signForRoundTradeProfit * instrumentQuantity * (exitPrice - entryPrice);
      roundTrade[ "#minutes" ] = timeSpanBetweenEnterAndExit.TotalMinutes;
      roundTrade[ "ProfitPerMinute" ] = (double)roundTrade[ "%Profit" ] / (int)roundTrade[ "#minutes" ];
      roundTradeDataTable.Rows.Add(roundTrade);
    }
    private bool doesCurrentRowIndexPointToABuyLongOrASellShort( int rowIndex )
		{
			bool returnValue;
			string currentTransactionType =
				(string)this.transactionTable.DataTable.Rows[ rowIndex ][fieldNameForTransactionType];
			returnValue =
				( currentTransactionType == "BuyLong" || currentTransactionType == "SellShort" );
			return returnValue;
		}
    private void getRoundTradeTable_setRows( DataTable roundTradeDataTable )
    {
      for(int rowIndex = 0; rowIndex < this.transactionTable.DataTable.Rows.Count; rowIndex++ )
      	if( this.doesCurrentRowIndexPointToABuyLongOrASellShort( rowIndex ) )
					getRoundTradeTable_addRow( rowIndex,
      	                           	 transactionTable.DataTable.Rows[rowIndex],
      	                             roundTradeDataTable );
    }
    private DataTable getRoundTrades()
    {
      DataTable roundTradeDataTable = new DataTable();
      getRoundTradeTable_setColumns( roundTradeDataTable );
      getRoundTradeTable_setRows( roundTradeDataTable );
      DataTable roundTradeDataTable_ordered =	
      	ExtendedDataTable.CopyAndSort(roundTradeDataTable, "ExitPrice > 0",
      	                              "EntryDate", true);
      return roundTradeDataTable_ordered;
    }
    #endregion
	}
}
