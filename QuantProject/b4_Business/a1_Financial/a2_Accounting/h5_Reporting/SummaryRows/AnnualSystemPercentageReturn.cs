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
	public class AnnualSystemPercentageReturn : PercentageSummaryRow
	{
		public AnnualSystemPercentageReturn( Summary summary )
		{
      double totalROA = summary.TotalPnl / ( summary.FinalAccountValue - summary.TotalPnl );
//      summary.AnnualSystemPercentageReturn = ( ( Math.Pow( 1 + totalROA ,
//        1.0 / ( (double)summary.IntervalDays/365.0 ) ) ) - 1 ) * 100;
      this.rowDescription = "Annual system % return";
      this.rowValue = ( ( Math.Pow( 1 + totalROA ,
        1.0 / ( (double)summary.IntervalDays/365.0 ) ) ) - 1 ) * 100;
    }
	}
}
