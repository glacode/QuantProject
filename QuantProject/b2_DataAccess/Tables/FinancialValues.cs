/*
QuantProject - Quantitative Finance Library

FinancialValues.cs
Copyright (C) 2010
Marco Milletti

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;
using System.Data;
using QuantProject.DataAccess;


namespace QuantProject.DataAccess.Tables
{
	/// <summary>
	/// Class to access the financialValues table
	/// </summary>
	public class FinancialValues
	{
		
		private DataTable financialValues;
		
		/// <summary>
		/// Gets the ticker whose financials are contained into the FinancialValues object
		/// </summary>
		/// <returns></returns>
		public string Ticker
		{
			get{ return ((string)this.financialValues.Rows[ 0 ][ "fvTiTicker" ]); }
		}

		public FinancialValues( string ticker, int periodLengthInMonths)
		{
			this.financialValues = FinancialValues.GetTickerFinancialValues( ticker, periodLengthInMonths );
		}
				
		/// <summary>
		/// It provides addition of the given financial values into table "financial values"
		/// </summary>
		public static void Add( string ticker, int financialDataCode, int sourceCode,
		                        DateTime endingPeriodDate, int periodLengthInMonths,
		                        double financialValue, 
		                        DateTime downloadDate)
		{
			try
			{
				SqlExecutor.ExecuteNonQuery("INSERT INTO financialValues(fvTiTicker, fvFdId, fvFsId, " +
				                            "fvEndingPeriodDate, fvPeriodLengthInMonths, " +
				                            "fvValue, fvDownloadDate) " +
				                            "VALUES('" + ticker + "', " + financialDataCode + ", " + sourceCode + ", " +
				                             SQLBuilder.GetDateConstant(endingPeriodDate) + ", " + periodLengthInMonths + ", " +
				                            financialValue + ", " + 
				                            SQLBuilder.GetDateConstant(downloadDate) + ")");
			}
			catch(Exception ex)
			{
				string notUsed = ex.ToString();
			}
		}
		
		private static int getInternalCode(FinancialValueType financialValueType)
		{
			int returnValue = 0;
			string financialValueTypeString = 
				Enum.GetName(financialValueType.GetType(), financialValueType);
			returnValue = (int)Enum.Parse(financialValueType.GetType(), financialValueTypeString);
			
			return returnValue;
		}
		
		/// <summary>
		/// Returns last available financial value for the given ticker
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="financialValueType"></param>
		/// <param name="atDate">Last financial value at the first immediate
		/// date previous than param atDate will be returned</param>
		/// <returns></returns>
		public static double GetLastFinancialValueForTicker( string ticker, FinancialValueType financialValueType, 
		                                                     int periodLengthInMonths, 
		                                                     DateTime atDate )
		{
			int financialDataCode = getInternalCode(financialValueType);
			string sqlString = "select * from financialValues where fvTiTicker = '" + ticker + "' " +
				"AND fvFdId = " + financialDataCode + " " +
				"AND fvPeriodLengthInMonths = " + periodLengthInMonths + " " +
				"AND fvEndingPeriodDate <= " + SQLBuilder.GetDateConstant(atDate) + " " +
				"order by fvEndingPeriodDate";
			DataTable dataTable = SqlExecutor.GetDataTable(sqlString);
			int numOfRows = dataTable.Rows.Count;
						
			return (double)(dataTable.Rows[ numOfRows - 1 ][ "fvValue" ]);
		}
		/// <summary>
		/// Returns last available financial values for the given ticker
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="financialValueType"></param>
		/// <param name="atDate">Last financial values with endingPeriodDate
		/// previous than param atDate will be returned</param>
		/// <returns></returns>
		public static DataTable GetLastFinancialValuesForTicker( string ticker, FinancialValueType financialValueType, 
		                                                     int periodLengthInMonths, 
		                                                     DateTime atDate )
		{
			int financialDataCode = getInternalCode(financialValueType);
			string sqlString = "select * from financialValues where fvTiTicker = '" + ticker + "' " +
				"AND fvFdId = " + financialDataCode + " " +
				"AND fvPeriodLengthInMonths = " + periodLengthInMonths + " " +
				"AND fvEndingPeriodDate <= " + SQLBuilder.GetDateConstant(atDate) + " " + 
				"order by fvEndingPeriodDate";
			
			DataTable dataTable = SqlExecutor.GetDataTable(sqlString);
						
			return dataTable;
		}
		/// <summary>
		/// Returns last available financial values for all tickers
		/// </summary>
		/// <param name="financialValueType"></param>
		/// <param name="atDate">Last financial values with endingPeriodDate
		/// previous than param atDate will be returned</param>
		/// <returns></returns>
		public static DataTable GetLastFinancialValues(FinancialValueType financialValueType, 
		                                               int periodLengthInMonths, 
		                                               DateTime atDate )
		{
			int financialDataCode = getInternalCode(financialValueType);
			string sqlString = "select * from financialValues where fvFdId = " + financialDataCode + " " +
				"AND fvPeriodLengthInMonths = " + periodLengthInMonths + " " +
				"AND fvEndingPeriodDate <= " + SQLBuilder.GetDateConstant(atDate) + " " + 
				"order by fvEndingPeriodDate";
			
			DataTable dataTable = SqlExecutor.GetDataTable(sqlString);
						
			return dataTable;
		}
		
		/// <summary>
		/// returns the financial values DataTable for the given ticker
		/// </summary>
		public static DataTable GetTickerFinancialValues( string instrumentKey,
		                                                  int periodLengthInMonths )
		{
			string sql = "select * from financialValues where fvTiTicker = '" + instrumentKey + "' " +
				"and fvPeriodLengthInMonths = " + periodLengthInMonths + " order by fvEndingPeriodDate";
			return SqlExecutor.GetDataTable( sql );
		}
		/// <summary>
		/// Returns financial value for the given ticker at the given date (exact match!)
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="financialValueType"></param>
		/// <param name="atDate">Financial value at the given date</param>
		/// <returns></returns>
		public static double GetFinancialValue( string ticker, FinancialValueType financialValueType, 
		                                        int periodLengthInMonths, DateTime atDate )
		{
			int financialDataCode = getInternalCode(financialValueType);
			string sqlString = "select * from financialValues where fvTiTicker = '" + ticker + "' " +
				"AND fvFdId = " + financialDataCode + " " +
				"AND fvPeriodLengthInMonths = " + periodLengthInMonths + " " +
				"AND fvEndingPeriodDate = " + SQLBuilder.GetDateConstant(atDate);
			DataTable dataTable = SqlExecutor.GetDataTable(sqlString);
						
			return (double)(dataTable.Rows[ 0 ][ "fvValue" ]);
		}
		
	}
}
