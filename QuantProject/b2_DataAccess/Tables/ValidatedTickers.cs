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


//		/// <summary>
//		/// Returns the hash value for the given instrument
//		/// </summary>
//		/// <param name="ticker">Instrument's ticker</param>
//		/// <param name="startDate">Starting instrument quote's date, for hash value computation</param>
//		/// <param name="endDate">Ending instrument quote's date, for hash value computation</param>
//		/// <returns></returns>
//		public static string GetHashValue( string ticker , DateTime startDate , DateTime endDate )
//		{
//			Quotes quotes = new Quotes( ticker );
//			return quotes.GetHashValue( startDate , endDate );
//		}

		/// <summary>
		/// Returns (if present) the validated ticker row
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public static void SetDataTable( string ticker , DataTable dataTable)
		{
			string sql =
				"select * from validatedTickers " +
				"where " + ValidatedTickers.Ticker + "='" + ticker + "'";
			SqlExecutor.SetDataTable( sql , dataTable );
		}

		public static void Validate( string ticker , DateTime startDate ,
			DateTime endDate , string hashValue )
		{
			try
			{
				SqlExecutor.ExecuteNonQuery( "delete * from validatedTickers " +
					"where " + ValidatedTickers.Ticker + "='" + ticker + "'" );
				SingleTableDbDataAdapter oleDbSingleTableAdapter =
					new SingleTableDbDataAdapter();
				oleDbSingleTableAdapter.SetAdapter( "validatedTickers" );
				oleDbSingleTableAdapter.DataTable.Rows.Add(	oleDbSingleTableAdapter.DataTable.NewRow() );
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ ValidatedTickers.Ticker ] = ticker;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ ValidatedTickers.StartDate ] = startDate;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ ValidatedTickers.EndDate ] = endDate;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ ValidatedTickers.HashValue ] = hashValue;
				oleDbSingleTableAdapter.DbDataAdapter.Update( oleDbSingleTableAdapter.DataTable );
			}
			catch ( Exception ex )
			{
				string exceptionMessage = ex.Message + "\n" + ex.StackTrace;
				Console.WriteLine( exceptionMessage );
			}
		}

	}
}
