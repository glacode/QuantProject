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
  public class LargestWinningTradePercentage : SummaryRow
	{
		public LargestWinningTradePercentage( Summary summary )
		{
      this.rowDescription = "Largest winning trade";
			try
			{
				this.rowValue =
					(double) summary.AccountReport.RoundTrades.DataTable.Compute( "max([%Profit])" , "([%Profit]>0)" );
			}
			catch (Exception ex)
			{
				ex = ex; // to avoid compilation warning;
			}
    }
	}
}
