using System;
using System.Data;
using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;
using QuantProject.Business.Financial.Instruments;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Account report Summary Row, containing the total
	/// commission amount.
	/// </summary>
	[Serializable]
  public class TotalCommissionAmount : SummaryRow
	{
		public TotalCommissionAmount( Summary summary )
		{
      this.rowDescription = "Total commission amount";
      double avgReturn =
        (double) summary.AccountReport.TransactionTable.DataTable.Compute( "sum([Commission])" , "true" );
      this.rowValue = avgReturn;
    }
	}
}
