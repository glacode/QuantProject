using System;
using System.Data;
using QuantProject.ADT;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Accounting.Reporting.SummaryRows;

namespace QuantProject.Business.Financial.Accounting.Reporting.Tables
{
	/// <summary>
	/// Summary description for Summary.
	/// </summary>
  public class Summary : ReportTable
  {
    private AccountReport accountReport;
    private double totalPnl;
    private double buyAndHoldPercentageReturn;
    private double finalAccountValue;
    private long intervalDays;
    public AccountReport AccountReport
    {
      get { return accountReport; }
    }
    public double TotalPnl
    {
      get { return totalPnl; }
    }
    public double BuyAndHoldPercentageReturn
    {
      get { return buyAndHoldPercentageReturn; }
      set { buyAndHoldPercentageReturn = value; }
    }
    public double FinalAccountValue
    {
      get { return finalAccountValue; }
    }
    public double IntervalDays
    {
      get { return intervalDays; }
    }
    public double ReturnOnAccount;
    public double AnnualSystemPercentageReturn;
    public Summary( AccountReport accountReport ) :
      base( accountReport.Name + " - Summary" )
    {
      this.accountReport = accountReport;
      this.getSummary();
    }
    #region "getSummary"
    private void getSummaryTable_setColumns( DataTable equityDataTable )
    {
      equityDataTable.Columns.Add( "Information"  , Type.GetType( "System.String" ) );
      equityDataTable.Columns.Add( "Value" , Type.GetType( "System.Double" ) );
    }
    #region "getSummaryTable_setRows"
    private void getSummary_setRow( SummaryRow summaryRow , DataTable summaryDataTable )
    {
      if ( summaryRow.Value != null )
      {
        DataRow summary = summaryDataTable.NewRow();
        summary[ "Information" ] = summaryRow.Description;
        summary[ "Value" ] = summaryRow.Value;
        summaryDataTable.Rows.Add( summary );
      }
    }
    private void getSummaryTable_setRows( DataTable summaryDataTable )
    {
      getSummary_setRow( new TotalNetProfit( this ) , summaryDataTable );
      getSummary_setRow( new ReturnOnAccount( this ) , summaryDataTable );
      getSummary_setRow( new BuyAndHoldPercentageReturn( this ) , summaryDataTable );
      getSummary_setRow( new AnnualSystemPercentageReturn( this ) , summaryDataTable );
      getSummary_setRow( new MaxEquityDrawDown( this ) , summaryDataTable );
      getSummary_setRow( new TotalNumberOfTrades( this ) , summaryDataTable );
      getSummary_setRow( new NumberWinningTrades( this ) , summaryDataTable );
      getSummary_setRow( new AverageTradePercentageReturn( this ) , summaryDataTable );
      getSummary_setRow( new LargestWinningTradePercentage( this ) , summaryDataTable );
      getSummary_setRow( new LargestLosingTradePercentage( this ) , summaryDataTable );
      getSummary_setRow( new TotalNumberOfLongTrades( this ) , summaryDataTable );
      getSummary_setRow( new NumberWinningLongTrades( this ) , summaryDataTable );
      getSummary_setRow( new AverageLongTradePercentageReturn( this ) , summaryDataTable );
      getSummary_setRow( new TotalNumberOfShortTrades( this ) , summaryDataTable );
      getSummary_setRow( new NumberWinningShortTrades( this ) , summaryDataTable );
      //      getSummary_setRow( summaryDataTable ,
//        new getSummaryTable_setRow( getSummaryTable_setRow_TotalNumberOfShortTrades ) );
//      getSummary_setRow( summaryDataTable ,
//        new getSummaryTable_setRow( getSummaryTable_setRow_NumberWinningShortTrades ) );
//      getSummary_setRow( summaryDataTable ,
//        new getSummaryTable_setRow( getSummaryTable_setRow_AverageShortTradePercentageReturn ) );
    }
    #endregion
    private DataTable getSummaryDataTable()
    {
      DataTable summaryDataTable = new DataTable();
      getSummaryTable_setColumns( summaryDataTable );
      getSummaryTable_setRows( summaryDataTable );
      return summaryDataTable;
    }
    private void getSummary()
    {
      this.totalPnl =
        (double)this.accountReport.Equity.DataTable.Rows[ this.accountReport.Equity.DataTable.Rows.Count - 1 ][ "PnL" ];
      this.finalAccountValue =
        (double)this.accountReport.Equity.DataTable.Rows[ this.accountReport.Equity.DataTable.Rows.Count - 1 ][ "AccountValue" ];
      this.intervalDays =
        ((TimeSpan)((DateTime)this.accountReport.Equity.DataTable.Rows[ this.accountReport.Equity.DataTable.Rows.Count - 1 ][ "Date" ] -
        (DateTime)this.accountReport.Equity.DataTable.Rows[ 0 ][ "Date" ])).Days;
      this.DataTable = getSummaryDataTable();
    }

    #endregion

	}
}
