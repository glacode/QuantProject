using System;
using System.Data;
using System.Text;
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// DataTable for quotes table data
	/// </summary>
	public class Quotes : DataTable
	{
		public static string TickerFieldName = "quTicker";	// Ticker cannot be simply used because
		// it is already used below
		public static string Date = "quDate";
		public static string Open = "quOpen";
		public static string High = "quHigh";
		public static string Low = "quLow";
		public static string Close = "quClose";
		public static string Volume = "quVolume";
		public static string AdjustedClose = "quAdjustedClose";
		public static string AdjustedCloseToCloseRatio = "quAdjustedCloseToCloseRatio";

		private History history;

		/// <summary>
		/// Gets the ticker whose quotes are contained into the Quotes object
		/// </summary>
		/// <returns></returns>
		public string Ticker
		{
			get{ return ((string)this.Rows[ 0 ][ Quotes.TickerFieldName ]); }
		}

		private void fillDataTable( string ticker , DateTime startDate , DateTime endDate )
		{
			QuantProject.DataAccess.Tables.Quotes.SetDataTable( 
				ticker , startDate , endDate , this );
		}
		public Quotes( string ticker , DateTime startDate , DateTime endDate )
		{
			this.fillDataTable( ticker , startDate , endDate );
		}
		public Quotes( string ticker )
		{
			this.fillDataTable( 
				ticker ,
				QuantProject.DataAccess.Tables.Quotes.GetStartDate( ticker ) ,
				QuantProject.DataAccess.Tables.Quotes.GetEndDate( ticker ) );
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
			DataView quotes = new DataView( this );
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
			DataView quotes = new DataView( this );
			quotes.RowFilter = "( (quDate>=" + FilterBuilder.GetDateConstant( startDate ) +
				") and (quDate<=" + FilterBuilder.GetDateConstant( endDate ) + ") )";
			return HashProvider.GetHashValue( getHashValue_getQuoteString( quotes ) );
		}
		#endregion

		private void setHistory()
		{
			if ( this.history == null )
				// history has not been set, yet
			{
				this.history = new History();
				this.history.Import( this , "quDate" , "quAdjustedClose" );
			}
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
			setHistory();
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
			setHistory();
			return (DateTime) history.GetKey( Math.Max( 0 ,
				history.IndexOfKeyOrPrevious( quoteDate ) -
				followingDays ) );
		}
    

//		public DateTime GetPrecedingDate( DateTime quoteDate , int precedingDays )
//		{
//			History history = new History();
//			history.Import( this.quotes , "quDate" , "quAdjustedClose" );
//			return (DateTime) history.GetKey( Math.Max( 0 ,
//				history.IndexOfKeyOrPrevious( quoteDate ) -
//				precedingDays ) );
//		}
	}
}
