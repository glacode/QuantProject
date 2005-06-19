using System;
using System.Data;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary description for TotalNumberOfTrades.
	/// </summary>
	[Serializable]
  public class MaxEquityDrawDown : PercentageSummaryRow
	{
    private Summary summary;
    private double drawDown;
    private double maxEquityValue;
    private void updateDrawDownForCurrentRow( DataRow dataRow )
    {
      this.maxEquityValue = Math.Max( this.maxEquityValue , (double)dataRow[ "AccountValue" ] );
      this.drawDown = Math.Max( this.drawDown ,
				( ( this.maxEquityValue - (double)dataRow[ "AccountValue" ] ) /
				this.maxEquityValue ) );
    }
    private void setDrawDown()
    {
      drawDown = 0.0;
      maxEquityValue = double.MinValue;
      foreach ( DataRow row in summary.AccountReport.Equity.DataTable.Rows )
        updateDrawDownForCurrentRow( row );
    }
		public MaxEquityDrawDown( Summary summary )
		{
      this.summary = summary;
      this.rowDescription = "Max equity drawdown (%)";
      setDrawDown();
      this.rowValue = this.drawDown*100;
    }
	}
}
