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
    private ValidationWarning validationWarning;

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

    /// <summary>
    /// The suspicious DataRow
    /// </summary>
    public ValidationWarning ValidationWarning
    {
      get
      {
        return this.validationWarning;
      }
      set
      {
        value = this.validationWarning;
      }
    }
    public SuspiciousDataRowEventArgs( DataRow dataRow )
    {
      this.dataRow = dataRow;
    }
    public SuspiciousDataRowEventArgs( DataRow dataRow , ValidationWarning validationWarning )
    {
      this.dataRow = dataRow;
      this.validationWarning = validationWarning;
    }
  }
}
