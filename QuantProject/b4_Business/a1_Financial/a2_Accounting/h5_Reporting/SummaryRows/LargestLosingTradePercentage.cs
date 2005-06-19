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
  public class LargestLosingTradePercentage : PercentageSummaryRow
	{
		public LargestLosingTradePercentage( Summary summary )
		{
      this.rowDescription = "Largest losing trade";
			try
			{
				this.rowValue =
					(double) summary.AccountReport.RoundTrades.DataTable.Compute( "min([%Profit])" , "([%Profit]<0)" );
			}
			catch (Exception ex)
			{
				ex = ex; // to avoid compilation warning;
			}
    }
	}
}
