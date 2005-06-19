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
	[Serializable]
  public class AverageTradePercentageReturn : PercentageSummaryRow
	{
		public AverageTradePercentageReturn( Summary summary )
		{
      this.rowDescription = "Average trade % Return";
			try
			{
				this.rowValue = (double) summary.AccountReport.RoundTrades.DataTable.Compute( "avg([%Profit])" , "true" );
			}
			catch (Exception ex)
			{
				ex = ex; // to avoid compilation warning;
			}
    }
	}
}
