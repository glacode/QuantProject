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
    public double FinalAccountValue
    {
      get { return finalAccountValue; }
    }
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
    private void getSummaryTable_setRow_BuyAndHoldPercentageReturn( DataRow summary )
    {
      if ( this.accountReport.BuyAndHoldTicker != "" )
      {
        // the report has to compare to a buy and hold benchmark
        Instrument buyAndHoldInstrument = new Instrument( this.accountReport.BuyAndHoldTicker );
        this.buyAndHoldPercentageReturn =
          ( buyAndHoldInstrument.GetMarketValue( this.accountReport.EndDateTime ) -
          buyAndHoldInstrument.GetMarketValue(
          new ExtendedDateTime( this.accountReport.StartDateTime , BarComponent.Open ) ) ) /
          buyAndHoldInstrument.GetMarketValue(
          new ExtendedDateTime( this.accountReport.StartDateTime , BarComponent.Open ) ) * 100;
        summary[ "Information" ] = "Buy & hold % return";
        summary[ "Value" ] = this.buyAndHoldPercentageReturn;
      }
    }
    private void getSummaryTable_setRow_AnnualSystemPercentageReturn( DataRow summary )
    {
      double totalROA = this.totalPnl / ( this.finalAccountValue - this.totalPnl );
      summary[ "Information" ] = "Annual system % return";
      summary[ "Value" ] = ( ( Math.Pow( 1 + totalROA ,
        1.0 / ( (double)this.intervalDays/365.0 ) ) ) - 1 ) * 100;
      //        r = [(1+T)^(1/n)]-1
    }
    private void getSummaryTable_setRow_NumberWinningTrades( DataRow summary )
    {
      summary[ "Information" ] = "Number winning trades";
      DataRow[] DataRows = this.accountReport.RoundTrades.DataTable.Select( "([%Profit] > 0)" );
      summary[ "Value" ] = DataRows.Length;
    }
    private void getSummaryTable_setRow_AverageTradePercentageReturn( DataRow summary )
    {
      summary[ "Information" ] = "Average trade % Return";
      double avgReturn = (double) this.accountReport.RoundTrades.DataTable.Compute( "avg([%Profit])" , "true" );
      summary[ "Value" ] = avgReturn;
    }
    private void getSummaryTable_setRow_LargestWinningTradePercentage( DataRow summary )
    {
      summary[ "Information" ] = "Largest winning trade";
      summary[ "Value" ] =
        (double) this.accountReport.RoundTrades.DataTable.Compute( "max([%Profit])" , "([%Profit]>0)" );
    }
    private void getSummaryTable_setRow_LargestLosingTradePercentage( DataRow summary )
    {
      summary[ "Information" ] = "Largest losing trade";
      summary[ "Value" ] =
        (double) this.accountReport.RoundTrades.DataTable.Compute( "min([%Profit])" , "([%Profit]<0)" );
    }
    private void getSummaryTable_setRow_TotalNumberOfLongTrades( DataRow summary )
    {
      double totalROA = this.totalPnl / ( this.finalAccountValue - this.totalPnl );
      summary[ "Information" ] = "Total # of long trades";
      DataRow[] DataRows =
        this.accountReport.RoundTrades.DataTable.Select( "((Trade='Long')and(ExitPrice is not null))" );
      summary[ "Value" ] = DataRows.Length;
    }
    private void getSummaryTable_setRow_NumberWinningLongTrades( DataRow summary )
    {
      summary[ "Information" ] = "Number winning long trades";
      DataRow[] DataRows = this.accountReport.RoundTrades.DataTable.Select( "((Trade='Long')and([%Profit] > 0))" );
      summary[ "Value" ] = DataRows.Length;
    }
    private void getSummaryTable_setRow_AverageLongTradePercentageReturn( DataRow summary )
    {
      summary[ "Information" ] = "Average long trade % Return";
      double avgReturn =
        (double) this.accountReport.RoundTrades.DataTable.Compute( "avg([%Profit])" , "(Trade='Long')" );
      summary[ "Value" ] = avgReturn;
    }
    private void getSummaryTable_setRow_TotalNumberOfShortTrades( DataRow summary )
    {
      double totalROA = this.totalPnl / ( this.finalAccountValue - this.totalPnl );
      summary[ "Information" ] = "Total # of short trades";
      DataRow[] DataRows =
        this.accountReport.RoundTrades.DataTable.Select( "((Trade='Short')and(ExitPrice is not null))" );
      summary[ "Value" ] = DataRows.Length;
    }
    private void getSummaryTable_setRow_NumberWinningShortTrades( DataRow summary )
    {
      summary[ "Information" ] = "Number winning short trades";
      DataRow[] DataRows = this.accountReport.RoundTrades.DataTable.Select( "((Trade='Short')and([%Profit] > 0))" );
      summary[ "Value" ] = DataRows.Length;
    }
    private void getSummaryTable_setRow_AverageShortTradePercentageReturn( DataRow summary )
    {
      summary[ "Information" ] = "Average short trade % Return";
      double avgReturn =
        (double) this.accountReport.RoundTrades.DataTable.Compute( "avg([%Profit])" , "(Trade='Short')" );
      summary[ "Value" ] = avgReturn;
    }
    private void getSummary_setRow( DataTable summaryDataTable ,
      getSummaryTable_setRow getSummaryTable_setRow_object )
    {
      DataRow summary = summaryDataTable.NewRow();
      getSummaryTable_setRow_object( summary );
      summaryDataTable.Rows.Add( summary );
    }
    private void getSummary_setRow( SummaryRow summaryRow , DataTable summaryDataTable )
    {
      DataRow summary = summaryDataTable.NewRow();
      summary[ "Information" ] = summaryRow.Description;
      summary[ "Value" ] = summaryRow.Value;
      summaryDataTable.Rows.Add( summary );
    }
    private void getSummaryTable_setRows( DataTable summaryDataTable )
    {
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_TotalNetProfit ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_ReturnOnAccount ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_BuyAndHoldPercentageReturn ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_AnnualSystemPercentageReturn ) );
      getSummary_setRow( new MaxEquityDrawDown( this ) , summaryDataTable );
      getSummary_setRow( new TotalNumberOfTrades( this ) , summaryDataTable );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_NumberWinningTrades ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_AverageTradePercentageReturn ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_LargestWinningTradePercentage ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_LargestLosingTradePercentage ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_TotalNumberOfLongTrades ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_NumberWinningLongTrades ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_AverageLongTradePercentageReturn ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_TotalNumberOfShortTrades ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_NumberWinningShortTrades ) );
      getSummary_setRow( summaryDataTable ,
        new getSummaryTable_setRow( getSummaryTable_setRow_AverageShortTradePercentageReturn ) );
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
