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
		public static string Ticker = "vvTicker";
		public static string Date = "vvDate";
		public static string ValidationType = "vvValidationType";
		public static string HashValue = "vvHashValue";
		public static string EditDate = "vvEditDate";

		public VisuallyValidatedQuotes()
		{
			//
			// TODO: Add constructor logic here
			//
		}
//		/// <summary>
//		/// Returns the hash value to be stored/read into/from the visuallyValidatedQuotes table
//		/// </summary>
//		/// <param name="quoteDate">Date whose neighborhood quotes are to be hashed</param>
//		/// <returns></returns>
//		private static string getHashValue( Quotes quotes , DateTime quoteDate )
//		{
//			return quotes.GetHashValue( 
//				quotes.GetPrecedingDate( quoteDate , ConstantsProvider.PrecedingDaysForVisualValidation ) ,
//				quotes.GetFollowingDate( quoteDate , ConstantsProvider.PrecedingDaysForVisualValidation ) );
//		}
//
//		private static void validate( Quotes quotes , DateTime quoteDate , ValidationTypes validationType )
//		{
//			try
//			{
//				SqlExecutor.ExecuteNonQuery(
//					"delete * from visuallyValidatedQuotes where " +
//					"vvTicker='" + quotes.Ticker + "' and " +
//					"vvDate=" + SQLBuilder.GetDateConstant( quoteDate ) );
//				OleDbSingleTableAdapter oleDbSingleTableAdapter =
//					new OleDbSingleTableAdapter(
//					"select * from visuallyValidatedQuotes where 1=2" );
//				string hashValue = getHashValue( quotes , quoteDate );
//				oleDbSingleTableAdapter.DataTable.Rows.Add(	oleDbSingleTableAdapter.DataTable.NewRow() );
//				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ VisuallyValidatedQuotes.Ticker ] = quotes.Ticker;
//				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ VisuallyValidatedQuotes.Date ] = quoteDate;
//				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ VisuallyValidatedQuotes.ValidationType ] =
//					validationType;
//				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ VisuallyValidatedQuotes.HashValue ] = hashValue;
//				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ VisuallyValidatedQuotes.EditDate ] = DateTime.Now;
//				oleDbSingleTableAdapter.OleDbDataAdapter.Update( oleDbSingleTableAdapter.DataTable );
//			}
//			catch ( Exception ex )
//			{
//				string exceptionMessage = ex.Message + "\n" + ex.StackTrace;
//				Console.WriteLine( exceptionMessage );
//			}
//		}
//
//		/// <summary>
//		/// writes to the database the visual validation of the Close to Close suspicious ratios
//		/// </summary>
//		/// <param name="quotes">contains all the quotes for the ticker to be validated</param>
//		/// <param name="quoteDate">date to be validated</param>
//		public static void ValidateCloseToClose( Quotes quotes , DateTime quoteDate )
//		{
//			validate( quotes , quoteDate , ValidationTypes.CloseToCloseRatio );
//		}
//
//		/// <summary>
//		/// writes to the database the visual validation of the Range to Range suspicious ratios
//		/// </summary>
//		/// <param name="quotes">contains all the quotes for the ticker to be validated</param>
//		/// <param name="quoteDate">date to be validated</param>
//		public static void ValidateRangeToRange( Quotes quotes , DateTime quoteDate )
//		{
//			validate( quotes , quoteDate , ValidationTypes.RangeToRangeRatio );
//		}

		public static DataTable GetValidatedQuotes(
			string ticker , ValidationTypes validationType )
		{
			DataTable validatedQuotes =	SqlExecutor.GetDataTable(
				"select * from visuallyValidatedQuotes " +
				"where " + VisuallyValidatedQuotes.Ticker + "='" + ticker + "'" +
				"and " + VisuallyValidatedQuotes.ValidationType + "=" +
				System.Convert.ToInt32( validationType ) );
//			foreach ( DataRow dataRow in validatedQuotes.Rows )
//				tickers.Add( dataRow[ "vvDate" ] );
			return validatedQuotes;
		}
//		/// <summary>
//		/// Returns the list of visually validated quote dates for the given ticker,
//		/// with respect to the Close To Close ratio
//		/// </summary>
//		/// <param name="ticker">Instrument ticker whose validated quote dates are to be found</param>
//		/// <returns></returns>
//		public static ArrayList GetCloseToCloseValidated( string ticker )
//		{
//			return getVisuallyValidatedArrayList( ticker , ValidationTypes.CloseToCloseRatio );
//		}
//		/// <summary>
//		/// Returns the list of visually validated quote dates for the given ticker,
//		/// with respect to the Range To Range ratio
//		/// </summary>
//		/// <param name="ticker">Instrument ticker whose validated quote dates are to be found</param>
//		/// <returns></returns>
//		public static ArrayList GetRangeToRangeValidated( string ticker )
//		{
//			return getVisuallyValidatedArrayList( ticker , ValidationTypes.RangeToRangeRatio );
//		}
	}
}
