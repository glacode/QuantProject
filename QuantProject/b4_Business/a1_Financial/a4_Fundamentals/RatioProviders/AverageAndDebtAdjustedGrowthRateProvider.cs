/*
QuantProject - Quantitative Finance Library

AverageAndDebtAdjustedGrowthRateProvider.cs
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
using QuantProject.DataAccess.Tables;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Fundamentals;
using QuantProject.Business.Financial.Fundamentals.RatioProviders;


namespace QuantProject.Business.Financial.Fundamentals.RatioProviders
{
	/// <summary>
	/// Class implementing IGrowthRateProvider Interface
	/// It provides an average growth rate, based on past
	/// incomes and then adjusted by the long term ratio level
	/// </summary>
	[Serializable]
	public class AverageAndDebtAdjustedGrowthRateProvider : 
							 FundamentalDataProvider,
							 IGrowthRateProvider													         
	{
		
		protected double optimalDebtEquityRatio;
		protected int maximumNumberOfGrowthRatesToTakeIntoAccount;
									 	
		public AverageAndDebtAdjustedGrowthRateProvider( int daysForAvailabilityOfData,
		                                                 int maximumNumberOfGrowthRatesToTakeIntoAccount,
							 	                                     double optimalDebtEquityRatio) :
																	 			  base( daysForAvailabilityOfData )
		{
			this.optimalDebtEquityRatio = optimalDebtEquityRatio;
			this.maximumNumberOfGrowthRatesToTakeIntoAccount = 
				maximumNumberOfGrowthRatesToTakeIntoAccount;
		}
		
		
		private double getGrowthRate_getLastAvailableDebtEquityRatio( string ticker , DateTime atDate )
		{
			double returnValue = this.optimalDebtEquityRatio;
			//if not available the given optimalDebtEquityRatio is returned,
			//so not to bias the estimated average growth rate
			DateTime limitDateForEndingPeriodDate = 
				atDate.AddDays(- this.daysForAvailabilityOfData);
		  DataTable tableOfDebtEquityRatios =
				FinancialValues.GetLastFinancialValuesForTicker(
					ticker, FinancialValueType.DebtEquityRatio, 12, limitDateForEndingPeriodDate);
			int numOfRows = tableOfDebtEquityRatios.Rows.Count;
			if( numOfRows > 0)
				returnValue = 
					Math.Abs((double)tableOfDebtEquityRatios.Rows[ numOfRows - 1 ]["fvValue"]);
			
			return returnValue;
		}
		
		private double[] getGrowthRate_getAverageGrowthRate_getGrowthRates( double[] allAvailableGrowthRates )
		{
			int numOfAvailableGrowthRates = allAvailableGrowthRates.Length;
			int numOfRatesToTakeIntoAccount =
				Math.Min(numOfAvailableGrowthRates, this.maximumNumberOfGrowthRatesToTakeIntoAccount);
			double[] returnValue = new double[numOfRatesToTakeIntoAccount];
			for(int i = 0; i < numOfRatesToTakeIntoAccount; i++)
			{
				returnValue[i] = 
					allAvailableGrowthRates[numOfAvailableGrowthRates - (numOfRatesToTakeIntoAccount - i)];
			}
			
			return returnValue;
		}
		
		private double getGrowthRate_getAverageGrowthRate( DataTable tableOfEarnings )
		{
			double returnValue;
			int numOfRows = tableOfEarnings.Rows.Count;
			double[] growthRates = new double[ numOfRows - 1 ];
			for(int i = 0; i < growthRates.Length ; i++)
			{
				growthRates[i] = 
					( (double)tableOfEarnings.Rows[ i + 1 ]["fvValue"] -
					  (double)tableOfEarnings.Rows[ i ]["fvValue"] ) /
					  (double)tableOfEarnings.Rows[ i ]["fvValue"];
			}
			double[] growthRatesToTakeIntoAccount =
				this.getGrowthRate_getAverageGrowthRate_getGrowthRates(growthRates);
			returnValue = ADT.Statistics.BasicFunctions.SimpleAverage(growthRatesToTakeIntoAccount);
			
			return returnValue;
		}
		
		public double GetGrowthRate( string ticker , DateTime atDate )
		{
			double returnValue;
			DateTime limitDateForEndingPeriodDate = 
				atDate.AddDays(- this.daysForAvailabilityOfData);
			DataTable tableOfEarnings =
				FinancialValues.GetLastFinancialValuesForTicker(
					ticker, FinancialValueType.NetIncome, 12, limitDateForEndingPeriodDate);
			double averageGrowthRate = 
				this.getGrowthRate_getAverageGrowthRate(tableOfEarnings);
			double lastAvailableDebtEquityRatio =
				this.getGrowthRate_getLastAvailableDebtEquityRatio(ticker, atDate);
			
			returnValue = averageGrowthRate * 
									 	Math.Min(1.0,	(this.optimalDebtEquityRatio /
				               			       lastAvailableDebtEquityRatio ) );
			return returnValue;
		}
		public override double GetValue( string ticker , DateTime atDate )
		{
			return this.GetGrowthRate(ticker, atDate);
		}
	}
}
