using System;
using System.Data;
using QuantProject.ADT;
using QuantProject.DataAccess;

namespace QuantProject.DataAccess.Tables
{
	/// <summary>
	/// Class to access the VisuallyValidatedTickers table
	/// </summary>
	public class VisuallyValidatedTickers
	{
		private string ticker;

		public string Ticker
		{
			get { return this.ticker; }
			set { this.ticker = value; }
		}
		public VisuallyValidatedTickers()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		/// <summary>
		/// writes to the database the visual validation of the Close to Close suspicious ratios
		/// </summary>
		/// <param name="ticker">ticker to be validated</param>
		public static void ValidateCloseToClose( string ticker )
		{
			try
			{
				Quotes quotes = new Quotes( ticker );
				OleDbSingleTableAdapter oleDbSingleTableAdapter =
					new OleDbSingleTableAdapter( "select * from visuallyValidatedTickers where vvTicker='" + ticker + "'" );
				string hashValue = quotes.GetHashValue(); 
				if ( oleDbSingleTableAdapter.DataTable.Rows.Count == 0 )
					// this ticker was not visually validated yet
					oleDbSingleTableAdapter.DataTable.Rows.Add( oleDbSingleTableAdapter.DataTable.NewRow() );
				else if ( ( (DateTime)oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvEndDate" ] != Quotes.GetEndDate( ticker ) ) ||
					( (DateTime)oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvEndDate" ] !=	Quotes.GetEndDate( ticker ) ) ||
					( (string)oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvHashValue" ] != hashValue ) )
					// this ticker was visually already validated, but with different values
					oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvRangeToRangeRatio" ] = false;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvTicker" ] = ticker;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvStartDate" ] =
					Quotes.GetStartDate( ticker );
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvEndDate" ] =
					Quotes.GetEndDate( ticker );
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvHashValue" ] = hashValue;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvCloseToCloseRatio" ] = true;
				oleDbSingleTableAdapter.OleDbDataAdapter.Update( oleDbSingleTableAdapter.DataTable );
			}
			catch ( Exception ex )
			{
				string exceptionMessage = ex.Message + "\n" + ex.StackTrace;
				Console.WriteLine( exceptionMessage );
			}
		}
		/// <summary>
		/// Writes to the database the visual validation of the Range to Range suspicious ratios
		/// </summary>
		/// <param name="ticker">ticker to be validated</param>
		/// <param name="ticker">quoteDate to be validated</param>
		public static void ValidateRangeToRange( string ticker , DateTime quoteDate)
		{
			try
			{
				Quotes quotes = new Quotes( ticker );
				OleDbSingleTableAdapter oleDbSingleTableAdapter =
					new OleDbSingleTableAdapter( "select * from visuallyValidatedTickers where vvTicker='"
						+ ticker + "'" );
				string hashValue = quotes.GetHashValue(
					quotes.GetPrecedingDate( quoteDate , ConstantsProvider.PrecedingDaysForVisualValidation ) ,
					quotes.GetFollowingDate( quoteDate , ConstantsProvider.PrecedingDaysForVisualValidation ) );
				if ( oleDbSingleTableAdapter.DataTable.Rows.Count == 0 )
					// this ticker was not visually validated yet
					oleDbSingleTableAdapter.DataTable.Rows.Add( oleDbSingleTableAdapter.DataTable.NewRow() );
				else if ( ( (DateTime)oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvEndDate" ] != Quotes.GetEndDate( ticker ) ) ||
					( (DateTime)oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvEndDate" ] !=	Quotes.GetEndDate( ticker ) ) ||
					( (string)oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvHashValue" ] != hashValue ) )
					// this ticker was visually already validated, but with different values
					oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvCloseToCloseRatio" ] = false;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvTicker" ] = ticker;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvStartDate" ] =
					Quotes.GetStartDate( ticker );
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvEndDate" ] =
					Quotes.GetEndDate( ticker );
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvHashValue" ] = hashValue;
				oleDbSingleTableAdapter.DataTable.Rows[ 0 ][ "vvRangeToRangeRatio" ] = true;
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
