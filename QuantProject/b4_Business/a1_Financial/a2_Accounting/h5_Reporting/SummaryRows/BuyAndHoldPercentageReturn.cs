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
	/// Summary description for TotalNumberOfTrades.
	/// </summary>
	public class BuyAndHoldPercentageReturn : SummaryRow
	{
		public BuyAndHoldPercentageReturn( Summary summary ,
			IHistoricalQuoteProvider historicalQuoteProvider )
		{
      if ( summary.AccountReport.BuyAndHoldTicker != "" )
      {
        // the report has to compare to a buy and hold benchmark
				double beginningMarketValue = historicalQuoteProvider.GetMarketValue(
					summary.AccountReport.BuyAndHoldTicker ,
					new EndOfDayDateTime( summary.AccountReport.StartDateTime , EndOfDaySpecificTime.MarketOpen ) );
				double finalMarketValue = historicalQuoteProvider.GetMarketValue(
					summary.AccountReport.BuyAndHoldTicker ,
					summary.AccountReport.EndDateTime );
				summary.BuyAndHoldPercentageReturn = ( finalMarketValue - beginningMarketValue ) /
					beginningMarketValue * 100;
        this.rowDescription = "Buy & hold % return";
        this.rowValue = summary.BuyAndHoldPercentageReturn;
      }
    }
	}
}
