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
using QuantProject.Business.Financial.Instruments;
namespace QuantProject.Business.Financial.Accounting.Reporting
{
	/// <summary>
	/// Summary description for AccountReport.
	/// </summary>
	public class AccountReport
	{
    private Account account;
    private Account accountCopy = new Account( "AccountCopy" );
    private string reportName;
    private ExtendedDateTime endDateTime;
    private string buyAndHoldTicker;
    //private long numDaysForInterval;
    private DataTable detailedDataTable;
    private Tables.Transactions transactionTable;
    private ReportTable roundTrades;
    private ReportTable equity;
    private ReportTable summary;

    public string Name
    {
      get { return reportName; }
    }    
    public ExtendedDateTime EndDateTime
    {
      get { return endDateTime; }
    }    
    public string BuyAndHoldTicker
    {
      get { return buyAndHoldTicker; }
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
    public ReportTable Summary
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
      ExtendedDateTime endDateTime , string buyAndHoldTicker )
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
      this.summary = new Tables.Summary( this );
      return this;
    }

    public AccountReport Create( string reportName , long numDaysForInterval ,
      ExtendedDateTime endDateTime )
    {
      return Create( reportName , numDaysForInterval , endDateTime , "" );
    }
	}
}
