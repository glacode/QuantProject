using System;
using System.Data;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary description for TotalNumberOfTrades.
	/// </summary>
	[Serializable]
  public class TotalNetProfit : DoubleSummaryRow
	{
		public TotalNetProfit( Summary summary ) : base( 2 )
		{
      this.rowDescription = "Total net profit";
      this.rowValue = summary.TotalPnl;
    }
	}
}
