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
  public class NumberWinningLongTrades : SummaryRow
	{
		public NumberWinningLongTrades( Summary summary )
		{
      this.rowDescription = "Number winning long trades";
			try
			{
				DataRow[] DataRows = summary.AccountReport.RoundTrades.DataTable.Select( "((Trade='Long')and([%Profit] > 0))" );
				this.rowValue = DataRows.Length;
			}
			catch (Exception ex)
			{
				ex = ex; // to avoid compilation warning;
			}
    }
	}
}
