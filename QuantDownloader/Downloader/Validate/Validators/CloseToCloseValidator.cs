using System;
using System.Data;
using QuantProject.ADT.Histories;
using QuantProject.Business.Validation;

namespace QuantProject.Business.Validation.Validators
{
	/// <summary>
	/// Searches for close to close divergences
	/// </summary>
  public class CloseToCloseValidator : IValidator
  {
    private DataTable dataTableToBeValidated;
    private double suspiciousRatio = 2;

    public double SuspiciousRatio
    {
      get { return this.suspiciousRatio; }
      set { this.suspiciousRatio = value; }
    }

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
      if ( Math.Abs( currentValue / averageValue ) > this.suspiciousRatio )
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
      History closeToClose = new History();
      History closeToCloseMovingAverage;
      int nextTickerStartingRowIndex =
        validate_currentTicker_set_closeToClose( currentTicker , currentTickerStartingRowIndex ,
          closeToClose );
      closeToCloseMovingAverage = closeToClose.GetSimpleMovingAverage( 20 );
      validate_currentTicker_withHistories( closeToClose , closeToCloseMovingAverage ,
        currentTickerStartingRowIndex , nextTickerStartingRowIndex );
      return nextTickerStartingRowIndex;
    }

    /// <summary>
    /// Validates close to close divergencies
    /// </summary>
    /// <param name="dataTableToBeValidated">Quote rows to be validated</param>
    public void Validate( DataTable dataTableToBeValidated )
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
