using System;
using System.Data;
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Business.Validation
{
	/// <summary>
	/// Validates tickers and checks for valid tickers
	/// </summary>
	public class Validator
	{
		public Validator()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region IsValid
		private static bool isValid_withoutNewBeguinningQuotes( string ticker , DataTable validatedTicker )
		{
			bool returnValue = ( ( validatedTicker.Rows.Count > 0 ) &&
				( (string)validatedTicker.Rows[ 0 ][ ValidatedTickers.HashValue ] ==
				ValidatedTickers.GetHashValue( ticker , (DateTime)validatedTicker.Rows[ 0 ][ ValidatedTickers.StartDate ] ,
				(DateTime)validatedTicker.Rows[ 0 ][ ValidatedTickers.EndDate ] ) ) );
			if ( returnValue )
				// the validated period is st qui!!!!
				returnValue = returnValue;
			return returnValue;
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
//			if ( ( validatedTicker.Rows.Count > 0 ) &&
//				( (DateTime)validatedTicker.Rows[ 0 ][ ValidatedTickers.StartDate ] <
//					Quotes.GetStartDate( ticker ) ) )
//				// new quotes have been added at the beguinning of the ticker quotes, since when it was validated
//				ValidatedTickers.RemoveValidation( ticker );
			return isValid_withoutNewBeguinningQuotes( ticker , validatedTicker );
		}
		#endregion
	}
}
