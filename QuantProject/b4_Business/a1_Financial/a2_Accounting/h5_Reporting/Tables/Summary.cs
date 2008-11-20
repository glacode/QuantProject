using System;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;

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
	public class Summary : ReportTable, ISerializable
	{
		private AccountReport accountReport;
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private double totalPnl;
		private BenchmarkPercentageReturn benchmarkPercentageReturn;
		private AnnualSystemPercentageReturn annualSystemPercentageReturn;
		private double finalAccountValue;
		private long intervalDays;
		private TotalNetProfit totalNetProfit;
		private ReturnOnAccount returnOnAccount;
		private MaxEquityDrawDown maxEquityDrawDown;
		private SharpeRatio sharpeRatio;
		private ExpectancyScore expectancyScore;
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
		private NumberPositivePeriods numberPositivePeriods;
		private NumberNegativePeriods numberNegativePeriods;
		private PercentagePositivePeriods percentagePositivePeriods;
		private NumberWinningPeriods numberWinningPeriods;
		private NumberLosingPeriods numberLosingPeriods;
		private PercentageWinningPeriods percentageWinningPeriods;
		private AverageNumberOfTransactionsPerDay averageNumberOfTransactionsPerDay;

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
		public SharpeRatio SharpeRatio
		{
			get { return this.sharpeRatio; }
		}
		public ExpectancyScore ExpectancyScore
		{
			get { return this.expectancyScore; }
		}
		public TotalCommissionAmount TotalCommissionAmount
		{
			get { return this.totalCommissionAmount; }
		}
		public NumberPositivePeriods NumberPositivePeriods
		{
			get { return this.numberPositivePeriods; }
		}
		public NumberNegativePeriods NumberNegativePeriods
		{
			get { return this.numberNegativePeriods; }
		}
		public PercentagePositivePeriods PercentagePositivePeriods
		{
			get
			{
				return this.percentagePositivePeriods;
			}
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
		public AverageNumberOfTransactionsPerDay AverageNumberOfTransactionsPerDay
		{
			get { return this.averageNumberOfTransactionsPerDay; }
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
		               HistoricalMarketValueProvider historicalMarketValueProvider ) :
			base( accountReport.Name + " - Summary" )
		{
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.summary( accountReport );
		}

		/// <summary>
		/// This constructor allows custom deserialization (see the ISerializable
		/// interface documentation)
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected Summary( SerializationInfo info , StreamingContext context ) :
			base( "Summary" )
		{
			// get the set of serializable members for this class and its base classes
			Type thisType = this.GetType();
			MemberInfo[] mi = FormatterServices.GetSerializableMembers(
				thisType , context);

			// deserialize the fields from the info object
			for (Int32 i = 0 ; i < mi.Length; i++)
			{
				FieldInfo fieldInfo = (FieldInfo) mi[i];

				// set the field to the deserialized value
				try
				{
					fieldInfo.SetValue( this ,
					                   info.GetValue( fieldInfo.Name, fieldInfo.FieldType ) );
				}
				catch (Exception ex)
				{
					string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
				}
			}
		}

		#region GetObjectData
		/// <summary>
		/// serialize the set of serializable members for this class and base classes
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(
			SerializationInfo info, StreamingContext context)
		{
			// get the set of serializable members for this class and base classes
			Type thisType = this.GetType();
			MemberInfo[] mi =
				FormatterServices.GetSerializableMembers( thisType , context);

			// serialize the fields to the info object
			for (Int32 i = 0 ; i < mi.Length; i++)
			{
				info.AddValue(mi[i].Name, ((FieldInfo) mi[i]).GetValue(this));
			}
		}
		#endregion

		#region "getSummary"
		private void getSummaryTable_setColumns( DataTable equityDataTable )
		{
			equityDataTable.Columns.Add( "Information"  , Type.GetType( "System.String" ) );
			equityDataTable.Columns.Add( "Value" , Type.GetType( "System.Double" ) );
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
			this.returnOnAccount = new ReturnOnAccount( this );
			this.benchmarkPercentageReturn =
				new BenchmarkPercentageReturn( this );
//				                              , this.historicalMarketValueProvider );
			this.numberWinningPeriods = new NumberWinningPeriods( this );
			this.numberLosingPeriods = new NumberLosingPeriods( this );
			this.percentageWinningPeriods = new PercentageWinningPeriods( this );
			this.numberPositivePeriods = new NumberPositivePeriods( this );
			this.numberNegativePeriods = new NumberNegativePeriods( this );
			this.percentagePositivePeriods = new PercentagePositivePeriods( this );
			this.totalNetProfit = new TotalNetProfit( this );
			this.annualSystemPercentageReturn = new AnnualSystemPercentageReturn( this );
			this.maxEquityDrawDown = new MaxEquityDrawDown( this );
			this.sharpeRatio = new SharpeRatio( this.accountReport.EquityLine );
			this.expectancyScore = new ExpectancyScore( this.accountReport.EquityLine );
			this.totalNumberOfTrades = new TotalNumberOfTrades( this );
			this.numberWinningTrades = new NumberWinningTrades( this );
			this.averageTradePercentageReturn = new AverageTradePercentageReturn( this );
			this.largestWinningTradePercentage = new LargestWinningTradePercentage( this );
			this.largestLosingTradePercentage = new LargestLosingTradePercentage( this );
			this.totalNumberOfLongTrades = new TotalNumberOfLongTrades( this );
			this.numberWinningLongTrades = new NumberWinningLongTrades( this );
			this.averageLongTradePercentageReturn = new AverageLongTradePercentageReturn( this );
			this.totalNumberOfShortTrades = new TotalNumberOfShortTrades( this );
			this.numberWinningShortTrades = new NumberWinningShortTrades( this );
			this.totalCommissionAmount = new TotalCommissionAmount( this );
			this.averageNumberOfTransactionsPerDay = new AverageNumberOfTransactionsPerDay(this);
			//this.DataTable = getSummaryDataTable();
		}

		#endregion

	}
}
