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
	/// Searches for close to close divergences
	/// </summary>
  public class CloseToCloseValidator : IValidator
  {
    private DataTable dataTableToBeValidated;
		private ArrayList closeToCloseVisuallyValidated;


    public CloseToCloseValidator()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    public event SuspiciousDataRowEventHandler SuspiciousDataRow;

    #region "Validate"
    private int validate_currentTicker_set_closeToClose(
      string currentTicker , int currentTickerStartingRowIndex , History closeToClose )
    {
      closeToClose.Clear();
      int localCurrentRowIndex = currentTickerStartingRowIndex + 1;
      closeToClose.Add( this.dataTableToBeValidated.Rows[ currentTickerStartingRowIndex ][ "quDate" ] ,
        null );
      while ( ( localCurrentRowIndex < this.dataTableToBeValidated.Rows.Count ) &&
        ( (string)this.dataTableToBeValidated.Rows[ localCurrentRowIndex ][ "quTicker" ]
        == currentTicker ) )
      {
        closeToClose.Add( this.dataTableToBeValidated.Rows[ localCurrentRowIndex ][ "quDate" ] ,
          Math.Abs(
          Convert.ToDouble( this.dataTableToBeValidated.Rows[ localCurrentRowIndex ][ "quAdjustedClose" ] ) -
          Convert.ToDouble( this.dataTableToBeValidated.Rows[ localCurrentRowIndex - 1 ][ "quAdjustedClose" ] ) ) );
        localCurrentRowIndex++;
      }
      return localCurrentRowIndex;
    }
    private void validate_currentTicker_withHistories_validateRow(
      DataRow quoteRow , double currentValue , double averageValue )
    {
			if (
				( currentValue > 0.011 ) &&
				( Math.Abs( currentValue / averageValue ) > ConstantsProvider.SuspiciousRatio ) &&
				( this.closeToCloseVisuallyValidated.IndexOf( quoteRow[ "quDate" ] ) < 0 ) )
        // the current close to close value is suspiciously larger
        // than the average close to close ratio
        this.SuspiciousDataRow( this , new SuspiciousDataRowEventArgs(
          quoteRow , ValidationWarning.SuspiciousCloseToCloseRatio ) );
    }
    private void validate_currentTicker_withHistories(
      History closeToClose , History closeToCloseMovingAverage ,
      int currentTickerStartingRowIndex , int nextTickerStartingRowIndex )
    {
      for (int i = currentTickerStartingRowIndex ; i < nextTickerStartingRowIndex ;
        i++ )
        if ( closeToCloseMovingAverage.GetByIndex( i - currentTickerStartingRowIndex ) != null )
          try
          {
//            validate_currentTicker_withHistories_validateRow(
//              this.dataTableToBeValidated.Rows[ i ] ,
//              (double)closeToClose.GetByIndex( i - currentTickerStartingRowIndex ) ,
//              (double)closeToCloseMovingAverage.GetByIndex( i - currentTickerStartingRowIndex ) );
            validate_currentTicker_withHistories_validateRow(
              this.dataTableToBeValidated.Rows[ i ] ,
              Convert.ToDouble(closeToClose.GetByIndex( i - currentTickerStartingRowIndex )) ,
              (double)(closeToCloseMovingAverage.GetByIndex( i - currentTickerStartingRowIndex )) );
          }
          catch (Exception ex)
          {
            Console.WriteLine( ex.ToString() );
          }
    }
    private int validate_currentTicker( string currentTicker , int currentTickerStartingRowIndex )
    {
			this.closeToCloseVisuallyValidated = VisuallyValidatedQuotes.GetCloseToCloseValidated( currentTicker );
			History closeToClose = new History();
      History closeToCloseMovingAverage;
      int nextTickerStartingRowIndex =
        validate_currentTicker_set_closeToClose( currentTicker , currentTickerStartingRowIndex ,
          closeToClose );
      closeToCloseMovingAverage =
				closeToClose.GetSimpleMovingAverage( ConstantsProvider.DaysForMovingAverageForSuspiciousRatioValidation );
      validate_currentTicker_withHistories( closeToClose , closeToCloseMovingAverage ,
        currentTickerStartingRowIndex , nextTickerStartingRowIndex );
      return nextTickerStartingRowIndex;
    }

    /// <summary>
    /// Validates close to close divergencies
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
