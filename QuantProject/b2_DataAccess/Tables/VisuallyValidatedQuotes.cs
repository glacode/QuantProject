using System;
using System.Data;
using QuantProject.ADT;
using QuantProject.DataAccess;

using System.Collections;
namespace QuantProject.DataAccess.Tables
{
	/// <summary>
	/// Class to access the visuallyValidatedQuotes table
	/// </summary>
	public class VisuallyValidatedQuotes
	{
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

		/// <summary>
		/// writes to the database the visual validation of the Close to Close suspicious ratios
		/// </summary>
		/// <param name="quotes">contains all the quotes for the ticker to be validated</param>
		/// <param name="quoteDate">date to be validated</param>
		public static void ValidateRangeToRange( Quotes quotes , DateTime quoteDate )
		{
			try
			{
				SqlExecutor.ExecuteNonQuery(
					"delete * from visuallyValidatedQuotes where " +
					"vvTicker='" + quotes.Ticker + "' and " +
					"vvDate=" + SQLBuilder.GetDateConstant( quoteDate ) );
				OleDbSingleTableAdapter oleDbSingleTableAdapter =
					new OleDbSingleTableAdapter(
					"select * from visuallyValidatedQuotes where 1=2" );
				string hashValue = getHashValue( quotes , quoteDate );
				oleDbSingleTableAdapter.DataTable.Rows.Add(	oleDbSingleTableAdapter.DataTable.NewRow() );
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvTicker" ] = quotes.Ticker;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvDate" ] = quoteDate;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvValidationType" ] =
					ValidationTypes.RangeToRangeRatio;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvHashValue" ] = hashValue;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvEditDate" ] = DateTime.Now;
				oleDbSingleTableAdapter.OleDbDataAdapter.Update( oleDbSingleTableAdapter.DataTable );
			}
			catch ( Exception ex )
			{
				string exceptionMessage = ex.Message + "\n" + ex.StackTrace;
				Console.WriteLine( exceptionMessage );
			}
		}

		/// <summary>
		/// Returns the list of validated quote dates for the given ticker
		/// </summary>
		/// <param name="ticker">Instrument ticker whose validated quote dates are to be found</param>
		/// <returns></returns>
		public static ArrayList GetRangeToRangeValidated( string ticker )
		{
			ArrayList tickers = new ArrayList();
			Quotes quotes = new Quotes( ticker );
			DataTable validatedQuotes =
				SqlExecutor.GetDataTable( "select * from visuallyValidatedQuotes where vvTicker='" + ticker + "'" );
			foreach ( DataRow dataRow in validatedQuotes.Rows )
				if ( (string)dataRow[ "vvHashValue" ] == getHashValue( quotes , (DateTime)dataRow[ "vvDate" ] ) )
					// the current quote date had been visually validated with respect to the neighborhood quotes
					tickers.Add( dataRow[ "vvDate" ] );
				/// TO DO !!! add else branch to raise event 'broken hash value'
			return tickers;
		}
	}
}
