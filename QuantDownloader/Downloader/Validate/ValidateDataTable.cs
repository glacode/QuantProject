using System;
using System.Data;
using System.Data.OleDb;

namespace QuantProject.Applications.Downloader.Validate
{
  /// <summary>
  /// DataTable to be bound to the Validate form DataGrid. It will
  /// contain all data validation errors with descriptions and it will
  /// be used to fetch user input data fixing and to apply updates to the database.
  /// </summary>
  public class ValidateDataTable : DataTable
  {
    private string selectStatement;
    private OleDbCommandBuilder oleDbCommandBuilder;
    private OleDbDataAdapter oleDbDataAdapter;

    public ValidateDataTable()
    {
      this.selectStatement =
        "select * from quotes where 1=2";
      this.oleDbDataAdapter = new OleDbDataAdapter( selectStatement , AdoNetTools.OleDbConnection );
      this.oleDbCommandBuilder = new OleDbCommandBuilder( oleDbDataAdapter );
      this.oleDbDataAdapter.UpdateCommand = this.oleDbCommandBuilder.GetUpdateCommand();
      this.oleDbDataAdapter.Fill( this );
    }

    #region "addRows_forCurrentQuotesRow"
    /// <summary>
    /// Adds quotesRow to the ValidateDataTable
    /// </summary>
    /// <param name="quotesRow">Row of quotes to added</param>
    private void addRow( DataRow quotesRow )
    {
      DataRow dataRow = this.NewRow();
      foreach (DataColumn dataColumn in quotesRow.Table.Columns )
        dataRow[ dataColumn.ColumnName ] = quotesRow[ dataColumn ];
      this.Rows.Add( dataRow );
      //this.Rows.Add( quotesRow );
    }
    #region "addRows_forCurrentQuotesRow_checkLogicalErrors"
    /// <summary>
    /// Adds a row if not ((Low <= Open) and (Open <= High) and (Low <= Close) and (Close <= High))
    /// </summary>
    /// <param name="quotesRow">Row of quotes to be checked</param>
    private void checkOHLC( DataRow quotesRow )
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
        addRow( quotesRow );
    }
    /// <summary>
    /// Adds an error row if quotesRow doesn't respect logical constraints
    /// </summary>
    /// <param name="quotesRow">Row of quotes to be checked</param>
    private void addRows_forCurrentQuotesRow_checkLogicalErrors( DataRow quotesRow )
    {
      checkOHLC( quotesRow );
    }
    #endregion
    /// <summary>
    /// Adds errors for the current quotesRow (if any)
    /// </summary>
    /// <param name="quotesRow">Row of quotes to be checked</param>
    private void addRows_forCurrentQuotesRow( DataRow quotesRow )
    {
      addRows_forCurrentQuotesRow_checkLogicalErrors( quotesRow );
    }
    #endregion

    /// <summary>
    /// Adds all the probably wrong data quotes rows
    /// </summary>
    /// <param name="tickerIsLike">Contains the is like clause to fetch the tickers
    /// whose quotes are to be checked</param>
    public void AddRows( string tickerIsLike )
    {
      QuotesToBeValidated quotesToBeValidated = new QuotesToBeValidated( tickerIsLike );
      foreach ( DataRow quotesRow in quotesToBeValidated.Rows )
        this.addRows_forCurrentQuotesRow( quotesRow );
      this.AcceptChanges();
    }

    /// <summary>
    /// Commits the ValidateDataTable changes to the database
    /// </summary>
    public void Update()
    {
      try
      {
        this.oleDbDataAdapter.Update( this );
        this.AcceptChanges();
      }
      catch (Exception exception)
      {
        Console.WriteLine( exception.ToString() );
      }
    }
  }
}
