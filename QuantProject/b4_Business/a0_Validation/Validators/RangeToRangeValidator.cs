using System;
using System.Collections;
using System.Data;
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.Business.Validation;
using QuantProject.Data.DataTables;

namespace QuantProject.Business.Validation.Validators
{
	/// <summary>
	/// Searches for range to range divergences
	/// </summary>
  public class RangeToRangeValidator : IValidator
  {
    private DataTable dataTableToBeValidated;

		private ArrayList rangeToRangeVisuallyValidated;

    public RangeToRangeValidator()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    public event SuspiciousDataRowEventHandler SuspiciousDataRow;

    #region "Validate"
    private int validate_currentTicker_set_rangeToRange(
      string currentTicker , int currentTickerStartingRowIndex , History rangeToRange )
    {
      rangeToRange.Clear();
      int localCurrentRowIndex = currentTickerStartingRowIndex + 1;
      rangeToRange.Add( this.dataTableToBeValidated.Rows[ currentTickerStartingRowIndex ][ "quDate" ] ,
        null );
      while ( ( localCurrentRowIndex < this.dataTableToBeValidated.Rows.Count ) &&
        ( (string)this.dataTableToBeValidated.Rows[ localCurrentRowIndex ][ "quTicker" ]
        == currentTicker ) )
      {
				double currentRange =
					( Convert.ToDouble( this.dataTableToBeValidated.Rows[ localCurrentRowIndex ][ "quHigh" ] ) -
					Convert.ToDouble( this.dataTableToBeValidated.Rows[ localCurrentRowIndex ][ "quLow" ] ) ) *
					Convert.ToDouble( this.dataTableToBeValidated.Rows[ localCurrentRowIndex ][ "quAdjustedClose" ] ) /
					Convert.ToDouble( this.dataTableToBeValidated.Rows[ localCurrentRowIndex ][ "quClose" ] );
				rangeToRange.Add( this.dataTableToBeValidated.Rows[ localCurrentRowIndex ][ "quDate" ] , currentRange );
        localCurrentRowIndex++;
      }
      return localCurrentRowIndex;
    }
    private void validate_currentTicker_withHistories_validateRow(
      DataRow quoteRow , double currentValue , double averageValue )
    {
      if ( ( Math.Abs( currentValue / averageValue ) > ConstantsProvider.SuspiciousRatio ) &&
				( this.rangeToRangeVisuallyValidated.IndexOf( quoteRow[ "quDate" ] ) < 0 ) )
        // the current close to close value is suspiciously larger
        // than the average close to close ratio
				// and it has not been visually validated yet
        this.SuspiciousDataRow( this , new SuspiciousDataRowEventArgs(
          quoteRow , ValidationWarning.SuspiciousRangeToRangeRatio ) );
    }
    private void validate_currentTicker_withHistories(
      History rangeToRange , History rangeToRangeMovingAverage ,
      int currentTickerStartingRowIndex , int nextTickerStartingRowIndex )
    {
      for (int i = currentTickerStartingRowIndex ; i < nextTickerStartingRowIndex ;
        i++ )
        if ( rangeToRangeMovingAverage.GetByIndex( i - currentTickerStartingRowIndex ) != null )
          try
          {
            validate_currentTicker_withHistories_validateRow(
              this.dataTableToBeValidated.Rows[ i ] ,
              Convert.ToDouble(rangeToRange.GetByIndex( i - currentTickerStartingRowIndex )) ,
              (double)(rangeToRangeMovingAverage.GetByIndex( i - currentTickerStartingRowIndex )) );
          }
          catch (Exception ex)
          {
            Console.WriteLine( ex.ToString() );
          }
    }
    private int validate_currentTicker( string currentTicker , int currentTickerStartingRowIndex )
    {
			this.rangeToRangeVisuallyValidated =
				QuantProject.DataAccess.Tables.VisuallyValidatedQuotes.GetRangeToRangeValidated( currentTicker );
      History rangeToRange = new History();
      History rangeToRangeMovingAverage;
      int nextTickerStartingRowIndex =
        validate_currentTicker_set_rangeToRange( currentTicker , currentTickerStartingRowIndex ,
          rangeToRange );
      rangeToRangeMovingAverage = rangeToRange.GetSimpleMovingAverage( 20 );
      validate_currentTicker_withHistories( rangeToRange , rangeToRangeMovingAverage ,
        currentTickerStartingRowIndex , nextTickerStartingRowIndex );
      return nextTickerStartingRowIndex;
    }

    /// <summary>
    /// Validates range to range divergencies
    /// </summary>
    /// <param name="dataTableToBeValidated">Quote rows to be validated</param>
    public void Validate( Quotes dataTableToBeValidated )
    {
      int currentRowIndex = 0;
      string currentTicker;
      this.dataTableToBeValidated = dataTableToBeValidated;
      while ( currentRowIndex < dataTableToBeValidated.Rows.Count )
      {
        currentTicker = (string)this.dataTableToBeValidated.Rows[ currentRowIndex ][ "quTicker" ];
        currentRowIndex = validate_currentTicker( currentTicker , currentRowIndex );
      }
    }
    #endregion

	}
}
