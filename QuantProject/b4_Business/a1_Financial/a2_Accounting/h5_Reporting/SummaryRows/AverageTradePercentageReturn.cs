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
	public class AverageTradePercentageReturn : SummaryRow
	{
		public AverageTradePercentageReturn( Summary summary )
		{
      this.rowDescription = "Average trade % Return";
      double avgReturn = (double) summary.AccountReport.RoundTrades.DataTable.Compute( "avg([%Profit])" , "true" );
      this.rowValue = avgReturn;
    }
	}
}
