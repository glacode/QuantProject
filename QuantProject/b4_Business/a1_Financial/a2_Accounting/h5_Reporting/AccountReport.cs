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
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.Transactions;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Financial.Accounting.Reporting
{
	/// <summary>
	/// Summary description for AccountReport.
	/// </summary>
	[Serializable]
	public class AccountReport
	{
    private Account account;
		private IHistoricalQuoteProvider historicalQuoteProvider;
    private Account accountCopy = new Account( "AccountCopy" );
    private string reportName;
    private EndOfDayDateTime endDateTime;
    private string buyAndHoldTicker;
    //private long numDaysForInterval;
    private DataTable detailedDataTable;
    private Tables.Transactions transactionTable;
    private ReportTable roundTrades;
    private ReportTable equity;
    private Tables.Summary summary;

    public string Name
    {
      get { return reportName; }
    }    
    public EndOfDayDateTime EndDateTime
    {
      get { return endDateTime; }
    }    
		public string BuyAndHoldTicker
		{
			get { return buyAndHoldTicker; }
		}
		public Account Account
		{
			get { return this.account; }
		}
		//    public long NumDaysForInterval
//    {
//      get { return numDaysForInterval; }
//    }
    public DataTable DetailedDataTable
    {
      get { return detailedDataTable; }
    }
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
    public Tables.Summary Summary
    {
      get { return summary; }
    }
    public DateTime StartDateTime
    {
      get
      {
        return (DateTime) account.Transactions.GetKey( 0 );
      }
    }

    /// <summary>
    /// Add the last account record to the AccountReport
    /// </summary>
    /// <param name="account"></param>
    public AccountReport( Account account ,
			IHistoricalQuoteProvider historicalQuoteProvider )
    {
      this.account = account;
			this.historicalQuoteProvider = historicalQuoteProvider;
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
    private void addBalanceItems ( EndOfDayDateTime endOfDayDateTime ,  DataRow dataRow )
    {
      dataRow[ "AccountCash" ] = this.accountCopy.CashAmount;
      dataRow[ "PortfolioValue" ] = this.accountCopy.Portfolio.GetMarketValue(
        endOfDayDateTime , this.historicalQuoteProvider );
      dataRow[ "AccountValue" ] = (double)dataRow[ "AccountCash" ] +
				(double)dataRow[ "PortfolioValue" ];
      dataRow[ "PnL" ] = (double)dataRow[ "AccountValue" ] +
        this.accountCopy.Transactions.TotalWithdrawn -
        this.accountCopy.Transactions.TotalAddedCash;
    }
    private void addTransactionRow( EndOfDayTransaction transaction ,
      System.Data.DataTable detailedDataTable )
    {
      DataRow dataRow = detailedDataTable.NewRow();
      dataRow[ "DateTime" ] = transaction.EndOfDayDateTime.DateTime;
      dataRow[ "BarComponent" ] = transaction.EndOfDayDateTime.EndOfDaySpecificTime.ToString();
      dataRow[ "TransactionType" ] = transaction.Type.ToString();
      if ( transaction.Instrument != null )
        dataRow[ "InstrumentKey" ] = transaction.Instrument.Key;
      else
        dataRow[ "InstrumentKey" ] = "";
      dataRow[ "Quantity" ] = transaction.Quantity;
      dataRow[ "Price" ] = transaction.InstrumentPrice;
      dataRow[ "TransactionAmount" ] = transaction.InstrumentPrice * transaction.Quantity;
      addBalanceItems( transaction.EndOfDayDateTime , dataRow );
      detailedDataTable.Rows.Add( dataRow );
    }
    private void addRowsForTransactions( DateTime currentDateTime ,
      System.Data.DataTable detailedDataTable )
    {
      if ( this.account.Transactions.ContainsKey( currentDateTime ) )
        foreach ( EndOfDayTransaction transaction in
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
      addBalanceItems( new EndOfDayDateTime( currentDate , EndOfDaySpecificTime.MarketClose ) , dataRow );
      detailedDataTable.Rows.Add( dataRow );
    }
    private void addRowForPnl( long numDaysForInterval , DateTime currentDate ,
      System.Data.DataTable detailedDataTable )
    {
      if ( ( Convert.ToInt32(((TimeSpan)(currentDate.Date -
        (DateTime) account.Transactions.GetKey( 0 ))).TotalDays )
        % numDaysForInterval ) == 0 )
        addRowForPnl_actually( currentDate , detailedDataTable );
    }
    private void setRows(  long numDaysForInterval , System.Data.DataTable detailedDataTable )
    {
      DateTime currentDate = (DateTime) account.Transactions.GetKey( 0 );
      try
      {
        while ( currentDate < this.endDateTime.DateTime )
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
    private System.Data.DataTable getDetailedDataTable( long numDaysForInterval )
    {
      System.Data.DataTable detailedDataTable = new System.Data.DataTable();
      setColumns( detailedDataTable );
      setRows( numDaysForInterval , detailedDataTable );
      return detailedDataTable;
    }
    #endregion
    #endregion

    public AccountReport Create( string reportName , long numDaysForInterval ,
      EndOfDayDateTime endDateTime , string buyAndHoldTicker )
    {
      this.reportName = reportName;
      this.endDateTime = endDateTime;
      this.buyAndHoldTicker = buyAndHoldTicker;
      detailedDataTable = getDetailedDataTable( numDaysForInterval );
      this.transactionTable = new Tables.Transactions( reportName , detailedDataTable );
//      this.transactionTable = getTransactionTable( reportName , detailedDataTable );
      this.roundTrades = new Tables.RoundTrades( reportName , this.transactionTable );
      this.equity = new Tables.Equity( reportName , detailedDataTable );
      //this.equity = getEquity( reportName , detailedDataTable );
      //this.summary = getSummary( reportName );
      this.summary = new Tables.Summary( this , historicalQuoteProvider );
      return this;
    }

    public AccountReport Create( string reportName , long numDaysForInterval ,
      EndOfDayDateTime endDateTime )
    {
      return Create( reportName , numDaysForInterval , endDateTime , "" );
    }
	}
}
