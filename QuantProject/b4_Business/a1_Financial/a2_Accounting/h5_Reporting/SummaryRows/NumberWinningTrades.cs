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
	public class NumberWinningTrades : SummaryRow
	{
		public NumberWinningTrades( Summary summary )
		{
      this.rowDescription = "Number winning trades";
      DataRow[] DataRows = summary.AccountReport.RoundTrades.DataTable.Select( "([%Profit] > 0)" );
      this.rowValue = DataRows.Length;
    }
	}
}
