using System;
using System.Collections;
using System.Data;
using QuantProject.ADT;
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;


namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// DataTable for visuallyValidatedQuotes table data
	/// </summary>
	public class VisuallyValidatedQuotes : DataTable
	{
		public static string Ticker = QuantProject.DataAccess.Tables.VisuallyValidatedQuotes.Ticker;
		public static string Date = QuantProject.DataAccess.Tables.VisuallyValidatedQuotes.Date;
		public static string ValidationType =
			QuantProject.DataAccess.Tables.VisuallyValidatedQuotes.ValidationType;
		public static string HashValue = QuantProject.DataAccess.Tables.VisuallyValidatedQuotes.HashValue;
		public static string EditDate = QuantProject.DataAccess.Tables.VisuallyValidatedQuotes.EditDate;

		public VisuallyValidatedQuotes()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		/// <summary>
		/// Returns the hash value to be stored/read into/from the visuallyValidatedQuotes table
		/// </summary>
		/// <param name="quoteDate">Date whose neighborhood quotes are to be hashed</param>
		/// <returns></returns>
		private static string getHashValue( Quotes quotes , DateTime quoteDate )
		{
			return quotes.GetHashValue( 
				quotes.GetPrecedingDate( quoteDate , ConstantsProvider.PrecedingDaysForVisualValidation ) ,
				quotes.GetFollowingDate( quoteDate , ConstantsProvider.PrecedingDaysForVisualValidation ) );
		}

		private static void validate( Quotes quotes , DateTime quoteDate , ValidationTypes validationType )
		{
			try
			{
				SqlExecutor.ExecuteNonQuery(
					"delete * from visuallyValidatedQuotes where " +
					"vvTicker='" + quotes.Ticker + "' and " +
					"vvDate=" + FilterBuilder.GetDateConstant( quoteDate ) );
				SingleTableDbDataAdapter oleDbSingleTableAdapter =
					new SingleTableDbDataAdapter(
					"select * from visuallyValidatedQuotes where 1=2" );
				string hashValue = getHashValue( quotes , quoteDate );
				oleDbSingleTableAdapter.DataTable.Rows.Add(	oleDbSingleTableAdapter.DataTable.NewRow() );
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ VisuallyValidatedQuotes.Ticker ] = quotes.Ticker;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ VisuallyValidatedQuotes.Date ] = quoteDate;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ VisuallyValidatedQuotes.ValidationType ] =
					validationType;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ VisuallyValidatedQuotes.HashValue ] = hashValue;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ VisuallyValidatedQuotes.EditDate ] = DateTime.Now;
				oleDbSingleTableAdapter.DbDataAdapter.Update( oleDbSingleTableAdapter.DataTable );
			}
			catch ( Exception ex )
			{
				string exceptionMessage = ex.Message + "\n" + ex.StackTrace;
				Console.WriteLine( exceptionMessage );
			}
		}

		/// <summary>
		/// writes to the database the visual validation of the Close to Close suspicious ratios
		/// </summary>
		/// <param name="quotes">contains all the quotes for the ticker to be validated</param>
		/// <param name="quoteDate">date to be validated</param>
		public static void ValidateCloseToClose( Quotes quotes , DateTime quoteDate )
		{
			validate( quotes , quoteDate , ValidationTypes.CloseToCloseRatio );
		}

		/// <summary>
		/// writes to the database the visual validation of the Range to Range suspicious ratios
		/// </summary>
		/// <param name="quotes">contains all the quotes for the ticker to be validated</param>
		/// <param name="quoteDate">date to be validated</param>
		public static void ValidateRangeToRange( Quotes quotes , DateTime quoteDate )
		{
			validate( quotes , quoteDate , ValidationTypes.RangeToRangeRatio );
		}

		private static ArrayList getVisuallyValidatedArrayList(
			string ticker , ValidationTypes validationType )
		{
			ArrayList tickers = new ArrayList();
			DataTable validatedQuotes =	QuantProject.DataAccess.Tables.VisuallyValidatedQuotes.GetValidatedQuotes(
				ticker , validationType );
			if ( validatedQuotes.Rows.Count > 0 )
				// some ticker quotes have been visually validated
			{
				Quotes quotes = new Quotes( ticker );
				foreach ( DataRow dataRow in validatedQuotes.Rows )
					if ( (string)dataRow[ VisuallyValidatedQuotes.HashValue ] ==
						getHashValue( quotes , (DateTime)dataRow[ VisuallyValidatedQuotes.Date ] ) )
						// the current quote date had been visually validated with respect to the neighborhood quotes
						tickers.Add( dataRow[ "vvDate" ] );
				/// TO DO !!! add else branch to raise event 'broken hash value'
			}
			return tickers;
		}
		/// <summary>
		/// Returns the list of visually validated quote dates for the given ticker,
		/// with respect to the Close To Close ratio
		/// </summary>
		/// <param name="ticker">Instrument ticker whose validated quote dates are to be found</param>
		/// <returns></returns>
		public static ArrayList GetCloseToCloseValidated( string ticker )
		{
			return getVisuallyValidatedArrayList( ticker , ValidationTypes.CloseToCloseRatio );
		}
		/// <summary>
		/// Returns the list of visually validated quote dates for the given ticker,
		/// with respect to the Range To Range ratio
		/// </summary>
		/// <param name="ticker">Instrument ticker whose validated quote dates are to be found</param>
		/// <returns></returns>
		public static ArrayList GetRangeToRangeValidated( string ticker )
		{
			return getVisuallyValidatedArrayList( ticker , ValidationTypes.RangeToRangeRatio );
		}
	}
}
