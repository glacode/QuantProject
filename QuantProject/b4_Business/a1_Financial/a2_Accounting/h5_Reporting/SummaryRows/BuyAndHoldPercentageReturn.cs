using System;
using System.Data;
using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;
using QuantProject.Business.Financial.Instruments;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary description for TotalNumberOfTrades.
	/// </summary>
	public class BuyAndHoldPercentageReturn : SummaryRow
	{
		public BuyAndHoldPercentageReturn( Summary summary )
		{
      if ( summary.AccountReport.BuyAndHoldTicker != "" )
      {
        // the report has to compare to a buy and hold benchmark
        Instrument buyAndHoldInstrument = new Instrument( summary.AccountReport.BuyAndHoldTicker );
        summary.BuyAndHoldPercentageReturn =
          ( buyAndHoldInstrument.GetMarketValue( summary.AccountReport.EndDateTime ) -
          buyAndHoldInstrument.GetMarketValue(
          new ExtendedDateTime( summary.AccountReport.StartDateTime , BarComponent.Open ) ) ) /
          buyAndHoldInstrument.GetMarketValue(
          new ExtendedDateTime( summary.AccountReport.StartDateTime , BarComponent.Open ) ) * 100;
        this.rowDescription = "Buy & hold % return";
        this.rowValue = summary.BuyAndHoldPercentageReturn;
      }
    }
	}
}
