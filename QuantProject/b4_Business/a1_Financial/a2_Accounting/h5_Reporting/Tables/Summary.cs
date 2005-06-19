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
    private BenchmarkPercentageReturn benchmarkPercentageReturn;
		private AnnualSystemPercentageReturn annualSystemPercentageReturn;
		private double finalAccountValue;
    private long intervalDays;
		private TotalNetProfit totalNetProfit;
		private ReturnOnAccount returnOnAccount;
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
		private NumberWinningPeriods numberWinningPeriods;
		private NumberLosingPeriods numberLosingPeriods;
		private PercentageWinningPeriods percentageWinningPeriods;
		public AccountReport AccountReport
    {
      get { return accountReport; }
    }
    public double TotalPnl
    {
      get { return totalPnl; }
    }
		public TotalNetProfit TotalNetProfit
		{
			get { return this.totalNetProfit; }
		}
		public ReturnOnAccount ReturnOnAccount
		{
			get { return this.returnOnAccount; }
		}
		public BenchmarkPercentageReturn BenchmarkPercentageReturn
    {
      get { return this.benchmarkPercentageReturn; }
    }
    public double FinalAccountValue
    {
      get { return finalAccountValue; }
    }
    public double IntervalDays
    {
      get { return intervalDays; }
    }
		public AnnualSystemPercentageReturn AnnualSystemPercentageReturn
		{
			get { return this.annualSystemPercentageReturn; }
		}
		public MaxEquityDrawDown MaxEquityDrawDown
		{
			get { return this.maxEquityDrawDown; }
		}
		public TotalCommissionAmount TotalCommissionAmount
		{
			get { return this.totalCommissionAmount; }
		}
		public NumberWinningPeriods NumberWinningPeriods
		{
			get { return this.numberWinningPeriods; }
		}
		public NumberLosingPeriods NumberLosingPeriods
		{
			get { return this.numberLosingPeriods; }
		}
		public int NumberEvenPeriods;
		public PercentageWinningPeriods PercentageWinningPeriods
		{
			get
			{
				return this.percentageWinningPeriods;
			}
		}
		public TotalNumberOfTrades TotalNumberOfTrades
		{
			get { return this.totalNumberOfTrades; }
		}
		public NumberWinningTrades NumberWinningTrades
		{
			get { return this.numberWinningTrades; }
		}
		public AverageTradePercentageReturn AverageTradePercentageReturn
		{
			get { return this.averageTradePercentageReturn; }
		}
		public LargestWinningTradePercentage LargestWinningTradePercentage
		{
			get { return this.largestWinningTradePercentage; }
		}
		public LargestLosingTradePercentage LargestLosingTradePercentage
		{
			get { return this.largestLosingTradePercentage; }
		}
		public TotalNumberOfLongTrades TotalNumberOfLongTrades
		{
			get { return this.totalNumberOfLongTrades; }
		}
		public NumberWinningLongTrades NumberWinningLongTrades
		{
			get { return this.numberWinningLongTrades; }
		}
		public AverageLongTradePercentageReturn AverageLongTradePercentageReturn
		{
			get { return this.averageLongTradePercentageReturn; }
		}
		public TotalNumberOfShortTrades TotalNumberOfShortTrades
		{
			get { return this.totalNumberOfShortTrades; }
		}
		public NumberWinningShortTrades NumberWinningShortTrades
		{
			get { return this.numberWinningShortTrades; }
		}



		private void summary( AccountReport accountReport )
		{
			this.accountReport = accountReport;
			this.numberWinningPeriods =	new NumberWinningPeriods( this );
			this.numberLosingPeriods = new NumberLosingPeriods( this );
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
			this.returnOnAccount = new ReturnOnAccount( this ); 
      getSummary_setRow( this.returnOnAccount , summaryDataTable );
			this.benchmarkPercentageReturn =
				new BenchmarkPercentageReturn( this , this.historicalQuoteProvider );
			getSummary_setRow( this.benchmarkPercentageReturn ,
				summaryDataTable );
			this.numberWinningPeriods = new NumberWinningPeriods( this );
			getSummary_setRow( this.numberWinningPeriods ,	summaryDataTable );
			this.numberLosingPeriods = new NumberLosingPeriods( this );
			getSummary_setRow( this.numberLosingPeriods ,	summaryDataTable );
			getSummary_setRow( new NumberEvenPeriods( this ) ,	summaryDataTable );
			this.percentageWinningPeriods = new PercentageWinningPeriods( this );
			getSummary_setRow( this.percentageWinningPeriods ,	summaryDataTable );
			//this.getSummary_setRows_forEquityVsBenchmarkComparison();
			this.totalNetProfit = new TotalNetProfit( this );
			this.annualSystemPercentageReturn = new AnnualSystemPercentageReturn( this );
			getSummary_setRow( this.annualSystemPercentageReturn , summaryDataTable );
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
