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
	}
}
