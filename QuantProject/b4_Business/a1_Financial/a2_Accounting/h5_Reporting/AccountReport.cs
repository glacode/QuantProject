/*
QuantProject - Quantitative Finance Library

AccountReport.cs
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
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using QuantProject.ADT;
using QuantProject.ADT.Histories;

namespace QuantProject.Business.Financial.Accounting.Reporting
{
	/// <summary>
	/// Summary description for AccountReport.
	/// </summary>
	public class AccountReport
	{
    private Account account;
    private Account accountCopy = new Account( "AccountCopy" );
    private double totalPnl;
    private double finalAccountValue;
    private long intervalDays;
    private ReportTable transactionTable;
    private ReportTable roundTrades;
    private ReportTable equity;
    private ReportTable summary;

    public ReportTable TransactionTable
    {
      get { return transactionTable; }
    }
    public ReportTable RoundTrades
    {
      get { return roundTrades; }
    }
    public ReportTable Equity
    {
      get { return equity; }
    }
    public ReportTable Summary
    {
      get { return summary; }
    }

    /// <summary>
    /// Add the last account record to the AccountReport
    /// </summary>
    /// <param name="account"></param>
    public AccountReport( Account account )
    {
      this.account = account;
    }

    #region "Create"

    #region "getDetailedDataTable"
    private void setColumns( System.Data.DataTable transactions )
    {
      transactions.Columns.Add( "DateTime" , Type.GetType( "System.DateTime" ) );
      transactions.Columns.Add( "BarComponent" , Type.GetType( "System.String" ) );
      transactions.Columns.Add( "TransactionType"  , Type.GetType( "System.String" ) );
      transactions.Columns.Add( "InstrumentKey"  , Type.GetType( "System.String" ) );
      transactions.Columns.Add( "Quantity"  , Type.GetType( "System.Int32" ) );
      transactions.Columns.Add( "Price"  , Type.GetType( "System.Double" ) );
      transactions.Columns.Add( "TransactionAmount"  , Type.GetType( "System.Double" ) );
      transactions.Columns.Add( "AccountCash"  , Type.GetType( "System.Double" ) );
      transactions.Columns.Add( "PortfolioValue"  , Type.GetType( "System.Double" ) );
      transactions.Columns.Add( "AccountValue"  , Type.GetType( "System.Double" ) );
      transactions.Columns.Add( "PnL"  , Type.GetType( "System.Double" ) );
    }
    #region "setRows"
    private void addBalanceItems ( ExtendedDateTime extendedDateTime ,  DataRow dataRow )
    {
      dataRow[ "AccountCash" ] = this.accountCopy.CashAmount;
      dataRow[ "PortfolioValue" ] = this.accountCopy.Portfolio.GetMarketValue(
        extendedDateTime );
      dataRow[ "AccountValue" ] = this.accountCopy.GetMarketValue( extendedDateTime );
      dataRow[ "PnL" ] = this.accountCopy.GetMarketValue( extendedDateTime ) +
        this.accountCopy.Transactions.TotalWithdrawn -
        this.accountCopy.Transactions.TotalAddedCash;
    }
    private void addTransactionRow( TimedTransaction transaction ,
      System.Data.DataTable detailedDataTable )
    {
      DataRow dataRow = detailedDataTable.NewRow();
      dataRow[ "DateTime" ] = transaction.ExtendedDateTime.DateTime;
      dataRow[ "BarComponent" ] = transaction.ExtendedDateTime.BarComponent.ToString();
      dataRow[ "TransactionType" ] = transaction.Type.ToString();
      if ( transaction.Instrument != null )
        dataRow[ "InstrumentKey" ] = transaction.Instrument.Key;
      else
        dataRow[ "InstrumentKey" ] = "";
      dataRow[ "Quantity" ] = transaction.Quantity;
      dataRow[ "Price" ] = transaction.InstrumentPrice;
      dataRow[ "TransactionAmount" ] = transaction.InstrumentPrice * transaction.Quantity;
      addBalanceItems( transaction.ExtendedDateTime , dataRow );
      detailedDataTable.Rows.Add( dataRow );
    }
    private void addRowsForTransactions( DateTime currentDateTime ,
      System.Data.DataTable detailedDataTable )
    {
      if ( this.account.Transactions.ContainsKey( currentDateTime ) )
        foreach ( TimedTransaction transaction in
          (ArrayList)this.account.Transactions[ currentDateTime ] )
        {
          this.accountCopy.Add( transaction );
          addTransactionRow( transaction , detailedDataTable );
        }
    }
    private void addRowForPnl_actually( DateTime currentDate ,
      System.Data.DataTable detailedDataTable )
    {
      DataRow dataRow = detailedDataTable.NewRow();
      dataRow[ "DateTime" ] = currentDate;
      addBalanceItems( new ExtendedDateTime( currentDate , BarComponent.Close ) , dataRow );
      detailedDataTable.Rows.Add( dataRow );
    }
    private void addRowForPnl( int numDaysForInterval , DateTime currentDate ,
      System.Data.DataTable detailedDataTable )
    {
      if ( ( Convert.ToInt32(((TimeSpan)(currentDate.Date -
        (DateTime) account.Transactions.GetKey( 0 ))).TotalDays )
        % numDaysForInterval ) == 0 )
        addRowForPnl_actually( currentDate , detailedDataTable );
    }
    private void setRows(  int numDaysForInterval ,
      ExtendedDateTime endDateTime, System.Data.DataTable detailedDataTable )
    {
      DateTime currentDate = (DateTime) account.Transactions.GetKey( 0 );
      try
      {
        while ( currentDate < endDateTime.DateTime )
        {
          //addTransactionsToAccountCopy( currentDate );
          addRowsForTransactions( currentDate , detailedDataTable );
          addRowForPnl( numDaysForInterval , currentDate , detailedDataTable );
          currentDate = currentDate.AddDays( 1 );
        }
//        foreach ( ArrayList transactionList in account.Transactions.Values )
//          foreach ( TimedTransaction transaction in transactionList )
//            addRow( transaction , transactions );
      }
      catch (Exception ex)
      {
        MessageBox.Show( ex.ToString() );
      }
    }
    #endregion
    private System.Data.DataTable getDetailedDataTable( int numDaysForInterval ,
      ExtendedDateTime endDateTime )
    {
      System.Data.DataTable detailedDataTable = new System.Data.DataTable();
      setColumns( detailedDataTable );
      setRows( numDaysForInterval , endDateTime , detailedDataTable );
      return detailedDataTable;
    }
    #endregion
    #region "getTransactionTable"
    private void getTransactionTable_setColumns( DataTable detailedDataTable ,
      DataTable transactionsDataTable )
    {
      transactionsDataTable.Columns.Add( "DateTime" , Type.GetType( "System.DateTime" ) );
      transactionsDataTable.Columns.Add( "BarComponent" , Type.GetType( "System.String" ) );
      transactionsDataTable.Columns.Add( "TransactionType"  , Type.GetType( "System.String" ) );
      transactionsDataTable.Columns.Add( "InstrumentKey"  , Type.GetType( "System.String" ) );
      transactionsDataTable.Columns.Add( "Quantity"  , Type.GetType( "System.Int32" ) );
      transactionsDataTable.Columns.Add( "Price"  , Type.GetType( "System.Double" ) );
      transactionsDataTable.Columns.Add( "TransactionAmount"  , Type.GetType( "System.Double" ) );
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
          dataRow[ "BarComponent" ] = detailedRow[ "BarComponent" ];
          dataRow[ "TransactionType" ] = detailedRow[ "TransactionType" ];
          dataRow[ "InstrumentKey" ] = detailedRow[ "InstrumentKey" ];
          dataRow[ "Quantity" ] = detailedRow[ "Quantity" ];
          dataRow[ "Price" ] = detailedRow[ "Price" ];
          dataRow[ "TransactionAmount" ] = detailedRow[ "TransactionAmount" ];
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
    private ReportTable getTransactionTable( string reportName , DataTable detailedDataTable )
    {
      DataTable transactionsDataTable = getTransactionTable_actually( detailedDataTable );
      return new ReportTable( reportName + " - Transactions" ,
        transactionsDataTable );
      //ExcelManager.Add( reportTable ); daCanc
    }
    #endregion
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
            ((double)roundTrade[ "ExitPrice" ] - (double)roundTrade[ "EntryPrice" ])/100;
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
    private DataTable getRoundTradeDataTable()
    {
      DataTable roundTradeDataTable = new DataTable();
      getRoundTradeTable_setColumns( roundTradeDataTable );
      getRoundTradeTable_setRows( roundTradeDataTable );
      return roundTradeDataTable;
    }
    public ReportTable getRoundTrades( string reportName )
    {
      DataTable roundTradeDataTable = getRoundTradeDataTable();
      return new ReportTable( reportName + " - Round Trades" ,
        roundTradeDataTable );
    }
    #endregion
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
    private DataTable getEquityDataTable( DataTable detailedDataTable )
    {
      DataTable equityDataTable = new DataTable();
      getEquityTable_setColumns( equityDataTable );
      getEquityTable_setRows( equityDataTable , detailedDataTable );
      return equityDataTable;
    }
    public ReportTable getEquity( string reportName , DataTable detailedDataTable )
    {
      DataTable equityDataTable = getEquityDataTable( detailedDataTable );
      return new ReportTable( reportName + " - Equity" ,
        equityDataTable );
    }

    #endregion
    #region "getSummary"
    private void getSummaryTable_setColumns( DataTable equityDataTable )
    {
      equityDataTable.Columns.Add( "Information"  , Type.GetType( "System.String" ) );
      equityDataTable.Columns.Add( "Value" , Type.GetType( "System.Double" ) );
    }
    #region "getSummaryTable_setRows"
    private delegate void getSummaryTable_setRow( DataRow summary );
    private void getSummaryTable_setRow_TotalNetProfit( DataRow summary )
    {
      summary[ "Information" ] = "Total net profit";
      summary[ "Value" ] = this.totalPnl;
    }
    private void getSummaryTable_setRow_ReturnOnAccount( DataRow summary )
    {
      summary[ "Information" ] = "Return on account";
      summary[ "Value" ] = this.totalPnl / ( this.finalAccountValue - this.totalPnl ) * 100;
    }
    private void getSummaryTable_setRow_AnnualSystemPercentageReturn( DataRow summary )
    {
      double totalROA = this.totalPnl / ( this.finalAccountValue - this.totalPnl );
      summary[ "Information" ] = "Annual system % return";
      summary[ "Value" ] = ( Math.Pow( 1 + totalROA ,
        1.0 / ( (double)this.intervalDays/365.0 ) ) ) - 1;
//        r = [(1+T)^(1/n)]-1
    }
    private void getSummary_setRow( DataTable summaryDataTable ,
      getSummaryTable_setRow getSummaryTable_setRow_object )
    {
      DataRow summary = summaryDataTable.NewRow();
      getSummaryTable_setRow_object( summary );
      summaryDataTable.Rows.Add( summary );
    }
    private void getSummaryTable_setRows( DataTable summaryDataTable )
    {
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_TotalNetProfit ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_ReturnOnAccount ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_AnnualSystemPercentageReturn ) );
    }
    #endregion
    private DataTable getSummaryDataTable()
    {
      DataTable summaryDataTable = new DataTable();
      getSummaryTable_setColumns( summaryDataTable );
      getSummaryTable_setRows( summaryDataTable );
      return summaryDataTable;
    }
    public ReportTable getSummary( string reportName )
    {
      this.totalPnl =
        (double)this.equity.DataTable.Rows[ this.equity.DataTable.Rows.Count - 1 ][ "PnL" ];
      this.finalAccountValue =
        (double)this.equity.DataTable.Rows[ this.equity.DataTable.Rows.Count - 1 ][ "AccountValue" ];
      this.intervalDays =
        ((TimeSpan)((DateTime)this.equity.DataTable.Rows[ this.equity.DataTable.Rows.Count - 1 ][ "Date" ] -
        (DateTime)this.equity.DataTable.Rows[ 0 ][ "Date" ])).Days;
      DataTable equityDataTable = getSummaryDataTable();
      return new ReportTable( reportName + " - Summary" ,
        equityDataTable );
    }

    #endregion
    public AccountReport Create( string reportName , int numDaysForInterval ,
      ExtendedDateTime endDateTime )
    {
      DataTable detailedDataTable = getDetailedDataTable( numDaysForInterval , endDateTime );
      this.transactionTable = getTransactionTable( reportName , detailedDataTable );
      this.roundTrades = getRoundTrades( reportName );
      this.equity = getEquity( reportName , detailedDataTable );
      this.summary = getSummary( reportName );
      return this;
    }
    #endregion


	}
}
