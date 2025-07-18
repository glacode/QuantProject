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
	public class TotalNumberOfLongTrades : IntegerSummaryRow
	{
		public TotalNumberOfLongTrades( Summary summary ) : base()
		{
			double totalROA = summary.TotalPnl / ( summary.FinalAccountValue - summary.TotalPnl );
			this.rowDescription = "Total # of long trades";
			try
			{
				DataRow[] DataRows =
					summary.AccountReport.RoundTrades.DataTable.Select( "((Trade='Long')and(ExitPrice is not null))" );
				this.rowValue = DataRows.Length;
			}
			catch (Exception ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
			}
		}
	}
}
