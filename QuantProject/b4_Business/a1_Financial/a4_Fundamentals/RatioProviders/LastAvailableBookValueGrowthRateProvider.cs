/*
QuantProject - Quantitative Finance Library

LastAvailableBookValueGrowthRateProvider.cs
Copyright (C) 2011
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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.*/

using System;
using System.Data;
using QuantProject.ADT;
using QuantProject.DataAccess.Tables;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Fundamentals;
using QuantProject.Business.Financial.Fundamentals.RatioProviders;


namespace QuantProject.Business.Financial.Fundamentals.RatioProviders
{
	/// <summary>
	/// Class implementing IGrowthRateProvider Interface
	/// It provides the last available growth rate of the book value
	/// </summary>
	[Serializable]
	public class LastAvailableBookValueGrowthRateProvider : 
							 FundamentalDataProvider,
							 IGrowthRateProvider													         
	{
		
		public LastAvailableBookValueGrowthRateProvider( int daysForAvailabilityOfData ) : 
																	 			  base( daysForAvailabilityOfData )
		{
		}
				
		public double GetGrowthRate( string ticker , DateTime atDate )
		{
			double returnValue;
			double lastBookValue;
			double previousBookValue;
			DateTime limitDateForEndingPeriodDate = 
				atDate.AddDays(- this.daysForAvailabilityOfData);
			DataTable tableOfBookValues =
				FinancialValues.GetLastFinancialValuesForTicker(
					ticker, FinancialValueType.BookValuePerShare, 12, limitDateForEndingPeriodDate);
			string[] tableForDebugging = 
				ExtendedDataTable.GetArrayOfStringFromRows(tableOfBookValues);
			int numOfRows = tableOfBookValues.Rows.Count;
			previousBookValue = (double)tableOfBookValues.Rows[numOfRows - 2]["fvValue"];
			lastBookValue = (double)tableOfBookValues.Rows[numOfRows - 1]["fvValue"];
			returnValue = (lastBookValue - previousBookValue)/previousBookValue;
			
			return returnValue;
		}
		public override double GetValue( string ticker , DateTime atDate )
		{
			return this.GetGrowthRate(ticker, atDate);
		}
	}
}
