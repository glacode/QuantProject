/*
QuantProject - Quantitative Finance Library

PVOLessVolatilePriceRatioChooser.cs
Copyright (C) 2009
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
using System.Collections;

using QuantProject.ADT;
using QuantProject.ADT.Messaging;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.TickersRelationships; 
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.OutOfSample;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers.PriceRatioChooser
{
	/// <summary>
	/// PVOLessVolatilePriceRatioChooser to be used for
	/// in sample optimization
	/// By means of price ratio analysis, the AnalyzeInSample method returns the 
	/// requested number of PVOPositions (positions for the PVO strategy)
	/// </summary>
	[Serializable]
	public class PVOLessVolatilePriceRatioChooser : IInSampleChooser
	{
		public event NewProgressEventHandler NewProgress;
		public event NewMessageEventHandler NewMessage;
		
		private string[] tickers;
		private PriceRatio[] priceRatios;
		private int numberOfBestTestingPositionsToBeReturned;
		private bool balancedWeightsOnVolatilityBase;
		
		public virtual string Description
		{
			get
			{
				string description = "PVOLessVolatilePriceRatioChooser_" +
														 "NumOfTickersReturned:\n" +
														 this.numberOfBestTestingPositionsToBeReturned.ToString();
				return description;
			}
		}
		
		/// <summary>
		/// PVOLessVolatilePriceRatioChooser to be used for
		/// in sample optimization
		/// </summary>
		/// <param name="numberOfBestTestingPositionsToBeReturned">
		/// The number of PVOPositions that the
		/// AnalyzeInSample method will return
		/// </param>
		public PVOLessVolatilePriceRatioChooser(int numberOfBestTestingPositionsToBeReturned,
															   						bool balancedWeightsOnVolatilityBase)
		{
			this.numberOfBestTestingPositionsToBeReturned = numberOfBestTestingPositionsToBeReturned;
			this.balancedWeightsOnVolatilityBase = balancedWeightsOnVolatilityBase;
		}

		#region AnalyzeInSample
		private void analyzeInSample_checkParameters(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			if ( eligibleTickers.Count <	2 )
				throw new Exception( "Eligible tickers contains " +
					"only " + eligibleTickers.Count +
					" elements, while Price Ratio computation requires at least 2 elements");
		}
		private double setTickersAndPriceRatios_setPriceRatios_getPriceRatioAverage(int indexOfFirstTicker,
		                                             																int indexOfSecondTicker,
		                                             															  DateTime firstDate,
		                                             															  DateTime lastDate)
		{
			double returnValue = 
				PriceRatioProvider.GetPriceRatioAverage(this.tickers[indexOfFirstTicker], this.tickers[indexOfSecondTicker],
				                                        firstDate, lastDate);
			return returnValue;
		}
		private double setTickersAndPriceRatios_setPriceRatios_getPriceRatioAbsoluteStandardDeviation(int indexOfFirstTicker,
		                                             																int indexOfSecondTicker,
		                                             															  DateTime firstDate,
		                                             															  DateTime lastDate)
		{
			double returnValue =  
				PriceRatioProvider.GetPriceRatioStandardDeviation(this.tickers[indexOfFirstTicker], this.tickers[indexOfSecondTicker],
				                                        firstDate, lastDate);
			return returnValue;
		}
		
		private void setTickersAndPriceRatios_setPriceRatios(DateTime firstDate,
		                                             DateTime lastDate)
		{
			int n = this.tickers.Length;
			//combinations without repetitions:
			//n_fatt /( k_fatt * (n-k)_fatt ): when k = 2,
			// it can be reduced to this simple formula:
			int numOfCombinationTwoByTwo = n * (n - 1) / 2;
			this.priceRatios = new PriceRatio[numOfCombinationTwoByTwo];
			int index = 0;
			for (int i = 0; i < this.tickers.Length; i++)
				for (int j = i + 1; j < this.tickers.Length; j++)
			{
				try{
					this.priceRatios[index] =
						new PriceRatio( this.tickers[i], this.tickers[j],
						                this.setTickersAndPriceRatios_setPriceRatios_getPriceRatioAverage( i , j, firstDate, lastDate),
						                this.setTickersAndPriceRatios_setPriceRatios_getPriceRatioAbsoluteStandardDeviation( i , j, firstDate, lastDate) );
				}
				catch(Exception ex)
				{
					string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
				}
				index++;
			}
			Array.Sort(this.priceRatios);
		}
		
		protected void setTickersAndPriceRatios(EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager)
		{
			DateTime firstDate = HistoricalEndOfDayTimer.GetMarketClose(returnsManager.ReturnIntervals[0].Begin);
			DateTime lastDate = HistoricalEndOfDayTimer.GetMarketClose(returnsManager.ReturnIntervals.LastDateTime);
			this.tickers = eligibleTickers.Tickers; 
			this.setTickersAndPriceRatios_setPriceRatios(firstDate, lastDate);
		}
		
		protected PVOPositions getTestingPositions(SignedTickers signedTickers,
																							 ReturnsManager returnsManager)
		{
			WeightedPositions weightedPositions;
			if(this.balancedWeightsOnVolatilityBase == true)
				weightedPositions = 
					new WeightedPositions(WeightedPositions.GetBalancedWeights(signedTickers, returnsManager),
																signedTickers.Tickers);
			else//just equal weights
				weightedPositions = new WeightedPositions(signedTickers);
			
			return new PVOPositions( weightedPositions, 0.0, 0.0, 0 );
		}

		private TestingPositions[] getBestTestingPositionsInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager	)
		{
			this.setTickersAndPriceRatios(eligibleTickers ,	returnsManager);
			TestingPositions[] bestTestingPositions = 
				new TestingPositions[this.numberOfBestTestingPositionsToBeReturned]; 
			int addedTestingPositions = 0;
			int counter = 0;
			while(addedTestingPositions < this.numberOfBestTestingPositionsToBeReturned && 
						counter < priceRatios.Length)
			{
					SignedTickers signedTickers = 
						new SignedTickers("-"+priceRatios[ counter ].FirstTicker + ";" +
															priceRatios[ counter ].SecondTicker);
					bestTestingPositions[addedTestingPositions] = this.getTestingPositions(signedTickers, returnsManager);
					((PVOPositions)bestTestingPositions[addedTestingPositions]).FitnessInSample = 
						priceRatios[ counter ].RelativeStandardDeviation;
					addedTestingPositions++;
					counter++;
			}	
			return bestTestingPositions;
		}

		/// <summary>
		/// Returns the best TestingPositions with respect to the in sample data
		/// </summary>
		/// <param name="eligibleTickers"></param>
		/// <returns></returns>
		public object AnalyzeInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			if ( this.NewMessage != null )
				this.NewMessage( this , new NewMessageEventArgs( "New Price Ratio Analysis" ) );
			if ( this.NewProgress != null )
				this.NewProgress( this , new NewProgressEventArgs( 1 , 1 ) );
			this.analyzeInSample_checkParameters( eligibleTickers ,
				returnsManager );
			TestingPositions[] bestTestingPositionsInSample =
				this.getBestTestingPositionsInSample( eligibleTickers ,
				returnsManager );
			return bestTestingPositionsInSample;
		}
		#endregion 
	}
}
