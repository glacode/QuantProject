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
	public class LargestWinningTradePercentage : SummaryRow
	{
		public LargestWinningTradePercentage( Summary summary )
		{
      this.rowDescription = "Largest winning trade";
      this.rowValue =
        (double) summary.AccountReport.RoundTrades.DataTable.Compute( "max([%Profit])" , "([%Profit]>0)" );
    }
	}
}
