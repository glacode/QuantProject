using System;
using System.Data;
using System.Data.OleDb;
using QuantProject.DataAccess;

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
	private DataTable tableOfTickersToBeValidated;

    public ValidateDataTable()
    {
//<<<<<<< ValidateDataTable.cs
//      this.selectStatement =
//        "select * from quotes where 1=2";
//      this.oleDbDataAdapter =
//        new OleDbDataAdapter( selectStatement , ConnectionProvider.OleDbConnection );
//      this.oleDbCommandBuilder = new OleDbCommandBuilder( oleDbDataAdapter );
//      this.oleDbDataAdapter.UpdateCommand = this.oleDbCommandBuilder.GetUpdateCommand();
//      this.oleDbDataAdapter.Fill( this );
//      this.Columns.Add( new DataColumn( "CloseToCloseHasBeenVisuallyValidated" ,
//        System.Type.GetType( "System.Boolean" ) ) );
//
//      this.TableName = "quotes";
////<<<<<<< ValidateDataTable.cs
//      this.Columns.Add( "ValidationWarning" ,
//        ValidationWarning.OpenHighLowCloseLogicalInconsistency.GetType() );
      
//=======
//      
//>>>>>>> 1.6
//=======
      initializeValidateDataTable();
//>>>>>>> 1.8
    }

	public ValidateDataTable(DataTable tableOfTickers)
	{
		initializeValidateDataTable();
		// specific code used by this constructor
		// the table member is used when the validation procedure
		// is called by the tickerViewer object
		this.tableOfTickersToBeValidated = tableOfTickers;
	}

	#region initializeValidateDataTable
		private void initializeValidateDataTable()
		{
      this.selectStatement =
				"select * from quotes where 1=2";
			this.oleDbDataAdapter =
				new OleDbDataAdapter( selectStatement , ConnectionProvider.OleDbConnection );
			this.oleDbCommandBuilder = new OleDbCommandBuilder( oleDbDataAdapter );
			this.oleDbDataAdapter.UpdateCommand = this.oleDbCommandBuilder.GetUpdateCommand();
			this.oleDbDataAdapter.Fill( this );
      this.Columns.Add( new DataColumn( "CloseToCloseHasBeenVisuallyValidated" ,
        System.Type.GetType( "System.Boolean" ) ) );
      this.TableName = "quotes";
			//<<<<<<< ValidateDataTable.cs
			this.Columns.Add( "ValidationWarning" ,
				ValidationWarning.OpenHighLowCloseLogicalInconsistency.GetType() );
	      
			//=======
			//      
			//>>>>>>> 1.6
		}
	#endregion

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
      dataRow[ "ValidationWarning" ] = eventArgs.ValidationWarning;
      this.Rows.Add( dataRow );
      //this.Rows.Add( quotesRow );
    }
    public void AddRows( string tickerIsLike , double suspiciousRatio )
    {
      QuotesToBeValidated quotesToBeValidated = new QuotesToBeValidated( tickerIsLike );
      quotesToBeValidated.SuspiciousRatio = suspiciousRatio;
      quotesToBeValidated.SuspiciousDataRow +=
        new SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
//      new QuotesToBeValidated.SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
      quotesToBeValidated.Validate();
      this.AcceptChanges();
    }
	public void AddRows(double suspiciousRatio )
	{
		QuotesToBeValidated quotesToBeValidated = new QuotesToBeValidated(this.tableOfTickersToBeValidated);
		quotesToBeValidated.SuspiciousRatio = suspiciousRatio;
		quotesToBeValidated.SuspiciousDataRow +=
			new SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
		//      new QuotesToBeValidated.SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
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
