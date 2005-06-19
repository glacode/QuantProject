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
		internal string format;

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
      set { this.rowValue = value; }
    }
		public string FormattedValue
		{
			get
			{
				string returnValue = "";
				if ( this.format == "" )
					throw new Exception( "No format has been defined!" );
				if ( this.rowValue != null )
					returnValue = string.Format( "{0:" + this.format + "}" , this.Value );
				return returnValue;
			}
		}

		public SummaryRow()
		{
			this.format = "";
		}
	}
}
