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

    /// <summary>
    /// Adds quotesRow to the ValidateDataTable
    /// </summary>
    /// <param name="quotesRow">Row of quotes to added</param>
    private void suspiciousDataRowEventHandler( Object sender ,
      SuspiciousDataRowEventArgs eventArgs )
    {
      DataRow quotesRow = eventArgs.DataRow;
      DataRow dataRow = this.NewRow();
      foreach (DataColumn dataColumn in quotesRow.Table.Columns )
        dataRow[ dataColumn.ColumnName ] = quotesRow[ dataColumn ];
      this.Rows.Add( dataRow );
      //this.Rows.Add( quotesRow );
    }
    public void AddRows( string tickerIsLike )
    {
      QuotesToBeValidated quotesToBeValidated = new QuotesToBeValidated( tickerIsLike );
      quotesToBeValidated.SuspiciousDataRow +=
        new QuotesToBeValidated.SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
      quotesToBeValidated.Validate();
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
