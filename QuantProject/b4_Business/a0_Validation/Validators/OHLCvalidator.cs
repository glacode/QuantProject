using System;
using System.Data;
using QuantProject.Business.Validation;
using QuantProject.Data.DataTables;

namespace QuantProject.Business.Validation.Validators
{
	/// <summary>
	/// Validates OHLC quotes values
	/// </summary>
	public class OHLCvalidator : IValidator
	{
    public event SuspiciousDataRowEventHandler SuspiciousDataRow;

		public OHLCvalidator()
		{
			//
			// TODO: Add constructor logic here
			//
		}
    #region "Validate"
    #region "validate_currentQuotesRow_checkLogicalErrors"
    /// <summary>
    /// Adds a row if not ((Low <= Open) and (Open <= High) and (Low <= Close) and (Close <= High))
    /// </summary>
    /// <param name="quotesRow">Row of quotes to be checked</param>
    private void validate_currentQuotesRow_checkLogicalErrors_checkOHLC( DataRow quotesRow )
    {
      if (!
        ( ( Convert.ToDouble( quotesRow[ "quLow" ] ) <=
        ( Convert.ToDouble( quotesRow[ "quOpen" ] ) ) ) &&
        ( Convert.ToDouble( quotesRow[ "quOpen" ] ) <=
        ( Convert.ToDouble( quotesRow[ "quHigh" ] ) ) ) &&
        ( Convert.ToDouble( quotesRow[ "quLow" ] ) <=
        ( Convert.ToDouble( quotesRow[ "quClose" ] ) ) ) &&
        ( Convert.ToDouble( quotesRow[ "quClose" ] ) <=
        ( Convert.ToDouble( quotesRow[ "quHigh" ] ) ) )
        )
        )
        SuspiciousDataRow( this , new SuspiciousDataRowEventArgs(
          quotesRow , ValidationWarning.OpenHighLowCloseLogicalInconsistency ) );
    }
    /// <summary>
    /// Adds an error row if quotesRow doesn't respect logical constraints
    /// </summary>
    /// <param name="quotesRow">Row of quotes to be checked</param>
    private void validate_currentQuotesRow_checkLogicalErrors( DataRow quotesRow )
    {
      validate_currentQuotesRow_checkLogicalErrors_checkOHLC( quotesRow );
    }
    #endregion
    /// <summary>
    /// Adds errors for the current quotesRow (if any)
    /// </summary>
    /// <param name="quotesRow">Row of quotes to be checked</param>
    private void validate_currentQuotesRow( DataRow quotesRow )
    {
      validate_currentQuotesRow_checkLogicalErrors( quotesRow );
    }
    /// <summary>
    /// Validates Open High Low Close consistencies
    /// </summary>
    /// <param name="dataTable">Quote rows to be validated</param>
    public void Validate( Quotes quotes )
    {
      foreach ( DataRow quotesRow in quotes.Rows )
        this.validate_currentQuotesRow( quotesRow );
    }
  }
  #endregion
}
