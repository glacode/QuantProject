using System;
using System.Data;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary description for TotalNumberOfTrades.
	/// </summary>
	public class ReturnOnAccount : SummaryRow
	{
		public ReturnOnAccount( Summary summary )
		{
      summary.ReturnOnAccount = summary.TotalPnl / ( summary.FinalAccountValue - summary.TotalPnl ) * 100;
      this.rowDescription = "Return on account";
      this.rowValue = summary.ReturnOnAccount;
    }
	}
}
