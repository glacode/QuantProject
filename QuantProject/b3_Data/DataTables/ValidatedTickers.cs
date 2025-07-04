using System;
using System.Data;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// DataTable for validatedTickers table data
	/// </summary>
	public class ValidatedTickers : DataTable
	{
		public static string Ticker = QuantProject.DataAccess.Tables.ValidatedTickers.Ticker;
		public static string StartDate = QuantProject.DataAccess.Tables.ValidatedTickers.StartDate;
		public static string EndDate = QuantProject.DataAccess.Tables.ValidatedTickers.EndDate;
		public static string HashValue = QuantProject.DataAccess.Tables.ValidatedTickers.HashValue;
		public static string EditDate = QuantProject.DataAccess.Tables.ValidatedTickers.EditDate;

		public ValidatedTickers( string ticker )
		{
			QuantProject.DataAccess.Tables.ValidatedTickers.SetDataTable( ticker , this );
		}
		/// <summary>
		/// Validates the ticker contained in the quotes DataTable
		/// </summary>
		/// <param name="quotes">Contains the quotes for the ticker to be validated</param>
		public static void Validate( QuantProject.Data.DataTables.Quotes quotes )
		{
			DateTime startDate = (DateTime)quotes.Rows[ 0 ][ QuantProject.Data.DataTables.Quotes.Date ];
			DateTime endDate = (DateTime)quotes.Rows[ quotes.Rows.Count - 1 ][
				QuantProject.Data.DataTables.Quotes.Date ];
			string ticker = quotes.Ticker;
			string hashValue = quotes.GetHashValue();
			QuantProject.DataAccess.Tables.ValidatedTickers.Validate(
				ticker , startDate , endDate , hashValue );
		}
	}
}
