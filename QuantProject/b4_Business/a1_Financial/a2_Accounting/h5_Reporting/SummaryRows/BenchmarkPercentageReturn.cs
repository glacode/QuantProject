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
  public class BenchmarkPercentageReturn : SummaryRow
	{
		public BenchmarkPercentageReturn( Summary summary ,
			IHistoricalQuoteProvider historicalQuoteProvider )
		{
      if ( summary.AccountReport.Benchmark != "" )
      {
        // the report has to compare to a buy and hold benchmark
				double beginningMarketValue = historicalQuoteProvider.GetMarketValue(
					summary.AccountReport.Benchmark ,
					new EndOfDayDateTime( summary.AccountReport.StartDateTime , EndOfDaySpecificTime.MarketOpen ) );
				double finalMarketValue = historicalQuoteProvider.GetMarketValue(
					summary.AccountReport.Benchmark ,
					summary.AccountReport.EndDateTime );
				summary.BenchmarkPercentageReturn = ( finalMarketValue - beginningMarketValue ) /
					beginningMarketValue * 100;
        this.rowDescription = "Buy & hold % return";
        this.rowValue = summary.BenchmarkPercentageReturn;
      }
    }
	}
}
