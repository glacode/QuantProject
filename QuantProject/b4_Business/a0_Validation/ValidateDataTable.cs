using System;
using System.Data;
using System.Data.Common;
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
		private DbCommandBuilder dbCommandBuilder;
		private DbDataAdapter dbDataAdapter;
		private DataTable tableOfTickersToBeValidated;

		public Quotes Quotes;

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
			this.dbDataAdapter =
				DbDataAdapterProvider.GetDbDataAdapter( this.selectStatement );
//				new OleDbDataAdapter( selectStatement , ConnectionProvider.DbConnection );
//			this.oleDbCommandBuilder = new OleDbCommandBuilder( dbDataAdapter );
			this.dbCommandBuilder =
				DbCommandBuilderProvider.GetDbCommanBuilder( dbDataAdapter );
			this.dbDataAdapter.UpdateCommand = this.dbCommandBuilder.GetUpdateCommand();
			this.dbDataAdapter.Fill( this );
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
		public void AddRows( string ticker )
		{
			this.Quotes = new Quotes( ticker );
//			quotesToBeValidated.SuspiciousDataRow +=
//				new SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
			//      new QuotesToBeValidated.SuspiciousDataRowEventHandler( suspiciousDataRowEventHandler );
			this.addRows_with_quotesToBeValidated( this.Quotes );
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
			this.Quotes =	new Quotes( ticker , startDate , endDate );
			this.addRows_with_quotesToBeValidated( this.Quotes );
		}
		/// <summary>
		/// Commits the ValidateDataTable changes to the database
		/// </summary>
		public void Update()
		{
			try
			{
				this.dbDataAdapter.Update( this );
				this.AcceptChanges();
			}
			catch (Exception exception)
			{
				Console.WriteLine( exception.ToString() );
			}
		}
	}
}
