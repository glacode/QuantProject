using System;
using System.Data;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary description for TotalNumberOfTrades.
	/// </summary>
	public class TotalNumberOfTrades : SummaryRow
	{
		public TotalNumberOfTrades( Summary summary )
		{
      double totalROA = summary.TotalPnl / ( summary.FinalAccountValue - summary.TotalPnl );
      this.rowDescription = "Total # of trades";
      DataRow[] DataRows =
        summary.AccountReport.RoundTrades.DataTable.Select( "(ExitPrice is not null)" );
      this.rowValue = DataRows.Length;
    }
	}
}
