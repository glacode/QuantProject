using System;
using System.Data;
using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Accounting.Reporting.SummaryRows;

namespace QuantProject.Business.Financial.Accounting.Reporting.Tables
{
	/// <summary>
	/// Summary description for Summary.
	/// </summary>
  [Serializable]
  public class Summary : ReportTable
  {
    private AccountReport accountReport;
		private IHistoricalQuoteProvider historicalQuoteProvider;
    private double totalPnl;
    private double buyAndHoldPercentageReturn;
    private double finalAccountValue;
    private long intervalDays;
		private MaxEquityDrawDown maxEquityDrawDown;
		private TotalNumberOfTrades totalNumberOfTrades;
		private NumberWinningTrades numberWinningTrades;
		private AverageTradePercentageReturn averageTradePercentageReturn;
		private LargestWinningTradePercentage largestWinningTradePercentage;
		private LargestLosingTradePercentage largestLosingTradePercentage;
		private NumberWinningLongTrades numberWinningLongTrades;
		private AverageLongTradePercentageReturn averageLongTradePercentageReturn;
		private NumberWinningShortTrades numberWinningShortTrades;
		private TotalNumberOfLongTrades totalNumberOfLongTrades;
		private TotalNumberOfShortTrades totalNumberOfShortTrades;
		private TotalCommissionAmount totalCommissionAmount;
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
		public double MaxEquityDrawDown
		{
			get { return Convert.ToDouble( this.maxEquityDrawDown.Value ); }
		}
		public double TotalNumberOfTrades
		{
			get { return (int)this.totalNumberOfTrades.Value; }
		}
		public int NumberWinningTrades
		{
			get { return (int)this.numberWinningTrades.Value; }
		}
		public double AverageTradePercentageReturn
		{
			get { return Convert.ToDouble( this.averageTradePercentageReturn.Value ); }
		}
		public double LargestWinningTradePercentage
		{
			get { return Convert.ToDouble( this.largestWinningTradePercentage.Value ); }
		}
		public double LargestLosingTradePercentage
		{
			get { return Convert.ToDouble( this.largestLosingTradePercentage.Value ); }
		}
		public double NumberWinningLongTrades
		{
			get { return (int)this.numberWinningLongTrades.Value; }
		}
		public double AverageLongTradePercentageReturn
		{
			get { return Convert.ToDouble( this.averageLongTradePercentageReturn.Value ); }
		}
		public double NumberWinningShortTrades
		{
			get { return (int)this.numberWinningShortTrades.Value; }
		}
		public double TotalNumberOfLongTrades
		{
			get { return (int)this.totalNumberOfLongTrades.Value; }
		}
		public double TotalNumberOfShortTrades
		{
			get { return (int)this.totalNumberOfShortTrades.Value; }
		}

		public double TotalCommissionAmount
		{
			get { return Convert.ToDouble( this.totalCommissionAmount.Value ); }
		}


		private void summary( AccountReport accountReport )
		{
			this.accountReport = accountReport;
			this.getSummary();
		}
		public Summary( AccountReport accountReport ) :
			base( accountReport.Name + " - Summary" )
		{
			this.summary( accountReport );
		}
		public Summary( AccountReport accountReport ,
			IHistoricalQuoteProvider historicalDataProvider ) :
			base( accountReport.Name + " - Summary" )
		{
			this.historicalQuoteProvider = historicalDataProvider;
			this.summary( accountReport );
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
      getSummary_setRow( new BuyAndHoldPercentageReturn( this , this.historicalQuoteProvider ) ,
				summaryDataTable );
      getSummary_setRow( new AnnualSystemPercentageReturn( this ) , summaryDataTable );
			this.maxEquityDrawDown = new MaxEquityDrawDown( this );
      getSummary_setRow( this.maxEquityDrawDown , summaryDataTable );
			this.totalNumberOfTrades = new TotalNumberOfTrades( this );
      getSummary_setRow( this.totalNumberOfTrades , summaryDataTable );
			this.numberWinningTrades = new NumberWinningTrades( this );
      getSummary_setRow( this.numberWinningTrades , summaryDataTable );
			this.averageTradePercentageReturn = new AverageTradePercentageReturn( this );
      getSummary_setRow( this.averageTradePercentageReturn , summaryDataTable );
			this.largestWinningTradePercentage = new LargestWinningTradePercentage( this );
      getSummary_setRow( this.largestWinningTradePercentage , summaryDataTable );
			this.largestLosingTradePercentage = new LargestLosingTradePercentage( this );
      getSummary_setRow( this.largestLosingTradePercentage , summaryDataTable );
			this.totalNumberOfLongTrades = new TotalNumberOfLongTrades( this );
      getSummary_setRow( this.totalNumberOfLongTrades , summaryDataTable );
			this.numberWinningLongTrades = new NumberWinningLongTrades( this );
      getSummary_setRow( this.numberWinningLongTrades , summaryDataTable );
			this.averageLongTradePercentageReturn = new AverageLongTradePercentageReturn( this );
      getSummary_setRow( this.averageLongTradePercentageReturn , summaryDataTable );
			this.totalNumberOfShortTrades = new TotalNumberOfShortTrades( this );
      getSummary_setRow( this.totalNumberOfShortTrades , summaryDataTable );
			this.numberWinningShortTrades = new NumberWinningShortTrades( this );
			getSummary_setRow( this.numberWinningShortTrades , summaryDataTable );
			this.totalCommissionAmount = new TotalCommissionAmount( this );
			getSummary_setRow( this.totalCommissionAmount , summaryDataTable );
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
			if ( this.accountReport.Equity.DataTable.Rows.Count == 0 )
				throw new Exception( "A Summary computation has been requested, but the equity line is empty" );
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
