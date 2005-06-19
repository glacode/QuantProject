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
  public class AverageShortTradePercentageReturn : PercentageSummaryRow
	{
		public AverageShortTradePercentageReturn( Summary summary )
		{
      this.rowDescription = "Average short trade % Return";
			double avgReturn = 0.0;
			try
			{
				avgReturn =
					(double) summary.AccountReport.RoundTrades.DataTable.Compute(
					"avg([%Profit])" , "(Trade='Short')" );
			}
			catch (Exception ex)
			{
				ex = ex; // to avoid compilation warning;
			}
      this.rowValue = avgReturn;
    }
	}
}
