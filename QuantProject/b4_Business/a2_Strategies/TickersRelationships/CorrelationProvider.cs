/*
QuantProject - Quantitative Finance Library

CorrelationProvider.cs
Copyright (C) 2008 
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
using System.Collections;

using QuantProject.ADT.Statistics;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;


namespace QuantProject.Business.Strategies.TickersRelationships
{
  /// <summary>
  /// Abstract class that provides correlation's indexes two by two within a 
  /// given set of tickers
  /// </summary>
  [Serializable]
  public abstract class CorrelationProvider
  {
		protected ReturnsManager returnsManager;
		protected string[] tickers;
		protected float minimumAbsoluteReturnValue;
		protected float maximumAbsoluteReturnValue;
		protected int numOfCombinationTwoByTwo;
		protected TickersPearsonCorrelation[] pearsonCorrelations;
		protected EndOfDayDateTime firstEndOfDayDateTime;
		protected EndOfDayDateTime lastEndOfDayDateTime;
		protected string benchmark;

		/// <summary>
		/// Creates the correlation provider
		/// </summary>
		/// <param name="tickersToAnalyze">Array of tickers to be analyzed</param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="minimumAbsoluteReturnValue">Both current tickers' returns
		/// have to be greater than minimumAbsoluteReturnValue for being considered
		/// significant and so computed in the correlation formula</param>
		/// <param name="maximumAbsoluteReturnValue">Both current tickers' returns
		/// have to be less than maximumAbsoluteReturnValue</param>
		/// <param name="benchmark">The benchmark used for computation
		/// of returns</param>
    public CorrelationProvider(string[] tickersToAnalyze,
					 DateTime startDate, DateTime endDate,
					 float minimumAbsoluteReturnValue, float maximumAbsoluteReturnValue,
					 string benchmark)
    {
			this.tickers = tickersToAnalyze;
			this.setEndOfDayDatesTime(startDate, endDate);
			this.minimumAbsoluteReturnValue = minimumAbsoluteReturnValue;
			this.maximumAbsoluteReturnValue = maximumAbsoluteReturnValue;
			this.benchmark = benchmark;
			
			this.numOfCombinationTwoByTwo =
				(int)((Math.Pow(this.tickers.Length, 2) - this.tickers.Length ) / 2);
			this.setReturnsManager();
    }

		/// <summary>
		/// Creates the correlation provider
		/// </summary>
		/// <param name="tickersToAnalyze">Array of tickers to be analyzed</param>
		/// <param name="returnsManager"></param>
		/// <param name="minimumAbsoluteReturnValue">Both current tickers' returns
		/// have to be greater than minimumAbsoluteReturnValue for being considered
		/// significant and so computed in the correlation formula</param>
		/// <param name="maximumAbsoluteReturnValue">Both current tickers' returns
		/// have to be less than maximumAbsoluteReturnValue</param>
		public CorrelationProvider( string[] tickersToAnalyze,
			ReturnsManager returnsManager,
			float minimumAbsoluteReturnValue, float maximumAbsoluteReturnValue )
		{
			this.tickers = tickersToAnalyze;
			this.returnsManager = returnsManager;
			this.minimumAbsoluteReturnValue = minimumAbsoluteReturnValue;
			this.maximumAbsoluteReturnValue = maximumAbsoluteReturnValue;
						
			this.numOfCombinationTwoByTwo = 
				(int)((Math.Pow(this.tickers.Length, 2) - this.tickers.Length ) / 2);
		}
			
		protected abstract void setEndOfDayDatesTime(DateTime startDate, DateTime endDate);
		protected abstract void setReturnsManager();
		
		protected float[] getOrderedTickersPearsonCorrelations_setCorrelations_getValue_getFilteredReturnsForFirstTicker(
			float[] firstTickerReturns, float[] secondTickerReturns)
		{
			float[] returnValue;
			float[] tempReturnValue = new float[firstTickerReturns.Length];
			int countSignificantReturns = 0;
			for(int i = 0; i<firstTickerReturns.Length; i++)
				if( Math.Abs(firstTickerReturns[i]) >= this.minimumAbsoluteReturnValue &&
				    Math.Abs(firstTickerReturns[i]) <= this.maximumAbsoluteReturnValue &&
						Math.Abs(secondTickerReturns[i]) >= this.minimumAbsoluteReturnValue && 
						Math.Abs(secondTickerReturns[i]) <= this.maximumAbsoluteReturnValue )
				{
					countSignificantReturns++;
					tempReturnValue[ countSignificantReturns - 1 ] = firstTickerReturns[ i ];
				}
			returnValue = new float[countSignificantReturns];
      Array.Copy(tempReturnValue, 0, returnValue, 0, countSignificantReturns);
			return returnValue;
		}

		protected double getOrderedTickersPearsonCorrelations_setCorrelations_getValue(
			             int indexOfFirstTicker, int indexOfSecondTicker)
		{
			double returnValue;
			float[] firstTickerSignificantReturns = 
				this.getOrderedTickersPearsonCorrelations_setCorrelations_getValue_getFilteredReturnsForFirstTicker(
				this.returnsManager.GetReturns(this.tickers[indexOfFirstTicker]),
				this.returnsManager.GetReturns(this.tickers[indexOfSecondTicker]) );
			
			float[] secondTickerSignificantReturns = 
				this.getOrderedTickersPearsonCorrelations_setCorrelations_getValue_getFilteredReturnsForFirstTicker(
				this.returnsManager.GetReturns(this.tickers[indexOfSecondTicker]),
				this.returnsManager.GetReturns(this.tickers[indexOfFirstTicker]) );
			
			returnValue = BasicFunctions.PearsonCorrelationCoefficient(
				firstTickerSignificantReturns,
				secondTickerSignificantReturns );
			
			if ( Double.IsNaN(returnValue) || Double.IsInfinity(returnValue) )
				throw new Exception( "correlation can't be computed for these two tickers: "+
														this.tickers[indexOfFirstTicker] + " and " +
														this.tickers[indexOfSecondTicker] + "!");

			return returnValue;
		}

		protected void getOrderedTickersPearsonCorrelations_setCorrelations()
		{
			this.pearsonCorrelations = 
				new TickersPearsonCorrelation[ this.numOfCombinationTwoByTwo ];
			int index = 0;
			for (int i = 0; i < this.tickers.Length; i++)
				for (int j = i + 1; j < this.tickers.Length; j++)
				{	
					this.pearsonCorrelations[index] = 
						new TickersPearsonCorrelation( this.tickers[i], this.tickers[j],
						this.getOrderedTickersPearsonCorrelations_setCorrelations_getValue( i , j) );
					index++;
				} 
			Array.Sort(this.pearsonCorrelations);
		}

		public TickersPearsonCorrelation[] GetOrderedTickersPearsonCorrelations()
		{
    	if( this.pearsonCorrelations == null )
				this.getOrderedTickersPearsonCorrelations_setCorrelations();
			
			return this.pearsonCorrelations;
		}
		
		public double GetPearsonCorrelation(string firstTicker, string secondTicker)
		{
    	double returnValue = double.NaN;
			if( this.pearsonCorrelations == null )
				this.getOrderedTickersPearsonCorrelations_setCorrelations();
			
    	for(int i = 0; i<this.pearsonCorrelations.Length; i++)
    	{
    		if( (this.pearsonCorrelations[i].FirstTicker == firstTicker &&
    		     this.pearsonCorrelations[i].SecondTicker == secondTicker) ||
    		     (this.pearsonCorrelations[i].FirstTicker == secondTicker &&
    		      this.pearsonCorrelations[i].SecondTicker == firstTicker) )
    		{
					returnValue = this.pearsonCorrelations[i].CorrelationValue;
					i = this.pearsonCorrelations.Length;//exit from for
				}
    	}
    	if( double.IsNaN(returnValue) )
				throw new MissingCorrelationException(firstTicker, secondTicker);
			
			return returnValue;
		}

  } // end of class
}
