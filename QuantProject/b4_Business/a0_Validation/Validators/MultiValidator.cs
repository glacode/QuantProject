using System;
using System.Data;
using QuantProject.Business.Validation;
using QuantProject.Data.DataTables;

namespace QuantProject.Business.Validation.Validators
{
	/// <summary>
	/// Calls several different quotes' validators
	/// </summary>
	public class MultiValidator : IValidator
	{
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
    private void validate_OHLC( Quotes quotes )
    {
      OHLCvalidator oHLCvalidator = new OHLCvalidator();
      oHLCvalidator.SuspiciousDataRow +=
        new SuspiciousDataRowEventHandler( suspiciousDataRowHandler );
      oHLCvalidator.Validate( quotes );
    }
		/// <summary>
		/// Validates the Close to Close differencies
		/// </summary>
		/// <param name="dataTable">Quote rows to be validated</param>
		private void validate_CloseToClose( Quotes quotes )
		{
			CloseToCloseValidator closeToCloseValidator = new CloseToCloseValidator();
			closeToCloseValidator.SuspiciousDataRow +=
				new SuspiciousDataRowEventHandler( suspiciousDataRowHandler );
			closeToCloseValidator.Validate( quotes );
		}
		/// <summary>
		/// Validates the Range to Range differencies
		/// </summary>
		/// <param name="dataTable">Quote rows to be validated</param>
		private void validate_RangeToRange( Quotes quotes )
		{
			RangeToRangeValidator rangeToRangeValidator = new RangeToRangeValidator();
			rangeToRangeValidator.SuspiciousDataRow +=
				new SuspiciousDataRowEventHandler( suspiciousDataRowHandler );
			rangeToRangeValidator.Validate( quotes );
		}
		/// <summary>
    /// Validates the quotes rows
    /// </summary>
    /// <param name="dataTable">Contains the quotes rows to be validated</param>
    public void Validate( Quotes quotes )
    {
      this.validate_OHLC( quotes );
			this.validate_CloseToClose( quotes );
			this.validate_RangeToRange( quotes );
		}
		/// <summary>
		/// Validates the instrument quotes for the given ticker
		/// </summary>
		/// <param name="ticker">Instrument's ticker</param>
		public void Validate( string ticker )
		{

		}
  }
  #endregion
}
