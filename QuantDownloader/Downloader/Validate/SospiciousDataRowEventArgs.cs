using System;
using System.Data;

namespace QuantProject.Applications.Downloader.Validate
{
	/// <summary>
	/// EventArgs for the SuspiciousDataRow event
	/// </summary>
	public class SuspiciousDataRowEventArgs : EventArgs
	{
    private DataRow dataRow;

    /// <summary>
    /// The suspicious DataRow
    /// </summary>
    public DataRow DataRow
    {
      get
      {
        return this.dataRow;
      }
      set
      {
        value = this.dataRow;
      }
    }

		public SuspiciousDataRowEventArgs( DataRow dataRow )
		{
			this.dataRow = dataRow;
		}
	}
}
