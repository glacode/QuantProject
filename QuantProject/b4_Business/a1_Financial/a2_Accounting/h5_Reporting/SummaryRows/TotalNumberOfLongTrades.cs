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
	public class TotalNumberOfLongTrades : SummaryRow
	{
		public TotalNumberOfLongTrades( Summary summary )
		{
      double totalROA = summary.TotalPnl / ( summary.FinalAccountValue - summary.TotalPnl );
      this.rowDescription = "Total # of long trades";
      DataRow[] DataRows =
        summary.AccountReport.RoundTrades.DataTable.Select( "((Trade='Long')and(ExitPrice is not null))" );
      this.rowValue = DataRows.Length;
    }
	}
}
