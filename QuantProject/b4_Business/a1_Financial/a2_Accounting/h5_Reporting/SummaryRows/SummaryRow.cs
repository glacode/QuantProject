using System;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary description for SummaryRow.
	/// </summary>
  [Serializable]
  public class SummaryRow
  {
    internal string rowDescription;
    internal object rowValue;

    public string Description
    {
      get { return rowDescription; }
      set { rowDescription = value; }
    }
    public object Value
    {
      get
			{
				object returnValue;
				if ( this.rowValue != null )
					returnValue = this.rowValue;
				else
					returnValue = 0;
				return returnValue;
			}
      set { rowValue = value; }
    }

		public SummaryRow()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
