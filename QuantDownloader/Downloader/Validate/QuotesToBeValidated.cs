using System;
using System.Data;
using System.Data.OleDb;

namespace QuantProject.Applications.Downloader.Validate
{
	/// <summary>
	/// Summary description for QuotesToBeValidated.
	/// </summary>
	public class QuotesToBeValidated : DataTable
	{
    private string selectStatement;
    private OleDbDataAdapter oleDbDataAdapter;
    
    public QuotesToBeValidated( string tickerIsLike )
		{
//      this.selectStatement =
//        "select * from quotes where quTicker like '" + tickerIsLike + "'";
      this.selectStatement =
        "select * from quotes where quTicker = '" + tickerIsLike + "'";
      this.oleDbDataAdapter = new OleDbDataAdapter( selectStatement , AdoNetTools.OleDbConnection );
      try
      {
        this.oleDbDataAdapter.Fill( this );
      }
      catch (Exception exception)
      {
        Console.WriteLine( exception.ToString() );
      }
    }

    public delegate void SuspiciousDataRowEventHandler(
      Object sender , SuspiciousDataRowEventArgs eventArgs );

    public event SuspiciousDataRowEventHandler SuspiciousDataRow;

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
        SuspiciousDataRow( this , new SuspiciousDataRowEventArgs( quotesRow ) );
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
    public void Validate()
    {
      foreach ( DataRow quotesRow in this.Rows )
        this.validate_currentQuotesRow( quotesRow );
    }
	}
  #endregion
}
