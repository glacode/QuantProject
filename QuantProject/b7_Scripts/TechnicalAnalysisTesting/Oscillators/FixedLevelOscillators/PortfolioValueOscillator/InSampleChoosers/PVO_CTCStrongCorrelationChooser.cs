/*
QuantProject - Quantitative Finance Library

PVO_CTCStrongCorrelationChooser.cs
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
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.OutOfSample;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers
{
	/// <summary>
	/// PVO_CTCStrongCorrelationChooser to be used for
	/// in sample optimization
	/// By means of correlation, the AnalyzeInSample method returns the 
	/// requested number of PVOPositions (positions for the PVO strategy)
	/// </summary>
	public class PVO_CTCStrongCorrelationChooser : IInSampleChooser
	{
		public event NewProgressEventHandler NewProgress;
		public event NewMessageEventHandler NewMessage;
		
		protected int numberOfBestTestingPositionsToBeReturned;
		protected int numDaysForOscillatingPeriod;
		protected double maxCorrelationValue;
		//correlations greater than this value are discarded
		private float minimumAbsoluteReturnValue;
		private float maximumAbsoluteReturnValue;
		//tickers'returns out of these limits are discarded
		protected bool balancedWeightsOnVolatilityBase;
		protected IntervalsType intervalsType;
		protected string benchmark;
		
		public virtual string Description
		{
			get
			{
				string description = "StrongCTCCrrltChsr";
				return description;
			}
		}
		
		public IntervalsType IntervalsType
		{
			get
			{
				return this.intervalsType;
			}
		}

		public int ReturnIntervalLength
		{
			get
			{
				return this.numDaysForOscillatingPeriod;
			}
		}
		
		/// <summary>
		/// PVO_CTCStrongCorrelationChooser to be used for
		/// in sample optimization
		/// In this implementation, PVOPositions with the highest
		/// strong correlation are returned.
		/// Strong correlation is defined as the average
		/// CTC correlation at 1, 2 and 3 days. 
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
		public PVO_CTCStrongCorrelationChooser(int numberOfBestTestingPositionsToBeReturned,
														     double maxCorrelationValue,
														     float minimumAbsoluteReturnValue,
																 float maximumAbsoluteReturnValue,
															   bool balancedWeightsOnVolatilityBase,
																 string benchmark)
		{
			this.numberOfBestTestingPositionsToBeReturned = numberOfBestTestingPositionsToBeReturned;
			this.numDaysForOscillatingPeriod = 1;
			this.maxCorrelationValue = maxCorrelationValue;
			this.minimumAbsoluteReturnValue = minimumAbsoluteReturnValue;
			this.maximumAbsoluteReturnValue = maximumAbsoluteReturnValue;
			this.balancedWeightsOnVolatilityBase = balancedWeightsOnVolatilityBase;
			this.intervalsType = IntervalsType.CloseToCloseIntervals;
			this.benchmark = benchmark;
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
			if (this.maxCorrelationValue < 0.0 || this.maxCorrelationValue > 1.0 )
				throw new OutOfRangeException( "maxCorrelationValue", 0.0, 1.0);
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
			
			return new PVOPositions(weightedPositions, 0.0, 0.0,
			                        this.numDaysForOscillatingPeriod );
		}

		private TickersPearsonCorrelation[] getBestTestingPositionsInSample_getAverageCorrelationsAt1_2_3_Days(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager	)
		{
			TickersPearsonCorrelation[] averageCorrelations;
			TickersPearsonCorrelation[] correlations_1Day;
			DateTime firstDate = returnsManager.ReturnIntervals[0].Begin;
			DateTime lastDate =  returnsManager.ReturnIntervals.LastDateTime;
			CloseToCloseCorrelationProvider correlationProviderCTC_1 =
				new CloseToCloseCorrelationProvider(eligibleTickers.Tickers, firstDate,
				                                    lastDate, 1 ,
				                                    this.minimumAbsoluteReturnValue,
				                                    this.maximumAbsoluteReturnValue,
				                                    this.benchmark);
			CloseToCloseCorrelationProvider correlationProviderCTC_2 =
				new CloseToCloseCorrelationProvider(eligibleTickers.Tickers, firstDate,
				                                    lastDate, 2 ,
				                                    this.minimumAbsoluteReturnValue,
				                                    this.maximumAbsoluteReturnValue,
				                                    this.benchmark);
			CloseToCloseCorrelationProvider correlationProviderCTC_3 =
				new CloseToCloseCorrelationProvider(eligibleTickers.Tickers, firstDate,
				                                    lastDate, 3 ,
				                                    this.minimumAbsoluteReturnValue,
				                                    this.maximumAbsoluteReturnValue,
				                                    this.benchmark);
			correlations_1Day = correlationProviderCTC_1.GetOrderedTickersPearsonCorrelations();
			averageCorrelations = correlations_1Day;
			for(int i = 0; i<averageCorrelations.Length; i++)
			{
				averageCorrelations[i].CorrelationValue = 
					( averageCorrelations[i].CorrelationValue + 
					  correlationProviderCTC_2.GetPearsonCorrelation(
					 	averageCorrelations[i].FirstTicker,averageCorrelations[i].SecondTicker) +
						correlationProviderCTC_3.GetPearsonCorrelation(
					 	averageCorrelations[i].FirstTicker,averageCorrelations[i].SecondTicker) ) / 3.0;
			}
			Array.Sort(averageCorrelations);
			return averageCorrelations;
		}
		
		private TestingPositions[] getBestTestingPositionsInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager	)
		{
			//this.setCorrelationProvider(eligibleTickers ,	returnsManager);
			TestingPositions[] bestTestingPositions = 
				new TestingPositions[this.numberOfBestTestingPositionsToBeReturned]; 
							
			TickersPearsonCorrelation[] averageCorrelations =
				this.getBestTestingPositionsInSample_getAverageCorrelationsAt1_2_3_Days(
								eligibleTickers , returnsManager);
			int addedTestingPositions = 0;
			int counter = 0;
			while(addedTestingPositions < this.numberOfBestTestingPositionsToBeReturned && 
						counter < averageCorrelations.Length)
			{
				if(averageCorrelations[averageCorrelations.Length - 1 - counter].CorrelationValue < this.maxCorrelationValue)
				{
					SignedTickers signedTickers = 
					new SignedTickers("-"+averageCorrelations[averageCorrelations.Length - 1 - counter].FirstTicker + ";" +
														averageCorrelations[averageCorrelations.Length - 1 - counter].SecondTicker);
					bestTestingPositions[addedTestingPositions] = this.getTestingPositions(signedTickers, returnsManager);
					((PVOPositions)bestTestingPositions[addedTestingPositions]).FitnessInSample = 
						averageCorrelations[averageCorrelations.Length - 1 - counter].CorrelationValue;
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
