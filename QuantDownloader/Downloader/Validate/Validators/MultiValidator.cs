using System;
using System.Data;
using QuantProject.Applications.Downloader.Validate;
using QuantProject.Applications.Downloader.Validate.Validators;

namespace QuantProject.Applications.Downloader.Validate.Validators
{
	/// <summary>
	/// Calls several different quotes' validators
	/// </summary>
	public class MultiValidator : IValidator
	{
    private double suspiciousRatio;

    public double SuspiciousRatio
    {
      get { return this.suspiciousRatio; }
      set { this.suspiciousRatio = value; }
    }

		public MultiValidator()
		{
			//
			// TODO: Add constructor logic here
			//
		}
    public event SuspiciousDataRowEventHandler SuspiciousDataRow;

    #region "Validate"
    private void suspiciousDataRowHandler( Object sender ,
      SuspiciousDataRowEventArgs eventArgs )
    {
      SuspiciousDataRow( this , eventArgs );
    }
    /// <summary>
    /// Validates the Open High Low Close consistencies
    /// </summary>
    /// <param name="dataTable">Quote rows to be validated</param>
    private void validate_OHLC( DataTable dataTable )
    {
      OHLCvalidator oHLCvalidator = new OHLCvalidator();
      oHLCvalidator.SuspiciousDataRow +=
        new SuspiciousDataRowEventHandler( suspiciousDataRowHandler );
      oHLCvalidator.Validate( dataTable );
    }
    /// <summary>
    /// Validates the Close to Close differencies
    /// </summary>
    /// <param name="dataTable">Quote rows to be validated</param>
    private void validate_CloseToClose( DataTable dataTable )
    {
      CloseToCloseValidator closeToCloseValidator = new CloseToCloseValidator();
      closeToCloseValidator.SuspiciousRatio = this.suspiciousRatio;
      closeToCloseValidator.SuspiciousDataRow +=
        new SuspiciousDataRowEventHandler( suspiciousDataRowHandler );
      closeToCloseValidator.Validate( dataTable );
    }
    /// <summary>
    /// Validates the quotes rows
    /// </summary>
    /// <param name="dataTable">Contains the quotes rows to be validated</param>
    public void Validate( DataTable dataTable )
    {
      this.validate_OHLC( dataTable );
      this.validate_CloseToClose( dataTable );
    }
  }
  #endregion
}
