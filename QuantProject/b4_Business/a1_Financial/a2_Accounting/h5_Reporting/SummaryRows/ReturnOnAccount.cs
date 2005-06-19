using System;
using System.Data;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary description for TotalNumberOfTrades.
	/// </summary>
	[Serializable]
  public class ReturnOnAccount : PercentageSummaryRow
	{
		public ReturnOnAccount( Summary summary )
		{
      this.rowDescription = "Return on account";
      this.rowValue = summary.TotalPnl / ( summary.FinalAccountValue - summary.TotalPnl ) * 100;
    }
	}
}
