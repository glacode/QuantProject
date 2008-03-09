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
	public class PVOCorrelationChooser : IInSampleChooser
	{
		public event NewProgressEventHandler NewProgress;
		public event NewMessageEventHandler NewMessage;
		
		protected CorrelationProvider correlationProvider;
		protected int numberOfBestTestingPositionsToBeReturned;
		protected double oversoldThreshold;
		protected double overboughtThreshold;
		protected int numDaysForOscillatingPeriod;
		
		public virtual string Description
		{
			get
			{
				string description = "PVOCorrelationChooser_" +
														 this.correlationProvider.GetType().ToString();
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
		/// /// <param name="numDaysForOscillatingPeriod">
		/// Interval's length of the return for the PVOPosition
		/// to be checked out of sample, in order to update the 
		/// status for the PVOPosition itself
		/// </param>
		public PVOCorrelationChooser(int numberOfBestTestingPositionsToBeReturned,
			double oversoldThreshold, double overboughtThreshold,
			int numDaysForOscillatingPeriod)
		{
			this.numberOfBestTestingPositionsToBeReturned = numberOfBestTestingPositionsToBeReturned;
			this.oversoldThreshold = oversoldThreshold;
			this.overboughtThreshold = overboughtThreshold;
			this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
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
		}
								
		protected virtual void setCorrelationProvider(EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager)
		{
			this.correlationProvider = 
				new OpenToCloseCorrelationProvider(eligibleTickers.Tickers, returnsManager,
																					 0.0001f, 0.5f);
		}
		
		protected virtual PVOPositions getTestingPositions(WeightedPositions weightedPositions)
		{
			return new PVOPositions(weightedPositions, this.oversoldThreshold, this.overboughtThreshold, this.numDaysForOscillatingPeriod );
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
			for(int i = 0; i<this.numberOfBestTestingPositionsToBeReturned; i++)
			{
				SignedTickers signedTickers = 
					new SignedTickers("-"+correlations[correlations.Length - 1 -i].FirstTicker + ";" +
														correlations[correlations.Length - 1 -i].SecondTicker);
				WeightedPositions weightedPositions = new WeightedPositions(signedTickers);
				bestTestingPositions[i] = this.getTestingPositions(weightedPositions);
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
