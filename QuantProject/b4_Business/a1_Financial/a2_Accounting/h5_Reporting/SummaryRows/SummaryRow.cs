using System;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary description for SummaryRow.
	/// </summary>
  public class SummaryRow
  {
    internal string rowDescription;
    internal object rowValue;

    public string Description
    {
      get { return rowDescription; }
//      set { rowDescription = value; }
    }
    public object Value
    {
      get { return rowValue; }
//      set { rowValue = value; }
    }

		public SummaryRow()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
