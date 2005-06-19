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
  public class NumberWinningTrades : IntegerSummaryRow
	{
		public NumberWinningTrades( Summary summary )
		{
      this.rowDescription = "Number winning trades";
			this.format = ConstantsProvider.FormatWithZeroDecimals;
			try
			{
				DataRow[] DataRows = summary.AccountReport.RoundTrades.DataTable.Select( "([%Profit] > 0)" );
				this.rowValue = DataRows.Length;
			}
			catch (Exception ex)
			{
				ex = ex; // to avoid compilation warning;
			}
    }
	}
}
