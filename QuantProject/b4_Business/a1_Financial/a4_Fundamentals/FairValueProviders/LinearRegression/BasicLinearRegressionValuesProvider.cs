/*
QuantProject - Quantitative Finance Library

BasicLinearRegressionValuesProvider.cs
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
using QuantProject.ADT.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Fundamentals;
using QuantProject.Data.Selectors;

namespace QuantProject.Business.Financial.Fundamentals.FairValueProviders.LinearRegression
{
	/// <summary>
	/// Class implementing ILinearRegressionValuesProvider Interface
	/// In this basic implementation, regressands are prices and
	/// regressors are fundamental values
	/// In this implementation a constant regressor is added by default
	/// </summary>
	[Serializable]
	public class BasicLinearRegressionValuesProvider : ILinearRegressionValuesProvider
	{
		private ITickerSelectorByDate tickerSelectorByDate;
		private FundamentalDataProvider[] fundamentalDataProviders;
		private DateTime regressandDateTimeForModelling;
		private DateTime regressorsDateTimeForModelling;
		private DayOfMonth dayOfMonthForRegressandAndRegressors;
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private double[] regressand;
		private double[,] regressors;
		
		public BasicLinearRegressionValuesProvider(ITickerSelectorByDate tickerSelectorByDate,
		                                           FundamentalDataProvider[] fundamentalDataProviders,
		                                           DayOfMonth dayOfMonthForRegressandAndRegressors)
		{
			this.tickerSelectorByDate = tickerSelectorByDate;
			this.fundamentalDataProviders = fundamentalDataProviders;
			this.dayOfMonthForRegressandAndRegressors = dayOfMonthForRegressandAndRegressors;
			this.regressandDateTimeForModelling = new DateTime(1900, 1 ,1);
			this.regressorsDateTimeForModelling = new DateTime(1900, 1 ,1);
			this.regressand = null;
			this.regressors = null;
			this.historicalMarketValueProvider = 
				new HistoricalAdjustedQuoteProvider();
		}
		
		protected virtual DataTable getTickers(DateTime dateTime)
		{
			DataTable returnValue = 
				this.tickerSelectorByDate.GetTableOfSelectedTickers(dateTime);
      
      return returnValue;
		}
		
		protected virtual DateTime getDateTimeForModelling(DateTime dateTime)
		{
			int year = dateTime.Year;
			DateTime returnValue = new DateTime(year, this.dayOfMonthForRegressandAndRegressors.Month,
			                                    this.dayOfMonthForRegressandAndRegressors.Day, 16, 0, 0);
			int maxDaysForAvailability = 
				this.fundamentalDataProviders[0].DaysForAvailabilityOfData;
			for(int i = 0; i < this.fundamentalDataProviders.Length; i++)
				if ( this.fundamentalDataProviders[i].DaysForAvailabilityOfData >
				     maxDaysForAvailability )
							maxDaysForAvailability = 
								this.fundamentalDataProviders[i].DaysForAvailabilityOfData;
			DateTime dateForAvailability = dateTime.AddDays(-maxDaysForAvailability);
			while(returnValue > dateForAvailability )
			{
				returnValue = new DateTime(returnValue.Year - 1, this.dayOfMonthForRegressandAndRegressors.Month,
			                             this.dayOfMonthForRegressandAndRegressors.Day, 16, 0, 0);
			}
			return returnValue;
		}			
		
		protected virtual DateTime getRegressandDateTimeForModelling(DateTime dateTime)
		{
			DateTime returnValue;
			if(dateTime != this.regressandDateTimeForModelling)
				returnValue = this.getDateTimeForModelling(dateTime);
			else
				returnValue = this.regressandDateTimeForModelling;
			
			return returnValue;
		}
		
		protected virtual DateTime getRegressorsDateTimeForModelling(DateTime dateTime)
		{
			DateTime returnValue;
			if(dateTime != this.regressorsDateTimeForModelling)
				returnValue = this.getDateTimeForModelling(dateTime);
			else
				returnValue = this.regressorsDateTimeForModelling;
			
			return returnValue;
		}
		
		protected virtual bool hasBothRegressandAndAllRegressors(string ticker, DateTime currentDateTimeForModelling)
		{
			bool returnValue = true;
			double currentRegressand = 0.0;
			double currentRegressor = 0.0;
			try
			{
				currentRegressand = this.historicalMarketValueProvider.GetMarketValue(ticker, currentDateTimeForModelling);
				for(int i = 0; i < this.fundamentalDataProviders.Length; i++)
				{
					int daysToAddForGettingRegressor = 
						this.fundamentalDataProviders[i].DaysForAvailabilityOfData;
					currentRegressor =
						this.fundamentalDataProviders[i].GetValue(ticker,
						                                          currentDateTimeForModelling.AddDays(daysToAddForGettingRegressor));
					}
			}
			catch(Exception ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
				returnValue = false;
			}
			return returnValue;
		}
		
		protected virtual int getNumberOfRegressands(DateTime currentDateTimeForModelling)
		{
			int returnValue = 0;
			DataTable tickers = 
				this.getTickers(currentDateTimeForModelling);
			foreach(DataRow row in tickers.Rows)
      {
				if(this.hasBothRegressandAndAllRegressors((string)row[0], currentDateTimeForModelling))
					returnValue++;
			}
			return returnValue;
		}
		
		protected virtual double getRegressand_getComputedRegressand_getSingleRegressand(string ticker,
		                                                                                 DateTime currentDateTimeForModelling)
		{
			double returnValue = 0.0;
			returnValue = this.historicalMarketValueProvider.GetMarketValue(ticker,
			                                                                currentDateTimeForModelling);
			return returnValue;
		}
		
		protected double[] getRegressand_getComputedRegressand(DateTime currentDateTimeForModelling)
		{
			double[] returnValue = 
				new double[this.getNumberOfRegressands(currentDateTimeForModelling)];
			DataTable tickers = this.getTickers(currentDateTimeForModelling);
			int idxCurrentRegressand = 0;
			foreach(DataRow row in tickers.Rows)
      {
				if(this.hasBothRegressandAndAllRegressors((string)row[0], currentDateTimeForModelling))
		   	{
				   	returnValue[idxCurrentRegressand] =
				   		this.getRegressand_getComputedRegressand_getSingleRegressand((string)row[0], currentDateTimeForModelling);
				   	idxCurrentRegressand++;
		   	}
			}
			this.regressandDateTimeForModelling = currentDateTimeForModelling;
			return returnValue;	
		}
		
		public double[] GetRegressand(DateTime dateTime)
		{
			double[] returnValue;
			DateTime currentDateTimeForModelling = 
				this.getRegressandDateTimeForModelling(dateTime);
			if( currentDateTimeForModelling == this.regressandDateTimeForModelling &&
			    this.regressand != null )
						returnValue = this.regressand;
			else
						returnValue = 
							this.getRegressand_getComputedRegressand(currentDateTimeForModelling);
			this.regressand = returnValue;
			return returnValue;
		}
		
		protected double[,] getRegressors_getComputedRegressors(DateTime currentDateTimeForModelling)
		{
			double[] currentRegressand = this.GetRegressand(currentDateTimeForModelling);
			int numOfRegressors = this.fundamentalDataProviders.Length + 1;
			//the first regressor is the constant regressor,
			//in order to compute the intercept
			double[,] returnValue =
				new double[currentRegressand.Length , numOfRegressors];
			
			DataTable tickers = this.getTickers(currentDateTimeForModelling);
			int idx_Ticker = 0;
			foreach(DataRow row in tickers.Rows)
      {
				if(this.hasBothRegressandAndAllRegressors((string)row[0], currentDateTimeForModelling))
		   	{
				   	for(int idxRegressor = 0; 
				   	    idxRegressor < numOfRegressors;
				   	    idxRegressor++)
				   	{
							if(idxRegressor == 0)
								returnValue[ idx_Ticker, idxRegressor ] = 1;
								//constant regressor
							else//idxRegressor - 1 is the fundamental non constant regressor
								returnValue[ idx_Ticker, idxRegressor ] =
				   				this.fundamentalDataProviders[idxRegressor - 1].GetValue( 
				   			       (string)row[0],
				   			       currentDateTimeForModelling.AddDays(fundamentalDataProviders[idxRegressor - 1].DaysForAvailabilityOfData));
				   	}
				   	idx_Ticker++;	
		   	}
			}
			this.regressorsDateTimeForModelling = currentDateTimeForModelling;
			return returnValue;	
		}
		
		public double[,] GetRegressors(DateTime dateTime)
		{
			double[,] returnValue;
			DateTime currentDateTimeForModelling = 
				this.getRegressorsDateTimeForModelling(dateTime);
			if( currentDateTimeForModelling == this.regressorsDateTimeForModelling &&
			    this.regressors != null )
						returnValue = this.regressors;
			else
						returnValue = 
							this.getRegressors_getComputedRegressors(currentDateTimeForModelling);
			this.regressors = returnValue;
			return returnValue;
		}
	}
}
