/*
QuantProject - Quantitative Finance Library

PVOCorrelationChooser.cs
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
using System.Collections;

using QuantProject.ADT;
using QuantProject.ADT.Messaging;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.TickersRelationships; 
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.OutOfSample;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers
{
	/// <summary>
	/// PVOCorrelationChooser to be used for
	/// in sample optimization
	/// By means of correlation, the AnalyzeInSample method returns the 
	/// requested number of PVOPositions (positions for the PVO strategy)
	/// </summary>
	public abstract class PVOCorrelationChooser : IInSampleChooser
	{
		public event NewProgressEventHandler NewProgress;
		public event NewMessageEventHandler NewMessage;
		
		protected CorrelationProvider correlationProvider;
		protected int numberOfBestTestingPositionsToBeReturned;
		protected int numDaysForOscillatingPeriod;
		protected double maxCorrelationValue;
		//correlations greater than this value are discarded
		protected bool balancedWeightsOnVolatilityBase;
		
		public virtual string Description
		{
			get
			{
				string description = "CorrelationChooserType:\n" +
														 this.correlationProvider.GetType().ToString() + "\n" +
														 "NumOfTickersReturned:\n" +
														 this.numberOfBestTestingPositionsToBeReturned.ToString() + 
														 "MaxCorrelationValue: " +
														 this.maxCorrelationValue.ToString();

				return description;
			}
		}
		
		/// <summary>
		/// PVOCorrelationChooser to be used for
		/// in sample optimization
		/// </summary>
		/// <param name="numberOfBestTestingPositionsToBeReturned">
		/// The number of PVOPositions that the
		/// AnalyzeInSample method will return
		/// </param>
		/// <param name="numDaysForOscillatingPeriod">
		/// Interval's length of the return for the PVOPosition
		/// to be checked out of sample, in order to update the 
		/// status for the PVOPosition itself
		/// </param>
		/// <param name="maxCorrelationValue">
		/// Correlations higher than given maxCorrelationValue are discarded
		/// (for avoiding analyzing tickers corresponding to the same stock)
		/// </param>
		public PVOCorrelationChooser(int numberOfBestTestingPositionsToBeReturned,
														     int numDaysForOscillatingPeriod,
																 double maxCorrelationValue,
															   bool balancedWeightsOnVolatilityBase)
		{
			this.numberOfBestTestingPositionsToBeReturned = numberOfBestTestingPositionsToBeReturned;
			this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
			this.maxCorrelationValue = maxCorrelationValue;
			this.balancedWeightsOnVolatilityBase = balancedWeightsOnVolatilityBase;
		}

		#region AnalyzeInSample
		private void analyzeInSample_checkParameters(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			if ( eligibleTickers.Count <	2 )
				throw new Exception( "Eligible tickers for driving positions contains " +
					"only " + eligibleTickers.Count +
					" elements, while NumberOfDrivingPositions is 2");
			if (this.maxCorrelationValue < 0.50 || this.maxCorrelationValue > 1.0 )
				throw new OutOfRangeException( "maxCorrelationValue", 0.5, 1.0);
		}
								
		protected abstract void setCorrelationProvider(EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager);
		
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
			
			return new PVOPositions(weightedPositions, 0.0, 0.0,
			                        this.numDaysForOscillatingPeriod );
		}

		private TestingPositions[] getBestTestingPositionsInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager	)
		{
			this.setCorrelationProvider(eligibleTickers ,	returnsManager);
			TestingPositions[] bestTestingPositions = 
				new TestingPositions[this.numberOfBestTestingPositionsToBeReturned]; 
			TickersPearsonCorrelation[] correlations = 
				this.correlationProvider.GetOrderedTickersPearsonCorrelations();
			int addedTestingPositions = 0;
			int counter = 0;
			while(addedTestingPositions < this.numberOfBestTestingPositionsToBeReturned && 
						counter < correlations.Length)
			{
				if(correlations[correlations.Length - 1 - counter].CorrelationValue < this.maxCorrelationValue)
				{
					SignedTickers signedTickers = 
					new SignedTickers("-"+correlations[correlations.Length - 1 - counter].FirstTicker + ";" +
														correlations[correlations.Length - 1 - counter].SecondTicker);
					bestTestingPositions[addedTestingPositions] = this.getTestingPositions(signedTickers, returnsManager);
					((PVOPositions)bestTestingPositions[addedTestingPositions]).FitnessInSample = 
						correlations[correlations.Length - 1 - counter].CorrelationValue;
					addedTestingPositions++;
				}
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
				this.NewMessage( this , new NewMessageEventArgs( "New Correlation Analysis" ) );
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
