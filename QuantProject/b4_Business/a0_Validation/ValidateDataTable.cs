using System;
using System.Data;
using System.Data.OleDb;
using QuantProject.ADT;
using QuantProject.Business.Validation.Validators;
using QuantProject.DataAccess;
using QuantProject.Data.DataTables;

namespace QuantProject.Business.Validation
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
			initializeValidateDataTable();
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
			DataColumn dataColumn = new DataColumn( "CloseToCloseHasBeenVisuallyValidated" ,
				System.Type.GetType( "System.Boolean" ) );
			dataColumn.DefaultValue = false;
			this.Columns.Add( dataColumn );
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
		private void addRows_with_quotesToBeValidated( Quotes quotes )
		{
//			quotesToBeValidated.SuspiciousDataRow +=
//				new SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
			//			quotesToBeValidated.Validate();
			MultiValidator multiValidator = new MultiValidator();
//			multiValidator.SuspiciousRatio = this.suspiciousRatio;
			multiValidator.SuspiciousDataRow +=
				new SuspiciousDataRowEventHandler( this.suspiciousDataRowEventHandler );
			multiValidator.Validate( quotes );
			this.AcceptChanges();
		}
		public void AddRows( string tickerIsLike )
		{
			Quotes quotesToBeValidated = new Quotes( tickerIsLike );
//			quotesToBeValidated.SuspiciousDataRow +=
//				new SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
			//      new QuotesToBeValidated.SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
			this.addRows_with_quotesToBeValidated( quotesToBeValidated );
		}
//		public void AddRows(double suspiciousRatio )
//		{
//			QuotesToBeValidated quotesToBeValidated = new QuotesToBeValidated(this.tableOfTickersToBeValidated);
//			quotesToBeValidated.SuspiciousRatio = suspiciousRatio;
//			quotesToBeValidated.SuspiciousDataRow +=
//				new SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
//			//      new QuotesToBeValidated.SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
//			quotesToBeValidated.Validate();
//			this.AcceptChanges();
//		}
		/// <summary>
		/// Add suspicious rows for the given instrument, since the startDate to the endDate
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		public void AddRows( string ticker , DateTime startDate , DateTime endDate )
		{
			Quotes quotesToBeValidated =
				new Quotes( ticker , startDate , endDate );
			this.addRows_with_quotesToBeValidated( quotesToBeValidated );
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
