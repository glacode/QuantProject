using System;
using System.Data;
using QuantProject.ADT;
using QuantProject.DataAccess;

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
				string hashValue = quotes.GetHashValue( 
					quotes.GetPrecedingDate( quoteDate , ConstantsProvider.PrecedingDaysForVisualValidation ) ,
					quotes.GetFollowingDate( quoteDate , ConstantsProvider.PrecedingDaysForVisualValidation ) );
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
	}
}
