using System;
using System.Data;
using System.Text;
using QuantProject.ADT;

namespace QuantProject.DataAccess.Tables
{
	/// <summary>
	/// Class to access the Quotes table
	/// </summary>
	public class Quotes
	{
		public Quotes()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		/// <summary>
		/// Returns the first date for the given ticker
		/// </summary>
		/// <param name="ticker">ticker for which the starting date has to be returned</param>
		/// <returns></returns>
		public static DateTime GetStartDate( string ticker )
		{
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select * from quotes where quTicker='" + ticker + "'" );
			return (DateTime)(dataTable.Rows[ 0 ][ "quDate" ]);
		}
		/// <summary>
		/// Returns the last date for the given ticker
		/// </summary>
		/// <param name="ticker">ticker for which the lasat date has to be returned</param>
		/// <returns></returns>
		public static DateTime GetEndDate( string ticker )
		{
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select * from quotes where quTicker='" + ticker + "'" );
			return (DateTime)(dataTable.Rows[ 0 ][ "quDate" ]);
		}
		#region GetHashValue
		private static string getHashValue_getQuoteString_getRowString_getSingleValueString( Object value )
		{
			string returnValue;
			if ( value.GetType() == Type.GetType( "System.DateTime" ) )
				returnValue = ( (DateTime) value ).ToString();
			else
			{
				if ( value.GetType() == Type.GetType( "System.Double" ) )
					returnValue = ( (float) value ).ToString( "F2" );
				else
					returnValue = value.ToString();
			}

			return returnValue + ";";
		}
		/// <summary>
		/// Computes the string representing the concatenation for a single quote row
		/// </summary>
		/// <param name="dataRow"></param>
		/// <returns></returns>
		private static StringBuilder getHashValue_getQuoteString_getRowString( DataRow dataRow )
		{
			StringBuilder returnValue = new StringBuilder( "" );
			foreach ( DataColumn dataColumn in dataRow.Table.Columns )
				if ( dataColumn.ColumnName != "quTicker" )
					returnValue.Append( getHashValue_getQuoteString_getRowString_getSingleValueString(
						dataRow[ dataColumn ] ) );
			//					returnValue += "ggg";
//					returnValue += getHashValue_getQuoteString_getRowString_getSingleValueString(
//						dataRow[ dataColumn ] );
			return returnValue;
		}
		/// <summary>
		/// Computes the string representing the concatenation of all the quotes
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		private static string getHashValue_getQuoteString( string ticker )
		{
			StringBuilder returnValue = new StringBuilder( "" );
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select * from quotes where quTicker='" + ticker + "'" );
			foreach ( DataRow dataRow in dataTable.Rows )
				returnValue.Append( getHashValue_getQuoteString_getRowString( dataRow ) );
			return returnValue.ToString();
		}
		/// <summary>
		/// Computes the hash value for the quotes for the given ticker
		/// </summary>
		/// <param name="ticker">Ticker whose quotes must be hashed</param>
		/// <returns>Hash value for all the quotes for the given ticker</returns>
		public static string GetHashValue( string ticker )
		{
			return HashProvider.GetHashValue( getHashValue_getQuoteString( ticker ) );
		}
		#endregion
	}
}
