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
	public class NumberWinningShortTrades : IntegerSummaryRow
	{
		public NumberWinningShortTrades( Summary summary )
		{
			this.rowDescription = "Number winning short trades";
			this.format = ConstantsProvider.FormatWithZeroDecimals;
			try
			{
				DataRow[] DataRows = summary.AccountReport.RoundTrades.DataTable.Select(
					"((Trade='Short')and([%Profit] > 0))" );
				this.rowValue = DataRows.Length;
			}
			catch (Exception ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
			}
		}
	}
}
