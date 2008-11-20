using System;
using System.Data;
using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Percentage return for the Benchmark
	/// </summary>
	[Serializable]
	public class BenchmarkPercentageReturn : PercentageSummaryRow
	{
		public BenchmarkPercentageReturn( Summary summary )
//		                                 HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			HistoricalAdjustedQuoteProvider historicalMarketValueProvider =
				new HistoricalAdjustedQuoteProvider();
			if ( summary.AccountReport.Benchmark != "" )
			{
				// the report has to compare to a buy and hold benchmark
				double beginningMarketValue = historicalMarketValueProvider.GetMarketValue(
					summary.AccountReport.Benchmark ,
					HistoricalEndOfDayTimer.GetMarketOpen(
						summary.AccountReport.StartDateTime ) );
//					new EndOfDayDateTime( summary.AccountReport.StartDateTime , EndOfDaySpecificTime.MarketOpen ) );
				double finalMarketValue = historicalMarketValueProvider.GetMarketValue(
					summary.AccountReport.Benchmark ,
					HistoricalEndOfDayTimer.GetMarketClose(
						summary.AccountReport.EndDateTime ) );
//				summary.BenchmarkPercentageReturn = ( finalMarketValue - beginningMarketValue ) /
//					beginningMarketValue * 100;
				this.rowDescription = "Buy & hold % return";
				this.rowValue = ( finalMarketValue - beginningMarketValue ) /
					beginningMarketValue * 100;
			}
		}
	}
}
