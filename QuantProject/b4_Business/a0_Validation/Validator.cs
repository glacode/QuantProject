using System;
using System.Data;
using QuantProject.Data.DataTables;
using QuantProject.DataAccess;

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
		/// <summary>
		/// Validates (if the case) the given ticker from startDate to endDate
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns>True if and only if the ticker is validated for the given range</returns>
		private static bool Validate( string ticker , DateTime startDate , DateTime endDate )
		{
			ValidateDataTable validateDataTable = new ValidateDataTable();
			validateDataTable.AddRows( ticker , startDate , endDate );
			return ( validateDataTable.Rows.Count == 0 );
		}
		/// <summary>
		/// Checks if the instrument is valid (since the first date to the last date in the quotes table)
		/// </summary>
		/// <param name="ticker">Instrument's ticker</param>
		/// <returns></returns>
		public static bool IsValid( string ticker , DateTime startDate , DateTime endDate )
		{
			QuantProject.Data.DataTables.ValidatedTickers validatedTickers =
				new QuantProject.Data.DataTables.ValidatedTickers( ticker );
			return ( ( validatedTickers.Rows.Count > 0 ) &&
				( (DateTime)validatedTickers.Rows[ 0 ][ ValidatedTickers.StartDate ] > startDate ) &&
				( (DateTime)validatedTickers.Rows[ 0 ][ ValidatedTickers.EndDate ] > endDate ) );
		}
	}
}
