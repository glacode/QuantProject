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
  public class AverageLongTradePercentageReturn : SummaryRow
	{
		public AverageLongTradePercentageReturn( Summary summary )
		{
      this.rowDescription = "Average long trade % Return";
      double avgReturn =
        (double) summary.AccountReport.RoundTrades.DataTable.Compute( "avg([%Profit])" , "(Trade='Long')" );
      this.rowValue = avgReturn;
    }
	}
}
