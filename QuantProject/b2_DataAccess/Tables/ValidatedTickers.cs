using System;
using System.Data;

namespace QuantProject.DataAccess.Tables
{
	/// <summary>
	/// Class to access the ValidatedTickers table.
	/// </summary>
	public class ValidatedTickers
	{
		public ValidatedTickers()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	
		public static string Ticker = "vtTicker";
		public static string StartDate = "vtStartDate";
		public static string EndDate = "vtEndDate";
		public static string HashValue = "vtHashValue";
		public static string EditDate = "vtEditDate";


		/// <summary>
		/// Returns the hash value for the given instrument
		/// </summary>
		/// <param name="ticker">Instrument's ticker</param>
		/// <param name="startDate">Starting instrument quote's date, for hash value computation</param>
		/// <param name="endDate">Ending instrument quote's date, for hash value computation</param>
		/// <returns></returns>
		private static string getHashValue( string ticker , DateTime startDate , DateTime endDate )
		{
			Quotes quotes = new Quotes( ticker );
			return quotes.GetHashValue( startDate , endDate );
		}

		/// <summary>
		/// Checks if the instrument is valid (since the first date to the last date in the quotes table)
		/// </summary>
		/// <param name="ticker">Instrument's ticker</param>
		/// <returns></returns>
		public static bool IsValid( string ticker )
		{
			DataTable validatedTicker =
				SqlExecutor.GetDataTable( "select * from validatedTickers where vvTicker='" + ticker + "'" );
			return ( ( validatedTicker.Rows.Count > 0 ) &&
				( (string)validatedTicker.Rows[ 0 ][ ValidatedTickers.HashValue ] ==
				getHashValue( ticker , (DateTime)validatedTicker.Rows[ 0 ][ ValidatedTickers.StartDate ] ,
				(DateTime)validatedTicker.Rows[ 0 ][ ValidatedTickers.EndDate ] ) ) );
		}
	}
}
