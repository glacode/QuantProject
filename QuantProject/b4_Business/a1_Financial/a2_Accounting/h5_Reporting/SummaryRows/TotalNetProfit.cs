using System;
using System.Data;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary description for TotalNumberOfTrades.
	/// </summary>
	[Serializable]
  public class TotalNetProfit : SummaryRow
	{
		public TotalNetProfit( Summary summary )
		{
      this.rowDescription = "Total net profit";
      this.rowValue = summary.TotalPnl;
    }
	}
}
