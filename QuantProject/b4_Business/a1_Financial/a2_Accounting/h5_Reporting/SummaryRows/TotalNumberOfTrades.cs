using System;
using System.Data;

using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary description for TotalNumberOfTrades.
	/// </summary>
	[Serializable]
	public class TotalNumberOfTrades : IntegerSummaryRow
	{
		public TotalNumberOfTrades( Summary summary )
		{
			double totalROA = summary.TotalPnl / ( summary.FinalAccountValue - summary.TotalPnl );
			this.rowDescription = "Total # of trades";
			this.format = ConstantsProvider.FormatWithZeroDecimals;
			try
			{
				DataRow[] DataRows =
					summary.AccountReport.RoundTrades.DataTable.Select( "(ExitPrice is not null)" );
				this.rowValue = DataRows.Length;
			}
			catch (Exception ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
			}
		}
	}
}
