/*
QuantProject - Quantitative Finance Library

LastAvailableGrowthRateProvider.cs
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
	/// It provides the last available growth rate (of earnings)
	/// </summary>
	[Serializable]
	public class LastAvailableGrowthRateProvider : 
							 FundamentalDataProvider,
							 IGrowthRateProvider													         
	{
		
		public LastAvailableGrowthRateProvider( int daysForAvailabilityOfData ) : 
																	 			  base( daysForAvailabilityOfData )
		{
		}
				
		public double GetGrowthRate( string ticker , DateTime atDate )
		{
			double returnValue;
			double lastEarnings;
			double previousEarnings;
			DateTime limitDateForEndingPeriodDate = 
				atDate.AddDays(- this.daysForAvailabilityOfData);
			DataTable tableOfEarnings =
				FinancialValues.GetLastFinancialValuesForTicker(
					ticker, FinancialValueType.NetIncome, 12, limitDateForEndingPeriodDate);
			string[] tableOfEarningsForDebugging = 
				ExtendedDataTable.GetArrayOfStringFromRows(tableOfEarnings);
			int numOfRows = tableOfEarnings.Rows.Count;
			previousEarnings = (double)tableOfEarnings.Rows[numOfRows - 2]["fvValue"];
			lastEarnings = (double)tableOfEarnings.Rows[numOfRows - 1]["fvValue"];
			returnValue = (lastEarnings - previousEarnings)/previousEarnings;
			return returnValue;
		}
		public override double GetValue( string ticker , DateTime atDate )
		{
			return this.GetGrowthRate(ticker, atDate);
		}
	}
}
