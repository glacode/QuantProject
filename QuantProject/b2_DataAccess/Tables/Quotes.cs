using System;
using System.Data;
using System.Text;
using QuantProject.ADT;
using QuantProject.ADT.Histories;

namespace QuantProject.DataAccess.Tables
{
	/// <summary>
	/// Class to access the Quotes table
	/// </summary>
	public class Quotes
	{
		private DataTable quotes;

		/// <summary>
		/// Gets the ticker whose quotes are contained into the Quotes object
		/// </summary>
		/// <returns></returns>
		public string Ticker
		{
			get{ return ((string)this.quotes.Rows[ 0 ][ "quTicker" ]); }
		}

		public Quotes( string ticker)
		{
			this.quotes = Quotes.GetTickerQuotes( ticker );
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
		private string getHashValue_getQuoteString_getRowString_getSingleValueString( Object value )
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
		private StringBuilder getHashValue_getQuoteString_getRowString( DataRowView dataRow )
		{
			StringBuilder returnValue = new StringBuilder( "" );
			foreach ( DataColumn dataColumn in dataRow.DataView.Table.Columns )
				if ( dataColumn.ColumnName != "quTicker" )
					returnValue.Append( getHashValue_getQuoteString_getRowString_getSingleValueString(
						dataRow[ dataColumn.Ordinal ] ) );
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
		private string getHashValue_getQuoteString( DataView quotes )
		{
			StringBuilder returnValue = new StringBuilder( "" );
			foreach ( DataRowView dataRow in quotes )
				returnValue.Append( getHashValue_getQuoteString_getRowString( dataRow ) );
			return returnValue.ToString();
		}
		/// <summary>
		/// Computes the hash value for the contained quotes
		/// </summary>
		/// <returns>Hash value for all the quotes</returns>
		public string GetHashValue()
		{
			DataView quotes = new DataView( this.quotes );
			return HashProvider.GetHashValue( getHashValue_getQuoteString( quotes ) );
		}
		/// <summary>
		/// Computes the hash value for the contained quotes
		/// since startDate, to endDate
		/// </summary>
		/// <param name="startDate">date where hash begins being computed</param>
		/// <param name="endDate">date where hash ends being computed</param>
		/// <returns></returns>
		public string GetHashValue( DateTime startDate , DateTime endDate )
		{
			DataView quotes = new DataView( this.quotes );
			quotes.RowFilter = "( (quDate>=" + SQLBuilder.GetDateConstant( startDate ) +
				") and (quDate<=" + SQLBuilder.GetDateConstant( endDate ) + ") )";
			return HashProvider.GetHashValue( getHashValue_getQuoteString( quotes ) );
		}
		#endregion

		/// <summary>
		/// Computes the hash value for the quotes for the given ticker
		/// </summary>
		/// <param name="ticker">Ticker whose quotes must be hashed</param>
		/// <returns>Hash value for all the quotes for the given ticker</returns>
//		public static string GetHashValue( string ticker )
//		{
//			return HashProvider.GetHashValue( GetHashValue( GetTickerQuotes( ticker ) ) );
//		}
		
		/// <summary>
		/// returns the quotes DataTable for the given ticker
		/// </summary>
		/// <param name="instrumentKey">ticker whose quotes are to be returned</param>
		/// <returns></returns>
		public static DataTable GetTickerQuotes( string instrumentKey )
		{
			string sql = "select * from quotes where quTicker='" + instrumentKey + "' " +
				"order by quDate";
			return SqlExecutor.GetDataTable( sql );
		}

		/// <summary>
		/// returns the Date for the quote that is precedingDays before
		/// quoteDate
		/// </summary>
		/// <param name="quoteDate"></param>
		/// <param name="precedingDays"></param>
		/// <returns></returns>
		public DateTime GetPrecedingDate( DateTime quoteDate , int precedingDays )
		{
			History history = new History();
			history.Import( this.quotes , "quDate" , "quAdjustedClose" );
			return (DateTime) history.GetKey( Math.Max( 0 ,
				history.IndexOfKeyOrPrevious( quoteDate ) -
				precedingDays ) );
		}

		/// <summary>
		/// returns the Date for the quote that is followingDays after
		/// quoteDate
		/// </summary>
		/// <param name="quoteDate"></param>
		/// <param name="precedingDays"></param>
		/// <returns></returns>
		public DateTime GetFollowingDate( DateTime quoteDate , int followingDays )
		{
			History history = new History();
			history.Import( this.quotes , "quDate" , "quAdjustedClose" );
			return (DateTime) history.GetKey( Math.Max( 0 ,
				history.IndexOfKeyOrPrevious( quoteDate ) -
				followingDays ) );
		}
	}
}
